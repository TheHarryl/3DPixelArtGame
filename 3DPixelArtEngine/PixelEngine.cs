using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;

namespace _3DPixelArtEngine
{
    public class PixelEngine
    {
        private GraphicsDevice _graphicsDevice;

        private int _width;
        private int _height;
        private int _pixelWidth;
        private int _pixelHeight;

        public int Width
        {
            get => _width;
            set
            {
                _width = value;
                _pixelWidth = (int)Math.Ceiling((float)_width / PixelRatio);
            }
        }
        public int Height
        {
            get => _height;
            set
            {
                _height = value;
                _pixelHeight = (int)Math.Ceiling((float)_height / PixelRatio);
            }
        }

        public Ray3 Camera;
        public float CameraSize;
        public bool CameraLocked;
        public int PixelRatio;

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

        public PixelEngine(GraphicsDevice graphicsDevice, int width, int height, int pixelRatio = 3, float cameraSize = 0.05f)
        {
            _graphicsDevice = graphicsDevice;

            _rectangle = new Texture2D(_graphicsDevice, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            _rectangle.SetData(data);

            PixelRatio = pixelRatio;
            Width = width;
            Height = height;

            Screen = new Color[_pixelWidth * _pixelHeight];
            Debug.WriteLine(_pixelWidth * _pixelHeight);
            _screen = new Texture2D(_graphicsDevice, _pixelWidth, _pixelHeight);

            //Object Sun = new Object(new Vector3(0f, 10000f, 0f));
            //Sun.Light = new PointLight(Color.White, 100000f, 100000f);
            //Sun.Light.Enabled = true;
            Scene = new List<Object>() { };

            Camera = new Ray3(new Vector3(-10f, 0f, 0f));
            CameraLocked = false;
            CameraSize = cameraSize;

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

            if (_lastMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 difference = new Vector2(mouseState.X - _lastMouseState.X, mouseState.Y - _lastMouseState.Y);
                Camera.TranslateLocal(new Vector3(0f, difference.Y / 10f, difference.X / 10f));
            }

            Scene[0].Rotation += new Vector3(0f, 0f, 50f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Scene[0].Mesh.Position += new Vector3(0f, 1f, 0f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

            _lastMouseState = mouseState;

            KeyboardState state = Keyboard.GetState();

            float amount = 0.1f;

            if (state.IsKeyDown(Keys.Up))
                Camera.Rotate(new Vector3(0f, 0f, amount * 10));
            if (state.IsKeyDown(Keys.Down))
                Camera.Rotate(new Vector3(0f, 0f, -amount * 10));
            if (state.IsKeyDown(Keys.Right))
                Camera.Rotate(new Vector3(0f, amount * 10, 0f));
            if (state.IsKeyDown(Keys.Left))
                Camera.Rotate(new Vector3(0f, -amount * 10, 0f));
            if (state.IsKeyDown(Keys.PageUp))
                Camera.Rotate(new Vector3(amount * 10, 0f, 0f));
            if (state.IsKeyDown(Keys.PageDown))
                Camera.Rotate(new Vector3(-amount * 10, 0f, 0f));

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

        private Vector2 NativeToPixelResolution(Vector2 native)
        {
            return new Vector2((float)Math.Round(native.X / PixelRatio), (float)Math.Round(native.Y / PixelRatio));
        }

        public Vector2 PositionToScreenOrthographic(Vector3 position)
        {
            float cameraXLength = Width * CameraSize;
            float cameraYLength = Height * CameraSize;
            Vector3 cameraStart = Camera.Origin + (Camera.LongitudinalAxis.Direction * Height + Camera.LateralAxis.Direction * Width) * CameraSize / 2f;
            Plane cameraPlane = new Plane(cameraStart, cameraStart + Camera.LateralAxis.Direction, cameraStart + Camera.LongitudinalAxis.Direction);
            Plane cameraLateralPlane = new Plane(cameraStart + Camera.Direction, cameraStart + Camera.LateralAxis.Direction, cameraStart - Camera.LateralAxis.Direction);
            Plane cameraLateralPlane2 = new Plane(cameraStart + Camera.Direction + Camera.LongitudinalAxis.Direction * 0.1f, cameraStart + Camera.LateralAxis.Direction + Camera.LongitudinalAxis.Direction * 0.1f, cameraStart - Camera.LateralAxis.Direction + Camera.LongitudinalAxis.Direction * 0.1f);
            Plane cameraLongitudinalPlane = new Plane(cameraStart + Camera.Direction, cameraStart + Camera.LongitudinalAxis.Direction, cameraStart - Camera.LongitudinalAxis.Direction);
            Plane cameraLongitudinalPlane2 = new Plane(cameraStart + Camera.Direction + Camera.LateralAxis.Direction * 0.1f, cameraStart + Camera.LongitudinalAxis.Direction + Camera.LateralAxis.Direction * 0.1f, cameraStart - Camera.LongitudinalAxis.Direction + Camera.LateralAxis.Direction * 0.1f);
            Vector3 cameraIntersection = cameraPlane.GetIntersection(new Ray(position, -Camera.Direction));
            Vector3 cameraXIntersection = cameraLongitudinalPlane.GetIntersection(new Ray(cameraIntersection, Camera.LateralAxis.Direction));
            Vector3 cameraXIntersection2 = cameraLongitudinalPlane2.GetIntersection(new Ray(cameraIntersection, Camera.LateralAxis.Direction));
            Vector3 cameraYIntersection = cameraLateralPlane.GetIntersection(new Ray(cameraIntersection, Camera.LongitudinalAxis.Direction));
            Vector3 cameraYIntersection2 = cameraLateralPlane2.GetIntersection(new Ray(cameraIntersection, Camera.LongitudinalAxis.Direction));
            float cameraXDistance = Vector3.Distance(cameraXIntersection, cameraIntersection);
            float cameraXDistance2 = Vector3.Distance(cameraXIntersection2, cameraIntersection);
            float cameraYDistance = Vector3.Distance(cameraYIntersection, cameraIntersection);
            float cameraYDistance2 = Vector3.Distance(cameraYIntersection2, cameraIntersection);
            return new Vector2((cameraXDistance > cameraXDistance2 ? -1 : 1) * (cameraXDistance / cameraXLength) * Width, (cameraYDistance > cameraYDistance2 ? -1 : 1) * (cameraYDistance / cameraYLength) * Height);

            /*
            Vector3 cameraLeftBottom = Camera.Origin - (Camera.LongitudinalAxis.Direction * Height * CameraSize / 2f) - (Camera.LateralAxis.Direction * Width * CameraSize / 2f);
            Vector3 cameraRightBottom = cameraLeftBottom + (Camera.LateralAxis.Direction * (_pixelWidth - 1) * CameraSize * PixelRatio);
            Vector3 cameraRightTop = cameraLeftBottom + (Camera.LongitudinalAxis.Direction * (_pixelHeight-1) * CameraSize * PixelRatio) + (Camera.LateralAxis.Direction * (_pixelWidth-1) * CameraSize * PixelRatio);
            Vector3 cameraLeftTop = cameraLeftBottom + (Camera.LongitudinalAxis.Direction * (_pixelHeight - 1) * CameraSize * PixelRatio);
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
            float bX = (_pixelHeight - 1) * CameraSize * PixelRatio;
            float sX = (distanceX1 + distanceX2 + bX) / 2f;
            float x = (2f * (float)Math.Sqrt(sX * (sX - distanceX1) * (sX - distanceX2) * (sX - bX))) / bX;
            float distanceY1 = Vector3.Distance(projectedPosition, cameraLeftBottom);
            float distanceY2 = Vector3.Distance(projectedPosition, cameraRightBottom);
            float bY = (_pixelWidth - 1) * CameraSize * PixelRatio;
            float sY = (distanceY1 + distanceY2 + bY) / 2f;
            float y = (2f * (float)Math.Sqrt(sY * (sY - distanceY1) * (sY - distanceY2) * (sY - bY))) / bY;

                return new Vector2((x)/CameraSize, (y) / CameraSize);
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
                cameraStart = Camera.Origin - (Camera.LongitudinalAxis.Direction * Height * CameraSize / 2f) - (Camera.LateralAxis.Direction * Width * CameraSize / 2f);
            else
                cameraStart = Camera.Origin + (Camera.Direction * (Width * CameraSize / 2f) / (float)Math.Tan(fov * Math.PI / 360f)) - (Camera.LongitudinalAxis.Direction * Height * CameraSize / 2f) - (Camera.LateralAxis.Direction * Width * CameraSize / 2f);

            Vector3 cameraStartPerspective = Camera.Origin + (Camera.Direction * (Width * CameraSize / 2f) / (float)Math.Tan(_perspectiveFOV * Math.PI / 360f)) - (Camera.LongitudinalAxis.Direction * Height * CameraSize / 2f) - (Camera.LateralAxis.Direction * Width * CameraSize / 2f);

            Screen = new Color[(int)Math.Ceiling((float)Width / PixelRatio) * (int)Math.Ceiling((float)Height / PixelRatio)];

            for (int y = 0; y < _pixelHeight; y++)
            {
                for (int x = 0; x < _pixelWidth; x++)
                {
                    Vector3 cameraOrigin = cameraStart + (Camera.LongitudinalAxis.Direction * y * CameraSize * PixelRatio) + (Camera.LateralAxis.Direction * x * CameraSize * PixelRatio);
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
                            spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X + (_pixelWidth - x - 1) * PixelRatio, (int)offset.Y + (_pixelHeight - y - 1) * PixelRatio, PixelRatio, PixelRatio), pixelColor);*/

                        }
                    }

                    if (closestTriangle != null)
                    {
                        float darken = (Vector3.Distance(cameraOrigin, closestTriangle.GetIntersection(pixelRay)) - 5f) / 10f;
                        //float darken = Vector3.Dot(pixelRay.Direction, closestTriangle.Normal);
                        //Debug.WriteLine(darken);
                        Screen[(_pixelWidth - x - 1) + (_pixelHeight - y - 1) * _pixelWidth] = Color.Lerp(Color.Black, Color.White, darken);
                    }
                }
            }

            for (int i = 0; i < Scene.Count; i++)
            {
                if (Scene[i].Mesh == null) continue;
                for (int v = 0; v < Scene[i].Mesh.GetTriangles().Count; v++)
                {
                    Triangle triangle = Scene[i].Mesh.GetTriangles()[v];
                    Vector2 Point1 = PositionToScreenOrthographic(triangle.Point1);
                    Vector2 Point2 = PositionToScreenOrthographic(triangle.Point2);
                    Vector2 Point3 = PositionToScreenOrthographic(triangle.Point3);
                    RenderLine(Point1, Point2, Color.Black, 2);
                    RenderLine(Point2, Point3, Color.Black, 2);
                    RenderLine(Point3, Point1, Color.Black, 2);
                }
            }

            Vector2 Screen1 = PositionToScreenOrthographic(Camera.Origin - Camera.LateralAxis.Direction * Width * CameraSize / 2f - Camera.LongitudinalAxis.Direction * Height * CameraSize / 2f);
            Vector2 Screen2 = PositionToScreenOrthographic(Camera.Origin - Camera.LateralAxis.Direction * Width * CameraSize / 2f + Camera.LongitudinalAxis.Direction * Height * CameraSize / 2f);
            Vector2 Screen3 = PositionToScreenOrthographic(Camera.Origin + Camera.LateralAxis.Direction * Width * CameraSize / 2f + Camera.LongitudinalAxis.Direction * Height * CameraSize / 2f);
            Vector2 Screen4 = PositionToScreenOrthographic(Camera.Origin + Camera.LateralAxis.Direction * Width * CameraSize / 2f - Camera.LongitudinalAxis.Direction * Height * CameraSize / 2f);
            RenderLine(Screen1, Screen2, Color.Black, 2);
            RenderLine(Screen2, Screen3, Color.Black, 2);
            RenderLine(Screen3, Screen4, Color.Black, 2);
            RenderLine(Screen4, Screen1, Color.Black, 2);

        }

        public void RenderLine(Vector2 begin, Vector2 end, Color color, int thickness = 1)
        {
            begin = NativeToPixelResolution(begin);
            end = NativeToPixelResolution(end);

            bool steep = Math.Abs(end.Y - begin.Y) > Math.Abs(end.X - begin.X);
            if (steep)
            {
                begin = new Vector2(begin.Y, begin.X);
                end = new Vector2(end.Y, end.X);
            }
            if (begin.X > end.X)
            {
                float t;
                t = begin.X; // swap begin.X and end.X
                begin.X = end.X;
                end.X = t;

                t = begin.Y; // swap begin.Y and end.Y
                begin.Y = end.Y;
                end.Y = t;
            }
            float dx = (end - begin).X;
            float dy = Math.Abs(end.Y - begin.Y);
            float error = (dx / 2f);
            int ystep = (begin.Y < end.Y) ? 1 : -1;
            float y = begin.Y;
            for (int x = (int)begin.X; x <= end.X; x++)
            {
                if (x - 1 >= 0 && x < _pixelWidth && y - 1 >= 0 && y < _pixelHeight)
                    Screen[(int)(steep ? y : x) - 1 + (int)((steep ? x : y) - 1) * _pixelWidth] = color;
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }

            /*Vector2 currentPixel = begin.X < end.X ? begin : end;
            Vector2 targetPixel = begin.X < end.X ? end : begin;
            while (currentPixel != targetPixel)
            {
                Screen[(int)(currentPixel.X) + (int)(currentPixel.Y) * _pixelWidth] = color;
                if (targetPixel.X == currentPixel.X)
                {
                    if (currentPixel.Y > targetPixel.Y)
                        currentPixel.Y--;
                    if (currentPixel.Y < targetPixel.Y)
                        currentPixel.Y++;
                    continue;
                } else if (targetPixel.Y == currentPixel.Y)
                {
                    currentPixel.X++;
                    continue;
                }

                float slope = (targetPixel - currentPixel).Y / (targetPixel - currentPixel).X;
                if (slope >= Math.Sqrt(2) + 1f)
                    currentPixel += new Vector2(0, 1);
                else if (slope >= Math.Sqrt(2) - 1f)
                    currentPixel += new Vector2(1, 1);
                else if (slope >= -(Math.Sqrt(2) - 1f))
                    currentPixel += new Vector2(1, 0);
                else if (slope >= -(Math.Sqrt(2) + 1f))
                    currentPixel += new Vector2(1, -1);
                else
                    currentPixel += new Vector2(0, -1);
            }
            Screen[(int)(currentPixel.X) + (int)(currentPixel.Y) * _pixelWidth] = color;*/
        }

        public void RenderTriangle(Vector2 point1, Vector2 point2, Vector2 point3, Color color)
        {
            point1 = NativeToPixelResolution(point1);
            point2 = NativeToPixelResolution(point2);
            point3 = NativeToPixelResolution(point3);

            //for (int )

            Screen[(int)(-1) + (int)(-1) * _pixelWidth] = color;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset = new Vector2())
        {
            _screen.SetData(Screen);
            spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X, (int)offset.Y, Width, Height), new Color(40, 40, 40));
            spriteBatch.Draw(_screen, new Rectangle((int)offset.X, (int)offset.Y, (int)Math.Ceiling((float)Width / PixelRatio) * PixelRatio, (int)Math.Ceiling((float)Height / PixelRatio) * PixelRatio), Color.White);

            //Debug.WriteLine(Vector3.Distance(Camera.Direction, new Vector3(1, 0, 0)) + " " + Vector3.Distance(Camera.LateralAxis.Direction, new Vector3(0, 0, 1)) + " " + Vector3.Distance(Camera.LongitudinalAxis.Direction, new Vector3(0, 1, 0)));
        }
    }
}
