using System.Collections;
using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;

public class PlayerController : CharacterController
{
    void Update() {
        if (/*IsGrounded() && */((Input.GetAxisRaw("Vertical") > 0) || (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)))
            jump = true;
    }

    void FixedUpdate() {
        if (jump) {
            GameController.SendState(
                GameController.ACTION_JUMP,
                new Dictionary<char, float>() {
                    {'x', transform.position.x},
                    {'y', transform.position.y}
                }.ToJson()
            );

            rigidbody2d.velocity = Vector2.up * jumpVelocity;
            jump = false;
        }
        HandleMovement();
        // todo:Set Animations
    }
}
