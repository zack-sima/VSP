﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Threading;
using System.Net.WebSockets;

//this is the standalone implementation of networking (disabled on Html5 builds)
public class NativeWebsocket : MonoBehaviour {
    public NetworkManager networkMaster;
    Uri u;
    ClientWebSocket cws = null;
    ArraySegment<byte> buf = new ArraySegment<byte>(new byte[1024]);
    public void EstablishWebsocket(Uri uri) {
        u = uri;
        Connect();
    }

    async void Connect() {
        cws = new ClientWebSocket();
        try {
            await cws.ConnectAsync(u, CancellationToken.None);
            if (cws.State == WebSocketState.Open) Debug.Log("connected");
            StartCoroutine(TimedRetriver());
        } catch (Exception e) { Debug.Log("woe " + e.Message); }
    }
    public void CloseWebSocket() {
        try {
            cws.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
        } catch {
            //cannot close
        }
    }
    bool sendThreadWaiting = false;
    async void SaySomething(string message) {
        sendThreadWaiting = true;
        ArraySegment<byte> b = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        await cws.SendAsync(b, WebSocketMessageType.Text, true, CancellationToken.None);
        sendThreadWaiting = false;
        GetStuff();
    }
    IEnumerator TimedRetriver() {
        while (true) {
            for (float i = 0f; i < networkMaster.callTime; i += Time.deltaTime)
                yield return null;
            if (!threadWaiting && !sendThreadWaiting) {
                SaySomething(networkMaster.GetSendData());
            } else {
                //print("thread not finished");
            }
        }
    }
    bool threadWaiting = false;
    async void GetStuff() {
        threadWaiting = true;
        WebSocketReceiveResult r = await cws.ReceiveAsync(buf, CancellationToken.None);
        threadWaiting = false;
        string message = Encoding.UTF8.GetString(buf.Array, 0, r.Count);
        networkMaster.ReceivedWebsocket(message);
    }
}



