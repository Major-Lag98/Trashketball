using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;



public class PlaceObjectOnPlane : MonoBehaviour
{
    [SerializeField]
    GameObject placedPrefab;
    GameObject spawnedObject;
    ARRaycastManager raycaster;

     public GameStateMachine GameStateMachine;

    List<ARRaycastHit> hits = new List<ARRaycastHit>();

    bool EnablePlacement = false;


    // Start is called before the first frame update
    void Start()
    {
        raycaster = GetComponent<ARRaycastManager>();
    }

    void OnPlaceObject(InputValue value)
    {
        // get the screen touch position
        Vector2 touchPosition = value.Get<Vector2>();
        // raycast from the touch position into the 3D scene looking for a plane
        // if the raycast hit a plane, then
        if (raycaster.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            // get the hit point (pose) on the plane
            Pose hitPose = hits[0].pose;
            // if this is the first time placing an object, or we are in placement mode
            if (EnablePlacement)
            {
                // instantiate the prefab at the hit position and rotation
                spawnedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation);
                EnablePlacement = false;
                GameStateMachine.ChangeState(GameStateMachine.GameState.Flicking);
            }
        }
    }

    public void BTNEnablePlancement()
    {
        EnablePlacement = true;
    }

}
