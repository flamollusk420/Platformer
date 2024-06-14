using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashCollider : MonoBehaviour {
    public bool canDoDamage;
    public bool canDoDamage2;
    private bool canOneShot;
    private PlayerController player;
    private SoundManager soundManager;
    
    void Start() {
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void OnTriggerStay2D(Collider2D collision) {
        if(collision.GetComponent<Enemy>() != null) { 
            canOneShot = false;
            if(collision.gameObject.CompareTag("Enemies") && collision.GetComponent<Enemy>().health == collision.GetComponent<Enemy>().maxHealth) {
                canOneShot = true;
            }
            if(collision.gameObject.CompareTag("Enemies") && canDoDamage && canDoDamage2 && !player.recoiling && collision.GetComponent<Enemy>().canTakeDamage) {
                collision.gameObject.GetComponent<Enemy>().Hit(1);
                if(collision.GetComponent<Enemy>().health <= 0) {
                    if(player.isUpDashing) {
                        player.style += 3;
                    }
                    if(player.isDownDashing) {
                        player.style += 3;
                    }
                    if(canOneShot) {
                        canOneShot = false;
                        player.style += 1;
                        player.sp += 1;
                    }
                }
                player.style += 1;
                if(player.sp < player.maxSP) {
                    player.sp += 1;
                }
                canDoDamage2 = false;
            }
            if(collision.gameObject.CompareTag("EnemyBullet") && canDoDamage && canDoDamage2) {
                Debug.Log("fucjkashuioesaishitasbfucyusditbuctchithch");
                collision.GetComponent<EnemyBullet>().canBeDeflected = false;
                collision.GetComponent<EnemyBullet>().deflectionTimer = 0.5f;
                collision.GetComponent<EnemyBullet>().stopped = true;
                collision.GetComponent<EnemyBullet>().fadingOut = true;
                collision.GetComponent<EnemyBullet>().fadeOutTimer = collision.GetComponent<EnemyBullet>().fadeOutTimerSet;
                soundManager.PlayClip(soundManager.BulletDestroyedByDash, transform, 1);
            }
        }
    }
}
