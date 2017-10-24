using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MalletAnimation : MonoBehaviour {

    private bool swinging = false;
    public float speed = 20f;

    private Vector3 eulers;


    public void Swing(float inSpeed)
    {
        swinging = true;
        speed = inSpeed;
    }

	// Use this for initialization
	private void Start () {
        eulers = transform.eulerAngles;
	}
	
	// Update is called once per frame
	private void Update () {
		
        if (Input.GetKeyDown(KeyCode.M))
        {
            Swing(speed);
        }
        
	}
    private void FixedUpdate()
    {
        if (swinging)
        {
            //increment position in swing

            eulers.x += speed * Time.deltaTime;

            if (false)
            {
                //set position back to normal (OR REMOVE MALLET FROM SCENE)
                swinging = false;
            }
        }

        transform.eulerAngles = eulers;
    }
}
