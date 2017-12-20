using UnityEngine;
using UnityEngine.Networking;

public class RockProjectileCollisionScript : NetworkBehaviour
{
    private float mTimeCount;
    [SerializeField] private float mLifeTime;

    void OnEnable()
    {
        mTimeCount = 0;
    }

    void Update()
    {
        if ((mTimeCount += Time.deltaTime) >= mLifeTime)
        {
            gameObject.SetActive(false);
            NetworkServer.UnSpawn(gameObject);
        }
    }
}
