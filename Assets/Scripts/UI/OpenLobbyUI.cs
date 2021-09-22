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

    public void SetFound(bool toggle)
    {
        found.enabled   = play.interactable = toggle;
        waiting.enabled = !toggle;
    }

    public void Awake()
    {
        IP.text = "Lobby IP: " + serverHost.FullIP;
    }
}
