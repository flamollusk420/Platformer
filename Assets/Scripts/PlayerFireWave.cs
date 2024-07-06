using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFireWave : MonoBehaviour {
    public Transform groundCheck;
    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool touchingGround = true;
    private bool touchingWall = false;
    private bool touchingBorder = false;
    public float movementSpeed;
    public float groundCheckHeight;
    public float groundCheckWidth;
    public float wallCheckLength;
    public float wallCheckHeightOffset;
    public float animationTimer;
    public float animationTimerSet;
    public float animationTimer2;
    public float animationTimerSet2;
    public float animationTimer3;
    public float animationTimerSet3;
    private bool animationTimer3Check;
    public int facingDirX = 1;
    public LayerMask ground;
    public LayerMask borders;

    void OnEnable() {
        transform.SetParent(null, true);
        animationTimer3Check = false;
        animationTimer = animationTimerSet;
        animationTimer2 = animationTimerSet2;
        animationTimer3 = animationTimerSet2 + animationTimerSet3;
        anim = GetComponent<Animator>();
        anim.SetBool("touchingGround", false);
        anim.SetBool("attackTransitionComplete", false);
        anim.SetBool("attackComplete", false);
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate() {
        touchingGround = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0, ground);
        touchingWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallCheckHeightOffset), transform.right * facingDirX, wallCheckLength, ground);
        touchingBorder = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallCheckHeightOffset), transform.right * facingDirX, wallCheckLength, borders);
        if(touchingGround && !touchingWall) {
            rb.velocity = new Vector2(movementSpeed * facingDirX, 0);
            anim.SetBool("touchingGround", true);
        }
        if(!touchingGround || touchingWall || touchingBorder) {
            rb.velocity = new Vector2(0, 0);
            if(animationTimer2 > 0) {
                animationTimer2 = 0;
            }
            if(!animationTimer3Check && animationTimer <= 0) {
                animationTimer3 = animationTimerSet3;
                animationTimer3Check = true;
            }
        }
        if(!sr.isVisible) {
            gameObject.SetActive(false);
        }
    }

    void Update() {
        animationTimer -= Time.deltaTime;
        animationTimer2 -= Time.deltaTime;
        animationTimer3 -= Time.deltaTime;
        if(animationTimer <= 0) {
            anim.SetBool("attackTransitionComplete", true);
        }
        if(animationTimer2 <= 0) {
            anim.SetBool("attackComplete", true);
        }
    }

    private void LateUpdate() {
        if(animationTimer3 <= 0) {
            anim.SetBool("touchingGround", false);
            anim.SetBool("attackTransitionComplete", false);
            anim.SetBool("attackComplete", false);
            gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(groundCheck.GetComponent<Transform>().position, new Vector3(groundCheckWidth, groundCheckHeight, 0.25f));
        //Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + wallCheckHeightOffset), new Vector3(transform.position.x + wallCheckLength, transform.position.y + wallCheckHeightOffset, transform.position.z));
    }
}
