using Microsoft.Xna.Framework;
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

        public PixelEngine(GraphicsDevice graphicsDevice, int width, int height, int pixelize = 3, float cameraSize = 0.1f)
        {
            _graphicsDevice = graphicsDevice;

            _rectangle = new Texture2D(_graphicsDevice, 1, 1);
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

            _perspectiveRendering = false;
            _perspectiveRenderingOuter = true;
            _perspectiveFOV = 120f;

            _lastMouseState = Mouse.GetState();
        }

        public static List<Triangle> ImportMesh(string fileLocation)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> vertexNormals = new List<Vector3>();
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
                    if (line.StartsWith("vn"))
                    {
                        string[] args = line.Split(" ");
                        vertexNormals.Add(Vector3.Normalize(new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]))));
                    }
                    if (line.StartsWith("f "))
                    {
                        string[] argsStr = line.Split(" ");
                        string[][] args = new string[argsStr.Length][];
                        for (int i = 0; i < argsStr.Length; i++)
                        {
                            args[i] = argsStr[i].Split("/");
                        }    

                        if (args.Length == 4) //three vertices => triangle
                        {
                            Triangle triangle = new Triangle(vertices[int.Parse(args[1][0]) - 1], vertices[int.Parse(args[2][0]) - 1], vertices[int.Parse(args[3][0]) - 1]);
                            if (args[1].Length == 3)
                            {
                                double angleBetweenNormals = Math.Acos(Vector3.Dot(Vector3.Normalize(vertexNormals[int.Parse(args[1][2]) - 1]), Vector3.Normalize(triangle.Normal)));
                                if (angleBetweenNormals > Math.PI/2)
                                    triangle = new Triangle(vertices[int.Parse(args[2][0]) - 1], vertices[int.Parse(args[1][0]) - 1], vertices[int.Parse(args[3][0]) - 1]);
                            }
                            triangles.Add(triangle);
                        }
                        else //otherwise, split into triangles using ears method
                        {
                            List<Vector3> polygonVertices = new List<Vector3>();
                            for (int i = 1; i < args.Length; i++)
                            {
                                
                                polygonVertices.Add(vertices[int.Parse(args[i][0])-1]);
                            }
                            for (int i = 0; i < polygonVertices.Count; i++)
                            {   
                                if (polygonVertices.Count == 3)
                                {
                                    Triangle triangle = new Triangle(polygonVertices[0], polygonVertices[1], polygonVertices[2]);
                                    if (args[1].Length == 3)
                                    {
                                        double angleBetweenNormals = Math.Acos(Vector3.Dot(Vector3.Normalize(vertexNormals[int.Parse(args[1][2]) - 1]), Vector3.Normalize(triangle.Normal)));
                                        if (angleBetweenNormals > Math.PI / 2)
                                            triangle = new Triangle(polygonVertices[1], polygonVertices[0], polygonVertices[2]);
                                    }
                                    triangles.Add(triangle);

                                }
                                Triangle testTriangle;
                                if (i == 0)
                                {
                                    testTriangle = new Triangle(polygonVertices[polygonVertices.Count - 1], polygonVertices[i], polygonVertices[i + 1]);
                                }
                                else if (i == polygonVertices.Count - 1)
                                {
                                    testTriangle = new Triangle(polygonVertices[i - 1], polygonVertices[i], polygonVertices[0]);
                                }
                                else
                                {
                                    testTriangle = new Triangle(polygonVertices[i - 1], polygonVertices[i], polygonVertices[i + 1]);
                                }
                                bool isEar = true;
                                for (int j = 0; j < polygonVertices.Count; j++)
                                {
                                    if (j == i)
                                        continue;
                                    if (testTriangle.Contains(polygonVertices[j]))
                                        isEar = false;
                                }
                                if (isEar) 
                                {
                                    if (args[1].Length == 3)
                                    {
                                        double angleBetweenNormals = Math.Acos(Vector3.Dot(Vector3.Normalize(vertexNormals[int.Parse(args[1][2]) - 1]), Vector3.Normalize(testTriangle.Normal)));
                                        if (angleBetweenNormals > Math.PI / 2)
                                            testTriangle = new Triangle(testTriangle.Point2, testTriangle.Point1, testTriangle.Point3);
                                    }
                                    triangles.Add(testTriangle);
                                    polygonVertices.RemoveAt(i);
                                    i = 0;
                                }
                            }    
                        }
                    }    
                }
            }

            return triangles;
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

            //Scene[1].Rotation += new Vector3(0f, 0f, 50f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

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

        public Vector2 PositionToScreen(Vector3 position)
        {
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





            
            Vector3 cameraStart = Camera.Origin - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f); 
            Triangle cameraPlane = new Triangle(cameraStart, cameraStart + Camera.LateralAxis.Direction, cameraStart + Camera.LongitudinalAxis.Direction);
            Triangle cameraLateralPlane = new Triangle(cameraStart + Camera.Direction, cameraStart + Camera.LateralAxis.Direction, cameraStart - Camera.LateralAxis.Direction);
            Triangle cameraLateralPlane2 = new Triangle(cameraStart + Camera.Direction + Camera.LongitudinalAxis.Direction * 0.001f, cameraStart + Camera.LateralAxis.Direction + Camera.LongitudinalAxis.Direction * 0.001f, cameraStart - Camera.LateralAxis.Direction + Camera.LongitudinalAxis.Direction * 0.001f);
            Triangle cameraLongitudinalPlane = new Triangle(cameraStart + Camera.Direction, cameraStart + Camera.LongitudinalAxis.Direction, cameraStart - Camera.LongitudinalAxis.Direction);
            Triangle cameraLongitudinalPlane2 = new Triangle(cameraStart + Camera.Direction + Camera.LateralAxis.Direction * 0.001f, cameraStart + Camera.LongitudinalAxis.Direction + Camera.LateralAxis.Direction * 0.001f, cameraStart - Camera.LongitudinalAxis.Direction + Camera.LateralAxis.Direction * 0.001f);
            Vector3 cameraIntersection = cameraPlane.GetIntersection(new Ray(position, -Camera.Direction)) - cameraStart;
            Vector3 cameraXIntersection = cameraLateralPlane.GetIntersection(new Ray(cameraIntersection, Camera.LateralAxis.Direction));
            Vector3 cameraXIntersection2 = cameraLateralPlane2.GetIntersection(new Ray(cameraIntersection, Camera.LateralAxis.Direction));
            Vector3 cameraYIntersection = cameraLongitudinalPlane.GetIntersection(new Ray(cameraIntersection, Camera.LongitudinalAxis.Direction));
            Vector3 cameraYIntersection2 = cameraLongitudinalPlane2.GetIntersection(new Ray(cameraIntersection, Camera.LongitudinalAxis.Direction));
            float cameraXDistance = Vector3.Distance(cameraXIntersection, cameraIntersection);
            float cameraXDistance2 = Vector3.Distance(cameraXIntersection2, cameraIntersection);
            float cameraYDistance = Vector3.Distance(cameraYIntersection, cameraIntersection);
            float cameraYDistance2 = Vector3.Distance(cameraYIntersection2, cameraIntersection);
            Debug.WriteLine(cameraXDistance + " " + cameraXDistance2);
            return new Vector2((cameraXDistance < cameraXDistance2 ? -1 : 1) * cameraXDistance * _cameraSize * _pixelize, (cameraYDistance < cameraYDistance2 ? -1 : 1) * cameraYDistance * _cameraSize * _pixelize);
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

        public void Draw(SpriteBatch spriteBatch, bool perspective = false, float fov = 120f, Vector2 offset = new Vector2())
        {
            Vector3 cameraStart;
            if (!perspective)
                cameraStart = Camera.Origin - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f);
            else
                cameraStart = Camera.Origin + (Camera.Direction * (_width * _cameraSize / 2f) / (float)Math.Tan(fov * Math.PI / 360f)) - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f);
            Vector3 cameraStartPerspective = Camera.Origin + (Camera.Direction * (_width * _cameraSize / 2f) / (float)Math.Tan(_perspectiveFOV * Math.PI / 360f)) - (Camera.LongitudinalAxis.Direction * _height * _cameraSize / 2f) - (Camera.LateralAxis.Direction * _width * _cameraSize / 2f);
            int xMax = (int)Math.Ceiling((float)_width / _pixelize);
            int yMax = (int)Math.Ceiling((float)_height / _pixelize);

            spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X, (int)offset.Y, xMax * _pixelize, yMax * _pixelize), new Color(40, 40, 40));

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
                        for (int v = 0; v < Scene[i].Mesh.Count; v++)
                        {
                            Triangle triangle = Scene[i].Mesh[v];
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
                        float darken = (Vector3.Distance(cameraOrigin, closestTriangle.GetIntersection(pixelRay)) - 5f)/ 10f;
                        spriteBatch.Draw(_rectangle, new Rectangle((int)offset.X + (xMax - x - 1) * _pixelize, (int)offset.Y + (yMax - y - 1) * _pixelize, _pixelize, _pixelize), Color.Lerp(Color.Black, Color.White, darken));
                    }
                }
            }
            /*for (int i = 0; i < Scene.Count; i++)
            {
                if (Scene[i].Mesh == null) continue;
                for (int v = 0; v < Scene[i].Mesh.Count; v++)
                {
                    Triangle triangle = Scene[i].Mesh[v];
                    Vector2 Point1 = PositionToScreen(triangle.Point1);
                    Vector2 Point2 = PositionToScreen(triangle.Point2);
                    Vector2 Point3 = PositionToScreen(triangle.Point3);
                    System.Diagnostics.Debug.WriteLine(Point1 + " " + Point2 + " " + Point3);
                    DrawLine(spriteBatch, Point1, Point2, Color.Black, 20);
                    DrawLine(spriteBatch, Point2, Point3, Color.Black, 20);
                    DrawLine(spriteBatch, Point3, Point1, Color.Black, 20);
                    DrawLine(spriteBatch, new Vector2(50f, 50f), new Vector2(100f, 100f), Color.Black, 20);
                }
            }*/
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
