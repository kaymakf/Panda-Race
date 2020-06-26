using System.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

public class Auth : UIScene
{
    public GameObject LoadingCircle;

    private ServerConnection Connection;

    void Start() {
        Connection = ServerConnection.Instance;
        EnterScene();
    }
    
    public async void GuestLogin() {
        await Login(ServerConnection.LoginTypeDeviceId);
    }

    public void FbLogin() {
        //await Login(ServerConnection.LoginTypeFacebook);
        ShowLoading(LoadingCircle);
    }

    private async Task Login(int loginType) {
        //Fade.gameObject.SetActive(true);
        //Fade.DOFade(.6f, .2f).SetEase(Ease.InSine);
        ShowLoading(LoadingCircle);

        Connection.LoginType = loginType;
        var session = await Connection.Session;
        Debug.LogFormat("Active Session: {0}", session);
        GlobalModel.Me = await Connection.Client.GetAccountAsync(session);
        Debug.LogFormat("Account id: {0}", GlobalModel.Me.User.Id);
        
        await Connection.Socket.ConnectAsync(session);
        ExitScene("MainMenu");
    }
}
