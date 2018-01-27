using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;
using System.Collections.Generic;

namespace Prototype.NetworkLobby
{
    public class LobbyServerList : MonoBehaviour
    {
        private LobbyManager mLobbyManager;

        public RectTransform serverListRect;
        public GameObject serverEntryPrefab;
        public GameObject noServerFound;
		public CustomNetworkDiscovery networkDiscovery;

        protected int currentPage = 0;
        protected int previousPage = 0;

        static Color OddServerColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        static Color EvenServerColor = new Color(.94f, .94f, .94f, 1.0f);

        void OnEnable()
        {
            currentPage = 0;
            previousPage = 0;


            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);

            noServerFound.SetActive(false);

            mLobbyManager = LobbyManager.s_Singleton;
            RequestPage(0);
			StartCoroutine(RefreshAfter (1));
        }

        public void OnClickJoin()
        {
            mLobbyManager.StopMatchMaker();
            mLobbyManager.isHost = false;

            mLobbyManager.StartClient();

            mLobbyManager.backDelegate = mLobbyManager.StopClientClbk;
            mLobbyManager.DisplayIsConnecting();
			mLobbyManager.networkAddress = mLobbyManager.mainMenu.ipInput.text;

            mLobbyManager.SetServerInfo("Connecting...", mLobbyManager.networkAddress);
			mLobbyManager.StartClient ();
        }

        public void OnGUIMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches)
		{
            if (matches.Count == 0 && networkDiscovery.localMatches.Count == 0)
			{
                if (currentPage == 0)
                {
                    noServerFound.SetActive(true);
                }

                currentPage = previousPage;
               
                return;
            }

            noServerFound.SetActive(false);
            foreach (Transform t in serverListRect)
                Destroy(t.gameObject);

			for (int i = 0; i < matches.Count; ++i)
			{
                GameObject o = Instantiate(serverEntryPrefab) as GameObject;

				o.GetComponent<LobbyServerEntry>().Populate(matches[i], mLobbyManager, (i % 2 == 0) ? OddServerColor : EvenServerColor);

				o.transform.SetParent(serverListRect, false);
            }
          
            foreach (LobbyServerEntry entry in networkDiscovery.localMatches)
			{
				entry.transform.SetParent(serverListRect, false);
			}
        }

		private IEnumerator RefreshAfter(float seconds)
		{
			yield return new WaitForSeconds (seconds);
			RequestPage (0);
			RefreshAfter (5);
		}

		void OnDisable()
		{
			StopAllCoroutines ();
		}

        public void ChangePage(int dir)
        {
            int newPage = Mathf.Max(0, currentPage + dir);

            //if we have no server currently displayed, need we need to refresh page0 first instead of trying to fetch any other page
            if (noServerFound.activeSelf)
                newPage = 0;

            RequestPage(newPage);
        }

        public void RequestPage(int page)
        {
            previousPage = currentPage;
            currentPage = page;
            networkDiscovery.UpdateLocalMatches();
            mLobbyManager.matchMaker.ListMatches(page, 6, "", true, 0, 0, OnGUIMatchList);
		}
    }
}