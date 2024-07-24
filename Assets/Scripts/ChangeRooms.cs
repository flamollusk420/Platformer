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
            if(player.currentRoomName != collision.gameObject.name) {
                if(camToggle == true) {
                    cam.gameObject.SetActive(true);
                    cam2.gameObject.SetActive(false);
                    confiner.m_BoundingShape2D = collision.gameObject.GetComponent<PolygonCollider2D>();
                    //camLock.lockX = collision.gameObject.GetComponent<RoomVars>().lockX;
                    //camLock.lockY = collision.gameObject.GetComponent<RoomVars>().lockY;
                    //camLock.m_XPosition = collision.gameObject.GetComponent<RoomVars>().xPos;
                    //camLock.m_YPosition = collision.gameObject.GetComponent<RoomVars>().yPos;
                }
                if(camToggle == false) {
                    cam2.gameObject.SetActive(true);
                    cam.gameObject.SetActive(false);
                    confiner2.m_BoundingShape2D = collision.gameObject.GetComponent<PolygonCollider2D>();
                    //camLock2.lockX = collision.gameObject.GetComponent<RoomVars>().lockX;
                    //camLock2.lockY = collision.gameObject.GetComponent<RoomVars>().lockY;
                    //camLock2.m_XPosition = collision.gameObject.GetComponent<RoomVars>().xPos;
                    //camLock2.m_YPosition = collision.gameObject.GetComponent<RoomVars>().yPos;
                }
                camToggle = !camToggle;
                player.currentRoomName = collision.gameObject.name;
                if(gameObject.GetComponent<EnemySpawner>() != null) {
                    player.currentRoomSpawner = GetComponent<EnemySpawner>();
                }
            }
        }
    }

}
