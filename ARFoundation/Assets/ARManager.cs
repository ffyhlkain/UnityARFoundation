using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    [SerializeField] private ARSession arSession;
    [SerializeField] private ARSessionOrigin arSessionOrigin;
    [SerializeField] private GameObject raycastHitGameObject;


    private List<ARRaycastHit> arRaycastHits = new List<ARRaycastHit>();

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


    //private void Update()
    //{
    //    if (arSessionOrigin != null)
    //    {
    //        arRaycastHits.Clear();

    //        var raycastHits = arSessionOrigin.Raycast(Vector3.zero, arRaycastHits);
    //        if (raycastHits)
    //        {

    //        }
    //    }
    //}

    private void OnGUI()
    {
        if (arSession != null && !arSession.enabled && GUI.Button(new Rect(100, 10, 100, 60), "enable"))
        {
            arSession.enabled = true;
        }
        else if (arSession != null && arSession.enabled && GUI.Button(new Rect(10, 100, 100, 60), "disable"))
        {
            arSession.enabled = false;
        }

        if (arSession != null && GUI.Button(new Rect(100, 80, 100, 60), "reset"))
        {
            arSession.Reset();
            
        }

        if (arSession != null && !arSession.lightEstimation && GUI.Button(new Rect(100, 150, 100, 60), "en. light est."))
        {
            arSession.lightEstimation = true;
        }
        else if (arSession != null && arSession.lightEstimation && GUI.Button(new Rect(100, 150, 100, 60), "dis. light est."))
        {
            arSession.lightEstimation = false;
        }

        if (arSession != null && GUI.Button(new Rect(100, 220, 100, 60), "cast ray"))
        {
            arSessionOrigin.Raycast(Vector3.zero, arRaycastHits);
            Debug.Log(arRaycastHits.Count + " objects hit.");
            for (int i = 0; i < arRaycastHits.Count; i++)
            {
                var raycastHit = arRaycastHits[i];
                var augmentedObject = Instantiate(raycastHitGameObject, raycastHit.sessionRelativePose.position, raycastHit.sessionRelativePose.rotation);
                augmentedObjects.Add(augmentedObject);
            }
        }

        if (arSession != null && GUI.Button(new Rect(100, 290, 100, 60), "clear"))
        {
            for (int i = 0; i < augmentedObjects.Count; i++)
            {
                Destroy(augmentedObjects[i]);
            }
            augmentedObjects.Clear();
        }
    }

    private List<GameObject> augmentedObjects = new List<GameObject>();
}