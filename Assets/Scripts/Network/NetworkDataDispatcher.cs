using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static ChessSerializer;

public class NetworkDataDispatcher : MonoBehaviour
{
    ConcurrentQueue<ChessObject> dataQueue;

    [SerializeField]
    ChessGameMgr chessGameMgr = null;

    [SerializeField]
    private GUIMgr inGameUI = null;

    [SerializeField]
    private OpenLobbyUI openLobbyUI = null;

    [SerializeField]
    private LobbySearchUI lobbySearchUI = null;
    
    Client client;
    ServerHost serverHost;

    private bool isHost = false;

    private void Start()
    {
        dataQueue = new ConcurrentQueue<ChessObject>();

        client = GetComponent<Client>();
        serverHost = GetComponent<ServerHost>();
    }

    private void Update()
    {
        ProcessData();
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

    // Message types
    public void SendPseudo()
    {
        byte[] msg = ChessSerializer.Serialize(ChessSerializer.DataType.NAME, lobbySearchUI.Pseudo);
        SendMessage(msg);
    }

    public void SendBegin()
    {
        byte[] begin = ChessSerializer.Serialize(ChessSerializer.DataType.BEGIN, true);
        SendMessage(begin);
    }

    public void SendColor()
    {
        byte[] color = ChessSerializer.Serialize(ChessSerializer.DataType.COLOR, chessGameMgr.Color);
        SendMessage(color);
    }

    public void SendMove(ChessGameMgr.Move move)
    {
        byte[] moveByte = ChessSerializer.Serialize(ChessSerializer.DataType.MOVE, move);
        SendMessage(moveByte);
    }

    public void Receive(byte[] packet)
    {
        dataQueue.Enqueue(ChessSerializer.Deserialize(packet));
    }

    public void ProcessData()
    {
        ChessObject chessObject;

        if (dataQueue.TryDequeue(out chessObject) == false)
            return;

        switch (chessObject.type)
        {
            case ChessSerializer.DataType.NAME:
                inGameUI.SetOpponentPseudo((string)chessObject.obj);
                openLobbyUI.FoundOpponent((string)chessObject.obj);
                lobbySearchUI.FoundOpponent((string)chessObject.obj);
                break;
            case ChessSerializer.DataType.READY:
                openLobbyUI.OpponentReady((bool)chessObject.obj);
                lobbySearchUI.OpponentReady((bool)chessObject.obj);
                break;
            case ChessSerializer.DataType.BEGIN:
                chessGameMgr.enabled = true;
                chessGameMgr.SetIAEnable(false);
                break;
            case ChessSerializer.DataType.COLOR:
                chessGameMgr.SetPlayingAs((bool)chessObject.obj);
                break;
            case ChessSerializer.DataType.MOVE:
                chessGameMgr.SetOpponentMove((ChessGameMgr.Move)chessObject.obj);
                break;
            default:
                Debug.LogError("Something is very wrong: the default case was reached in ProcessData()");
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
