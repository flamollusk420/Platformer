using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayAtPosition : MonoBehaviour {
    public Transform objectToUse;
    public Transform objectToUse2;
    public bool goToObjectPosition = true;
    public bool goToCoordinatePosition = false;
    public bool moveObjectToThisPosition = false;
    public bool moveObjectToObject2Position = false;
    public bool moveObjectToCoordinatePosition = false;
    public float xToGoTo;
    public float yToGoTo;
    public bool moveOnce;
    public bool moveObjectOnce;

    void OnEnable() {
        if(goToObjectPosition) {
            transform.position = objectToUse.position;
        }
        if(goToCoordinatePosition) {
            transform.position = new Vector2(xToGoTo, yToGoTo);
        }
        if(moveObjectToThisPosition) {
            objectToUse.position = transform.position;
        }
        if(moveObjectToObject2Position) {
            objectToUse.position = objectToUse2.position;
        }
        if(moveObjectToCoordinatePosition) {
            objectToUse.position = new Vector2(xToGoTo, yToGoTo);
        }
    }

    void Update() {
        if(!moveOnce) {
            if(goToObjectPosition) {
                transform.position = objectToUse.position;
            }
            if(goToCoordinatePosition) {
                transform.position = new Vector2(xToGoTo, yToGoTo);
            }
        }
        if(!moveObjectOnce) {
            if(moveObjectToThisPosition) {
                objectToUse.position = transform.position;
            }
            if(moveObjectToObject2Position) {
                objectToUse.position = objectToUse2.position;
            }
            if(moveObjectToCoordinatePosition) {
                objectToUse.position = new Vector2(xToGoTo, yToGoTo);
            }
        }
    }
}
