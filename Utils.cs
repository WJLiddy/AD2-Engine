using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

//A collection of utility methods and variables for the AD2 Engine.
public class Utils
{
    //Path to your assets folder. Essential for referring to resources by name.
    public static string PathToAssets;

    //A white rectangle so that drawing rectangles doesn't require loading sprites.
    private static Texture2D WhiteRect;

    //TODO: Random number generator.
    public static Random Random;

    //A default font for quick writing.
    public static PixelFont DefaultFont { get; private set; }

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

    public static void SetAssetDirectory(string relativePathToAssets)
    {
        Directory.SetCurrentDirectory(relativePathToAssets);
        Utils.PathToAssets = Directory.GetCurrentDirectory() + @"\";
    }
       
    public static void Load()
    {
        WhiteRect = Utils.TextureLoader(@"..\..\API\assets\rect.png");
        DefaultFont = new PixelFont(@"..\..\API\assets\spireFont.png");
        SoundManager.Load("sounds/");
        Random = new Random();
    }
    
    //An expensive operation to find the color blend bewtween two colors.
    //Use sparingly
    //REDO this. 
    public static Color Mix(float minDuration, float position, Color last, Color next)
    {
        float delta = position / minDuration;

        float R = ((last.R / 255f) * (1f - delta)) + (delta * (next.R / 255f));
        float G = ((last.G / 255f) * (1f - delta)) + (delta * (next.G / 255f));
        float B = ((last.B / 255f) * (1f - delta)) + (delta * (next.B / 255f));
        float A = ((last.A / 255f) * (1f - delta)) + (delta * (next.A / 255f));
        return new Color(R, G, B, A);
    }
    
    public static double Dist(int x1, int x2, int y1, int y2)
    {
        return (Math.Sqrt(((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2))));
    }

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
        } catch (XmlException)
        {
            Utils.Log("Xml File " + pathToXML + " is invalid.");
            return new Dictionary<string, LinkedList<string>>();
        }
        return allEntries;
    }
}


