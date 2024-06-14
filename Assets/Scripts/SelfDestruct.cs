using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {
    public float timer;
    public float timerSet;

    void Start() {
        timer = timerSet;
    }

    void Update() {
        timer -= Time.deltaTime;
        if(timer <= 0) {
            gameObject.SetActive(false);
        }
    }
}
