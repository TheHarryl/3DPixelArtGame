using _3DPixelArtEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Vector3 = System.Numerics.Vector3;

namespace _3DPixelArtGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private PixelEngine _pixelEngine;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _pixelEngine = new PixelEngine(_graphics.GraphicsDevice, 800, 480);

            Object testObject = new Object();
            testObject.Mesh = new List<Triangle>()
            {
                new Triangle(new Vector3(0f, 2f, 0f), new Vector3(3f, -2f, -2f), new Vector3(0f, -2f, 5f))
            };

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

            _spriteBatch.Begin();

            // TODO: Add your drawing code here
            _pixelEngine.DrawPerspective(_spriteBatch);

            base.Draw(gameTime);

            _spriteBatch.End();
        }
    }
}
