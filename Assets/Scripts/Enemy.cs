using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    private SoundManager soundManager;
    private MusicManager musicManager;
    [HideInInspector]
    public SpriteRenderer sr;
    private Animator anim;
    private Transform playerTR;
    private PlayerController player;
    private LayerMask ground;
    [HideInInspector]
    public Rigidbody2D rb;
    public GameObject objectToModify;
    public GameObject hostEnemy;
    public AudioClip customDeathSound;
    [HideInInspector]
    public EnemySpawner spawner;

    public float customDeathSoundVolume = 1;
    [HideInInspector]
    public bool hasBeenSpawned;
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
    [HideInInspector]
    public float startingX = 0;
    [HideInInspector]
    public float startingY = 0;
    public bool detectCollisionsWithEnemies = true;
    private bool touchingOtherEnemies;
    public bool useCustomEnemyCheckCollider = false;
    public BoxCollider2D customEnemyCheckCollider;
    public bool cannotBeLaunchedByEnemyOnEnemyCollisions = false;
    [HideInInspector]
    public PhysicsMaterial2D customEnemyCheckColliderMaterial;
    [HideInInspector]
    public GameObject enemyOnEnemyColliderGameObject;
    private bool hasEnemyOnEnemyCollider;
    public float enemyCheckSizeX;
    public float enemyCheckSizeY;
    //displayEnemyCheckSizeX and displayEnemyCheckSizeY just show in the inspector how big sr.bounds.size.x and sr.bounds.size.y are respectively
    public float displayEnemyCheckSizeX;
    public float displayEnemyCheckSizeY;
    public float enemyCheckOffsetX;
    public float enemyCheckOffsetY;
    public bool setAnimJumpBoolOnDeathJump;
    public bool hasHitEffect = true;
    public bool onlySpawnIfDisabled = false;
    public bool hasCustomDeathSound = false;
    public bool canBeKnockedBack = true;
    //canBeKnockedBack still has to be true for stunning to work, I'm so good at naming variables
    //also doesn't apply to damage areas knocking enemies back
    public bool getStunnedInsteadOfBeingKnockedBack;
    //beingKnockedBack is true when stunned
    public bool beingKnockedBack;
    [HideInInspector]
    public bool canTakeDamage = true;
    [HideInInspector]
    public bool canCurrentlyBeKnockedBack = true;
    private float canBeKnockedBackAgainTimer;
    private float canBeKnockedBackAgainTimerSet = 0.2f;
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
    public bool freezeXonStun = true;
    public bool freezeYonStun;
    private bool initialLockX;
    private bool initialLockY;
    public bool usePositionDependentAirKnockback = true;
    public bool useZeroGravityPositionDependentAirKnockback;
    public float airKnockbackMultiplierY = 1;
    public float airKnockbackMultiplierX = 1;
    public bool touchingGround;
    public bool touchingWall;
    public float groundCheckLength = 0.1f;
    public float groundCheckOffsetX;
    public float wallCheckDirectionMultiplier = 1;
    private float wallCheckDirectionMultiplierSet;
    public float wallCheckLength;
    public float wallCheckOffsetY;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerTR = GameObject.FindWithTag("Player").GetComponent<Transform>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        musicManager = GameObject.FindWithTag("MusicManager").GetComponent<MusicManager>();
        ground = LayerMask.GetMask("Ground");
        startingX = transform.position.x;
        startingY = transform.position.y;
        if(GetComponent<BoxCollider2D>() != null) {
            customEnemyCheckCollider = GetComponent<BoxCollider2D>();
        }
        if(enemyCheckSizeX == 0 || enemyCheckSizeY == 0) {
            enemyCheckSizeX = sr.bounds.size.x;
            enemyCheckSizeY = sr.bounds.size.y;
        }
        if(GetComponent<Animator>() != null) {
            anim = GetComponent<Animator>();
        }
        invincibleSet = invincible;
        defaultGravity = rb.gravityScale;
        if(rb.constraints == RigidbodyConstraints2D.FreezePositionX) {
            initialLockX = true;
        }
        if(rb.constraints == RigidbodyConstraints2D.FreezePositionY) {
            initialLockY = true;
        }
        wallCheckDirectionMultiplierSet = wallCheckDirectionMultiplier;
        customEnemyCheckColliderMaterial = player.customEnemyCheckColliderMaterial;
        startCompleted = true;
        if(!dontKillEnemy) {
            gameObject.SetActive(false);
        }
    }

    void OnEnable() {
        sr = GetComponent<SpriteRenderer>();
        hasEnemyOnEnemyCollider = false;
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
            wallCheckDirectionMultiplier = wallCheckDirectionMultiplierSet;
        }
        if(invincibilityTimerSet != 0) {
            invincible = true;
            invincibilityTimer = invincibilityTimerSet;
        }
    }

    void FixedUpdate() {
        HealthCheck();
        UpdateEffect();
        CheckCollisions();
        if(player.respawned) {
            gameObject.SetActive(false);
        }
        initialNonDamageTimer -= Time.deltaTime;
        fastKillTimer -= Time.deltaTime;
        invincibilityTimer -= Time.deltaTime;
        canBeKnockedBackAgainTimer -= Time.deltaTime;
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
                    if(player.jumpsLeft == 1) {
                        player.jumpsLeft = 2;
                    }
                    player.dashesLeft = 2;
                    player.canUpDash = true;
                    player.style += 2;
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
            if(spawner != null) {
                spawner.CheckDeaths();
            }
        }
        sr.color = new Color(0, 0, 0, 0.6f);
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, deathJumpStrength);
    }

    private void CheckCollisions() {
        if(startCompleted) {
            touchingGround = Physics2D.Raycast(new Vector2(transform.position.x + groundCheckOffsetX, transform.position.y), transform.up * -1, groundCheckLength, ground);
            touchingWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + wallCheckOffsetY + (sr.bounds.size.x / 2)), transform.right * wallCheckDirectionMultiplier, wallCheckLength, ground);
            if(detectCollisionsWithEnemies) {
                if(!hasEnemyOnEnemyCollider && sr.isVisible) {
                    hasEnemyOnEnemyCollider = true;
                    enemyOnEnemyColliderGameObject = EnemyOnEnemyColliderObjectPool.instance.GetPooledObject();
                    enemyOnEnemyColliderGameObject.SetActive(true);
                    enemyOnEnemyColliderGameObject.transform.position = transform.position;
                    Enemy enemyToUse = GetComponent<Enemy>();
                    if(useCustomEnemyCheckCollider) {
                        enemyOnEnemyColliderGameObject.GetComponent<EnemyOnEnemyCollider>().CreateCollider(enemyCheckOffsetX, enemyCheckOffsetY, enemyCheckSizeX, enemyCheckSizeY, true, customEnemyCheckCollider, gameObject.GetComponent<Enemy>());
                    }
                    if(!useCustomEnemyCheckCollider) {
                        enemyOnEnemyColliderGameObject.GetComponent<EnemyOnEnemyCollider>().CreateCollider(enemyCheckOffsetX, enemyCheckOffsetY, enemyCheckSizeX, enemyCheckSizeY, false, null, gameObject.GetComponent<Enemy>());
                    }
                }
                if(hasEnemyOnEnemyCollider && !sr.isVisible) {
                    hasEnemyOnEnemyCollider = false;
                    enemyOnEnemyColliderGameObject.SetActive(false);
                }
            }
        }
    }

    private void OnDrawGizmos() {
        displayEnemyCheckSizeX = GetComponent<SpriteRenderer>().bounds.size.x;
        displayEnemyCheckSizeY = GetComponent<SpriteRenderer>().bounds.size.y;
        Gizmos.DrawWireCube(new Vector2(transform.position.x + enemyCheckOffsetX, transform.position.y + GetComponent<SpriteRenderer>().bounds.size.y / 2 + enemyCheckOffsetY), new Vector3(enemyCheckSizeX, enemyCheckSizeY, 0.25f));
        Gizmos.DrawLine(new Vector2(transform.position.x + groundCheckOffsetX, transform.position.y), new Vector2(transform.position.x + groundCheckOffsetX, transform.position.y - groundCheckLength));
        Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + wallCheckOffsetY + (GetComponent<SpriteRenderer>().bounds.size.x / 2)), new Vector2(transform.position.x + (wallCheckLength * wallCheckDirectionMultiplier), transform.position.y + wallCheckOffsetY + (GetComponent<SpriteRenderer>().bounds.size.x / 2)));
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
                Hit(5);
                if(player.transform.position.x < transform.position.x) {
                    KnockBack(false, false, null, 1, 0, 0, 1, 1);
                }
                if(player.transform.position.x > transform.position.x) {
                    KnockBack(false, false, null, -1, 0, 0, 1, 1);
                }
            }
        }
        if(enemyCollider.CompareTag("PlayerFireWaveGroundCheck") && gameObject.GetComponent<Enemy>().enabled == true) {
            if(canTakeDamage) {
                if((enemyCollider.gameObject.name == "FireWave1GroundCheck" && !hitByRightWave) || (enemyCollider.gameObject.name == "FireWave2GroundCheck" && !hitByLeftWave)) {
                    Hit(1);
                    player.style += 1;
                    player.sp += 2;
                    player.ResetStyleDeductionTimer();
                    if(health <= 0) {
                        player.style += 3;
                        if(maxHealth - 1 <= 0) {
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
                    if(maxHealth - 1 <= 0) {
                        player.rightFireWaveHasOneShottedEnemy = true;
                    }
                }
                if(enemyCollider.gameObject.name == "FireWave2GroundCheck" && !hitByLeftWave) {
                    hitByLeftWave = true;
                    player.leftFireWaveHasHitEnemy = true;
                    if(health <= 0) {
                        player.leftFireWaveHasKilledEnemy = true;
                        player.leftFireWaveHasOneShottedEnemy = true;
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
            if(beingKnockedBack) {
                beingKnockedBack = false;
                if(useZeroGravityPositionDependentAirKnockback && usePositionDependentAirKnockback) {
                    rb.gravityScale = defaultGravity;
                }
            }
            if(getStunnedInsteadOfBeingKnockedBack) {
                if(!initialLockX && !initialLockY) {
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                if(!initialLockX && initialLockY) {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                }
                if(initialLockX && !initialLockY) {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
                if(initialLockX && initialLockY) {
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                }
            }
        }
        if(canBeKnockedBackAgainTimer <= 0 && !canCurrentlyBeKnockedBack) {
            canCurrentlyBeKnockedBack = true;
        }
        if(invincibilityTimer <= 0 && invincible && invincibilityTimerSet != 0) {
            invincible = false;
        }
    }

    public void KnockBack(bool customKnockBack, bool enemyOnEnemyKnockback, Transform enemyTransform, float knockbackDirX, float customKnockBackX, float customKnockBackY, float knockBackMultiplierX, float knockBackMultiplierY) {
        if(health > 0 && canCurrentlyBeKnockedBack) {
            player.currentHighestEnemyLayerOrder += 1;
            sr.sortingOrder = player.currentHighestEnemyLayerOrder;
            canBeKnockedBackAgainTimer = canBeKnockedBackAgainTimerSet;
            canCurrentlyBeKnockedBack = false;
            if(!getStunnedInsteadOfBeingKnockedBack) {
                if(!usePositionDependentAirKnockback || (usePositionDependentAirKnockback && touchingGround)) {
                    beingKnockedBack = true;
                    if(!customKnockBack) {
                        rb.velocity = new Vector2(knockbackStrengthX * knockbackDirX * knockBackMultiplierX, knockbackStrengthY * knockBackMultiplierY);
                    }
                    if(customKnockBack) {
                        rb.velocity = new Vector2(customKnockBackX * knockbackDirX * knockBackMultiplierX, customKnockBackY * knockBackMultiplierY);
                    }
                }
                if(usePositionDependentAirKnockback && !touchingGround) {
                    beingKnockedBack = true;
                    float airKnockbackDirY = 0;
                    if(!enemyOnEnemyKnockback) {
                        if(playerTR.position.y > transform.position.y) {
                            airKnockbackDirY = -1;
                            player.style += 3;
                        }
                        if(playerTR.position.y <= transform.position.y) {
                            airKnockbackDirY = 1;
                        }
                    }
                    if(enemyOnEnemyKnockback) {
                        if(transform.position.y > enemyTransform.position.y) {
                            airKnockbackDirY = -1;
                            player.style += 8;
                        }
                        if(transform.position.y <= enemyTransform.position.y) {
                            airKnockbackDirY = 1;
                        }
                    }
                    if(useZeroGravityPositionDependentAirKnockback) {
                        airKnockbackDirY = 0;
                        rb.gravityScale = 0;
                    }
                    if(!customKnockBack) {
                        rb.velocity = new Vector2((knockbackStrengthX * airKnockbackMultiplierX) * knockbackDirX * knockBackMultiplierX, (knockbackStrengthY * airKnockbackMultiplierY) * airKnockbackDirY * knockBackMultiplierY);
                    }
                    if(customKnockBack) {
                        rb.velocity = new Vector2((customKnockBackX * airKnockbackMultiplierX) * knockbackDirX * knockBackMultiplierX, (customKnockBackY * airKnockbackMultiplierY) * airKnockbackDirY * knockBackMultiplierY);
                    }
                }
            }
            if(getStunnedInsteadOfBeingKnockedBack) {
                if(!freezeXonStun && !freezeYonStun) {
                    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                }
                if(!freezeXonStun && freezeYonStun) {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
                }
                if(freezeXonStun && !freezeYonStun) {
                    rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
                }
                if(freezeXonStun && freezeYonStun) {
                    rb.constraints = RigidbodyConstraints2D.FreezeAll | RigidbodyConstraints2D.FreezeRotation;
                }
            }
            knockbackTimer = knockbackTimerSet;
        }
    }
}
