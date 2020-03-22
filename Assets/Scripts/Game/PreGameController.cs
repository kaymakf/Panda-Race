using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PreGameController : UIScene {
    public GameObject GameplayContainer;
    public GameObject GridContainer;
    public GameObject PickAvatarDialog;
    public TextMeshProUGUI[] Texts;

    public float CountDown = 5f;

    void Start() {
        GameplayContainer.SetActive(false);
        GridContainer.SetActive(false);
        EnterScene();
        OpenDialog(PickAvatarDialog);

        Texts[GlobalModel.MyCharacter].text = GlobalModel.Me?.User.Username;
        Texts[(GlobalModel.MyCharacter + 1) % 2].text = GlobalModel.Opponent?.Username;

        StartCoroutine(TimeCount());
    }

    private IEnumerator TimeCount() {
        while (CountDown >= 0) {
            CountDown -= 1;
            Texts[2].text = ((int)CountDown + 1).ToString();
            Texts[2].gameObject.transform.DOScale(.2f, .2f).From();
            Debug.Log(CountDown);
            yield return new WaitForSeconds(1);
        }
        Debug.Log("bitti");
        GameController.SendState(GameController.ACTION_READY, "");
        StartCoroutine(StartGameplay());
    }

    private IEnumerator StartGameplay() {
        yield return new WaitUntil(() => GlobalModel.OppenentReady);
        Fade.gameObject.SetActive(false);
        PickAvatarDialog.SetActive(false);
        GridContainer.SetActive(true);
        GameplayContainer.SetActive(true);
    }
}
