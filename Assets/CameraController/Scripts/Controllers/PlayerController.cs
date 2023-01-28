using ССP.Tools.Constants;
using ССP.Tools.Loggers;
using System;
using UnityEngine;
using System.ComponentModel;

namespace ССP.Controllers
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        private const bool IsRelativeToCamera = true;
        private const float Error = 0.01f;
        private const float MaxMovementSpeed = 30;
        private const float MaxJumpHeight = 500;

        private Transform _cameraTransform;
        private Rigidbody _playerRigidbody;
        private float _movingSpeed;
        private float _jumpingHeight;
        private bool _isForwardKeyPressed;
        private bool _isBackKeyPressed;
        private bool _isLeftKeyPressed;
        private bool _isRightKeyPressed;
        private bool _isJumpKeyPressed;
        
        [Range(Limits.MinSliderValue, Limits.MaxSliderValue)] 
        [SerializeField] private float _movementSpeed;
        [Range(Limits.MinSliderValue, Limits.MaxSliderValue)]
        [SerializeField] private float _jumpHeight;
        // Control keys
        [SerializeField] private KeyCode _forwardKey;
        [SerializeField] private KeyCode _leftKey; 
        [SerializeField] private KeyCode _backKey; 
        [SerializeField] private KeyCode _rightKey; 
        [SerializeField] private KeyCode _jumpKey;
        // Physics or kinematic
        [SerializeField] private bool _isKinematic;

        private void Start()
        {
            // Definition of player Rigidbody component for physical control
            _cameraTransform = Camera.main.transform;
            _playerRigidbody = GetComponent<Rigidbody>();
        }

        // Fixed framerate update for Rigidbody component of player
        private void FixedUpdate()
        {
            if (!_isKinematic && Mathf.Abs(_playerRigidbody.velocity.y) <= Error)
            {
                _movingSpeed = MaxMovementSpeed * _movementSpeed / Limits.MaxSliderValue;

                if (_isJumpKeyPressed)
                {
                    Jump();
                }

                if (_isForwardKeyPressed)
                {
                    MoveStraight();
                }
                else if (_isBackKeyPressed)
                {
                    MoveStraight(-1);
                }

                if (_isRightKeyPressed)
                {
                    MoveToSide();
                }
                else if (_isLeftKeyPressed)
                {
                    MoveToSide(-1);
                }
            }
        }

        // Check of movement direction according to pressed keys
        private void Update()
        {
            ResetKeysStates();

            if (MouseController.IsKeyPressed(_forwardKey))
            {
                _isForwardKeyPressed = true;
            }

            if (MouseController.IsKeyPressed(_backKey))  
            {
                _isBackKeyPressed = true;
            }

            if (MouseController.IsKeyPressed(_leftKey))
            {
                _isLeftKeyPressed = true;
            }

            if (MouseController.IsKeyPressed(_rightKey))
            {
                _isRightKeyPressed = true;
            }

            if (MouseController.IsKeyPressed(_jumpKey))
            {
                _isJumpKeyPressed = true;
            }

            UpdateKinematics();

            if (_isKinematic)
            {
                _movingSpeed = MaxMovementSpeed * _movementSpeed / Limits.MaxSliderValue;

                if (_isForwardKeyPressed)
                {
                    MoveStraight();
                }
                else if (_isBackKeyPressed)
                {
                    MoveStraight(-1);
                }
                if (_isRightKeyPressed)
                {
                    MoveToSide();
                }
                else if (_isLeftKeyPressed)
                {
                    MoveToSide(-1);
                }
            }
        }

        // Back and Forward movement according to direction parameter - sign: {1, -1}
        private void MoveStraight(int sign = 1)
        {
            Vector3 forwardDirection;
            if (IsRelativeToCamera)
            {
                forwardDirection = _cameraTransform.forward.normalized;
            }
            else
            {
                forwardDirection = transform.forward.normalized;
            }

            forwardDirection.y = 0;
            var deltaPosition = forwardDirection * Time.fixedDeltaTime * _movingSpeed * sign;

            if (_isKinematic)
            {
                transform.position += deltaPosition;
            }
            else
            {
                _playerRigidbody.MovePosition(_playerRigidbody.position + deltaPosition);
            }
        }

        // Left and Right movement accoring to direction parameter - sign: {1, -1}
        private void MoveToSide(int sign = 1)
        {
            Vector3 rightDirection;
            if (IsRelativeToCamera)
            {
                rightDirection = _cameraTransform.right.normalized;
            }
            else
            {
                rightDirection = transform.right.normalized;
            }

            rightDirection.y = 0;
            var deltaPosition = rightDirection * Time.fixedDeltaTime * _movingSpeed * sign;

            if (_isKinematic)
                transform.position += deltaPosition;
            else
                _playerRigidbody.MovePosition(_playerRigidbody.position + deltaPosition);
        }

        // Jumping if physics is enabled
        private void Jump()
        {
            _jumpingHeight = MaxJumpHeight * _jumpHeight / Limits.MaxSliderValue;

            _playerRigidbody.AddForce(Vector3.up * _jumpingHeight);
            _isJumpKeyPressed = false;
        }

        // Set of control keys unpressed
        private void ResetKeysStates()
        {
            _isForwardKeyPressed = false; 
            _isBackKeyPressed = false; 
            _isLeftKeyPressed = false; 
            _isRightKeyPressed = false; 
            _isJumpKeyPressed = false;
        }
        
        // Set of the player kinematics according to his settings
        private void UpdateKinematics()
        {
            if (_isKinematic)
            {
                _playerRigidbody.isKinematic = true;
            }
            else
            {
                _playerRigidbody.isKinematic = false;
            }
        }
    }
}