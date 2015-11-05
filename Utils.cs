using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;


//TODO: come back to this.

//A collection of utility methods and variables for the AD2 Engine.
public class Utils
{
    // TODO _ protections.The graphics device. Used to render stuff to the screen. 
    public static GraphicsDevice gfx;

    //TODO the sprite batch. needs a better name
    public static SpriteBatch sb;

    //Path to your assets folder. Essential for referring to resources by name.
    public static string pathToAssets;

    //A default font for quick writing.
    public static PixelFont defaultFont { get; private set; }

    //TODO better name. A sound manager. Use it to play musics and sound.
    public static SoundManager soundManager { get; private set; }

    //A white rectangle so that drawing rectangles doesn't require loading sprites.
    private static Texture2D whiteRect;

    //TODO: Random number generator.
    public static Random random;

    public struct Mix
    {
        public float delta;
        public Color last;
        public Color next;

        public Mix(float d, Color l, Color n)
        {
            delta = d;
            last = l;
            next = n;
        }

        public override bool Equals(Object m)
        {
            return ((Mix)m).delta == delta &&
            ((Mix)m).last.Equals(last) &&
            ((Mix)m).next.Equals(next);
        }

        public override int GetHashCode()
        {
            return ((int)(Int32.MaxValue * delta)/2) + (int)((last.PackedValue / 4)) + (int)((next.PackedValue / 4));
        }
    }

    public static Texture2D TextureLoader(String pathToTexture)
    {
 
        //TODO: Look for internal API, not project! for font and 
        System.IO.Stream stream = File.Open(Utils.pathToAssets + pathToTexture, FileMode.Open);
        Texture2D t =  Texture2D.FromStream(gfx, stream);
        stream.Close();
        return t;
    }

    public static void load()
    {
        whiteRect = Utils.TextureLoader(@"misc/rect.png");
        defaultFont = new PixelFont(@"misc/spireFont.png");
        random = new Random();
        soundManager = new SoundManager("sound\\");
    }

    public static void drawTexture(Texture2D t, int x, int y)
    {
        sb.Draw(t, new Rectangle(x, y, t.Bounds.Width, t.Bounds.Height), Color.White);   
    }

    public static void drawTexture(Texture2D t, int x, int y, Color c)
    {
        sb.Draw(t, new Rectangle(x, y, t.Bounds.Width, t.Bounds.Height), c);
    }

    public static void drawTexture(Texture2D t, int x, int y, int w, int h)
    {
        sb.Draw(t, new Rectangle(x, y, w, h), Color.White);
    }

    public static void drawTextureHFlip(Texture2D t, int x, int y)
    {
        sb.Draw(t, new Rectangle(x, y, t.Bounds.Width, t.Bounds.Height), new Rectangle(0,0, t.Bounds.Width, t.Bounds.Height), Color.White,0f,new Vector2(),SpriteEffects.FlipHorizontally,0f);
    }

    public static void drawTexture(Texture2D t, int x, int y, int w, int h, Color c)
    {
        sb.Draw(t, new Rectangle(x, y, w, h), c);
    }

    public static void drawTexture(object logo, int x, int y, int size1, int size2)
    {
        throw new NotImplementedException();
    }

    public static void drawRect(Color c, int x, int y, int w, int h)
    {
        Utils.drawTexture(whiteRect, x, y, w, h, c);
    }

    public static void drawString(string s, int x, int y, Color c, int scale = 1, bool outline = false)
    {
        defaultFont.draw(s, x,y, c, scale, outline);
    }
    
    //An expensive operation to find the color blend bewtween two colors.
    //Use sparingly
    public static Color mix(float minDuration, float position, Color last, Color next)
    {
        float delta = position / minDuration;

        float R = ((last.R / 255f) * (1f - delta)) + (delta * (next.R / 255f));
        float G = ((last.G / 255f) * (1f - delta)) + (delta * (next.G / 255f));
        float B = ((last.B / 255f) * (1f - delta)) + (delta * (next.B / 255f));
        float A = ((last.A / 255f) * (1f - delta)) + (delta * (next.A / 255f));
        return new Color(R, G, B, A);
    }
    

    public static double dist(int x1, int x2, int y1, int y2)
    {
        return (Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2))));
    }

    public static void log(String message)
    {
        System.Console.WriteLine("LOG: " + message);
    }

}


