using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOnEnemyCollider : MonoBehaviour {
    private Enemy enemyScript;
    private LayerMask enemies;
    private bool touchingOtherEnemies;
    private BoxCollider2D boxCollider;
    private bool enemyScriptHasBeenSet;
    private float offsetX;
    private float offsetY;

    void OnEnable() {
        enemies = LayerMask.GetMask("Enemies");
        boxCollider = GetComponent<BoxCollider2D>();
        enemyScriptHasBeenSet = false;
    }


    public void CreateCollider(float enemyCheckOffsetX, float enemyCheckOffsetY, float enemyCheckSizeX, float enemyCheckSizeY, bool useCustomCollider, BoxCollider2D customCollider, Enemy enemyScriptSet) {
        enemyScript = enemyScriptSet;
        enemyScriptHasBeenSet = true;
        if(useCustomCollider) {
            boxCollider.size = customCollider.size;
            boxCollider.offset = customCollider.offset;
            boxCollider.sharedMaterial = customCollider.sharedMaterial;
        }
        if(!useCustomCollider) {
            boxCollider.size = new Vector2(enemyCheckSizeX, enemyCheckSizeY);
            offsetX = enemyCheckOffsetX;
            offsetY = enemyCheckOffsetY;
            boxCollider.sharedMaterial = enemyScript.customEnemyCheckColliderMaterial;
        }
    }

    void FixedUpdate() {
        if(enemyScriptHasBeenSet) {
            transform.position = new Vector2(enemyScript.transform.position.x + offsetX, enemyScript.transform.position.y + offsetY);
        }
    }

    void LateUpdate() {
        if(enemyScriptHasBeenSet) {
            transform.position = new Vector2(enemyScript.transform.position.x + offsetX, enemyScript.transform.position.y + offsetY);
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Enemies") && enemyScriptHasBeenSet && collision.gameObject != enemyScript.gameObject) {
            Enemy enemyScript2 = collision.GetComponent<Enemy>();
            if(enemyScript.beingKnockedBack && (enemyScript.rb.velocity.x > 2 || enemyScript.rb.velocity.x < -2)) {
                //enemyScript2.canTakeDamage = true;
                enemyScript2.Hit(enemyScript.enemySize);
                if(!enemyScript2.cannotBeLaunchedByEnemyOnEnemyCollisions && enemyScript2.canBeKnockedBack && !enemyScript2.beingKnockedBack) {
                    float knockbackYtoUse = 0;
                    if(enemyScript2.touchingGround && enemyScript.rb.velocity.y / 2 < enemyScript2.knockbackStrengthY / 2) {
                        knockbackYtoUse = enemyScript2.knockbackStrengthY / 2;
                    }
                    if(enemyScript2.touchingGround && enemyScript.rb.velocity.y / 2 > enemyScript2.knockbackStrengthY / 2) {
                        knockbackYtoUse = enemyScript.rb.velocity.y / 2;
                    }
                    if(!enemyScript2.touchingGround) {
                        knockbackYtoUse = enemyScript.rb.velocity.y * 1.25f;
                    }
                    if(enemyScript.rb.velocity.x < 0) {
                        enemyScript2.KnockBack(true, true, enemyScript.transform, -1, enemyScript.rb.velocity.x / 2, knockbackYtoUse, 1, 1);
                    }
                    if(enemyScript.rb.velocity.x > 0) {
                        enemyScript2.KnockBack(true, true, enemyScript.transform, 1, enemyScript.rb.velocity.x / 2, knockbackYtoUse, 1, 1);
                    }
                }
            }
        }
    }
}
