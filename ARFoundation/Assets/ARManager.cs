using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    [SerializeField] private ARSession arSession;

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
}