using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPoints : MonoBehaviour {

    public Transform GetStartPosition(int index)
    {
        return transform.GetChild(index);
    }

    public List<Transform> GetRandomStartPositions()
    {
        List<Transform> startPositions = new List<Transform>();
        for(int i = 0; i < transform.childCount; i++)
        {
            startPositions.Add(transform.GetChild(i));
        }
        startPositions.Shuffle();
        return startPositions;
    }
}
