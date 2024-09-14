using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ChangeRooms : MonoBehaviour {
    private CinemachineConfiner confiner;
    private CinemachineConfiner confiner2;
    private CinemachineConfiner confiner3;
    private CinemachineConfiner confiner4;
    private CinemachineVirtualCamera cam;
    private CinemachineVirtualCamera cam2;
    private CinemachineVirtualCamera cam3;
    private CinemachineVirtualCamera cam4;
    private LockCameraPos camLock;
    private LockCameraPos camLock2;
    private BarrierContainer barriers;
    //private bool camToggle = false;
    private float cameraNumber = 1;
    [HideInInspector]
    public bool oneTimeCameraMove;
    private GameObject collisionObject;
    private PlayerController player;
    private float cameraChangeTimer;
    public float cameraChangeTimerSet = 0.01f;
    private string oldRoomName;
    [HideInInspector]
    public float roomEntryTimer;
    [HideInInspector]
    public float roomEntryTimerSet = 0.2f;
    private bool tryingToLockPlayerIntoRoom = false;

    void Start() {
        cam = GameObject.FindWithTag("CMcam").GetComponent<CinemachineVirtualCamera>();
        cam2 = GameObject.FindWithTag("CMcam2").GetComponent<CinemachineVirtualCamera>();
        cam3 = GameObject.FindWithTag("CMcam3").GetComponent<CinemachineVirtualCamera>();
        cam4 = GameObject.FindWithTag("CMcam4").GetComponent<CinemachineVirtualCamera>();
        confiner = GameObject.FindWithTag("CMcam").GetComponent<CinemachineConfiner>();
        confiner2 = GameObject.FindWithTag("CMcam2").GetComponent<CinemachineConfiner>();
        confiner3 = GameObject.FindWithTag("CMcam3").GetComponent<CinemachineConfiner>();
        confiner4 = GameObject.FindWithTag("CMcam4").GetComponent<CinemachineConfiner>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        barriers = GameObject.FindWithTag("BarrierContainer").GetComponent<BarrierContainer>();
        cam2.gameObject.SetActive(false);
        cam3.gameObject.SetActive(false);
        cam4.gameObject.SetActive(false);
    }

    void Update() {
        cameraChangeTimer -= Time.deltaTime;
        roomEntryTimer -= Time.deltaTime;
        if(tryingToLockPlayerIntoRoom && roomEntryTimer <= 0) {
            tryingToLockPlayerIntoRoom = false;
            RoomVars varsForLockingRoom = collisionObject.GetComponent<RoomVars>();
            collisionObject.GetComponent<EnemySpawner>().CheckDeaths();
            barriers.LockPlayerInRoom(collisionObject, varsForLockingRoom.needsBarrierL, varsForLockingRoom.needsBarrierR, varsForLockingRoom.needsBarrierU, varsForLockingRoom.needsBarrierD, varsForLockingRoom.showBarrierL, varsForLockingRoom.showBarrierR, varsForLockingRoom.showBarrierU, varsForLockingRoom.showBarrierD);
            Debug.Log("ass");
        }
    }

    //swap between cameras when moving between rooms for a smooth transition
    void OnTriggerExit2D(Collider2D collision) {
        if(collisionObject != null && collisionObject.gameObject.CompareTag("Room") && cameraChangeTimer <= 0) {
            if(player.currentRoomName != collisionObject.name || oneTimeCameraMove) {
                oldRoomName = player.currentRoomName;
                cameraChangeTimer = cameraChangeTimerSet;
                if(oneTimeCameraMove) {
                    oneTimeCameraMove = false;
                }
                if(cameraNumber == 1) {
                    cam.gameObject.SetActive(true);
                    cam2.gameObject.SetActive(false);
                    cam3.gameObject.SetActive(false);
                    cam4.gameObject.SetActive(false);
                    confiner.m_BoundingShape2D = collisionObject.GetComponent<PolygonCollider2D>();
                }
                if(cameraNumber == 2) {
                    cam2.gameObject.SetActive(true);
                    cam.gameObject.SetActive(false);
                    cam3.gameObject.SetActive(false);
                    cam4.gameObject.SetActive(false);
                    confiner2.m_BoundingShape2D = collisionObject.GetComponent<PolygonCollider2D>();
                }
                if(cameraNumber == 3) {
                    cam3.gameObject.SetActive(true);
                    cam.gameObject.SetActive(false);
                    cam2.gameObject.SetActive(false);
                    cam4.gameObject.SetActive(false);
                    confiner3.m_BoundingShape2D = collisionObject.GetComponent<PolygonCollider2D>();
                }
                if(cameraNumber == 4) {
                    cam4.gameObject.SetActive(true);
                    cam.gameObject.SetActive(false);
                    cam2.gameObject.SetActive(false);
                    cam3.gameObject.SetActive(false);
                    confiner4.m_BoundingShape2D = collisionObject.GetComponent<PolygonCollider2D>();
                }
                cameraNumber += 1;
                if(cameraNumber > 4) {
                    cameraNumber = 1;
                }
                player.currentRoomName = collisionObject.name;
                if(player.respawnRoomName == "") {
                    player.respawnRoomName = collisionObject.name;
                }
                if(collisionObject.GetComponent<EnemySpawner>() != null) {
                    player.currentRoomSpawner = collisionObject.GetComponent<EnemySpawner>();
                }
                if(collisionObject.GetComponent<RoomVars>() != null && collisionObject.GetComponent<EnemySpawner>() != null) {
                    if(collisionObject.GetComponent<RoomVars>().lockPlayerInRoom && !collisionObject.GetComponent<EnemySpawner>().allEnemiesAreDead && !barriers.removingBarriers && !barriers.roomIsLocked) {
                        tryingToLockPlayerIntoRoom = true;
                    }
                }
            }
        }
    }

    void FixedUpdate() {
        if(player.startCompleted == true && collisionObject != null) {
            if(collisionObject.gameObject.CompareTag("Room") && cameraChangeTimer <= 0 && !player.sr.isVisible) {
                if(player.currentRoomName != collisionObject.name || oneTimeCameraMove) {
                    oldRoomName = player.currentRoomName;
                    cameraChangeTimer = cameraChangeTimerSet;
                    if(oneTimeCameraMove) {
                        oneTimeCameraMove = false;
                    }
                    if(cameraNumber == 1) {
                        cam.gameObject.SetActive(true);
                        cam2.gameObject.SetActive(false);
                        cam3.gameObject.SetActive(false);
                        cam4.gameObject.SetActive(false);
                        confiner.m_BoundingShape2D = collisionObject.GetComponent<PolygonCollider2D>();
                    }
                    if(cameraNumber == 2) {
                        cam2.gameObject.SetActive(true);
                        cam.gameObject.SetActive(false);
                        cam3.gameObject.SetActive(false);
                        cam4.gameObject.SetActive(false);
                        confiner2.m_BoundingShape2D = collisionObject.GetComponent<PolygonCollider2D>();
                    }
                    if(cameraNumber == 3) {
                        cam3.gameObject.SetActive(true);
                        cam.gameObject.SetActive(false);
                        cam2.gameObject.SetActive(false);
                        cam4.gameObject.SetActive(false);
                        confiner3.m_BoundingShape2D = collisionObject.GetComponent<PolygonCollider2D>();
                    }
                    if(cameraNumber == 4) {
                        cam4.gameObject.SetActive(true);
                        cam.gameObject.SetActive(false);
                        cam2.gameObject.SetActive(false);
                        cam3.gameObject.SetActive(false);
                        confiner4.m_BoundingShape2D = collisionObject.GetComponent<PolygonCollider2D>();
                    }
                    cameraNumber += 1;
                    if(cameraNumber > 4) {
                        cameraNumber = 1;
                    }
                    player.currentRoomName = collisionObject.name;
                    if(player.respawnRoomName == "") {
                        player.respawnRoomName = collisionObject.name;
                    }
                    if(collisionObject.GetComponent<EnemySpawner>() != null) {
                        player.currentRoomSpawner = collisionObject.GetComponent<EnemySpawner>();
                    }
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Room") && cameraChangeTimer <= 0) {
            if(player.currentRoomName != collision.gameObject.name || oneTimeCameraMove) {
                if(collisionObject != collision.gameObject) {
                    roomEntryTimer = roomEntryTimerSet;
                }
                collisionObject = collision.gameObject;
            }
        }
    }
}
