using System.Collections;
using System.Collections.Generic;
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

        Texts[GlobalModel.MyCharacter].text = GlobalModel.Me.User.Username;
        Texts[(GlobalModel.MyCharacter + 1) % 2].text = GlobalModel.Opponent.Username;
    }

    void Update() {
        CountDown -= Time.deltaTime;
        Texts[2].text = ((int)CountDown + 1).ToString();
        if (CountDown < 0)
            StartGameplay();
    }

    private void StartGameplay() {
        //CloseDialog(PickAvatarDialog);
        Fade.gameObject.SetActive(false);
        PickAvatarDialog.SetActive(false);
        GridContainer.SetActive(true);
        GameplayContainer.SetActive(true);
    }
}
