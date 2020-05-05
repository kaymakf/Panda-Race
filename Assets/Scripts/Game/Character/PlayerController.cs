using System.Collections;
using Nakama.TinyJson;
using UnityEngine;

public class PlayerController : CharacterController {

    void Start() {
        StartCoroutine(SendPosition());
    }

    void Update() {
        if (IsGrounded() && ((Input.GetAxisRaw("Vertical") > 0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)))
            jump = true;
    }

    void FixedUpdate() {
        if (jump) {
            rigidbody2d.velocity = Vector2.up * jumpVelocity;
            jump = false;
            GameController.SendState(
                GameController.ACTION_JUMP,
                (transform.position.x, transform.position.y).ToJson()
            );
        }
        HandleMovement();
        // todo:Set Animations
    }

    private IEnumerator SendPosition() {
        while (true) {
            yield return new WaitForSeconds(1f);
            GameController.SendState(
                GameController.ACTION_POSITION_UPDATE,
                (transform.position.x, transform.position.y).ToJson()
            );
        }
    }
}
