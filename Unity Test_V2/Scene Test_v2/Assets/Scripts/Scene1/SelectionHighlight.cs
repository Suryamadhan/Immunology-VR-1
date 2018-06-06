using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class SelectionHighlight : MonoBehaviour {

    Collider col;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    private Outline _outline;
    

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        
    }
    
	
	// Update is called once per frame
	void Update () {

        if (Controller.GetHairTrigger())
        {
            RaycastHit hit;
            int layer_mask = 1 << 10;

            if (Physics.Raycast(trackedObj.transform.position, transform.forward, out hit, 800, layer_mask))
            {
                
                if (hit.collider.tag == "Highlightable")
                {
                    col = hit.collider;
                    _outline = hit.collider.gameObject.GetComponent<Outline>();
                    _outline.isHit = true;

                    Debug.Log("Highlightable is hit!");
                }

            }
            else
            {
                if(hit.collider != null)
                {
                    _outline.isHit = false;

                }

                //Debug.Log("Highlightable NOT hit!");
            }

        }
        else
        {
            if(col != null)
            {
                _outline.isHit = false;
            }
               
    
        }

	}
}
