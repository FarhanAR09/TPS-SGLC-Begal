using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataOutputter : MonoBehaviour
{
    [SerializeField]
    private bool begalEnabled = true;

    private List<float> times = new();

    private void OnEnable()
    {
        GameEvents.OnStudentReachClass.Add(TrackTimeTaken);
    }

    private void OnDisable()
    {
        GameEvents.OnStudentReachClass.Remove(TrackTimeTaken);
    }

    private void OnDestroy()
    {
        TextWriter tw = new StreamWriter(begalEnabled ? "D:/SimulationDataBegal.csv" : "D:/SimulationDataNoBegal.csv", false);
        foreach (float time in times)
        {
            if (time > 0)
                tw.WriteLine(Mathf.RoundToInt(time));
        }
        tw.Close();
    }

    private void TrackTimeTaken(float timeTaken)
    {
        times.Add(timeTaken);
    }
}

public partial class GameEvents
{
    public static GameEvent<float> OnStudentReachClass = new();
}
