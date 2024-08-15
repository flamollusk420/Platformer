using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogSizeInConsole : MonoBehaviour {
    void LateUpdate() {
        Debug.Log(GetComponent<SpriteRenderer>().sprite.bounds.size.x + ", " + GetComponent<SpriteRenderer>().sprite.bounds.size.y);
    }
}
