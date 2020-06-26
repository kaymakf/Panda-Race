using UnityEngine;
using TMPro;

public class MainMenuButtons : UIScene {
	public GameObject OptionsMenuObject;
	public GameObject NickTextObject;
	public GameObject FindingMatchDialog;

    private ServerConnection Connection;
	private ServerConnection.Matchmaker Matchmaker;

	void Start() {
		Connection = ServerConnection.Instance;
		Matchmaker = Connection.matchmaker;
		NickTextObject.GetComponent<TMP_InputField>().text = GlobalModel.Me.User.Username;

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

    public async void OnChangeNick() {
		var newNick = NickTextObject.GetComponent<TMP_InputField>().text;
		if (newNick == null || newNick.Length < 2 || newNick == GlobalModel.Me.User.Username)
			return;
		var session = await Connection.Session;
		await Connection.Client.UpdateAccountAsync(session, newNick);
		GlobalModel.Me = await Connection.Client.GetAccountAsync(session); ;
    }
}