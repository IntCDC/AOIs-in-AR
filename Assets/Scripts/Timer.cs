using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    Text startStop;
    float Timer_time = 0;
    bool TimerStarted = false;
    public GameObject frameManager;
    FrameManager_withoutLayout frame_mnger_Script;
    DateTime t1;
    DateTime t2;
    float t12;


    void Start()
    {
        frame_mnger_Script = frameManager.GetComponent<FrameManager_withoutLayout>();
        startStop = gameObject.GetComponentInChildren<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (TimerStarted)
            Timer_time = Time.deltaTime + Timer_time;


    }

    private void OnApplicationQuit()
    {
        if (TimerStarted)
        {
            startTimer();
        }
    }

    public void startTimer()
    {
        TimerStarted = !TimerStarted;
        if (TimerStarted)
        {
            t1 = DateTime.Now;
            Timer_time = 0;
            startStop.text = "Stop";
        }
        if (!TimerStarted)
        {
            t2 = DateTime.Now;
            t12 = (float)((t2 - t1).TotalMilliseconds);
            WriteTime(t12);
            frame_mnger_Script.saveToJson();
            startStop.text = "Start";
        }
    }

    public void WriteTime(float time)
    {
        string path = Application.streamingAssetsPath + "/timer.txt";
        var t = TimeSpan.FromMilliseconds(time);
        string time_ = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                        t.Hours,
                        t.Minutes,
                        t.Seconds,
                        t.Milliseconds);
        //Write some text to the file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(time_);
        writer.Close();
        StreamReader reader = new StreamReader(path);
        reader.Close();
    }
    public void ReadString()
    {
        string path = Application.streamingAssetsPath + "/timer.txt";
        //Read the text from directly from the file
        StreamReader reader = new StreamReader(path);
        reader.Close();
    }
}
