using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private SoundManager soundManager;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator anim;
    private PlayerControls controls;
    public GameObject fireWave1;
    public GameObject fireWave2;
    public Transform groundCheck;
    public LayerMask ground;
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

    public float movementSpeed;
    public float dashSpeed;
    public float dashLength;
    public float upDashSpeed;
    public float upDashLength;
    public float downDashSpeed;
    public float jumpStrength;
    public float wallJumpKickback;
    public int health;
    public int maxHealth;
    public float sp;
    public float maxSP = 25;
    public float style;
    public float styleWithinRank;
    public int currentRank;
    public int jumpsLeft = 1;
    public int dashesLeft = 2;
    public float fireWaveOffsetX = 4.5f;
    public float fireWaveOffsetY = 0.06f;

    public bool respawned;
    public bool recoiling;
    public bool facingRight = true;
    public bool touchingGround;
    public bool touchingWall;
    public bool isJumping;
    public bool startedJumpWhileTouchingWall;
    //wallJumping is only true for the amount of time specified in wallJumpTimerSet
    //it's used to stop the player from walking into the wall by freezing movement while it's active
    //once it's turned off, any movement direction the player was holding will work again
    public bool wallJumping;
    public bool isFalling;
    public bool isWalking;
    public bool isDashing;
    //isDashing2 is used for disabling wall sliding while dashing off a wall and gets turned off a split second later
    //it's also used to prevent wall sliding while exploding and being knocked back
    public bool isDashing2;
    //isDashing2Indicator is used for enemies that need to detect if the player just started a dash
    [HideInInspector]
    public bool isDashing2Indicator;
    private float isDashing2IndicatorTimer;
    private float isDashing2IndicatorTimerSet = 0.1f;
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
    public bool hasFlipped;
    public bool primaryAttackIsMelee;

    private float velocityBeforeMove;
    public float dirX;
    public float dirY;
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
    public bool rightFireWaveHasHitEnemy = false;
    public bool rightFireWaveHasKilledEnemy = false;
    public bool rightFireWaveHasOneShottedEnemy = false;
    public bool leftFireWaveHasHitEnemy = false;
    public bool leftFireWaveHasKilledEnemy = false;
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

    public bool canShoot;
    public int equippedBulletType = 1;
    public float bulletSpeed = 3.5f;
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
        controls = new PlayerControls();
        sp = maxSP;
        health = maxHealth;
        styleDeductionTimer = styleDeductionTimerSet;
    }

    void FixedUpdate() {
        CheckCollisions();
        CheckState();
        MovePlayer();
        UpdateAnimations();
        UpdateTimers();
        UpdateUI();
        UpdateEffect();
    }

    private void MovePlayer() {
        if(isWalking && !wallJumping && !(touchingWall && dirX == wallFacingDirX * -1) && !(touchingWall && isDashing) && !(isDashJumping2 && dirX == dashJumpFacingDirX && Mathf.Abs(rb.velocity.x) > 0.2f) && !isExploding && !beingKnockedBack && !isShootingFireWave) {
            rb.velocity = new Vector2(movementSpeed * dirX, rb.velocity.y);
        }
        if(isWalking && wallJumping && !isExploding && !beingKnockedBack && !isShootingFireWave) {
            rb.velocity = new Vector2(velocityBeforeMove + (0.5f * (movementSpeed * dirX)), rb.velocity.y);
        }
        if(isDashing && !isExploding && !beingKnockedBack && !isShootingFireWave) {
            rb.velocity = new Vector2(dashSpeed * dashFacingDirX, 0);
        }
        if(isUpDashing && !isExploding && !beingKnockedBack && !isShootingFireWave) {
            rb.velocity = new Vector2(0, upDashSpeed);
        }
        if(isDownDashing && !isExploding && !beingKnockedBack && !isShootingFireWave) {
            rb.velocity = new Vector2(0, downDashSpeed);
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
        if(facingRight && !isDashing && !isExploding) {
            transform.localScale = new Vector2(1, 1);
            facingDirX = 1;
        }
        if(!facingRight && !isDashing && !isExploding) {
            transform.localScale = new Vector2(-1, 1);
            facingDirX = -1;
        }
        if(isDashing && facingDirX != dashFacingDirX) {
            facingDirX = dashFacingDirX;
            transform.localScale = new Vector2(facingDirX, 1);
        }
        if(rb.velocity.y < 0 && !isExploding && !beingKnockedBack) {
            isFalling = true;
            isJumping = false;
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
            isFalling = false;
            wallJumping = false;
            if(isDownDashing) {
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
        if(touchingWall && !touchingGround && !isDownDashing && !isUpDashing && !isExitingUpDash && !isExitingDownDash && !isExploding && !beingKnockedBack && !(isJumping && !isDashJumping && !isFalling && startedJumpWhileTouchingWall)) {
            wallFacingDirX = facingDirX * -1;
            rb.gravityScale = 0.65f;
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
        //this stops the player from sliding around like a hockey puck after landing from a dash jump
        if(dirX == 0 && touchingGround && !touchingWall && !isJumping && !isDamaging && !isDamaging && !isUpDashing && !isDownDashing && !isDashJumping && isDashJumping2) {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        if(!touchingWall && rb.gravityScale == 0.5f && !isExploding && !beingKnockedBack) {
            rb.gravityScale = 4;
        }
        if(touchingWall && wallJumping && !isExploding && !beingKnockedBack) {
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
        }
        if(isDamaging || isDashJumping || isExitingDownDash || isExitingUpDash) {
            if(playerDashCollider.canDoDamage == false && playerDashCollider.canDoDamage2 == false) {
                playerDashCollider.canDoDamage2 = true;
            }
            playerDashCollider.canDoDamage = true;
        }
        if(!isDamaging && !isDashJumping && !isExitingDownDash && !isExitingUpDash) {
            playerDashCollider.canDoDamage = false;
        }
        if(leftFireWaveHasHitEnemy && rightFireWaveHasHitEnemy) {
            leftFireWaveHasHitEnemy = false;
            rightFireWaveHasHitEnemy = false;
            style += 3;
            sp += 3;
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
    }

    private void CheckCollisions() {
        if(!isUpDashing) {
            touchingGround = Physics2D.OverlapBox(groundCheck.position, new Vector2(1.04f, 0.25f), 0, ground);
        }
        if(!isDashing) {
            touchingWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.65f), transform.right * facingDirX, 0.6f, ground);
        }
        if(isDashing && isDamaging) {
            touchingWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.65f), transform.right * facingDirX, 1.1f, ground);
        }
        if(isDashing2 && isDamaging) {
            touchingWall = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 0.65f), transform.right * facingDirX, 0, ground);
        }
    }

    private void UpdateTimers() {
        //there are a lot of timers
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
        groundedJumpNextToWallTimer -= Time.deltaTime;
        isDashing2IndicatorTimer -= Time.deltaTime;
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
                rb.gravityScale = 4;
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
        if(wallJumpTimer <= 0 && wallJumping) {
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
            isExploding = false;
            rb.gravityScale = 4;
            isDashing2 = false;
        }
        if(fireWaveShootTimer <= 0) {
            isShootingFireWave = false;
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
    }

    private void UpdateUI() {
        healthBar.position = health;
        spBar.position = sp;
        normalDash.position = dashesLeft;
        upDash.toggle = canUpDash;
        equippedPrimaryUI.toggle = primaryAttackIsMelee;
        //because of the weird way the jumpsLeft variable works in this game, the jump indicator's position has to be set manually instead of just being set to jumpsLeft
        if(!(touchingGround || touchingWall)) {
            jumps.position = jumpsLeft;
        }
        if(touchingGround || touchingWall) {
            jumps.position = 2;
        }
        if(currentRank != 0) {
            styleWithinRank = style - 25 * (currentRank - 1);
        }
        if(currentRank == 0) {
            styleWithinRank = style;
        }
        styleBar.position = styleWithinRank;
        if(currentRank > currentRankUI.position && styleWithinRank < 25) {
            soundManager.PlayClip(soundManager.StyleRankUp, transform, 1);
        }
        currentRankUI.position = currentRank;
        if((style >= 25 * currentRank && currentRank != 0) || (style >= 25 && currentRank == 0)) {
            if(currentRank < 7) {
                currentRank += 1;
                sp += 2;
            }
        }
        if(style < 25 * (currentRank - 1)) {
            currentRank -= 1;
        }
        if(style < 0) {
            style = 0;
        }
        if(style > 175) {
            style = 175;
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
        soundManager.PlayClip(soundManager.PlayerHeal, transform, 1);
    }

    public void OnHeal() {
        if(health < maxHealth && sp >= 7 && !timeScaleIsZero) {
           sp -= 7;
           style -= 3;
           Heal(1, false);
        }
    }

    public void Hit(int damageDealt) {
        if(!isExploding) {
            recoiling = true;
            health -= damageDealt;
            if(damageDealt != 0) {
                soundManager.PlayClip(soundManager.PlayerHit, transform, 1);
            }
            effectTimer = effectCooldown;
            sr.color = new Color(0, 0, 0, 1);
        }
    }

    public void KnockBack(float knockbackStrengthX, float knockbackStrengthY) {
        wallJumpDashTimer = wallJumpDashTimerSet;
        isDashing2 = true;
        wallJumping = false;
        beingKnockedBack = true;
        isDashing = false;
        isUpDashing = false;
        isDownDashing = false;
        knockbackTimer = knockbackTimerSet;
        rb.velocity = new Vector2(knockbackStrengthX, knockbackStrengthY);
        rb.gravityScale = 4;
        isDamaging = false;
        isExitingDownDash = false;
        isExitingUpDash = false;
        isExitingDash = false;
    }

    public void UpdateEffect() {
        effectTimer -= Time.deltaTime;
        recoilTimer -= Time.deltaTime;
        if(effectTimer <= 0 && sr.color == new Color(0, 0, 0, 1)) {
            sr.color = new Color(0.45f, 0.45f, 0.45f, 0.75f);
            recoilTimer = 0.5f;
        }
        if(recoilTimer <= 0 && sr.color == new Color(0.45f, 0.45f, 0.45f, 0.75f)) {
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
            if(!isDashing) {
                groundedJumpNextToWallTimer = 0.2f;
                rb.velocity = new Vector2(rb.velocity.x, jumpStrength);
                isJumping = true;
                soundManager.PlayClip(soundManager.PlayerJump, transform, 2);
            }
            if(isDashing && touchingGround && !touchingWall && !isExploding && !beingKnockedBack) {
                startedJumpWhileTouchingWall = false;
                isDashing = false;
                if(isDamaging) {
                    isDamaging = false;
                }
                isWalking = false;
                isMeleeAttacking = false;
                anim.SetBool("isMeleeAttacking", false);
                isDashJumping = true;
                isDashJumping2 = true;
                dashJumpFacingDirX = facingDirX;
                rb.velocity = new Vector2((dashSpeed * 0.85f) * dashJumpFacingDirX, jumpStrength * 1.25f);
                dashJumpTimer = 0.35f;
                rb.gravityScale = 4;
                soundManager.PlayClip(soundManager.PlayerJump, transform, 2);
            }
            if(touchingWall && !beingKnockedBack && !touchingGround) {
                startedJumpWhileTouchingWall = false;
                wallJumpTimer = wallJumpTimerSet;
                rb.gravityScale = 4;
                rb.velocity = new Vector2(wallJumpKickback * wallFacingDirX, jumpStrength);
                velocityBeforeMove = rb.velocity.x - (movementSpeed * dirX);
                wallJumping = true;
                isShooting = false;
                soundManager.PlayClip(soundManager.PlayerJump, transform, 2);
            }
            jumpsLeft -= 1;
        }
    }

    private void OnDash1() {
        if(dashesLeft > 0 && dashCooldown <= 0 && !isDashing && !(touchingWall && !isFalling && touchingGround && wallFacingDirX == facingDirX) && (!isMeleeAttacking || (isMeleeAttacking && meleeCancelTimer > 0)) && !isExploding && !isShooting && !beingKnockedBack && !timeScaleIsZero) {
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
            rb.velocity = new Vector2(rb.velocity.x, 0);
            dashesLeft -= 1;
            isDashing = true;
            rb.gravityScale = 0;
            isDamaging = true;
            isShooting = false;
            isDownDashing = false;
            isMeleeAttacking = false;
            playerDashCollider.canDoDamage = false;
            anim.SetBool("isMeleeAttacking", false);
            soundManager.PlayClip(soundManager.Dash, transform, 1f);
        }
    }

    private void OnUpDash() {
        if(canUpDash && !isDashing && !isDownDashing && (!isMeleeAttacking || (isMeleeAttacking && meleeCancelTimer > 0)) && !isExploding && !isShooting && !beingKnockedBack && !timeScaleIsZero) {
            upDashTimer = upDashLength;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            isDamaging = true;
            isDashJumping = false;
            isUpDashing = true;
            canUpDash = false;
            isMeleeAttacking = false;
            playerDashCollider.canDoDamage = false;
            anim.SetBool("isMeleeAttacking", false);
            soundManager.PlayClip(soundManager.Dash, transform, 1f);
        }
    }

    private void OnDownDash() {
        if(canDownDash && !isDashing && !isUpDashing && !touchingGround && (!isMeleeAttacking || (isMeleeAttacking && meleeCancelTimer > 0)) && !isExploding && !isShooting && !beingKnockedBack && !timeScaleIsZero) {
            rb.velocity = new Vector2(0, 0);
            isDamaging = true;
            isDashJumping = false;
            isDownDashing = true;
            canDownDash = false;
            isShooting = false;
            isMeleeAttacking = false;
            playerDashCollider.canDoDamage = false;
            anim.SetBool("isMeleeAttacking", false);
            soundManager.PlayClip(soundManager.Dash, transform, 1f);
        }
        if(!isDashing && !isUpDashing & sp > 0 && touchingGround && !isDownDashing && !isMeleeAttacking && !isExploding && !beingKnockedBack && !timeScaleIsZero) {
            if(!fireWave1.activeInHierarchy && !fireWave2.activeInHierarchy) {
                fireWaveShootTimer = fireWaveShootTimerSet;
                isShootingFireWave = true;
                rb.velocity = new Vector2(0, 0);
                isExitingDownDash = false;
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
            }
        }
    }

    private void OnDashJump1() {
        if(touchingGround && !timeScaleIsZero) {
            OnDash1();
            //if(isDashing && touchingGround && !touchingWall && (!isMeleeAttacking || (isMeleeAttacking && meleeCancelTimer > 0)) && !isExploding && !beingKnockedBack) {
            //    isDashing = false;
            //    if(isDamaging) {
            //        isDamaging = false;
            //       isDashJumping = true;
            //    }
            //    isDashJumping2 = true;
            //    isWalking = false;
            //    isMeleeAttacking = false;
            //    anim.SetBool("isMeleeAttacking", false);
            //   dashJumpFacingDirX = facingDirX;
            //    rb.velocity = new Vector2((dashSpeed * 0.85f) * dashJumpFacingDirX, jumpStrength * 1.25f);
            //    dashJumpTimer = 0.35f;
            //    rb.gravityScale = 4;
            //    playerDashCollider.canDoDamage = false;
            //}
            OnJump();
        }
        //if(!touchingGround && !timeScaleIsZero) {
        //    OnJump();
        //}
    }

    private void OnSPattack() {
        if(sp >= 7 && !isExploding && !timeScaleIsZero) {
            sp -= 7;
            explosionTimer = explosionTimerSet;
            isExploding = true;
            recoiling = false;
            isDamaging = false;
            isDashing = false;
            isWalking = false;
            touchingWall = false;
            isMeleeAttacking = false;
            isShooting = false;
            isUpDashing = false;
            isDownDashing = false;
            isDashJumping = false;
            isDashJumping2 = false;
            isExitingDash = false;
            isExitingDownDash = false;
            isExitingUpDash = false;
            playerDashCollider.canDoDamage = false;
            playerMeleeCollider.canDoDamage = false;
            isDashing2 = true;
            sr.color = new Color(1, 1, 1, 1);
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
            soundManager.PlayClip(soundManager.PlayerExplosion, transform, 1);
        }
    }

    private void OnShoot() {
        if(!timeScaleIsZero) {
            GameObject bullet = BulletObjectPool.instance.GetPooledObject();
            if(bullet != null && canShoot && health > 0 && Time.timeScale == 1 && !isDamaging && !isDashing && !isExitingDash && !isMeleeAttacking && !isExploding) {
                bullet.transform.position = transform.position;
                bullet.transform.localScale = new Vector2(transform.localScale.x, 1);
                bullet.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 1);
                bullet.SetActive(true);
                bullet.GetComponent<PlayerBullet>().animationTimer = bullet.GetComponent<PlayerBullet>().animationTimerSet;
                bullet.GetComponent<PlayerBullet>().transitionCompleteTimerCheck = false;
                bullet.GetComponent<PlayerBullet>().transitionComplete = false;
                bullet.GetComponent<PlayerBullet>().Shoot(gameObject.GetComponent<PlayerController>(), equippedBulletType, bulletSpeed);
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
        if(!isDamaging && !isMeleeAttacking && !anim.GetBool("isMeleeAttacking") && !timeScaleIsZero) {
            meleeTimer = meleeCooldown;
            meleeAnimationTimer = meleeAnimationCooldown;
            meleeCancelTimer = meleeCancelTimerSet;
            isMeleeAttacking = true;
            playerMeleeCollider.canDoDamage = true;
            playerMeleeCollider.Attack(meleeAttackDamage, facingDirX);
            anim.SetBool("isMeleeAttacking", true);
            isDamaging = false;
            dashTimer = 0;
            isDashJumping = false;
            soundManager.PlayClip(soundManager.PlayerMelee, transform, 1);
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
        if(dirX == 0 && !isDashing && !isUpDashing && !isDownDashing) {
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
        anim.SetBool("isShootingFireWave", isShootingFireWave);
        anim.SetBool("touchingWall", touchingWall);
        anim.SetBool("touchingGround", touchingGround);
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
        if(touchingWall && !touchingGround && (!isJumping && startedJumpWhileTouchingWall)) {
            anim.SetFloat("MeleeBlend", 1);
            anim.SetFloat("ShootBlend", 1);
        }
    }

    private void OnDrawGizmos() {
        //draws visual indicators in the editor for the raycast that checks if a wall is being touched and the box that checks if the ground is being touched
        //Gizmos.DrawWireCube(GameObject.FindWithTag("PlayerGroundCheck").GetComponent<Transform>().position, new Vector3(1.04f, 0.25f, 0.25f));
        //Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y + 0.6f), new Vector2(transform.position.x + 1.1f, transform.position.y + 0.6f));
    }
}
