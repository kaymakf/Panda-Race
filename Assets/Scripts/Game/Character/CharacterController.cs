using UnityEngine;

public class CharacterController : MonoBehaviour {
    public float moveSpeed = 7f;
    public float jumpVelocity = 14f;
    public float maxMoveSpeed = 16f;
    public float moveSpeedIncrement = .01f;

    public LayerMask platformsLayerMask;
    protected Rigidbody2D rigidbody2d;
    protected BoxCollider2D boxCollider2d;
    protected Animator runAnim;

    protected ServerConnection Connection;
    private Vector2 velocityVector = new Vector2();

    public bool jump { get; set; }

    void Awake() {
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
        runAnim = transform.GetComponent<Animator>();
    }

    void Start() {
        Connection = ServerConnection.Instance;
    }

    protected bool IsGrounded() {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 1f, platformsLayerMask);
        return raycastHit2d.collider != null;
    }

    protected void HandleMovement() {
        velocityVector.Set(moveSpeed, rigidbody2d.velocity.y);
        rigidbody2d.velocity = velocityVector;
        AdjustSpeed();
    }

    private void AdjustSpeed() {
        if (GlobalModel.GameFinished)
            moveSpeed -= moveSpeed > 0.5f ? moveSpeedIncrement * 8f : 0;
        else if (moveSpeed < maxMoveSpeed)
            moveSpeed += moveSpeedIncrement;
        runAnim.SetFloat("RunSpeed", moveSpeed / 21f + .7f);
    }
}