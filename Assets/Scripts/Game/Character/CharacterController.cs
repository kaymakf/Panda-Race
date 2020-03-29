using UnityEngine;

public class CharacterController : MonoBehaviour {
    public float moveSpeed = 7f;
    public float jumpVelocity = 14f;

    [SerializeField] protected LayerMask platformsLayerMask;
    protected Rigidbody2D rigidbody2d;
    protected BoxCollider2D boxCollider2d;

    protected ServerConnection Connection;

    public bool jump { get; set; }

    void Awake() {
        rigidbody2d = transform.GetComponent<Rigidbody2D>();
        boxCollider2d = transform.GetComponent<BoxCollider2D>();
    }

    void Start() {
        Connection = ServerConnection.Instance;
    }

    protected bool IsGrounded() {
        RaycastHit2D raycastHit2d = Physics2D.BoxCast(boxCollider2d.bounds.center, boxCollider2d.bounds.size, 0f, Vector2.down, 1f, platformsLayerMask);
        return raycastHit2d.collider != null;
    }

    protected void HandleMovement() {
        rigidbody2d.velocity = new Vector2(moveSpeed, rigidbody2d.velocity.y);
    }
}