using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabCheck : MonoBehaviour {

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Grabbable")
        {
            Debug.Log(other.gameObject.name);
        }    
    }
}
