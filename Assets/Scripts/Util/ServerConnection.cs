using System;
using System.Threading.Tasks;
using UnityEngine;
using Nakama;

public class ServerConnection : MonoBehaviour {
	private const string Host = "ec2-52-57-140-254.eu-central-1.compute.amazonaws.com";
	private const int Port = 9000;
	
	public int LoginType { get; set; }
	public const int LoginTypeDeviceId = 0;
	public const int LoginTypeFacebook = 1;

	private const string SessionPrefName = "pandarace.session";
	private const string SingletonName = "/[ServerConnection]";

	private const string SocketServerKey = "2r5u8x/A?D*G-KaPdSgVkYp3s6v9y$B&";
	private const string SessionEncryptionKey = "G-KaPdSgVkYp3s6v9y$B?E(H+MbQeThW";
	private const string RuntimeHttpKey = "!z%C*F-JaNdRgUkXp2s5u8x/A?D(G+Kb";

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
		Client = new Client("http", Host, Port, SocketServerKey) {
#if UNITY_EDITOR
			Logger = new UnityLogger()
#endif
		};
		Socket = Client.NewSocket();
		Socket.Closed += () => Debug.Log("Socket closed.");
		Socket.Connected += () => Debug.Log("Socket connected.");
		Socket.ReceivedError += e => Debug.LogErrorFormat("Socket error: {0}", e.Message);
	}

	private Task<ISession> Authenticate() {
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
		var expiredDate = DateTime.UtcNow;
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