using UnityEngine;
using UnityEngine.UI;

public class OpenLobbyUI : MonoBehaviour
{
    [SerializeField]
    private Text waiting;

    [SerializeField]
    private Text found;

    [SerializeField]
    private Button play;
    
    private void SetFound(bool toggle)
    {
        found.enabled   = play.interactable = toggle;
        waiting.enabled = !toggle;
    }
}
