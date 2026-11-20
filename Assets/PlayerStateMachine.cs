using NUnit.Framework.Api;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{

    public CinemachineCamera redCam, greenCam, blueCam;
    public float speed = 5f;
    public float jumpForce = 20f;
    public float absoluteMaxSpeed = 6f;
    public float mouseSpeed = 2f;
    public bool sprint;
    public GameObject footStepParticles;
    GameObject leftFootPart, rightFootPart;
    public GameObject leftFoot, rightFoot;

    //our data-validated enum
    public enum playerState
    {
        RED,
        BLUE,
        GREEN
    }

    //a concrete instance of the playerState data type
    public playerState myState;

    Animator myAnim;
    MeshRenderer myMesh;
    Material myMat;
    Rigidbody myRB;
    AudioSource myAudio;

    bool moving;
    public bool jump, canJump, grounded;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAnim = GetComponent<Animator>();
        myRB = GetComponent<Rigidbody>();
        myMesh = GetComponent<MeshRenderer>();
        myMat = myMesh.material;
        TryGetComponent<AudioSource>(out myAudio);
        myRB.maxLinearVelocity = absoluteMaxSpeed;
        canJump = true;

        leftFootPart = Instantiate(footStepParticles, Vector3.zero, Quaternion.identity);
        rightFootPart = Instantiate(footStepParticles, Vector3.zero, Quaternion.identity);
        leftFootPart.SetActive(false);
        rightFootPart.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if(Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)
        {
            sprint = true;
        }
        else { sprint = false; }

        if (Keyboard.current.spaceKey.isPressed && grounded && canJump)
        {
            jump = true;
        }
        else { jump = false; }

        switch(myState)
        {
            case playerState.BLUE:
                //Debug.Log("blue");
                myMat.color = Color.blue;
                blueCam.Prioritize();
                return;

            case playerState.RED:
                //Debug.Log("red");
                myMat.color = Color.red;
                redCam.Prioritize();
                return;

            case playerState.GREEN:
                //Debug.Log("green");
                myMat.color = Color.green;
                greenCam.Prioritize();
                return;

            default:
                Debug.Log("CASE NOT FOUND ERROR");
                return;
        }
        
    }

    void FixedUpdate()
    {
        Vector3 dir = Vector3.zero;

        if(Keyboard.current.aKey.isPressed)
        {
            dir += Vector3.left;
        }
        if(Keyboard.current.wKey.isPressed)
        {
            dir += Vector3.forward;
        }
        if(Keyboard.current.dKey.isPressed)
        {
            dir += Vector3.right;
        }
        if(Keyboard.current.sKey.isPressed)
        {
            dir += Vector3.back;
        }
        dir = dir.normalized * speed * Time.fixedDeltaTime;
        if(sprint) { dir *= 2f; }
        dir = transform.TransformDirection(dir);

        Debug.DrawRay(transform.position, dir, Color.magenta);
        myRB.linearVelocity = dir;

        if(jump) 
        {
            //this forces a .25 second wait between jump requests
            StartCoroutine(jumpRequest(.25f));
        }


        Vector2 mouseOffset = Mouse.current.delta.value;
        float xRot = mouseOffset.x * mouseSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, xRot, 0);

        if(dir != Vector3.zero) { moving = true; }
        else { moving = false; }

        myAnim.SetBool("moving", moving);
        myAnim.SetFloat("speed", myRB.linearVelocity.magnitude);


    }

    public void LeftFootStep(AudioClip c)
    {
        Debug.Log("left foot stepped");
        if (myAudio != null)
        {
            myAudio.PlayOneShot(c);
        }
        leftFootPart.SetActive(false);
        leftFootPart.transform.position = leftFoot.transform.position;
        leftFootPart.SetActive(true);
    }

    public void RightFootStep(AudioClip c)
    {
        Debug.Log("right foot stepped");
        if(myAudio != null)
        {
            myAudio.PlayOneShot(c);
        }
        rightFootPart.SetActive(false);
        rightFootPart.transform.position = rightFoot.transform.position;
        rightFootPart.SetActive(true);
    }


    public void OnCollisionStay(Collision collision)
    {
        grounded = true;
    }

    public void OnCollisionExit(Collision collision)
    {
        grounded = false;
    }

    //this lets us buffer jump requests to avoid double inputs
    IEnumerator jumpRequest(float time)
    {
        canJump = false;
        jump = false;

        Debug.Log("jump fired off on frame: " + Time.frameCount);

        myRB.AddForce(Vector3.up * jumpForce);
        myAnim.SetTrigger("jump");

        //code above this line runs as soon as you execute the coroutine
        yield return new WaitForSeconds(time);

        //code below this line runs once TIME seconds have passed
        canJump = true;
    }


    IEnumerator badCoroutine()
    {
        //while loop can never exit "true" so it runs forever
        while(true)
        {
            //code here will run every fixed update frame
            yield return new WaitForFixedUpdate();
            //and then wait for the next one
        }
    }
}
