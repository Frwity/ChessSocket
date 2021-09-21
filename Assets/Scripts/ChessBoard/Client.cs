using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class Client : MonoBehaviour
{
    Socket socket;
    private IPAddress networkIP;
    bool isReceiving = false;
    public bool Connected { get { return socket.Connected; } }

    private void Awake()
    {
        if (RegisterNetworkIP())
        {
            socket = new Socket(networkIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Debug.Log("Client Creation");
        }
        else
        {
            Debug.Log("[CLIENT] Could not find this machine's IP LAN address. Are you connected to a network?");
            Disconnect();
        }
    }

    public bool RegisterNetworkIP()
    {
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

        // Look for the network IP of this machine
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                networkIP = ip;
                return true;
            }
        }

        return false;
    }

    void Start()
    {
        StartConnect("10.2.102.167", 11000);
    }

    void Update()
    {
        if (Connected && !isReceiving)
            StartReceiveMessage();

        if (Input.GetKeyDown(KeyCode.F))
            SendMessageToServer("salut le server");
    }

    void StartConnect(string serverIP, int port)
    {
        Debug.Log("Trying Connection To" + serverIP);

        ThreadStart threadstart = delegate { ConnectThread(serverIP, port); };

        new Thread(threadstart).Start();
    }

    public void ConnectThread(string serverIP, int port)
    {
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIP), port);

        try
        {
            socket.Connect(serverEndPoint);
            Debug.Log("Connected to server at " + serverIP.ToString());
        }
        catch (Exception e)
        {
            Debug.Log("error connecting to server " + e.ToString());
            Disconnect();
        }
    }

    public void SendMessageToServer(string message)
    {
        byte[] msg = Encoding.ASCII.GetBytes(message);
        try
        {
            socket.Send(msg);
        }
        catch (Exception e)
        {
            Debug.Log("error sending message : " + e.ToString());
        }
    }

    void StartReceiveMessage()
    {
        ThreadStart threadstart = delegate { ReceiveMessageThread(); };

        new Thread(threadstart).Start();
        isReceiving = true;
    }

    public void ReceiveMessageThread()
    {
        try
        {
            byte[] messageReceived = new byte[1024];
            int nbBytes = socket.Receive(messageReceived);
            if (nbBytes > 0)
                Debug.Log(Encoding.ASCII.GetString(messageReceived, 0, nbBytes));
        }
        catch (Exception e)
        {
            Debug.Log("error receiving message : " + e.ToString());
        }

        isReceiving = false;
    }

    public void Disconnect()
    {
        if (socket != null)
        {
            Debug.Log("Disconect Client Socket");

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}