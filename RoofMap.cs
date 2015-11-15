using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

// A Collision Map is a texture with associated collision pixels

// It has 3 layers:
// BaseMap, which is always drawn under everything
// CollisionMap, which is just true or false if there is a collision
// (Optional)AlwaysMap, which is always drawn all the time.
public class RoofMap : CollisionMap
{
    // The base layer.
    public Texture2D BaseMap;
    // An optional texture that is always drawn.
    private Texture2D AlwaysMap;

    // True if this part of the world can be collided with.
    private bool[,] Collision;

    // The width and the height of the game screen.
    // Needed for fast drawing (i.e., not rendering the whole map)
    private int ScreenWidth;
    private int ScreenHeight;

  
    public RoofMap(string pathToMapXML, int screenWidth, int screenHeight) : base(pathToMapXML,screenWidth, screenHeight)
    {
        this.ScreenWidth = screenWidth;
        this.ScreenHeight = screenHeight;
        Dictionary<string, LinkedList<string>> mapXML = Utils.GetXMLEntriesHash(pathToMapXML);

        // load in the two imporant layers. Also, collision data.
        Texture2D collisionMap;
        Color collisionKey;
        try
        {
            // load the texture of the "base" element.
            BaseMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMapXML) + @"\" + mapXML["base"].First.Value);
            // load the texture of the "collision" element.
            collisionMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMapXML) + @"\" + mapXML["collision"].First.Value);
            // load in the color key.
            collisionKey = new Color(Int32.Parse(mapXML["collisionKeyR"].First.Value), Int32.Parse(mapXML["collisionKeyG"].First.Value), Int32.Parse(mapXML["collisionKeyB"].First.Value));
        }
        catch
        {
            Utils.Log("CollisionMap " + pathToMapXML + " is missing an element or has a bad color value");
            return;
        }

        if (mapXML.ContainsKey("always"))
        {
            AlwaysMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMapXML) + @"\" + mapXML["always"].First.Value);
        }
        
        //fill the boolean array for colissions and walls.
        Collision = getCollisionArray(collisionMap, collisionKey);
    }

    public void drawBase(AD2SpriteBatch sb, int x, int y)
    {
       sb.Draw(BaseMap, new Rectangle(0, 0, ScreenWidth, ScreenHeight), new Rectangle(x, y, ScreenWidth, ScreenHeight), Color.White);
    }

    public void drawAlways(AD2SpriteBatch sb, int x, int y)
    {
        if (AlwaysMap == null)
            return;
        sb.Draw(AlwaysMap, new Rectangle(0, 0, ScreenWidth, ScreenHeight), new Rectangle(x, y, ScreenWidth, ScreenHeight), Color.White);
    }

    public bool collide(int x, int y)
    {
         return x < 0 || y < 0 || x >= BaseMap.Width || y >= BaseMap.Height || Collision[x, y];
    }

    public bool[,] getCollisionArray(Texture2D collisionMap, Color collisionKey)
    {
        //pull out the data.
        Color[] colorsMap = new Color[collisionMap.Width * collisionMap.Height];
        collisionMap.GetData<Color>(colorsMap);

        //get the collision array to return
        bool[,] collision = new bool[collisionMap.Width, collisionMap.Height];

        for (int x = 0; x != collisionMap.Width; x++)
        {
            for (int y = 0; y != collisionMap.Height; y++)
            {
                Color c = colorsMap[x + (y * collisionMap.Width)];
                collision[x, y] = c.Equals(collisionKey);
            }
        }
        return collision;
    } 
}