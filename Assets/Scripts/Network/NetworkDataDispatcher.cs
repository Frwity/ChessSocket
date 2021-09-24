using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using static ChessSerializer;

public class NetworkDataDispatcher : MonoBehaviour
{
    ConcurrentQueue<ChessObject> dataQueue;

    [Header("Network")]
    [SerializeField]
    [Range(1024, 49151)]
    private int port = 5683;

    [Header("Managed objects")]
    [SerializeField]
    private ChessGameMgr chessGameMgr = null;

    [SerializeField]
    private GameObject startMenu = null;

    [SerializeField]
    private GUIMgr inGameUI = null;

    [SerializeField]
    private OpenLobbyUI openLobbyUI = null;

    [SerializeField]
    private LobbySearchUI lobbySearchUI = null;

    [SerializeField]
    private Chat chat = null;

    [SerializeField]
    private LeaveGame leaveGame = null;

    private ServerHost serverHost;
    private Client     client;

    private bool       isHost = false;

    public int Port
    {
        get { return port; }
    }

    public string Pseudo
    {
        get { return isHost ? openLobbyUI.Pseudo : lobbySearchUI.Pseudo; }
    }

    private void Start()
    {
        dataQueue  = new ConcurrentQueue<ChessObject>();
        client     = GetComponent<Client>();
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
        string pseudo = isHost ? openLobbyUI.Pseudo : lobbySearchUI.Pseudo;
        byte[] msg = ChessSerializer.Serialize(ChessSerializer.DataType.NAME, pseudo);
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

    public void SendChat(string chat)
    {
        byte[] message = ChessSerializer.Serialize(ChessSerializer.DataType.CHAT, chat);
        SendMessage(message);
    }

    public void SendPing()
    {
        byte[] ping = ChessSerializer.Serialize(ChessSerializer.DataType.PING, "ping");
        SendMessage(ping);
    }

    public void Receive(byte[] packet)
    {
        ChessObject obj = ChessSerializer.Deserialize(packet);

        if (obj.type != ChessSerializer.DataType.PING)
        {
            dataQueue.Enqueue(obj);
        }
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

            case ChessSerializer.DataType.BEGIN:
                leaveGame.setCanLeave(true);
                lobbySearchUI.gameObject.SetActive(false);
                chessGameMgr.gameObject.SetActive(true);
                inGameUI.gameObject.SetActive(true);

                chessGameMgr.enabled = true;
                chessGameMgr.SetIAEnable(false);
                break;

            case ChessSerializer.DataType.COLOR:
                chessGameMgr.SetPlayingAs(!((bool)chessObject.obj));
                chessGameMgr.PrepareGame();
                chessGameMgr.UpdateCameraRotation();
                break;

            case ChessSerializer.DataType.MOVE:
                chessGameMgr.SetOpponentMove((ChessGameMgr.Move)chessObject.obj);
                break;

            case ChessSerializer.DataType.CHAT:
                chat.Receive((string)chessObject.obj);
                break;

            case ChessSerializer.DataType.PING:
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

    public void BeginGame()
    {
        SendBegin();

        leaveGame.setCanLeave(true);
        openLobbyUI.gameObject.SetActive(false);
        chessGameMgr.gameObject.SetActive(true);
        inGameUI.gameObject.SetActive(true);
        
        SendColor();

        chessGameMgr.enabled = true;
        chessGameMgr.PrepareGame();
        chessGameMgr.UpdateCameraRotation();
    }

    public void QuitGame()
    {
        leaveGame.setCanLeave(false);
        leaveGame.transform.GetChild(0).gameObject.SetActive(false);
        if (isHost)
        {
            if (openLobbyUI != null)
            {
                serverHost.Disconnect();
                openLobbyUI.NoOpponent();
            }
        }
        else
        {
            if (lobbySearchUI != null)
            {
                client.Disconnect();
                lobbySearchUI.SetFound(false);
            }
        }

        if (chessGameMgr != null)
        {
            chessGameMgr.gameObject.SetActive(false);
            chessGameMgr.enabled = false;
        }
        if (inGameUI != null)
            inGameUI.gameObject.SetActive(false);

        startMenu.SetActive(true);
    }
}
