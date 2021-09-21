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
    Thread lobbyThread;
    Socket serverSocket;
    int port = 11000;
    IPEndPoint localEP;
    IPAddress networkIP;
    Socket clientSocket;
    bool hasClient = false;
    bool isReceiving = false;

    void Awake()
    {
        if (RegisterNetworkIP())
        {
            localEP = new IPEndPoint(networkIP, port);
            serverSocket = new Socket(networkIP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Debug.Log("Initialazing Server");
            serverSocket.Bind(localEP);
            serverSocket.Listen(10);
        }
        else
        {
            Debug.Log("[SERVER] Could not find this machine's IP LAN address. Are you connected to a network?");
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

    void Update()
    {
        if (hasClient && !isReceiving)
            StartReceiveMessage();

        if (Input.GetKeyDown(KeyCode.G) && hasClient)
            SendMessageToClient("salut le client");
    }

    public void StartAcceptClient()
    {
        Debug.Log("Creating Lobby !");

        ThreadStart threadstart = delegate { AcceptClientThread(); };

        lobbyThread = new Thread(threadstart);
        lobbyThread.Start();
    }

    void AcceptClientThread()
    {
        try
        {
            clientSocket = serverSocket.Accept();
            Debug.Log("Accepted Client !");
            hasClient = true;
        }
        catch (Exception e)
        {
            Debug.Log("error " + e.ToString());
            Disconnect();
        }
    }
    
    public void CancelAcceptClient()
    {
        //Disconnect();
        //lobbyThread.Abort();
        Debug.Log("Closing Lobby (WIP)");
    }

    public void SendMessageToClient(string message)
    {
        if (!hasClient)
            return;
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
            // shutdown client Socket
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

        if (serverSocket != null)
        {
            Debug.Log("Closing Socket");
            // server Socket : no shutdown necessary
            serverSocket.Close();
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
