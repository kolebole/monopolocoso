using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using SummerProject.ScreenManager;
using SummerProject.Screens;

namespace SummerProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MacroGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        private ScreenManager.ScreenManager screenManager;

        #endregion

        #region Initialization
        
        public MacroGame()
        {
            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            // Create the screen manager component.
            screenManager = new ScreenManager.ScreenManager(this);

            Components.Add(screenManager);
        }

        protected override void Initialize()
        {
            // Activate the first screens.
            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);

            base.Initialize();
        }
        
        #endregion

        #region Draw

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }

        #endregion

        #region Entry Point

        static void Main()
        {
            using (MacroGame game = new MacroGame())
            {
                game.Run();
            }
        }

        #endregion
    }
}
