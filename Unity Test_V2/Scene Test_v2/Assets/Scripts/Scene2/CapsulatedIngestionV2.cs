using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class CapsulatedIngestionV2 : MonoBehaviour
{
    public bool isEncapsulated;
    public bool isEnemyTooBig;

    public float alignSpeed;
    public GameObject player;
    public int counter;

    public LineRenderer selectionLine;
    public Transform endPoint;
    private Transform tempEnd;
    private Vector3 lineEnd;

    private bool startIngestion;

    public GameObject capsule;
    public GameObject pivot;
    public GameObject anchor;
    public GameObject carouselAssem;
    
    public GameObject storageScale;
    public GameObject bacteriaCapsulePrefeb;
    public GameObject particlesInsidePrefab;

    public ParticleSystem enzymeParticles1;

    public GameObject scoreTextPrefab;

    //public GameObject textMeshInvalidIngest;
    public TextMeshPro invalidIngestWarning;
    public TextMeshPro header;

    private GameObject target;
    private float ingestDist;
    private float distOnHit;
    private Vector3 anchorOnHit;
    private Vector3 hitObjectCenter;
    private Vector3 tempPos;
    private float scaleDownRatio;
    private float scaleDifference;

    private Vector2 touchPad;
//    private bool isScaledDown;
    
    private GameObject indicator;
    private Renderer rend;

    private RotateWithStops rotateScript;

    private AudioSource errorBeep;
    public AudioClip error;
    private bool isWrongTarget;

    //vive controller tracking and input
    public SteamVR_TrackedObject controller;
    private SteamVR_Controller.Device device
    {
        get { return SteamVR_Controller.Input((int)controller.index); }
    }


    private void Awake()
    {
        controller = GetComponent<SteamVR_TrackedObject>();
        capsule.SetActive(false);
        counter = 0;

        rotateScript = pivot.GetComponent<RotateWithStops>();

        //invalidIngestWarning = textMeshInvalidIngest.GetComponent<TextMeshPro>();

        tempEnd = endPoint;

        errorBeep = GetComponent<AudioSource>();
    }




    // Update is called once per frame
    void Update()
    {


        if (device.GetHairTrigger())
        {

            selectionLine.enabled = true;
            endPoint = tempEnd;
            lineEnd = endPoint.position;

            selectionLine.SetPosition(0, controller.transform.position);
            selectionLine.SetPosition(1, lineEnd);


            RaycastHit hit;
            if (Physics.Raycast(controller.transform.position, controller.transform.forward, out hit, 20f))
            {
                Debug.DrawRay(controller.transform.position, controller.transform.forward*20f, Color.red);

                if (hit.collider.tag == "Bacteria")
                {

                    endPoint = null;
                    selectionLine.SetPosition(1, hit.point);

                    target = hit.collider.gameObject;

                    hitObjectCenter = hit.collider.bounds.center;

                    //show a capsule surrounding selected bacteria
                    capsule.SetActive(true);
                    capsule.transform.position = hit.collider.bounds.center;
                    capsule.transform.localScale = hit.collider.bounds.size * 1.5f;
                    isEncapsulated = true; //turn on audio guide

                    
                    //anchorOnHit = anchor.transform.position;
                    //distOnHit = Vector3.Distance(target.transform.position, anchorOnHit);
                    //Debug.Log("distOnHit is " + distOnHit);

                }

                if ((hit.collider.tag == "Bacteria Sausage" && !isWrongTarget)|| (hit.collider.tag == "Bacteria Bighead" && !isWrongTarget))
                {
                    isWrongTarget = true;

                    StartCoroutine(WrongTargetWarning());
                }
                else
                {
                    invalidIngestWarning.color = new Color32(255, 255, 255, 255);
                    invalidIngestWarning.text = "Phagocytosis in action!";
                    header.color = new Color32(255, 78, 0, 255);
                    header.text = "[status]";
                    isWrongTarget = false;
                }

            }
            else
            {
                lineEnd = endPoint.position;
                isWrongTarget = false;
            }

        }
        else
        {
            selectionLine.enabled = false;
            isWrongTarget = false;
        }




        if (device.GetHairTriggerUp() && target != null && target.tag == "Bacteria")
        {
            selectionLine.enabled = false;
            startIngestion = true;
            target.tag = "Captured Bacteria";
        }


        if(startIngestion)
        {
            //calculate distance between bacteria and an anchor point in player
            if (target != null)
            {
                ingestDist = Vector3.Distance(target.transform.position, anchor.transform.position);
                Debug.Log("Ingestion Dist is " + ingestDist);

            }

            //vertical alignment of player to bacteria
            Vector3 tempPosPlayer = player.transform.position;
            tempPosPlayer.y = hitObjectCenter.y + 5f;
            player.transform.position += (tempPosPlayer - player.transform.position).normalized * alignSpeed;

            //afte vertical alignment is done, player horizontal movement to bacteria
            if (Mathf.Abs(player.transform.position.y - tempPosPlayer.y) <= 0.05f)
            {
                tempPosPlayer.z = hitObjectCenter.z;
                tempPosPlayer.x = hitObjectCenter.x;

                player.transform.position += (tempPosPlayer - player.transform.position).normalized * alignSpeed;
            }

            
            scaleDownRatio = 1f;
            if(target != null)
            {
                scaleDifference = target.transform.localScale.sqrMagnitude - storageScale.transform.localScale.sqrMagnitude;
            }
            

            if (scaleDifference > 0f && ingestDist > 4f)
            {
                //target.transform.localScale -= (target.transform.localScale - storageScale.transform.localScale) / distOnHit * Time.deltaTime * scaleDownRatio;
                target.transform.localScale = Vector3.Lerp(target.transform.localScale, storageScale.transform.localScale * 1.5f, scaleDownRatio * Time.deltaTime);
                capsule.transform.localScale = Vector3.Lerp(capsule.transform.localScale, storageScale.transform.localScale * 3.5f, scaleDownRatio * Time.deltaTime);


                if (ingestDist <= 4f)
                {
                    target.transform.localScale = storageScale.transform.localScale;
                    scaleDownRatio = 0f;
                }

            }


            //if distance between the bacteria and the anchor is less than a set value, the bacteria will snap to the anchor
            if (ingestDist <= 4f )
            {

                target.transform.position = anchor.transform.position;
                //target.transform.localScale = storageScale.transform.localScale;  


                //remove bacteria rotation scripts
                Rotate[] rScripts = target.GetComponents<Rotate>();
                foreach (Rotate r in rScripts)
                {
                    Destroy(r);
                }

                target.transform.rotation = storageScale.transform.rotation;

                capsule.SetActive(false);

                GameObject bacteriaCapsule = Instantiate(bacteriaCapsulePrefeb, anchor.transform.position, storageScale.transform.rotation);
                bacteriaCapsule.transform.localScale = target.transform.localScale * 1.2f;


                target.transform.parent = pivot.transform;
                bacteriaCapsule.transform.parent = pivot.transform;

                Destroy(target.GetComponent<Animation>());

                counter++;

                startIngestion = false;

                //stop carousel rotation
                rotateScript.isRotating = false;

                //show particle enzyme effects
                enzymeParticles1.Play();

                GameObject particlesInside = Instantiate(particlesInsidePrefab, anchor.transform.position, target.transform.rotation);
                particlesInside.transform.localScale = storageScale.transform.localScale * 0.3f;
                particlesInside.transform.parent = bacteriaCapsule.transform;
                
                //show score text and increase neutrophil life (score)
                GameObject scoreText = Instantiate(scoreTextPrefab, anchor.transform.position, anchor.transform.rotation);
                Destroy(scoreText, 1f);

                
                ScoreManager.score += 50;

            }

        }
        



        if (rotateScript.isRotating == true)
        {
            enzymeParticles1.Stop();
        }

    }


    IEnumerator WrongTargetWarning()
    {
        header.color = Color.red;
        header.text = "[warning]";
        invalidIngestWarning.color = Color.red;
        invalidIngestWarning.text = "Enemy is too big to be ingested!";
        isEnemyTooBig = true; //turn on warning audio
        errorBeep.PlayOneShot(error, 1f);

        yield return new WaitForSeconds(2f);
        invalidIngestWarning.color = new Color32(255, 255, 255, 255);
        invalidIngestWarning.text = "Phagocytosis in action!";
        header.color = new Color32(255, 78, 0, 255);
        header.text = "[status]";
    }
}
