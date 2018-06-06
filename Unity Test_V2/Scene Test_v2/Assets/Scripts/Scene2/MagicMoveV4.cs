using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class MagicMoveV4 : MonoBehaviour
{

    //   public Slider slider;
    //   public GameObject sliderPanel;
    //   private bool isON;
   //    private float minBeamLength = 20f;
   //    private float maxBeamLength = 120f;


    private Vector2 touchPad;


    public float beamLength=120f;
    public GameObject pointerHead;
    private CollisionDetection _colDetect;

    public GameObject player;
    public float speed;
    private float speedReset;
    public float sensitivityRot;
    public GameObject sphere;
    public float turningRate;
    private Vector3 target;
    public bool isMoving;
    public GameObject lineR; // create a reference to linerenderer on an empty gameobject in right controller
    //private BoxCollider boxCol; // box collider on the linerenderer

    //create a list of hitpoints
    private List<Vector3> hitPoints = new List<Vector3>();

    public Transform endPoint;
    public float dist;
    //private float counter = 0f;
    Vector3 pointA;
    public bool lengthLimit;
    private float x = 0f;
    //private float y = 0f;

 

    //vive controller tracking and input
    public SteamVR_TrackedObject controller;
    private SteamVR_Controller.Device device
    {
        get { return SteamVR_Controller.Input((int)controller.index); }
    }

    public LineRenderer lineHit;
    public LineRenderer lineEmpty;

    //private Vector3 tempController;
    

    void Awake()
    {

        controller = GetComponent<SteamVR_TrackedObject>();
        sphere.SetActive(false);
        pointerHead.SetActive(false);
        //boxCol = lineR.GetComponent<BoxCollider>();
        _colDetect = pointerHead.GetComponent<CollisionDetection>();
        speedReset = speed;
        
      
    }

    

    void Update()
    {

        if (device.GetHairTrigger())
        {
            pointerHead.SetActive(true);

            lineEmpty.enabled = true;

            lineEmpty.SetPosition(0, controller.transform.position);

            lineEmpty.material.mainTextureOffset = new Vector2(lineEmpty.material.mainTextureOffset.x - Random.Range(-0.01f, 0.05f), 0f);

            if (x < beamLength && !lengthLimit && !_colDetect.isColliding)
            {
                x += 0.9f;
                pointA = controller.transform.position;
                Vector3 pointAlongLine = x * controller.transform.forward + pointA;

                if (x > beamLength)
                {
                    lengthLimit = true;
                    x = beamLength;
                }

                lineEmpty.SetPosition(1, pointAlongLine);
                endPoint.transform.position = pointAlongLine;
                
            }

            if (x <= beamLength && lengthLimit && !_colDetect.isColliding)
            {
                x -= 1f;

                if (x < 0f)
                {
                    x = 0f;
                    pointerHead.SetActive(false);
                    lineEmpty.enabled = false;
                }

                Vector3 pointAlongLineBack = x * controller.transform.forward + pointA;
                //dist = Vector3.Distance(controller.transform.position, pointAlongLineBack);

                endPoint.transform.position = pointAlongLineBack;
                lineEmpty.SetPosition(1, pointAlongLineBack);

            }


            if (_colDetect.isColliding)
            {
                lineEmpty.enabled = false;
                lineHit.enabled = true;
                lineHit.material.mainTextureOffset = new Vector2(lineEmpty.material.mainTextureOffset.x - Random.Range(-0.01f, 0.05f), 0f);
                pointerHead.SetActive(false);

                RaycastHit hit;
                if (Physics.Raycast(controller.transform.position, controller.transform.forward, out hit))
                {
                    Debug.DrawRay(controller.transform.position, controller.transform.forward * 50f, Color.red);
                    //Debug.Log("hit point is " + hit.point);

                    hitPoints.Add(hit.point);
                    target = hitPoints[0];

                    lineHit.SetPosition(0, controller.transform.position);
                    lineHit.SetPosition(1, target);

                    endPoint.transform.position = target;

                    sphere.SetActive(true);
                    sphere.transform.position = target;

                    
                }

                //start cockpit rotation and movement
                Quaternion r = Quaternion.LookRotation(target - player.transform.position);
                Quaternion r2 = Quaternion.Euler(r.eulerAngles.x, r.eulerAngles.y, player.transform.rotation.z);
                player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, r2, turningRate * Time.deltaTime);

                if (player.transform.rotation == r2)
                {
                    isMoving = true;
                }

                if (isMoving)
                {
                    player.transform.position += (target - controller.transform.position).normalized * speed;

                    dist = Vector3.Distance(target, player.transform.position);

                    if (dist <= 10f)
                    {
                        speed = 0f;
                    }

                    if(hit.collider != null && dist <= 30f && hit.collider.tag == "Bacteria Bighead")
                    {
                        speed = 0f;
                      
                    }
                }

            }

        }
        else
        {
            lineHit.enabled = false;
            lineEmpty.enabled = false;
            sphere.SetActive(false);
            isMoving = false;
            hitPoints.Clear();
            lengthLimit = false;
            x = 0f;
            speed = speedReset;
            endPoint.transform.position = new Vector3(0f, 0f, 0f);
            pointerHead.SetActive(false);
            _colDetect.isColliding = false;
        }




        //touchpad control pitch and yaw of neutrophil body
        if (device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {


            touchPad = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

            if (touchPad.x > 0.5f || touchPad.x < -0.5f)
            {
                player.transform.Rotate(0, touchPad.x * sensitivityRot, 0);
            }


            if (touchPad.y > 0.5f || touchPad.y < -0.5f)
            {
                player.transform.Rotate((-1f) * touchPad.y * sensitivityRot, 0, 0);
            }
        }


/*
        //touchpad slider to change beam length

        if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {
            sliderPanel.SetActive(!isON);
            isON = !isON;
        }

        if (isON && device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            touchPad = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

            slider.minValue = 0f;
            slider.maxValue = 1f;

            if(touchPad.y > 0 || touchPad.y < 0) {

                touchPad.y = 1f + touchPad.y;
            }

            if (touchPad.y == 0f)
            {
                touchPad.y = 0f;
            }

            
            slider.value = touchPad.y/2;
            beamLength = slider.value*(maxBeamLength - minBeamLength) + minBeamLength;
            
        }

*/
        
    }


}
