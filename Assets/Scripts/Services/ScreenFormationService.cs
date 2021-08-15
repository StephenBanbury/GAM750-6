using System.Collections.Generic;
using Assets.Scripts.Enums;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Services
{
    public class ScreenFormationService
    {
        private readonly float _xPos;
        private readonly float _yPos;
        private readonly float _zPos;

        public ScreenFormationService(Scene scene)
        {
            var sceneService = new SceneService(scene);
            ScenePosition = sceneService.GetScenePosition();

            _xPos = ScenePosition.x;
            _yPos = ScenePosition.y;
            _zPos = ScenePosition.z;
        }
        
        public Vector3 ScenePosition { set; get; }


        public List<ScreenPosition> LargeSquare()
        {
            List<ScreenPosition> formation = new List<ScreenPosition>
            {
                new ScreenPosition {Id = 1, Vector3 = new Vector3(1.2f + _xPos, 0 + _yPos, 3.32f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 2, Vector3 = new Vector3(2.73f + _xPos, 0 + _yPos, 3.32f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 3, Vector3 = new Vector3(3.51f + _xPos, 0 + _yPos, 2.55f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 4, Vector3 = new Vector3(3.51f + _xPos, 0 + _yPos, 1.01f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 5, Vector3 = new Vector3(3.51f + _xPos, 0 + _yPos, -0.52f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 6, Vector3 = new Vector3(3.51f + _xPos, 0 + _yPos, -2.05f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 7, Vector3 = new Vector3(2.73f + _xPos, 0 + _yPos, -2.81f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 8, Vector3 = new Vector3(1.2f + _xPos, 0 + _yPos, -2.81f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 9, Vector3 = new Vector3(-0.34f + _xPos, 0 + _yPos, -2.81f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 10, Vector3 = new Vector3(-1.87f + _xPos, 0 + _yPos, -2.81f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 11, Vector3 = new Vector3(-2.63f + _xPos, 0 + _yPos, -2.05f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 12, Vector3 = new Vector3(-2.63f + _xPos, 0 + _yPos, -0.52f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 13, Vector3 = new Vector3(-2.63f + _xPos, 0 + _yPos, 1.01f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 14, Vector3 = new Vector3(-2.63f + _xPos, 0 + _yPos, 2.55f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 15, Vector3 = new Vector3(-1.87f + _xPos, 0 + _yPos, 3.32f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 16, Vector3 = new Vector3(-0.34f + _xPos, 0 + _yPos, 3.32f + _zPos), Rotation = 0}
            };

            return formation;
        }
        public List<ScreenPosition> SmallSquare()
        {
            List<ScreenPosition> formation = new List<ScreenPosition>
            {
                new ScreenPosition {Id = 1, Vector3 = new Vector3(1.86f + _xPos, 0 + _yPos, 2.51f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 2, Vector3 = new Vector3(3.39f + _xPos, 0 + _yPos, 2.51f + _zPos), Rotation = 0, Hide = true},
                new ScreenPosition {Id = 3, Vector3 = new Vector3(2.62f + _xPos, 0 + _yPos, 1.74f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 4, Vector3 = new Vector3(2.62f + _xPos, 0 + _yPos, 0.2f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 5, Vector3 = new Vector3(2.62f + _xPos, 0 + _yPos, -1.33f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 6, Vector3 = new Vector3(4.17f + _xPos, 0 + _yPos, -2.86f + _zPos), Rotation = 90, Hide = true},
                new ScreenPosition {Id = 7, Vector3 = new Vector3(3.39f + _xPos, 0 + _yPos, -3.62f + _zPos), Rotation = 180, Hide = true},
                new ScreenPosition {Id = 8, Vector3 = new Vector3(1.86f + _xPos, 0 + _yPos, -2.09f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 9, Vector3 = new Vector3(0.32f + _xPos, 0 + _yPos, -2.09f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 10, Vector3 = new Vector3(-1.21f + _xPos, 0 + _yPos, -2.09f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 11, Vector3 = new Vector3(-1.97f + _xPos, 0 + _yPos, -2.86f + _zPos), Rotation = 270, Hide = true},
                new ScreenPosition {Id = 12, Vector3 = new Vector3(-1.97f + _xPos, 0 + _yPos, -1.33f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 13, Vector3 = new Vector3(-1.97f + _xPos, 0 + _yPos, 0.2f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 14, Vector3 = new Vector3(-1.97f + _xPos, 0 + _yPos, 1.74f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 15, Vector3 = new Vector3(-1.21f + _xPos, 0 + _yPos, 2.51f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 16, Vector3 = new Vector3(0.32f + _xPos, 0 + _yPos, 2.51f + _zPos), Rotation = 0}
            };

            return formation;
        }
        public List<ScreenPosition> Cross()
        {
            List<ScreenPosition> formation = new List<ScreenPosition>
            {
                new ScreenPosition {Id = 1, Vector3 = new Vector3(1.16f + _xPos, 0 + _yPos, 3.33f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 2, Vector3 = new Vector3(1.923f + _xPos, 0 + _yPos, 2.56f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 3, Vector3 = new Vector3(2.69f + _xPos, 0 + _yPos, 1.79f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 4, Vector3 = new Vector3(3.47f + _xPos, 0 + _yPos, 1.02f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 5, Vector3 = new Vector3(3.47f + _xPos, 0 + _yPos, -0.51f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 6, Vector3 = new Vector3(2.69f + _xPos, 0 + _yPos, -1.27f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 7, Vector3 = new Vector3(1.923f + _xPos, 0 + _yPos, -2.04f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 8, Vector3 = new Vector3(1.16f + _xPos, 0 + _yPos, -2.8f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 9, Vector3 = new Vector3(-0.38f + _xPos, 0 + _yPos, -2.8f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 10, Vector3 = new Vector3(-1.15f + _xPos, 0 + _yPos, -2.04f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 11, Vector3 = new Vector3(-1.91f + _xPos, 0 + _yPos, -1.27f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 12, Vector3 = new Vector3(-2.67f + _xPos, 0 + _yPos, -0.51f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 13, Vector3 = new Vector3(-2.67f + _xPos, 0 + _yPos, 1.02f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 14, Vector3 = new Vector3(-1.91f + _xPos, 0 + _yPos, 1.79f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 15, Vector3 = new Vector3(-1.15f + _xPos, 0 + _yPos, 2.56f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 16, Vector3 = new Vector3(-0.38f + _xPos, 0 + _yPos, 3.33f + _zPos), Rotation = 0}
            };

            return formation;
        }
        public List<ScreenPosition> SmallStar()
        {
            List<ScreenPosition> formation = new List<ScreenPosition>
            {
                new ScreenPosition {Id = 1, Vector3 = new Vector3(0.76f + _xPos, 0 + _yPos, 2.21f + _zPos), Rotation = 60},
                new ScreenPosition {Id = 2, Vector3 = new Vector3(1.91f + _xPos, 0 + _yPos, 1.55f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 3, Vector3 = new Vector3(2.29f + _xPos, 0 + _yPos, 0.88f + _zPos), Rotation = 120},
                new ScreenPosition {Id = 4, Vector3 = new Vector3(2.30f + _xPos, 0 + _yPos, -0.45f + _zPos), Rotation = 60},
                new ScreenPosition {Id = 5, Vector3 = new Vector3(1.91f + _xPos, 0 + _yPos, -1.11f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 6, Vector3 = new Vector3(0.76f + _xPos, 0 + _yPos, -1.77f + _zPos), Rotation = 120},
                new ScreenPosition {Id = 7, Vector3 = new Vector3(2.63f + _xPos, 0 + _yPos, -3.92f + _zPos), Rotation = 180, Hide = true},
                new ScreenPosition {Id = 8, Vector3 = new Vector3(1.10f + _xPos, 0 + _yPos, -3.92f + _zPos), Rotation = 180, Hide = true},
                new ScreenPosition {Id = 9, Vector3 = new Vector3(-0.44f + _xPos, 0 + _yPos, -3.92f + _zPos), Rotation = 180, Hide = true},
                new ScreenPosition {Id = 10, Vector3 = new Vector3(-1.97f + _xPos, 0 + _yPos, -3.92f + _zPos), Rotation = 180, Hide = true},
                new ScreenPosition {Id = 11, Vector3 = new Vector3(0.03f + _xPos, 0 + _yPos, -1.77f + _zPos), Rotation = 240},
                new ScreenPosition {Id = 12, Vector3 = new Vector3(-1.12f + _xPos, 0 + _yPos, -1.11f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 13, Vector3 = new Vector3(-1.5f + _xPos, 0 + _yPos, -0.45f + _zPos), Rotation = 300},
                new ScreenPosition {Id = 14, Vector3 = new Vector3(-1.49f + _xPos, 0 + _yPos, 0.88f + _zPos), Rotation = 240},
                new ScreenPosition {Id = 15, Vector3 = new Vector3(-1.12f + _xPos, 0 + _yPos, 1.55f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 16, Vector3 = new Vector3(0.03f + _xPos, 0 + _yPos, 2.21f + _zPos), Rotation = 300}
            };

            return formation;
        }
        public List<ScreenPosition> LargeStar()
        {
            List<ScreenPosition> formation = new List<ScreenPosition>
            {
                new ScreenPosition {Id = 1, Vector3 = new Vector3(0.75f + _xPos, 0 + _yPos, 2.50f + _zPos), Rotation = 45},
                new ScreenPosition {Id = 2, Vector3 = new Vector3(2.07f + _xPos, 0 + _yPos, 1.95f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 3, Vector3 = new Vector3(2.82f + _xPos, 0 + _yPos, 1.18f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 4, Vector3 = new Vector3(3.37f + _xPos, 0 + _yPos, -0.15f + _zPos), Rotation = 45},
                new ScreenPosition {Id = 5, Vector3 = new Vector3(3.38f + _xPos, 0 + _yPos, -1.25f + _zPos), Rotation = 135},
                new ScreenPosition {Id = 6, Vector3 = new Vector3(2.84f + _xPos, 0 + _yPos, -2.55f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 7, Vector3 = new Vector3(2.07f + _xPos, 0 + _yPos, -3.35f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 8, Vector3 = new Vector3(0.75f + _xPos, 0 + _yPos, -3.85f + _zPos), Rotation = 135},
                new ScreenPosition {Id = 9, Vector3 = new Vector3(-0.39f + _xPos, 0 + _yPos, -3.85f + _zPos), Rotation = 225},
                new ScreenPosition {Id = 10, Vector3 = new Vector3(-1.70f + _xPos, 0 + _yPos, -3.35f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 11, Vector3 = new Vector3(-2.46f + _xPos, 0 + _yPos, -2.55f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 12, Vector3 = new Vector3(-3.00f + _xPos, 0 + _yPos, -1.25f + _zPos), Rotation = 225},
                new ScreenPosition {Id = 13, Vector3 = new Vector3(-3.00f + _xPos, 0 + _yPos, -0.15f + _zPos), Rotation = 315},
                new ScreenPosition {Id = 14, Vector3 = new Vector3(-2.47f + _xPos, 0 + _yPos, 1.18f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 15, Vector3 = new Vector3(-1.69f + _xPos, 0 + _yPos, 1.95f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 16, Vector3 = new Vector3(-0.39f + _xPos, 0 + _yPos, 2.50f + _zPos), Rotation = 315}
            };

            return formation;
        }
        public List<ScreenPosition> Circle()
        {
            List<ScreenPosition> formation = new List<ScreenPosition>
            {
                new ScreenPosition {Id = 1, Vector3 = new Vector3(0.44f + _xPos, 0 + _yPos, 4.16f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 2, Vector3 = new Vector3(1.92f + _xPos, 0 + _yPos, 3.86f + _zPos), Rotation = 22.5f},
                new ScreenPosition {Id = 3, Vector3 = new Vector3(3.17f + _xPos, 0 + _yPos, 3.03f + _zPos), Rotation = 45},
                new ScreenPosition {Id = 4, Vector3 = new Vector3(4.0f + _xPos, 0 + _yPos, 1.77f + _zPos), Rotation = 67.5f},
                new ScreenPosition {Id = 5, Vector3 = new Vector3(4.29f + _xPos, 0 + _yPos, 0.3f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 6, Vector3 = new Vector3(4.0f + _xPos, 0 + _yPos, -1.18f + _zPos), Rotation = 112.5f},
                new ScreenPosition {Id = 7, Vector3 = new Vector3(3.17f + _xPos, 0 + _yPos, -2.43f + _zPos), Rotation = 135},
                new ScreenPosition {Id = 8, Vector3 = new Vector3(1.91f + _xPos, 0 + _yPos, -3.27f + _zPos), Rotation = 157.5f},
                new ScreenPosition {Id = 9, Vector3 = new Vector3(0.44f + _xPos, 0 + _yPos, -3.56f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 10, Vector3 = new Vector3(-1.03f + _xPos, 0 + _yPos, -3.27f + _zPos), Rotation = 202.5f},
                new ScreenPosition {Id = 11, Vector3 = new Vector3(-2.29f + _xPos, 0 + _yPos, -2.43f + _zPos), Rotation = 225},
                new ScreenPosition {Id = 12, Vector3 = new Vector3(-3.12f + _xPos, 0 + _yPos, -1.18f + _zPos), Rotation = 247.5f},
                new ScreenPosition {Id = 13, Vector3 = new Vector3(-3.41f + _xPos, 0 + _yPos, 0.3f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 14, Vector3 = new Vector3(-3.12f + _xPos, 0 + _yPos, 1.77f + _zPos), Rotation = 292.5f},
                new ScreenPosition {Id = 15, Vector3 = new Vector3(-2.28f + _xPos, 0 + _yPos, 3.03f + _zPos), Rotation = 315},
                new ScreenPosition {Id = 16, Vector3 = new Vector3(-1.03f + _xPos, 0 + _yPos, 3.86f + _zPos), Rotation = 337.5f}
            };

            return formation;
        }
        public List<ScreenPosition> Triangle()
        {
            List<ScreenPosition> formation = new List<ScreenPosition>
            {
                new ScreenPosition {Id = 1, Vector3 = new Vector3(0.55f + _xPos, 0 + _yPos, 4.92f + _zPos), Rotation = 70.529f},
                new ScreenPosition {Id = 2, Vector3 = new Vector3(1.06f + _xPos, 0 + _yPos, 3.48f + _zPos), Rotation = 70.529f},
                new ScreenPosition {Id = 3, Vector3 = new Vector3(1.57f + _xPos, 0 + _yPos, 2.03f + _zPos), Rotation = 70.529f},
                new ScreenPosition {Id = 4, Vector3 = new Vector3(2.08f + _xPos, 0 + _yPos, 0.58f + _zPos), Rotation = 70.529f},
                new ScreenPosition {Id = 5, Vector3 = new Vector3(2.59f + _xPos, 0 + _yPos, -0.86f + _zPos), Rotation = 70.529f},
                new ScreenPosition {Id = 6, Vector3 = new Vector3(3.11f + _xPos, 0 + _yPos, -2.31f + _zPos), Rotation = 70.529f},
                new ScreenPosition {Id = 7, Vector3 = new Vector3(2.59f + _xPos, 0 + _yPos, -3.02f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 8, Vector3 = new Vector3(1.06f + _xPos, 0 + _yPos, -3.02f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 9, Vector3 = new Vector3(-0.47f + _xPos, 0 + _yPos, -3.02f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 10, Vector3 = new Vector3(-2.01f + _xPos, 0 + _yPos, -3.02f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 11, Vector3 = new Vector3(-2.51f + _xPos, 0 + _yPos, -2.31f + _zPos), Rotation = -70.529f},
                new ScreenPosition {Id = 12, Vector3 = new Vector3(-2f + _xPos, 0 + _yPos, -0.86f + _zPos), Rotation = -70.529f},
                new ScreenPosition {Id = 13, Vector3 = new Vector3(-1.49f + _xPos, 0 + _yPos, 0.58f + _zPos), Rotation = -70.529f},
                new ScreenPosition {Id = 14, Vector3 = new Vector3(-0.98f + _xPos, 0 + _yPos, 2.03f + _zPos), Rotation = -70.529f},
                new ScreenPosition {Id = 15, Vector3 = new Vector3(-0.47f + _xPos, 0 + _yPos, 3.48f + _zPos), Rotation = -70.529f},
                new ScreenPosition {Id = 16, Vector3 = new Vector3(0.04f + _xPos, 0 + _yPos, 4.92f + _zPos), Rotation = -70.529f}
            };

            return formation;
        }
        public List<ScreenPosition> ShortRectangle()
        {
            List<ScreenPosition> formation = new List<ScreenPosition>
            {
                new ScreenPosition {Id = 1, Vector3 = new Vector3(1.2f + _xPos, 0 + _yPos, 1.77f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 2, Vector3 = new Vector3(2.73f + _xPos, 0 + _yPos, 1.77f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 3, Vector3 = new Vector3(4.27f + _xPos, 0 + _yPos, 1.77f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 4, Vector3 = new Vector3(5.03f + _xPos, 0 + _yPos, 1.0f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 5, Vector3 = new Vector3(5.03f + _xPos, 0 + _yPos, -0.53f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 6, Vector3 = new Vector3(4.27f + _xPos, 0 + _yPos, -1.3f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 7, Vector3 = new Vector3(2.73f + _xPos, 0 + _yPos, -1.3f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 8, Vector3 = new Vector3(1.2f + _xPos, 0 + _yPos, -1.3f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 9, Vector3 = new Vector3(-0.33f + _xPos, 0 + _yPos, -1.3f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 10, Vector3 = new Vector3(-1.87f + _xPos, 0 + _yPos, -1.3f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 11, Vector3 = new Vector3(-3.4f + _xPos, 0 + _yPos, -1.3f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 12, Vector3 = new Vector3(-4.16f + _xPos, 0 + _yPos, -0.53f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 13, Vector3 = new Vector3(-4.16f + _xPos, 0 + _yPos, 1.0f + _zPos), Rotation = 270},
                new ScreenPosition {Id = 14, Vector3 = new Vector3(-3.4f + _xPos, 0 + _yPos, 1.77f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 15, Vector3 = new Vector3(-1.87f + _xPos, 0 + _yPos, 1.77f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 16, Vector3 = new Vector3(-0.33f + _xPos, 0 + _yPos, 1.77f + _zPos), Rotation = 0}
            };

            return formation;
        }
        public List<ScreenPosition> LongRectangle()
        {
            List<ScreenPosition> formation = new List<ScreenPosition>
            {
                new ScreenPosition {Id = 1, Vector3 = new Vector3(0.46f + _xPos, 0 + _yPos, 0.97f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 2, Vector3 = new Vector3(1.99f + _xPos, 0 + _yPos, 0.97f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 3, Vector3 = new Vector3(3.53f + _xPos, 0 + _yPos, 0.97f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 4, Vector3 = new Vector3(5.06f + _xPos, 0 + _yPos, 0.97f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 5, Vector3 = new Vector3(5.79f + _xPos, 0 + _yPos, 0.2f + _zPos), Rotation = 90},
                new ScreenPosition {Id = 6, Vector3 = new Vector3(5.05f + _xPos, 0 + _yPos, -0.56f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 7, Vector3 = new Vector3(3.51f + _xPos, 0 + _yPos, -0.56f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 8, Vector3 = new Vector3(1.98f + _xPos, 0 + _yPos, -0.56f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 9, Vector3 = new Vector3(0.45f + _xPos, 0 + _yPos, -0.56f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 10, Vector3 = new Vector3(-1.09f + _xPos, 0 + _yPos, -0.56f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 11, Vector3 = new Vector3(-2.62f + _xPos, 0 + _yPos, -0.56f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 12, Vector3 = new Vector3(-4.15f + _xPos, 0 + _yPos, -0.56f + _zPos), Rotation = 180},
                new ScreenPosition {Id = 13, Vector3 = new Vector3(-4.91f + _xPos, 0 + _yPos, 0.2f + _zPos), Rotation = -90},
                new ScreenPosition {Id = 14, Vector3 = new Vector3(-4.14f + _xPos, 0 + _yPos, 0.97f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 15, Vector3 = new Vector3(-2.61f + _xPos, 0 + _yPos, 0.97f + _zPos), Rotation = 0},
                new ScreenPosition {Id = 16, Vector3 = new Vector3(-1.07f + _xPos, 0 + _yPos, 0.97f + _zPos), Rotation = 0}
            };

            return formation;
        }
    }
}
