using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EcoPlayer : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float moveSpeed = 6f;
    public float jumpForce = 15f;

    [Header("Suelo")]
    public Transform groundCheck;
    public float groundRadius = 0.18f;

    [Header("Estado")]
    public bool controlEnabled = true;
    public float invuln;

    Rigidbody2D rb;
    SpriteRenderer sr;
    EcoSpriteAnimator anim;
    Vector3 spawn;
    float moveX, coyote, jumpBuffer;
    bool grounded, facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<EcoSpriteAnimator>();
        spawn = transform.position;
    }

    public void SetCheckpoint(Vector3 p) { spawn = p; }

    void Update()
    {
        if (!controlEnabled || Time.timeScale == 0f) { moveX = 0f; if (anim != null) anim.speed = 0f; return; }

        moveX = EcoInput.Horizontal();
        if (EcoInput.JumpPressed()) jumpBuffer = 0.12f;

        // Flip del sprite (como en el taller: se invierte la escala X)
        if (moveX > 0 && !facingRight) Flip();
        else if (moveX < 0 && facingRight) Flip();

        if (anim != null)
        {
            anim.speed = Mathf.Abs(moveX);
            anim.airborne = !grounded;
            anim.rising = rb.linearVelocity.y > 0f;
        }

        if (invuln > 0f)
        {
            invuln -= Time.deltaTime;
            sr.enabled = ((int)(invuln * 14f)) % 2 == 0;
            if (invuln <= 0f) sr.enabled = true;
        }

        if (transform.position.y < -9f && EcoGame.Instance != null) EcoGame.Instance.Hurt(true);
    }

    void FixedUpdate()
    {
        grounded = CheckGround();
        coyote = grounded ? 0.1f : coyote - Time.fixedDeltaTime;
        jumpBuffer -= Time.fixedDeltaTime;

        Vector2 v = rb.linearVelocity;
        v.x = controlEnabled ? moveX * moveSpeed : 0f;
        if (controlEnabled && jumpBuffer > 0f && coyote > 0f)
        {
            v.y = jumpForce;
            jumpBuffer = 0f;
            coyote = 0f;
            EcoAudio.Play("jump");
        }
        rb.linearVelocity = v;
    }

    bool CheckGround()
    {
        Vector2 p = groundCheck != null ? (Vector2)groundCheck.position : (Vector2)transform.position;
        foreach (var h in Physics2D.OverlapCircleAll(p, groundRadius))
        {
            if (h.isTrigger || h.attachedRigidbody == rb) continue;
            return true;
        }
        return false;
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 s = transform.localScale;
        s.x *= -1f;
        transform.localScale = s;
    }

    public void Bounce(float factor = 0.7f)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * factor);
    }

    public void Respawn()
    {
        transform.position = spawn;
        rb.linearVelocity = Vector2.zero;
    }

    public float VerticalSpeed => rb.linearVelocity.y;
}
