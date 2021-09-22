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
}
