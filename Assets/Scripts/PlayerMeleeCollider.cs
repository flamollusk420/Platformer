using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMeleeCollider : MonoBehaviour {
    private SoundManager soundManager;
    private PlayerController player;
    private float damageDealt;
    private float dirX;
    public bool canDoDamage;

    void Start() {
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    public void Attack(float damageDealtPlayer, float playerDirX) {
        damageDealt = damageDealtPlayer;
        dirX = playerDirX;
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Enemies") && collision.GetComponent<Enemy>() != null && canDoDamage && !player.recoiling) {
            if(player.sp < player.maxSP && collision.gameObject.GetComponent<Enemy>().canTakeDamage) {
                player.sp += 2;
                if(collision.gameObject.GetComponent<Enemy>().beingKnockedBack) {
                    player.sp += 3;
                }
            }
            if(collision.gameObject.GetComponent<Enemy>().canTakeDamage) {
                player.style += 3;
                player.ResetStyleDeductionTimer();
                if(player.touchingWall && player.GetComponent<Rigidbody2D>().velocity.y < 0) {
                    player.style += 2;
                    player.sp += 1;
                }
                if(collision.gameObject.GetComponent<Enemy>().health - damageDealt <= 0) {
                    player.style += 1;
                    if(collision.gameObject.GetComponent<Enemy>().health == collision.gameObject.GetComponent<Enemy>().maxHealth) {
                        player.style += 2;
                    }
                }
                if(collision.gameObject.GetComponent<Enemy>().health - (damageDealt + 1) <= 0 && (collision.gameObject.GetComponent<Rigidbody2D>().velocity.y < -0.2f || collision.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0.2f)) {
                    player.style += 2;
                    if(collision.gameObject.GetComponent<Enemy>().health == collision.gameObject.GetComponent<Enemy>().maxHealth) {
                        player.style += 2;
                        player.sp += 1;
                    }
                }
            }
            collision.gameObject.GetComponent<Enemy>().Hit(damageDealt);
            soundManager.PlayClip(soundManager.PlayerMeleeHit, transform, 1f);
            if(collision.gameObject.GetComponent<Enemy>().canBeKnockedBack) {
                collision.gameObject.GetComponent<Enemy>().KnockBack(false, dirX, 0, 0);
            }
        }
        if(collision.CompareTag("EnemyBullet") && collision.GetComponent<EnemyBullet>() != null && canDoDamage && !collision.GetComponent<EnemyBullet>().fadingOut && !collision.GetComponent<EnemyBullet>().stopped) {
            EnemyBullet collisionBullet = collision.GetComponent<EnemyBullet>();
            if(collisionBullet.canBeDeflected) {
                if(collisionBullet.parryable) {
                    collisionBullet.Deflect();
                    if(player.isDashJumping2) {
                        player.style += 1;
                    }
                }
                if(!collisionBullet.parryable && collisionBullet.canBeDestroyedByPlayer) {
                    soundManager.PlayClip(soundManager.BulletCollision, transform, 1);
                    collisionBullet.canBeDeflected = false;
                    collisionBullet.deflectionTimer = 0.5f;
                    collisionBullet.stopped = true;
                    collisionBullet.fadingOut = true;
                    collisionBullet.fadeOutTimer = collisionBullet.fadeOutTimerSet;
                    FlexibleAnimation explosion = BulletCollisionObjectPool.instance.GetPooledObject().GetComponent<FlexibleAnimation>();
                    explosion.gameObject.SetActive(true);
                    explosion.currentFrame = 0;
                    explosion.transform.position = Vector3.Lerp(transform.position, collision.transform.position, 1);
                    player.style += 1;
                    player.sp += 1;
                }
            }
        }
    }
}
