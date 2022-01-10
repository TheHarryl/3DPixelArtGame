﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using System.Diagnostics;

namespace _3DPixelArtEngine
{
    public class PixelEngine
    {
        private GraphicsDevice _graphicsDevice;

        private int _width;
        private int _height;

        public Ray3 Camera;
        public bool CameraLocked;
        private int _pixelize;
        private float _cameraSize;

        public List<Object> Scene;

        private Texture2D _rectangle;
        private MouseState _lastMouseState;

        private bool _perspectiveRendering;
        private bool _perspectiveRenderingOuter;
        private float _perspectiveRenderingDistance;
        private float _perspectiveFOV;
        private float _perspectiveCameraSize;

        public Color[] Screen;
        private Texture2D _screen;

        public PixelEngine(GraphicsDevice graphicsDevice, int width, int height, int pixelize = 3, float cameraSize = 0.1f)
        {
            _graphicsDevice = graphicsDevice;

            _rectangle = new Texture2D(_graphicsDevice, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            _rectangle.SetData(data);

            Screen = new Color[(int)Math.Ceiling((float)width / pixelize) * (int)Math.Ceiling((float)height / pixelize)];
            _screen = new Texture2D(_graphicsDevice, (int)Math.Ceiling((float)width / pixelize), (int)Math.Ceiling((float)height / pixelize));

            //Object Sun = new Object(new Vector3(0f, 10000f, 0f));
            //Sun.Light = new PointLight(Color.White, 100000f, 100000f);
            //Sun.Light.Enabled = true;
            Scene = new List<Object>() {  };

            _width = width;
            _height = height;

            Camera = new Ray3(new Vector3(-10f, 0f, 0f));
            CameraLocked = false;
            _pixelize = pixelize;
            _cameraSize = cameraSize;

            _perspectiveRendering = false;
            _perspectiveRenderingOuter = true;
            _perspectiveFOV = 120f;

            _lastMouseState = Mouse.GetState();
        }

        public Texture2D ImportTexture(string fileLocation)
        {
            FileStream fileStream = new FileStream(fileLocation, FileMode.Open);
            Texture2D texture = Texture2D.FromStream(_graphicsDevice, fileStream);
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

            Scene[0].Mesh.Rotation += new Vector3(0f, 0f, 50f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Scene[0].Mesh.Position += new Vector3(0f, 1f, 0f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _lastMouseState = mouseState;

            KeyboardState state = Keyboard.GetState();

            float amount = 0.1f;

            if (state.IsKeyDown(Keys.Up))
                Camera.Rotate(new Vector3(0f, 0f, amount*10));
            if (state.IsKeyDown(Keys.Down))
                Camera.Rotate(new Vector3(0f, 0f, -amount*10));
            if (state.IsKeyDown(Keys.Right))
                Camera.Rotate(new Vector3(0f, amount*10, 0f));
            if (state.IsKeyDown(Keys.Left))
                Camera.Rotate(new Vector3(0f, -amount*10, 0f));
            if (state.IsKeyDown(Keys.PageUp))
                Camera.Rotate(new Vector3(amount*10, 0f, 0f));
            if (state.IsKeyDown(Keys.PageDown))
                Camera.Rotate(new Vector3(-amount*10, 0f, 0f));

            if (state.IsKeyDown(Keys.W))
                Camera.TranslateLocal(new Vector3(amount, 0f, 0f));
            if (state.IsKeyDown(Keys.S))
                Camera.TranslateLocal(new Vector3(-amount, 0f, 0f));
            if (state.IsKeyDown(Keys.A))
                Camera.TranslateLocal(new Vector3(0f, 0f, amount));
            if (state.IsKeyDown(Keys.D))
                Camera.TranslateLocal(new Vector3(0f, 0f, -amount));
            if (state.IsKeyDown(Keys.E))
                Camera.TranslateLocal(new Vector3(0f, amount, 0f));
            if (state.IsKeyDown(Keys.Q))
                Camera.TranslateLocal(new Vector3(0f, -amount, 0f));

            
        }

        public Vector2 PositionToScreenOrthographic(Vector3 position)
        {
            float cameraXLength = _width * _cameraSize;
            float cameraYLength = _height * _cameraSize;
            Vector3 cameraStart = Camera.Origin + (Camera.LongitudinalAxis.Direction * _height + Camera.LateralAxis.Direction * _width) * _cameraSize / 2f;
            Triangle cameraPlane = new Triangle(cameraStart, cameraStart + Camera.LateralAxis.Direction, cameraStart + Camera.LongitudinalAxis.Direction);
            Triangle cameraLateralPlane = new Triangle(cameraStart + Camera.Direction, cameraStart + Camera.LateralAxis.Direction, cameraStart - Camera.LateralAxis.Direction);
            Triangle cameraLateralPlane2 = new Triangle(cameraStart + Camera.Direction + Camera.LongitudinalAxis.Direction * 0.1f, cameraStart + Camera.LateralAxis.Direction + Camera.LongitudinalAxis.Direction * 0.1f, cameraStart - Camera.LateralAxis.Direction + Camera.LongitudinalAxis.Direction * 0.1f);
            Triangle cameraLongitudinalPlane = new Triangle(cameraStart + Camera.Direction, cameraStart + Camera.LongitudinalAxis.Direction, cameraStart - Camera.LongitudinalAxis.Direction);
            Triangle cameraLongitudinalPlane2 = new Triangle(cameraStart + Camera.Direction + Camera.LateralAxis.Direction * 0.1f, cameraStart + Camera.LongitudinalAxis.Direction + Camera.LateralAxis.Direction * 0.1f, cameraStart - Camera.LongitudinalAxis.Direction + Camera.LateralAxis.Direction * 0.1f);
            Vector3 cameraIntersection = cameraPlane.GetIntersection(new Ray(position, -Camera.Direction)) - Camera.Origin;
            Vector3 cameraXIntersection = cameraLongitudinalPlane.GetIntersection(new Ray(cameraIntersection, Camera.LateralAxis.Direction));
            Vector3 cameraXIntersection2 = cameraLongitudinalPlane2.GetIntersection(new Ray(cameraIntersection, Camera.LateralAxis.Direction));
            Vector3 cameraYIntersection = cameraLateralPlane.GetIntersection(new Ray(cameraIntersection, Camera.LongitudinalAxis.Direction));
            Vector3 cameraYIntersection2 = cameraLateralPlane2.GetIntersection(new Ray(cameraIntersection, Camera.LongitudinalAxis.Direction));
            float cameraXDistance = Vector3.Distance(cameraXIntersection, cameraIntersection);
            float cameraXDistance2 = Vector3.Distance(cameraXIntersection2, cameraIntersection);
            float cameraYDistance = Vector3.Distance(cameraYIntersection, cameraIntersection);
            float cameraYDistance2 = Vector3.Distance(cameraYIntersection2, cameraIntersection);
            return new Vector2((cameraXDistance > cameraXDistance2 ? -1 : 1) * (cameraXDistance / cameraXLength) * _width, (cameraYDistance > cameraYDistance2 ? -1 : 1) * (cameraYDistance / cameraYLength) * _height);

            /*
            int xMax = (int)Math.Ceiling((float)_width / _pixelize);
            int yMax = (int)Math.Ceiling((float)_height / _pixelize);
            Vector3 cameraLeftBottom = Camera.Origin - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f);
            Vector3 cameraRightBottom = cameraLeftBottom + (Camera.LateralAxis.Direction * (xMax - 1) * _cameraSize * _pixelize);
            Vector3 cameraRightTop = cameraLeftBottom + (Camera.LongitudinalAxis.Direction * (yMax-1) * _cameraSize * _pixelize) + (Camera.LateralAxis.Direction * (xMax-1) * _cameraSize * _pixelize);
            Vector3 cameraLeftTop = cameraLeftBottom + (Camera.LongitudinalAxis.Direction * (yMax - 1) * _cameraSize * _pixelize);
            Ray positionRay = new Ray(Camera.Origin, Vector3.Normalize(position - Camera.Origin));
            //try reversing the order of the normals if doesn't work
            Triangle canvasLeftBottom = new Triangle(cameraLeftTop, cameraLeftBottom, cameraRightBottom);
            Triangle canvasRightTop = new Triangle(cameraLeftTop, cameraRightBottom, cameraRightTop);
            Vector3 projectedPosition;
            if (canvasLeftBottom.Contains(positionRay))
                projectedPosition = canvasLeftBottom.GetIntersection(positionRay);
            else if (canvasRightTop.Contains(positionRay))
                projectedPosition = canvasRightTop.GetIntersection(positionRay);
            else
                return new Vector2(-1, -1);

            float distanceX1 = Vector3.Distance(projectedPosition, cameraLeftBottom);
            float distanceX2 = Vector3.Distance(projectedPosition, cameraLeftTop);
            float bX = (yMax - 1) * _cameraSize * _pixelize;
            float sX = (distanceX1 + distanceX2 + bX) / 2f;
            float x = (2f * (float)Math.Sqrt(sX * (sX - distanceX1) * (sX - distanceX2) * (sX - bX))) / bX;
            float distanceY1 = Vector3.Distance(projectedPosition, cameraLeftBottom);
            float distanceY2 = Vector3.Distance(projectedPosition, cameraRightBottom);
            float bY = (xMax - 1) * _cameraSize * _pixelize;
            float sY = (distanceY1 + distanceY2 + bY) / 2f;
            float y = (2f * (float)Math.Sqrt(sY * (sY - distanceY1) * (sY - distanceY2) * (sY - bY))) / bY;

                return new Vector2((x)/_cameraSize, (y) / _cameraSize);
            */
        }

        public void PositionToScreenPerspective()
        {

        }

        public void EnablePerspectiveRenderingFrom(float fromDistance, float FOV, float perspectiveCameraSize)
        {
            _perspectiveRendering = true;
            _perspectiveRenderingOuter = true;
            _perspectiveRenderingDistance = fromDistance;
            _perspectiveFOV = FOV;
            _perspectiveCameraSize = perspectiveCameraSize;
        }

        public void EnablePerspectiveRenderingUntil(float untilDistance, float FOV, float perspectiveCameraSize)
        {
            _perspectiveRendering = true;
            _perspectiveRenderingOuter = false;
            _perspectiveRenderingDistance = untilDistance;
            _perspectiveFOV = FOV;
            _perspectiveCameraSize = perspectiveCameraSize;
        }

        public void DisablePerspectiveRendering()
        {
            _perspectiveRendering = false;
        }

        private bool IsPerspectiveRenderingAtDistance(float distance)
        {
            return _perspectiveRendering && ((_perspectiveRenderingOuter && distance >= _perspectiveRenderingDistance) || (!_perspectiveRenderingOuter && distance <= _perspectiveRenderingDistance));
        }

        public void Render(bool perspective = false, float fov = 120f)
        {
            Vector3 cameraStart;
            if (!perspective)
                cameraStart = Camera.Origin - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f);
            else
                cameraStart = Camera.Origin + (Camera.Direction * (_width * _cameraSize / 2f) / (float)Math.Tan(fov * Math.PI / 360f)) - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f);
            
            Vector3 cameraStartPerspective = Camera.Origin + (Camera.Direction * (_width * _cameraSize / 2f) / (float)Math.Tan(_perspectiveFOV * Math.PI / 360f)) - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f);
            int xMax = (int)Math.Ceiling((float)_width / _pixelize);
            int yMax = (int)Math.Ceiling((float)_height / _pixelize);

            Screen = new Color[(int)Math.Ceiling((float)_width / _pixelize) * (int)Math.Ceiling((float)_height / _pixelize)];

            for (int y = 0; y < yMax; y++)
            {
                for (int x = 0; x < xMax; x++)
                {
                    Vector3 cameraOrigin = cameraStart + (Camera.LongitudinalAxis.Direction * y * _cameraSize * _pixelize) + (Camera.LateralAxis.Direction * x * _cameraSize * _pixelize);
                    Ray pixelRay;
                    if (!perspective)
                        pixelRay = new Ray(cameraOrigin, Camera.Direction);
                    else
                        pixelRay = new Ray(Camera.Origin, Vector3.Normalize(cameraOrigin - Camera.Origin));

                    Triangle closestTriangle = null;

                    for (int i = 0; i < Scene.Count; i++)
                    {
                        if (Scene[i].Mesh == null) continue;
                        for (int v = 0; v < Scene[i].Mesh.GetTriangles().Count; v++)
                        {
                            Triangle triangle = Scene[i].Mesh.GetTriangles()[v];
                            if (!triangle.Contains(pixelRay))
                                continue;
                            if ((closestTriangle == null || Vector3.Distance(cameraOrigin, closestTriangle.GetIntersection(pixelRay)) > Vector3.Distance(cameraOrigin, triangle.GetIntersection(pixelRay))))
                                closestTriangle = triangle;
                            /*Color pixelColor = Color.Black;
                            for (int l = 0; l < Scene.Count; l++)
                            {
                                if (Scene[l].Light == null || !Scene[l].Light.Enabled || Vector3.Distance(Scene[l].Position, triangle.Center) > Scene[l].Light.OuterRange) continue;
                                float intensity = Scene[l].Light.GetIntensityAtDistance(Vector3.Distance(Scene[l].Position, triangle.Center));
                                pixelColor = Color.Lerp(pixelColor, Scene[l].Light.Color, intensity * Vector3.Dot(triangle.GetReflection(pixelRay).Direction, triangle.Normal));
                            }
                            spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X + (xMax - x - 1) * _pixelize, (int)offset.Y + (yMax - y - 1) * _pixelize, _pixelize, _pixelize), pixelColor);*/

                        }
                    }

                    if (closestTriangle != null)
                    {
                        float darken = (Vector3.Distance(cameraOrigin, closestTriangle.GetIntersection(pixelRay)) - 5f) / 10f;
                        //float darken = Vector3.Dot(pixelRay.Direction, closestTriangle.Normal);
                        //Debug.WriteLine(darken);
                        Screen[(xMax - x - 1) + (yMax - y - 1) * xMax] = Color.Lerp(Color.Black, Color.White, darken);
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset = new Vector2())
        {
            _screen.SetData(Screen);
            spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X, (int)offset.Y, _width, _height), new Color(40, 40, 40));
            spriteBatch.Draw(_screen, new Rectangle((int)offset.X, (int)offset.Y, (int)Math.Ceiling((float)_width / _pixelize) * _pixelize, (int)Math.Ceiling((float)_height / _pixelize) * _pixelize), Color.White);



            for (int i = 0; i < Scene.Count; i++)
            {
                if (Scene[i].Mesh == null) continue;
                for (int v = 0; v < Scene[i].Mesh.GetTriangles().Count; v++)
                {
                    Triangle triangle = Scene[i].Mesh.GetTriangles()[v];
                    Vector2 Point1 = PositionToScreenOrthographic(triangle.Point1);
                    Vector2 Point2 = PositionToScreenOrthographic(triangle.Point2);
                    Vector2 Point3 = PositionToScreenOrthographic(triangle.Point3);
                    Vector2 Screen1 = PositionToScreenOrthographic(Camera.Origin - Camera.LateralAxis.Direction * _width * _cameraSize / 2f - Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f);
                    Vector2 Screen2 = PositionToScreenOrthographic(Camera.Origin - Camera.LateralAxis.Direction * _width * _cameraSize / 2f + Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f);
                    Vector2 Screen3 = PositionToScreenOrthographic(Camera.Origin + Camera.LateralAxis.Direction * _width * _cameraSize / 2f + Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f);
                    Vector2 Screen4 = PositionToScreenOrthographic(Camera.Origin + Camera.LateralAxis.Direction * _width * _cameraSize / 2f - Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f);
                    System.Diagnostics.Debug.WriteLine(Point1 + " " + Point2 + " " + Point3);
                    DrawLine(spriteBatch, Point1, Point2, Color.Black, 2);
                    DrawLine(spriteBatch, Point2, Point3, Color.Black, 2);
                    DrawLine(spriteBatch, Point3, Point1, Color.Black, 2);
                    DrawLine(spriteBatch, Screen1, Screen2, Color.Black, 2);
                    DrawLine(spriteBatch, Screen2, Screen3, Color.Black, 2);
                    DrawLine(spriteBatch, Screen3, Screen4, Color.Black, 2);
                    DrawLine(spriteBatch, Screen4, Screen1, Color.Black, 2);
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
