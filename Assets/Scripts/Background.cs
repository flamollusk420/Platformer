using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour {
    private Transform cameraTransform;
    private SpriteRenderer sr;
    private float currentPlayerX;
    private float currentPlayerY;
    public float movementMultiplierX;
    public float movementMultiplierY;
    public float resetY;
    public bool stretchX = true;
    public bool stretchY = false;
    public float stretchTimesX = 3;
    public float stretchTimesY = 1;
    public float smoothing = 1;
    private float spriteWidthInUnits;


    void Start() {
        cameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        sr.drawMode = SpriteDrawMode.Tiled;
        if(stretchX) {
            sr.size = new Vector2(sr.size.x * stretchTimesX, sr.size.y);
        }
        if(stretchY) {
            sr.size = new Vector2(sr.size.x, sr.size.y * stretchTimesY);
        }
        spriteWidthInUnits = sr.sprite.texture.width / sr.sprite.pixelsPerUnit;
        currentPlayerX = cameraTransform.position.x;
        currentPlayerY = cameraTransform.position.y;
    }

    void FixedUpdate() {
        //transform.position = new Vector2((transform.position.x + ((cameraTransform.position.x - currentPlayerX) * movementMultiplierX)), (transform.position.y + ((cameraTransform.position.y - currentPlayerY) * movementMultiplierY)));
        transform.position = Vector3.Lerp(transform.position, new Vector3((transform.position.x + ((cameraTransform.position.x - currentPlayerX) * movementMultiplierX)), (transform.position.y + ((cameraTransform.position.y - currentPlayerY) * movementMultiplierY)), transform.position.z), smoothing);
        currentPlayerX = cameraTransform.position.x;
        currentPlayerY = cameraTransform.position.y;
        if(Mathf.Abs(cameraTransform.position.x - transform.position.x) >= spriteWidthInUnits) {
            transform.position = new Vector2(cameraTransform.position.x, transform.position.y);
        }
    }
}
