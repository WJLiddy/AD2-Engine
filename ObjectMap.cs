using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

// A Map contains "objects" which have heights and therefore players can go in front/behind them.
// Ordered drawing is a must in this case, so
// Requires you to draw things on the screen in order of y.
public class ObjectMap : CollisionMap
{
    public class AD2Object
    {
        public Texture2D t;
        public int X, Y;
    }

    private LinkedList<AD2Object>[] objectRows;

    public ObjectMap(string pathToMap, int w, int h) : base (pathToMap,w,h)
    {
        Dictionary<string, LinkedList<string>> mapXML = Utils.GetXMLEntriesHash(pathToMap);
        Dictionary<string, LinkedList<string>> objectXML;
        
        try
        {
            // load the objects.
            objectXML = Utils.GetXMLEntriesHash(Path.GetDirectoryName(pathToMap) + @"\" + mapXML["object"].First.Value);
        }
        catch
        {
            Utils.Log("ObjectMap " + pathToMap + " failed to load object XML properly");
            return;
        }

        objectRows = new LinkedList<AD2Object>[BaseMap.Height];
        for(int i = 0; i != BaseMap.Height; i++)
        {
            objectRows[i] = new LinkedList<AD2Object>();
        }

       // try
        {
            LinkedList <string> allObjects = objectXML["object"];
            Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

            foreach(string s in allObjects)
            {
                string[] param = s.Split(',');
                AD2Object a = new AD2Object();
                param[0] = Path.GetDirectoryName(pathToMap) + "\\objects\\" + param[0];
                a.X = Int32.Parse(param[1]);
                a.Y = Int32.Parse(param[2]);
                if (!textureCache.ContainsKey(param[0]))
                    textureCache[param[0]] = Utils.TextureLoader(param[0]);
                a.t = textureCache[param[0]];

                objectRows[a.Y + (a.t.Height - 1)].AddFirst(a);
            }
        }
       // catch (Exception e)
        {
       //     Utils.Log("Some objects failed to load");
            
       //     return;
        }
        

    }

    public void DrawObjects(AD2SpriteBatch sb, int camX, int camY, int floorY)
    {
        foreach (AD2Object a in objectRows[floorY])
        {
            sb.DrawTexture(a.t, a.X + -camX, floorY + -(a.t.Height - 1) + -camY);
        }
    }
}


