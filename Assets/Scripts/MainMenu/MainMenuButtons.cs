using Nakama.TinyJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuButtons : UIScene {
	public GameObject OptionsMenuObject;
	public GameObject FindingMatchDialog;

    private ServerConnection Connection;
	private ServerConnection.Matchmaker Matchmaker;

	void Start() {
		Connection = ServerConnection.Instance;
		Matchmaker = Connection.matchmaker;

		EnterScene();
	}

	void Update() {
		if (Matchmaker != null && Matchmaker.IsMatchReady)
			ExitScene("Game");
	}

	public void OnOptions() {
		OptionsMenuObject.SetActive(true);
		gameObject.SetActive(false);
	}

	public void OnLogOut() {
		Connection.Socket?.CloseAsync();
		ExitScene("Login");
	}

	public void OnPlay() {
		Matchmaker.Search("*", 2, 2);
		OpenDialog(FindingMatchDialog);
	}

    public void OnCancelMatchmaking() {
		Matchmaker.CancelSearch();
		CloseDialog(FindingMatchDialog);
	}
}