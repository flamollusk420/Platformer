using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {
    private SoundManager soundManager;
    private ShootingEnemy shootingEnemy;
    private Enemy enemyScript;
    private PlayerController player;
    private Animator anim;
    private FlexibleAnimation flexAnim;
    public float movementSpeed;
    private float originalMovementSpeed;
    public int damageDealt;
    public int timesParried;
    public float offsetX = 0.45f;
    public float enemyScriptOffsetY;
    public float dirX = 0;
    public float dirY = 0;
    private float flatDirX = 0;
    private float flatDirY = 0;
    private float flatDirX2 = 0;
    private float flatDirY2 = 0;
    public bool canBeDestroyedByPlayer = true;
    public bool canBeDeflected = true;
    public bool parryable = true;
    public bool hasBeenParried = false;
    public bool fadingOut = false;
    private float fadeOutSpeed = 0.1f;
    public bool stopped = false;
    public bool piercing = false;
    public bool transitionComplete = false;
    public bool transitionCompleteTimerCheck = false;
    private float transitionCompleteMovementSpeedMultiplier = 1.75f;
    private float speedMultiplierTimer;
    private float speedMultiplierTimerSet = 0.15f;
    private bool speedMultiplierCheck;
    private bool canTouchLevel;
    private bool staysWithEnemyBeforeBeingShot;
    private bool flipBulletX;
    private bool flipBulletY;
    public float animationTimer;
    public float animationTimerSet = 0.15f;
    public float deflectionTimer;
    public float fadeOutTimer;
    public float fadeOutTimerSet = 0.05f;

    void Start() {
        anim = GetComponent<Animator>();
        staysWithEnemyBeforeBeingShot = false;
    }

    void FixedUpdate() {
        animationTimer -= Time.deltaTime;
        deflectionTimer -= Time.deltaTime;
        if(animationTimer <= 0) {
            if(!transitionCompleteTimerCheck) {
                transform.position = new Vector2(transform.position.x + (offsetX * flatDirX), transform.position.y);
                transitionCompleteTimerCheck = true;
            }
            transitionComplete = true;
            if(dirX > 0) {
                transform.localScale = new Vector2(1, 1);
            }
            if(dirX < 0) {
                transform.localScale = new Vector2(-1, 1);
            }
            if(flipBulletX) {
                transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            }
            if(flipBulletY) {
                transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y * -1);
            }
        }
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
        if(deflectionTimer <= 0 && parryable) {
            canBeDeflected = true;
        }
        if(animationTimer > 0) {
            transitionComplete = false;
        }
        if(speedMultiplierTimer <= 0) {
            transitionCompleteMovementSpeedMultiplier = 1;
        }
        if(transitionComplete) {
            if(!stopped) {
                transform.position = new Vector2(((dirX * 0.0125f) * transitionCompleteMovementSpeedMultiplier) + transform.position.x, transform.position.y);
            }
            if(speedMultiplierCheck) {
                speedMultiplierTimer = speedMultiplierTimerSet;
                transitionCompleteMovementSpeedMultiplier = 1.75f;
                speedMultiplierCheck = false;
            }
        }
        if(anim != null) {
            anim.SetBool("transitionComplete", transitionComplete);
        }
    }

    void LateUpdate() {
        if(shootingEnemy != null && !transitionComplete && staysWithEnemyBeforeBeingShot) {
            transform.position = new Vector2(transform.position.x + (offsetX * flatDirX), transform.position.y + enemyScriptOffsetY);
        }
    }

    public void Shoot(float dirXset, float dirYset, int damageDealtSet, float movementSpeedSet, bool flipBulletXset, bool flipBulletYset, bool staysWithEnemyBeforeBeingShotSet, bool hasCustomFrameList, bool canTouchLevelSet, List<Sprite> frameListSet, float customTimeBetweenFramesSet, ShootingEnemy shootingEnemyScript, Enemy enemyScriptSet, bool hasCustomBoxColliderShapeSet, BoxCollider2D customBoxColliderShapeSet, float enemyScriptOffsetYSet) {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
        staysWithEnemyBeforeBeingShot = staysWithEnemyBeforeBeingShotSet;
        canTouchLevel = canTouchLevelSet;
        fadingOut = false;
        hasBeenParried = false;
        stopped = false;
        timesParried = 0;
        enemyScriptOffsetY = enemyScriptOffsetYSet;
        shootingEnemy = shootingEnemyScript;
        enemyScript = enemyScriptSet;
        if(hasCustomBoxColliderShapeSet) {
            GetComponent<BoxCollider2D>().size = customBoxColliderShapeSet.size;
            GetComponent<BoxCollider2D>().offset = customBoxColliderShapeSet.offset;
            GetComponent<BoxCollider2D>().sharedMaterial = customBoxColliderShapeSet.sharedMaterial;
        }
        if(GetComponent<Animator>() != null) {
            anim = GetComponent<Animator>();
        }
        if(GetComponent<FlexibleAnimation>() != null) {
            flexAnim = GetComponent<FlexibleAnimation>();
            if(hasCustomFrameList) {
                flexAnim.frameList = frameListSet;
                flexAnim.numberOfFrames = frameListSet.Count - 1;
                flexAnim.timeUntilNextFrame = customTimeBetweenFramesSet;
            }
            GetComponent<SpriteRenderer>().sprite = flexAnim.frameList[0];
        }
        if(hasCustomFrameList && GetComponent<FlexibleAnimation>() == null) {
            Debug.Log("No FlexibleAnimation component to assign custom frames to");
        }
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        damageDealt = damageDealtSet;
        movementSpeed = movementSpeedSet;
        originalMovementSpeed = movementSpeed;
        transitionCompleteMovementSpeedMultiplier = 1.75f;
        speedMultiplierCheck = true;
        if(player.respawned) {
            gameObject.SetActive(false);
        }
        flipBulletX = flipBulletXset;
        flipBulletY = flipBulletYset;
        flatDirX = dirXset;
        flatDirY = dirYset;
        flatDirX2 = dirXset / Mathf.Abs(dirXset);
        flatDirY2 = dirYset / Mathf.Abs(dirYset);
        dirX = flatDirX * (movementSpeed / 50);
        dirY = flatDirY * (movementSpeed / 50);
    }

    public void Deflect() {
        if(canBeDeflected && parryable) {
            deflectionTimer = 0.5f;
            canBeDeflected = false;
            hasBeenParried = true;
            timesParried += 1;
            bool dirXflipped = false;
            bool dirYflipped = false;
            if(player.wallFacingDirX == flatDirX2 * -1) {
                dirX *= -2;
                dirY *= -1;
                dirXflipped = true;
                dirYflipped = true;
                player.style += 2;
            }
            if(player.wallFacingDirX == flatDirX2) {
                dirX *= 3;
                dirY *= -1;
                dirYflipped = true;
                player.style += 1;
            }
            if(dirXflipped) {
                flatDirX2 *= -1;
            }
            if(dirYflipped) {
                flatDirY2 *= -1;
            }
            if(!canBeDestroyedByPlayer) {
                player.style += 1;
            }
            player.sp += 1;
            soundManager.PlayClip(soundManager.Parry, transform, 1);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Border")) {
            gameObject.SetActive(false);
        }
        if(collision.CompareTag("Level") && !canTouchLevel) {
            stopped = true;
            fadingOut = true;
            fadeOutSpeed = 0.3f;
            fadeOutTimer = fadeOutTimerSet;
        }
        if(collision.CompareTag("PlayerSecondaryCollider") && player != null && !stopped) {
            if(!player.recoiling && !player.isExploding && gameObject.GetComponent<EnemyBullet>().enabled == true) {
                if(!player.isDamaging && !player.isDashJumping && player.exitDashTimer <= 0 && player.exitDownDashTimer <= 0 && player.exitDownDashTimer <= 0 ) {
                    if(damageDealt > 0) {
                        player.Hit(damageDealt);
                    }
                    player.style -= damageDealt * 2;
                    player.ResetStyleDeductionTimer();
                    stopped = true;
                    fadingOut = true;
                    fadeOutSpeed = 0.35f;
                    fadeOutTimer = fadeOutTimerSet;
                }
            }
        }
        if(collision.CompareTag("PlayerSecondaryCollider") && player != null && !stopped) {
            if(!player.recoiling && player.isExploding || player.playerDashCollider.canDoDamage && gameObject.GetComponent<EnemyBullet>().enabled == true) {
                player.style += 1;
                stopped = true;
                fadingOut = true;
                fadeOutSpeed = 0.35f;
                fadeOutTimer = fadeOutTimerSet;
                soundManager.PlayClip(soundManager.BulletDestroyedByDash, transform, 1);
            }
        }
        if(collision.CompareTag("PlayerBullet")) {
            if(!collision.GetComponent<PlayerBullet>().fadingOut && !collision.GetComponent<PlayerBullet>().stopped && !fadingOut && !stopped) {
                stopped = true;
                fadingOut = true;
                fadeOutSpeed = 0.1f;
                fadeOutTimer = fadeOutTimerSet;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
                collision.GetComponent<PlayerBullet>().stopped = true;
                collision.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0.5f);
                collision.GetComponent<PlayerBullet>().fadingOut = true;
                collision.GetComponent<PlayerBullet>().fadeOutTimer = collision.GetComponent<PlayerBullet>().fadeOutTimerSet;
                soundManager.PlayClip(soundManager.BulletCollision, transform, 1);
                FlexibleAnimation explosion = BulletCollisionObjectPool.instance.GetPooledObject().GetComponent<FlexibleAnimation>();
                explosion.gameObject.SetActive(true);
                explosion.currentFrame = 0;
                explosion.transform.position = Vector3.Lerp(transform.position, collision.transform.position, 1);
                player.style += 2;
                if(player.isDashJumping2) {
                    player.style += 1;
                }
                player.sp += 1;
            }
        }
        if(collision.CompareTag("Enemies")) {
            if(hasBeenParried && !fadingOut && !stopped) {
                bool canOneShot = false;
                if(collision.GetComponent<Enemy>() != null && collision.GetComponent<Enemy>().canTakeDamage) {
                    if(collision.GetComponent<Enemy>() == enemyScript) {
                        player.style += 2;
                    }
                    if(collision.GetComponent<Enemy>().health == collision.GetComponent<Enemy>().maxHealth) {
                        canOneShot = true;
                    }
                    if(collision.GetComponent<Enemy>().health - ((damageDealt * 1.5f) * timesParried) <= 0) {
                        player.style += 2;
                        if(canOneShot) {
                            player.style += 2;
                        }
                        soundManager.PlayClip(soundManager.ParriedBulletDeath, transform, 1);
                    }
                    if(collision.GetComponent<Enemy>().beingKnockedBack) {
                        player.style += 7;
                    }
                    player.sp += 1;
                    player.style += timesParried * timesParried;
                    collision.GetComponent<Enemy>().Hit((damageDealt * 1.5f) * timesParried);
                    fadingOut = true;
                    fadeOutSpeed = 0.35f;
                    stopped = true;
                }
            }
        }
    }
}
