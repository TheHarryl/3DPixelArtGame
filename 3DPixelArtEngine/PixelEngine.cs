using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        private int _pixelize;
        private float _cameraSize;

        private Texture2D _rectangle;

        public List<Mesh> Scene;

        public PixelEngine(GraphicsDevice graphicsDevice, int width, int height, int pixelize = 3, float cameraSize = 0.1f)
        {
            _rectangle = new Texture2D(graphicsDevice, 1, 1);
            Color[] data = new Color[1];
            data[0] = Color.White;
            _rectangle.SetData(data);

            Scene = new List<Mesh>();

            _width = width;
            _height = height;

            Camera = new Ray(new Vector3(0f, 0f, 0f), new Vector3(-1f, 0f, 0f));
            _pixelize = pixelize;
            _cameraSize = cameraSize;
        }

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(SpriteBatch spriteBatch, Vector2 offset = new Vector2())
        {
            Vector2 direction = Vector2.Normalize(new Vector2(Camera.Direction.Z, Camera.Direction.X));
            Vector3 cameraStart = Camera.Point - new Vector3(direction.X * _width * _cameraSize / 2f, 0f, direction.Y * _height * _cameraSize / 2f);
            for (int y = 0; y < Math.Ceiling((float)_height / _pixelize); y++)
            {
                for (int x = 0; x < Math.Ceiling((float)_width / _pixelize); x++)
                {
                    for (int i = 0; i < Scene.Count; i++)
                    {
                        for (int v = 0; v < Scene[i].Triangles.Count; v++)
                        {
                            Triangle triangle = Scene[i].Triangles[v];
                            Ray pixelRay = new Ray(cameraStart + new Vector3(direction.X * x * _cameraSize * _pixelize, 0f, direction.Y * y * _cameraSize * _pixelize), Camera.Direction);
                            if (triangle.Contains(pixelRay))
                            {
                                spriteBatch.Draw(_rectangle, new Rectangle(x * _pixelize, y * _pixelize, _pixelize, _pixelize), Color.White);
                            }
                        }
                    }
                }
            }
        }
    }
}
