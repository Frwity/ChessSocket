using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine;
using static ChessGameMgr;

public class ChessSerializer
{
    [Serializable]
    public enum DataType
    {
        NAME  = (1     << 0),
        READY = (NAME  << 1),
        BEGIN = (READY << 1),
        COLOR = (BEGIN << 1),
        MOVE  = (COLOR << 1),
    }

    [Serializable]
    public struct ChessObject
    {
        public DataType type;
        public object   obj;

        public ChessObject(DataType type_, object obj_)
        {
            type = type_;
            obj  = obj_;
        }
    }

    static public byte[] Serialize(DataType type, object obj)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            ChessObject chessObj = new ChessObject(type, obj);
            formatter.Serialize(stream, chessObj);

            return stream.ToArray();
        }
    }

    // After retrieving the ChessObject, check the "type" data member to know what "obj" is
    static public ChessObject Deserialize(byte[] bytes)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(bytes))
        {
            return (ChessObject)formatter.Deserialize(stream);
        }
    }
}