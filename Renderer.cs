using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Renderer
{
    //Graphics, for drawing cool stuff.
    //Graphics Device Manager is needed by a lot functions.
    public static GraphicsDeviceManager graphicsDeviceManager;

    public static GraphicsDevice graphicsDevice;

    //A Spritebatch, that gets rendered to the screen.
    public SpriteBatch primarySpriteBatch { get; private set; }

    //A default font for quick writing.
    public PixelFont defaultFont { get; private set; }


    //Draw parameters
    int drawScaleX = 1;
    int drawScaleY = 1;
    int drawXOff= 0;
    int drawYOff = 0;
    int baseHeight = 0;
    int baseWidth = 0;
    Matrix matrixScale;

    public Renderer(int baseWidth, int baseHeight)
    {
        primarySpriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);
        this.baseWidth = baseWidth;
        this.baseHeight = baseHeight;
        configureResolution(false,false);

        //force vsync?
        //graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
    }

    private void configureResolution(bool fullscreen, bool fullscreenWithAntiAlias)
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
        matrixScale.M11 = drawScaleX; matrixScale.M22 = drawScaleY;
        matrixScale.Translation = new Vector3(drawXOff, drawYOff, 0);

        graphicsDeviceManager.ApplyChanges();
    }

    public void startDraw()
    {  //nuke old graphics.
        graphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

        //set the spritebatch to start.
        primarySpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, matrixScale);
    }

    public void endDraw()
    {
        primarySpriteBatch.End();
    }

    public static void drawTexture(SpriteBatch sb, Texture2D t, int x, int y)
    {
        sb.Draw(t, new Rectangle(x, y, t.Bounds.Width, t.Bounds.Height), Color.White);
    }

    public static void drawTexture(SpriteBatch sb, Texture2D t, int x, int y, Color c)
    {
        sb.Draw(t, new Rectangle(x, y, t.Bounds.Width, t.Bounds.Height), c);
    }

    public static void drawTexture(SpriteBatch sb, Texture2D t, int x, int y, int w, int h)
    {
        sb.Draw(t, new Rectangle(x, y, w, h), Color.White);
    }

    public static void drawTextureHFlip(SpriteBatch sb, Texture2D t, int x, int y)
    {
        sb.Draw(t, new Rectangle(x, y, t.Bounds.Width, t.Bounds.Height), new Rectangle(0, 0, t.Bounds.Width, t.Bounds.Height), Color.White, 0f, new Vector2(), SpriteEffects.FlipHorizontally, 0f);
    }

    public static void drawTexture(SpriteBatch sb, Texture2D t, int x, int y, int w, int h, Color c)
    {
        sb.Draw(t, new Rectangle(x, y, w, h), c);
    }


 //   public void drawRect(Color c, int x, int y, int w, int h)
  //  {
   //     Utils.drawTexture(whiteRect, x, y, w, h, c);
    //}

    public void drawString(SpriteBatch sb, string s, int x, int y, Color c, int scale = 1, bool outline = false)
    {
        defaultFont.draw(sb, s, x, y, c, scale, outline);
    }
}
