using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeAdjuster : MonoBehaviour {
    void Start() {
        Screen.SetResolution(2048, 1152, FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
    }
    void Update() {
        if(Input.GetKey(KeyCode.LeftControl)) {
            if(Input.GetKeyDown(KeyCode.Alpha1)) {
                Screen.fullScreen = !Screen.fullScreen;
            }
            if(Input.GetKeyDown(KeyCode.Alpha2)) {
                Screen.SetResolution(512, 288, FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha3)) {
                Screen.SetResolution(1024, 576, FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha4)) {
                Screen.SetResolution(1536, 864, FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha5)) {
                Screen.SetResolution(2048, 1152, FullScreenMode.FullScreenWindow, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha6)) {
                Screen.SetResolution(1024, 576, FullScreenMode.Windowed, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha7)) {
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha8)) {
                Screen.SetResolution(1366, 768, FullScreenMode.Windowed, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha9)) {
                Screen.SetResolution(1600, 900, FullScreenMode.Windowed, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha0)) {
                Screen.SetResolution(1920, 1080, FullScreenMode.Windowed, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha9)) {
                //Screen.SetResolution(2560, 1440, FullScreenMode.Windowed, Screen.currentResolution.refreshRate);
            }
            if(Input.GetKeyDown(KeyCode.Alpha0)) {
                //Screen.SetResolution(3200, 1800, FullScreenMode.Windowed, Screen.currentResolution.refreshRate);
            }
        }
    }
}
