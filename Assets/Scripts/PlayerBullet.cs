using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour {
    private PlayerController player;
    private Animator anim;
    public float movementSpeed;
    private float originalMovementSpeed;
    public int bulletType = 1;
    public float damageDealt;
    private float flatDirX = 0;
    public float offsetX = 0.45f;
    public float dirX = 0;
    public float dirY = 0;
    public bool fadingOut = false;
    private float fadeOutSpeed = 0.1f;
    public bool stopped = false;
    public bool piercing = false;
    public bool transitionComplete = false;
    public bool transitionCompleteTimerCheck = false;
    private float transitionCompleteMovementSpeedMultiplier = 1.75f;
    //the speed multiplier these timers are for and the one for shooting while moving fast are different
    private float speedMultiplierTimer;
    private float speedMultiplierTimerSet = 0.15f;
    private bool speedMultiplierCheck;
    public float animationTimer;
    public float animationTimerSet = 0.15f;
    private float bulletExitYfollowTimer;
    public float bulletExitYfollowTimerSet = 0.25f;
    private bool enemyIsAtFullHealth;
    public float fadeOutTimer;
    public float fadeOutTimerSet = 0.05f;
    public float fadeOutTimerSet2 = 0.05f;

    void Start() {
        anim = GetComponent<Animator>();
    }

    void FixedUpdate() {
        animationTimer -= Time.deltaTime;
        speedMultiplierTimer -= Time.deltaTime;
        bulletExitYfollowTimer -= Time.deltaTime;
        if(fadingOut) {
            fadeOutTimer -= Time.deltaTime;
            if(fadeOutTimer <= 0) {
                fadeOutTimer = fadeOutTimerSet;
                GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, GetComponent<SpriteRenderer>().color.a - fadeOutSpeed);
            }
            if(GetComponent<SpriteRenderer>().color.a <= 0) {
                gameObject.SetActive(false);
            }
        }
        if(player != null) {
            if(player.isDashJumping2) {
                if(bulletExitYfollowTimer > 0) {
                    movementSpeed = originalMovementSpeed * 1.5f;
                }
            }
            if(!player.isDashJumping2) {
                if(bulletExitYfollowTimer > 0) {
                    movementSpeed = originalMovementSpeed;
                }
            }
            if(bulletExitYfollowTimer > 0 && (player.isDashJumping2 || player.GetComponent<Rigidbody2D>().velocity.y < -22 || player.GetComponent<Rigidbody2D>().velocity.y > 22)) {
                transform.position = new Vector2(transform.position.x, player.transform.position.y);
            }
        }
        if(animationTimer <= 0) {
            if(player != null && !transitionComplete) {
                dirX = player.wallFacingDirX * (movementSpeed / 50);
                flatDirX = player.wallFacingDirX;
            }
            if(!transitionCompleteTimerCheck) {
                transform.position = new Vector2(transform.position.x + (offsetX * flatDirX), transform.position.y);
                transitionCompleteTimerCheck = true;
            }
            transitionComplete = true;
        }
        if(animationTimer > 0) {
            transitionComplete = false;
        }
        if(speedMultiplierTimer <= 0) {
            transitionCompleteMovementSpeedMultiplier = 1;
        }
        if(player != null && !transitionComplete) {
            if(player.touchingWall && player.isFalling && !player.touchingGround) {
                anim.SetFloat("transitionBlend", 1);
            }
            if(!player.touchingWall) {
                anim.SetFloat("transitionBlend", 0);
            }
            transform.position = player.transform.position;
        }
        if(transitionComplete) {
            if(!stopped) {
                transform.position = new Vector2(((dirX * 0.0125f) * transitionCompleteMovementSpeedMultiplier) + transform.position.x, transform.position.y);
            }
            if(dirX > 0) {
                transform.localScale = new Vector2(1, 1);
            }
            if(dirX < 0) {
                transform.localScale = new Vector2(-1, 1);
            }
            if(speedMultiplierCheck) {
                speedMultiplierTimer = speedMultiplierTimerSet;
                transitionCompleteMovementSpeedMultiplier = 1.75f;
                speedMultiplierCheck = false;
            }
        }
        anim.SetBool("transitionComplete", transitionComplete);
    }

    void LateUpdate() {
        if(player != null && !transitionComplete) {
            transform.position = player.transform.position;
        }
    }

    public void Shoot(PlayerController playerScript, int equippedBulletType, float movementSpeedSet) {
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
        anim = GetComponent<Animator>();
        player = playerScript;
        bulletType = equippedBulletType;
        movementSpeed = movementSpeedSet;
        originalMovementSpeed = movementSpeed;
        damageDealt = 1.5f;
        transitionCompleteMovementSpeedMultiplier = 1.75f;
        speedMultiplierCheck = true;
        if(playerScript.respawned) {
            gameObject.SetActive(false);
        }
        flatDirX = playerScript.wallFacingDirX;
        dirX = playerScript.wallFacingDirX * (movementSpeed / 50);
        bulletExitYfollowTimer = bulletExitYfollowTimerSet;
        fadingOut = false;
        stopped = false;
        fadeOutTimer = fadeOutTimerSet;
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Border")) {
            gameObject.SetActive(false);
        }
        if(collision.gameObject.CompareTag("Level")) {
            stopped = true;
            fadingOut = true;
            fadeOutSpeed = 0.3f;
            fadeOutTimer = fadeOutTimerSet;
        }
        if(collision.gameObject.CompareTag("Enemies") && !stopped) {
            if(collision.GetComponent<Enemy>().canTakeDamage) {
                enemyIsAtFullHealth = false;
                if(collision.GetComponent<Enemy>().health == collision.GetComponent<Enemy>().maxHealth) {
                    enemyIsAtFullHealth = true;
                }
                collision.gameObject.GetComponent<Enemy>().Hit(damageDealt);
                if(collision.GetComponent<Rigidbody2D>().velocity.x < 0.2f && collision.GetComponent<Rigidbody2D>().velocity.x > -0.2f) {
                    if(collision.GetComponent<Rigidbody2D>().velocity.y < 0.2f && collision.GetComponent<Rigidbody2D>().velocity.y > -0.2f) {
                        player.style += 2;
                    }
                }
                if(collision.GetComponent<Enemy>().beingKnockedBack) {
                    player.style += 4;
                }
                player.style += 1;
                player.ResetStyleDeductionTimer();
                if(collision.GetComponent<Enemy>().health <= 0) {
                    player.style += 1;
                    if(enemyIsAtFullHealth) {
                        player.style += 1;
                        player.sp += 1;
                    }
                }
                if(!piercing) {
                    stopped = true;
                    fadingOut = true;
                    fadeOutSpeed = 0.35f;
                    fadeOutTimer = fadeOutTimerSet2;
                }
                player.sp += 1;
                if(player.isDashing2) {
                    player.sp += 1;
                }
            }
        }
    }
}
