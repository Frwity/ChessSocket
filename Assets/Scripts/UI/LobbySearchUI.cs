using UnityEngine;
using UnityEngine.UI;

public class LobbySearchUI : MonoBehaviour
{
    [SerializeField]
    private ScrollRect lobbies;

    [SerializeField]
    private Button join;

    [SerializeField]
    private Toggle ready;

    private void OnValidate()
    {
        if (lobbies == null)
        {
            lobbies = GetComponentInChildren<ScrollRect>();
        }

        if (join == null)
        {
            join = GetComponentInChildren<Button>();
        }

        if (ready == null)
        {
            ready = GetComponentInChildren<Toggle>();
        }
    }

    private void Update()
    {
        
    }

    public void AddLobby(string lobbyName)
    {

    }

    public void RemoveLobby(string lobbyName)
    {

    }
}
