using System.Collections;

using UnityEngine;

public class MobileCheck : MonoBehaviour
{
    public delegate void MobileAction();
    public static event MobileAction CheckForMobile;
    public static event MobileAction CheckForDesktop;

    [SerializeField] private float timeToWait = 1f;

    public static void PerformMobileCheck() {
        if (CheckForMobile != null) {
            CheckForMobile();
        }
    }
    public static void PerformDesktopCheck() {
        if (CheckForDesktop != null) {
            CheckForDesktop();
        }
    }
    

    /*void Update() {
        StartCoroutine(SwitchToMobile());
        StartCoroutine(SwitchToDesktop());
        this.enabled = false;
    }*/


    public static void SwitchPlatforms() {
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            Debug.Log("<color=lime>Switching to Mobile!</color>");
            PerformMobileCheck();
        }

        #if UNITY_EDITOR                                                            // Had to separate these statements because of the UNITY_EDITOR check
            if (UnityEditor.EditorApplication.isRemoteConnected) {
                Debug.Log("<color=lime>Switching to Mobile!</color>");
                PerformMobileCheck();
            }

            if (SystemInfo.deviceType == DeviceType.Desktop && !UnityEditor.EditorApplication.isRemoteConnected) {
                Debug.Log("<color=lime>Switching to Desktop!</color>");
                PerformDesktopCheck();
            }
        #endif
    }
}
