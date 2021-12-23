using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace _3DPixelArtEngine
{
    public class PixelEngine
    {
        private int _width;
        private int _height;

        public Ray3 Camera;
        public bool CameraLocked;
        private int _pixelize;
        private float _cameraSize;

        public List<Object> Scene;

        private Texture2D _rectangle;
        private MouseState _lastMouseState;

        public PixelEngine(GraphicsDevice graphicsDevice, int width, int height, int pixelize = 3, float cameraSize = 0.1f)
        {
            _rectangle = new Texture2D(graphicsDevice, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            _rectangle.SetData(data);

            Object Sun = new Object(new Vector3(0f, 10000f, 0f));
            Sun.Light = new PointLight(Color.White, 100000f, 100000f);
            Sun.Light.Enabled = true;
            Scene = new List<Object>() { Sun };

            _width = width;
            _height = height;

            Camera = new Ray3(new Vector3(-10f, 0f, 0f));
            CameraLocked = false;
            _pixelize = pixelize;
            _cameraSize = cameraSize;

            _lastMouseState = Mouse.GetState();
        }

        public List<Triangle> ImportMesh(string fileLocation)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Triangle> triangles = new List<Triangle>();

            var lines = File.ReadLines(fileLocation);
            if (fileLocation.EndsWith(".obj"))
            {
                foreach (string line in lines)
                {
                    if (line.StartsWith("v "))
                    {
                        string[] args = line.Split(" ");
                        vertices.Add(new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])));
                    }
                }
            }

            return triangles;
        }

        public Texture2D ImportTexture(GraphicsDevice graphicsDevice, string fileLocation)
        {
            FileStream fileStream = new FileStream(fileLocation, FileMode.Open);
            Texture2D texture = Texture2D.FromStream(graphicsDevice, fileStream);
            fileStream.Dispose();

            return texture;
        }

        public void Update(GameTime gameTime)
        {
            MouseState mouseState = Mouse.GetState();

            if (_lastMouseState.RightButton == ButtonState.Pressed && mouseState.RightButton == ButtonState.Pressed)
            {
                Vector2 difference = new Vector2(mouseState.X - _lastMouseState.X, mouseState.Y - _lastMouseState.Y);
                Camera.TranslateLocal(new Vector3(0f, difference.Y / 10f, difference.X / 10f));
            }

            Scene[1].Rotation += new Vector3(0f, 0f, 50f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _lastMouseState = mouseState;

            KeyboardState state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Up))
                Camera.Rotate(new Vector3(0f, 0f, 1f));
            if (state.IsKeyDown(Keys.Down))
                Camera.Rotate(new Vector3(0f, 0f, -1f));
            if (state.IsKeyDown(Keys.Right))
                Camera.Rotate(new Vector3(0f, 1f, 0f));
            if (state.IsKeyDown(Keys.Left))
                Camera.Rotate(new Vector3(0f, -1f, 0f));
            if (state.IsKeyDown(Keys.PageUp))
                Camera.Rotate(new Vector3(1f, 0f, 0f));
            if (state.IsKeyDown(Keys.PageDown))
                Camera.Rotate(new Vector3(-1f, 0f, 0f));

            if (state.IsKeyDown(Keys.W))
                Camera.TranslateLocal(new Vector3(1f, 0f, 0f));
            if (state.IsKeyDown(Keys.S))
                Camera.TranslateLocal(new Vector3(-1f, 0f, 0f));
            if (state.IsKeyDown(Keys.A))
                Camera.TranslateLocal(new Vector3(0f, 0f, 1f));
            if (state.IsKeyDown(Keys.D))
                Camera.TranslateLocal(new Vector3(0f, 0f, -1f));
            if (state.IsKeyDown(Keys.E))
                Camera.TranslateLocal(new Vector3(0f, 1f, 0f));
            if (state.IsKeyDown(Keys.Q))
                Camera.TranslateLocal(new Vector3(0f, -1f, 0f));
        }

        public Vector2 PositionToScreen(Vector3 position)
        {
            Triangle cameraPlane = new Triangle(Camera.Origin, Camera.Origin + Camera.LateralAxis.Direction, Camera.Origin + Camera.LongitudinalAxis.Direction);
            Vector3 cameraOffset = cameraPlane.GetIntersection(new Ray(position, -Camera.Direction)) - Camera.Origin;

            return new Vector2();
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset = new Vector2())
        {
            Vector3 cameraStart = Camera.Origin - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f);
            int xMax = (int)Math.Ceiling((float)_width / _pixelize);
            int yMax = (int)Math.Ceiling((float)_height / _pixelize);

            spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X, (int)offset.Y, xMax * _pixelize, yMax * _pixelize), new Color(40, 40, 40));

            for (int y = 0; y < yMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    for (int i = 0; i < Scene.Count; i++)
                    {
                        if (Scene[i].Mesh == null) continue;
                        for (int v = 0; v < Scene[i].Mesh.Count; v++)
                        {
                            Triangle triangle = Scene[i].Mesh[v];
                            Vector3 cameraOrigin = cameraStart + (Camera.LongitudinalAxis.Direction * y * _cameraSize * _pixelize) + (Camera.LateralAxis.Direction * x * _cameraSize * _pixelize);
                            Ray pixelRay = new Ray(cameraOrigin, Camera.Direction);
                            if (triangle.Contains(pixelRay))
                            {
                                Color pixelColor = Color.Black;
                                for (int l = 0; l < Scene.Count; l++)
                                {
                                    if (Scene[l].Light == null || !Scene[l].Light.Enabled || Vector3.Distance(Scene[l].Position, triangle.Center) > Scene[l].Light.OuterRange) continue;
                                    float intensity = Scene[l].Light.GetIntensityAtDistance(Vector3.Distance(Scene[l].Position, triangle.Center));
                                    pixelColor = Color.Lerp(pixelColor, Scene[l].Light.Color, intensity * Vector3.Dot(triangle.GetReflection(new Ray(Scene[l].Position, triangle.Center)).Direction, triangle.Perpendicular));
                                }
                                spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X + (xMax - x - 1) * _pixelize, (int)offset.Y + (yMax - y - 1) * _pixelize, _pixelize, _pixelize), pixelColor);
                            }
                        }
                    }
                }
            }
        }

        public void DrawPerspective(SpriteBatch spriteBatch, float fov = 120, Vector2 offset = new Vector2())
        {
            _cameraSize = .0025f;
            Vector3 cameraStart = Camera.Origin + (Camera.Direction * (_width * _cameraSize / 2f) / (float) Math.Tan(fov * Math.PI / 360f)) - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f);
            int xMax = (int)Math.Ceiling((float)_width / _pixelize);
            int yMax = (int)Math.Ceiling((float)_height / _pixelize);

            spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X, (int)offset.Y, xMax * _pixelize, yMax * _pixelize), new Color(40, 40, 40));
            for (int y = 0; y < yMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    for (int i = 0; i < Scene.Count; i++)
                    {
                        for (int v = 0; v < Scene[i].Mesh.Count; v++)
                        {
                            Triangle triangle = Scene[i].Mesh[v];
                            Vector3 cameraOrigin = cameraStart + (Camera.LongitudinalAxis.Direction * y * _cameraSize * _pixelize) + (Camera.LateralAxis.Direction * x * _cameraSize * _pixelize);
                            Ray pixelRay = new Ray(Camera.Origin, Vector3.Normalize(cameraOrigin - Camera.Origin));
                            if (triangle.Contains(pixelRay))
                            {
                                float darken = (Vector3.Distance(cameraOrigin, triangle.GetIntersection(pixelRay)) - 5f) / 10f;
                                spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X + (xMax - x - 1) * _pixelize, (int)offset.Y + (yMax - y - 1) * _pixelize, _pixelize, _pixelize), Color.Lerp(Color.White, Color.Black, darken));
                            }
                            /*DrawLine(spriteBatch, PositionToScreen(triangle.Point1), PositionToScreen(triangle.Point2), Color.Black);
                            DrawLine(spriteBatch, PositionToScreen(triangle.Point2), PositionToScreen(triangle.Point3), Color.Black);
                            DrawLine(spriteBatch, PositionToScreen(triangle.Point3), PositionToScreen(triangle.Point1), Color.Black);*/
                        }
                    }
                }
            }
        }

        private void DrawLine(SpriteBatch spriteBatch, Vector2 begin, Vector2 end, Color color, int width = 1)
        {
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + width, width);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;
            spriteBatch.Draw(_rectangle, r, null, color, angle, Microsoft.Xna.Framework.Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
