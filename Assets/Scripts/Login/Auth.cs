using System;
using Amazon.CognitoIdentity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Auth : MonoBehaviour {
	public GameObject LoadingCircle;
	private Action LoginSuccess = () => SceneManager.LoadScene("MainMenu");
	private Action<GameObject> LoginFailed = (loading) => loading.SetActive(false);

	void Awake() {
		if (WebServicesManager.I == null)
			SceneManager.LoadScene("Singleton");
	}

	public void OnGuestLoginClick() {
		LoadingCircle.SetActive(true);
		WebServicesManager.I.Credentials.GetIdentityIdAsync(Login);
	}

	void Login(AmazonCognitoIdentityResult<string> result) {
		if (result.Exception != null) {
			Debug.Log("Login failed.");
			return;
		}

		string identityId = result.Response;
		Debug.Log("Login successful: " + identityId);

		WebServicesManager.I.LoadStateAsync(LoginSuccess, LoginFailed, LoadingCircle);
	}
}