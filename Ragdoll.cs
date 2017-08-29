using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ragdoll : MonoBehaviour {

    private bool alive;

    // Use this for initialization
    public void SetKinematic(bool newValue)
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
        AudioSource[] audio_m4 = GetComponentsInParent<AudioSource>();
        if(audio_m4[0].isPlaying)
            audio_m4[0].Stop();
        if (!audio_m4[1].isPlaying)
            audio_m4[1].Play();
        //GameObject soldier = GameObject.FindGameObjectWithTag("Army_Pilot");
        //RectTransform rt = (RectTransform)soldier.transform; 

        //float width = rt.rect.width;
        //float height = rt.rect.height;
    }

    public bool getAlive() {
        return alive;
    }

    public void setAlive(bool alive)
    {
        this.alive = alive;
        AudioSource[] audio = GetComponentsInParent<AudioSource>();
        if (audio.Length > 0)
        {
            audio[0].Play();
            audio[1].Stop();
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
