using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PortalEnter : MonoBehaviour
{

    public TextMeshPro missionText;


    private void OnTriggerEnter(Collider other)
    {
        missionText.text = "Please enter the Portal to start transmigration.";

    }

    

    private void OnTriggerExit(Collider other)
    {
        missionText.text = "Look for a portal to transmigrate to body tissues";

    }
}
