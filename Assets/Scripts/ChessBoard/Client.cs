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
    Thread joinThread;
    Socket socket;
    private IPAddress networkIP;
    bool isReceiving = false;
    public bool Connected { get { return socket.Connected; } }

    private NetworkDataDispatcher dispatcher = null;

    public void Awake()
    {
        if (!enabled)
            return;
        if (RegisterNetworkIP())
        {
            socket = new Socket(networkIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Debug.Log("Initialazing Client");
        }
        else
        {
            Debug.Log("[CLIENT] Could not find this machine's IP LAN address. Are you connected to a network?");
            Disconnect();
        }

        dispatcher = FindObjectOfType<NetworkDataDispatcher>();
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

    void Update()
    {
        if (Connected && !isReceiving)
            StartReceiveMessage();

        //if (Input.GetKeyDown(KeyCode.F) && socket.Connected)
        //    SendMessageToServer("salut le server");
    }

    public void StartConnect(string serverIP, int port)
    {
        Debug.Log("Trying Connection To" + serverIP);

        ThreadStart threadstart = delegate { ConnectThread(serverIP, port); };

        joinThread = new Thread(threadstart);
        joinThread.Start();
    }

    public void CancelJoinServer()
    {
        if (joinThread != null)
            joinThread.Abort();
        Debug.Log("Cancel Join");
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

    public void SendMessageToServer(byte[] message)
    {
        if (!Connected)
            return;
        try
        {
            socket.Send(message);
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
            {
                Debug.Log(Encoding.ASCII.GetString(messageReceived, 0, nbBytes));
                dispatcher.ProcessReceivedMessage(messageReceived);
            }
        }
        catch (Exception e)
        {
            Debug.Log("error receiving message : " + e.ToString());
        }

        isReceiving = false;
    }

    public void Disconnect()
    {
        if (socket != null && socket.Connected)
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
