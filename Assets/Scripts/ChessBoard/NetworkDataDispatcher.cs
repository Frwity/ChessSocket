using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetworkDataDispatcher : MonoBehaviour
{
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
                inGameUI.SetOpponentPseudo((string)chessObject.obj);
                openLobbyUI.FoundOpponent((string)chessObject.obj);
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
                chessGameMgr.PlayTurn((ChessGameMgr.Move)chessObject.obj);
                break;
            default:
                Debug.LogError("Something is very wrong: the default case was reached in ProcessReceivedMessage()");
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
