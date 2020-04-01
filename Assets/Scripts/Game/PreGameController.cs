using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class PreGameController : UIScene {
    public GameObject GameplayContainer;
    //public GameObject GridContainer;
    public GameObject Ground;
    public GameObject PickAvatarDialog;
    public LevelGenerator Level;
    public TextMeshProUGUI[] Texts;
    public CameraFollowSetup CameraSetup;
    public GradientFollow BgFollow;

    public float CountDown = 5f;

    void Start() {
        GameplayContainer.SetActive(false);
        //GridContainer.SetActive(false);
        Ground.SetActive(false);
        EnterScene();
        OpenDialog(PickAvatarDialog);
        StartCoroutine(TimeCount());
    }

    private IEnumerator TimeCount() {
        yield return new WaitUntil(() => GlobalModel.MyCharacter != -1);
        Texts[GlobalModel.MyCharacter].text = GlobalModel.Me?.User.Username;
        Texts[(GlobalModel.MyCharacter + 1) % 2].text = GlobalModel.Opponent?.Username;

        while (CountDown >= 0) {
            Texts[2].text = ((int)CountDown).ToString();
            var countdownTransform = Texts[2].gameObject.transform;
            if (countdownTransform != null)
                countdownTransform.DOScale(.2f, .2f).From();
            CountDown -= 1;
            yield return new WaitForSeconds(1);
        }
        yield return new WaitUntil(() => Level.Ready);
        GameController.SendState(GameController.ACTION_READY, "");
        StartCoroutine(StartGameplay());
    }

    private IEnumerator StartGameplay() {
        yield return new WaitUntil(() => GlobalModel.OppenentReady);
        Fade.gameObject.SetActive(false);
        PickAvatarDialog.SetActive(false);
        //GridContainer.SetActive(true);
        Ground.SetActive(true);
        SetCharacterControllers();
        GameplayContainer.SetActive(true);
    }

    private void SetCharacterControllers() {
        var cat = GameplayContainer.transform.Find("Cat");
        var chick = GameplayContainer.transform.Find("Chick");
        if (GlobalModel.MyCharacter == GlobalModel.CHARACTER_CHICK) {
            chick.GetComponent<PlayerController>().enabled = true;
            chick.GetComponent<OpponentController>().enabled = false;
            cat.GetComponent<PlayerController>().enabled = false;
            cat.GetComponent<OpponentController>().enabled = true;
            CameraSetup.followTransform = chick;
            BgFollow.target = chick;
        }
        else {
            chick.GetComponent<PlayerController>().enabled = false;
            chick.GetComponent<OpponentController>().enabled = true;
            cat.GetComponent<PlayerController>().enabled = true;
            cat.GetComponent<OpponentController>().enabled = false;
            CameraSetup.followTransform = cat;
            BgFollow.target = cat;
        }
    }
}
