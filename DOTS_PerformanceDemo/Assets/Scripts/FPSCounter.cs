using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private float intervall = 1f;
    private float nextValueTime;

    private List<int> fpsList;
    bool addToList = true;

    private void Start()
    {
        fpsList = new List<int>();
        StartCoroutine(ExecuteAfterTime(20));
    }
    
    void Update()
    {
        if (Time.unscaledTime > nextValueTime)
        {
            int fps = (int)(1f / Time.unscaledDeltaTime);
            if (addToList)
                fpsList.Add(fps);
            nextValueTime = Time.unscaledTime + intervall;
        }
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        addToList = false;
        fpsList.Remove(0);
        string allValues = string.Join(",", fpsList.ToArray());
        Debug.Log("Values (" + fpsList.Count + "): " + allValues);
        Debug.Log("Highest: " + fpsList.Max());
        Debug.Log("Lowest: " + fpsList.Min());
        Debug.Log("Average: " + fpsList.Average());
    }
}
