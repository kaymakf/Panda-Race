using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtons : MonoBehaviour
{
    public GameObject OptionsMenuObject;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnOptions()
    {
        OptionsMenuObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void OnLogOut()
    {
        ServerConnection.Instance.Socket?.CloseAsync();
        SceneManager.LoadScene("Login");
    }

    public void OnPlay() {
        ServerConnection.Instance.Socket.AddMatchmakerAsync("*", 2, 2);
        ServerConnection.Instance.Socket.ReceivedMatchmakerMatched += async matched => {
            Debug.LogFormat("Received: {0}", matched);
            await ServerConnection.Instance.Socket.JoinMatchAsync(matched);
            
            SceneManager.LoadScene("Game");
        };
        
        
    }
}
