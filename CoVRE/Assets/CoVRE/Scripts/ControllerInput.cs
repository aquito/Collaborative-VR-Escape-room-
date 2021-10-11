using Normal.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class ControllerInput : MonoBehaviour
{
    //public PrimaryButtonEvent leftPrimaryButtonPress, rightPrimaryButtonPress;

    public static event Action<bool> LeftSecondaryButtonPress, RightPrimaryButtonPress;

    private bool lastLeftSecondaryState, lastRightPrimaryState;
    private InputDevice leftController, rightController;
    private InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
    private InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

    // Start is called before the first frame update
    void Start()
    {
        TryGetControllers();
        InputDevices.deviceConnected += OnDeviceConnected;
        InputDevices.deviceDisconnected += OnDeviceDisconnected;        
    }

    // Update is called once per frame
    void Update()
    {
        // check for left secondary button press
        if (leftController != default)
        {
            bool tempLeftState = false;
            tempLeftState = leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool leftSecondaryValue) && leftSecondaryValue || tempLeftState;
            if (tempLeftState != lastLeftSecondaryState) // button state changed since last frame
            {
                LeftSecondaryButtonPress?.Invoke(tempLeftState);
                lastLeftSecondaryState = tempLeftState;
            }
        }

        // check for right primary button press
        if (rightController != default)
        {
            bool tempRightState = false;
            tempRightState = rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool rightPrimaryValue) && rightPrimaryValue || tempRightState;
            if (tempRightState != lastRightPrimaryState) // button state changed since last frame
            {
                RightPrimaryButtonPress?.Invoke(tempRightState);
                lastRightPrimaryState = tempRightState;
            }
        }
    }

    // Requests ownership of Normcore Realtime transforms on grab
    public void RequestOwnership(XRBaseInteractor baseInteractor)
    {
        RealtimeTransform realtimeTransform = baseInteractor.selectTarget.gameObject.GetComponent<RealtimeTransform>();
        if (realtimeTransform != null)
        {
  
            realtimeTransform.RequestOwnership();
        }
    }



    private void OnDeviceConnected(InputDevice inputDevice)
    {
        if((inputDevice.characteristics & leftControllerCharacteristics) == leftControllerCharacteristics)
        {
            leftController = inputDevice;
        }
        else if ((inputDevice.characteristics & rightControllerCharacteristics) == rightControllerCharacteristics)
        {
            rightController = inputDevice;
        }
    }

    private void OnDeviceDisconnected(InputDevice inputDevice)
    {
        if (leftController == inputDevice) leftController = default;
        else if (rightController == inputDevice) rightController = default;
    }

    private void TryGetControllers(InputDevice inputDevice = default)
    {
        TryGetController(leftControllerCharacteristics, out leftController);
        TryGetController(rightControllerCharacteristics, out rightController);
    }

    // Attempts to return the first controller that matches the given InputDeviceCharacteristics
    private bool TryGetController(InputDeviceCharacteristics characteristics, out InputDevice controller)
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(characteristics, devices);
        if(devices.Count > 0)
        {
            controller = devices[0];
            return true;
        }
        else
        {
            Debug.Log("No controller found");
            controller = default;
            return false;
        }
    }
}
