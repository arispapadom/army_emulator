using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    private bool alive;

    // Use this for initialization
    void SetKinematic(bool newValue)
    {
        Rigidbody[] bodies = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = newValue;
            rb.useGravity = true;
        }
    }

    void Start()
    {
        SetKinematic(true);
        alive = true;
        //GameObject soldier = GameObject.FindGameObjectWithTag("Army_Pilot");
        //RectTransform rt = (RectTransform)soldier.transform; 

        //float width = rt.rect.width;
        //float height = rt.rect.height;
        Debug.Log(transform.lossyScale);
    }

    public bool getAlive()
    {
        return alive;
    }

    public void setAlive(bool alive)
    {
        this.alive = alive;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
