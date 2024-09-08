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
            if(collision.gameObject.CompareTag("Enemies") && canDoDamage && !player.recoiling && collision.GetComponent<Enemy>().canTakeDamage) {
                collision.gameObject.GetComponent<Enemy>().Hit(0.5f);
                if(player.isDashJumping) {
                    collision.gameObject.GetComponent<Enemy>().Hit(0.25f);
                }
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
                    }
                }
                player.style += 1;
                player.sp += 1;
                canDoDamage2 = false;
            }
        }
    }
}
