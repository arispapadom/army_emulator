using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaycastShoot : MonoBehaviour {

    public int gunDamage = 1;

    public Terrain terrain;
    public AudioSource audioSource;

    public Text txt;
    public static int InNumOfShoots = 10;
    public int shoots = 1;


    public Vector3 ArmPos;
    public Vector3 CamPos;

    public Vector2 hitPoint;
    public float distance;
    public Vector2 ArPoint;

    public float fireRate = .25f;

    public Animation anim;

    public AudioSource[] audio_m4;

    public float weaponRange = 50f;

    public float hitForce = 90000f;

    public Transform gunEnd;

    private Camera fpsCam;

    private WaitForSeconds shotDuration = new WaitForSeconds(.07f);

    private LineRenderer laserLine;

    private float nextFire;
    private int shot;
    private int shot_hit;

    public Thread thread;

    public bool alive;
    public string ArName;

    private bool paused;
    private bool callibration;

    GameObject[] pauseObjects;
    GameObject[] targetObjects;
    GameObject ArPilot;
    GameObject temp;
    GameObject can;
    GameObject img;
    GameObject cam;
    GameObject scoreObject; // gameObject in Hierarchy
    Text scoreText;

    //Canvas can;

    InputField iField;
    

    // Use this for initialization
    void Start () {
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        targetObjects = GameObject.FindGameObjectsWithTag("Army_Pilot");

        // Find gameObject with name "MyText"
        scoreObject = GameObject.Find("score");
        // Get component Text from that gameObject
        scoreText = scoreObject.GetComponent<Text>();

        hide_targets_except(" ");

        img = GameObject.FindGameObjectWithTag("Cal_Img");
        img.SetActive(false);

        cam = GameObject.FindGameObjectWithTag("MainCamera");
        CamPos = cam.transform.position;

        can = GameObject.FindGameObjectWithTag("Canvas");
        iField = can.GetComponentInChildren<InputField>();
       // ArPilot = GameObject.FindGameObjectWithTag("Army_Pilot");
        //ArPoint = new Vector2();
        hitPoint = new Vector3();
        //ArPoint.x = ArPilot.transform.position.x;
        //ArPoint.y = ArPilot.transform.position.y;
        //ArmPos = ArPilot.transform.position;

        var se = new InputField.SubmitEvent();
        se.AddListener(SubmitName);
        iField.onEndEdit = se;
        //Cursor.visible = false;
        hidePaused();
        paused = false;
        callibration = false;
        AudioListener.pause = false;

        /**example for hiding all targets except the target @ 200m
         * 
         * 
         */
        //hide_targets_except("Target_200");


    }
	
	// Update is called once per frame
	void Update () {
        nextFire = Time.time + fireRate;
            RaycastHit hit;
        //StartCoroutine(ShotEffect());
        if (ArPilot != null & temp != null)
            if (ArPilot.active == true & temp.active == true)
            {
                temp.SetActive(false);
            }
        if (Input.GetKeyDown(KeyCode.Escape))
        {//When a key is pressed down it see if it was the escape key. if it was it will execute the code
            if (Time.timeScale == 0) //play
            {
                if (callibration == false)
                {
                    Time.timeScale = 1;
                    hidePaused();
                    paused = false;
                    Cursor.visible = false;
                    AudioListener.pause = false;
                }
                else
                {
                    img.SetActive(false);
                    Cursor.visible = true;
                    showPaused();
                    callibration = false;
                }
            }
            else //pause
            {
                showPaused();
                Time.timeScale = 0;
                Cursor.visible = true;
                AudioListener.pause = true;
                paused = true;
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (paused == false)
                shot++;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.rigidbody != null)
                {

                    Debug.Log("You Hit " + hit.transform.gameObject.name + " in "+hit.distance*2+" meters.");
                    string forced = hit.transform.gameObject.name;
                    Debug.Log(hit.transform.root.gameObject.name);
                    if (paused == false)
                    {
                        shot_hit++;
                        if (!hit.transform.gameObject.name.Contains("Target"))
                        {

                            ArPilot = hit.transform.root.gameObject;
                            temp.SetActive(true);
                            ArPilot.SetActive(false);
                            Animator hitObj = temp.GetComponent<Animator>();
                            Ragdoll rd = temp.GetComponentInChildren<Ragdoll>();
                            alive = rd.getAlive();
                            Rigidbody[] parent = temp.GetComponentsInChildren<Rigidbody>();

                            hitObj.enabled = false;
                            alive = rd.getAlive();
                            if (alive == true)
                            {
                                rd.setAlive(false);
                                alive = rd.getAlive();
                            }

                            foreach (Rigidbody rb in parent)
                            {
                                rb.isKinematic = false;
                                rb.useGravity = true;
                                rb.WakeUp();
                                if (rb.name.Contains(forced))
                                {
                                    rb.AddForce(-hit.normal * hitForce);
                                    //Debug.Log(rb.name);
                                }
                            }

                            thread = new Thread(() => NewGameObject(3000));
                            thread.Start();
                        }

                    }

                }
                else  //hit terrain
                {
                    terrain = hit.transform.GetComponentInParent<Terrain>();
                    audioSource = terrain.GetComponent<AudioSource>();
                    hitPoint.x = hit.point.x;
                    hitPoint.y = hit.point.y;
                    distance = Vector2.Distance(hitPoint, ArPoint);
                    if (paused == false)
                        audioSource.Play();
                }
                

            }
        }

       
            if (alive == false)
            {
                if (thread != null && !thread.IsAlive)
                {
                    reanimation();
                }
            }
            if(shoots == InNumOfShoots)
            {
                //end of story
            }
        if (scoreText != null) {
            if (shot_hit == -1)
                shot = -1;
                scoreText.text = shot_hit + "/" + shot;
        }
    }

    private void reanimation()
    {
        Destroy(temp);
        ArPilot.SetActive(true);
        temp = Instantiate(ArPilot);
        temp.SetActive(true);
        audio_m4 = ArPilot.GetComponentsInParent<AudioSource>();
        alive = true;
        shoots++;
    }

    private void SubmitName(string arg0)
    {
        if (arg0 != null)
            InNumOfShoots = Int16.Parse(arg0);
    }

    public void Reload(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
        Time.timeScale = 1;
    }

    public void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnPause tag
    public void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    public void NewGameObject(int msec)
    {
        Thread.Sleep(msec);

    }

    public void ExitEm()
    {
        Application.Quit();
    }

    public void Callibrate()
    {
        hidePaused();
        Cursor.visible = false;
        img.SetActive(true);
        callibration = true;
    }
    public void hide_targets_except(string name)
    {
        shot = -1;
        shot_hit = -1;
        scoreText.text = shot_hit + "/"+shot;
        foreach (GameObject g in targetObjects)
        {
            if (!g.name.Contains(name))
                g.SetActive(false);
            else
            {
                g.SetActive(true);
                ArPilot = g;
                temp = Instantiate(g);
                temp.SetActive(true);
                Debug.Log(ArPilot.name);
            }
        }
    }
}
