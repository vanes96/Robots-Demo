using ССP.Tools.Constants;
using ССP.Tools;
using UnityEngine;

namespace ССP.Controllers
{
    public static class MouseController // TODO
    {
        private const float MaxHorizontalDrag = 1.5f, MaxVerticalDrag = 1.5f, MaxScroll = 0.001f;

        public static float Scroll
        {
            get
            {
                return Input.GetAxis(Names.ScrollAxis);
            }
        }

        public static Vector3 Position
        {
            get
            {
                return Input.mousePosition;
            }
        }

        public static bool IsKeyPressed(KeyCode key)
        {
            return Input.GetKey(key);
        }

        public static float GetNormalizedScroll(float deltaTime)
        {
            return Fixer.NormalizeValue(Scroll * deltaTime, MaxScroll);
        }

        public static Vector3 GetInitialDrag(Vector3 previousDragPosition)
        {
            var mousePosition = Input.mousePosition;
            bool isBegin = previousDragPosition == Vector3.zero;
            float horizontalDrag = mousePosition.x - (isBegin ? mousePosition.x : previousDragPosition.x),
                    verticalDrag = mousePosition.y - (isBegin ? mousePosition.y : previousDragPosition.y);

            return new Vector3(horizontalDrag, verticalDrag);
        }

        public static Vector3 GetNormalizedDrag(Vector3 previousDragPosition, float deltaTime)
        {
            var normalizedDrag = NormalizeDrag(GetInitialDrag(previousDragPosition), deltaTime);

            return normalizedDrag;
        }

        // Get previous mouse/touchpad drag position
        public static Vector3 GetPreviousDragPosition(Vector3 dragDistance, bool isMouseDragging)
        {
            if (dragDistance != Vector3.zero || isMouseDragging)
            {
                return Input.mousePosition;
            }
            else
            {
                return Vector3.zero;
            }
        }

        // Fix mouse drag if it is bigger than valid maximum limit
        private static Vector3 NormalizeDrag(Vector3 drag, float deltaTime)
        {
            float horizontalMax = MaxHorizontalDrag, verticalMax = MaxVerticalDrag;
            drag = new Vector3(drag.x * deltaTime, drag.y * deltaTime, drag.z * deltaTime);

            if (Mathf.Abs(drag.x) > horizontalMax)
            {
                drag.x = horizontalMax * Mathf.Sign(drag.x);
            }

            if (Mathf.Abs(drag.y) > verticalMax)
            {
                drag.y = verticalMax * Mathf.Sign(drag.y);
            }

            return drag;
        }
    }
}
