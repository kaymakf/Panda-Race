using System.Collections;
using TMPro;
using UnityEngine;

public class EndGameController : UIScene
{
    public GameObject EndGameDialog;
    public LevelGenerator Level;
    public Transform CameraTransform;
    public TextMeshProUGUI ResultText;

    public GameObject WinnerExplosionPrefab;
    public GameObject LoserExplosionPrefab;
    private GameObject ExplosionGameObject;

    int Winner = -1;

    private void Start() {
        StartCoroutine(SetFlagPosition());
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if (Winner != -1) return;
        DecideWinner(collision.gameObject.name);
        GlobalModel.GameFinished = true;
        OpenDialog(EndGameDialog, false);
        ExplosionGameObject = Instantiate(Winner == GlobalModel.MyCharacter ? WinnerExplosionPrefab : LoserExplosionPrefab, CameraTransform.position, Quaternion.identity);
        StartCoroutine(SetExplosionPosition());
        ResultText.text = Winner == GlobalModel.MyCharacter ? "You Won" : "You Lost";
    }

    void DecideWinner(string finishedName) {
        Winner = GlobalModel.CharacterId(finishedName);
        GameController.SendState(GameController.ACTION_GAME_OVER, Winner.ToString());
    }

    private IEnumerator SetFlagPosition() {
        yield return new WaitUntil(() => GlobalModel.GeneratedSplinePoints != null);
        (float, float) pos = GlobalModel.GeneratedSplinePoints[((Level.LevelLength - 50) / Level.PointFreq) - 1];
        transform.position = new Vector2(pos.Item1 - 15f, pos.Item2 - 9.5f);
    }

    private IEnumerator SetExplosionPosition() {
        while (ExplosionGameObject != null) {
            ExplosionGameObject.transform.position = CameraTransform.position;
            yield return new WaitForEndOfFrame();
        }
    }
}
