using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveGame : MonoBehaviour
{
    [SerializeField]
    public GameObject leaveMenu;

    private bool canLeave = true;

    public void setCanLeave(bool state)
    {
        canLeave = state;
    }

    private void Update()
    {
        if (canLeave && Input.GetKeyDown(KeyCode.Escape))
            leaveMenu.SetActive(!leaveMenu.activeSelf);
    }
}
