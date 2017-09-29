using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client Instance = null;

    public string clientName;
    public bool isWhite;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;

    public List<GameClient> players = new List<GameClient>();

    public bool sentMove = false;  

    public void Start()
    {
        if (Instance != null)
            Destroy(gameObject);
        else
            Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (socketReady && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            if (data != null)
                OnIncomingData(data);
        }

    }

    public bool ConnectToServer(string host, int port)
    {
        if (socketReady)
            return false;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            socketReady = true;
        }
        catch(Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }

        return socketReady;
    }
    

    // Sending messages to the server
    public void Send(string data)
    {
        if (socketReady == false)
        {
            Debug.LogError("Send error: tried to send data when the socket was not ready. Probably not connected to server.");
            return;
        }

        writer.WriteLine(data);
        writer.Flush();
    }

    // Read Messgages form server
    private void OnIncomingData(string data)
    {
        string[] aData = data.Split('|');
        Debug.Log(data);

        switch(aData[0])
        {
            case "SWHO":
                for (int i = 1; i < aData.Length - 1; i++)
                    UserConnected(aData[i], false);
                Send("CWHO|" + clientName);
                break;
            case "SCNN":
                UserConnected(aData[1], aData[2] == "1");
                break;
            case "SMOV":
                if (sentMove)
                    sentMove = false;
                else
                    BoardManager.Instance.TryMove(aData[1]);
                break;
        }
    }

    private void UserConnected(string name, bool isHost)
    {
        GameClient c = new GameClient()
        {
            name = name,
            isHost = isHost
        };
        players.Add(c);

        if(players.Count == 2)
        {
            GameManager.Instance.StartGame();
        }
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }
    private void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        stream.Close();
        socket.Close();
        socketReady = false;

    }
}

public class GameClient
{
    public string name;
    public bool isHost;
}
