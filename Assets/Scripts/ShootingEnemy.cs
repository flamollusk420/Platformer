using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : MonoBehaviour {
    private SoundManager soundManager;
    private Animator anim;
    private float bulletTimer;
    private float bulletTimer2;
    private float animationTimer;
    public float bulletCooldown = 0.35f;
    public float timeUntilNextBurst = 3;
    public float animationTimeBeforeBurst;
    public AnimationClip shootAnimation;
    public float animationCutoff;
    private float animationLength;
    public bool shootsOnTimer = true;
    public bool customAnimationTimeBeforeBurst;
    public int timesToShoot = 3;
    private int timesShot = 0;
    private bool canShoot;
    public bool bulletHasCustomBoxColliderShape;
    public BoxCollider2D customBoxColliderShape;
    //public int equippedBulletType = 1;
    public float dirX = -1;
    public float dirY = 0;
    public float offsetX = 0;
    public float offsetY = 0;
    public bool customDamageDealt = false;
    public int damageDealt;
    public float movementSpeed;
    public bool bulletStaysWithEnemyBeforeBeingShot = false;
    public bool flipBulletX = false;
    public bool flipBulletY = false;
    public bool customBulletFrameList;
    public bool bulletCanTouchLevel = false;
    public float customTimeBetweenFrames;
    public List<Sprite> frameListBullet = new List<Sprite>();
    public bool hasCustomShootSound;
    public float customShootSoundVolume = 1;
    public AudioClip customShootSound;

    void Start() {
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        if(GetComponent<Animator>() != null) {
            anim = GetComponent<Animator>();
        }
        animationLength = shootAnimation.length - animationCutoff;
        if(!customAnimationTimeBeforeBurst) {
            animationTimeBeforeBurst = animationLength;
        }
    }

    void Update() {
        bulletTimer -= Time.deltaTime;
        bulletTimer2 -= Time.deltaTime;
        animationTimer -= Time.deltaTime;
        if(bulletTimer <= 0) {
            canShoot = true;
        }
        if(GetComponent<Enemy>() == null || (GetComponent<Enemy>().deathJumpTimer <= 0)) {
            if(canShoot && bulletTimer2 <= 0) {
                Shoot();
            }
            if(bulletTimer2 - animationTimeBeforeBurst <= 0 && anim != null && animationTimer <= 0) {
                anim.SetBool("isShooting", true);
                animationTimer = animationLength;
            }
        }
        if(anim.GetBool("isShooting") == true && animationTimer <= 0) {
            anim.SetBool("isShooting", false);
        }
    }

    public void Shoot() {
        GameObject bullet = EnemyBulletObjectPool.instance.GetPooledObject();
        if(bullet != null) {
            bullet.transform.position = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);
            bullet.SetActive(true);
            if(!bulletHasCustomBoxColliderShape) {
                customBoxColliderShape = GetComponent<BoxCollider2D>();
            }
            bullet.GetComponent<EnemyBullet>().Shoot(dirX, dirY, damageDealt, movementSpeed, flipBulletX, flipBulletY, bulletStaysWithEnemyBeforeBeingShot, bulletCanTouchLevel, customBulletFrameList, frameListBullet, customTimeBetweenFrames, GetComponent<ShootingEnemy>(), GetComponent<Enemy>(), bulletHasCustomBoxColliderShape, customBoxColliderShape);
            bulletTimer = bulletCooldown;
            canShoot = false;
            timesShot += 1;
            if(hasCustomShootSound) {
                soundManager.PlayClip(customShootSound, transform, customShootSoundVolume);
            }
            if(timesShot >= timesToShoot) {
                timesShot = 0;
                bulletTimer2 = timeUntilNextBurst;
            }
        }
    }
}
