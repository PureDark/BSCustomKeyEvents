using DynamicOpenVR.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using Valve.VR;

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

        public enum IndexButton
        {
            LeftInnerFace = KeyCode.JoystickButton2,
            RightInnerFace = KeyCode.JoystickButton0,
            LeftOutterFace = KeyCode.JoystickButton3,
            RightOutterFace = KeyCode.JoystickButton1,
            LeftTrackpadPress = KeyCode.JoystickButton8,
            RightTrackpadPress = KeyCode.JoystickButton9,
            LeftTrackpadTouch = KeyCode.JoystickButton16,
            RightTrackpadTouch = KeyCode.JoystickButton17,
            LeftTrigger = KeyCode.JoystickButton14,
            RightTrigger = KeyCode.JoystickButton15,
            Space = KeyCode.Space,
            None = KeyCode.None
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
        public IndexButton IndexTriggerButton = IndexButton.None;

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

        private KeyCode triggerButton;
        
        protected const float interval = 0.5f;
        protected const float longClickInterval = 0.6f;
        protected float pressTime;
        protected float releaseTime;
        protected bool checkClick = false;
        protected bool checkDoubleClick = false;
        protected bool checkLongClick = false;
        protected bool longClicked = false;

        protected bool triggerPressed = false;


        private VectorInput _leftTriggerAction, _rightTriggerAction;
        private BooleanInput _leftAAction, _rightAAction;
        private BooleanInput _leftTrackpadAction, _rightTrackpadAction;


        // Use this for initialization
        void Start()
        {
            _leftTriggerAction = new VectorInput("/actions/main/in/lefttriggervalue");
            _rightTriggerAction = new VectorInput("/actions/main/in/righttriggervalue");
            _leftAAction = new BooleanInput("/actions/main/in/lefta");
            _rightAAction = new BooleanInput("/actions/main/in/righta");
            _leftTrackpadAction = new BooleanInput("/actions/main/in/lefttrackpadpress");
            _rightTrackpadAction = new BooleanInput("/actions/main/in/righttrackpadpress");

            string model = XRDevice.model != null ? XRDevice.model.ToLower() : "";
            

            if (model.Contains("index"))
            {
                triggerButton = (KeyCode)IndexTriggerButton;
            }
            else if (model.Contains("vive"))
            {
                triggerButton = (KeyCode)ViveTriggerButton;
            }
            else if (model.Contains("oculus"))
            {
                triggerButton = (KeyCode)OculusTriggerButton;
            }
            else
            {
                triggerButton = (KeyCode)WMRTriggerButton;
            }
            //Debug.Log("model: " + model);
        }

        // Update is called once per frame
        void Update()
        {
            if (triggerButton == KeyCode.None) {
                enabled = false;
                return;
            }

            BooleanInput buttonAction = null;

            if(triggerButton == (KeyCode)IndexButton.LeftInnerFace)
            {
                buttonAction = _leftAAction;
            }
            else if (triggerButton == (KeyCode)IndexButton.RightInnerFace)
            {
                buttonAction = _rightAAction;
            }
            else if (triggerButton == (KeyCode)IndexButton.LeftTrackpadPress)
            {
                buttonAction = _leftTrackpadAction;
            }
            else if (triggerButton == (KeyCode)IndexButton.RightTrackpadPress)
            {
                buttonAction = _rightTrackpadAction;
            }
            else
            {
                return;
            }

            if (triggerButton == (KeyCode)ViveButton.LeftTrigger || triggerButton == (KeyCode)ViveButton.RightTrigger)
            {
                var triggerAction = (triggerButton == (KeyCode)ViveButton.LeftTrigger) ? _leftTriggerAction : _rightTriggerAction;
                if (!triggerAction.isActive)
                    return;
                float triggerValue = triggerAction.value;
                Console.WriteLine("triggerValue : " + triggerValue);

                if (triggerValue > 0.5f)
                {
                    //GetKeyDown
                    if (!triggerPressed)
                    {
                        Console.WriteLine(triggerAction.name + " is down");
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
                        Console.WriteLine(triggerAction.name + " is longClicked");
                        checkLongClick = false;
                        OnLongClick();
                        longClicked = true;
                    }
                }
                if (triggerPressed && triggerValue < 0.1f)
                {
                    Console.WriteLine(triggerAction.name + " is up");
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
                if (checkClick && Time.time - releaseTime > interval)
                {
                    checkClick = false;
                    OnClick();
                }
            }
            else
            {
                if (buttonAction.activeChange)
                {
                    //Console.WriteLine(buttonAction.name + " is pressed");
                    checkDoubleClick = (Time.time - pressTime <= interval);
                    pressTime = Time.time;
                    OnPress();
                    checkLongClick = true;
                    checkClick = false;
                }
                if (buttonAction.state)
                {
                    //Console.WriteLine(buttonAction.name + " is hold");
                    OnHold();
                    if (checkLongClick && Time.time - pressTime >= longClickInterval)
                    {
                        //Console.WriteLine(buttonAction.name + " is longClicked");
                        checkLongClick = false;
                        OnLongClick();
                        longClicked = true;
                    }
                }
                if (buttonAction.inactiveChange)
                {
                    //Console.WriteLine(buttonAction.name + " is up");
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
                if (checkClick && Time.time - releaseTime > interval)
                {
                    checkClick = false;
                    OnClick();
                }
            }


        }

        void OnClick()
        {
            //Console.WriteLine("OnClick");
            clickEvents.Invoke();
        }

        void OnDoubleClick()
        {
            //Console.WriteLine("OnDoubleClick");
            doubleClickEvents.Invoke();
        }

        void OnLongClick()
        {
            //Console.WriteLine("OnLongClick");
            longClickEvents.Invoke();
        }

        void OnPress()
        {
            //Console.WriteLine("OnPress");
            pressEvents.Invoke();
        }

        void OnHold()
        {
            //Console.WriteLine("OnHold");
            holdEvents.Invoke();
        }

        void OnRelease()
        {
            //Console.WriteLine("OnRelease");
            releaseEvents.Invoke();
        }

        void OnReleaseAfterLongClick()
        {
            //Console.WriteLine("OnReleaseAfterLongClick");
            releaseAfterLongClickEvents.Invoke();
        }

    }
}

