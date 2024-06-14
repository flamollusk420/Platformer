using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingArea : MonoBehaviour {
    private SpriteRenderer sr;
    private PlayerController player;
    private Enemy enemy;
    public bool resetColor = true;
    public bool instakillPlayer;
    public bool instakillEnemies = true;
    public bool damagePlayer = true;
    public bool damageEnemies = true;
    public bool knockBackEnemies = true;
    public bool knockBackPlayer = true;
    public bool customEnemyKnockback = false;
    public float knockBackX;
    public float knockBackY;
    public bool ignoreImmunity = false;
    public bool ignoreKnockbackImmunity = false;
    public int damageDealtEnemies = 1;
    public int damageDealtPlayer = 1;
    private float playerKnockbackTimer;

    void Start() {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if(GetComponent<SpriteRenderer>() != null) {
            sr = GetComponent<SpriteRenderer>();
        }
    }

    private void OnEnable() {
        if(sr != null && resetColor) {
            sr.color = new Color(255, 255, 255, 0);
        }
    }

    private void Update() {
        playerKnockbackTimer -= Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D damageCollider) {
        if(damageCollider.CompareTag("Enemies") && damageEnemies) {
            enemy = damageCollider.gameObject.GetComponent<Enemy>();
            if(!enemy.immuneToDamageAreas || ignoreImmunity) {
                if(instakillEnemies) {
                    enemy.health = 0;
                    player.style += 4;
                    player.ResetStyleDeductionTimer();
                }
                else {
                    enemy.Hit(damageDealtEnemies);
                    player.style += 2;
                    player.ResetStyleDeductionTimer();
                    if(enemy.health <= 0) {
                        player.style += 2;
                    }
                }
            }
            if(knockBackEnemies && (enemy.canBeKnockedBack || ignoreKnockbackImmunity)) {
                if(customEnemyKnockback) {
                    if(transform.position.x > enemy.transform.position.x) {
                        enemy.KnockBack(true, 1, knockBackX, knockBackY);
                    }
                    if(transform.position.x < enemy.transform.position.x) {
                        enemy.KnockBack(true, -1, knockBackX, knockBackY);
                    }
                    if(transform.position.x == enemy.transform.position.x) {
                        enemy.KnockBack(true, 0, knockBackX, knockBackY);
                    }
                }
                if(!customEnemyKnockback) {
                    if(transform.position.x > enemy.transform.position.x) {
                        enemy.KnockBack(false, 1, 0, 0);
                    }
                    if(transform.position.x < enemy.transform.position.x) {
                        enemy.KnockBack(false, -1, 0, 0);
                    }
                    if(transform.position.x == enemy.transform.position.x) {
                        enemy.KnockBack(false, 0, 0, 0);
                    }
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D damageCollider) {
        if(damageCollider.CompareTag("PlayerSecondaryCollider") && damagePlayer && playerKnockbackTimer <= 0) {
            playerKnockbackTimer = 0.025f;
            if(!player.recoiling) {
                if(instakillPlayer) {
                    player.health = 0;
                }
                else {
                    player.Hit(damageDealtPlayer);
                    player.style -= damageDealtPlayer;
                    player.ResetStyleDeductionTimer();
                }
            }
            if(knockBackPlayer) {
                player.style -= 1;
                player.ResetStyleDeductionTimer();
                if(transform.position.x <= player.transform.position.x) {
                    player.KnockBack(knockBackX, knockBackY);
                }
                if(transform.position.x > player.transform.position.x) {
                    player.KnockBack(knockBackX * -1, knockBackY);
                }
            }
        }
    }
}
