using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

//A collection of static utility methods and variables for the AD2 Engine.
public class Utils
{
    //Path to the assets folder. Essential for referring to resources by name.
    public static string PathToAssets { get; private set; }

    //A default tiny font for writing.
    public static PixelFont DefaultFont { get; private set; }

    //Random number generator.
    private static Random Random;

    //A white rectangle so that drawing rectangles doesn't require loading sprites.
    private static Texture2D WhiteRect;

    //Loads a texture specified on the "assets/" folder in every ad2 engine game.
    public static Texture2D TextureLoader(String pathToTexture)
    {
        Texture2D t;
        try
        {
            Stream stream = File.Open(Utils.PathToAssets + pathToTexture, FileMode.Open);
            t = Texture2D.FromStream(Renderer.GraphicsDevice, stream);
            stream.Close();
        } catch 
        {
            Log("Something went wrong when trying to load " + pathToTexture);
            return WhiteRect;
        }
        return t;
    }

    //Set the path to the game assets. By default, this is "assets/", but you can change it.
    public static void SetAssetDirectory(string relativePathToAssets)
    {
        Directory.SetCurrentDirectory(relativePathToAssets);
        Utils.PathToAssets = Directory.GetCurrentDirectory() + @"\";
    }
       
    //Loads a basic rectangle, some text, some sounds. And a RNG.
    public static void Load()
    {
        WhiteRect = Utils.TextureLoader(@"..\..\API\assets\rect.png");
        DefaultFont = new PixelFont(@"..\..\API\assets\spireFont.xml");
        SoundManager.Load("sounds/");
        Random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);
    }
    
    //An expensive operation to find the color blend bewtween two colors.
    //Use sparingly
    public static Color Mix(float percentFirst, Color first, Color second)
    {
        float R = ((second.R / 255f) * (1f - percentFirst)) + (percentFirst * (first.R / 255f));
        float G = ((second.G / 255f) * (1f - percentFirst)) + (percentFirst * (first.G / 255f));
        float B = ((second.B / 255f) * (1f - percentFirst)) + (percentFirst * (first.B / 255f));
        float A = ((second.A / 255f) * (1f - percentFirst)) + (percentFirst * (first.A / 255f));
        return new Color(R, G, B, A);
    }
    
    //Distance between two integers
    public static double Dist(int x1, int x2, int y1, int y2)
    {
        return (Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2))));
    }

    //Log errors.
    public static void Log(String message)
    {
        Console.WriteLine("LOG: " + message);
    }

    // Return all elements from an XML, as a linked list hash
    public static Dictionary<string,LinkedList<string>> GetXMLEntriesHash(string pathToXML)
    {
        Dictionary<string, LinkedList<string>> allEntries = new Dictionary<string,LinkedList<string>>();
        try
        { 
            XmlReader rdr = XmlReader.Create(Utils.PathToAssets + pathToXML);
            rdr.Read();
            while (rdr.Read())
            {
                if (rdr.NodeType == XmlNodeType.Element)
                {
                    string key = rdr.LocalName;
                    string value = rdr.ReadElementContentAsString();
                    if (!allEntries.ContainsKey(key))
                    {
                        LinkedList<string> newList = new LinkedList<string>();
                        newList.AddLast(value);
                        allEntries.Add(key, newList);
                    }
                    else
                    {
                        allEntries[key].AddLast(value);
                    }
                }
            }
            rdr.Close();
        } catch
        {
            Log("Xml File " + pathToXML + " is invalid.");
            return new Dictionary<string, LinkedList<string>>();
        }
        return allEntries;
    }

    //return random number, 0 through 1.
    public static double randomNumber()
    {
        return Random.NextDouble();
    }

    public static void drawRect(AD2SpriteBatch sb, int x, int y, int w, int h, Color c)
    {
        sb.DrawTexture(WhiteRect, x, y, w, h, c);
    }
}


