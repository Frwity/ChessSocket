using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class ServerHost : MonoBehaviour
{
    Socket socket;
    int port = 11000;
    IPEndPoint localEP;
    Socket clientSocket;
    bool hasClient = false;
    bool isReceiving = false;

    void Awake()
    {
        IPHostEntry host = Dns.GetHostEntry("localhost");
        IPAddress ipAdress = host.AddressList[0];
        localEP = new IPEndPoint(ipAdress, port);
        socket = new Socket(ipAdress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        Debug.Log("Starting Server");
        socket.Bind(localEP);
        socket.Listen(10);
    }

    void Start()
    {
        StartAcceptClient();
    }

    void Update()
    {
        if (hasClient && !isReceiving)
            StartReceiveMessage();

        if (Input.GetKeyDown(KeyCode.G))
            SendMessageToClient("salut le client");
    }

    void StartAcceptClient()
    {
        ThreadStart threadstart = delegate { AcceptClientThread(); };

        new Thread(threadstart).Start();
    }

    void AcceptClientThread()
    {
        try
        {
            clientSocket = socket.Accept();
            Debug.Log("Accepted Client !");
            hasClient = true;
        }
        catch (Exception e)
        {
            Debug.Log("error " + e.ToString());
            Disconnect();
        }
    }

    public void SendMessageToClient(string message)
    {
        byte[] msg = Encoding.ASCII.GetBytes(message);
        try
        {
            clientSocket.Send(msg);
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
            int nbBytes = clientSocket.Receive(messageReceived);
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
        if (clientSocket != null)
        {
            // shutdown client socket
            try
            {
                Debug.Log("Shutdown Client Socket");
                clientSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Debug.Log("error " + e.ToString());
            }
            finally
            {
                Debug.Log("Closing Client Socket");

                clientSocket.Close();
            }
        }

        if (socket != null)
        {
            Debug.Log("Closing Socket");
            // server socket : no shutdown necessary
            socket.Close();
        }
    }
    private void OnDestroy()
    {
        Disconnect();
    }
}
