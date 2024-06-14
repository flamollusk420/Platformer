using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleSprite : MonoBehaviour {
    public bool toggle = true;
    private SpriteRenderer sr;
    public Sprite sprite1;
    public Sprite sprite2;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update() {
        if(toggle) {
            sr.sprite = sprite1;
        }
        if(!toggle) {
            sr.sprite = sprite2;
        }
    }
}
