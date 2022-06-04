using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

public class PlayerInfo {
    public int pid;
    public float position_x, position_y, position_z, rotation_y;
    public List<string> new_messages;
    public PlayerInfo(int pid, Transform player, List<string> new_messages) {
        this.pid = pid;
        position_x = player.position.x;
        position_y = player.position.y;
        position_z = player.position.z;
        rotation_y = player.eulerAngles.y;
        this.new_messages = new_messages;
    }
}
//converted from python
public class Player {
    public List<float> position;
    public List<float> rotation;
    public string player_name;
    public int player_id;
}
public class ClientInfo {
    public Dictionary<string, Player> players;
    public List<string> recent_messages;
}

public class NetworkManager : MonoBehaviour {
    public GameObject playerPrefab;
    private Dictionary<string, GameObject> players;

    public Transform player;
    public NativeWebsocket nativeWeb;
    public WebGLWebsocket webGLWebsocket;

    Uri u;

    //time between calls
    public float callTime = 0.05f;

    private int playerId = -1;

    private readonly bool localNetworking = false;

    void InitializeURI() {
        if (localNetworking) {
            u = new Uri("ws://localhost:8000/server/my_name");
        } else {
            u = new Uri("ws://47.88.27.128:8000/server/" + (PlayerPrefs.GetString("playerName") != "" ? PlayerPrefs.GetString("playerName") : "Player"));
            //u = new Uri("ws://retrocombat.com/server/my_name");
        }
    }
    private void OpenWebSockets() {
#if UNITY_WEBGL && !UNITY_EDITOR
        webGLWebsocket.BeginWebsocket(u.ToString());
#else
        nativeWeb.EstablishWebsocket(u);
#endif
    }
    public void CloseWebsockets() {
#if UNITY_WEBGL && !UNITY_EDITOR
        webGLWebsocket.CloseWebsocket();
#else
        nativeWeb.CloseWebSocket();
#endif
        foreach (GameObject g in players.Values) Destroy(g);
    }

    public void ReceivedWebsocket(string message) {
        if (playerId == -1) {
            //parse player id
            if (!int.TryParse(message, out playerId)) {
                Debug.LogWarning("id not found");
            } else {
                print("id assigned: " + playerId);
            }
        } else {
            //parse for json
            print("original: " + message);
            ClientInfo c = (ClientInfo)MyJsonUtility.FromJson(typeof(ClientInfo), message);

            //make a deepcopy of the original dictionary
            foreach (KeyValuePair<string, GameObject> kv in new Dictionary<string, GameObject>(players)) {
                if (!c.players.ContainsKey(kv.Key)) {
                    //remove player
                    Destroy(kv.Value);
                    players.Remove(kv.Key);
                }
            }
            //spawn players if doesn't exist; update positions
            foreach (KeyValuePair<string, Player> p in c.players) {
                //skip if it is local player
                if (p.Key == playerId.ToString()) continue;

                if (!players.ContainsKey(p.Key)) {
                    //spawn in players
                    GameObject insItem = Instantiate(playerPrefab);
                    insItem.GetComponent<OtherPlayer>().SetName(p.Value.player_name);
                    players.Add(p.Key, insItem);
                }
                
                //position
                players[p.Key].transform.position = new Vector3(p.Value.position[0], p.Value.position[1], p.Value.position[2]);
            }

            //control other players (temporary)
            
            print("new: " + MyJsonUtility.ToJson(typeof(ClientInfo), c));
        }
    }
    //called by both compiled implementation and webgl implementation
    public string GetSendData() {
        if (playerId == -1) {
            print("requesting id");
            return "request_id";
        }

        print(MyJsonUtility.ToJson(typeof(PlayerInfo), new PlayerInfo(playerId, player, null)));
        return MyJsonUtility.ToJson(typeof(PlayerInfo), new PlayerInfo(playerId, player, null));
    }

    void Start() {
        players = new Dictionary<string, GameObject>();

        InitializeURI();
        OpenWebSockets();
    }

    void Update() {

    }
}
