
using UnityEngine;

namespace Robo.PlayerController
{
    public class PlayerAnimator : PlayerMotor
    {             
        public float WalkAnimationSpeed = 0.5f;
        public float SprintAnimationSpeed = 1f;

        public virtual void UpdateAnimator()
        {
            if (animator == null || !animator.enabled) return;

            animator.SetBool(AnimatorParameter.IsSprinting, isSprinting);
            animator.SetBool(AnimatorParameter.IsGrounded, isGrounded);
            animator.SetFloat(AnimatorParameter.GroundDistance, groundDistance);

            //animator.SetFloat(vAnimatorParameters.InputVertical, stopMove ? 0 : verticalSpeed, freeSpeed.animationSmooth, Time.deltaTime);
            animator.SetFloat(AnimatorParameter.InputMagnitude, stopMove ? 0f : inputMagnitude, freeSpeed.animationSmooth, Time.deltaTime);
        }

        public virtual void SetAnimatorMoveSpeed(PlayerSpeed speed)
        {
            Vector3 relativeInput = transform.InverseTransformDirection(moveDirection);
            verticalSpeed = relativeInput.z;
            horizontalSpeed = relativeInput.x;

            var newInput = new Vector2(verticalSpeed, horizontalSpeed);

            inputMagnitude = Mathf.Clamp(newInput.magnitude, 0, isSprinting ? SprintAnimationSpeed : WalkAnimationSpeed);
            //else
            //    inputMagnitude = Mathf.Clamp(isSprinting ? newInput.magnitude + 0.5f : newInput.magnitude, 0, isSprinting ? sprintSpeed : runningSpeed);
        }
    }

    public static partial class AnimatorParameter
    {
        public static int InputMagnitude = Animator.StringToHash("InputMagnitude");
        public static int IsGrounded = Animator.StringToHash("IsGrounded");
        public static int IsSprinting = Animator.StringToHash("IsSprinting");
        public static int GroundDistance = Animator.StringToHash("GroundDistance");
    }
}