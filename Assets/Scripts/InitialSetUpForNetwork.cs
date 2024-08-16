using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialSetUpForNetwork : MonoBehaviour
{
    private void Start()
    {
        // Figure out how to switch between different platforms. Don't forget to consider Input Systems.

        if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            Debug.Log("The application is on Windows");

            // If application is on Windows, then disable OVR Rig

        }
        else
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Meta_Link_Quest_3 || OVRPlugin.GetSystemHeadsetType() == OVRPlugin.SystemHeadset.Meta_Quest_3)
                {
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 72;
                    Debug.Log("The application is on Oculus Quest");

                    // If appllication is on Oculus Quest, then enable OVR Rig

                }
                else
                {
                    QualitySettings.vSyncCount = 0;
                    Application.targetFrameRate = 60;
                    Debug.Log("The application is on Android");

                    // Application should not be on Android, but if it is, then disable OVR Rig
                }
            }
        }
    }
}
