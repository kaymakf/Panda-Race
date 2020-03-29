using System.Collections;
using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;

public class OpponentController : CharacterController {
    void FixedUpdate() {
        if (GameController.recievedActions.Count > 0) {
            var state = GameController.recievedActions.Dequeue();
            switch (state.OpCode) {
                case GameController.ACTION_JUMP:
                    var enc = System.Text.Encoding.UTF8;
                    var content = enc.GetString(state.State);
                    var c = content.FromJson<Dictionary<string, float>>();
                    Debug.Log(c);
                    transform.position = new Vector3(c["x"], c["y"], transform.position.z);
                    rigidbody2d.velocity = Vector2.up * jumpVelocity;
                    break;
            }
        }
        HandleMovement();
    }
}
