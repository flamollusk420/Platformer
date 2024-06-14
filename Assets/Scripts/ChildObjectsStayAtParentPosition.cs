using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildObjectsStayAtParentPosition : MonoBehaviour {
    public bool multipleChildren;
    public bool enableChildrenOnStart = true;
    public int childToMove;
    private Transform singleChild;
    public List<Transform> children;
    public bool useChildListSetInEditor;
    private bool hasPopulatedChildList;

    void OnEnable() {
        if(!useChildListSetInEditor) {
            children.Clear();
        }
        hasPopulatedChildList = false;
        if(!multipleChildren) {
            singleChild = transform.GetChild(childToMove);
        }
        if(multipleChildren) {
            if(!useChildListSetInEditor) {
                for(int i = 0; i < transform.childCount; i++) {
                    children.Add(transform.GetChild(i));
                }
            }
            hasPopulatedChildList = true;
        }
        if(enableChildrenOnStart) {
            if(multipleChildren && !useChildListSetInEditor) {
                for(int i = 0; i < transform.childCount; i++) {
                    children[i].gameObject.SetActive(true);
                }
            }
            if(multipleChildren && useChildListSetInEditor) {
                for(int i = 0; i < children.Count; i++) {
                    children[i].gameObject.SetActive(true);
                }
            }
            if(!multipleChildren) {
                singleChild.gameObject.SetActive(true);
            }
        }
    }

    void FixedUpdate() {
        if(multipleChildren && hasPopulatedChildList && !useChildListSetInEditor) {
            for(int i = 0; i < transform.childCount; i++) {
                children[i].position = transform.position;
            }
        }
        if(multipleChildren && hasPopulatedChildList && useChildListSetInEditor) {
            for(int i = 0; i < children.Count; i++) {
                children[i].position = transform.position;
            }
        }
        if(!multipleChildren && singleChild != null) {
            singleChild.position = transform.position;
        }
    }
}
