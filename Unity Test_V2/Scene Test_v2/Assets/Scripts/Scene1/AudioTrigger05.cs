using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AudioTrigger05 : MonoBehaviour {

    public bool playAudio05;
    public TextMeshPro missionText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playAudio05 = true;

            GetComponent<BoxCollider>().isTrigger = false;

            missionText.text = "prepare to stop and look for a transmigration portal";
        }
    }
}
