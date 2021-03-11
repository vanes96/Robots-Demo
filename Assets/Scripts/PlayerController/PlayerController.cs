using System;
using UnityEngine;

namespace Invector.vCharacterController
{
    public class PlayerController : PlayerAnimator
    {
        private float lastShotTime = 0;
        private bool lastShotWasLeft = false;       
        
        public virtual void ControlAnimatorRootMotion()
        {
            if (!this.enabled) return;

            if (inputSmooth == Vector3.zero)
            {
                transform.position = animator.rootPosition;
                transform.rotation = animator.rootRotation;
            }
        }

        public virtual void ControlLocomotionType()
        {
            if (lockMovement) return;

            if (true)
            {
                SetControllerMoveSpeed(freeSpeed);
                SetAnimatorMoveSpeed(freeSpeed);
            }

            MoveCharacter(moveDirection);
            

            if (Input.GetKey(KeyCode.Mouse0) && lastShotTime > ShotDuration)
            {         
                lastShotTime = 0;
                var bullet = Instantiate(BulletPrefab);
                var bulletStartPlace = LeftBulletStartPlace;

                if (lastShotWasLeft)
                {
                    bulletStartPlace = RightBulletStartPlace;
                }

                bullet.transform.SetPositionAndRotation(bulletStartPlace.position, bulletStartPlace.rotation);
                bullet.GetComponent<Rigidbody>().AddForce(bulletStartPlace.up * ShotForce);
                lastShotWasLeft = !lastShotWasLeft;

            }
            else
            {
                lastShotTime += Time.deltaTime;
            }
        }

        public virtual void ControlRotationType()
        {
            if (lockRotation) return;

            Camera camera = Camera.main;
            Plane plane = new Plane(transform.up, -transform.position.y);
            Vector3 mousePosition = Input.mousePosition;
            Vector3 mouseWorldPosition = Vector3.zero;

            float distance;
            Ray ray = camera.ScreenPointToRay(mousePosition);
            if (plane.Raycast(ray, out distance))
            {
                mouseWorldPosition = ray.GetPoint(distance);
            }

            var direction = mouseWorldPosition - transform.position;
            direction.Normalize();
            float rotationY = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(0, 90 - rotationY, 0);    
        }

        public virtual void UpdateMoveDirection(Transform referenceTransform = null)
        {
            if (input.magnitude <= 0.01)
            {
                moveDirection = Vector3.Lerp(moveDirection, Vector3.zero, freeSpeed.movementSmooth * Time.deltaTime);
                return;
            }

            if (referenceTransform)
            {
                var right = referenceTransform.right;
                right.y = 0;

                var forward = Quaternion.AngleAxis(-90, Vector3.up) * right;
                // determine the direction the player will face based on input and the referenceTransform's right and forward directions
                moveDirection = (inputSmooth.x * right) + (inputSmooth.z * forward);
            }
            else
            {
                moveDirection = new Vector3(inputSmooth.x, 0, inputSmooth.z);
            }
        }

        public virtual void Sprint(bool value)
        {
            var sprintConditions = (input.sqrMagnitude > 0.1f && isGrounded && !(horizontalSpeed >= 0.5 || horizontalSpeed <= -0.5 || verticalSpeed <= 0.1f));

            if (value && sprintConditions)
            {
                if (input.sqrMagnitude > 0.1f)
                {
                    if (!isSprinting)
                    {
                        isSprinting = true;
                    }
                }
                else if (isSprinting)
                {
                    isSprinting = false;
                }
            }
            else if (isSprinting)
            {
                isSprinting = false;
            }
        }

        public virtual void Jump()
        {
            // trigger jump behaviour
            jumpCounter = jumpDuration;
            isJumping = true;

            // trigger jump animations
            if (input.sqrMagnitude < 0.1f)
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
                animator.CrossFadeInFixedTime("JumpMove", .2f);
        }
    }
}