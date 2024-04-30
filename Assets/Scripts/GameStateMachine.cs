using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateMachine : MonoBehaviour
{
    // Reference to the PlaceObjectOnPlane script
    public PlaceObjectOnPlane placeObjectOnPlane;

    // Button to enable placing
    [Space(10)]
    public GameObject EnablePlacingBTN;

    // Text Mesh Pro for the different states
    [Space(10)]
    public GameObject StartingTXT;
    public GameObject PlacingTXT;
    public GameObject FlickingTXT;
    public GameObject WaitingTXT;
    public GameObject EndingTXT;

    // ball & camera for flicking state
    [Space(10)]
    public GameObject Ball;
    public GameObject MainCamera;

    // variables for shot distance
    public Vector3 shotLocation;

    // variables for the timer
    [Space(10)]
    float currentTime = 0;
    public float maxTime = 3;

    // Touching
    Vector2 StartPos;
    Vector2 Direction;
    public float Scaler = 0.1f;

    // line renderer for waiting
    public LineRenderer lineRenderer;

    // Enum for the different states
    public enum GameState
    {
        Starting,
        Placing,
        Flicking,
        Waiting,
        Ending
    }
    public static GameState CurrentState;


    // create a dictionary to store the game state and the text that will be displayed
    Dictionary<GameState, GameObject> keyValuePairs = new Dictionary<GameState, GameObject>();

    // awake is before start
    private void Awake()
    {
        // add the game states and the text to the dictionary on awake
        keyValuePairs.Add(GameState.Starting, StartingTXT);
        keyValuePairs.Add(GameState.Placing, PlacingTXT);
        keyValuePairs.Add(GameState.Flicking, FlickingTXT);
        keyValuePairs.Add(GameState.Waiting, WaitingTXT);
        keyValuePairs.Add(GameState.Ending, EndingTXT);

        // disable all the text because we are enabling them in change state
        foreach (var item in keyValuePairs)
        {
            item.Value.SetActive(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // start in starting state
        ChangeState(GameState.Starting);
    }

    private void Update()
    {
        switch (CurrentState)
        {
            // if the ball is flicked, track the finger location and when the finger is lifted, apply force to the ball and change the state to waiting
            case GameState.Flicking:



                // if the user touches the screen get the location of the touch and record it into a list
                // if the user lets go of the screen, calculate the force
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        // shotLocation = MainCamera.transform.position;
                           StartPos = touch.position;
                        break;

                    case TouchPhase.Moved:
                        Direction = touch.position - StartPos;
                        break;

                    case TouchPhase.Ended:
                        GameObject BallToFlick = GameObject.FindGameObjectWithTag("Ball");
                        BallToFlick.transform.parent = null;
                        BallToFlick.GetComponent<Rigidbody>().useGravity = true;
                        BallToFlick.GetComponent<Rigidbody>().isKinematic = false;
                        // add an upwards force and a forward force to the ball
                        Vector3 directionOfForce = (BallToFlick.transform.up + BallToFlick.transform.forward * 0.5f).normalized;
                        BallToFlick.GetComponent<Rigidbody>().AddForce(directionOfForce * Direction.magnitude * Scaler);
                        ChangeState(GameState.Waiting);
                        // clear the line renderer
                        lineRenderer.positionCount = 0;
                        break;
                }
                break;

            case GameState.Waiting:
                // every frame crate a point at the balls location and add it to the line renderer
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, GameObject.FindGameObjectWithTag("Ball").transform.position);

                // if the ball is not moving or X seconds have passed since waiting state has started, change the state back to flicking
                currentTime += Time.deltaTime;
                if (currentTime >= maxTime)
                {
                    // if we are in the waiting state for too long go back to flicking
                    ChangeState(GameState.Flicking);
                    currentTime = 0;
                }
                break;

                
        }
    }

    // Change the state of the game
    public void ChangeState(GameState newState)
    {
        // disable the current state text
        keyValuePairs[CurrentState]?.SetActive(false);

        // set the new state to the current state and enable the text
        CurrentState = newState;
        keyValuePairs[CurrentState].SetActive(true);

        switch (newState) 
        {
            case GameState.Starting:
                ToStarting();
                break;

            case GameState.Placing:
                ToPlacing();
                break;

            case GameState.Flicking:
                ToFlicking();
                break;

            case GameState.Waiting:
                ToWaiting();
                break;

            case GameState.Ending:
                ToEnding();
                break;
        }

    }

    void ToEnding()
    {
        // get the distace between the shot location and the trash can (tag=PlacedThing)
        Vector3 trashCan = GameObject.FindGameObjectWithTag("PlacedThing").transform.position;
        float distance = Vector3.Distance(shotLocation, trashCan);
        EndingTXT.GetComponent<TMPro.TextMeshProUGUI>().text = "Ending State. \nDistance: " + distance.ToString("F2") + "m";
    }

    void ToWaiting()
    {

    }

    void ToStarting()
    {
        // FlickingTXT.SetActive(false); // turn off the flicking text
        EnablePlacingBTN.SetActive(true); // turn on the placing button
    }

    void ToPlacing()
    {
        // ToFlicking();
        GameObject PlacedThing = GameObject.FindGameObjectWithTag("PlacedThing");
        GameObject BallToFlick = GameObject.FindGameObjectWithTag("Ball");
        if (BallToFlick != null)
        {
            Destroy(BallToFlick);
        }
        if (PlacedThing != null)
        {
            Destroy(PlacedThing);
        }
        FlickingTXT.SetActive(false);
        placeObjectOnPlane.BTNEnablePlancement();
        EnablePlacingBTN.SetActive(false);
        
    }

    void ToFlicking()
    {
        
        // if the ball exists for some reason destroy it
        GameObject BallToDestroy = GameObject.FindGameObjectWithTag("Ball");
        if (BallToDestroy != null)
        {
            Destroy(BallToDestroy);
        }

        // initialize the flicking state
        FlickingTXT.SetActive(true);
        EnablePlacingBTN.SetActive(true);
        // create a new ball
        GameObject newBall = Instantiate(Ball, MainCamera.transform.position, MainCamera.transform.rotation);
        // traslate the ball local forward by 1 unit, and down by 0.35 units
        newBall.transform.Translate(Vector3.forward + Vector3.down * 0.35f);
        // set the ball color to red
        newBall.GetComponent<MeshRenderer>().material.color = Color.red;
        // parent the ball to the camera
        newBall.transform.parent = MainCamera.transform;

    }

    public void BTNChangeToPlacing()
    {
        ChangeState(GameState.Placing);
    }

    // When we placed an object, Go to Flicking
    // When we pressed the button go back to Placing


}
