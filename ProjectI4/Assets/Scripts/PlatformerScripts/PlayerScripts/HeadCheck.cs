using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadCheck : MonoBehaviour {

    public bool inTrigger;

	void Start () {
        inTrigger = false;
	}

    public bool getTrigger()
    {
        return inTrigger;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9) 
        {
            inTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        inTrigger = false;
    }
}
