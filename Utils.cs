using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;


//TODO: come back to this.

//A collection of utility methods and variables for the AD2 Engine.
public class Utils
{
    //Path to your assets folder. Essential for referring to resources by name.
    public static string pathToAssets;

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
        Texture2D t =  Texture2D.FromStream(Renderer.graphicsDevice, stream);
        stream.Close();
        return t;
    }

    //TODO setDirectory
    public static void setAssetDirectory(string relativePathToAssets)
    {
        Directory.SetCurrentDirectory(relativePathToAssets);
        Utils.pathToAssets = Directory.GetCurrentDirectory() + @"\";
    }
       

    public static void load()
    {
        //TODO only works with debug
        //Assume that assets is in the usual location.
        //We can now read from this directory

        //TODO wrong location
      //  whiteRect = Utils.TextureLoader(@"misc/rect.png");
        random = new Random();
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

    // Return all elements from an XML, as a hash
    public static Dictionary<string,string> getXMLEntriesHash(string pathToXML)
    {
        Dictionary<string, string> allEntries = new Dictionary<string,string>();
        XmlReader rdr = XmlReader.Create(Utils.pathToAssets + pathToXML);
        rdr.Read();
        while (rdr.Read())
        { 
            if (rdr.NodeType == XmlNodeType.Element)
            {
                string key = rdr.LocalName;
                string value = rdr.ReadElementContentAsString();
                allEntries.Add(key, value);
            }
        }
        rdr.Close();
        return allEntries;
    }
}


