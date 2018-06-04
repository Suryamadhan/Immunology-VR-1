using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBeeping : MonoBehaviour
{
    public GameObject chemoSensor;
    public Transform chemokine;
    public GameObject sensor;
    //public GameObject getCloserInfo;
    //public GameObject outOfRangeInfo;
    private ChemoSensorAudio chemoAudio;
    private float dist;
    //private PortalEnter portalEnterScript;
    //public GameObject trigger;


    void Awake()
    {
        chemoAudio = chemoSensor.GetComponent<ChemoSensorAudio>();
        chemoAudio.enabled = false;
        //portalEnterScript = trigger.GetComponent<PortalEnter>();
    }
    // Use this for initialization
    void Start()
    {
        //getCloserInfo.SetActive(false);
        //outOfRangeInfo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        dist = Vector3.Distance(chemokine.position, transform.position);

        if (dist <= 250)
        {
            chemoAudio.enabled = true;
/*            if (portalEnterScript.stepIntoPortalInfo.activeInHierarchy == false)
            {
                getCloserInfo.SetActive(true);
            }
            else
            {
                getCloserInfo.SetActive(false);
            }
            outOfRangeInfo.SetActive(false);
*/
        }
        else
        {
            chemoAudio.enabled = false;
            chemoAudio.oneBeep.Stop();
//            getCloserInfo.SetActive(false);
//            outOfRangeInfo.SetActive(true);
        }

    }

}
