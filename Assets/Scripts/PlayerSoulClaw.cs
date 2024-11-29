using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoulClaw : MonoBehaviour {
    private PlayerController player;
    public Transform groundCheck;
    private SoundManager soundManager;
    private Animator anim;
    private SpriteRenderer sr;
    private bool touchingGround = true;
    private bool touchingWall = false;
    private bool touchingBorder = false;
    private bool hasPlayedSound = false;
    public float groundCheckHeight;
    public float groundCheckWidth;
    public float wallCheckLength;
    public float wallCheckHeightOffset;
    public float animationTimer;
    public float animationTimerSet;
    public int facingDirX = 1;
    public LayerMask ground;
    public LayerMask borders;

    private void Start() {
        transform.SetParent(null, true);
        animationTimer = animationTimerSet;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        sr = GetComponent<SpriteRenderer>();
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        gameObject.SetActive(false);
    }

    void OnEnable() {
        animationTimer = animationTimerSet;
        GetComponent<SpriteRenderer>().enabled = false;
        hasPlayedSound = false;
    }

    void FixedUpdate() {
        touchingGround = Physics2D.OverlapBox(groundCheck.position, new Vector2(groundCheckWidth, groundCheckHeight), 0, ground);
        touchingWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallCheckHeightOffset), transform.right * facingDirX, wallCheckLength, ground);
        touchingBorder = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallCheckHeightOffset), transform.right * facingDirX, wallCheckLength, borders);
        if(touchingGround && !touchingWall) {
            sr.enabled = true;
            player.isShootingSoulClaw = true;
            player.anim.SetBool("isShootingSoulClaw", true);
            if(!hasPlayedSound) {
                soundManager.PlayClip(soundManager.PlayerSoulClaw, transform, 1);
                hasPlayedSound = true;
            }
        }
        if(!touchingGround || touchingWall) {
            gameObject.SetActive(false);
            player.isShootingSoulClaw = false;
            player.anim.SetBool("isShootingSoulClaw", false);
        }
    }

    void Update() {
        animationTimer -= Time.deltaTime;
        if(animationTimer <= 0) {
            gameObject.SetActive(false);
            player.isShootingSoulClaw = false;
            player.anim.SetBool("isShootingSoulClaw", false);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireCube(groundCheck.GetComponent<Transform>().position, new Vector3(groundCheckWidth, groundCheckHeight, 0.25f));
        //Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + wallCheckHeightOffset), new Vector3(transform.position.x + wallCheckLength, transform.position.y + wallCheckHeightOffset, transform.position.z));
    }
}
