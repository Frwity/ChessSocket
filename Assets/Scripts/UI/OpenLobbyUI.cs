using UnityEngine;
using UnityEngine.UI;

public class OpenLobbyUI : MonoBehaviour
{
    [SerializeField]
    private Text waiting = null;

    [SerializeField]
    private InputField IP = null;

    [SerializeField]
    private Text found = null;

    [SerializeField]
    private Button play = null;

    [SerializeField]
    private ServerHost serverHost = null;

    private void Start()
    {
        IP.text = "Lobby IP: " + serverHost.FullIP;
    }

    public void FoundOpponent(string name)
    {
        found.text = name + " joined. Ready to play";

        found.enabled   = play.interactable = true;
        waiting.enabled = false;
    }

    public void NoOpponent()
    {
        found.enabled   = play.interactable = false;
        waiting.enabled = true;
    }

    public void OpponentReady(bool toggle)
    {

    }

    public void SendReady()
    {

    }
}
