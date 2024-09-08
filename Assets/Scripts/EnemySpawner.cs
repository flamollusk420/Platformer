using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {
    public List<GameObject> enemyList = new List<GameObject>();
    private SpriteRenderer sr;
    private PlayerController player;
    private BarrierContainer barriers;
    [HideInInspector]
    public bool automaticallyPopulateEnemyList = true;
    public bool canSpawnEnemies = true;
    public bool canSpawnEnemiesAtStart = true;
    public bool changeColor = true;
    public bool repeatSpawning;
    public float spawnTimerSet;
    public float spawnTimer;
    public bool spawnEveryOtherRepetition;
    public bool repetitionToggle = true;
    public bool playerIsInRoom;
    public bool onlySpawnIfPlayerIsInRoom = true;
    public bool isRoom = true;
    public bool timerHasRunOut;
    [HideInInspector]
    public bool checkIfEnemiesAreDead;
    private bool hasSpawnedEnemies;
    private int deadEnemies;

    void OnEnable() {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        barriers = GameObject.FindWithTag("BarrierContainer").GetComponent<BarrierContainer>();
        deadEnemies = 0;
        hasSpawnedEnemies = false;
        if(!canSpawnEnemiesAtStart) {
            canSpawnEnemies = false;
        }
        if(automaticallyPopulateEnemyList) {
            for(int i = 0; i < transform.childCount; i++) {
                enemyList.Add(transform.GetChild(i).gameObject);
            }
        }
        if(changeColor) {
            sr.color = new Color(255, 255, 255, 0);
        }
    }
    
    private void UpdateTimer() {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer <= 0) {
            canSpawnEnemies = true;
            if(!repeatSpawning) {
                timerHasRunOut = true;
            }
        }
    }

    void Update() {
        if(player.currentRoomName != gameObject.name && isRoom) {
            playerIsInRoom = false;
        }
        if(player.currentRoomName == gameObject.name && isRoom) {
            playerIsInRoom = true;
        }
        if(!timerHasRunOut && spawnTimerSet > 0 && ((onlySpawnIfPlayerIsInRoom && playerIsInRoom) || (!onlySpawnIfPlayerIsInRoom))) {
            UpdateTimer();
        }
        if(canSpawnEnemies) {
            if((onlySpawnIfPlayerIsInRoom && playerIsInRoom) || !onlySpawnIfPlayerIsInRoom) {
                if(spawnTimer <= 0) {
                    SpawnEnemies();
                }
            }
        }
    }

    private void SpawnEnemies() {
        canSpawnEnemies = false;
        spawnTimer = spawnTimerSet;
        if((!repeatSpawning || (repeatSpawning && !spawnEveryOtherRepetition)) || (repeatSpawning && spawnEveryOtherRepetition && repetitionToggle)) {
            for(int i = 0; i < enemyList.Count; i++) {
                if(enemyList[i] != null && !(enemyList[i].GetComponent<Enemy>().onlySpawnIfDisabled == true && enemyList[i].gameObject.activeInHierarchy == true)) {
                    enemyList[i].gameObject.GetComponent<Enemy>().health = enemyList[i].gameObject.GetComponent<Enemy>().maxHealth;
                    enemyList[i].gameObject.GetComponent<Enemy>().effectTimer = enemyList[i].gameObject.GetComponent<Enemy>().effectCooldown;
                    enemyList[i].gameObject.GetComponent<Enemy>().hasBeenSpawned = true;
                    enemyList[i].gameObject.GetComponent<Enemy>().spawner = GetComponent<EnemySpawner>();
                    enemyList[i].gameObject.SetActive(false);
                    enemyList[i].gameObject.SetActive(true);
                    enemyList[i].transform.position = new Vector2(enemyList[i].GetComponent<Enemy>().startingX, enemyList[i].GetComponent<Enemy>().startingY);
                    hasSpawnedEnemies = true;
                }
            }
        }
    }

    public void CheckDeaths() {
        if(checkIfEnemiesAreDead && hasSpawnedEnemies) {
            for(int i = 0; i < enemyList.Count; i++) {
                if(!enemyList[i].gameObject.activeInHierarchy) {
                    deadEnemies += 1;
                }
                if(deadEnemies >= enemyList.Count) {
                    barriers.RemoveBarriers();
                }
            }
        }
    }
}
