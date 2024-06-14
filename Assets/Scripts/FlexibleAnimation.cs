using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlexibleAnimation : MonoBehaviour {
    public List<Sprite> frameList = new List<Sprite>();
    public float timeUntilNextFrame;
    public int currentFrame = 0;
    private float timer;
    public int numberOfFrames;
    public bool stopAtLastFrame = false;
    public bool dieAtLastFrame = false;
    private SpriteRenderer sr;
    private PlayerController player;

    void Start() {
        timer = timeUntilNextFrame;
        sr = GetComponent<SpriteRenderer>();
        numberOfFrames = frameList.Count - 1;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    void Update() {
        timer -= Time.deltaTime;
        if(timer <= 0) {
            timer = timeUntilNextFrame;
            if(!stopAtLastFrame || stopAtLastFrame && currentFrame < numberOfFrames) {
                ChangeFrame();
            }
            if(dieAtLastFrame && currentFrame >= numberOfFrames) {
                currentFrame = 0;
                gameObject.SetActive(false);
            }
        }
        if(player.respawned) {
            currentFrame = 0;
            timer = timeUntilNextFrame;
            sr.sprite = frameList[currentFrame];
        }
    }

    private void ChangeFrame() {
        currentFrame += 1;
        if(currentFrame > numberOfFrames) {
            currentFrame = 0;
        }
        sr.sprite = frameList[currentFrame];
    }
}
