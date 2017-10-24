using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour {

    public float maxPower = 50000;
    public GameObject ball;
    public float arrowSpeed;
    public float ballSpeed;
    public Material arrowMaterial;

    private Rigidbody ballRb;
    private float arrowVertical;
    private float arrowHorizontal;
    private Vector3 eulers;
    private float power;



	// Use this for initialization
	private void Start ()
    {
        ballRb = ball.GetComponent<Rigidbody>();

        power = 0;

        eulers = transform.eulerAngles;

    }
	
	
	private void Update ()
    {
        //update arrow position
        transform.position = ball.transform.position;

        //get input from user..
        arrowVertical = Input.GetAxis("Vertical");
        arrowHorizontal = Input.GetAxis("Horizontal");

        eulers.y -= arrowHorizontal * arrowSpeed * Time.deltaTime;
        eulers.z -= arrowVertical * arrowSpeed * Time.deltaTime;

        //while enter key is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            if (power < maxPower)
            {
                power += ballSpeed;
            }
            arrowMaterial.SetColor("_Color", new Color(0.3f + 0.9f * power / maxPower, 0.9f - 0.9f * power / maxPower, 0f));
        }
        else
        {
            power = 0;
            arrowMaterial.SetColor("_Color", Color.yellow);
        }

        transform.eulerAngles = eulers;

        
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Vector3 force = transform.right * power * Time.deltaTime;
            ballRb.AddForce(force);
        }

    }
}
