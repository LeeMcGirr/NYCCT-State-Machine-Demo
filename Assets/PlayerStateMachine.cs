using NUnit.Framework.Api;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    public CinemachineCamera redCam, greenCam, blueCam;
    public float speed = 5f;
    public float mouseSpeed = 2f;
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

    bool moving;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myAnim = GetComponent<Animator>();
        myRB = GetComponent<Rigidbody>();
        myMesh = GetComponent<MeshRenderer>();
        myMat = myMesh.material;
    }

    // Update is called once per frame
    void Update()
    {

        switch(myState)
        {
            case playerState.BLUE:
                Debug.Log("blue");
                myMat.color = Color.blue;
                blueCam.Prioritize();
                return;

            case playerState.RED:
                Debug.Log("red");
                myMat.color = Color.red;
                redCam.Prioritize();
                return;

            case playerState.GREEN:
                Debug.Log("green");
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
        Debug.Log("local dir: " + dir);
        dir = dir.normalized * speed * Time.fixedDeltaTime;
        dir = transform.TransformDirection(dir);

        Debug.DrawRay(transform.position, dir, Color.magenta);
        Debug.Log("world dir: " + dir);

        myRB.linearVelocity = dir;



        Vector2 mouseOffset = Mouse.current.delta.value;
        float xRot = mouseOffset.x * mouseSpeed * Time.fixedDeltaTime;
        transform.Rotate(0, xRot, 0);

        if(dir != Vector3.zero) { moving = true; }
        else { moving = false; }
        myAnim.SetBool("moving", moving);


    }
}
