using UnityEngine;

public class UnityChanController : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private float moveForce = 10;
    [SerializeField] private float maxAbsMoveVelocity = 2;
    [SerializeField] private float jumpVelocity = 5;

    private float _horInput;
    private bool _isGrounded;
    
    public float HorAbsInput => Mathf.Abs(_horInput);
    public float VerSpeed => _rigidbody2D.velocity.y;
    public bool JumpTriggered { get; private set; }
    public bool GroundedTriggered { get; private set; }
    public bool FireTriggered { get; private set; }

    void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        _horInput = Input.GetAxisRaw("Horizontal");
        JumpTriggered = _isGrounded && Input.GetButtonDown("Jump");
        FireTriggered = Input.GetButtonDown("Fire1");

        if (_horInput != 0)
            _spriteRenderer.flipX = _horInput < 0;
        
        if (JumpTriggered)
        {
            _isGrounded = false;
            _rigidbody2D.AddForce(Vector2.up * jumpVelocity, ForceMode2D.Impulse);
        }

        if (FireTriggered)
        {
            // 実際はよしなに弾を発射する
        }
    }

    void FixedUpdate()
    {
        if (_horInput != 0)
            _rigidbody2D.AddForce(_horInput * moveForce * Vector2.right);

        if (Mathf.Abs(_rigidbody2D.velocity.x) > maxAbsMoveVelocity)
        {
            var velSign = Mathf.Sign(_rigidbody2D.velocity.x);
            var newVel = new Vector2(velSign * maxAbsMoveVelocity, _rigidbody2D.velocity.y);
            _rigidbody2D.velocity = newVel;
        }
    }

    void OnCollisionEnter2D()
    {
        _isGrounded = true;
        GroundedTriggered = true;
    }

    public void ResetTriggers()
    {
        GroundedTriggered = false;
        JumpTriggered = false;
        FireTriggered = false;
    }
}
