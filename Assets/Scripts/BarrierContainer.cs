using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierContainer : MonoBehaviour {
    public Camera cam;
    public Transform barrierL;
    public Transform barrierR;
    public Transform barrierU;
    public Transform barrierD;
    public Transform barrierImageL;
    public Transform barrierImageR;
    public Transform barrierImageU;
    public Transform barrierImageD;
    private float timerL;
    private float timerR;
    private float timerU;
    private float timerD;
    public bool removingBarriers;
    public bool roomIsLocked;

    void Start() {
        barrierL.parent = null;
        barrierR.parent = null;
        barrierU.parent = null;
        barrierD.parent = null;
    }

    void OnEnable() {
        roomIsLocked = false;
        removingBarriers = false;
    }

    void Update() {
        if(barrierL.gameObject.activeInHierarchy || removingBarriers) {
            timerL -= Time.deltaTime;
            if(timerL <= 0 && (barrierImageL.gameObject.activeInHierarchy || barrierImageL.parent.gameObject.activeInHierarchy)) {
                if(barrierImageL.gameObject.activeInHierarchy) {
                    barrierImageL.GetComponent<Animator>().SetBool("entering", false);
                }
                if(removingBarriers) {
                    barrierImageL.gameObject.SetActive(false);
                    barrierImageL.parent.gameObject.SetActive(false);
                }
            }
        }
        if(barrierR.gameObject.activeInHierarchy || removingBarriers) {
            timerR -= Time.deltaTime;
            if(timerR <= 0 && (barrierImageR.gameObject.activeInHierarchy || barrierImageR.parent.gameObject.activeInHierarchy)) {
                if(barrierImageR.gameObject.activeInHierarchy) {
                    barrierImageR.GetComponent<Animator>().SetBool("entering", false);
                }
                if(removingBarriers) {
                    barrierImageR.gameObject.SetActive(false);
                    barrierImageR.parent.gameObject.SetActive(false);
                }
            }
        }
        if(barrierU.gameObject.activeInHierarchy || removingBarriers) {
            timerU -= Time.deltaTime;
            if(timerU <= 0 && (barrierImageU.gameObject.activeInHierarchy || barrierImageU.parent.gameObject.activeInHierarchy)) {
                if(barrierImageU.gameObject.activeInHierarchy) {
                    barrierImageU.GetComponent<Animator>().SetBool("entering", false);
                }
                if(removingBarriers) {
                    barrierImageU.gameObject.SetActive(false);
                    barrierImageU.parent.gameObject.SetActive(false);
                }
            }
        }
        if(barrierD.gameObject.activeInHierarchy || removingBarriers) {
            timerD -= Time.deltaTime;
            if(timerD <= 0 && (barrierImageD.gameObject.activeInHierarchy || barrierImageD.parent.gameObject.activeInHierarchy)) {
                if(barrierImageD.gameObject.activeInHierarchy) {
                    barrierImageD.GetComponent<Animator>().SetBool("entering", false);
                }
                if(removingBarriers) {
                    barrierImageD.gameObject.SetActive(false);
                    barrierImageD.parent.gameObject.SetActive(false);
                }
            }
        }
        if(timerL <= 0 || timerR <= 0 || timerU <= 0 || timerD <= 0) {
            if(removingBarriers) {
                removingBarriers = false;
            }
            if(!roomIsLocked && !removingBarriers) {
                if(barrierL.gameObject.activeInHierarchy || barrierR.gameObject.activeInHierarchy || barrierU.gameObject.activeInHierarchy || barrierD.gameObject.activeInHierarchy) {
                    roomIsLocked = true;
                }
            }
        }
    }

    public void LockPlayerInRoom(GameObject room, bool needsBarrierL, bool needsBarrierR, bool needsBarrierU, bool needsBarrierD, bool showBarrierL, bool showBarrierR, bool showBarrierU, bool showBarrierD) {
        removingBarriers = false;
        PolygonCollider2D roomCollider = room.GetComponent<PolygonCollider2D>();
        if(needsBarrierL) {
            timerL = 0.15f;
            barrierL.gameObject.SetActive(true);
            barrierImageL.gameObject.SetActive(true);
            barrierImageL.parent.gameObject.SetActive(true);
            barrierL.position = new Vector2(roomCollider.bounds.center.x - roomCollider.bounds.size.x / 2, roomCollider.bounds.center.y);
            if(barrierL.position.x > cam.ViewportToWorldPoint(new Vector3(0,0,0)).x) {
                barrierL.position = new Vector2(cam.ViewportToWorldPoint(new Vector3(0,0,0)).x, barrierL.position.y);
            }
            barrierL.localScale = new Vector3(barrierL.localScale.x, roomCollider.bounds.size.y * 1.25f, barrierL.localScale.z);
            if(showBarrierL) {
                barrierImageL.gameObject.SetActive(true);
                barrierImageL.parent.gameObject.SetActive(true);
                Transform barrierImageLparent = barrierImageL.parent;
                barrierImageLparent.gameObject.SetActive(true);
                barrierImageLparent.transform.position = new Vector2(cam.ViewportToWorldPoint(new Vector3(0,0,0)).x, barrierImageLparent.transform.position.y);
                barrierImageL.GetComponent<Animator>().SetBool("entering", true);
            }
        }
        if(needsBarrierR) {
            timerR = 0.15f;
            barrierR.gameObject.SetActive(true);
            barrierR.position = new Vector2(roomCollider.bounds.center.x + roomCollider.bounds.size.x / 2, roomCollider.bounds.center.y);
            if(barrierR.position.x < cam.ViewportToWorldPoint(new Vector3(1,0,0)).x) {
                barrierR.position = new Vector2(cam.ViewportToWorldPoint(new Vector3(1,0,0)).x, barrierL.position.y);
            }
            barrierR.localScale = new Vector3(barrierR.localScale.x, roomCollider.bounds.size.y * 1.25f, barrierR.localScale.z);
            if(showBarrierR) {
                barrierImageR.gameObject.SetActive(true);
                barrierImageR.parent.gameObject.SetActive(true);
                Transform barrierImageRparent = barrierImageR.parent;
                barrierImageRparent.gameObject.SetActive(true);
                barrierImageRparent.transform.position = new Vector2(cam.ViewportToWorldPoint(new Vector3(1,0,0)).x, barrierImageRparent.transform.position.y);
                barrierImageR.GetComponent<Animator>().SetBool("entering", true);
            }
        }
        if(needsBarrierU) {
            timerU = 0.15f;
            barrierU.gameObject.SetActive(true);
            barrierU.position = new Vector2(roomCollider.bounds.center.x, roomCollider.bounds.center.y + roomCollider.bounds.size.y / 2);
            if(barrierU.position.y < cam.ViewportToWorldPoint(new Vector3(0,1,0)).y) {
                barrierU.position = new Vector2(barrierU.position.x, cam.ViewportToWorldPoint(new Vector3(0,1,0)).y);
            }
            barrierU.localScale = new Vector3(roomCollider.bounds.size.x * 1.25f, barrierU.localScale.y, barrierU.localScale.z);
            if(showBarrierU) {
                barrierImageU.gameObject.SetActive(true);
                barrierImageU.parent.gameObject.SetActive(true);
                Transform barrierImageUparent = barrierImageU.parent;
                barrierImageUparent.gameObject.SetActive(true);
                barrierImageUparent.transform.position = new Vector2(barrierImageUparent.transform.position.x, cam.ViewportToWorldPoint(new Vector3(0,1,0)).y);
                barrierImageU.GetComponent<Animator>().SetBool("entering", true);
            }
        }
        if(needsBarrierD) {
            timerD = 0.15f;
            barrierD.gameObject.SetActive(true);
            barrierD.position = new Vector2(roomCollider.bounds.center.x, roomCollider.bounds.center.y - roomCollider.bounds.size.y / 2);
            if(barrierD.position.y > cam.ViewportToWorldPoint(new Vector3(0,0,0)).y) {
                barrierD.position = new Vector2(barrierD.position.x, cam.ViewportToWorldPoint(new Vector3(0,0,0)).y);
            }
            barrierD.localScale = new Vector3(roomCollider.bounds.size.x * 1.25f, barrierU.localScale.y, barrierU.localScale.z);
            if(showBarrierD) {
                barrierImageD.gameObject.SetActive(true);
                barrierImageD.parent.gameObject.SetActive(true);
                Transform barrierImageDparent = barrierImageD.parent;
                barrierImageDparent.gameObject.SetActive(true);
                barrierImageDparent.transform.position = new Vector2(barrierImageDparent.transform.position.x, cam.ViewportToWorldPoint(new Vector3(0,0,0)).y);
                barrierImageD.GetComponent<Animator>().SetBool("entering", true);
            }
        }
    }

    public void RemoveBarriers() {
        roomIsLocked = false;
        barrierL.gameObject.SetActive(false);
        barrierR.gameObject.SetActive(false);
        barrierU.gameObject.SetActive(false);
        barrierD.gameObject.SetActive(false);
        timerL = 0.15f;
        timerR = 0.15f;
        timerU = 0.15f;
        timerD = 0.15f;
        removingBarriers = true;
        if(barrierImageL.gameObject.activeInHierarchy) {
            barrierImageL.GetComponent<Animator>().SetBool("exiting", true);
            barrierImageL.GetComponent<Animator>().SetBool("entering", false);
        }
        if(barrierImageR.gameObject.activeInHierarchy) {
            barrierImageR.GetComponent<Animator>().SetBool("exiting", true);
            barrierImageR.GetComponent<Animator>().SetBool("entering", false);
        }
        if(barrierImageU.gameObject.activeInHierarchy) {
            barrierImageU.GetComponent<Animator>().SetBool("exiting", true);
            barrierImageU.GetComponent<Animator>().SetBool("entering", false);
        }
        if(barrierImageD.gameObject.activeInHierarchy) {
            barrierImageD.GetComponent<Animator>().SetBool("exiting", true);
            barrierImageD.GetComponent<Animator>().SetBool("entering", false);
        }
    }
}
