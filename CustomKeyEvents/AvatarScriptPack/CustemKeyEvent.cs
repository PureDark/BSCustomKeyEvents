using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

namespace AvatarScriptPack
{
    public class CustomKeyEvent : MonoBehaviour
    {
        public enum ButtonEventType
        {
            Click,
            DoubleClick,
            LongClick,
            Press,
            Hold,
            Release,
            ReleaseAfterLongClick
        }

        public enum ViveButton
        {
            RightMenu = KeyCode.JoystickButton0,
            LeftMenu = KeyCode.JoystickButton2,
            LeftTrackpadPress = KeyCode.JoystickButton8,
            RightTrackpadPress = KeyCode.JoystickButton9,
            LeftTrigger = KeyCode.JoystickButton14,
            RightTrigger = KeyCode.JoystickButton15,
            LeftTrackpadTouch = KeyCode.JoystickButton16,
            RightTrackpadTouch = KeyCode.JoystickButton17,
            Space = KeyCode.Space,
            None = KeyCode.None
        }

        public enum OculusButton
        {
            A = KeyCode.JoystickButton0,
            B = KeyCode.JoystickButton1,
            X = KeyCode.JoystickButton2,
            Y = KeyCode.JoystickButton3,
            Start = KeyCode.JoystickButton7,
            LeftThumbstickPress = KeyCode.JoystickButton8,
            RightThumbstickPress = KeyCode.JoystickButton9,
            LeftTrigger = KeyCode.JoystickButton14,
            RightTrigger = KeyCode.JoystickButton15,
            LeftThumbstickTouch = KeyCode.JoystickButton16,
            RightThumbstickTouch = KeyCode.JoystickButton17,
            LeftThumbRestTouch = KeyCode.JoystickButton18,
            RightThumbRestTouch = KeyCode.JoystickButton19,
            Space = KeyCode.Space,
            None = KeyCode.None
        }

        public enum WMRButton
        {
            LeftMenu = KeyCode.JoystickButton6,
            RightMenu = KeyCode.JoystickButton7,
            LeftThumbstickPress = KeyCode.JoystickButton8,
            RightThumbstickPress = KeyCode.JoystickButton9,
            LeftTrigger = KeyCode.JoystickButton14,
            RightTrigger = KeyCode.JoystickButton15,
            LeftTouchpadPress = KeyCode.JoystickButton16,
            RightTouchpadPress = KeyCode.JoystickButton17,
            LeftTouchpadTouch = KeyCode.JoystickButton18,
            RightTouchpadTouch = KeyCode.JoystickButton19,
            Space = KeyCode.Space,
            None = KeyCode.None
        }

        [Tooltip("Button to trigger the animation.")]
        public ViveButton ViveTriggerButton = ViveButton.None;

        [Tooltip("Button to trigger the animation.")]
        public OculusButton OculusTriggerButton = OculusButton.None;

        [Tooltip("Button to trigger the animation.")]
        public WMRButton WMRTriggerButton = WMRButton.None;

        [Space(20)]

        [Tooltip("Called when the click event is triggered.")]
        public UnityEvent clickEvents = new UnityEvent();

        [Tooltip("Called when the double click event is triggered.")]
        public UnityEvent doubleClickEvents = new UnityEvent();

        [Tooltip("Called when the long click event is triggered.")]
        public UnityEvent longClickEvents = new UnityEvent();

        [Tooltip("Called when the press event is triggered.")]
        public UnityEvent pressEvents = new UnityEvent();

        [Tooltip("Called when the hold event is triggered.")]
        public UnityEvent holdEvents = new UnityEvent();

        [Tooltip("Called when the release event is triggered.")]
        public UnityEvent releaseEvents = new UnityEvent();

        [Tooltip("Called when the released after long click.")]
        public UnityEvent releaseAfterLongClickEvents = new UnityEvent();

        protected bool checkVive, checkOculus, checkWMR;
        protected const float interval = 0.5f;
        protected const float longClickInterval = 0.6f;
        protected float pressTime;
        protected float releaseTime;
        protected bool checkClick = false;
        protected bool checkDoubleClick = false;
        protected bool checkLongClick = false;
        protected bool triggerButtonDown = false;
        protected bool longClicked = false;


        // Use this for initialization
        void Start()
        {
            checkVive = true;
            checkOculus = (KeyCode)OculusTriggerButton != (KeyCode)ViveTriggerButton;
            checkWMR = (KeyCode)WMRTriggerButton != (KeyCode)ViveTriggerButton && (KeyCode)WMRTriggerButton != (KeyCode)OculusTriggerButton;
        }

        // Update is called once per frame
        void Update()
        {

            for(int i = 0; i < 3; i++)
            {
                KeyCode triggerButton = (i == 0 && checkVive)
                                        ? (KeyCode)ViveTriggerButton
                                        : (i == 1 && checkOculus)
                                        ? (KeyCode)OculusTriggerButton
                                        : (i == 2 && checkWMR)
                                        ? (KeyCode)WMRTriggerButton
                                        : KeyCode.None;
                if (triggerButton == KeyCode.None)
                    continue;
                bool allowCheck = true;
                if (triggerButton == (KeyCode)ViveButton.LeftTrigger || triggerButton == (KeyCode)ViveButton.RightTrigger)
                {
                    //float triggerValue = VRControllersInputManager.TriggerValue((triggerButton == ViveButton.LeftTrigger) ? XRNode.LeftHand : XRNode.RightHand);
                    string axisName = (triggerButton == (KeyCode)ViveButton.LeftTrigger) ? "TriggerLeftHand" : "TriggerRightHand";
                    try
                    {
                        float triggerValue = Input.GetAxis(axisName);
                        if (triggerValue < 0.1f)
                            allowCheck = false;
                    }
                    catch (Exception e)
                    {
                    }
                }

                if (allowCheck && Input.GetKeyDown(triggerButton))
                {
                    //Debug.Log(pressedKey + " is pressed");
                    checkDoubleClick = (Time.time - pressTime <= interval);
                    pressTime = Time.time;
                    OnPress();
                    checkLongClick = true;
                    checkClick = false;
                }
                else if (allowCheck && Input.GetKey(triggerButton))
                {
                    //Debug.Log(pressedKey + " is hold");
                    OnHold();
                    if (checkLongClick && Time.time - pressTime >= longClickInterval)
                    {
                        checkLongClick = false;
                        OnLongClick();
                        longClicked = true;
                    }
                }
                else if (Input.GetKeyUp(triggerButton))
                {
                    //Debug.Log(pressedKey + " is up");
                    releaseTime = Time.time;
                    OnRelease();
                    if (longClicked)
                    {
                        OnReleaseAfterLongClick();
                        longClicked = false;
                    }
                    //Debug.Log("GetKeyUp : releaseTime - pressTime = " + (releaseTime - pressTime));
                    if (releaseTime - pressTime <= interval)
                    {
                        if (checkDoubleClick)
                        {
                            OnDoubleClick();
                        }
                        else
                        {
                            checkClick = true;
                        }
                    }
                }
                else if (checkClick && Time.time - releaseTime > interval)
                {
                    checkClick = false;
                    OnClick();
                }
            }
            
        }

        void OnClick()
        {
            //Debug.Log("OnClick");
            clickEvents.Invoke();
        }

        void OnDoubleClick()
        {
            //Debug.Log("OnDoubleClick");
            doubleClickEvents.Invoke();
        }

        void OnLongClick()
        {
            //Debug.Log("OnLongClick");
            longClickEvents.Invoke();
        }

        void OnPress()
        {
            //Debug.Log("OnPress");
            pressEvents.Invoke();
        }

        void OnHold()
        {
            //Debug.Log("OnHold");
            holdEvents.Invoke();
        }

        void OnRelease()
        {
            //Debug.Log("OnRelease");
            releaseEvents.Invoke();
        }

        void OnReleaseAfterLongClick()
        {
            //Debug.Log("OnRelease");
            releaseAfterLongClickEvents.Invoke();
        }

    }
}

