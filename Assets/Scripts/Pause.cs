using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Pause : MonoBehaviour {
    private SoundManager soundManager;
    private PlayerController player;
    public Transform cameraTransform;
    public float pauseCooldown = 1;
    private float timer;
    public bool paused = false;

    void Start() {
        timer = pauseCooldown;
        soundManager = GameObject.FindWithTag("SoundManager").GetComponent<SoundManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Update() {
        timer -= 0.1f;
    }

    //this gets called by the player controller
    public void PausePressed() {
        if(timer <= 0 && paused) {
            soundManager.PlayClip(soundManager.Unpause, transform, 1);
            timer = pauseCooldown;
            Time.timeScale = 1f;
            gameObject.transform.position = new Vector2(0, -1000);
            player.timeScaleIsZero = false;
            paused = false;
        }
        if(timer <= 0 && !paused) {
            soundManager.PlayClip(soundManager.Pause, transform, 1);
            timer = pauseCooldown;
            Time.timeScale = 0f;
            gameObject.transform.position = new Vector2(cameraTransform.position.x, cameraTransform.position.y);
            player.timeScaleIsZero = true;
            paused = true;
        }
    }
}
