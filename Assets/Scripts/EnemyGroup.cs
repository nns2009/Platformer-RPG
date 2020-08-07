using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    private GameObject initialState;
    private GameObject currentState;

    void Start()
    {
        var children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }
        initialState = new GameObject("Initial State");
        foreach (var child in children)
        {
            child.parent = initialState.transform;
        }
        initialState.transform.parent = transform;
        initialState.SetActive(false);

        Reset();
    }

    public void CheckpointReset()
    {
        bool fullyDestroyed = currentState.transform.childCount == 0;

        if (!fullyDestroyed)
        {
            Destroy(currentState);
            Reset();
        }
    }

    public void Reset()
    {
        currentState = Instantiate(initialState, transform);
        currentState.name = "Current State";
        currentState.SetActive(true);
    }
}
