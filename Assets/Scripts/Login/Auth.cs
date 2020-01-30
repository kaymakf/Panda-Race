using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Auth : MonoBehaviour
{
    public void GuestLogin() {
        Login(ServerConnection.LoginTypeDeviceId);
    }

    private async Task Login(int loginType) {
        ServerConnection.Instance.LoginType = loginType;
        
        var session = await ServerConnection.Instance.Session;
        Debug.LogFormat("Active Session: {0}", session);
        var account = await ServerConnection.Instance.Client.GetAccountAsync(session);
        Debug.LogFormat("Account id: {0}", account.User.Id);
        
        ServerConnection.Instance.Socket.Closed += () => Debug.Log("Socket closed.");
        ServerConnection.Instance.Socket.Connected += () => Debug.Log("Socket connected.");
        ServerConnection.Instance.Socket.ReceivedError += Debug.LogError;
        await ServerConnection.Instance.Socket.ConnectAsync(session);
        
        SceneManager.LoadScene("MainMenu");
    }
}
