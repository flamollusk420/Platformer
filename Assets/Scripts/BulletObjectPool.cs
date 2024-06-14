using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletObjectPool : MonoBehaviour {
    public static BulletObjectPool instance;
    private List<GameObject> pooledObjects = new List<GameObject>();
    private int amountToPool = 50;

    [SerializeField] private GameObject prefab;

    private void Awake() {
        if(instance == null) {
            instance = this;
        }
    }

    void Start() {
        for(int i = 0; i < amountToPool; i++) {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject() {
        for (int i = 0; i < pooledObjects.Count; i++) {
            if(!pooledObjects[i].activeInHierarchy) {
                return pooledObjects[i];
            }
        }
        return null;
    }
}
