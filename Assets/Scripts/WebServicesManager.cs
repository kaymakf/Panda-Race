using System;
using System.Collections.Generic;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.CognitoSync;
using Amazon.CognitoSync.SyncManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WebServicesManager : MonoBehaviour {
	#region Constants and configuration values

	private const string CognitoIdentityPoolId = "us-east-2:279f6dd1-0bc4-45c5-ab9c-49bad80ef4d8";
	private static readonly RegionEndpoint _cognitoRegion = RegionEndpoint.USEast2;

	private const string PlayerDatasetName = "PlayerInfo";

	#endregion

	#region AWS Clients, Managers, and Contexts, And Info

	private RegionEndpoint CognitoRegion {
		get { return _cognitoRegion != null ? _cognitoRegion : AWSConfigs.RegionEndpoint; }
	}

	private CognitoAWSCredentials _credentials;

	public CognitoAWSCredentials Credentials {
		get {
			if (_credentials == null)
				_credentials = new CognitoAWSCredentials(CognitoIdentityPoolId, CognitoRegion);
			return _credentials;
		}
	}

	private CognitoSyncManager _cognitoSyncManager;

	public CognitoSyncManager CognitoSyncManager {
		get {
			if (_cognitoSyncManager == null)
				_cognitoSyncManager =
					new CognitoSyncManager(Credentials, new AmazonCognitoSyncConfig {RegionEndpoint = CognitoRegion});
			return _cognitoSyncManager;
		}
	}

	#endregion

	#region One-time Creation

	public static WebServicesManager I { get; private set; }

	void Awake() {
		I = this;
		// This WebServicesManager object will persist until the game is closed
		DontDestroyOnLoad(gameObject);
		// Initialize AWS SDK
		UnityInitializer.AttachToGameObject(this.gameObject);
		AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
		Debug.Log("Singleton loaded.");
		SceneManager.LoadScene("Login");
	}

	#endregion

	#region Using Amazon Cognito Sync

	// Save the game state to CognitoSync's local storage.
	public void SaveStateLocal() {
		if (GlobalModel.Me != null) {
			Dataset playerDataset = CognitoSyncManager.OpenOrCreateDataset(PlayerDatasetName);
			playerDataset.Put(GlobalModel.Me.Id, GlobalModel.Me.Name);
		}
	}

	// Synchronize the locally saved data with Cognito
	public void SynchronizeLocalDataAsync() {
		Dataset playerDataset = CognitoSyncManager.OpenOrCreateDataset(PlayerDatasetName);
		playerDataset.OnSyncFailure += (object sender, SyncFailureEventArgs e) => {
			Debug.LogWarning("Failed to sync, but still saved locally.");
			playerDataset.Dispose();
		};
		playerDataset.OnSyncSuccess += (object sender, SyncSuccessEventArgs e) => {
			Debug.Log("Sync successful.");
			playerDataset.Dispose();
		};
		playerDataset.SynchronizeAsync();
	}

	// Load friends and local matches with Cognito Sync. If there is no network available, the CognitoSyncManager uses the locally saved data.
	public void LoadStateAsync(Action successAction = null, Action<GameObject> failAction = null,
							   GameObject failedActionParameter = null) {
		Dataset playerDataset = CognitoSyncManager.OpenOrCreateDataset(PlayerDatasetName);

		playerDataset.OnSyncSuccess += (object sender, SyncSuccessEventArgs e) => {
			var dataset = sender as Dataset;

			Debug.Log("Successfully synced for dataset: " + dataset.Metadata?.ToString());

			PlayerModel me = new PlayerModel() {Id = dataset.Get("id"), Name = dataset.Get("username")};
			GlobalModel.Me = me;

			Debug.Log("id: " + GlobalModel.Me.Id + ", name: " + GlobalModel.Me.Name);
			successAction?.Invoke();
		};

		playerDataset.OnSyncFailure += (object sender, SyncFailureEventArgs e) => {
			var dataset = sender as Dataset;
			Debug.Log("Sync failed for dataset : " + dataset.Metadata.DatasetName);
			Debug.LogException(e.Exception);
			failAction?.Invoke(failedActionParameter);
		};

		playerDataset.OnSyncConflict += (Dataset dataset, List<SyncConflict> conflicts) => {
			Debug.Log("OnSyncConflict");
			List<Record> resolvedRecords = new List<Record>();

			foreach (SyncConflict conflictRecord in conflicts) {
				// This example resolves all the conflicts using ResolveWithRemoteRecord 
				// SyncManager provides the following default conflict resolution methods:
				//      ResolveWithRemoteRecord - overwrites the local with remote records
				//      ResolveWithLocalRecord - overwrites the remote with local records
				//      ResolveWithValue - for developer logic  
				resolvedRecords.Add(conflictRecord.ResolveWithRemoteRecord());
			}

			// resolves the conflicts in local storage
			dataset.Resolve(resolvedRecords);

			// on return true the synchronize operation continues where it left,
			//      returning false cancels the synchronize operation
			return true;
		};

		//playerDataset.OnDatasetDeleted +=;
		//playerDataset.OnDatasetMerged += ;

		playerDataset.SynchronizeAsync();
	}


	//// Use the Facebook sdk to log in with Facebook credentials
	//public void LogInToFacebookAsync()
	//{
	//    if (!FB.IsInitialized)
	//    {
	//        FB.Init(() =>
	//        {
	//            FB.LogInWithReadPermissions(null, FacebookLoginCallback);
	//        });
	//    }
	//    else
	//    {
	//        FB.LogInWithReadPermissions(null, FacebookLoginCallback);
	//    }
	//}

	//// Attch the Facebook Login token to our Cognito Identity.
	//private void FacebookLoginCallback(ILoginResult result)
	//{
	//    if (result.Error != null || !FB.IsLoggedIn)
	//    {
	//        Debug.LogError(result.Error);
	//    }
	//    else
	//    {
	//        Debug.Log("Adding login to credentials");
	//        Credentials.AddLogin("graph.facebook.com", result.AccessToken.TokenString);
	//    }
	//    GameManager.Instance.Load();
	//}

	#endregion
}