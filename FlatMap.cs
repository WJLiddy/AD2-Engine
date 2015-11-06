using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Xml;

//A FlatMap is a 2d (non-isometric) map. It has some basic features such as collision masks and roof occlusion.
//It has 3 layers:
//BaseMap, which is always drawn under everything
//RoofMap, which is an option that allows conditional line of sight (for example, if you didn't want the player to see around corners)
//AlwaysMap, which is always drawn all the time.

//TODO: leave off optional parameters. don't walys need roofs
//TODO : should not need to pass screen width or height
//TODO : get access to map size!
public class FlatMap
{
    Texture2D baseMap;
    Texture2D roofMap;
    Texture2D alwaysMap;

    //These handle the drawing of individual roofs.
    RenderTarget2D roofs;
    SpriteBatch roofBatch;

    //true if this part of the world can be collided with.
    bool[,] collision;
    //true if this part of the world is a wall (and as a result, has roof occlusion)
    public bool[,] wall;

    int screenWidth;
    int screenHeight;

    public class FlatMapXML
    {
        //name of the basemap png.
        public string baseName;
        //name of the always-draw png.
        public string alwaysName;
        //name of the roof png.
        public string roofName;

        //name of the collision map png.
        public string collisionName;
        //keys for collisions and walls.
        public Color collisionKey;
        public Color wallKey;
        
    }


    public FlatMap(string pathToMap, int screenWidth, int screenHeight)
    {
        this.screenHeight = screenHeight;
        this.screenWidth = screenWidth;
        FlatMapXML mapXML  = readMapXML(pathToMap);

        //load in the three layers.
        baseMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + mapXML.baseName);
        alwaysMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" +  mapXML.alwaysName);
        roofMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + mapXML.roofName);

        //Load in the collision map.
        Texture2D collisionMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + mapXML.collisionName);

        //fill the boolean array for colissions and walls.
        fillCollisionAndWallArray(collisionMap, mapXML.collisionKey, mapXML.wallKey);
        
        //get the roof map.
        roofs = new RenderTarget2D(Renderer.graphicsDevice, screenWidth, screenHeight);
        roofBatch = new SpriteBatch(Renderer.graphicsDevice);

    }

    public void drawBase(AD2SpriteBatch sb, int x, int y, int w, int h)
    {
        sb.Draw(baseMap, new Rectangle(0, 0, w, h), new Rectangle(x, y, w, h), Color.White);
    }

    public void drawBase(AD2SpriteBatch sb, int x, int y)
    {
       sb.Draw(baseMap, new Rectangle(0, 0, screenWidth, screenHeight), new Rectangle(x, y, screenWidth, screenHeight), Color.White);
    }

    public void drawAlways(AD2SpriteBatch sb, int x, int y, int w, int h)
    {
        sb.Draw(alwaysMap, new Rectangle(0, 0, w, h), new Rectangle(x, y, w, h), Color.White);
    }

    public void drawAlways(AD2SpriteBatch sb, int x, int y)
    {
        sb.Draw(alwaysMap, new Rectangle(0, 0, screenWidth, screenHeight), new Rectangle(x, y, screenWidth, screenHeight), Color.White);
    }

    public bool collide(int x, int y)
    {
         return x < 0 || y < 0 || x >= baseMap.Width || y >= baseMap.Height || collision[x, y];
    }

    public void fillCollisionAndWallArray(Texture2D collisionMap, Color collisionKey, Color wallKey)
    {
        //pull out the data.
        Color[] colorsMap = new Color[collisionMap.Width * collisionMap.Height];
        collisionMap.GetData<Color>(colorsMap);

        //get the collision to return
        collision = new bool[collisionMap.Width, collisionMap.Height];
        wall = new bool[collisionMap.Width, collisionMap.Height];

        for (int x = 0; x != collisionMap.Width; x++)
        {
            for (int y = 0; y != collisionMap.Height; y++)
            {
                Color c = colorsMap[x + (y * collisionMap.Width)];
                collision[x, y] = c.Equals(wallKey) || c.Equals(collisionKey);
                wall[x, y] = c.Equals(wallKey);
            }
        }
    }

    public FlatMapXML readMapXML(String pathToMap)
    {
        FlatMapXML pmx = new FlatMapXML();
        XmlReader reader = XmlReader.Create(pathToMap);

        reader.ReadToFollowing("base");
        pmx.baseName = reader.ReadElementContentAsString();

        reader.ReadToFollowing("collision");
        pmx.collisionName = reader.ReadElementContentAsString();

        reader.ReadToFollowing("always");
        pmx.alwaysName = reader.ReadElementContentAsString();

        reader.ReadToFollowing("roof");
        pmx.roofName = reader.ReadElementContentAsString();

        reader.ReadToFollowing("collisionKeyR");
        string collisionR = reader.ReadElementContentAsString();
        reader.ReadToFollowing("collisionKeyG");
        string collisionG = reader.ReadElementContentAsString();
        reader.ReadToFollowing("collisionKeyB");
        string collisionB = reader.ReadElementContentAsString();
        pmx.collisionKey = new Color(Int32.Parse(collisionR), Int32.Parse(collisionG), Int32.Parse(collisionB));

        reader.ReadToFollowing("wallKeyR");
        string wallR = reader.ReadElementContentAsString();
        reader.ReadToFollowing("wallKeyG");
        string wallG = reader.ReadElementContentAsString();
        reader.ReadToFollowing("wallKeyB");
        string wallB = reader.ReadElementContentAsString();
        pmx.wallKey = new Color(Int32.Parse(wallR), Int32.Parse(wallG), Int32.Parse(wallB));

        reader.Close();
        return pmx;
    }

    //renders roofs based on passed-in los array
    public void renderRoofs(AD2SpriteBatch sb,Texture2D los, int camX, int camY, int w, int h)
    {
        //clear the roof map to completely opaque
        Renderer.graphicsDevice.SetRenderTarget(roofs);
        Renderer.graphicsDevice.Clear(new Color(0, 0, 0, 1f));
      
        roofBatch.Begin(SpriteSortMode.Deferred, getRoofBlendState(), null, null, null);

        //draw the line of sight map over the roof.
        roofBatch.Draw(los, new Rectangle(0, 0, w, h), new Rectangle(0, 0, w, h), Color.White);
        roofBatch.Draw(roofMap, new Rectangle(0, 0, w, h), new Rectangle(camX, camY, w, h), Color.White);

        roofBatch.End();
        Renderer.graphicsDevice.SetRenderTarget(null);

        //finally, render to spritebatch.
        sb.drawTexture(roofs, 0, 0);
    }

    //renders roofs based on passed-in los array
    public void renderRoofs(AD2SpriteBatch sb, Texture2D los, int camX, int camY)
    {
        renderRoofs(sb, los, camX, camY, screenWidth, screenHeight);
    }

    //Does an OR with zeros: any pixel with an alpha of 0 is transparent no matter how many are piled on top
    public BlendState getRoofBlendState()
    {
        BlendState bl = new BlendState();

        //Once I am zero, i stay zero.
        bl.AlphaDestinationBlend = Blend.Zero;
        bl.AlphaSourceBlend = Blend.DestinationAlpha;

        bl.ColorSourceBlend = Blend.DestinationAlpha;
        bl.ColorDestinationBlend = Blend.Zero;

        return bl;
    }
}