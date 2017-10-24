using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BallController : MonoBehaviour
{

    //balls keep track of where they are on the course and other balls they have roqueyed this turn
    public int currentHoop = 1;
    public Text currentHoopText;
    public int player;

    public int number;
    public string color;
    public GameController controller;
    public AudioSource ballOnBallSound;
    public AudioSource ballOnHoopSound;
    public float volumeScaleFactor;

    private Rigidbody rb;

    private bool[] roquets = new bool[4];

    private void resetRoquets()
    {
        for (int i = 1; i < roquets.Length; i++)
        {
            roquets[i] = false;
        }
    }


    // Use this for initialization
    private void Start()
    {
        //currentHoop = 1;
        resetRoquets();
        rb = GetComponent<Rigidbody>();
        currentHoopText.text = color + " (Player " + player + ") : " + currentHoop;

    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "ball")
        {
            ballOnBallSound.volume = rb.velocity.magnitude * volumeScaleFactor;
            ballOnBallSound.Play();
            controller.Collided(number);
        }

        
        if (collision.gameObject.tag == "post")
        {

            float vol = rb.velocity.magnitude * volumeScaleFactor * 2;
            ballOnHoopSound.volume = vol;
            ballOnHoopSound.Play();

        }
    }


    

    private void OnTriggerExit(Collider other)
    {
        if (currentHoop < 7)
        {
            if (other.tag == "hoop1" || other.tag == "hoop2" || other.tag == "hoop5" || other.tag == "hoop6")
            {
            
                //check direction been through hoop
                Vector3 relative = transform.position - other.transform.position;
                if (relative.z > 0)
                {
                    if (other.tag == "hoop" + currentHoop)
                    {
                        controller.ThroughHoop();
                        currentHoop += 1;
                    }
                }
            }

            if (other.tag == "hoop3" || other.tag == "hoop4")
            {

                //check direction been through hoop
                Vector3 relative = transform.position - other.transform.position;
                if (relative.z < 0)
                {
                    if (other.tag == "hoop" + currentHoop)
                    {
                        controller.ThroughHoop();
                        currentHoop += 1;
                    }
                }
            }
        }

        currentHoopText.text = color + " (Player " + player + ") : " + currentHoop;
        

        //TODO implement for second hoop round


    }
}
