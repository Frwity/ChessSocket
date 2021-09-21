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
    int port = 11000;
    bool isReceiving = false;
    public bool Connected { get { return socket.Connected; } }

    private void Awake()
    {
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAdress = host.AddressList[0];
        socket = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("Client Creation");
    }

    void Start()
    {
        Connect();
    }

    void Update()
    {
        if (Connected && !isReceiving)
            StartReceiveMessage();

        if (Input.GetKeyDown(KeyCode.F))
            SendMessageToServer("salut le server");
    }

    public void Connect()
    {
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAdress = host.AddressList[0];
        IPEndPoint serverEP = new IPEndPoint(ipAdress, 11000);
        try
        {
            socket.Connect(serverEP);
            Debug.Log("Connected to server at " + ipAdress.ToString());
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
