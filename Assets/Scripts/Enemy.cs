using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private SoundManager soundManager;
    //private MusicManager musicManager;
    private SpriteRenderer sr;
    private Animator anim;
    private Transform playerTR;
    private PlayerController player;
    private Rigidbody2D rb;
    public GameObject objectToModify;
    public GameObject hostEnemy;
    public AudioClip customDeathSound;

    public float customDeathSoundVolume = 1;
    public float effectTimer;
    public float effectCooldown = 0.15f;
    [HideInInspector]
    public float deathJumpTimer;
    public float deathJumpCooldown = 20;
    public float deathJumpStrength = 65;
    public bool invincible;
    public bool hitEffectWhileInvincible;
    private bool invincibleSet;
    private float invincibilityTimer;
    public float invincibilityTimerSet;
    [HideInInspector]
    public float initialNonDamageTimer;
    public float initialNonDamageTimerSet = 0.2f;
    [HideInInspector]
    public float knockbackTimer;
    public float knockbackTimerSet = 1.25f;
    public float health = 3;
    public float maxHealth = 3;
    public int damageDealt = 1;
    public int enemySize = 1;
    public int SFXimportance = 1;
    public int deathAnimImportance = 1;
    public float explosionOffsetX;
    public float explosionOffsetY;
    public float defaultGravity = 4;
    public float startingX = 0;
    public float startingY = 0;
    public bool setAnimJumpBoolOnDeathJump = false;
    public bool hasHitEffect = true;
    public bool onlySpawnIfDisabled = false;
    public bool hasCustomDeathSound = false;
    public bool canBeKnockedBack = true;
    public bool beingKnockedBack;
    [HideInInspector]
    public bool canTakeDamage = true;
    public float knockbackStrengthX = 15;
    public float knockbackStrengthY = 15;
    public bool dontKillEnemy = false;
    public bool dontResetColor = false;
    public bool immuneToDamageAreas = false;
    private bool hasBeenHitByExplosion;
    private bool hitByLeftWave;
    private bool hitByRightWave;
    private float fastKillTimer;
    private bool startCompleted;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        if(!dontKillEnemy) {
            gameObject.SetActive(false);
        }
        if(GetComponent<Animator>() != null) {
            anim = GetComponent<Animator>();
        }
        invincibleSet = invincible;
        defaultGravity = rb.gravityScale;
        startCompleted = true;
    }

    void OnEnable() {
        sr = GetComponent<SpriteRenderer>();
        if(health <= 0) {
            health = maxHealth;
        }
        if(rb != null) {
            rb.gravityScale = defaultGravity;
        }
        if(!dontResetColor && initialNonDamageTimerSet == 0 && sr != null) {
            sr.color = new Color(255, 255, 255, 1);
        }
        initialNonDamageTimer = initialNonDamageTimerSet;
        if(initialNonDamageTimerSet != 0) {
            effectTimer = initialNonDamageTimerSet;
            sr.color = new Color(255, 255, 255, 0.65f);
        }
        fastKillTimer = 5;
        if(startCompleted) {
            invincible = invincibleSet;
        }
        if(invincibilityTimerSet != 0) {
            invincible = true;
            invincibilityTimer = invincibilityTimerSet;
        }
    }

    void FixedUpdate() {
        HealthCheck();
        UpdateEffect();
        if(player.respawned) {
            gameObject.SetActive(false);
        }
        initialNonDamageTimer -= Time.deltaTime;
        fastKillTimer -= Time.deltaTime;
        invincibilityTimer -= Time.deltaTime;
    }

    private void HealthCheck() {
        deathJumpTimer -= Time.deltaTime;
        if(health <= 0) {
            CreateDeathEffect();
            if(deathJumpTimer <= 0) {
                player.style += enemySize;
                player.ResetStyleDeductionTimer();
                if(rb.velocity.y < -1 || rb.velocity.y > 1) {
                    player.style += 2;
                    if(player.isUpDashing) {
                        player.style += 3;
                    }
                    if(player.isMeleeAttacking) {
                        player.style += 2;
                    }
                }
                if(!player.touchingGround) {
                    if(player.jumpsLeft == 0) {
                        player.jumpsLeft = 1;
                    }
                    player.dashesLeft = 2;
                    player.canUpDash = true;
                    player.sp += 2;
                }
                if(fastKillTimer > 0) {
                    player.style += 1;
                    if(fastKillTimer > 2) {
                        player.style += 1;
                    }
                    if(fastKillTimer > 4) {
                        player.style += 2;
                    }
                }
                gameObject.SetActive(false);
            }
        }
        if(health > maxHealth) {
            health = maxHealth;
        }
    }

    private void CreateDeathEffect() {
        beingKnockedBack = false;
        if(setAnimJumpBoolOnDeathJump) {
            anim.SetBool("isJumping", true);
        }
        if(deathJumpTimer <= 0 && sr.color != new Color(0, 0, 0, 0.6f)) {
            if(deathAnimImportance == 1) {
                FlexibleAnimation explosion1 = SmallExplosionObjectPool.instance.GetPooledObject().GetComponent<FlexibleAnimation>();
                explosion1.gameObject.SetActive(true);
                explosion1.currentFrame = 0;
                explosion1.transform.position = new Vector2(transform.position.x + explosionOffsetX, transform.position.y + (sr.bounds.size.x / 2) + explosionOffsetY);
            }
            if(deathAnimImportance == 2) {
                soundManager.PlayClip(soundManager.EnemyDeath2, transform, 1f);
                FlexibleAnimation explosion2 = MediumExplosionObjectPool.instance.GetPooledObject().GetComponent<FlexibleAnimation>();
                explosion2.gameObject.SetActive(true);
                explosion2.currentFrame = 0;
                explosion2.transform.position = new Vector2(transform.position.x + explosionOffsetX, transform.position.y + (sr.bounds.size.x / 2) + explosionOffsetY);
            }
            if(deathAnimImportance == 3) {
                soundManager.PlayClip(soundManager.EnemyDeath3, transform, 1f);
                FlexibleAnimation explosion3 = LargeExplosionObjectPool.instance.GetPooledObject().GetComponent<FlexibleAnimation>();
                explosion3.gameObject.SetActive(true);
                explosion3.currentFrame = 0;
                explosion3.transform.position = new Vector2(transform.position.x + explosionOffsetX, transform.position.y + (sr.bounds.size.x / 2) + explosionOffsetY);
            }
            if(!hasCustomDeathSound) {
                if(SFXimportance == 1 || SFXimportance == 0) {
                    soundManager.PlayClip(soundManager.EnemyDeath1, transform, 1f);
                }
                if(SFXimportance == 2) {
                    soundManager.PlayClip(soundManager.EnemyDeath2, transform, 1f);
                }
                if(SFXimportance == 3) {
                    soundManager.PlayClip(soundManager.EnemyDeath3, transform, 1f);
                }
            }
            if(hasCustomDeathSound) {
                soundManager.PlayClip(customDeathSound, transform, customDeathSoundVolume);
            }
            deathJumpTimer = deathJumpCooldown;
            if(player.multikillTimer <= 0) {
                player.multikillTimer = player.multikillTimerSet;
            }
            player.multikillCombo += 1;
            player.canGainMultikillStyle = true;
        }
        sr.color = new Color(0, 0, 0, 0.6f);
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, deathJumpStrength);
    }

    void OnTriggerStay2D(Collider2D enemyCollider) {
        if((enemyCollider.CompareTag("PlayerSecondaryCollider"))  && !player.recoiling && !player.isExploding && initialNonDamageTimer <= 0 && gameObject.GetComponent<Enemy>().enabled == true) {
            if(damageDealt > 0 && !player.isDamaging && !player.isDashJumping && player.exitDashTimer <= 0 && player.exitDownDashTimer <= 0 && player.exitDownDashTimer <= 0 ) {
                player.Hit(damageDealt);
                player.style -= damageDealt * 2;
                player.ResetStyleDeductionTimer();
            }
        }
        if(enemyCollider.CompareTag("PlayerSecondaryCollider") && player.isExploding && gameObject.GetComponent<Enemy>().enabled == true) {
            if(canTakeDamage && !hasBeenHitByExplosion) {
                hasBeenHitByExplosion = true;
                player.style += 1;
                player.ResetStyleDeductionTimer();
                Hit(4);
                if(player.transform.position.x < transform.position.x) {
                    KnockBack(false, 1, 0, 0);
                }
                if(player.transform.position.x > transform.position.x) {
                    KnockBack(false, -1, 0, 0);
                }
            }
        }
        if(enemyCollider.CompareTag("PlayerFireWaveGroundCheck") && gameObject.GetComponent<Enemy>().enabled == true) {
            if(canTakeDamage) {
                if((enemyCollider.gameObject.name == "FireWave1GroundCheck" && !hitByRightWave) || (enemyCollider.gameObject.name == "FireWave2GroundCheck" && !hitByLeftWave)) {
                    Hit(1);
                    player.style += 1;
                    player.sp += 1;
                    player.ResetStyleDeductionTimer();
                    if(health <= 0) {
                        player.style += 3;
                        if(health == maxHealth) {
                            player.style += 2;
                        }
                    }
                }
                if(enemyCollider.gameObject.name == "FireWave1GroundCheck" && !hitByRightWave) {
                    hitByRightWave = true;
                    player.rightFireWaveHasHitEnemy = true;
                    if(health <= 0) {
                        player.rightFireWaveHasKilledEnemy = true;
                    }
                }
                if(enemyCollider.gameObject.name == "FireWave2GroundCheck" && !hitByLeftWave) {
                    hitByLeftWave = true;
                    player.leftFireWaveHasHitEnemy = true;
                    if(health <= 0) {
                        player.leftFireWaveHasKilledEnemy = true;
                    }
                }
            }
        }
    }
    
    public void Hit(float damageDealt) {
        if(canTakeDamage) {
            if(beingKnockedBack) {
                player.style += 3;
            }
            canTakeDamage = false;
            health -= damageDealt;
            effectTimer = effectCooldown;
            if(hasHitEffect && health > 0 && (!invincible || hitEffectWhileInvincible)) {
                sr.color = new Color(0, 0, 0);
            }
        }
    }

    public void UpdateEffect() {
        effectTimer -= Time.deltaTime;
        knockbackTimer -= Time.deltaTime;
        if(effectTimer <= 0 && health > 0 && hasHitEffect) {
            sr.color = new Color(255, 255, 255);
            canTakeDamage = true;
            if(hasBeenHitByExplosion && !player.isExploding) {
                hasBeenHitByExplosion = false;
            }
            if(hitByRightWave && !player.fireWave1.activeInHierarchy) {
                hitByRightWave = false;
            }
            if(hitByLeftWave && !player.fireWave2.activeInHierarchy) {
                hitByLeftWave = false;
            }
        }
        if(knockbackTimer <= 0) {
            beingKnockedBack = false;
        }
        if(invincibilityTimer <= 0 && invincible && invincibilityTimerSet != 0) {
            invincible = false;
        }
    }

    public void KnockBack(bool customKnockBack, float knockbackDirX, float customKnockBackX, float customKnockBackY) {
        if(health > 0) {
            beingKnockedBack = true;
            if(!customKnockBack) {
                rb.velocity = new Vector2(knockbackStrengthX * knockbackDirX, knockbackStrengthY);
            }
            if(customKnockBack) {
                rb.velocity = new Vector2(customKnockBackX * knockbackDirX, customKnockBackY);
            }
            knockbackTimer = knockbackTimerSet;
        }
    }
}
