using Nakama.TinyJson;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class MainMenuButtons : MonoBehaviour {
	public GameObject OptionsMenuObject;
	public Image Fade;
	
	private bool isMatchReady = false;

	void Start() {
		var fadeAnim = Fade.DOFade(1, .5f).From().SetEase(Ease.Linear);
		fadeAnim.onComplete = () => Fade.gameObject.SetActive(false);
		
		gameObject.transform.DOMoveY(-2, .5f).From().SetEase(Ease.InSine);
	}

	// Update is called once per frame
	void Update() {
		if (isMatchReady)
			SceneManager.LoadSceneAsync("Game");
	}

	public void OnOptions() {
		OptionsMenuObject.SetActive(true);
		gameObject.SetActive(false);
	}

	public void OnLogOut() {
		ServerConnection.Instance.Socket?.CloseAsync();
		SceneManager.LoadScene("Login");
	}

	public void OnPlay() {
		ServerConnection.Instance.Socket.AddMatchmakerAsync("*", 2, 2);
		ServerConnection.Instance.Socket.ReceivedMatchmakerMatched += async matched => {
			Debug.LogFormat("Received: {0}", matched);
			var match = await ServerConnection.Instance.Socket.JoinMatchAsync(matched);

			Debug.LogFormat("Self: {0}", match.Self);
			Debug.LogFormat("Presences: {0}", match.Presences.ToJson());

			isMatchReady = true;
		};
	}
}