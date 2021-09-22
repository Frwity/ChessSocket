using UnityEngine;
using UnityEngine.UI;
using System;

public class Chat : MonoBehaviour
{
    [SerializeField]
    private InputField writeField;
    NetworkDataDispatcher dispatcher = null;

    private void Start()
    {
        dispatcher = FindObjectOfType<NetworkDataDispatcher>();
    }

    public void Send()
    {
        if (writeField.text.Length <= 0)
            return;

        string message = DateTime.Now.ToString("[hh:mm:ss] ") + dispatcher.Pseudo + ": " + writeField.text;
        dispatcher.SendChat(message);

        writeField.text = "";
    }
}
