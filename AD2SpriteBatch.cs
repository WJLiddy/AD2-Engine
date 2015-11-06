using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

//Provides a nice set of shortcuts for drawing plain textures.
public class AD2SpriteBatch : SpriteBatch
{
    public AD2SpriteBatch(GraphicsDevice g) : base(g)
    {
        //lol constructor sub
    }
   
    public void drawTexture(Texture2D t, int x, int y)
    {
        Draw(t, new Rectangle(x, y, t.Bounds.Width, t.Bounds.Height), Color.White);
    }

    public void drawTexture(Texture2D t, int x, int y, Color c)
    {
        Draw(t, new Rectangle(x, y, t.Bounds.Width, t.Bounds.Height), c);
    }

    public void drawTexture(Texture2D t, int x, int y, int w, int h)
    {
       Draw(t, new Rectangle(x, y, w, h), Color.White);
    }

    public void drawTexture(Texture2D t, int x, int y, int w, int h, Color c)
    {
        Draw(t, new Rectangle(x, y, w, h), c);
    }

    public void drawTextureHFlip(Texture2D t, int x, int y)
    {
       Draw(t, new Rectangle(x, y, t.Bounds.Width, t.Bounds.Height), new Rectangle(0, 0, t.Bounds.Width, t.Bounds.Height), Color.White, 0f, new Vector2(), SpriteEffects.FlipHorizontally, 0f);
    }

}
