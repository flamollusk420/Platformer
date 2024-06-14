using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TransparentWhenTouched : MonoBehaviour {
    public bool isText = false;
    public float transparentAlpha = 0.8f;
    public float originalAlpha = 1;
    private TextMeshPro tmp;
    private SpriteRenderer sr;

    void Start() {
        if(!isText) {
            sr = GetComponent<SpriteRenderer>();
        }
        if(isText) {
            tmp = GetComponent<TextMeshPro>();
        }
    }

    void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemies" || collision.gameObject.tag == "Bullets" || collision.gameObject.tag == "Explosion") {
            if(!isText && sr.enabled) {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, transparentAlpha);
            }
            if(isText && tmp.enabled) {
                tmp.alpha = transparentAlpha;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.tag == "Player" || collision.gameObject.tag == "Enemies" || collision.gameObject.tag == "Bullets" || collision.gameObject.tag == "Explosion") {
            if(!isText && sr.enabled) {
                sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, originalAlpha);
            }
            if(isText && tmp.enabled) {
                tmp.alpha = originalAlpha;
            }
        }
    }
}
