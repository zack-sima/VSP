
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class WebGLWebsocket : MonoBehaviour {
    public NetworkManager networkMaster;

#if !UNITY_EDITOR && UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void WebSocketInit(string uri);

    [DllImport("__Internal")]
    private static extern void WebSocketSend(string message);

    [DllImport("__Internal")]
    private static extern void WebSocketClose();

    public void BeginWebsocket(string uri) {
        WebSocketInit(uri);
    }
    public void CloseWebsocket() {
        WebSocketClose();
    }
    public void ReceivedWebsocket(string message) {
        networkMaster.ReceivedWebsocket(message);
    }
    public void OnConnected() {
        StartCoroutine(TimedRetriver());
    }
    IEnumerator TimedRetriver() {
        while (true) {
            for (float i = 0f; i < networkMaster.callTime; i += Time.deltaTime)
                yield return null;

            try {
                WebSocketSend(networkMaster.GetSendData());
            } catch {
                Debug.Log("network error");
            }
        }
    }
#endif
}

