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

    public static void setResolution(int baseWidthi, int baseHeighti)
    {
        baseWidth = baseWidthi;
        baseHeight = baseHeighti;
        configureResolution(false,false);

        //force vsync?
        //graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
    }

    private static void configureResolution(bool fullscreen, bool fullscreenWithAntiAlias)
    {
        graphicsDeviceManager.IsFullScreen = fullscreen;

        int resWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int resHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        if (!fullscreen)
        {
            //Simply make the window as big as possible while remaining a multiple. Easy.
            drawScaleX = System.Math.Min(resHeight / baseHeight, resWidth / baseWidth);
            drawScaleY = drawScaleX;

            graphicsDeviceManager.PreferredBackBufferHeight = baseHeight * drawScaleY;
            graphicsDeviceManager.PreferredBackBufferWidth = baseWidth * drawScaleX;
        }

        else if (!fullscreenWithAntiAlias)
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

            //full screen, no alias
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
