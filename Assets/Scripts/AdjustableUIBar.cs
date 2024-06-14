using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustableUIBar : MonoBehaviour {
    public List<Sprite> spriteList = new List<Sprite>();
    public float position = 0;
    public float divideBy = 1;
    private float listPosition = 0;
    private int numberOfSprites;
    private SpriteRenderer sr;

    void Start() {
        sr = GetComponent<SpriteRenderer>();
        numberOfSprites = spriteList.Count - 1;
    }

    void Update() {
        if(position > numberOfSprites) {
            position = numberOfSprites;
        }
        listPosition = position / divideBy;
        if(listPosition <= spriteList.Count - 1 && listPosition > -1) {
            sr.sprite = spriteList[Mathf.RoundToInt(listPosition)];
        }
    }
}
