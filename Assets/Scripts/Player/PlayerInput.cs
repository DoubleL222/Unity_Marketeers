using UnityEngine;
using System;
using System.Collections.Generic;

namespace Player
{
    public class PlayerInput : MonoBehaviour
    {
        public enum ButtonName
        {
            Button1,
            Button2,
            Button3,
            Button4,
            Button5,
            ZoomOut,
            Pause,
        }
        public enum ButtonState
        {
            None,
            Down,
            Held,
            Up
        }

        private class Button
        {
            public Button(ButtonName name, ButtonState state)
            {
                Name = name;
                State = state;
            }

            public ButtonName Name { get; private set; }
            public ButtonState State { get; set; }
        }

        [Range(1, 4)]
        public int playerNumber = 1;
        private string _movementHorizontalAxis, _movementVerticalAxis, _cameraHorizontalAxis, _cameraVerticalAxis, _dPadXAxis, _dPadYAxis, _button1Axis, _button2Axis, _button3Axis, _button4Axis, _button5Axis, _zoomOutAxis, _pauseAxis;
        private Vector2 _movementThisFrame;
        public Vector2 MovementThisFrame { get { return _movementThisFrame; } private set{_movementThisFrame = value;} }
        private Vector2 _cameraMovementThisFrame;
        public Vector2 CameraMovementThisFrame { get { return _cameraMovementThisFrame; } private set { _cameraMovementThisFrame = value; } }
        private Vector2 _dpadMovementThisFrame;
        public Vector2 DpadMovementThisFrame { get { return _dpadMovementThisFrame; } private set { _dpadMovementThisFrame = value; } }
        private Dictionary<ButtonName, Button> ButtonData;

        public Action<Vector2>
            OnMovement,
            OnCameraMovement,
            OnDpadMovement;

        public Action<float>
            OnHorizontal,
            OnVertical,
            OnCameraHorizontal,
            OnCameraVertical,
            OnDpadX,
            OnDpadY;

        public Action
            OnButton1Down,
            OnButton1Held,
            OnButton1Up,
            OnButton2Down,
            OnButton2Held,
            OnButton2Up,
            OnButton3Down,
            OnButton3Held,
            OnButton3Up,
            OnButton4Down,
            OnButton4Held,
            OnButton4Up, 
            OnButton5Down,
            OnButton5Held,
            OnButton5Up,
            OnZoomOutDown,
            OnZoomOutHeld,
            OnZoomOutUp,
            OnPauseDown,
            OnPauseHeld,
            OnPauseUp;

        public bool IgnoreInput { get; set; }

        void Start()
        {
            ButtonData = new Dictionary<ButtonName, Button>
            {
                {ButtonName.Button1, new Button(ButtonName.Button1, ButtonState.None) },
                {ButtonName.Button2, new Button(ButtonName.Button2, ButtonState.None) },
                {ButtonName.Button3, new Button(ButtonName.Button3, ButtonState.None) },
                {ButtonName.Button4, new Button(ButtonName.Button4, ButtonState.None) },
                {ButtonName.Button5, new Button(ButtonName.Button5, ButtonState.None) },
                {ButtonName.ZoomOut, new Button(ButtonName.ZoomOut, ButtonState.None) },
                {ButtonName.Pause, new Button(ButtonName.Pause, ButtonState.None) }
            };

            // Set the controller-strings.
            _movementHorizontalAxis = string.Concat("P", playerNumber, "_MovementHorizontal");
            _movementVerticalAxis = string.Concat("P", playerNumber, "_MovementVertical");
            _cameraHorizontalAxis = string.Concat("P", playerNumber, "_CameraHorizontal");
            _cameraVerticalAxis = string.Concat("P", playerNumber, "_CameraVertical");
            _dPadXAxis = string.Concat("P", playerNumber, "_DpadX");
            _dPadYAxis = string.Concat("P", playerNumber, "_DpadY");
            _button1Axis = string.Concat("P", playerNumber, "_Button1");
            _button2Axis = string.Concat("P", playerNumber, "_Button2");
            _button3Axis = string.Concat("P", playerNumber, "_Button3");
            _button4Axis = string.Concat("P", playerNumber, "_Button4");
            _button5Axis = string.Concat("P", playerNumber, "_Button5");
            _zoomOutAxis = string.Concat("P", playerNumber, "_ZoomOut");
            _pauseAxis = string.Concat("P", playerNumber, "_Pause");

            _movementThisFrame = Vector2.zero;
            _cameraMovementThisFrame = Vector2.zero;
            _dpadMovementThisFrame = Vector2.zero;
        }

        void FixedUpdate()
        {
            if (IgnoreInput)
                return;
            
            _movementThisFrame.x = Input.GetAxis(_movementHorizontalAxis);
            _movementThisFrame.y = Input.GetAxis(_movementVerticalAxis);
            _cameraMovementThisFrame.x = Input.GetAxis(_cameraHorizontalAxis);
            _cameraMovementThisFrame.y = Input.GetAxis(_cameraVerticalAxis);
            _dpadMovementThisFrame.x = Input.GetAxis(_dPadXAxis);
            _dpadMovementThisFrame.y = Input.GetAxis(_dPadYAxis);

            // If either of the movement axes are used, invoke OnMovement
            if (OnMovement != null && _movementThisFrame != Vector2.zero)
                OnMovement.Invoke(_movementThisFrame);
            // If either of the camera movement axes are used, invoke OnMovement
            if (OnCameraMovement != null && _cameraMovementThisFrame != Vector2.zero)
                OnCameraMovement.Invoke(_cameraMovementThisFrame);
            // If either of the dpad movement axes are used, invoke OnDpadMovement
            if (OnDpadMovement != null && _dpadMovementThisFrame != Vector2.zero)
                OnDpadMovement.Invoke(_dpadMovementThisFrame);

            // If movement axis X is used, invoke OnHorizontal
            if (OnHorizontal != null && _movementThisFrame.x != 0)
                OnHorizontal.Invoke(_movementThisFrame.x);
            // If movement axis Y is used, invoke OnVertical
            if (OnVertical != null && _movementThisFrame.y != 0)
                OnVertical.Invoke(_movementThisFrame.y);
            // If camera movement axis X is used, invoke OnHorizontal
            if (OnCameraHorizontal != null && _cameraMovementThisFrame.x != 0)
                OnCameraHorizontal.Invoke(_cameraMovementThisFrame.x);
            // If camera movement axis Y is used, invoke OnVertical
            if (OnCameraVertical != null && _cameraMovementThisFrame.y != 0)
                OnCameraVertical.Invoke(_cameraMovementThisFrame.y);
            // If dpad movement axis X is used, invoke OnDpadX
            if (OnDpadX != null && _dpadMovementThisFrame.x != 0)
                OnDpadX.Invoke(_dpadMovementThisFrame.x);
            // If dpad movement axis Y is used, invoke OnDpadY
            if (OnDpadY != null && _dpadMovementThisFrame.y != 0)
                OnDpadY.Invoke(_dpadMovementThisFrame.y);

            // React to button presses.
            ReactToButtonState(ButtonName.Button1, Input.GetAxis(_button1Axis), OnButton1Down, OnButton1Held, OnButton1Up);
            ReactToButtonState(ButtonName.Button2, Input.GetAxis(_button2Axis), OnButton2Down, OnButton2Held, OnButton2Up);
            ReactToButtonState(ButtonName.Button3, Input.GetAxis(_button3Axis), OnButton3Down, OnButton3Held, OnButton3Up);
            ReactToButtonState(ButtonName.Button4, Input.GetAxis(_button4Axis), OnButton4Down, OnButton4Held, OnButton4Up);
            ReactToButtonState(ButtonName.Button5, Input.GetAxis(_button5Axis), OnButton5Down, OnButton5Held, OnButton5Up);
            ReactToButtonState(ButtonName.ZoomOut, Input.GetAxis(_zoomOutAxis), OnZoomOutDown, OnZoomOutHeld, OnZoomOutUp);
            ReactToButtonState(ButtonName.Pause, Input.GetAxis(_pauseAxis), OnPauseDown, OnPauseHeld, OnPauseUp);

            // DEBUGGING
            //if (Input.GetAxis(_horizontalAxis) != 0)
            //    Debug.Log(_horizontalAxis + " = " + Input.GetAxis(_horizontalAxis));
            //if (Input.GetAxis(_verticalAxis) != 0)
            //    Debug.Log(_verticalAxis + " = " + Input.GetAxis(_verticalAxis));
            //if (Input.GetAxis(_cameraHorizontalAxis) != 0)
            //    Debug.Log(_cameraHorizontalAxis + " = " + Input.GetAxis(_cameraHorizontalAxis));
            //if (Input.GetAxis(_cameraVerticalAxis) != 0)
            //    Debug.Log(_cameraVerticalAxis + " = " + Input.GetAxis(_cameraVerticalAxis));
            //if (Input.GetAxis(_dPadXAxis) != 0)
            //    Debug.Log(_dPadXAxis + " = " + Input.GetAxis(_dPadXAxis));
            //if (Input.GetAxis(_dPadYAxis) != 0)
            //    Debug.Log(_dPadYAxis + " = " + Input.GetAxis(_dPadYAxis));
            //if (ButtonData[ButtonName.Button1].State != ButtonState.None)
            //    Debug.Log(_button1Axis + " state is " + ButtonData[ButtonName.Button1].State);
            //if (ButtonData[ButtonName.Button2].State != ButtonState.None)
            //    Debug.Log(_button2Axis + " state is " + ButtonData[ButtonName.Button2].State);
            //if (ButtonData[ButtonName.Button3].State != ButtonState.None)
            //    Debug.Log(_button3Axis + " state is " + ButtonData[ButtonName.Button3].State);
            //if (ButtonData[ButtonName.Button4].State != ButtonState.None)
            //    Debug.Log(_button4Axis + " state is " + ButtonData[ButtonName.Button4].State);
            //if (ButtonData[ButtonName.Pause].State != ButtonState.None)
            //    Debug.Log(_pauseAxis + " state is " + ButtonData[ButtonName.Pause].State);
            //if (ButtonData[ButtonName.ZoomOut].State != ButtonState.None)
            //    Debug.Log(_zoomOutAxis + " state is " + ButtonData[ButtonName.ZoomOut].State);
        }

        private void ReactToButtonState(ButtonName buttonName, float axisMeasurement, Action onDown, Action onHeld, Action onUp)
        {
            switch (ButtonData[buttonName].State)
            {
                case ButtonState.None:
                    if (axisMeasurement != 0)
                    {
                        ButtonData[buttonName].State = ButtonState.Down;
                        if(onDown != null) onDown.Invoke();
                    }
                    break;
                case ButtonState.Down:
                    if (axisMeasurement != 0)
                    {
                        ButtonData[buttonName].State = ButtonState.Held;
                        if (onHeld != null) onHeld.Invoke();
                    }
                    else
                    {
                        ButtonData[buttonName].State = ButtonState.Up;
                        if (onUp != null) onUp.Invoke();
                    }
                    break;
                case ButtonState.Held:
                    if (axisMeasurement != 0)
                    {
                        if (onHeld != null) onHeld.Invoke();
                    }
                    else
                    {
                        ButtonData[buttonName].State = ButtonState.Up;
                        if (onUp != null) onUp.Invoke();
                    }
                    break;
                case ButtonState.Up:
                    if (axisMeasurement != 0)
                    {
                        ButtonData[buttonName].State = ButtonState.Down;
                        if (onDown != null) onDown.Invoke();
                    }
                    else
                        ButtonData[buttonName].State = ButtonState.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}