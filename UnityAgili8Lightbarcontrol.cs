using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class UnityAgili8Lightbarcontrol: MonoBehaviour
{
    
    AndroidJavaClass unityPlayer=null;
    AndroidJavaObject activity=null;
    private AndroidJavaObject _lightbarPlugin = null;
    AndroidJavaClass lightbarPluginClass;
    AndroidJavaObject instance { get { return lightbarPluginClass.GetStatic<AndroidJavaObject>("instance"); } }
    private Thread lightbarThread;

    void Start()
    {
        try
        {
            lightbarPluginClass = new AndroidJavaClass("com.agili8.plugins.LightBarPlugin");
            lightbarPluginClass.CallStatic("start");
            /*
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (unityPlayer != null)
            {
                activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                lightbarPluginClass = new AndroidJavaClass("com.llvision.arsdk.ARActivity");
                if (lightbarPluginClass != null)
                        Debug.Log("LightBar - Get Instance");
                        _lightbarPlugin = lightbarPluginClass.CallStatic<AndroidJavaObject>("getInstance", activity);
            }
            */
        }
        catch (Exception e)
        {
            Debug.Log("Exception writing to lcd from unity:" + e.Message);
        }
        Debug.Log("LightBar - Turn lights off");
        DirectLightBarOff();
        Debug.Log("LightBar - Set Colour Black");
        DirectSetLightBarColour("0,0,0,0,0");
        lightbarThread = new Thread(ThreadExecution) { IsBackground = true, Priority = System.Threading.ThreadPriority.Normal, Name = "lightbarThread" };
        lightbarThread.Start();
    }
	
	private bool running = true;
	void OnApplicationQuit()
    {
        running = false;
    }

	void Update()
    {
    }
	
    private string lastLine1 = null;   
    private void ThreadExecution()
    {
        AndroidJNI.AttachCurrentThread();
        while (running)
        {
            if (LightbarLine1 != null && (!LightbarLine1.Equals(lastLine1)))
            {
                DirectSetLightBarColour(LightbarLine1);
                lastLine1 = LightbarLine1;
            }
            Thread.Sleep(1000);
        }
    }

    private string LightbarLine1=null;
    public void SetColour(string line1)
    {
        LightbarLine1 = line1;
        
    }

    private int MAX_LINE_LENGTH = 18; //16x2 LCD
    private string PrepareColourLine(string line)
    {
        if (line != null)
        { 
            if (line.Length > MAX_LINE_LENGTH)
            {
                line = line.Substring(0, 18);
            }
            else if (line.Length < MAX_LINE_LENGTH)
            {
                line = line.PadRight(MAX_LINE_LENGTH - line.Length);
            }
        }
        else
        {
            line = "".PadRight(16);
        }
        return line;
    }
    public void DirectSetLightBarColour(string line1)
    {
        
        try
        {
            if (unityPlayer != null)
            {
                if (lightbarPluginClass != null)
                {
                    Debug.Log("SetLightBarColour:" + line1);
                    lightbarPluginClass.Call("SetLightBarColour", line1);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception setting Lightbar Colour from unity:" + e.Message);
        }
        
    }

    private void DirectLightBarOff()
    {
        try
        {
            if (unityPlayer != null)
            {
                if (lightbarPluginClass != null)
                {
                    lightbarPluginClass.Call("LightBarOff");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception turning LightBar Off from unity:" + e.Message);
        }
    }
}
