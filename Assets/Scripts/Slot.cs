using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour {

    public Node node;

    public Transform ballImage;
    public Animator anim;

    void Start()
    {
        ballImage = transform.Find("SlotButton").Find("Image");
        anim = ballImage.GetComponent<Animator>();
    }

    public void SelectBall()
    {
        anim.SetBool("isSelected", true);
    }
    public void DeselectBall()
    {
        anim.SetBool("isSelected", false);
    }
    public void DieAndRemoveBall() //<----------------Удаление теперь тут!
    {
        anim.SetBool("isDie", true); //В конце этой анимации
    }
}
