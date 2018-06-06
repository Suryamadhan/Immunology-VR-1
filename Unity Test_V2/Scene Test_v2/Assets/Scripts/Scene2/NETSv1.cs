using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class NETSv1 : MonoBehaviour
{

    public GameObject player;
    public bool isTriggerDown;

    public GameObject dnaChain1a;
    public GameObject dnaChain2a;
    public GameObject dnaChain1b;
    public GameObject dnaChain2b;
    
    private GameObject dnaChainTemp1;
    private GameObject dnaChainTemp2;
    public float speed = 0f;

    public GameObject gameTimer;
    public GameObject selfDestructionTimer;
    public bool isSelfDestructTimerON;

    private Vector3 originalScale;
    //public bool isReseting;

    //public GameObject textMeshAbandonShip;
    public TextMeshPro abandonShipText;
    public TextMeshPro header;

    private Teleportation teleportScript;
    private SelfDestructionCountdown selfDestructScript;

    //stop player movement after NETS triggered
    public MagicMoveV4 magicMoveV4;
    public GameObject pointerSphere;
    SphereCollider col;

    //vive controller tracking and input
    public SteamVR_TrackedObject controller;
    private SteamVR_Controller.Device device
    {
        get { return SteamVR_Controller.Input((int)controller.index); }
    }


    private void Awake()
    {
        controller = GetComponent<SteamVR_TrackedObject>();
        //originalScale = dnaChain1.transform.localScale;

        //abandonShipText = textMeshAbandonShip.GetComponent<TextMeshPro>();


        teleportScript = GetComponent<Teleportation>();

        selfDestructScript = selfDestructionTimer.GetComponentInChildren<SelfDestructionCountdown>();

        
        dnaChainTemp1 = dnaChain1a;
        dnaChainTemp1.SetActive(false);

        
        dnaChainTemp2 = dnaChain2a;
        dnaChainTemp2.SetActive(false);

        originalScale = dnaChainTemp1.transform.localScale;

        dnaChain1b.SetActive(false);
        dnaChain2b.SetActive(false);

        col = pointerSphere.GetComponent<SphereCollider>();
    }

 

    // Update is called once per frame
    void Update()
    {

        
        if (device.GetHairTrigger())
        {
            dnaChainTemp1.SetActive(true);
            dnaChainTemp2.SetActive(true);

            speed += 0.1f;
            dnaChainTemp1.transform.localScale = originalScale * speed;
            dnaChainTemp2.transform.localScale = originalScale * speed;

            isTriggerDown = true;

            gameTimer.SetActive(false);
            selfDestructionTimer.SetActive(true);
            isSelfDestructTimerON = true;

            abandonShipText.text = "Abandon neutrophil body now!";
            abandonShipText.color = Color.red;
            header.color = Color.red;
            header.text = "[Alert]";

            teleportScript.enabled = true;
            //magicMoveV4.speed = 0f;
            col.enabled = false;
        }

    }


    public void ResetNETS()
    {
        dnaChainTemp1.transform.parent = null;
        dnaChain1a = dnaChainTemp1;
        dnaChain1a.transform.position = new Vector3 (teleportScript.newNeutrophilBody.transform.position.x - 1f, teleportScript.newNeutrophilBody.transform.position.y, teleportScript.newNeutrophilBody.transform.position.z);
        dnaChain1a.transform.rotation = teleportScript.newNeutrophilBody.transform.rotation;

        dnaChainTemp1 = dnaChain1b;


        dnaChainTemp2.transform.parent = null;
        dnaChain2a = dnaChainTemp2;
        dnaChain2a.transform.position = new Vector3 (teleportScript.newNeutrophilBody.transform.position.x + 1f, teleportScript.newNeutrophilBody.transform.position.y, teleportScript.newNeutrophilBody.transform.position.z);
        dnaChain2a.transform.rotation = teleportScript.newNeutrophilBody.transform.rotation;

        dnaChainTemp2 = dnaChain2b;

        speed = 0f;

        dnaChainTemp1.SetActive(false);
        dnaChainTemp2.SetActive(false);

        selfDestructScript.m_leftTime = 60f;

        //magicMoveV4.speed = 0.2f;
        col.enabled = true;

    }
}
