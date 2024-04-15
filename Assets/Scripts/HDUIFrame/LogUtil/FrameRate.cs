using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    private float updateInterval = 0.5f;    //更新间隔
    private float timeleft;
    private float accum = 0.0f;             //累计
    private float frames = 0;         
    private string showFrame;
    private int fpsValue;

    void Start()
    {
        timeleft = updateInterval;
    }

    void Update()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        if (timeleft <= 0.0)
        {
            fpsValue = ((int)(accum / frames));
            showFrame = "FPS: " + fpsValue.ToString();
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
    }

    void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        if(fpsValue < 60)
        {
            bb.normal.textColor = Color.red;
        }
        else
        {
            bb.normal.textColor = Color.green;
        }

        bb.fontSize = 40;
        GUI.Label(new Rect(0, 0, 200, 200), showFrame, bb);
    }
}
