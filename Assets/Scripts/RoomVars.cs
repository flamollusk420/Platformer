using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//variables that decide how a room works
public class RoomVars : MonoBehaviour {
    //public float xPos;
    //public float yPos;
    //public bool lockX;
    //public bool lockY;
    public bool lockPlayerInRoom;
    public bool needsBarrierL = true;
    public bool needsBarrierR = true;
    public bool needsBarrierU = true;
    public bool needsBarrierD = true;
    public bool showBarrierL = true;
    public bool showBarrierR = true;
    public bool showBarrierU = true;
    public bool showBarrierD = true;

    void OnEnable() {
        if(lockPlayerInRoom) {
            GetComponent<EnemySpawner>().checkIfEnemiesAreDead = true;
        }
    }
}
