using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Little_Might.Utils
{
    internal class ResolutionHandler
    {
        public static int WindowWidth;
        public static int WindowHeight;

        public static void ChangeResolution(GraphicsDeviceManager graphics, int width, int height, bool fullscreen = false, GraphicsDevice graphicsDevice = null)
        {
            if (!fullscreen)
            {
                graphics.PreferredBackBufferWidth = width;
                graphics.PreferredBackBufferHeight = height;
            }
            else
            {
                graphics.PreferredBackBufferWidth = graphicsDevice.Adapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferHeight = graphicsDevice.Adapter.CurrentDisplayMode.Height;
                graphics.ToggleFullScreen();
            }

            WindowHeight = graphics.PreferredBackBufferHeight;
            WindowWidth = graphics.PreferredBackBufferWidth;

            graphics.ApplyChanges();
        }
    }
}
