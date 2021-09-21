using UnityEngine;
using UnityEngine.UI;

public class LobbySearchUI : MonoBehaviour
{
    // Inspector
    [SerializeField]
    private InputField IP = null;

    [SerializeField]
    private InputField port = null;

    [SerializeField]
    private Button join = null;

    [SerializeField]
    private Text found = null;

    [SerializeField]
    private Text notFound = null;

    [SerializeField]
    private Client client = null;

    // Properties
    public InputField IPField { get { return IP; } }
    public InputField portField { get { return port; } }

    // Internal
    bool IPOk   = false;
    bool portOk = false;

    // Methods
    private void OnValidate()
    {
        join.interactable = false;
    }

    private void Refresh()
    {
        join.interactable = IPOk && portOk;
        found.enabled = false;
        notFound.enabled = false;
    }

    public void IPUpdated()
    {
        IPOk = IP.text.Length > 0;
        Refresh();
    }

    public void PortUpdated()
    {
        portOk = port.text.Length > 0;
        Refresh();
    }

    public void SetFound(bool toggle)
    {
        found.enabled = toggle;
        notFound.enabled = !toggle;
    }
}
