﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;
using UnityEngine.UI;

public class Teleportation : MonoBehaviour {


    public GameObject[] neutrophils;
    private GameObject teleportTarget;

    public GameObject player;
    public GameObject neutrophilCockpitPrefab;

    public GameObject newNeutrophilBody;
    public bool isTeleported;

    public GameObject gameTimer;
    public GameObject selfDestructionTimer;
    private TimerCountdown gameTimerScript;
    
    public GameObject tmproDisplay;
    private TextMeshPro resetText;
    private Color32 resetTextColor = new Color32(2, 189, 251, 255);

    //private NETSv1 netsV1Script;

    public ParticleSystem teleportEffect;
    public GameObject panelBaseOld;
    public GameObject panelBaseNew;

    void Awake()
    {

        resetText = tmproDisplay.GetComponent<TextMeshPro>();
        gameTimerScript = gameTimer.GetComponentInChildren<TimerCountdown>();
        //netsV1Script = GetComponent<NETSv1>();
    }

        // Use this for initialization
    void Start () {
        teleportTarget = FindClosest();
    }


    public GameObject FindClosest()
    {
        neutrophils = GameObject.FindGameObjectsWithTag("Neutrophil");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject n in neutrophils)
        {
            Vector3 diff = n.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = n;
                distance = curDistance;
            }
        }

        return closest;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Teleport Button" && !isTeleported && teleportTarget != null)
        {
            StartCoroutine(Teleport());

            DisplayInfoReset();
        }

        //Debug.Log("controller touched!");
    }

    private void Update()
    {
        //Debug.DrawLine(transform.position, teleportTarget.transform.position, Color.red);
    }


    // Update is called once per frame
    /*    void Update () {



            if (device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && !isTeleported && teleportTarget != null)
            {
                newNeutrophilBody = Instantiate(neutrophilCockpitPrefab, player.transform.position, player.transform.rotation);

                Teleport();

                DisplayInfoReset();

                //netsV1Script.isReseting = true;

                //netsV1Script.enabled = false;
            }
        }
    */

    IEnumerator Teleport()
    {
        //teleportTarget.SetActive(false);

        teleportEffect.Emit(100);
        yield return new WaitForSeconds(2f);

        newNeutrophilBody = Instantiate(neutrophilCockpitPrefab, player.transform.position, player.transform.rotation);
        player.transform.position = teleportTarget.transform.position;
        player.transform.rotation = teleportTarget.transform.rotation;
        

        isTeleported = true;

        Destroy(panelBaseOld);
        panelBaseNew.SetActive(true);
        Destroy(teleportTarget);
        Destroy(teleportEffect);
    }

    private void DisplayInfoReset()
    {
        resetText.text = "Teleporation to another neutrophil success!";
        resetText.color = resetTextColor;

        gameTimer.SetActive(true);
        selfDestructionTimer.SetActive(false);
        gameTimerScript.m_leftTime = 300f;
    }
}
