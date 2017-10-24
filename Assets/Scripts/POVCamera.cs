using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POVCamera : MonoBehaviour {


    public Rigidbody[] allBallRbs; //array of all balls in order blue, red, black, yellow.
    public GameObject ball; //target (initially set in inspector)
    public GameController controller;

    private Rigidbody ballRb; //apply appropriate forces to this
    private Transform ballTrans;
    private Transform RoqueeTrans;

    public Transform swingTransform; //used to swing mallet and camera
    public Transform rotateTransform;
    public Vector3 aimLocation = new Vector3(0f, 7.1f, -8.8f);
    public Vector3 aimRotation = new Vector3(-35f, 0f, 0f);
    public Vector3 shootLocation = new Vector3(0f, 8f, -2.2f);
    public Vector3 shootRotation = new Vector3(0f, 0f, 0f);
    public GameObject malletChild;
    public float rotateSpeed;
    public float backSwingSpeed = 0.5f;
    public float strikeSpeed = 15f;
    public float powerIncrement = 8f;
    public float knockBackFactor = 0.7f;
    public float forceModifier = 3f;
    public float backSwingLimit; //the limit to how far you can swing the mallet back
    public float forwardSwingLimit; //how far the mallet swings forward in the strike before becoming inactive
    public float stopVelocity = 0.2f; //the velocity of the ball at which we consider the shot ended (takes too long to wait for exact zero)
    public float croquetAimSpeed = 5f;

    private float horizontalMove;
    private float verticalMove;
    private float power;

    public AudioSource hitSound;
    public float hitSoundFactor = 0.01f;

    private Vector3 veiwMoveVelocity; //used to track velocity whilst moving camera to shoot;
    private float veiwAngleVelocity;
    public float veiwDampTime = 5f;

    //TO DOO: CHANGE these bools to State Structure
    private bool veiwToShoot;
    private bool shooting;
    private bool forwardSwinging;
    private bool forceApplied;
    private bool lineUpCroquet;

    public bool finished;

	// Use this for initialization
	private void Start () {
    
    }

    public float MaxSpeed()
    {
        float max = 0;
        for (int i = 0; i < allBallRbs.Length; i++)
        {
            float vel = allBallRbs[i].velocity.magnitude;
            if (vel > max)
            {
                max = vel;
            }
        }

        return max;
    }

    public void RoquetReset(GameObject playersBall, GameObject roquee)
    {
        ball = playersBall;
        ballRb = playersBall.GetComponent<Rigidbody>();
        ballTrans = ball.transform;

        RoqueeTrans = roquee.transform;
        //put players ball up against the roquee
        ballTrans.position = RoqueeTrans.position + transform.right; 

        transform.position = ballTrans.position;
        swingTransform.localPosition = aimLocation;
        swingTransform.localEulerAngles = aimRotation;
        shooting = false;
        forwardSwinging = false;
        forceApplied = false;
        veiwToShoot = false;
        finished = false;
        lineUpCroquet = true;
        malletChild.SetActive(true);
    }

    public void MalletReset(GameObject inBall) //modify to reset for particular target
    {
        ball = inBall;
        ball.SetActive(true);
        ballRb = ball.GetComponent<Rigidbody>();
        ballTrans = ball.transform;
        transform.position = ballTrans.position;
        swingTransform.localPosition = aimLocation;
        swingTransform.localEulerAngles = aimRotation;
        shooting = false;
        forwardSwinging = false;
        forceApplied = false;
        veiwToShoot = false;
        finished = false;
        lineUpCroquet = false;
        malletChild.SetActive(true);
    }

	// Update is called once per frame
	private void Update () {
        //gather input
        //AXIS for camera
        

        horizontalMove = Input.GetAxis("Horizontal");
        verticalMove = Input.GetAxis("Vertical");

        if (Input.GetKeyDown(KeyCode.E) && controller.startOfTurn)
        {
            controller.switchBall();
        }

        if (Input.GetKeyDown(KeyCode.Space)){
            //snap mallet and camera to shooting position

            lineUpCroquet = false;

            if (veiwToShoot == false)
            {
                veiwToShoot = true;
            }
            else
            {
                veiwToShoot = false;

                swingTransform.localPosition = shootLocation; //make sure mallet in correct position and rotation
                swingTransform.localEulerAngles = shootRotation;

                //disable aiming (enter shooting mode)
                shooting = true;
                power = 0f;
            }
            
        }
        
        if (!veiwToShoot && Input.GetKey(KeyCode.Space))
        {  
            if (swingTransform.eulerAngles.x < 60)
            {
                swingTransform.RotateAround(swingTransform.position, swingTransform.right, rotateSpeed * backSwingSpeed * Time.deltaTime);
                power += powerIncrement;
            }
        }

        if (!veiwToShoot && Input.GetKeyUp(KeyCode.Space))
        {
            forwardSwinging = true;
        }

        if (veiwToShoot)
        {
            swingTransform.localPosition = Vector3.SmoothDamp(swingTransform.localPosition, shootLocation, ref veiwMoveVelocity, veiwDampTime);
            float angle = swingTransform.localEulerAngles.x;
            angle = Mathf.SmoothDamp(angle, 359.9f, ref veiwAngleVelocity, veiwDampTime);
            swingTransform.localEulerAngles = new Vector3(angle, swingTransform.localEulerAngles.y, swingTransform.localEulerAngles.z);
        }
        
        if (forwardSwinging)
        {
            
            if (swingTransform.eulerAngles.x > 360-forwardSwingLimit || swingTransform.eulerAngles.x <= backSwingLimit+1) {
                if (swingTransform.eulerAngles.x < 180)
                {
                    swingTransform.RotateAround(swingTransform.position, swingTransform.right, -strikeSpeed * power * Time.deltaTime);
                }
                else
                {
                    swingTransform.RotateAround(swingTransform.position, swingTransform.right, -strikeSpeed * power * Time.deltaTime * knockBackFactor);
                }

            }
            else
            {
                forwardSwinging = false;
                malletChild.SetActive(false);
            }
        }
        
           
        if (!shooting)
        {
            //set 
            if (!veiwToShoot)
            {
                swingTransform.RotateAround(swingTransform.position, swingTransform.right, -rotateSpeed * verticalMove * Time.deltaTime);
            }
            
            rotateTransform.RotateAround(rotateTransform.position, transform.up, rotateSpeed * horizontalMove * Time.deltaTime);
        }
        
        if (lineUpCroquet)
        {
            if (Input.GetKey(KeyCode.Comma))
            {
                //move ball round clockwise
                ballTrans.RotateAround(RoqueeTrans.position, transform.up, croquetAimSpeed);
                transform.position = ballTrans.position;
            }
            else if (Input.GetKey(KeyCode.Period))
            {
                //move ball round anticlockwise
                ballTrans.RotateAround(RoqueeTrans.position, transform.up, -croquetAimSpeed);
                transform.position = ballTrans.position;
            }
        }


        if (shooting && forceApplied && MaxSpeed() < stopVelocity)
        {
            for (int i = 0; i < allBallRbs.Length; i++)
            {
                allBallRbs[i].velocity = Vector3.zero;
                allBallRbs[i].angularVelocity = Vector3.zero;
            }
            
            finished = true;
        }

    }

    private void FixedUpdate()
    {
        if (forwardSwinging)
        {
            //Debug.Log(swingTransform.eulerAngles.x);
            //Debug.Log(forceApplied);
            if (swingTransform.eulerAngles.x > 350 && !forceApplied)
            {
                Vector3 direction = ballTrans.position - swingTransform.position;
                direction.y = 0;
                //Debug.Log(direction * power * forceModifier);
                ballRb.AddForce(direction * power * forceModifier);
                hitSound.volume = power * hitSoundFactor;
                hitSound.Play();
                forceApplied = true;
            }
        }

        
    }
}
