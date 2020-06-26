using System;
using System.Threading.Tasks;
using UnityEngine;
using Nakama;
using Nakama.TinyJson;
using System.Collections.Generic;
using System.Collections;

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

    public Matchmaker matchmaker { get => Matchmaker.I; }

    private ServerConnection() {
		Client = new Client("http", Host, Port, SocketServerKey) {
#if UNITY_EDITOR
			Logger = new UnityLogger()
#endif
		};
		Socket = Client.NewSocket();
		Socket.Closed += () => Debug.Log("Socket closed.");
		Socket.Connected += () => Debug.Log("Socket connected.");
		Socket.ReceivedError += async e => {
			Debug.LogErrorFormat("Socket error: {0}", e.Message);
			if (!Socket.IsConnected) await Socket.ConnectAsync(await Session);
		};
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

    public class Matchmaker {
        public bool IsMatchReady { get; private set; }
        public IMatch Match { get; private set; }

        private IMatchmakerTicket matchticket;

        private static Matchmaker _instance;
        public static Matchmaker I {
            get {
                if (_instance == null) _instance = new Matchmaker();
                return _instance;
            }
        }

        private Matchmaker() {
            IsMatchReady = false;
        }

        public async void Search(string query, int minCount, int maxCount) {
            ServerConnection.Instance.Socket.ReceivedMatchmakerMatched += async matched => {
                var match = await ServerConnection.Instance.Socket.JoinMatchAsync(matched);
                Match = match;
#if UNITY_EDITOR
                Debug.LogFormat("Received: {0}", matched);
                Debug.LogFormat("Self: {0}", match.Self);
                Debug.LogFormat("Presences: {0}", match.Presences.ToJson());
#endif
                foreach (var u in matched.Users)
                    if (u.Presence.UserId != GlobalModel.Me.User.Id)
                        GlobalModel.Opponent = u.Presence;

                ServerConnection.Instance.Socket.ReceivedMatchState += GameController.RecieveState;

                if (string.Compare(GlobalModel.Me?.User.Id, GlobalModel.Opponent?.UserId) > 0) {
                    int myChar = new System.Random().Next(2);
                    GlobalModel.SetMyCharacter(myChar);
                    GameController.SendState(GameController.ACTION_PICKED_CHARACTER, myChar.ToJson());
                }
                IsMatchReady = true;
            };

			ServerConnection.Instance.Socket.ReceivedMatchPresence += presenceEvent => {
				if (presenceEvent.Leaves != null && (presenceEvent.Leaves as IList).Count != 0) {
					Debug.Log("Opponent Left the game...");
					Instance.Socket.LeaveMatchAsync(Match);
					CancelSearch();
					GlobalModel.ResetGameFlags();
                }
			};

			matchticket = await ServerConnection.Instance.Socket.AddMatchmakerAsync(query, minCount, maxCount);
        }

        public void CancelSearch() {
            if (matchticket != null) ServerConnection.Instance.Socket.RemoveMatchmakerAsync(matchticket);
			IsMatchReady = false;
        }
    }
}
