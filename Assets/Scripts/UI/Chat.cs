using UnityEngine;
using UnityEngine.UI;
using System;

public class Chat : MonoBehaviour
{
    [SerializeField]
    private InputField writeField;
    NetworkDataDispatcher dispatcher = null;

    [SerializeField]
    Text text1;
    [SerializeField]
    Text text2;
    [SerializeField]
    Text text3;
    [SerializeField]
    Text text4;
    [SerializeField]
    Text text5;
    [SerializeField]
    Text text6;

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

        Receive(message);
    }

    public void Receive(string message)
    {
        text6.text = text5.text;
        text5.text = text4.text;
        text4.text = text3.text;
        text3.text = text2.text;
        text2.text = text1.text;
        text1.text = message;
    }
}
