/*using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpHeight = 5f;
    float direction = 0;
    bool isGrounded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move(direction);
    }

    void OnMove(InputValue value)
    {
        float v = value.Get<float>();
        direction = v;
    }

    void Move(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }
    void OnJump()
    {
        if(isGrounded)
            Jump();
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        
    }

    void OnCollisionStay2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Ground")) {
            isGrounded = false;
            for(int i = 0; i < collision.contactCount; i++) {
                ContactPoint2D contact = collision.GetContact(i);
                if(Vector2.Angle(collision.GetContact(i).normal, Vector2.up) < 45f) {
                    isGrounded = true;
                }
            }
        }
    }


    void OnCollisionExit2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Ground")) 
            isGrounded = false;
    }
}
*/

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 5f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] float dashSpeed = 15f;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 1f;

    float direction = 0;
    float lastNonZeroDirection = 1f;
    bool isGrounded = false;
    bool canDoubleJump = false;
    bool isDashing = false;
    bool canDash = true;
    float dashTimeLeft = 0f;
    float dashCooldownTimeLeft = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isDashing)
        {
            Move(direction);
        }

        if (isDashing)
        {
            dashTimeLeft -= Time.deltaTime;
            if (dashTimeLeft <= 0)
            {
                isDashing = false;
                rb.gravityScale = 1f;
            }
        }

        if (!canDash)
        {
            dashCooldownTimeLeft -= Time.deltaTime;
            if (dashCooldownTimeLeft <= 0)
            {
                canDash = true;
            }
        }
    }

    void OnMove(InputValue value)
    {
        float v = value.Get<float>();
        direction = v;

        if (v != 0)
        {
            lastNonZeroDirection = v;
        }
    }

    void Move(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
    }

    void OnJump()
    {
        if (isGrounded)
        {
            Jump();
            canDoubleJump = true;
        }
        else if (canDoubleJump)
        {
            Jump();
            canDoubleJump = false;
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
    }

    void OnDash()
    {
        if (canDash && !isDashing)
        {
            StartDash();
        }
    }

    void StartDash()
    {
        isDashing = true;
        canDash = false;
        dashTimeLeft = dashDuration;
        dashCooldownTimeLeft = dashCooldown;

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(lastNonZeroDirection * dashSpeed, 0f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing && collision.gameObject.CompareTag("Ground"))
        {
            isDashing = false;
            rb.gravityScale = 1f;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint2D contact = collision.GetContact(i);
                if (Vector2.Angle(collision.GetContact(i).normal, Vector2.up) < 45f)
                {
                    isGrounded = true;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}