using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

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

    private async Task Login(int loginType) {
        //UI
        //gameObject.transform.DOMoveY(-2, .5f).SetEase(Ease.InSine);
        //gameObject.SetActive(false);
        Fade.gameObject.SetActive(true);
        Fade.DOFade(.6f, .2f).SetEase(Ease.InSine);
        
        Transform canvasTransform = GameObject.Find("Canvas").transform;
        GameObject loadingGameObj = Instantiate(LoadingCircle, canvasTransform.position, Quaternion.identity, canvasTransform);
        loadingGameObj.transform.DOScale(.2f, .5f).From();
        
        Connection.LoginType = loginType;        
        var session = await Connection.Session;
        Debug.LogFormat("Active Session: {0}", session);
        GlobalModel.Me = await Connection.Client.GetAccountAsync(session);
        Debug.LogFormat("Account id: {0}", GlobalModel.Me.User.Id);
        
        await Connection.Socket.ConnectAsync(session);
        ExitScene("MainMenu");
    }
}
