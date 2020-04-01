using UnityEngine;

public class OpponentController : CharacterController {
    private Vector3 newPosition = new Vector3();

    void FixedUpdate() {
        if (GameController.recievedJumps.Count > 0) {
            SetPosition(GameController.recievedJumps.Dequeue());
            rigidbody2d.velocity = Vector2.up * jumpVelocity;
        }

        if (GameController.recievedPositions.Count > 0)
            SetPosition(GameController.recievedPositions.Dequeue());

        HandleMovement();
    }

    private void SetPosition((float, float) state) {
        newPosition.Set(state.Item1, state.Item2, transform.position.z);
        transform.position = newPosition;
    }
}
