using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Renderer
{
    //Graphics, for drawing cool stuff.
    //Graphics Device Manager is needed by a lot functions.
    public static GraphicsDeviceManager graphicsDeviceManager;

    //Graphics device to use.
    public static GraphicsDevice graphicsDevice;

    //The streching we do to fill the screen with the game.
    //TODO: private?
    public static Matrix matrixScale;

    //Draw parameters
    public static int drawScaleX = 1;
    public static int drawScaleY = 1;
    public static int drawXOff = 0;
    public static int drawYOff = 0;
    public static int baseHeight = 0;
    public static int baseWidth = 0;

    //some basic resolution options
   public enum Resolution
    {
        //full screen, bars on the side so exact pixel values are preserved
        FULL_SCREEN_ANTI_ALIAS,
        //full screen with strectching
        FULL_SCREEN,
        //windowed, but scaled up.
        WINDOWED_LARGE,
        //windowed, no scaling
        WINDOWED
    }

    public static Resolution resolution;

    public static void setResolution(int baseWidthi, int baseHeighti)
    {
        baseWidth = baseWidthi;
        baseHeight = baseHeighti;
        configureResolution();
        //force vsync?
        //graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
    }

    private static void configureResolution()
    {
        graphicsDeviceManager.IsFullScreen = (resolution == Resolution.FULL_SCREEN) || (resolution == Resolution.FULL_SCREEN_ANTI_ALIAS);

        int resWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int resHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        if (!(graphicsDeviceManager.IsFullScreen))
        {
            //Make the window as big as possible while remaining a multiple. 
            //BUT: Subtract one, because of that pesky windows bar.

            if (resolution == Resolution.WINDOWED_LARGE)
            {
                //find the dimension that is most limiting.
                int maxScale = System.Math.Min(resHeight / baseHeight, resWidth / baseWidth);
                //don't fill the whole screen, only most.
                drawScaleX = System.Math.Max(maxScale - 1, 1);
                drawScaleY = drawScaleX;
            }
            else
            {
                drawScaleX = 1;
                drawScaleY = 1;
            }

            graphicsDeviceManager.PreferredBackBufferHeight = baseHeight * drawScaleY;
            graphicsDeviceManager.PreferredBackBufferWidth = baseWidth * drawScaleX;
        }
        else if ((resolution == Resolution.FULL_SCREEN))
        {
            //Simply make the window as big as possible while remaining a multiple.
            drawScaleX = System.Math.Min(resHeight / baseHeight, resWidth / baseWidth);
            drawScaleY = drawScaleX;

            //now, we actually do want to set the dimentions correctly.
            graphicsDeviceManager.PreferredBackBufferHeight = resHeight;
            graphicsDeviceManager.PreferredBackBufferWidth = resWidth;

            //now, find offsets.
            drawXOff = (resWidth - (baseWidth * drawScaleY)) / 2;
            drawYOff = (resHeight - (baseHeight * drawScaleX)) / 2;

        }
        else
        {

            //full screen, anti-alias
            //boy this is gonna be ugly but you asked for it.
            drawScaleY = 1;
            drawScaleX = 1;
        }

        //now that we know the resolution, make the matrix for it for scaling later.
        matrixScale = Matrix.Identity;
        matrixScale.M11 = drawScaleX;
        matrixScale.M22 = drawScaleY;
        matrixScale.Translation = new Vector3(drawXOff, drawYOff, 0);

        graphicsDeviceManager.ApplyChanges();
    }

   //public void drawRect(Color c, int x, int y, int w, int h)
   // {
   //     Utils.drawTexture(whiteRect, x, y, w, h, c);
   // }

}
