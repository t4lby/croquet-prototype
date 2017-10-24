using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
public class Player
{
    int turn;
    GameObject[] balls;
    string name;

    public Player(string inName, GameObject[] inBalls)
    {
        name = inName;
        balls = inBalls;
        turn = 1;
    }

    
}*/



public class GameController : MonoBehaviour {

    public POVCamera mallet;

    public GameObject blueBall; //ball0
    public GameObject redBall; //ball1
    public GameObject blackBall; //ball2
    public GameObject yellowBall; //ball3

    private bool[] collisions = new bool[4]; //keeps track of collisions
    private bool[] roquets = new bool[4]; //true if can roquet ball i

    public Vector3 startPoint;

    public GameObject[] balls;
    public BoxCollider[] hoops;
    public Text playerText;
    public bool startOfTurn;

    private int currentPlayer;
    private int currentBall;
    public bool croquetShot;
    private bool continuation;
    private int croquee;
    private int currentBallCurrentHoop;

    public void ThroughHoop()
    {
        continuation = true;
        resetRoquets();
    }

    public void switchBall()
    {
        currentBall = (currentBall + 2) % 4;
        mallet.MalletReset(balls[currentBall]);    
    }

    public void Collided(int ballNo)
    {
        collisions[ballNo] = true;
    }

    private void resetCollisions()
    {
        for (int i = 0; i < 4; i++)
        {
            collisions[i] = false;
        }
    }

    private void resetRoquets()
    {
        for (int i = 0; i < 4; i++)
        {
            if (i != currentBall)
                roquets[i] = true;
            else
                roquets[i] = false;
        }
    }

    private bool isCollision()
    {
        bool col = false;
        for (int i=0; i<4; i++)
        {
            if (collisions[i])
                col = true;
        }
        return col;
    }
    
	// Use this for initialization
	private void Start () {
        for (int i = 0; i < 4; i++)
        {
            balls[i].SetActive(false);
            balls[i].transform.position = startPoint;
        }

        currentBall = 0;
        
        mallet.MalletReset(balls[0]);

        currentPlayer = 0;
        playerText.text = "Current Player : " + (currentPlayer + 1);

        resetCollisions();
        resetRoquets();
        croquetShot = false;
        startOfTurn = true;
 
	}
	
	// Update is called once per frame
	private void Update () {

        if (mallet.finished)
        {
            
            if (continuation)
            {
                mallet.MalletReset(balls[currentBall]);
                continuation = false;
                startOfTurn = false;
            }
            else if (croquetShot)
            {
                mallet.RoquetReset(balls[currentBall], balls[croquee]);
                continuation = true;
                croquetShot = false;
                startOfTurn = false;
            }
            else
            {
                currentPlayer = (currentPlayer + 1) % 2;
                currentBall = (currentBall + 1) % 4;
                playerText.text = "Current Player : " + (currentPlayer + 1);
                startOfTurn = true;
                mallet.MalletReset(balls[currentBall]);
                resetRoquets();
            }
            
            
        }
        
	}

    private void FixedUpdate()
    {
        if (isCollision())
        {
            //process
            if (!croquetShot && !continuation)
            {
                for (int i=0; i<4; i++)
                {
                    if (collisions[i] && currentBall != i && roquets[i])
                    {
                        croquetShot = true;
                        croquee = i;
                        roquets[i] = false;
                    }
                }
            }

            //reset
            resetCollisions();
        }
    }
}
