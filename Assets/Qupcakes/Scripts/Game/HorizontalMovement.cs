using System;
using UnityEngine;

namespace Qupcakery
{
    public enum Direction
    {
        Left = -1, Right = 1
    }

    public class HorizontalMovement
    {
        private float speed;

        public HorizontalMovement(float initialSpeed)
        {
            speed = initialSpeed;
        }

        // Calculate new position based on elapsed time and speed
        public Vector2 UpdatePosition(Vector2 currPosition,
            float deltaTime, Direction direction)
        {
            float dx = ((int)direction) * deltaTime * speed;
            return new Vector2(currPosition.x + dx, currPosition.y);
        }
    }
}
