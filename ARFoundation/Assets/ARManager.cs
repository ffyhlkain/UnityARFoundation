using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARSessionOrigin arSessionOrigin;
    [SerializeField] private GameObject raycastHitGameObject;

    private GameObject spawnedObject;

    private List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();
    private List<GameObject> augmentedObjects = new List<GameObject>();
    private bool enableTouchControl;

    private void Awake()
    {
        ARSubsystemManager.systemStateChanged += ARSubsystemManager_systemStateChanged;
    }

    private void OnDestroy()
    {
        ARSubsystemManager.systemStateChanged -= ARSubsystemManager_systemStateChanged;
    }

    private void ARSubsystemManager_systemStateChanged(ARSystemStateChangedEventArgs newState)
    {
        
    }

    private void Update()
    {
        if (arSessionOrigin != null && enableTouchControl && Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);

            arRaycastHits.Clear();

            var raycastHits = arSessionOrigin.Raycast(touch.position, arRaycastHits, TrackableType.PlaneWithinPolygon);
            if (raycastHits)
            {
                Debug.Log("Touch input hits: " + arRaycastHits.Count + " colliders.");
                for (int i = 0; i < arRaycastHits.Count; i++)
                {
                    var raycastHit = arRaycastHits[i];
                    var hitPose = raycastHit.pose;
                    
                    if (spawnedObject == null)
                    {
                        spawnedObject = Instantiate(raycastHitGameObject, hitPose.position, hitPose.rotation);
                        Debug.Log("Touch input spawns object at: " + hitPose.position + "-" + hitPose.rotation);
                    }
                    else
                    {
                        spawnedObject.transform.position = hitPose.position;
                        Debug.Log("Touch input moves object to: " + hitPose.position);
                    }
                }
            }
        }
    }

    private void OnGUI()
    {
        if (arSession != null && !arSession.enabled && GUI.Button(new Rect(100, 10, 200, 120), "enable"))
        {
            arSession.enabled = true;
        }
        else if (arSession != null && arSession.enabled && GUI.Button(new Rect(10, 10, 200, 120), "disable"))
        {
            arSession.enabled = false;
        }

        if (arSession != null && GUI.Button(new Rect(100, 150, 200, 120), "reset"))
        {
            arSession.Reset();
        }

        if (arSession != null && !arSession.lightEstimation && GUI.Button(new Rect(100, 290, 200, 120), "en. light est."))
        {
            arSession.lightEstimation = true;
        }
        else if (arSession != null && arSession.lightEstimation && GUI.Button(new Rect(100, 290, 200, 120), "dis. light est."))
        {
            arSession.lightEstimation = false;
        }

        if (!enableTouchControl && arSession != null && GUI.Button(new Rect(100, 430, 200, 120), "cast ray"))
        {
            arSessionOrigin.Raycast(Vector3.zero, arRaycastHits);
            Debug.Log(arRaycastHits.Count + " objects hit.");
            for (int i = 0; i < arRaycastHits.Count; i++)
            {
                var raycastHit = arRaycastHits[i];
                var relativePose = raycastHit.sessionRelativePose;
                var pose = raycastHit.pose;
                var augmentedObject = Instantiate(raycastHitGameObject, pose.position, pose.rotation);
                Debug.Log("place object at pose: " + pose.position + "-" + pose.rotation + " relative pose: " + relativePose.position + "-" + relativePose.rotation);
                augmentedObjects.Add(augmentedObject);
            }

            arRaycastHits.Clear();
        }

        if (arSession != null && GUI.Button(new Rect(100, 570, 200, 120), "clear"))
        {
            for (int i = 0; i < augmentedObjects.Count; i++)
            {
                Destroy(augmentedObjects[i]);
            }
            augmentedObjects.Clear();
        }

        if (arSession != null && GUI.Button(new Rect(100, 710, 200, 120), "Touch: " + enableTouchControl))
        {
            enableTouchControl = !enableTouchControl;
        }
    }
}