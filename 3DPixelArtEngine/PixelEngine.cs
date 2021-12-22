using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace _3DPixelArtEngine
{
    public class PixelEngine
    {
        private int _width;
        private int _height;

        public Ray Camera;
        public bool CameraLocked;
        private int _pixelize;
        private float _cameraSize;

        private Texture2D _rectangle;

        public List<Object> Scene;

        private MouseState _lastMouseState;

        public PixelEngine(GraphicsDevice graphicsDevice, int width, int height, int pixelize = 3, float cameraSize = 0.1f)
        {
            _rectangle = new Texture2D(graphicsDevice, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            _rectangle.SetData(data);

            Scene = new List<Object>();

            _width = width;
            _height = height;

            Camera = new Ray(new Vector3(-10f, 0f, 0f), new Vector3(1f, 0f, 0f));
            CameraLocked = false;
            _pixelize = pixelize;
            _cameraSize = cameraSize;

            _lastMouseState = Mouse.GetState();
        }

        public List<Triangle> ImportMesh(string fileLocation)
        {
            List<Triangle> triangles = new List<Triangle>();

            

            return triangles;
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (_lastMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 difference = new Vector2(mouseState.X - _lastMouseState.X, mouseState.Y - _lastMouseState.Y);
                Camera.Rotate(new Vector3(0f, difference.X / -30f, 0f));
            }

            _lastMouseState = mouseState;

            KeyboardState state = Keyboard.GetState();

            float amount = Vector2.Distance(new Vector2(), new Vector2(Camera.Direction.X, Camera.Direction.Z));
            Vector2 cameraLateralDirection = Vector2.Normalize(new Vector2(Camera.Direction.X, Camera.Direction.Z));

            if (state.IsKeyDown(Keys.Right))
                Camera.Rotate(new Vector3(0f, 1f, 0f));
            if (state.IsKeyDown(Keys.Left))
                Camera.Rotate(new Vector3(0f, -1f, 0f));
            if (state.IsKeyDown(Keys.Up))
                Camera.Rotate(new Vector3(-1f, 0f, -1f));
            if (state.IsKeyDown(Keys.Down))
                Camera.Rotate(new Vector3(1f, 0f, 1f));

            //Camera.Direction = new Vector3(cameraLateralDirection.X * amount, Camera.Direction.Y, cameraLateralDirection.Y * amount);

            System.Diagnostics.Debug.WriteLine(Camera.Direction);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset = new Vector2())
        {
            Ray horizontalRay = Camera.Clone();
            Ray verticalRay = Camera.Clone();
            horizontalRay.Rotate(new Vector3(0f, 90f, 0f));
            verticalRay.Rotate(new Vector3(90f, 0f, 90f));

            Vector3 cameraStart = Camera.Point - new Vector3(_width * _cameraSize / 2f, 0f, _width * _cameraSize / 2f) * horizontalRay.Direction - new Vector3(_height * _cameraSize / 2f) * verticalRay.Direction;
            int xMax = (int)Math.Ceiling((float)_width / _pixelize);
            int yMax = (int)Math.Ceiling((float)_height / _pixelize);

            spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X, (int)offset.Y, xMax * _pixelize, yMax * _pixelize), new Color(40, 40, 40));

            for (int y = 0; y < yMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    for (int i = 0; i < Scene.Count; i++)
                    {
                        for (int v = 0; v < Scene[i].Triangles.Count; v++)
                        {
                            Triangle triangle = Scene[i].Triangles[v];
                            Ray pixelRay = new Ray(cameraStart + new Vector3(x * _cameraSize / 2f, 0f, x * _cameraSize / 2f) * horizontalRay.Direction + new Vector3(y * _cameraSize / 2f) * verticalRay.Direction, Camera.Direction);
                            if (triangle.Contains(pixelRay))
                            {
                                spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X + (xMax - x) * _pixelize, (int)offset.Y + (yMax - y) * _pixelize, _pixelize, _pixelize), Color.White);
                            }
                        }
                    }
                }
            }

            /*for (int y = 0; y < Math.Ceiling((float)_height / _pixelize); y++)
            {
                for (int x = 0; x < Math.Ceiling((float)_width / _pixelize); x++)
                {
                    for (int i = 0; i < Scene.Count; i++)
                    {
                        for (int v = 0; v < Scene[i].Triangles.Count; v++)
                        {
                            Triangle triangle = Scene[i].Triangles[v];
                            Ray pixelRay = new Ray(Camera.Point, new Vector3(-45f + 90f * (x / (float)Math.Ceiling((float)_width / _pixelize)), 0, -45f + 90f * (y / (float)Math.Ceiling((float)_height / _pixelize))));
                            if (triangle.Contains(pixelRay))
                            {
                                spriteBatch.Draw(_rectangle, new Rectangle(x * _pixelize, y * _pixelize, _pixelize, _pixelize), Color.White);
                            }
                        }
                    }
                }
            }*/
        }
    }
}
