using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageBallController : MonoBehaviour {
    Node node;
    Slot slot;
    void Start()
    {
        slot = transform.parent.transform.parent.GetComponent<Slot>();
        node = transform.parent.transform.parent.GetComponent<Slot>().node;
    }

    public void RemoveBall()
    {
        slot.anim.SetBool("isDie", false);
        node.RemoveBall();
    }
}
