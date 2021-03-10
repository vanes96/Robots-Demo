using UnityEngine;
using ССP.Tools.States;
using System.Collections.Generic;
using System.Linq;
using System;
using ССP.Tools.Loggers;
using ССP.Tools.Constants;
using ССP.Tools;

namespace ССP.Controllers
{
    [RequireComponent(typeof(Camera))]
    /// <summary>
    /// Class that execute all functions of camera
    /// </summary>
    [ExecuteInEditMode]
    public class CameraController : MonoBehaviour
    {
        #region Private Fields       
        #region Objects
        // Clone of camera for player rotation
        private GameObject _cameraClone, _cameraTarget;
        // Enable physics or Rigidbody is Kinematic
        private Rigidbody _playerRigidbody;
        private Transform _cameraTransform, _targetTransform ;      
        #endregion

        #region Positions
        private float _targetDistance, _targetRotation, _targetHeight;
        public float _absoluteTargetRotation; // test
        private float _currentDistance, _currentHeight, _absoluteCurrentRotation, _currentRotation;
        private Vector3 _currentCameraTargetPosition, _smoothCameraTargetPosition;
        // Valid limits of different values
        private float _minDistance, _maxDistance, _minHeight, _maxHeight;
        //public Vector3 _currentCameraPosition, _targetCameraPosition;
        #endregion

        #region Controlling
        private float _zoomingSpeed, _rotatingSpeed, _liftingSpeed;
        private float _zoomingSmooth, _rotatingSmooth, _liftingSmooth, _followingSmooth;
        private const CameraState Rotating = CameraState.Rotating, Lifting = CameraState.Lifting, Zooming = CameraState.Zooming, Following = CameraState.Following;
        public Vector3 _previousMouseDragPosition = Vector3.zero, _previousCameraTargetPosition = Vector3.zero;
        #endregion
        #endregion

        #region Public Fields
        #region Positions
        public Transform Player;
        public Vector3 PositionOffset = new Vector3(0, 0, 0);
        public float Distance, Height, Rotation;
        public float MinDistance, MaxDistance, MinHeight, MaxHeight;
        #endregion

        // Settings of camera control by scrolling and dragging
        #region Controlling
        // Key to rotate and lift camera
        public float ZoomingSpeed, RotatingSpeed, LiftingSpeed;
        public float ZoomingSmoothness, RotatingSmoothness, LiftingSmoothness;
        public float FollowingSmoothness;
        
        #endregion

        #region Options
        public KeyCode DragKey = KeyCode.Mouse1;
        public bool LookAtCamera = false;
        #endregion
        #endregion

        #region Methods
        #region Main
        // Definition of major variables once
        private void Start()
        {
            Time.timeScale = 1.0f;
            _cameraTransform = Camera.main.transform;
            int cameraSiblingIndex = _cameraTransform.GetSiblingIndex();

            _cameraClone = GameObject.Find(Names.CameraClone);
            if (_cameraClone == null)
            {
                _cameraClone = new GameObject(Names.CameraClone);
            }
            //_cameraClone.transform.SetSiblingIndex(cameraSiblingIndex + 1);

            _cameraTarget = GameObject.Find(Names.CameraTarget);
            if (_cameraTarget == null)
            {
                _cameraTarget = new GameObject(Names.CameraTarget);
            }
            //_cameraTarget.transform.SetSiblingIndex(cameraSiblingIndex + 2);
            _targetTransform = _cameraTarget.transform;

            //if (Player == null)
            Player = GameObject.FindGameObjectWithTag(Names.Player).transform;
            //if (_playerRigidbody == null)
            _playerRigidbody = Player.gameObject.GetComponent<Rigidbody>();

            // ===========================================================================================

            ConvertSliderValuesToFields();

            var targetCameraRotation = Quaternion.Euler(_targetHeight, _targetRotation, 0);

            _currentRotation = _targetRotation;
            _currentHeight = _targetHeight;                        
            _currentDistance = _targetDistance;           
            _currentCameraTargetPosition = _targetTransform.position;

            _smoothCameraTargetPosition = _currentCameraTargetPosition;
            _previousCameraTargetPosition = _currentCameraTargetPosition;

            _cameraTransform.rotation = targetCameraRotation;
            _cameraTransform.position = _currentCameraTargetPosition - targetCameraRotation * Vector3.forward * _targetDistance;
            _cameraTransform.LookAt(_targetTransform);
        }

        // Updates that called every frame and keep all variables in actual state
        // Fixed framerate update for Rigidbody component of player
        private void FixedUpdate()
        {
            if (LookAtCamera)
            {
                RotatePlayer(_playerRigidbody.isKinematic);
            }
        }

        // Сamera movement according to pressed key
        private void Update()
        {
            float absoluteTargetRotation, targetHeight, targetDistance, targetRotation;
            float deltaTime = Time.deltaTime;
            float mouseScroll = MouseController.GetNormalizedScroll(deltaTime); //Logger_.Instance.Log("mouseScroll", mouseScroll); // * deltaTime 
            bool isMouseDragging = MouseController.IsKeyPressed(DragKey);
            var currentMouseDrag = Vector3.zero;

            _cameraTarget.transform.SetPositionAndRotation(Player.position + PositionOffset, Player.rotation);
            if (LookAtCamera)
            {
                RotatePlayer(_playerRigidbody.isKinematic);
            }

            ConvertSliderValuesToFields();
            absoluteTargetRotation = _absoluteTargetRotation; targetHeight = _targetHeight; 
            targetDistance = _targetDistance; targetRotation = _targetRotation;
            // =======================================================================================================================================
            _currentCameraTargetPosition += _targetTransform.position - _previousCameraTargetPosition;

            if (isMouseDragging)
            {
                // * deltaTime 
                currentMouseDrag = MouseController.GetNormalizedDrag(_previousMouseDragPosition, deltaTime); //Logger_.Instance.Log("mouseDragDistance", currentMouseDrag);
                float deltaRotation = Converter.ConvertInitialDeltaToResult(currentMouseDrag.x, _rotatingSpeed); // * speed    
                absoluteTargetRotation += deltaRotation;
                targetRotation += deltaRotation;
                Fixer.FixAngle(ref targetRotation);

                targetHeight -= Converter.ConvertInitialDeltaToResult(currentMouseDrag.y, _liftingSpeed); // * speed
                targetHeight = Fixer.FixValue(targetHeight, _minHeight, _maxHeight);
            }

            if (mouseScroll != 0)
            {
                targetDistance += Converter.ConvertInitialDeltaToResult(-mouseScroll, _zoomingSpeed); // * speed
                targetDistance = Fixer.FixValue(targetDistance, _minDistance, _maxDistance);
            }
            // =======================================================================================================================================
            bool isSmooth = true;
            bool isRotating = Checker.IsDeltaEnough(absoluteTargetRotation - _absoluteCurrentRotation, Rotating),
                 isLifting = Checker.IsDeltaEnough(targetHeight - _currentHeight, Lifting),
                 isZooming = Checker.IsDeltaEnough(targetDistance - _currentDistance, Zooming),
                 isFollowing = Checker.IsDeltaEnough(Vector3.Distance(_smoothCameraTargetPosition, _currentCameraTargetPosition), Following),
                 isRotatingSmooth = Checker.IsFunctionEnabled(_rotatingSmooth, isSmooth),
                 isLiftingSmooth = Checker.IsFunctionEnabled(_liftingSmooth, isSmooth),
                 isZoomingSmooth = Checker.IsFunctionEnabled(_zoomingSmooth, isSmooth),
                 isFollowingSmooth = Checker.IsFunctionEnabled(_followingSmooth, isSmooth);

            if (isFollowing)
            {
                FollowPlayer(ref _smoothCameraTargetPosition, _currentCameraTargetPosition, isFollowingSmooth, deltaTime);
            }

            if (isZooming)
            {
                _targetDistance = targetDistance;
                
                Zoom(ref _currentDistance, _targetDistance, isZoomingSmooth, deltaTime);
            }

            if (isLifting)
            {
                _targetHeight = targetHeight;

                Lift(_smoothCameraTargetPosition, ref _currentHeight, _targetHeight, isLiftingSmooth, deltaTime);
            }

            if (isRotating)
            {
                _absoluteTargetRotation = absoluteTargetRotation;
                _targetRotation = targetRotation;

                Rotate(_smoothCameraTargetPosition, ref _currentRotation, ref _absoluteCurrentRotation, _absoluteTargetRotation, isRotatingSmooth, deltaTime);
            }
            // =======================================================================================================================================
            ConvertFieldsToSliderValues();
            //Logger_.Instance.Log("MousePosition.x", MouseController.Position.x);
            _previousMouseDragPosition = MouseController.GetPreviousDragPosition(currentMouseDrag, isMouseDragging);

            if (_previousCameraTargetPosition != _targetTransform.position)
            {
                _previousCameraTargetPosition = _targetTransform.position;
            }
        }

        private void LateUpdate()
        {}
        #endregion

        // Movement of camera in 3 directions: back-forward, up-down, around     
        #region Camera Controlling
        private void FollowPlayer(ref Vector3 smoothPlayerPosition, Vector3 currentPlayerPosition, bool smooth, float deltaTime)
        {
            //Logger_.Instance.Log("Following");
            var deltaPosition = currentPlayerPosition - smoothPlayerPosition;
            deltaPosition = smooth ? Vector3.Lerp(Vector3.zero, deltaPosition, _followingSmooth * deltaTime) : deltaPosition;

            smoothPlayerPosition += deltaPosition;
            //currentCameraPosition += deltaPosition;
            _cameraTransform.position += deltaPosition;
        }

        private void Rotate(Vector3 smoothPlayerPosition, ref float currentRotation, ref float absoluteCurrentRotation, float absoluteTargetRotation, bool smooth, float deltaTime)
        {
            //Logger_.Instance.Log("Rotating");
            float absoluteDeltaRotation = absoluteTargetRotation - absoluteCurrentRotation;
            float rotatingSign = Mathf.Sign(absoluteDeltaRotation);
            float smoothDeltaRotation = smooth ? Mathf.Lerp(0, absoluteDeltaRotation, _rotatingSmooth * deltaTime) : absoluteDeltaRotation;
            float singleTurnAngle = Angles.Turn * rotatingSign;
            int turnsNumber = Mathf.FloorToInt(smoothDeltaRotation / singleTurnAngle);
            float angleRemainder = smoothDeltaRotation % singleTurnAngle;

            for (int i = 0; i < turnsNumber; i++)
            {
                absoluteCurrentRotation += singleTurnAngle;
                currentRotation += singleTurnAngle;
                Fixer.FixAngle(ref currentRotation);
                _cameraTransform.RotateAround(smoothPlayerPosition, Vector3.up, singleTurnAngle);
            }
         
            absoluteCurrentRotation += angleRemainder;
            currentRotation += angleRemainder;
            Fixer.FixAngle(ref currentRotation);
            _cameraTransform.RotateAround(smoothPlayerPosition, Vector3.up, angleRemainder);
        }

        private void Lift(Vector3 smoothPlayerPosition, ref float currentHeight, float targetHeight, bool smooth, float deltaTime)
        {
            //Logger_.Instance.Log("Lifting");

            float deltaHeight = targetHeight - currentHeight;
            deltaHeight = smooth ? Mathf.LerpAngle(0, deltaHeight, _liftingSmooth * deltaTime) : deltaHeight;
                                
            currentHeight += deltaHeight;
            _cameraTransform.RotateAround(smoothPlayerPosition, _cameraTransform.right, deltaHeight);
        }

        private void Zoom(ref float currentDistance, float targetDistance, bool smooth, float deltaTime)
        {
            //Logger_.Instance.Log("Zooming");

            float deltaDistance = targetDistance - currentDistance;
            deltaDistance = smooth ? Mathf.Lerp(0, deltaDistance, _zoomingSmooth * deltaTime) : deltaDistance;

            currentDistance += deltaDistance;
            _cameraTransform.position -= _cameraTransform.rotation * Vector3.forward * deltaDistance;
        }
        #endregion

        #region Values Convertions
        // Update and conversion of float fields according to int values in editor sliders
        private void ConvertSliderValuesToFields()
        {
            _minDistance = Converter.ConvertSliderValueToField(MinDistance, Limits.MaxLowerDistance, Limits.MinLowerDistance);
            _maxDistance = Converter.ConvertSliderValueToField(MaxDistance, Limits.MaxUpperDistance, Limits.MinUpperDistance);
            _minHeight = Converter.ConvertSliderValueToField(MinHeight, Limits.MaxLowerHeight, Limits.MinLowerHeight);
            _maxHeight = Converter.ConvertSliderValueToField(MaxHeight, Limits.MaxUpperHeight, Limits.MinUpperHeight);

            _targetDistance = Converter.ConvertSliderValueToField(Distance, _maxDistance, _minDistance);
            _targetHeight = Converter.ConvertSliderValueToField(Height, _maxHeight, _minHeight);
            //ValuesProcessor.ConvertSliderValueToField(ref _rotation, Rotation, MaxRotation);
            _absoluteTargetRotation = Rotation;
            //_targetRotation = Rotation;

            _zoomingSpeed = Converter.ConvertSliderValueToField(ZoomingSpeed, Limits.MaxScrollingSpeed);
            _rotatingSpeed = Converter.ConvertSliderValueToField(RotatingSpeed, Limits.MaxRotatingSpeed);
            _liftingSpeed = Converter.ConvertSliderValueToField(LiftingSpeed, Limits.MaxLiftingSpeed);
            _followingSmooth = Converter.ConvertSliderValueToField(FollowingSmoothness, Limits.MaxSmoothness, Limits.MinSmoothing);

            _zoomingSmooth = Converter.ConvertSliderValueToField(ZoomingSmoothness, Limits.MaxSmoothness, Limits.MinSmoothing);
            _rotatingSmooth = Converter.ConvertSliderValueToField(RotatingSmoothness, Limits.MaxSmoothness, Limits.MinSmoothing);
            _liftingSmooth = Converter.ConvertSliderValueToField(LiftingSmoothness, Limits.MaxSmoothness, Limits.MinSmoothing);

            //_maxLiftingThreshold = Processor.ConvertByAspectRatio(MaxRotationThreshold);
        }

        // Update and conversion of int slider values (in editor) to float
        private void ConvertFieldsToSliderValues()
        {
            MinDistance = Converter.ConvertFieldToSliderValue(_minDistance, Limits.MaxLowerDistance, Limits.MinLowerDistance);
            MaxDistance = Converter.ConvertFieldToSliderValue(_maxDistance, Limits.MaxUpperDistance, Limits.MinUpperDistance);
            MinHeight = Converter.ConvertFieldToSliderValue(_minHeight, Limits.MaxLowerHeight, Limits.MinLowerHeight);
            MaxHeight = Converter.ConvertFieldToSliderValue(_maxHeight, Limits.MaxUpperHeight, Limits.MinUpperHeight);

            Distance = Converter.ConvertFieldToSliderValue(_targetDistance, _maxDistance, _minDistance);
            Height = Converter.ConvertFieldToSliderValue(_targetHeight, _maxHeight, _minHeight);
            //ValuesProcessor.ConvertFieldToSliderValue(ref Rotation, _rotation, MaxRotation);
            Rotation = _absoluteTargetRotation;
            //Rotation = _targetRotation;

            ZoomingSpeed = Converter.ConvertFieldToSliderValue(_zoomingSpeed, Limits.MaxScrollingSpeed);
            RotatingSpeed = Converter.ConvertFieldToSliderValue(_rotatingSpeed, Limits.MaxRotatingSpeed);
            LiftingSpeed = Converter.ConvertFieldToSliderValue(_liftingSpeed, Limits.MaxLiftingSpeed);
            FollowingSmoothness = Converter.ConvertFieldToSliderValue(_followingSmooth, Limits.MaxSmoothness, Limits.MinSmoothing);

            ZoomingSmoothness = Converter.ConvertFieldToSliderValue(_zoomingSmooth, Limits.MaxSmoothness, Limits.MinSmoothing);
            RotatingSmoothness = Converter.ConvertFieldToSliderValue(_rotatingSmooth, Limits.MaxSmoothness, Limits.MinSmoothing);
            LiftingSmoothness = Converter.ConvertFieldToSliderValue(_liftingSmooth, Limits.MaxSmoothness, Limits.MinSmoothing);
        }
        #endregion

        // Player rotation with the camera
        private void RotatePlayer(bool isKinematic)
        {
            _cameraClone.transform.SetPositionAndRotation(_cameraTransform.position, Quaternion.identity);
            _cameraClone.transform.LookAt(Player.transform);

            float playerAngleY, cameraAngleY = _cameraClone.transform.eulerAngles.y;
            float maxRotation = Angles.Turn;

            if (cameraAngleY >= 0 && cameraAngleY < maxRotation)
                playerAngleY = cameraAngleY + maxRotation;
            else
                playerAngleY = cameraAngleY - maxRotation;

            if (isKinematic)
                Player.rotation = Quaternion.Euler(0, playerAngleY, 0);
            else
                _playerRigidbody.MoveRotation(Quaternion.Euler(0, playerAngleY, 0));
        }


        #endregion
    };
}

//bool rotatingOnly = Mathf.Abs(ValuesProcessor.ConvertByAspectRatio(_currentMouseDrag.Item1)) >= Mathf.Abs(_currentMouseDrag.Item2);
//if (Mathf.Abs(absoluteDeltaRotation) > 20)
//    Logger_.Instance.Log("absoluteDeltaRotation !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!", absoluteDeltaRotation);

//_targetCameraPosition = _targetPlayerPosition - targetCameraRotation * Vector3.forward * _targetDistance;
//_currentCameraPosition = _targetCameraPosition;