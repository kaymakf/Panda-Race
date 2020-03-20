using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Debug_RestartGameplay : MonoBehaviour
{
    public void Restart() {
        SceneManager.LoadSceneAsync("MainMenu");
        ServerConnection.Instance.Socket.LeaveMatchAsync(
            ServerConnection.Instance.matchmaker.Match.Id).ContinueWith(
            (x) => ServerConnection.Instance.matchmaker.Search("*", 2, 2)
        );
    }
}
