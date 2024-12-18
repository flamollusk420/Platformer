using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private SoundManager soundManager;
    [HideInInspector]
    public Rigidbody2D rb;
    [HideInInspector]
    public SpriteRenderer sr;
    [HideInInspector]
    public Animator anim;
    private PlayerControls controls;
    public GameObject fireWave1;
    public GameObject fireWave2;
    public GameObject soulClaw;
    public Transform groundCheck;
    public LayerMask ground;
    public LayerMask enemies;
    private AdjustableUIBar healthBar;
    private AdjustableUIBar spBar;
    private AdjustableUIBar styleBar;
    private AdjustableUIBar currentRankUI;
    private AdjustableUIBar normalDash;
    private AdjustableUIBar jumps;
    private ToggleSprite upDash;
    private ToggleSprite equippedPrimaryUI;
    private Transform playerMelee;
    private Pause pauseUI;
    public PlayerMeleeCollider playerMeleeCollider;
    public PlayerDashCollider playerDashCollider;
    [HideInInspector]
    public PhysicsMaterial2D customEnemyCheckColliderMaterial;
    [HideInInspector]
    public EnemySpawner currentRoomSpawner;
    private FlexibleAnimation explosion;
    [HideInInspector]
    public ChangeRooms roomChanger;
    private BarrierContainer barriers;
    [HideInInspector]
    public AudioSource windLoop;
    private RaycastHit2D[] touchingEnemyRay;
    private RaycastHit2D[] touchingEnemyRay2;

    public float movementSpeed;
    public float dashSpeed = 20;
    public float dashLength = 0.35f;
    public float upDashSpeed = 20;
    public float upDashLength = 0.35f;
    public float downDashSpeed;
    public float jumpStrength;
    public float highJumpStrength;
    public float wallJumpKickback;
    public int health;
    public int maxHealth;
    public float sp;
    public float maxSP = 25;
    public float style;
    public float styleWithinRank;
    public int coins;
    [HideInInspector]
    public float respawnX;
    [HideInInspector]
    public float respawnY;
    [HideInInspector]
    public string respawnRoomName;
    public int currentRank;
    public int jumpsLeft = 1;
    public int dashesLeft = 2;
    public float fireWaveOffsetX = 4.5f;
    public float fireWaveOffsetY = 0.06f;
    public float soulClawOffsetX = 4.5f;
    public float soulClawOffsetY = 0.06f;
    public float soulClawEnemyCheckLength;
    private bool touchingEnemyRayCheck;
    private bool touchingEnemyRayCheck2;
    [HideInInspector]
    //used to keep enemies that are being knocked back above other enemies in the sorting layer order
    public int currentHighestEnemyLayerOrder = 25;

    [HideInInspector]
    public bool startCompleted;
    public bool respawned;
    public bool recoiling;
    public bool facingRight = true;
    public bool touchingGround;
    public bool touchingWall;
    public bool touchingWallBackwards;
    private bool touchingWallBackwardsCheck;
    public bool isJumping;
    public bool startedJumpWhileTouchingWall;
    //wallJumping is only true for the amount of time specified in wallJumpTimerSet
    //it's used to stop the player from walking into the wall by freezing movement while it's active
    //once it's turned off, any movement direction the player was holding will work again
    public bool wallJumping;
    public bool isFalling;
    public bool isWalking;
    public bool isSliding;
    private float slideVelocity;
    private float storedSlideVelocity;
    public bool isDashing;
    //isDashing2 is used for disabling wall sliding while dashing off a wall and gets turned off a split second later
    //it's also used to prevent wall sliding while exploding and being knocked back
    public bool isDashing2;
    //isDashing2Indicator is used for enemies that need to detect if the player just started a dash
    [HideInInspector]
    public bool isDashing2Indicator;
    private float isDashing2IndicatorTimer;
    private float isDashing2IndicatorTimerSet = 0.1f;
    //isDashing3 is true while the dash can't be canceled
    public bool isDashing3;
    private float isDashing3Timer;
    private float isDashing3TimerSet = 0.25f;
    public bool isDashJumping;
    //isDashJumping2 is true when the player is using momentum from a dash jump after the dash jump animation
    public bool isDashJumping2;
    public bool isExitingDash;
    public bool isDamaging;
    public bool isDownDashing;
    public bool canDownDash;
    public bool isUpDashing;
    public bool canUpDash;
    public bool isExitingUpDash;
    public bool isExitingDownDash;
    public bool isDownDashAttacking;
    public bool isShooting;
    public bool isMeleeAttacking;
    public bool isExploding;
    public bool isShootingFireWave;
    public bool isShootingSoulClaw;
    [HideInInspector]
    public bool isShootingGroundAttack;
    public bool isGoingAbove25;
    public bool isCrouching;
    public bool isHighJumping;
    public bool isAboutToHighJump;
    private bool isAboutToHighJump2;
    public bool hasFlipped;
    public bool primaryAttackIsMelee;

    private float velocityBeforeMove;
    public float dirX;
    public float dirY;
    public float startingFacingDirX = 1;
    public float facingDirX = 1;
    public float dashJumpFacingDirX = 1;
    public float wallFacingDirX = 1;
    public float dashFacingDirX = 1;
    private float dashTimer;
    public float dashCooldown;
    private float upDashTimer;
    public float exitUpDashTimer;
    public float exitUpDashTimerSet;
    public float exitDownDashTimer;
    public float exitDownDashTimerSet;
    public float exitDashTimer;
    public float exitDashTimerSet;
    public float wallJumpDashTimer;
    public float wallJumpDashTimerSet;
    public float styleDeductionTimer;
    public float styleDeductionTimerSet = 1.5f;
    private float styleDeductionCancelTimer;
    private float highJumpTimer;
    private float highJumpTimerSet = 0.083f;
    private float highJumpTimer2;
    private float highJumpTimer2Set = 0.11f;
    private float dashJumpTimer;
    private float wallJumpTimer;
    //has to be really low so the wall jump feels good to use to climb up walls as well as jump off them
    public float wallJumpTimerSet = 0.075f;
    private float explosionTimer;
    public float explosionTimerSet;
    private float groundedJumpNextToWallTimer;
    private float downDashAttackTimer;
    public float downDashAttackCooldown;
    private float fireWaveShootTimer;
    private float fireWaveShootTimerSet = 0.15f;
    private float soulClawShootTimer;
    public float soulClawShootTimerSet = 0.15f;
    [HideInInspector]
    public bool rightFireWaveHasHitEnemy = false;
    [HideInInspector]
    public bool rightFireWaveHasKilledEnemy = false;
    [HideInInspector]
    public bool rightFireWaveHasOneShottedEnemy = false;
    [HideInInspector]
    public bool leftFireWaveHasHitEnemy = false;
    [HideInInspector]
    public bool leftFireWaveHasKilledEnemy = false;
    [HideInInspector]
    public bool leftFireWaveHasOneShottedEnemy = false;
    private bool hasPlayedFireWaveDestroyedSound = true;
    private bool canStopOnTouchWall = true;
    public string currentRoomName;

    public float recoilTimer;
    private float effectTimer;
    public float effectCooldown = 0.15f;
    public float multikillTimer;
    public float multikillTimerSet;
    public float multikillCombo;
    public bool canGainMultikillStyle;
    public bool beingKnockedBack;
    public float knockbackTimer;
    public float knockbackTimerSet;
    public bool timeScaleIsZero;
    private bool deathEffectIsHappening;
    private bool deathEffectComplete;
    public float deathEffectPlayerHidingTimerSet;
    private float deathEffectPlayerHidingTimer;
    private float oneSecondTimer;

    public bool canShoot;
    public int equippedBulletType = 1;
    private float temporaryBulletType = 1;
    public float bulletSpeed = 3.5f;
    private float temporaryBulletSpeed = 3.5f;
    public float bulletCooldown = 0.2f;
    private float bulletTimer;
    public float bulletAnimationCooldown = 0.1f;
    private float bulletAnimationTimer;
    private float meleeTimer;
    private float meleeAnimationTimer;
    private float meleeCancelTimer;
    public float meleeCooldown;
    public float meleeAnimationCooldown;
    public float meleeCancelTimerSet;
    public float meleeAttackDamage = 2.5f;
    
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        groundCheck = GameObject.FindWithTag("PlayerGroundCheck").GetComponent<Transform>();
        customEnemyCheckColliderMaterial = GetComponent<BoxCollider2D>().sharedMaterial;
        healthBar = GameObject.FindWithTag("HealthBarUI").GetComponent<AdjustableUIBar>();
        spBar = GameObject.FindWithTag("SPBarUI").GetComponent<AdjustableUIBar>();
        styleBar = GameObject.FindWithTag("StyleBarUI").GetComponent<AdjustableUIBar>();
        currentRankUI = GameObject.FindWithTag("CurrentRankUI").GetComponent<AdjustableUIBar>();
        normalDash = GameObject.FindWithTag("NormalDashUI").GetComponent<AdjustableUIBar>();
        jumps = GameObject.FindWithTag("JumpUI").GetComponent<AdjustableUIBar>();
        upDash = GameObject.FindWithTag("UpDashUI").GetComponent<ToggleSprite>();
        equippedPrimaryUI = GameObject.FindWithTag("EquippedPrimaryUI").GetComponent<ToggleSprite>();
        playerMeleeCollider = GameObject.FindWithTag("PlayerMeleeCollider").GetComponent<PlayerMeleeCollider>();
        playerDashCollider = GameObject.FindWithTag("PlayerDashCollider").GetComponent<PlayerDashCollider>();
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        pauseUI = GameObject.FindWithTag("PauseUI").GetComponent<Pause>();
        fireWave1 = GameObject.FindWithTag("PlayerFireWave");
        fireWave2 = GameObject.FindWithTag("PlayerFireWave2");
        //soulClaw = GameObject.FindWithTag("PlayerSoulClaw");
        roomChanger = GameObject.FindWithTag("PlayerSecondaryCollider").GetComponent<ChangeRooms>();
        barriers = GameObject.FindWithTag("BarrierContainer").GetComponent<BarrierContainer>();
        windLoop = GameObject.FindWithTag("WindLoop").GetComponent<AudioSource>();
        controls = new PlayerControls();
        //this will be changed later
        respawnX = transform.position.x;
        respawnY = transform.position.y;
        facingDirX = startingFacingDirX;
        startCompleted = true;
    }

    void OnEnable() {
        deathEffectIsHappening = false;
        deathEffectComplete = false;
        sp = maxSP;
        health = maxHealth;
        styleDeductionTimer = styleDeductionTimerSet;
        facingDirX = startingFacingDirX;
    }

    void FixedUpdate() {
        CheckCollisions();
        CheckState();
        CheckHealth();
        MovePlayer();
        UpdateAnimations();
        UpdateTimers();
        UpdateUI();
        UpdateEffect();
    }

    private void MovePlayer() {
        if(!deathEffectIsHappening) {
            if(isWalking && !wallJumping && !isCrouching && !(touchingWall && dirX == wallFacingDirX * -1) && !(touchingWall && isDashing) && !(isDashJumping2 && dirX == dashJumpFacingDirX && Mathf.Abs(rb.velocity.x) > 0.2f) && !isExploding && !beingKnockedBack && !isShootingGroundAttack && !isExitingDownDash) {
                if(touchingGround) {
                    rb.velocity = new Vector2((movementSpeed + Mathf.Abs(slideVelocity)) * dirX, rb.velocity.y);
                    isSliding = false;
                }
                else {
                    rb.velocity = new Vector2((movementSpeed) * dirX, rb.velocity.y);
                }
            }
            if(isWalking && wallJumping && !isExploding && !beingKnockedBack && !isShootingGroundAttack && !isCrouching) {
                rb.velocity = new Vector2(velocityBeforeMove + (0.5f * (movementSpeed * dirX)), rb.velocity.y);
            }
            if(isDashing && !isExploding && !beingKnockedBack && !isCrouching && !isShootingGroundAttack) {
                if(!isSliding) {
                    rb.velocity = new Vector2(dashSpeed * dashFacingDirX, 0);
                }
                if(isSliding) {
                    rb.velocity = new Vector2((dashSpeed + Mathf.Abs(slideVelocity) * 0.85f) * dashFacingDirX, 0);
                    slideVelocity *= 0.9f;
                }
            }
            if(isUpDashing && !isExploding && !beingKnockedBack && !isCrouching && !isShootingGroundAttack) {
                rb.velocity = new Vector2(0, upDashSpeed);
            }
            if(isDownDashing && !isExploding && !beingKnockedBack && !isCrouching && !isShootingGroundAttack) {
                rb.velocity = new Vector2(0, downDashSpeed);
            }
            if(isHighJumping && !isExploding && !beingKnockedBack) {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        if(deathEffectIsHappening) {
            rb.velocity = new Vector2(0, 0);
        }
        //if the player moves in the opposite direction of a dash jump, isDashJumping2 turns off
        if(dirX != dashJumpFacingDirX && dirX != 0) {
            isDashJumping2 = false;
        }
    }

    //god had no hand in the creation of this function
    private void CheckState() {
        if(dirX == 1 && !isDashing) {
            facingRight = true;
        }
        if(dirX == -1 && !isDashing) {
            facingRight = false;
        }
        if(dirX != 0 && !isDashing && !isExploding) {
            isWalking = true;
        }
        if(facingRight && !isDashing && !isShootingSoulClaw && !isExploding && !(touchingWallBackwards && !touchingGround)) {
            transform.localScale = new Vector2(1, 1);
            facingDirX = 1;
        }
        if(!facingRight && !isDashing && !isShootingSoulClaw && !isExploding && !(touchingWallBackwards && !touchingGround)) {
            transform.localScale = new Vector2(-1, 1);
            facingDirX = -1;
        }
        if(isDashing && facingDirX != dashFacingDirX && !isShootingSoulClaw && !(touchingWallBackwards && !touchingGround)) {
            facingDirX = dashFacingDirX;
            transform.localScale = new Vector2(facingDirX, 1);
        }
        if(rb.velocity.y < 0 && !isExploding && !beingKnockedBack) {
            isFalling = true;
            isJumping = false;
            isHighJumping = false;
            wallJumping = false;
            isDashJumping = false;
            rb.gravityScale = 5.5f;
        }
        if(!isFalling && rb.gravityScale == 5 && !isExploding && !beingKnockedBack) {
            rb.gravityScale = 4;
        }
        if(rb.velocity.y > 0) {
            isJumping = true;
            isFalling = false;
        }
        if(touchingGround) {
            isJumping = false;
            isHighJumping = false;
            isFalling = false;
            wallJumping = false;
            if(isDownDashing) {
                slideVelocity = storedSlideVelocity;
                isDownDashing = false;
                isDamaging = false;
                exitDownDashTimer = exitDownDashTimerSet;
                isExitingDownDash = true;
            }
        }
        if((!isJumping && startedJumpWhileTouchingWall && groundedJumpNextToWallTimer <= 0)) {
            startedJumpWhileTouchingWall = false;
        }
        if(isDashJumping && startedJumpWhileTouchingWall) {
            startedJumpWhileTouchingWall = false;
            groundedJumpNextToWallTimer = 0;
        }
        if((touchingGround || touchingWall) && !isDownDashing && !isUpDashing && !isExitingUpDash && !(isJumping && !isDashJumping && !isFalling && startedJumpWhileTouchingWall)) {
            //jumpsLeft is set to 1 instead of 2 even though there's a double jump
            //this is because after a jump is started and jumpsLeft is subtracted by 1 the player's ground detection hitbox still detects the ground
            //and CheckState() is run again, meaning jumpsleft is set to 1 again after the jump starts
            //if it was set to 2 there would be a triple jump
            jumpsLeft = 1;
            dashesLeft = 2;
            canDownDash = true;
            canUpDash = true;
        }
        if(Mathf.Abs(rb.velocity.x) >= 22) {
            if(Mathf.Abs(rb.velocity.x) < 25) {
                isGoingAbove25 = false;
                windLoop.volume = ((Mathf.Abs(rb.velocity.x) - 20) / 5);
            }
            if(Mathf.Abs(rb.velocity.x) >= 25) {
                isGoingAbove25 = true;
                windLoop.volume = 1;
            }
        }
        if(Mathf.Abs(rb.velocity.x) < 22) {
            windLoop.volume = 0;
            isGoingAbove25 = false;
        }
        if(touchingWallBackwards && !touchingWallBackwardsCheck && !touchingWall && !touchingGround && !isDownDashing && !isUpDashing && !isExitingUpDash && !isExitingDownDash && !isExploding && !deathEffectIsHappening && !beingKnockedBack && !(isJumping && !isDashJumping && !isFalling)) {
            touchingWallBackwardsCheck = true;
            facingRight = !facingRight;
            facingDirX = facingDirX * -1;
            wallFacingDirX = facingDirX * -1;
            if(!deathEffectIsHappening) {
                rb.gravityScale = 0.65f;
            }
            dashCooldown = 0;
            isExitingDash = false;
            if(isDashJumping) {
                isDashJumping = false;
            }
            if(isDashing && !isDashing2) {
                isDashing = false;
                jumpsLeft = 1;
                dashesLeft = 2;
                isDamaging = false;
            }
            if(canStopOnTouchWall && !(isDashing)) {
                rb.velocity = new Vector2(facingDirX * 100, 0);
                canStopOnTouchWall = false;
            }
        }
        if(!touchingWallBackwards) {
            touchingWallBackwardsCheck = false;
        }
        if(touchingWall && !touchingGround && !isDownDashing && !isUpDashing && !isExitingUpDash && !isExitingDownDash && !isExploding && !deathEffectIsHappening && !beingKnockedBack && !(isJumping && !isDashJumping && !isFalling && startedJumpWhileTouchingWall)) {
            wallFacingDirX = facingDirX * -1;
            if(!deathEffectIsHappening) {
                rb.gravityScale = 0.65f;
            }
            dashCooldown = 0;
            isExitingDash = false;
            if(isDashJumping) {
                isDashJumping = false;
            }
            if(isDashing && !isDashing2) {
                isDashing = false;
                jumpsLeft = 1;
                dashesLeft = 2;
                isDamaging = false;
            }
            if(canStopOnTouchWall && !(isDashing)) {
                rb.velocity = new Vector2(facingDirX * 100, 0);
                canStopOnTouchWall = false;
            }
        }
        if(!touchingWall || touchingGround) {
            canStopOnTouchWall = true;
            wallFacingDirX = facingDirX;
        }
        //lets the player slide around for a little bit before stopping
        if(dirX == 0 && touchingGround && !isJumping && !isDamaging && !isDashJumping && !isCrouching && !isDownDashing) {
            rb.velocity = new Vector2(rb.velocity.x * 0.95f, rb.velocity.y);
            slideVelocity = rb.velocity.x;
            if(Mathf.Abs(rb.velocity.x) <= 3) {
                rb.velocity = new Vector2(0, rb.velocity.y);
                isSliding = false;
                slideVelocity = 0;
            }
            else {
                isSliding = true;
            }
        }
        if(isSliding == false && !isCrouching) {
            slideVelocity = 0;
        }
        if(!touchingWall && rb.gravityScale == 0.5f && !isExploding && !beingKnockedBack && !deathEffectIsHappening) {
            rb.gravityScale = 4;
        }
        if(touchingWall && wallJumping && !isExploding && !beingKnockedBack && !deathEffectIsHappening) {
            rb.gravityScale = 4;
        }
        if(!touchingGround && dashesLeft == 2 && (isDashing || isDashJumping && isDamaging)) {
            dashesLeft = 1;
        }
        if(((!isFalling && !isJumping) || touchingWall) && hasFlipped) {
            hasFlipped = false;
        }
        if(isDashJumping2 && (isDamaging || touchingWall || (touchingGround && !isDashJumping))) {
            styleDeductionCancelTimer = 0.25f;
            isDashJumping2 = false;
        }
        if(isDamaging || isDashJumping || isExitingDownDash || isExitingUpDash || isExitingDash) {
            if(playerDashCollider.canDoDamage == false && playerDashCollider.canDoDamage2 == false) {
                playerDashCollider.canDoDamage2 = true;
            }
            playerDashCollider.canDoDamage = true;
        }
        if(!isDamaging && !isDashJumping && !isExitingDownDash && !isExitingUpDash && !isExitingDash) {
            playerDashCollider.canDoDamage = false;
        }
        if(isExitingDownDash && slideVelocity == 0) {
            slideVelocity = storedSlideVelocity;
        }
        if(leftFireWaveHasHitEnemy && rightFireWaveHasHitEnemy) {
            leftFireWaveHasHitEnemy = false;
            rightFireWaveHasHitEnemy = false;
            style += 3;
        }
        if(leftFireWaveHasKilledEnemy && rightFireWaveHasKilledEnemy) {
            leftFireWaveHasKilledEnemy = false;
            rightFireWaveHasKilledEnemy = false;
            style += 5;
            if(leftFireWaveHasOneShottedEnemy && rightFireWaveHasOneShottedEnemy) {
                style += 5;
                soundManager.PlayClip(soundManager.DoubleFireWaveOneShot, transform, 1);
            }
            else {
                soundManager.PlayClip(soundManager.DoubleFireWaveKill, transform, 1);
            }
        }
        if(!fireWave1.activeInHierarchy && !fireWave2.activeInHierarchy) {
            if(!hasPlayedFireWaveDestroyedSound) {
                soundManager.PlayClip(soundManager.BothFireWavesDestroyed, transform, 1.25f);
                hasPlayedFireWaveDestroyedSound = true;
            }
        }
        if(sp > maxSP) {
            sp = maxSP;
        }
        if(dirY < 0 && !isDashing && !isDashJumping && !isDownDashing && !isShootingGroundAttack && !isJumping && touchingGround) {
            if(!isCrouching) {
                rb.velocity = new Vector2(0, 0);
            }
            isCrouching = true;
            isWalking = false;
            storedSlideVelocity = slideVelocity;
        }
        if((dirY >= 0 || !touchingGround || isDashing || isDashJumping || isJumping || isShootingGroundAttack) && isCrouching) {
            storedSlideVelocity = 0;
            slideVelocity = storedSlideVelocity;
            isCrouching = false;
        }
    }

    private void CheckCollisions() {
        if(!isUpDashing) {
            touchingGround = Physics2D.OverlapBox(groundCheck.position, new Vector2(1.04f, 0.25f), 0, ground);
        }
        if(!isDashing) {
            touchingWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.65f), transform.right * facingDirX, 0.6f, ground);
            touchingWallBackwards = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.65f), transform.right * facingDirX * -1, 0.6f, ground);
        }
        if(isDashing && isDamaging) {
            touchingWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.65f), transform.right * facingDirX, 1.1f, ground);
            touchingWallBackwards = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.65f), transform.right * facingDirX * -1, 0, ground);
        }
        if(isDashing2 && isDamaging) {
            touchingWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.65f), transform.right * facingDirX, 0, ground);
            touchingWallBackwards = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.65f), transform.right * facingDirX * -1, 0, ground);
            if(isDashing && jumpsLeft < 2) {
                if(!beingKnockedBack) {
                    jumpsLeft = 1;
                }
            }
        }
        touchingEnemyRay = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y + 0.15f), transform.right * facingDirX, soulClawEnemyCheckLength, enemies);
        touchingEnemyRayCheck = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.15f), transform.right * facingDirX, soulClawEnemyCheckLength, enemies);
        touchingEnemyRay2 = Physics2D.RaycastAll(new Vector2(transform.position.x, transform.position.y + 1.9f), transform.right * facingDirX, soulClawEnemyCheckLength, enemies);
        touchingEnemyRayCheck2 = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 1.9f), transform.right * facingDirX, soulClawEnemyCheckLength, enemies);
    }

    private void CheckHealth() {
        if(health <= 0 && !deathEffectComplete) {
            CreateDeathEffect();
        }
        if(deathEffectComplete) {
            health = maxHealth;
            RespawnPlayer();
            deathEffectComplete = false;
            sp = maxSP;
        }
    }

    private void CreateDeathEffect() {
        if(!deathEffectIsHappening) {
            deathEffectPlayerHidingTimer = deathEffectPlayerHidingTimerSet;
            explosion = MediumExplosionObjectPool.instance.GetPooledObject().GetComponent<FlexibleAnimation>();
            explosion.GetComponent<SelfDestruct>().timer = explosion.GetComponent<SelfDestruct>().timerSet;
            explosion.currentFrame = 0;
            explosion.transform.position = new Vector2(transform.position.x, transform.position.y + (sr.bounds.size.x / 2));
            explosion.gameObject.SetActive(true);
            soundManager.PlayClip(soundManager.PlayerDeath, transform, 1);
            style = 0;
            deathEffectIsHappening = true;
        }
        currentRoomName = respawnRoomName;
        effectTimer = effectCooldown;
        recoiling = true;
        rb.gravityScale = 0;
        if(explosion.GetComponent<SelfDestruct>().timer <= 0) {
            deathEffectComplete = true;
        }
    }

    private void RespawnPlayer() {
        DespawnObjectsOnPlayerRespawn();
        Hit(0);
        transform.position = new Vector2(respawnX, respawnY);
        facingDirX = startingFacingDirX;
        barriers.RemoveBarriers();
        barriers.removingBarriers = true;
        barriers.roomIsLocked = false;
        roomChanger.tryingToLockPlayerIntoRoom = false;
        roomChanger.oneTimeCameraMove = true;
        sr.enabled = true;
        deathEffectIsHappening = false;
        deathEffectComplete = false;
        rb.gravityScale = 4;
    }

    private void DespawnObjectsOnPlayerRespawn() {
        GameObject[] bullets = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach(GameObject bullet in bullets) {
            bullet.SetActive(false);
        }
        GameObject[] enemyBullets = GameObject.FindGameObjectsWithTag("PlayerBullet");
        foreach(GameObject enemyBullet in enemyBullets) {
            enemyBullet.SetActive(false);
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
        foreach(GameObject enemy in enemies) {
            enemy.SetActive(false);
            enemy.GetComponent<Enemy>().health = enemy.GetComponent<Enemy>().maxHealth;
        }
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("Room");
        foreach(GameObject spawner in spawners) {
            if(spawner.GetComponent<EnemySpawner>() != null) {
                spawner.SetActive(false);
                EnemySpawner spawnerScript = spawner.GetComponent<EnemySpawner>();
                spawnerScript.spawnTimer = spawnerScript.spawnTimerSet;
                spawnerScript.canSpawnEnemies = true;
                spawner.SetActive(true);
            }
        }
    }

    private void UpdateTimers() {
        //wheeeeeeeeeee
        dashTimer -= Time.deltaTime;
        wallJumpDashTimer -= Time.deltaTime;
        dashJumpTimer -= Time.deltaTime;
        dashCooldown -= Time.deltaTime;
        upDashTimer -= Time.deltaTime;
        wallJumpTimer -= Time.deltaTime;
        exitUpDashTimer -= Time.deltaTime;
        exitDownDashTimer -= Time.deltaTime;
        exitDashTimer -= Time.deltaTime;
        bulletTimer -= Time.deltaTime;
        bulletAnimationTimer -= Time.deltaTime;
        meleeTimer -= Time.deltaTime;
        meleeAnimationTimer -= Time.deltaTime;
        meleeCancelTimer -= Time.deltaTime;
        explosionTimer -= Time.deltaTime;
        styleDeductionTimer -= Time.deltaTime;
        styleDeductionCancelTimer -= Time.deltaTime;
        knockbackTimer -= Time.deltaTime;
        fireWaveShootTimer -= Time.deltaTime;
        soulClawShootTimer -= Time.deltaTime;
        groundedJumpNextToWallTimer -= Time.deltaTime;
        isDashing2IndicatorTimer -= Time.deltaTime;
        deathEffectPlayerHidingTimer -= Time.deltaTime;
        isDashing3Timer -= Time.deltaTime;
        highJumpTimer -= Time.deltaTime;
        highJumpTimer2 -= Time.deltaTime;
        oneSecondTimer -= Time.deltaTime;
        if(bulletTimer <= 0) {
            canShoot = true;
        }
        if(bulletAnimationTimer <= 0 && isShooting) {
            isShooting = false;
        }
        if(dashTimer <= 0) {
            if(isDashing && !isExploding && !beingKnockedBack) {
                rb.velocity = new Vector2(0, 0);
                isDamaging = false;
                if(!deathEffectIsHappening) {
                    rb.gravityScale = 4;
                }
                isDashing = false;
                exitDashTimer = exitDashTimerSet;
                isExitingDash = true;
            }
        }
        if(upDashTimer <= 0) {
            if(isUpDashing) {
                rb.velocity = new Vector2(0, 0);
                isDamaging = false;
                isUpDashing = false;
                exitUpDashTimer = exitUpDashTimerSet;
                isExitingUpDash = true;
            }
        }
        if(dashJumpTimer <= 0 && isDashJumping) {
            isDashJumping = false;
        }
        if(exitDashTimer <= 0) {
            isExitingDash = false;
        }
        if(exitUpDashTimer <= 0) {
            isExitingUpDash = false;
        }
        if(exitDownDashTimer <= 0) {
            isExitingDownDash = false;
        }
        if(wallJumpTimer <= 0) {
            wallJumping = false;
        }
        if(meleeTimer <= 0 && isMeleeAttacking) {
            isMeleeAttacking = false;
            playerMeleeCollider.canDoDamage = false;
        }
        if(meleeAnimationTimer <= 0 && anim.GetBool("isMeleeAttacking") == true) {
            anim.SetBool("isMeleeAttacking", false);
        }
        if(wallJumpDashTimer <= 0 && isDashing2) {
            isDashing2 = false;
        }
        if(knockbackTimer <= 0) {
            beingKnockedBack = false;
        }
        if(explosionTimer <= 0 && isExploding) {
            anim.SetBool("isExploding", false);
            isExploding = false;
            sr.sortingLayerName = "Player";
            sr.sortingOrder = 1;
            if(!deathEffectIsHappening) {
                rb.gravityScale = 4;
            }
            isDashing2 = false;
        }
        if(fireWaveShootTimer <= 0) {
            isShootingFireWave = false;
            if(!isShootingSoulClaw) {
                isShootingGroundAttack = false;
            }
        }
        if(soulClawShootTimer <= 0) {
            isShootingSoulClaw = false;
            if(!isShootingFireWave) {
                isShootingGroundAttack = false;
            }
        }
        if(styleDeductionTimer <= 0) {
            styleDeductionTimer = styleDeductionTimerSet;
            style -= 1;
            //only takes away a second style point if the player isn't using an attack, a movement ability, or moving with momentum from a dash jump
            if(styleDeductionCancelTimer <= 0 && !isDamaging && !isDashing && !isMeleeAttacking && !isDashJumping2 && !isExploding && !isUpDashing && !isDownDashing) {
                style -= 1;
                if(rb.velocity.x > -0.2f && rb.velocity.x < 0.2f && rb.velocity.y > -0.2f && rb.velocity.y < 0.2f) {
                    style -= 2;
                }
            }
        }
        if(multikillTimer <= 0 && canGainMultikillStyle) {
            canGainMultikillStyle = false;
            if(multikillCombo > 1) {
                style += Mathf.CeilToInt(multikillCombo * 1.5f);
                if(multikillCombo > 2) {
                    style += 2 * (multikillCombo - 2);
                }
                if(multikillCombo > 5) {
                    style += 3 * (multikillCombo - 5);
                }
                if(multikillCombo > 10) {
                    style += 4 * (multikillCombo - 10);
                }
            }
        }
        if(isDashing2IndicatorTimer <= 0) {
            isDashing2Indicator = false;
        }
        if(deathEffectPlayerHidingTimer <= 0 && deathEffectIsHappening) {
            sr.enabled = false;
        }
        if(isDashing3Timer <= 0) {
            isDashing3 = false;
        }
        if(highJumpTimer <= 0 && !isHighJumping && isAboutToHighJump) {
            if(touchingGround && isCrouching && !isJumping) {
                rb.velocity = new Vector2(0, highJumpStrength + Mathf.Abs(storedSlideVelocity) / 2);
                isHighJumping = true;
                isAboutToHighJump = false;
                isJumping = true;
            }
            else {
                isAboutToHighJump = false;
            }
        }
        if(highJumpTimer2 <= 0) {
            isAboutToHighJump2 = false;
        }
        if(oneSecondTimer <= 0) {
            OneSecondPassed();
            oneSecondTimer = 1;
        }
    }

    private void UpdateUI() {
        healthBar.position = health;
        if(sp > maxSP) {
            sp = maxSP;
        }
        spBar.position = sp;
        normalDash.position = dashesLeft;
        upDash.toggle = canUpDash;
        equippedPrimaryUI.toggle = primaryAttackIsMelee;
        //because of the weird way jumpsLeft works, the jump indicator's position has to be set manually instead of just being set to jumpsLeft
        if(!(touchingGround || touchingWall)) {
            jumps.position = jumpsLeft;
        }
        if(touchingGround || touchingWall) {
            jumps.position = 2;
        }
        if(currentRank != 0) {
            styleWithinRank = style - 25 * (currentRank);
        }
        if(currentRank == 0) {
            styleWithinRank = style;
        }
        styleBar.position = styleWithinRank;
        if(currentRank > currentRankUI.position && styleWithinRank < 25) {
            soundManager.PlayClip(soundManager.StyleRankUp, transform, 1);
        }
        currentRankUI.position = currentRank;
        if((style >= 25 * (currentRank + 1) && currentRank > 1) || (style >= 25 && currentRank == 0) || (style >= 50 && currentRank == 1)) {
            if(currentRank < 7) {
                currentRank += 1;
                sp += 2;
            }
        }
        if(style < 25 * (currentRank)) {
            currentRank -= 1;
        }
        if(style < 0) {
            style = 0;
        }
        if(style > 200) {
            style = 200;
        }
        //actually 8 because it starts at 0
        if(currentRank > 7) {
            currentRank = 7;
        }
        if(currentRank < 0) {
            currentRank = 0;
        }
        if(currentRank == 7 && styleWithinRank > 25) {
            styleWithinRank = 25;
        }
    }
    
    public void ResetStyleDeductionTimer() {
        styleDeductionTimer = styleDeductionTimerSet;
    }

    public void Heal(int healAmount, bool fullHeal) {
        if(!fullHeal) {
            health += healAmount;
        }
        if(fullHeal) {
            health = maxHealth;
        }
        soundManager.PlayClip(soundManager.PlayerHeal, transform, 3.5f);
    }

    public void OnHeal() {
        if(health < maxHealth && sp >= 12 && !timeScaleIsZero) {
           sp -= 12;
           style -= 3;
           recoilTimer = 0;
           recoiling = false;
           Heal(1, false);
        }
    }

    public void Hit(int damageDealt) {
        if(!isExploding) {
            recoiling = true;
            health -= damageDealt;
            if(damageDealt > 0) {
                soundManager.PlayClip(soundManager.PlayerHit, transform, 1);
            }
            effectTimer = effectCooldown;
            recoilTimer = 0.5f;
            sr.color = new Color(0, 0, 0, 1);
        }
    }

    public void KnockBack(float knockbackStrengthX, float knockbackStrengthY) {
        wallJumpDashTimer = wallJumpDashTimerSet;
        isDashing2 = true;
        wallJumping = false;
        beingKnockedBack = true;
        isHighJumping = false;
        isDashing = false;
        isUpDashing = false;
        isDownDashing = false;
        isShootingFireWave = false;
        isShootingSoulClaw = false;
        knockbackTimer = knockbackTimerSet;
        isDamaging = false;
        isExitingDownDash = false;
        isExitingUpDash = false;
        isExitingDash = false;
        if(!deathEffectIsHappening) {
            rb.velocity = new Vector2(knockbackStrengthX, knockbackStrengthY);
            rb.gravityScale = 4;
        }
    }

    public void UpdateEffect() {
        effectTimer -= Time.deltaTime;
        recoilTimer -= Time.deltaTime;
        if(effectTimer <= 0 && sr.color == new Color(0, 0, 0, 1) && !recoiling) {
            sr.color = new Color(0.45f, 0.45f, 0.45f, 0.75f);
        }
        if(recoilTimer > 0 && effectTimer <= 0) {
            sr.color = new Color(0.45f, 0.45f, 0.45f, 0.75f);
            recoiling = true;
        }
        if((recoilTimer <= 0) && sr.color == new Color(0.45f, 0.45f, 0.45f, 0.75f)) {
            sr.color = new Color(1, 1, 1, 1);
            recoiling = false;
        }
    }

    private void OnJump() {
        if(jumpsLeft > 0 && !isDownDashing && !isUpDashing && !isExploding && !beingKnockedBack && !timeScaleIsZero) {
            if(!touchingWall) {
                startedJumpWhileTouchingWall = false;
            }
            if(touchingWall && touchingGround && !isFalling && !isDashing) {
                startedJumpWhileTouchingWall = true;
            }
            if(!isDashing && !isCrouching) {
                isShootingSoulClaw = false;
                groundedJumpNextToWallTimer = 0.2f;
                rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
                if(isSliding && touchingGround) {
                    rb.velocity = new Vector2(slideVelocity * 1.15f, jumpStrength);
                }
                isJumping = true;
                soundManager.PlayClip(soundManager.PlayerJump, transform, 2);
            }
            if(isDashing && touchingGround && !touchingWall && !isExploding && !beingKnockedBack && !isDashJumping) {
                startedJumpWhileTouchingWall = false;
                isWalking = false;
                isShootingSoulClaw = false;
                isMeleeAttacking = false;
                anim.SetBool("isMeleeAttacking", false);
                dashJumpFacingDirX = facingDirX;
                isDashJumping = true;
                isDashJumping2 = true;
                isDashing = false;
                if(isCrouching) {
                    isCrouching = false;
                    slideVelocity = storedSlideVelocity;
                    storedSlideVelocity = 0;
                }
                if(isDamaging) {
                    isDamaging = false;
                }
                float extraSpeed = 0;
                if(isExitingDash) {
                    extraSpeed += 2.5f;
                }
                if(isExitingDownDash) {
                    extraSpeed += 2.5f;
                }
                rb.velocity = new Vector2((dashSpeed * 0.85f + Mathf.Abs(slideVelocity) * 0.45f + extraSpeed) * dashJumpFacingDirX, jumpStrength * 1.25f);
                dashJumpTimer = 0.35f;
                if(!deathEffectIsHappening) {
                    rb.gravityScale = 4;
                }
                soundManager.PlayClip(soundManager.PlayerJump, transform, 2);
            }
            if(touchingWall && !beingKnockedBack && !touchingGround) {
                startedJumpWhileTouchingWall = false;
                wallJumpTimer = wallJumpTimerSet;
                if(!deathEffectIsHappening) {
                    rb.gravityScale = 4;
                    rb.velocity = new Vector2(wallJumpKickback * wallFacingDirX, jumpStrength);
                    if(isSliding) {
                        rb.velocity = new Vector2((wallJumpKickback + Mathf.Abs(slideVelocity) * 0.9f) * wallFacingDirX, jumpStrength);
                    }
                    if(dirX == 0) {
                        facingDirX *= -1;
                    }
                }
                velocityBeforeMove = rb.velocity.x - (movementSpeed * dirX);
                wallJumping = true;
                isShooting = false;
                soundManager.PlayClip(soundManager.PlayerJump, transform, 2);
                jumpsLeft = 2;
            }
            if(isCrouching && !isDashing && !isDashJumping && touchingGround) {
                isShootingSoulClaw = false;
                highJumpTimer = highJumpTimerSet;
                highJumpTimer2 = highJumpTimer2Set;
                isAboutToHighJump = true;
                soundManager.PlayClip(soundManager.PlayerHighJump, transform, 2);
            }
            jumpsLeft -= 1;
        }
    }

    private void OnDash1() {
        if(dashesLeft > 0 && dashCooldown <= 0 && !isDashing && !(touchingWall && !isFalling && touchingGround && wallFacingDirX == facingDirX) && (!isMeleeAttacking || (isMeleeAttacking && meleeCancelTimer > 0)) && !isExploding && !isShooting && !beingKnockedBack && !timeScaleIsZero && !deathEffectIsHappening) {
            isShootingSoulClaw = false;
            if(isCrouching) {
                isCrouching = false;
                storedSlideVelocity = 0;
                slideVelocity = storedSlideVelocity;
            }
            wallJumpDashTimer = wallJumpDashTimerSet;
            if(touchingWall) {
                isDashing2 = true;
            }
            isDashing2IndicatorTimer = isDashing2IndicatorTimerSet;
            isDashing2Indicator = true;
            dashFacingDirX = wallFacingDirX;
            touchingWall = false;
            dashTimer = dashLength;
            dashCooldown = 0.5f;
            isWalking = false;
            isUpDashing = false;
            isHighJumping = false;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            dashesLeft -= 1;
            isDashing = true;
            isDashing3 = true;
            isDashing3Timer = isDashing3TimerSet;
            rb.gravityScale = 0;
            isDamaging = true;
            isShooting = false;
            isShootingFireWave = false;
            isDownDashing = false;
            isMeleeAttacking = false;
            playerDashCollider.canDoDamage = false;
            anim.SetBool("isMeleeAttacking", false);
            soundManager.PlayClip(soundManager.Dash, transform, 1f);
        }
    }

    private void OnUpDash() {
        if(canUpDash && !isDashing3 && !isUpDashing && !isDownDashing && (!isMeleeAttacking || (isMeleeAttacking && meleeCancelTimer > 0)) && !isExploding && !isShooting && !beingKnockedBack && !timeScaleIsZero) {
            upDashTimer = upDashLength;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            isDamaging = true;
            isDashJumping = false;
            isUpDashing = true;
            isShootingSoulClaw = false;
            canUpDash = false;
            isSliding = false;
            isShooting = false;
            isShootingFireWave = false;
            isMeleeAttacking = false;
            playerDashCollider.canDoDamage = false;
            anim.SetBool("isMeleeAttacking", false);
            soundManager.PlayClip(soundManager.Dash, transform, 1f);
        }
    }

    private void OnDownDash() {
        if(canDownDash && !isDashing3 && !isUpDashing && !isDownDashing && !touchingGround && (!isMeleeAttacking || (isMeleeAttacking && meleeCancelTimer > 0)) && !isExploding && !isShooting && !beingKnockedBack && !timeScaleIsZero) {
            storedSlideVelocity = slideVelocity;
            rb.velocity = new Vector2(0, 0);
            isDamaging = true;
            isShootingSoulClaw = false;
            isDashJumping = false;
            isDownDashing = true;
            canDownDash = false;
            isShooting = false;
            isShootingFireWave = false;
            isMeleeAttacking = false;
            playerDashCollider.canDoDamage = false;
            anim.SetBool("isMeleeAttacking", false);
            soundManager.PlayClip(soundManager.Dash, transform, 1f);
        }
        if(!isDashing3 && !isUpDashing && touchingGround && !isDownDashing && !isMeleeAttacking && !isExploding && !isShootingSoulClaw && !beingKnockedBack && !timeScaleIsZero) {
            if(!fireWave1.activeInHierarchy && !fireWave2.activeInHierarchy) {
                isSliding = false;
                fireWaveShootTimer = fireWaveShootTimerSet;
                isShootingFireWave = true;
                isShootingGroundAttack = true;
                rb.velocity = new Vector2(0, 0);
                isExitingDownDash = false;
                isExitingDash = false;
                playerDashCollider.canDoDamage = false;
                isMeleeAttacking = false;
                anim.SetBool("isMeleeAttacking", false);
                fireWave1.transform.position = new Vector2(transform.position.x + fireWaveOffsetX, transform.position.y + fireWaveOffsetY);
                fireWave2.transform.position = new Vector2(transform.position.x - fireWaveOffsetX, transform.position.y + fireWaveOffsetY);
                fireWave1.SetActive(true);
                fireWave2.SetActive(true);
                hasPlayedFireWaveDestroyedSound = false;
                soundManager.PlayClip(soundManager.PlayerFireWave, transform, 1);
                leftFireWaveHasHitEnemy = false;
                leftFireWaveHasKilledEnemy = false;
                leftFireWaveHasOneShottedEnemy = false;
                rightFireWaveHasHitEnemy = false;
                rightFireWaveHasKilledEnemy = false;
                rightFireWaveHasOneShottedEnemy = false;
                recoilTimer = 0;
                recoiling = false;
            }
        }
    }

    private void OnDashJump1() {
        if(!timeScaleIsZero) {
            if(dashesLeft > 0 && touchingGround) {
                if(!touchingWall && !isExploding && !beingKnockedBack) {
                    startedJumpWhileTouchingWall = false;
                    isWalking = false;
                    isShootingSoulClaw = false;
                    isMeleeAttacking = false;
                    anim.SetBool("isMeleeAttacking", false);
                    dashJumpFacingDirX = facingDirX;
                    isDashJumping = true;
                    isDashJumping2 = true;
                    isDashing = false;
                    if(isCrouching) {
                        isCrouching = false;
                        slideVelocity = storedSlideVelocity;
                        storedSlideVelocity = 0;
                    }
                    if(isDamaging) {
                        isDamaging = false;
                    }
                    float extraSpeed = 0;
                    if(isExitingDash) {
                        extraSpeed += 2.5f;
                    }
                    if(isExitingDownDash) {
                        extraSpeed += 2.5f;
                    }
                    rb.velocity = new Vector2((dashSpeed * 0.85f + Mathf.Abs(slideVelocity) * 0.45f + extraSpeed) * dashJumpFacingDirX, jumpStrength * 1.25f);
                    dashJumpTimer = 0.35f;
                    if(!deathEffectIsHappening) {
                        rb.gravityScale = 4;
                    }
                    soundManager.PlayClip(soundManager.PlayerJump, transform, 2);
                }
            }
            else {
                //OnJump();
            }
        }
    }

    private void OnSPattack() {
        if(sp >= 20 && !isExploding && !timeScaleIsZero && !deathEffectIsHappening) {
            sp -= 20;
            explosionTimer = explosionTimerSet;
            isExploding = true;
            recoilTimer = 0;
            recoiling = false;
            sr.color = new Color(1, 1, 1, 1);
            isDamaging = false;
            isDashing = false;
            isSliding = false;
            isWalking = false;
            touchingWall = false;
            isMeleeAttacking = false;
            isShooting = false;
            isUpDashing = false;
            isHighJumping = false;
            isDownDashing = false;
            isDashJumping = false;
            isDashJumping2 = false;
            isExitingDash = false;
            isExitingDownDash = false;
            isExitingUpDash = false;
            playerDashCollider.canDoDamage = false;
            playerMeleeCollider.canDoDamage = false;
            isDashing2 = true;
            effectTimer = 0;
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, 0);
            if(jumpsLeft < 2) {
                jumpsLeft = 1;
            }
            dashesLeft = 2;
            style += 2;
            canUpDash = true;
            canDownDash = true;
            sr.sortingLayerName = "Explosions";
            sr.sortingOrder = 20;
            soundManager.PlayClip(soundManager.PlayerExplosion, transform, 1);
        }
    }

    private void OnShoot() {
        if(!timeScaleIsZero) {
            GameObject bullet = BulletObjectPool.instance.GetPooledObject();
            if(bullet != null && canShoot && health > 0 && Time.timeScale == 1 && !isDamaging && !isDashing3 && !isMeleeAttacking && !isExploding && !isHighJumping) {
                isDashing = false;
                isCrouching = false;
                isShootingSoulClaw = false;
                temporaryBulletSpeed = bulletSpeed;
                temporaryBulletType = equippedBulletType;
                bullet.transform.position = transform.position;
                bullet.transform.localScale = new Vector2(transform.localScale.x, 1);
                bullet.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
                bullet.SetActive(true);
                bullet.GetComponent<PlayerBullet>().animationTimer = bullet.GetComponent<PlayerBullet>().animationTimerSet;
                bullet.GetComponent<PlayerBullet>().transitionCompleteTimerCheck = false;
                bullet.GetComponent<PlayerBullet>().transitionComplete = false;
                if(isExitingDash || (!isDashing3 && isDashing)) {
                    temporaryBulletSpeed = bulletSpeed * 1.75f;
                    if(!isDashing3 && isDashing) {
                        temporaryBulletSpeed = bulletSpeed * 2;
                    }
                }
                if(isSliding) {
                    if(Mathf.Abs(rb.velocity.x) * 0.75f > bulletSpeed) {
                        temporaryBulletSpeed = Mathf.Abs(rb.velocity.x) * 0.75f;
                    }
                    if((Mathf.Abs(rb.velocity.x) * 0.75f + bulletSpeed) > bulletSpeed && Mathf.Abs(rb.velocity.x) > 19) {
                        temporaryBulletSpeed = Mathf.Abs(rb.velocity.x) * 0.75f + bulletSpeed;
                        temporaryBulletType = 0.5f;
                    }
                    if((Mathf.Abs(rb.velocity.x) * 0.75f + bulletSpeed) > bulletSpeed && Mathf.Abs(rb.velocity.x) > 21) {
                        temporaryBulletSpeed = Mathf.Abs(rb.velocity.x) * 0.75f + bulletSpeed;
                        temporaryBulletType = 0.5f;
                    }
                    if((Mathf.Abs(rb.velocity.x) * 0.85f + bulletSpeed) > bulletSpeed && Mathf.Abs(rb.velocity.x) > 24.5f) {
                        temporaryBulletSpeed = Mathf.Abs(rb.velocity.x) * 0.85f + bulletSpeed;
                        temporaryBulletType = 0;
                    }
                }
                if(!isDashing2 && !isSliding && !isWalking && !isExitingDash && dirX == 0) {
                    temporaryBulletSpeed -= 1.5f;
                }
                bullet.GetComponent<PlayerBullet>().Shoot(gameObject.GetComponent<PlayerController>(), temporaryBulletType, temporaryBulletSpeed);
                bulletTimer = bulletCooldown;
                bulletAnimationTimer = bulletAnimationCooldown;
                canShoot = false;
                isDashJumping = false;
                isShooting = true;
                isExitingUpDash = false;
                isExitingDownDash = false;
                soundManager.PlayClip(soundManager.PlayerShoot, transform, 1);
            }
        }
    }

    private void OnMelee() {
        if(!isCrouching && !isDamaging && !isMeleeAttacking && !anim.GetBool("isMeleeAttacking") && !timeScaleIsZero && !isHighJumping && !isShootingGroundAttack) {
            meleeTimer = meleeCooldown;
            meleeAnimationTimer = meleeAnimationCooldown;
            meleeCancelTimer = meleeCancelTimerSet;
            isMeleeAttacking = true;
            playerMeleeCollider.canDoDamage = true;
            playerMeleeCollider.Attack(meleeAttackDamage, wallFacingDirX);
            anim.SetBool("isMeleeAttacking", true);
            isDamaging = false;
            dashTimer = 0;
            isDashJumping = false;
            isCrouching = false;
            soundManager.PlayClip(soundManager.PlayerMelee, transform, 1);
        }
        if(isCrouching && touchingGround && !isDownDashing && !isMeleeAttacking && !isExploding && !beingKnockedBack && !timeScaleIsZero && !isShootingGroundAttack) {
            if(!soulClaw.activeInHierarchy) {
                if(touchingEnemyRayCheck || touchingEnemyRayCheck2) {
                    isSliding = false;
                    soulClawShootTimer = soulClawShootTimerSet;
                    isShootingSoulClaw = true;
                    isShootingGroundAttack = true;
                    rb.velocity = new Vector2(0, 0);
                    isExitingDownDash = false;
                    isExitingDash = false;
                    playerDashCollider.canDoDamage = false;
                    isMeleeAttacking = false;
                    anim.SetBool("isMeleeAttacking", false);
                    if(touchingEnemyRayCheck) {
                        soulClaw.transform.position = new Vector2(touchingEnemyRay[0].point.x, transform.position.y + soulClawOffsetY);
                    }
                    if(touchingEnemyRayCheck2 && !touchingEnemyRayCheck) {
                        soulClaw.transform.position = new Vector2(touchingEnemyRay2[0].point.x, transform.position.y + soulClawOffsetY);
                    }
                    soulClaw.transform.localScale = new Vector2(facingDirX, 1);
                    soulClaw.SetActive(true);
                }
            }
        }
    }

    private void OnPrimary() {
        if(primaryAttackIsMelee && !timeScaleIsZero) {
            OnMelee();
        }
        if(!primaryAttackIsMelee && !timeScaleIsZero) {
            OnShoot();
        }
    }

    private void OnPrimarySwap() {
        if(!timeScaleIsZero) {
            primaryAttackIsMelee = !primaryAttackIsMelee;
            soundManager.PlayClip(soundManager.PrimarySwap, transform, 2);
        }
    }

    private void OnMenu1() {
        pauseUI.PausePressed();
    }

    private void OnMovement(InputValue input) {
        dirX = Mathf.Round(input.Get<Vector2>().x);
        dirY = Mathf.Round(input.Get<Vector2>().y);
        if(dirX == 0 && !isDashing && !isUpDashing && !isDownDashing && !isSliding && !wallJumping) {
            isWalking = false;
            if(!isDashJumping2) {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    private void UpdateAnimations() {
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isJumping", isJumping);
        anim.SetBool("isFalling", isFalling);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("isDashing", isDashing);
        anim.SetBool("isDownDashing", isDownDashing);
        anim.SetBool("isExitingUpDash", isExitingUpDash);
        anim.SetBool("isExitingDownDash", isExitingDownDash);
        anim.SetBool("isExitingDash", isExitingDash);
        anim.SetBool("isUpDashing", isUpDashing);
        anim.SetBool("isDamaging", isDamaging);
        anim.SetBool("isDashJumping", isDashJumping);
        anim.SetBool("hasFlipped", hasFlipped);
        anim.SetBool("isShooting", isShooting);
        anim.SetBool("isExploding", isExploding);
        anim.SetBool("isCrouching", isCrouching);
        anim.SetBool("isShootingFireWave", isShootingFireWave);
        //anim.SetBool("isShootingSoulClaw", isShootingSoulClaw);
        anim.SetBool("touchingWall", touchingWall);
        anim.SetBool("touchingGround", touchingGround);
        anim.SetBool("isHighJumping", isHighJumping);
        anim.SetBool("isAboutToHighJump", isAboutToHighJump2);
        //idle shooting/melee animation
        if(!(touchingGround && isWalking) && (!touchingWall || ((touchingWall && touchingGround)))) {
            anim.SetFloat("ShootBlend", 0);
            anim.SetFloat("MeleeBlend", 0);
        }
        //walking shooting/melee animation
        if(touchingGround && isWalking && !touchingWall) {
            anim.SetFloat("ShootBlend", 0.5f);
            anim.SetFloat("MeleeBlend", 0.5f);
        }
        //wall sliding shooting/melee animation
        if(touchingWall && !touchingGround && !(isJumping && startedJumpWhileTouchingWall)) {
            anim.SetFloat("MeleeBlend", 1);
            anim.SetFloat("ShootBlend", 1);
        }
        //sliding animation
        if(Mathf.Abs(rb.velocity.x) < 20) {
            anim.SetFloat("SlideBlend", 0);
        }
        if(Mathf.Abs(rb.velocity.x) >= 20) {
            anim.SetFloat("SlideBlend", 0.5f);
        }
        if(Mathf.Abs(rb.velocity.x) >= 24.5) {
            anim.SetFloat("SlideBlend", 1);
        }
    }

    public void OneSecondPassed() {
        if(Mathf.Abs(rb.velocity.x) > 22) {
            style += 1;
        }
        if(Mathf.Abs(rb.velocity.x) > 24.5) {
            style += 1;
        }
    }

    private void OnDrawGizmos() {
        //Gizmos.DrawWireCube(GameObject.FindWithTag("PlayerGroundCheck").GetComponent<Transform>().position, new Vector3(1.04f, 0.25f, 0.25f));
        //Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + 0.6f), new Vector2(transform.position.x + 1.1f, transform.position.y + 0.6f));
        //Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + 0.6f), new Vector2(transform.position.x + soulClawEnemyCheckLength, transform.position.y + 0.6f));
    }
}
