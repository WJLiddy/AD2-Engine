using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Renderer
{
    //Graphics, for drawing cool stuff.
    //Graphics Device Manager is needed by a lot functions.
    public static GraphicsDeviceManager GraphicsDeviceManager;

    //Graphics device to use.
    public static GraphicsDevice GraphicsDevice;

    //The streching we do to fill the screen with the game.
    //TODO: private?
    public static Matrix MatrixScale;

    //Draw parameters
    public static int DrawScaleX = 1;
    public static int DrawScaleY = 1;
    public static int DrawXOff = 0;
    public static int DrawYOff = 0;
    public static int BaseHeight = 0;
    public static int BaseWidth = 0;

    //some basic resolution options
   public enum ResolutionType
    {
        //full screen, bars on the side so exact pixel values are preserved
        FullScreenAntiAlias,
        //full screen with strectching
        FullScreen,
        //windowed, but scaled up.
        WindowedLarge,
        //windowed, no scaling
        Windowed
    }

    public static ResolutionType Resolution;

    public static void setResolution(int baseWidthi, int baseHeighti)
    {
        BaseWidth = baseWidthi;
        BaseHeight = baseHeighti;
        ConfigureResolution();
        //force vsync?
        //graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
    }

    private static void ConfigureResolution()
    {
        bool fullscreen = (Resolution == ResolutionType.FullScreen) || (Resolution == ResolutionType.FullScreenAntiAlias);
        GraphicsDeviceManager.IsFullScreen = fullscreen;

        int resWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int resHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        if (!fullscreen)
        {
            Utils.Log("Starting in a window.");
            //Make the window as big as possible while remaining a multiple. 
            //BUT: Subtract one, because of that pesky windows bar.

            if (Resolution == ResolutionType.WindowedLarge)
            {
                //find the dimension that is most limiting.
                int maxScale = System.Math.Min(resHeight / BaseHeight, resWidth / BaseWidth);
                //don't fill the whole screen, only most.
                DrawScaleX = System.Math.Max(maxScale - 1, 1);
                DrawScaleY = DrawScaleX;
            }
            else
            {
                DrawScaleX = 1;
                DrawScaleY = 1;
            }

            GraphicsDeviceManager.PreferredBackBufferHeight = BaseHeight * DrawScaleY;
            GraphicsDeviceManager.PreferredBackBufferWidth = BaseWidth * DrawScaleX;
        }
        else if ((Resolution == ResolutionType.FullScreen))
        {
            Utils.Log("Starting fullscreen.");
            //Simply make the window as big as possible while remaining a multiple.
            DrawScaleX = System.Math.Min(resHeight / BaseHeight, resWidth / BaseWidth);
            DrawScaleY = DrawScaleX;

            //now, we actually do want to set the dimentions correctly.
            GraphicsDeviceManager.PreferredBackBufferHeight = resHeight;
            GraphicsDeviceManager.PreferredBackBufferWidth = resWidth;

            //now, find offsets.
            DrawXOff = (resWidth - (BaseWidth * DrawScaleY)) / 2;
            DrawYOff = (resHeight - (BaseHeight * DrawScaleX)) / 2;

        }
        else
        {
            Utils.Log("Starting fullscreen with anti-alias");
            //full screen, anti-alias
            //boy this is gonna be ugly because of strectching
            DrawScaleX = resWidth / BaseWidth;
            DrawScaleY = resHeight / BaseHeight;

            //now, we actually do want to set the dimentions correctly.
            GraphicsDeviceManager.PreferredBackBufferHeight = resHeight;
            GraphicsDeviceManager.PreferredBackBufferWidth = resWidth;

        }

        //now that we know the resolution, make the matrix for it for scaling later.
        MatrixScale = Matrix.Identity;
        MatrixScale.M11 = DrawScaleX;
        MatrixScale.M22 = DrawScaleY;
        MatrixScale.Translation = new Vector3(DrawXOff, DrawYOff, 0);

        GraphicsDeviceManager.ApplyChanges();
    }
}
