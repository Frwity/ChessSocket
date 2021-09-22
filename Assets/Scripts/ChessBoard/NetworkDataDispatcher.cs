using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkDataDispatcher : MonoBehaviour
{
    [SerializeField]
    ChessGameMgr chessGameMgr = null;

    public static void ProcessReceivedMessage(byte[] message)
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
}
