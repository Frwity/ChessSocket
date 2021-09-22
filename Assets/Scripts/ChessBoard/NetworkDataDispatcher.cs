using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDataDispatcher : MonoBehaviour
{
    [SerializeField]
    ChessGameMgr chessGameMgr = null;

    Client client;
    ServerHost serverHost;

    private bool isHost = false;

    private void Start()
    {
        client = GetComponent<Client>();
        serverHost = GetComponent<ServerHost>();
    }

    public void SetHost(bool state)
    {
        isHost = state;
        if (isHost)
        {
            if (client.enabled)
                client.Disconnect();
            client.enabled = false;
            serverHost.enabled = true;
            serverHost.Awake();
        }
        else
        {
            if (serverHost.enabled)
                serverHost.Disconnect();
            serverHost.enabled = false;
            client.enabled = true;
            client.Awake();
        }
    }

    public void ProcessReceivedMessage(byte[] message)
    {
        ChessSerializer.ChessObject chessObject = ChessSerializer.Deserialize(message);

        switch (chessObject.type)
        {
            case ChessSerializer.DataType.NAME:
                break;
            case ChessSerializer.DataType.READY:
                break;
            case ChessSerializer.DataType.BEGIN:
                break;
            case ChessSerializer.DataType.MOVE:
                break;
            default:
                break;
        }
    }

    public void SendMessage(byte[] message)
    {
        if (isHost)
            serverHost.SendMessageToClient(message);
        else
            client.SendMessageToServer(message);

    }
}
