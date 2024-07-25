using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour {
    private Transform player;
    private Transform tr;
    private SpriteRenderer sr;
    private bool moveX = true;
    private bool moveY = true;
    private bool moveXoriginal;
    private bool moveYoriginal;
    private bool startCompleted = false;
    public bool stopAtX;
    public bool stopAtY;
    private bool initialNeedsToBeVisibleToMove;
    public bool needsToBeVisibleToMove = true;
    public bool disableNeedingVisibilityAfterMoving = true;
    public float movementSpeed = 300;
    public float dirX = 1;
    public float dirY = 1;
    public float stopAtXorYdeadZone = 0.016f;
    public float xMultiplier = 1;
    public float yMultiplier = 1;
    private float temporaryYmultiplier;
    private float temporaryXmultiplier;
    private float movementX = 0;
    private float movementY = 0;

    void Start() {
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        sr = GetComponent<SpriteRenderer>();
        tr = GetComponent<Transform>();
        moveXoriginal = moveX;
        moveYoriginal = moveY;
        initialNeedsToBeVisibleToMove = needsToBeVisibleToMove;
        startCompleted = true;
    }

    void FixedUpdate() {
        CheckPosition();
        if(!(needsToBeVisibleToMove && !sr.isVisible)) {
            Move();
        }
    }

    private void OnEnable() {
        if(startCompleted) {
            moveX = moveXoriginal;
            moveY = moveYoriginal;
            needsToBeVisibleToMove = initialNeedsToBeVisibleToMove;
        }
    }

    private void CheckPosition() {
        if(player.position.x > tr.position.x) {
            dirX = 1;
        }
        if(player.position.x < tr.position.x) {
            dirX = -1;
        }
        if(player.position.y > tr.position.y) {
            dirY = 1;
        }
        if(player.position.y < tr.position.y) {
            dirY = -1;
        }
        if(tr.position.y > (player.position.y - stopAtXorYdeadZone) && tr.position.y < (player.position.y + stopAtXorYdeadZone)) {
            if(stopAtY) {
                moveY = false;
            }
            temporaryYmultiplier = 0;
        }
        else {
            temporaryYmultiplier = 1;
        }
        if(tr.position.x > (player.position.x - stopAtXorYdeadZone) && tr.position.x < (player.position.x + stopAtXorYdeadZone)) {
            if(stopAtY) {
                moveX = false;
            }
            temporaryXmultiplier = 0;
        }
        else {
            temporaryXmultiplier = 1;
        }
    }

    private void Move() {
        movementX = (dirX * (movementSpeed / 100));
        movementY = (dirY * (movementSpeed / 100));
        if(!moveX) {
            movementX = 0;
        }
        if(!moveY) {
            movementY = 0;
        }
        if(disableNeedingVisibilityAfterMoving && needsToBeVisibleToMove) {
            needsToBeVisibleToMove = false;
        }
        transform.position = new Vector2((movementX * xMultiplier) * 0.016f * temporaryXmultiplier + transform.position.x, (movementY * yMultiplier) * 0.016f * temporaryYmultiplier + transform.position.y);
    }
}
