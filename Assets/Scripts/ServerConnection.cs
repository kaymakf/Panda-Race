using System;
using System.Threading.Tasks;
using UnityEngine;
using Nakama;

public class ServerConnection : MonoBehaviour {
	public int LoginType { get; set; }
	public const int LoginTypeDeviceId = 0;
	public const int LoginTypeFacebook = 1;

	private const string SessionPrefName = "pandarace.session";
	private const string SingletonName = "/[ServerConnection]";

	private static readonly object Lock = new object();
	private static ServerConnection _instance;
	
	public static ServerConnection Instance {
		get {
			lock (Lock) {
				if (_instance != null) return _instance;
				var obj = GameObject.Find(SingletonName);
				if (obj == null) {
					obj = new GameObject(SingletonName);
				}

				if (obj.GetComponent<ServerConnection>() == null) {
					obj.AddComponent<ServerConnection>();
				}

				DontDestroyOnLoad(obj);
				_instance = obj.GetComponent<ServerConnection>();
				return _instance;
			}
		}
	}

	public IClient Client { get; }
	public ISocket Socket { get; }

	public Task<ISession> Session { get; private set; }

	private ServerConnection() {
		Client = new Client("http", "127.0.0.1", 7350, "defaultkey") {
#if UNITY_EDITOR
			Logger = new UnityLogger()
#endif
		};
		Socket = Client.NewSocket();
	}

	public Task<ISession> Authenticate() {
		switch (LoginType) {
			case LoginTypeDeviceId:
				const string deviceIdPrefName = "deviceid";
				var deviceId = PlayerPrefs.GetString(deviceIdPrefName, SystemInfo.deviceUniqueIdentifier);
#if UNITY_EDITOR
				Debug.LogFormat("Device id: {0}", deviceId);
#endif
				// With device IDs save it locally in case of OS updates which can change the value on device.
				PlayerPrefs.SetString(deviceIdPrefName, deviceId);
				return Client.AuthenticateDeviceAsync(deviceId);
			default:
				return null;
		}
	}

	private void Awake() {
		// Restore session or create a new one.
		var authToken = PlayerPrefs.GetString(SessionPrefName);
		var session = Nakama.Session.Restore(authToken);
		var expiredDate = DateTime.UtcNow.AddDays(-1);
		if (session == null || session.HasExpired(expiredDate)) {
			var sessionTask = Authenticate();
			Session = sessionTask;
			sessionTask.ContinueWith(t => {
				if (t.IsCompleted) {
					PlayerPrefs.SetString(SessionPrefName, t.Result.AuthToken);
				}
			}, TaskScheduler.FromCurrentSynchronizationContext());
		}
		else {
			Session = Task.FromResult(session);
		}
	}

	private void OnApplicationQuit() => Socket?.CloseAsync();
}