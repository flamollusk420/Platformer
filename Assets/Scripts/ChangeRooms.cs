using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ChangeRooms : MonoBehaviour {
    private CinemachineConfiner confiner;
    private CinemachineConfiner confiner2;
    private CinemachineVirtualCamera cam;
    private CinemachineVirtualCamera cam2;
    private LockCameraPos camLock;
    private LockCameraPos camLock2;
    private bool camToggle = false;
    [HideInInspector]
    public bool oneTimeCameraMove;
    private PlayerController player;

    void Start() {
        cam = GameObject.FindWithTag("CMcam").GetComponent<CinemachineVirtualCamera>();
        cam2 = GameObject.FindWithTag("CMcam2").GetComponent<CinemachineVirtualCamera>();
        confiner = GameObject.FindWithTag("CMcam").GetComponent<CinemachineConfiner>();
        confiner2 = GameObject.FindWithTag("CMcam2").GetComponent<CinemachineConfiner>();
        camLock = GameObject.FindWithTag("CMcam").GetComponent<LockCameraPos>();
        camLock2 = GameObject.FindWithTag("CMcam2").GetComponent<LockCameraPos>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        cam2.gameObject.SetActive(false);
    }

    //swap between cameras when moving between rooms for a smooth transition
    void OnTriggerStay2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Room")) {
            if(player.currentRoomName != collision.gameObject.name || oneTimeCameraMove) {
                if(oneTimeCameraMove) {
                    oneTimeCameraMove = false;
                }
                if(camToggle == true) {
                    cam.gameObject.SetActive(true);
                    cam2.gameObject.SetActive(false);
                    confiner.m_BoundingShape2D = collision.gameObject.GetComponent<PolygonCollider2D>();
                }
                if(camToggle == false) {
                    cam2.gameObject.SetActive(true);
                    cam.gameObject.SetActive(false);
                    confiner2.m_BoundingShape2D = collision.gameObject.GetComponent<PolygonCollider2D>();
                }
                camToggle = !camToggle;
                player.currentRoomName = collision.gameObject.name;
                if(player.respawnRoomName == "") {
                    player.respawnRoomName = collision.gameObject.name;
                }
                if(collision.gameObject.GetComponent<EnemySpawner>() != null) {
                    player.currentRoomSpawner = collision.gameObject.GetComponent<EnemySpawner>();
                }
            }
        }
    }

    //this function is currently unused
    public void MoveCameraToCustomRoom(GameObject roomToUse) {
        if(camToggle == true) {
            cam.gameObject.SetActive(true);
            cam2.gameObject.SetActive(false);
            confiner.m_BoundingShape2D = roomToUse.gameObject.GetComponent<PolygonCollider2D>();
        }
        if(camToggle == false) {
            cam2.gameObject.SetActive(true);
            cam.gameObject.SetActive(false);
            confiner2.m_BoundingShape2D = roomToUse.gameObject.GetComponent<PolygonCollider2D>();
        }
        camToggle = !camToggle;
        if(roomToUse.gameObject.GetComponent<EnemySpawner>() != null) {
            player.currentRoomSpawner = roomToUse.gameObject.GetComponent<EnemySpawner>();
        }
    }
}
