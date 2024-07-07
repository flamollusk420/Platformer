using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkTowardsPlayerAndJump : MonoBehaviour {
    private Enemy enemyScript;
    public bool isWalking;
    public bool isJumping;
    [HideInInspector]
    public bool jumpSetup;
    public bool isFalling;
    public bool touchingGround;
    public bool touchingWall;
    public bool followPlayerWhileWalking = true;
    public bool bounceOfWallsIfNotFollowingPlayer = true;
    public int dirXifNotFollowingPlayer = 1;
    private int dirXifNotFollowingPlayerSet = 1;
    public float movementSpeed;
    public float jumpWalkSpeedMultiplier = 1;
    public float jumpHeight;
    public float jumpMoveDistance;
    public float range;
    public float jumpRange;
    public float jumpTimerSet;
    public float jumpTimer;
    private float enableTimer;
    public float enableTimerSet = 0.35f;
    private float jumpSetupTimer;
    private float doNotMoveWhileJumpingCheckTimer;
    public float doNotMoveWhileJumpingCheckTimerSet = 0.2f;
    public float faceDirectionInversionMultiplier = 1;
    public float jumpSetupTimerSet = 0.35f;
    public float deadZoneRange = 1;
    public bool endKnockbackTimerOnHitGround = true;
    public bool doNotWalk = false;
    public bool setAnimatorWalkBool = true;
    public bool setAnimatorJumpBool = true;
    public bool setAnimatorFallBool;
    public bool setAnimatorJumpSetupBool;
    public bool setAnimatorAttackBool;
    public bool attackWhileJumping;
    public bool attackWhileWalking;
    private float attackTimer;
    public float attackTimerSet;
    public bool resetJumpTimerOnLand;
    public bool resetJumpTimerWhileBeingKnockedBack;
    public bool needsToBeWithinRange;
    public bool needsToBeWithinRangeForJump;
    public bool canChangeDirection = true;
    public bool cannotChangeDirectionInAir = false;
    public bool facePlayer = true;
    public bool facePlayerWhileJumpMoving;
    public bool invertFaceDirection;
    public bool initialNonDamageTimerHasToBeZero = true;
    private float wallBounceTimer;
    private float wallBounceTimerSet = 0.2f;
    public bool setBlendFloats;
    public bool setIsWalkingBlendFloat;
    public bool setIsJumpingBlendFloat;
    public bool setIsFallingBlendFloat;
    public bool setIsAttackingBlendFloat;
    public string customBlendFloatName = "Blend";
    public bool jumpsOnTimer;
    public bool jumpsOnStartWalking;
    public bool jumpsOnPlayerDash;
    public bool jumpsOnPlayerMelee;
    public bool jumpsOnPlayerShoot;
    public bool stopOnLand;
    //disabling/enabling this doesn't disable/enable jumping, it just says when the enemy can jump
    private bool canJump;
    public bool stopWhenNotWithinRange = false;
    private bool stopWhenNotWithinRangeCheck = false;
    public bool canSlideFromKnockback;
    private bool canSlideFromKnockbackCheck = false;
    public bool needsToBeVisibleToMove = true;
    public bool disableNeedingVisibilityAfterMoving = true;
    private bool initialNeedsToBeVisibleToMove = true;
    private bool startCompleted = false;
    private bool jumpWalking = false;
    //if both of these are true, the enemy will move in both states
    public bool onlyMoveWhileJumping;
    public bool onlyMoveWhileFalling;
    public bool doNotMoveWhileJumping;
    private int jumpWalkDirectionIfCannotChangeDirectionInAir;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private Animator anim;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        enemyScript = GetComponent<Enemy>();
        if(GetComponent<Animator>() != null) {
            anim = GetComponent<Animator>();
        }
        playerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        if(jumpRange == 0) {
            jumpRange = range;
        }
        initialNeedsToBeVisibleToMove = needsToBeVisibleToMove;
        if(setIsWalkingBlendFloat || setIsJumpingBlendFloat || setIsFallingBlendFloat || setIsAttackingBlendFloat) {
            setBlendFloats = true;
        }
        if(customBlendFloatName == "" || customBlendFloatName == " ") {
            customBlendFloatName = "Blend";
        }
        dirXifNotFollowingPlayerSet = dirXifNotFollowingPlayer;
        startCompleted = true;
    }

    void OnEnable() {
        enableTimer = enableTimerSet;
        if(!invertFaceDirection) {
            faceDirectionInversionMultiplier = 1;
        }
        if(invertFaceDirection) {
            faceDirectionInversionMultiplier = -1;
        }
        if(startCompleted) {
            needsToBeVisibleToMove = initialNeedsToBeVisibleToMove;
            dirXifNotFollowingPlayer = dirXifNotFollowingPlayerSet;
        }
        stopWhenNotWithinRangeCheck = false;
        canSlideFromKnockbackCheck = false;
    }

    void FixedUpdate() {
        DetectGround();
        enableTimer -= Time.deltaTime;
        if((initialNonDamageTimerHasToBeZero && enemyScript.initialNonDamageTimer <= 0) || !initialNonDamageTimerHasToBeZero) {
            jumpTimer -= Time.deltaTime;
            wallBounceTimer -= Time.deltaTime;
            jumpSetupTimer -= Time.deltaTime;
            doNotMoveWhileJumpingCheckTimer -= Time.deltaTime;
            attackTimer -= Time.deltaTime;
            Walk();
            if(jumpsOnPlayerDash) {
                if(playerTransform.GetComponent<PlayerController>().isDashing2Indicator == true) {
                    JumpSetup();
                }
            }
            if(jumpsOnPlayerMelee) {
                if(playerTransform.GetComponent<PlayerController>().isMeleeAttacking == true) {
                    JumpSetup();
                }
            }
            if(jumpsOnPlayerShoot) {
                if(playerTransform.GetComponent<PlayerController>().isShooting == true) {
                    JumpSetup();
                }
            }
            if(jumpsOnTimer && resetJumpTimerWhileBeingKnockedBack) {
                jumpTimer = jumpTimerSet;
            }
            if(jumpTimer <= 0 && jumpsOnTimer) {
                JumpSetup();
            }
            if(touchingGround) {
                if(resetJumpTimerOnLand && !canJump) {
                    jumpTimer = jumpTimerSet;
                }
                if(stopOnLand && !canJump) {
                    rb.velocity = new Vector2(0, 0);
                }
                if(endKnockbackTimerOnHitGround && !canJump) {
                    if(enemyScript.beingKnockedBack) {
                        enemyScript.knockbackTimer = 0;
                    }
                }
                canJump = true;
                if(setAnimatorJumpBool && anim != null) {
                    anim.SetBool("isJumping", false);
                }
                if(setAnimatorFallBool && anim != null) {
                    anim.SetBool("isFalling", false);
                }
                isJumping = false;
                isFalling = false;
                jumpWalking = false;
            }
            if(!touchingGround) {
                canJump = false;
            }
            if(!touchingGround) {
                if(setAnimatorJumpBool && anim != null) {
                    anim.SetBool("isJumping", true);
                }
                isJumping = true;
            }
            if(rb.velocity.y < -0.2f && isJumping) {
                if(setAnimatorFallBool && anim != null) {
                    anim.SetBool("isFalling", true);
                }
                isFalling = true;
            }
            if(rb.velocity.y >= 0) {
                if(setAnimatorFallBool && anim != null) {
                    anim.SetBool("isFalling", false);
                }
                isFalling = false;
            }
        }
        if(enemyScript.beingKnockedBack) {
            canSlideFromKnockbackCheck = false;
            if(!canSlideFromKnockback && rb.velocity.y > -0.2f && rb.velocity.y < 0.2f && touchingGround && !canSlideFromKnockbackCheck) {
                rb.velocity = new Vector2(0, rb.velocity.y);
                canSlideFromKnockbackCheck = true;
            }
            if(jumpWalking) {
                jumpWalking = false;
            }
        }
        if(attackWhileWalking && isWalking) {
            if(!anim.GetBool("isAttacking")) {
                attackTimer = attackTimerSet;
                anim.SetBool("isAttacking", true);
            }
        }
        if(attackWhileJumping && isJumping) {
            if(!anim.GetBool("isAttacking")) {
                attackTimer = attackTimerSet;
                anim.SetBool("isAttacking", true);
            }
        }
        if(attackTimer <= 0 && setAnimatorAttackBool) {
            anim.SetBool("isAttacking", false);
        }
        if(touchingWall && wallBounceTimer <= 0) {
            wallBounceTimer = wallBounceTimerSet;
            dirXifNotFollowingPlayer *= -1;
            enemyScript.wallCheckDirectionMultiplier *= -1;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
        if(setBlendFloats && startCompleted) {
            if(setIsWalkingBlendFloat && isWalking) {
                anim.SetFloat(customBlendFloatName, 1);
            }
            if(setIsWalkingBlendFloat && !isWalking) {
                anim.SetFloat(customBlendFloatName, 0);
            }
            if(setIsJumpingBlendFloat && isJumping) {
                anim.SetFloat(customBlendFloatName, 1);
            }
            if(setIsJumpingBlendFloat && !isJumping) {
                anim.SetFloat(customBlendFloatName, 0);
            }
            if(setIsFallingBlendFloat && isFalling) {
                anim.SetFloat(customBlendFloatName, 1);
            }
            if(setIsFallingBlendFloat && !isFalling) {
                anim.SetFloat(customBlendFloatName, 0);
            }
            if(setIsAttackingBlendFloat && anim.GetBool("isAttacking")) {
                anim.SetFloat(customBlendFloatName, 1);
            }
            if(setIsAttackingBlendFloat && !anim.GetBool("isAttacking")) {
                anim.SetFloat(customBlendFloatName, 0);
            }
        }
    }

    private void DetectGround() {
        if(startCompleted) {
            touchingGround = enemyScript.touchingGround;
            touchingWall = enemyScript.touchingWall;
        }
    }

    private void Walk() {
        if(jumpWalkDirectionIfCannotChangeDirectionInAir != 0 && !jumpWalking) {
            jumpWalkDirectionIfCannotChangeDirectionInAir = 0;
        }
        if(!(doNotMoveWhileJumping && isJumping) && !(needsToBeVisibleToMove && !sr.isVisible) && enemyScript.deathJumpTimer <= 0 && !doNotWalk && doNotMoveWhileJumpingCheckTimer <= 0) {
            if((!onlyMoveWhileJumping && !onlyMoveWhileFalling) || (onlyMoveWhileFalling && isFalling) || (onlyMoveWhileJumping && isJumping)) {
                if(!((playerTransform.position.x - deadZoneRange < transform.position.x) && (playerTransform.position.x + deadZoneRange > transform.position.x))) {
                    if(followPlayerWhileWalking && playerTransform.position.x < transform.position.x && enemyScript.health > 0 && enemyScript.beingKnockedBack == false && !(cannotChangeDirectionInAir && isJumping)) {
                        if(Vector3.Distance(playerTransform.position, transform.position) <= range || !needsToBeWithinRange) {
                            rb.velocity = new Vector2(movementSpeed * -1, rb.velocity.y);
                            if(facePlayer) {
                                transform.localScale = new Vector2(1 * faceDirectionInversionMultiplier, 1);
                            }
                            if(isWalking == false && jumpsOnStartWalking) {
                                JumpSetup();
                            }
                        }
                    }
                    if(followPlayerWhileWalking && playerTransform.position.x > transform.position.x && enemyScript.health > 0 && enemyScript.beingKnockedBack == false && !(cannotChangeDirectionInAir && isJumping)) {
                        if(Vector3.Distance(playerTransform.position, transform.position) <= range || !needsToBeWithinRange) {
                            rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
                            if(facePlayer) {
                                transform.localScale = new Vector2(-1 * faceDirectionInversionMultiplier, 1);
                            }
                            if(isWalking == false && jumpsOnStartWalking) {
                                JumpSetup();
                            }
                        }
                    }
                    if(!followPlayerWhileWalking && enemyScript.health > 0 && enemyScript.beingKnockedBack == false && !(cannotChangeDirectionInAir && isJumping)) {
                        if(Vector3.Distance(playerTransform.position, transform.position) <= range || !needsToBeWithinRange) {
                            rb.velocity = new Vector2(movementSpeed * dirXifNotFollowingPlayer, rb.velocity.y);
                            if(isWalking == false && jumpsOnStartWalking) {
                                JumpSetup();
                            }
                        }
                    }
                    if(disableNeedingVisibilityAfterMoving && needsToBeVisibleToMove) {
                        needsToBeVisibleToMove = false;
                    }
                }
                if(followPlayerWhileWalking && cannotChangeDirectionInAir && isJumping) {
                    if(playerTransform.position.x < transform.position.x && jumpWalkDirectionIfCannotChangeDirectionInAir == 0 && !jumpWalking) {
                        jumpWalkDirectionIfCannotChangeDirectionInAir = -1;
                    }
                    if( playerTransform.position.x > transform.position.x && jumpWalkDirectionIfCannotChangeDirectionInAir == 0 && !jumpWalking) {
                        jumpWalkDirectionIfCannotChangeDirectionInAir = 1;
                    }
                    if(enemyScript.beingKnockedBack == false && jumpWalkDirectionIfCannotChangeDirectionInAir != 0) {
                        jumpWalking = true;
                        rb.velocity = new Vector2(movementSpeed * jumpWalkDirectionIfCannotChangeDirectionInAir, rb.velocity.y);
                    }
                }
                if(isJumping) {
                    rb.velocity = new Vector2(rb.velocity.x * jumpWalkSpeedMultiplier, rb.velocity.y);
                }
            }
        }
        if(anim != null && setAnimatorWalkBool) {
            anim.SetBool("isWalking", true);
        }
        isWalking = true;
        if(!(Vector3.Distance(playerTransform.position, transform.position) <= range)) {
            if(needsToBeWithinRange) {
                if(setAnimatorWalkBool) {
                    anim.SetBool("isWalking", false);
                }
                isWalking = false;
            }
            if(stopWhenNotWithinRange && !stopWhenNotWithinRangeCheck) {
                transform.position = new Vector2(0, 0);
                stopWhenNotWithinRangeCheck = true;
            }
        }
        if(stopWhenNotWithinRange) {
            if(Vector3.Distance(playerTransform.position, transform.position) <= range) {
                stopWhenNotWithinRangeCheck = false;
            }
        }
    }

    public void JumpSetup() {
        if(!(needsToBeVisibleToMove && !sr.isVisible)) {
            if(anim == null || !setAnimatorJumpSetupBool) {
                Jump();
            }
            if(anim != null && setAnimatorJumpSetupBool) {
                if(!jumpSetup) {
                    jumpSetupTimer = jumpSetupTimerSet;
                }
            }
            jumpSetup = true;
            if(setAnimatorJumpSetupBool && anim != null) {
                anim.SetBool("jumpSetup", true);
            }
            if(jumpSetupTimer <= 0) {
                if(anim != null && setAnimatorJumpSetupBool) {
                    anim.SetBool("jumpSetup", false);
                }
                jumpSetup = false;
                Jump();
            }
        }
    }

    public void Jump() {
        if(canJump && enemyScript.health > 0 && enemyScript.beingKnockedBack == false && !(needsToBeVisibleToMove && !sr.isVisible)) {
            if(doNotMoveWhileJumping) {
                doNotMoveWhileJumpingCheckTimer = doNotMoveWhileJumpingCheckTimerSet;
            }
            jumpTimer = jumpTimerSet;
            if((needsToBeWithinRangeForJump && (Vector3.Distance(playerTransform.position, transform.position) <= jumpRange)) || !needsToBeWithinRangeForJump) {
                isJumping = true;
                canJump = false;
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight);
                if(jumpMoveDistance != 0) {
                    if(playerTransform.position.x < transform.position.x) {
                        if(Vector3.Distance(playerTransform.position, transform.position) <= range || !needsToBeWithinRange) {
                            rb.velocity = new Vector2(jumpMoveDistance * -1, rb.velocity.y);
                            if(facePlayerWhileJumpMoving) {
                                transform.localScale = new Vector2(1, 1);
                            }
                        }
                    }
                    if(playerTransform.position.x > transform.position.x) {
                        if(Vector3.Distance(playerTransform.position, transform.position) <= range || !needsToBeWithinRange) {
                            rb.velocity = new Vector2(jumpMoveDistance, rb.velocity.y);
                            if(facePlayerWhileJumpMoving) {
                                transform.localScale = new Vector2(-1, 1);
                            }
                        }
                    }
                }
                if(setAnimatorJumpBool) {
                    anim.SetBool("isJumping", true);
                }
                if(disableNeedingVisibilityAfterMoving && needsToBeVisibleToMove) {
                    needsToBeVisibleToMove = false;
                }
            }
        }
    }
}
