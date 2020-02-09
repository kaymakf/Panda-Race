using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class Auth : MonoBehaviour
{
    public GameObject LoadingCircle;
    public Image Fade;
    
    void Start() {
        //todo: animasyonlar daha düzenli hale getirilecek
        var fadeAnim = Fade.DOFade(1, .5f).From().SetEase(Ease.Linear);
        fadeAnim.onComplete = () => Fade.gameObject.SetActive(false);
        gameObject.transform.DOMoveY(-1, .5f).From().SetEase(Ease.InSine);
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
        
        //Connection
        ServerConnection.Instance.LoginType = loginType;
        
        var session = await ServerConnection.Instance.Session;
        Debug.LogFormat("Active Session: {0}", session);
        GlobalModel.Me = await ServerConnection.Instance.Client.GetAccountAsync(session);
        Debug.LogFormat("Account id: {0}", GlobalModel.Me.User.Id);
        
        await ServerConnection.Instance.Socket.ConnectAsync(session);
        SceneManager.LoadScene("MainMenu");
    }
}
