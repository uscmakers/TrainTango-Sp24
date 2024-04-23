using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    private WebSocket websocket = null;
    private Vector2Int dir = Vector2Int.zero;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        ConnectToGameServer();
    }

    public async void ConnectToGameServer()
    {
        string url = "ws://localhost:8765";
        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connected to " + url);
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            if (!message.Contains(',')) return;

            try
            {
                // parse 2 numbers from the message
                string[] numbers = message.Split(',');
                Vector2Int newDir = new Vector2Int(int.Parse(numbers[0]), int.Parse(numbers[1]));
                if (newDir.x != dir.x)
                {
                    if (newDir.x == 1)
                        PlayerMovement.instance.MoveRight();
                    else if (newDir.x == -1)
                        PlayerMovement.instance.MoveLeft();
                    else
                        PlayerMovement.instance.MoveCenter();
                }
                if (newDir.y != dir.y)
                {
                    if (newDir.y == 1)
                        PlayerMovement.instance.Jump();
                }

                dir = newDir;
            }
            catch (System.Exception e)
            {
                Debug.Log("Error! " + e);
            }
        };

        // waiting for messages
        await websocket.Connect();
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (websocket != null)
            websocket.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
            await websocket.Close();
    }
}
