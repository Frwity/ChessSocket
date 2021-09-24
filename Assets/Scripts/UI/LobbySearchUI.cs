using UnityEngine;
using UnityEngine.UI;

public class LobbySearchUI : MonoBehaviour
{
    // Inspector
    [SerializeField]
    private InputField pseudo = null;

    public string Pseudo
    {
        get { return pseudo.text; }
    }

    [SerializeField]
    private InputField IP = null;

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

    // Internal
    bool pseudoOK = false;
    bool IPOk     = false;

    // Methods
    private void OnValidate()
    {
        join.interactable = false;
    }

    private void Refresh()
    {
        join.interactable = IPOk & pseudoOK;
        found.enabled     = false;
        notFound.enabled  = false;
    }

    public void PseudoUpdated()
    {
        pseudoOK = pseudo.text.Length > 0;
        Refresh();
    }

    public void IPUpdated()
    {
        IPOk = IP.text.Length > 0;
        Refresh();
    }

    public void SetFound(bool toggle)
    {
        found.enabled = toggle;
        notFound.enabled = !toggle;
    }

    public void FoundOpponent(string name)
    {
        SetFound(true);
        found.text = "Connected to " + name + ". The host will launch the game";
    }

    public void Connect()
    {
        client.StartConnect(IP.text);
    }
}
