using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameMenu : UIScene {
    private ServerConnection Connection;
    private ServerConnection.Matchmaker Matchmaker;

    void Start() {
        Connection = ServerConnection.Instance;
        Matchmaker = Connection.matchmaker;
    }

    public void LeaveGame() {
        Connection.Socket.LeaveMatchAsync(Matchmaker.Match);
        Matchmaker.CancelSearch();
        GlobalModel.GameFinished = false;
        ExitScene("MainMenu");
    }

    //public void NewGame() {
    //    LeaveGame();
    //    Matchmaker.Search("*", 2, 2);
    //}
}
