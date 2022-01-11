using _3DPixelArtEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Vector3 = System.Numerics.Vector3;
using System;
using System.IO;
using Object = _3DPixelArtEngine.Object;
using System.Diagnostics;

namespace _3DPixelArtGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private PixelEngine _pixelEngine;

        private string path;
        private string objPath;

        private int frameCount = 0;
        private DateTime startFrameCount = DateTime.Now;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _pixelEngine = new PixelEngine(_graphics.GraphicsDevice, 800, 480, 4);

            path = Directory.GetCurrentDirectory();
            objPath = Directory.GetParent(Directory.GetParent(Directory.GetParent(path).ToString()).ToString()).ToString();

            Object testObject = new Object();
            testObject.Mesh = new Mesh(new List<Triangle>()
            {
                new Triangle(new Vector3(0f, 2f, 0f), new Vector3(3f, -2f, -2f), new Vector3(0f, -2f, 5f)),
                new Triangle(new Vector3(0f, 2f, 0f), new Vector3(0f, -2f, 5f), new Vector3(3f, -2f, -2f)),
            });
            Object testCube = new Object();
            testCube.Mesh = new Mesh(objPath + "/dodecahedron.obj");
            _pixelEngine.Scene.Add(testObject);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _pixelEngine.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            // TODO: Add your drawing code here
            _pixelEngine.Render();

            _pixelEngine.Draw(_spriteBatch);

            base.Draw(gameTime);

            _spriteBatch.End();

            frameCount++;
            if (DateTime.Now - startFrameCount >= new TimeSpan(0, 0, 1))
            {
                Debug.WriteLine(frameCount + " FPS");
                frameCount = 0;
                startFrameCount = DateTime.Now;
            }

        }
    }
}
