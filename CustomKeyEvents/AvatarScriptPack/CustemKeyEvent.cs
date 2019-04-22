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

        [Tooltip("Button to trigger the events.")]
        public ViveButton ViveTriggerButton = ViveButton.None;

        [Tooltip("Button to trigger the events.")]
        public OculusButton OculusTriggerButton = OculusButton.None;

        [Tooltip("Button to trigger the events.")]
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

        [Tooltip("Called when released after long click.")]
        public UnityEvent releaseAfterLongClickEvents = new UnityEvent();

        protected bool checkVive, checkOculus, checkWMR;
        protected const float interval = 0.5f;
        protected const float longClickInterval = 0.6f;
        protected float pressTime;
        protected float releaseTime;
        protected bool checkClick = false;
        protected bool checkDoubleClick = false;
        protected bool checkLongClick = false;
        protected bool longClicked = false;
        
        protected bool triggerPressed = false;


        // Use this for initialization
        void Start()
        {
            string model = XRDevice.model != null ? XRDevice.model.ToLower() : "";
            if (model.Contains("vive"))
            {
                checkVive = true;
                checkOculus = false;
                checkWMR = false;
            }
            else if (model.Contains("oculus"))
            {
                checkVive = false;
                checkOculus = true;
                checkWMR = false;
            }
            else
            {
                checkVive = false;
                checkOculus = false;
                checkWMR = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            KeyCode triggerButton = (checkVive)
                                           ? (KeyCode)ViveTriggerButton
                                           : (checkOculus)
                                           ? (KeyCode)OculusTriggerButton
                                           : (checkWMR)
                                           ? (KeyCode)WMRTriggerButton
                                           : KeyCode.None;
            if (triggerButton == KeyCode.None)
                return;
            float triggerValue;
            bool isTrigger = triggerButton == (KeyCode)ViveButton.LeftTrigger || triggerButton == (KeyCode)ViveButton.RightTrigger;
            if (isTrigger)
            {
                string axisName = (triggerButton == (KeyCode)ViveButton.LeftTrigger) ? "TriggerLeftHand" : "TriggerRightHand";
                triggerValue = Input.GetAxis(axisName);
                
                if (triggerValue > 0.5f)
                {
                    //GetKeyDown
                    if (!triggerPressed)
                    {
                        triggerPressed = true;
                        checkDoubleClick = (Time.time - pressTime <= interval);
                        pressTime = Time.time;
                        OnPress();
                        checkLongClick = true;
                        checkClick = false;
                    }
                    //GetKey
                    OnHold();
                    if (checkLongClick && Time.time - pressTime >= longClickInterval)
                    {
                        checkLongClick = false;
                        OnLongClick();
                        longClicked = true;
                    }
                }
                else if (triggerPressed && triggerValue < 0.1f)
                {
                    //GetKeyUp
                    triggerPressed = false;
                    releaseTime = Time.time;
                    OnRelease();
                    if (longClicked)
                    {
                        OnReleaseAfterLongClick();
                        longClicked = false;
                    }
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
            else
            {
                if (Input.GetKeyDown(triggerButton))
                {
                    //Debug.Log(pressedKey + " is pressed");
                    checkDoubleClick = (Time.time - pressTime <= interval);
                    pressTime = Time.time;
                    OnPress();
                    checkLongClick = true;
                    checkClick = false;
                }
                else if (Input.GetKey(triggerButton))
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

