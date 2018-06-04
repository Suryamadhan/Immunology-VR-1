using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioTrigger06 : MonoBehaviour {

    public bool playAudio06;
    public TextMeshPro missionText;
  

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playAudio06 = true;

            GetComponent<BoxCollider>().isTrigger = false;

            missionText.text = "Look for a portal to transmigrate to body tissues";
            
        }
    }
}
