using System;
using UnityEngine;

namespace BlackBox
{
    public class Ray
    {
        public Vector3Int position;
        public Dir direction;
        
        private bool directHit = false;
        private bool rayDetoured = false;

        private Vector3Int origin;
        private Dir originDirection;

        private Vector3Int destination;
        private Dir destDirection;

        private static int detourPairNumber = 1;
        private static int maxDetourPairNumber = 9;

        public Ray(Vector3Int origin, Dir dir, int width, int height)
        {
            direction = dir;
            originDirection = dir;
            this.origin = origin;

            switch (dir)
            {
                case Dir.Left:
                    position.y = origin.y;
                    break;
                case Dir.Bot:
                    position.x = origin.x;
                    break;
                case Dir.Right:
                    position.x = width - 1;
                    position.y = origin.y;
                    break;
                case Dir.Top:
                    position.y = height - 1;
                    position.x = origin.x;
                    break;
            }
        }

        public void AddMarkers()
        {
            SetDestination();

            if (originDirection == destDirection)
            {
                if (origin == destination)
                {
                    if (directHit)
                        BBEvents.MarkUnits?.Invoke(Marker.Hit, originDirection, origin, true);
                    else // only other case for returning to the same cell is a reflection
                        BBEvents.MarkUnits?.Invoke(Marker.Reflect, originDirection, origin, true);
                }
                else // different cell
                    BBEvents.MarkDetourUnits?.Invoke(originDirection, origin, destDirection, destination, GetDetourPairNumber());
            }
            else // diff entry/exit direction
            {
                if (rayDetoured)
                    BBEvents.MarkDetourUnits?.Invoke(originDirection, origin, destDirection, destination, GetDetourPairNumber());
                else // straight through
                {
                    BBEvents.MarkUnits?.Invoke(Marker.Miss, originDirection, origin, false);
                    BBEvents.MarkUnits?.Invoke(Marker.Miss, destDirection, destination, true);
                }
            }
        }

        private void SetDestination()
        {
            destination = Vector3Int.zero;

            switch (direction)
            {
                case Dir.Left:
                    destination.y = position.y;
                    destDirection = Dir.Right;
                    break;
                case Dir.Right:
                    destination.y = position.y;
                    destDirection = Dir.Left;
                    break;
                case Dir.Bot:
                    destination.x = position.x;
                    destDirection = Dir.Top;
                    break;
                case Dir.Top:
                    destination.x = position.x;
                    destDirection = Dir.Bot;
                    break;
            }
        }

        public void Forward()
        {
            switch (direction)
            {
                case Dir.Left: position.x++; break;
                case Dir.Bot: position.y++; break;
                case Dir.Right: position.x--; break;
                case Dir.Top: position.y--; break;
            }
        }

        public void Flip()
        {
            switch(direction)
            {
                case Dir.Left: direction = Dir.Right; break;
                case Dir.Bot: direction = Dir.Top; break;
                case Dir.Right: direction = Dir.Left; break; 
                case Dir.Top: direction = Dir.Bot; break;
            }
        }

        public void TurnLeft()
        {
            switch (direction)
            {
                case Dir.Left: direction = Dir.Bot; break;
                case Dir.Bot: direction = Dir.Right; break;
                case Dir.Right: direction = Dir.Top; break;
                case Dir.Top: direction = Dir.Left; break;
            }

            rayDetoured = true;
        }

        public void TurnRight()
        {
            switch (direction)
            {
                case Dir.Left: direction = Dir.Top; break;
                case Dir.Bot: direction = Dir.Left; break;
                case Dir.Right: direction = Dir.Bot; break;
                case Dir.Top: direction = Dir.Right; break;
            }

            rayDetoured = true;
        }

        public void Kill(int gridLength) //todo: better fix
        {
            direction = originDirection;
            position = origin;
            directHit = true;

            switch(direction)
            {
                case Dir.Right: position.x = gridLength - 1; break;
                case Dir.Top: position.y = gridLength - 1; break;
                default: break;
            }

            Flip();
        }

        private int GetDetourPairNumber()
        {
            detourPairNumber++;

            if (detourPairNumber > maxDetourPairNumber) // reset
                detourPairNumber = 1;

            return detourPairNumber;
        }
    }
}
