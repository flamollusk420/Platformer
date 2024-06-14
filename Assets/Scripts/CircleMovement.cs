using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMovement : MonoBehaviour {
    public float amplitude;
    public float frequency;
    private Rigidbody2D rb;
    public bool moveX = true;
    public bool moveY = true;
    private float x;
    private float y;

    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if(moveX) {
            x = Mathf.Cos(Time.time * frequency) * amplitude;
        }
        if(moveY) {
            y = Mathf.Sin(Time.time * frequency) * amplitude;
        }
        rb.velocity = new Vector2(x, y);
    }
}
