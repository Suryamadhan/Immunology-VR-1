using System.Collections;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class MagicMoveV4 : MonoBehaviour
{

    bool webbing; //true when trying to move through spider-man metaphor
    bool modifiedMovement = true;
    int webCount; //0 when you run the movement for the first time
    float length, adjustmentConstant, currentDist, adjustSpeed, constant;

    private Vector2 touchPad;

    public float beamLength = 170f;
    public GameObject pointerHead;
    private CollisionDetection _colDetect;

    public GameObject player;
    float speed;
    private float speedReset;
    public float sensitivityRot;
    public GameObject sphere;
    public float turningRate;
    private Vector3 target;
    public bool isMoving;
    public GameObject lineR; // create a reference to linerenderer on an empty gameobject in right controller


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

        _colDetect = pointerHead.GetComponent<CollisionDetection>();
        speedReset = speed;
    }

    void setSpeed()
    {
        if (webbing)
        {
            if (webCount == 0)
            {
                length = Mathf.Abs(Vector3.Distance(player.transform.position, hitPoints[0])); //orginal length of full web
                currentDist = Mathf.Abs(Vector3.Distance(player.transform.position, hitPoints[0])); //current distance from neutrophil to destination
                adjustmentConstant = length * (.45f / 120f); //returns max speed (apex of the function)
                if (currentDist <= 10)
                {
                    adjustSpeed = 0;
                }
                else if (currentDist > (length / 2.0f))
                {
                    adjustSpeed = (-2 * ((currentDist * adjustmentConstant) / length)) + (2 * adjustmentConstant) + .2f;
                }
                else if (currentDist < (length / 2.0f))
                {
                    adjustSpeed = (2.8f * ((currentDist * adjustmentConstant) / length)) + .05f;
                }
                adjustSpeed *= .8f;
                speed = adjustSpeed;
                adjustSpeed = 0;
                webCount++;
            }
            else
            {
                currentDist = Mathf.Abs(Vector3.Distance(player.transform.position, hitPoints[0])); //current distance from neutrophil to destination
                adjustmentConstant = length * (.45f / 120f); //returns max speed (apex of the function)
                if (currentDist <= 10)
                {
                    adjustSpeed = 0;
                }
                else if (currentDist > (length / 2.0f))
                {
                    adjustSpeed = (-2 * ((currentDist * adjustmentConstant) / length)) + (2 * adjustmentConstant) + .2f;
                }
                else if (currentDist < (length / 2.0f))
                {
                    adjustSpeed = (2.8f * ((currentDist * adjustmentConstant) / length)) + .05f;
                }
                adjustSpeed *= .8f;
                speed = adjustSpeed;
                adjustSpeed = 0;
            }
        }
        else
        {
            speed = .18f;
            adjustSpeed = 0;
            webCount = 0;
        }

    }

    void Update()
    {
        if (modifiedMovement)
        {
            if (device.GetHairTrigger())
            {
                webbing = true;
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
                        setSpeed();
                        player.transform.position += (target - controller.transform.position).normalized * speed;

                        dist = Vector3.Distance(target, player.transform.position);

                        if (dist <= 10f)
                        {
                            speed = 0f;
                        }

                        if (hit.collider != null && dist <= 30f && hit.collider.tag == "Bacteria Bighead")
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
                webbing = false;
                webCount = 0;
                speed = .18f;
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
        }
        else
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
                        speed = .2f;
                        dist = Mathf.Abs(Vector3.Distance(player.transform.position, hitPoints[0]));

                        if (dist <= 10f)
                        {
                            speed = 0f;
                        }

                        if (hit.collider != null && dist <= 30f && hit.collider.tag == "Bacteria Bighead")
                        {
                            speed = 0f;

                        }
                        player.transform.position += (target - controller.transform.position).normalized * speed;
                        
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

        }
    }
}
