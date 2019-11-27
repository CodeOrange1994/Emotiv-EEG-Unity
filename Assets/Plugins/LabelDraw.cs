using UnityEngine;
using System.Collections;
using System;

public class LabelDraw : MonoBehaviour {

    public String str_Information;

    public bool enable_LabelInformation;
    public GUISkin skin;
    float  time_count;
    public bool isLoop = false;
    public float time;
    public float timeDelay = 5.0f;


	// Use this for initialization
	void Start () 
    {
		
	}
	
	// Update is called once per frame
	void Update () 
    {
        time = Time.time;
	}

    void OnGUI ()
    {
        if (enable_LabelInformation)
        {
            if (!isLoop)
            {
                GUI.Label(new Rect(Screen.width - str_Information.Length * 10, Screen.height - 40, Screen.width, Screen.height), str_Information,skin.label);
                if ((Time.time - time_count) > timeDelay)
                {
                    enable_LabelInformation = false;
                }
            }
            else
            {             
                if((Time.realtimeSinceStartup - time_count)%2 > 0.5)
                {
                    GUI.Label(new Rect(Screen.width - str_Information.Length * 10, Screen.height - 40, Screen.width, Screen.height), str_Information, skin.label);
                }
            }
        }
    }

    public void DrawLabel (String message,bool loop)
    {
        str_Information = message;
        isLoop = loop;
        enable_LabelInformation = true;
        time_count = Time.realtimeSinceStartup;   
    }

    public void DrawLabel(String message, float delay)
    {
        str_Information = message;
        isLoop = false;
        enable_LabelInformation = true;
        time_count = Time.realtimeSinceStartup;
        timeDelay = delay;
    }

    public void Stop()
    {
        enable_LabelInformation = false;
        isLoop = false;
    }
}
