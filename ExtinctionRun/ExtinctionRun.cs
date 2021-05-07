using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ExtinctionRun.Screens;
using ExtinctionRun.StateManagement;

namespace ExtinctionRun
{
    public class ExtinctionRun : Game
    {
        private GraphicsDeviceManager _graphics;
        private readonly ScreenManager _screenManager;

        public ExtinctionRun()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;

            var screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), screenFactory);

            _screenManager = new ScreenManager(this);
            Components.Add(_screenManager);

            AddInitialScreens();
        }

        private void AddInitialScreens()
        {
            _screenManager.AddScreen(new BackgroundScreen(), null);
            _screenManager.AddScreen(new MainMenuScreen(), null);
        }

        protected override void Initialize()
        {
            Window.Title = Constants.Title;
            _graphics.PreferredBackBufferWidth = Constants.GameWidth;
            _graphics.PreferredBackBufferHeight = Constants.GameHeight;
            _graphics.ApplyChanges();
            base.Initialize();
        }

        protected override void LoadContent() { }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);    // The real drawing happens inside the ScreenManager component
        }
    }
}
