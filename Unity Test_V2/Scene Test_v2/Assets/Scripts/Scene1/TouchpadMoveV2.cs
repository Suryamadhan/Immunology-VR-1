using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class TouchpadMoveV2 : MonoBehaviour
{
    public enum Rotation
    {
        Forward,
        Back,
        Left,
        Right
    }


    public GameObject player;
    public float speed;
    public float sensitivityX = 0.3f;
    public GameObject shell;

    Rotation shellRotation;
    
    // Controller tracked object setup
    SteamVR_Controller.Device device;
    SteamVR_TrackedObject controller;

    public Vector2 touchpad;

       
    void Start()
    {
        controller = gameObject.GetComponent<SteamVR_TrackedObject>();

    }

    // Update is called once per frame
    void Update()
    {
        device = SteamVR_Controller.Input((int)controller.index);
        //If finger is on touchpad
        if (device.GetTouch(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //Read the touchpad values 
            touchpad = device.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

            
            // Handle movement via touchpad
            if (touchpad.y > 0.5f && touchpad.y > touchpad.x)
            {
                // Move Forward
                player.transform.position += player.transform.forward * Time.deltaTime * (touchpad.y * speed);
                shellRotation = Rotation.Forward;
                Forward();

                //shell.transform.Rotate(Vector3.right* 2f);

            }
           


            if (touchpad.y < -0.5f && touchpad.y < touchpad.x)
            {
                // Move Back
                player.transform.position += player.transform.forward * Time.deltaTime * (touchpad.y * speed);

                //shell.transform.Rotate(Vector3.right* (-2f));
                shellRotation = Rotation.Back;
                Back();
            }
            

            // Rotation: yaw
            if (touchpad.x > 0.5f && touchpad.x > touchpad.y)
            {
                player.transform.Rotate(0, touchpad.x * sensitivityX, 0);

                //shell.transform.Rotate(Vector3.up * 2f);
                shellRotation = Rotation.Right;
                Right();
            }
            


            if (touchpad.x < -0.5f && touchpad.x < touchpad.y)
            {
                player.transform.Rotate(0, touchpad.x * sensitivityX, 0);

                //shell.transform.Rotate(Vector3.up * (-2f));
                shellRotation = Rotation.Left;
                Left();

            }
            

        }
        

    }


    void Forward()
    {
        if(shellRotation == Rotation.Forward)
        {
            shell.transform.Rotate(Vector3.right * 2f);
        }
    }
   
    void Back()
    {
        if(shellRotation == Rotation.Back)
        {
            shell.transform.Rotate(Vector3.right * (-2f));
        }
    }

    void Left()
    {
        if(shellRotation== Rotation.Left)
        {
            shell.transform.Rotate(Vector3.up * -2f);
        }
    }

    void Right()
    {
        if(shellRotation == Rotation.Right)
        {
            shell.transform.Rotate(Vector3.up * 2f);
        }
    }

}
