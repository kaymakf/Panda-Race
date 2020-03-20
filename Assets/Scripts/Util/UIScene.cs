using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIScene : MonoBehaviour
{
	public Image Fade;

	protected void EnterScene() {
		Fade.gameObject.SetActive(true);
		Fade.gameObject.layer = 1;
		var fadeAnim = Fade.DOFade(0f, .3f).SetEase(Ease.Linear);
		fadeAnim.onComplete = () => Fade.gameObject.SetActive(false);
		gameObject.transform.DOMoveY(-2f, .5f).From().SetEase(Ease.InSine);
	}

	protected void ExitScene(string targetScene) {
		Fade.gameObject.SetActive(true);
		Fade.gameObject.layer = 1;
		var fadeAnim = Fade.DOFade(1f, .3f).SetEase(Ease.Linear);
		fadeAnim.onComplete = () => SceneManager.LoadScene(targetScene);
		gameObject.transform.DOMoveY(-2f, .5f).SetEase(Ease.InSine);
	}

	protected void OpenDialog(GameObject dialog) {
        Fade.gameObject.SetActive(true);
        Fade.gameObject.layer = 0;
		transform.DOScale(0, .2f);
        dialog.SetActive(true);
        dialog.transform.DOScale(1f, .2f).From(.5f).SetEase(Ease.InSine);
        Fade.DOFade(.6f, .5f).SetEase(Ease.InSine);
    }

	protected void CloseDialog(GameObject dialog) {
        transform.DOScale(1f, .2f);
		Fade.gameObject.layer = 0;
        Fade.DOFade(0f, .5f).SetEase(Ease.Linear).onComplete = () => Fade.gameObject.SetActive(false);
		var scaleAnim = dialog.transform.DOScale(.2f, .2f).SetEase(Ease.InSine).onComplete = () => dialog.SetActive(false);
	}

    protected void ShowLoading(GameObject LoadingCircle) {
		gameObject.transform.DOScale(.2f, .2f).onComplete = () => {
			gameObject.SetActive(false);
			Transform canvasTransform = GameObject.Find("Canvas").transform;
			GameObject loadingGameObj = Instantiate(LoadingCircle, Vector3.right * 2.7f, Quaternion.identity, canvasTransform);
			loadingGameObj.transform.DOScale(.2f, .5f).From();
		};
	}

    protected void Bounce(GameObject o) {

    }

}
