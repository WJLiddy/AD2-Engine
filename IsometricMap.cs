using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Xml;

//A Map contains 
public class Map
{
    Texture2D baseMap;
    Texture2D alwaysMap;
    Texture2D roofMap;

    RenderTarget2D roofs;
    SpriteBatch roofBatch;

    bool[,] collision;
   public bool[,] wall;

    public static readonly int MAX_OBJECT_HEIGHT = 90;

    public class ParsedMapXML
    {
        public string baseName;
        public string collisionName;
        public string objectMask;
        public string objectLayer;
        public string always;
        public string roof;
        public Color collisionKey;
        public Color wallKey;
        public Color baseKey;
        public Color yKey;

    }

    public class MapObjectLine
    {
        public RenderTarget2D t;
        public int w;
        public int h;
    }

    MapObjectLine[] allObjects;

    public Map(string pathToMap, int w, int h)
    {
        ParsedMapXML pmx  = readMapXML(pathToMap);

        baseMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + pmx.baseName);
        alwaysMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + pmx.always);
        roofMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + pmx.roof);

        Texture2D collisionMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + pmx.collisionName);
        Color[] collisionColorsMap = new Color[collisionMap.Width * collisionMap.Height];
        collisionMap.GetData<Color>(collisionColorsMap);

        createCollisionMap(collisionMap, collisionColorsMap, pmx.collisionKey, pmx.wallKey);

        Texture2D objectMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + pmx.objectLayer);

        Color[] objectMapColor = new Color[objectMap.Width * objectMap.Height];
        objectMap.GetData<Color>(objectMapColor);

        Texture2D objectMaskTexture = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + pmx.objectMask);

        Color[] objectMaskColor = new Color[objectMap.Width * objectMap.Height];
        objectMaskTexture.GetData<Color>(objectMaskColor);

        createObjectMap(objectMapColor, objectMaskColor, pmx.baseKey, pmx.yKey);

        roofs = new RenderTarget2D(Renderer.graphicsDevice, w, h);
        roofBatch = new SpriteBatch(Renderer.graphicsDevice);

    }

    public void drawBase(SpriteBatch sb, int x, int y, int w, int h)
    {
        sb.Draw(baseMap, new Rectangle(0, 0, w, h), new Rectangle(x, y, w, h), Color.White);
    }

    public void drawAlways(SpriteBatch sb, int x, int y, int w, int h)
    {
        sb.Draw(alwaysMap, new Rectangle(0, 0, w, h), new Rectangle(x, y, w, h), Color.White);
    }

    public void drawObjectLine(SpriteBatch sb, int x, int y, int w, int h, int step)
    {
        if (step + y >= 0 && step + y < (baseMap.Height))
        {
            if (allObjects[step + y] != null)
            {
                MapObjectLine l = allObjects[step + y];
                 sb.Draw(l.t, new Rectangle(-x, -y + (y + step) +- (l.h - 1), l.w, l.h), new Rectangle(0, 0, l.w, l.h), Color.White);
            }
        }
    }

    public bool collide(int x, int y)
    {
         return x < 0 || y < 0 || x >= baseMap.Width || y >= baseMap.Height || collision[x, y];
    }

    public void createCollisionMap(Texture2D collisionMap, Color[] colorsMap, Color collisionKey, Color wallKey)
    {
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

    //Create a list of objects to render
    //TODO: Refactor
    public void createObjectMap(Color[] objectLayer, Color[] objectMask, Color baseKey, Color yKey)
    { 
        bool[,] objectBaseMask = new bool[baseMap.Width, baseMap.Height];
        bool[,] objectHeightMask = new bool[baseMap.Width, baseMap.Height];

        populateBaseAndHeightMaps(objectMask, objectBaseMask, baseKey, objectHeightMask, yKey);

        allObjects = new MapObjectLine[baseMap.Height];
        //Now we have the base mask and height mask figured out. We need to find all of the bases, climb up the height, and map them to objects.
        for (int y = 0; y != baseMap.Height; y++)
        {

            allObjects[y] = new MapObjectLine();
            allObjects[y].w = baseMap.Width;
            allObjects[y].h = MAX_OBJECT_HEIGHT;
            allObjects[y].t = new RenderTarget2D(Renderer.graphicsDevice, baseMap.Width, MAX_OBJECT_HEIGHT);

            SpriteBatch objRender = new SpriteBatch(Renderer.graphicsDevice);
            Renderer.graphicsDevice.SetRenderTarget(allObjects[y].t);
            Renderer.graphicsDevice.Clear(Color.Transparent);

            objRender.Begin();
            bool drewSomething = false;

            for (int x = 0; x != baseMap.Width ; x++)
            {
                if (objectBaseMask[x, y])
                {
                    drewSomething = true;
                    int h = 1;
                    while (objectHeightMask[x, y - h])
                    {
                        h++;
                    }

                    Texture2D newObject = new Texture2D(Renderer.graphicsDevice, 1, h);
                    Color[] colorData = new Color[h];

                    for (int dh = 0; dh != h; dh++)
                    {
                        //we know the object is 'h' pixels high from the base.
                        //we also know that the top is at y = 0.
                        colorData[dh] = objectLayer[x + ((y + -((h - 1) - dh)) * baseMap.Width)];
                    }

                    newObject.SetData<Color>(colorData);
                    objRender.Draw(newObject, new Rectangle(x, (allObjects[y].h - 1) - (h - 1), 1, h), new Rectangle(0, 0, 1, h), Color.White);
                }
            }
            objRender.End();
            if (!drewSomething)
                allObjects[y] = null;

        }
    }

    public ParsedMapXML readMapXML(String pathToMap)
    {
        ParsedMapXML pmx = new ParsedMapXML();
        XmlReader reader = XmlReader.Create(pathToMap);

        reader.ReadToFollowing("base");
        pmx.baseName = reader.ReadElementContentAsString();

        reader.ReadToFollowing("collision");
        pmx.collisionName = reader.ReadElementContentAsString();

        reader.ReadToFollowing("objectmask");
        pmx.objectMask = reader.ReadElementContentAsString();

        reader.ReadToFollowing("objectlayer");
        pmx.objectLayer = reader.ReadElementContentAsString();

        reader.ReadToFollowing("always");
        pmx.always = reader.ReadElementContentAsString();

        reader.ReadToFollowing("roof");
        pmx.roof = reader.ReadElementContentAsString();

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

        reader.ReadToFollowing("objectBaseKeyR");
        string objectBaseKeyR = reader.ReadElementContentAsString();
        reader.ReadToFollowing("objectBaseKeyG");
        string objectBaseKeyG = reader.ReadElementContentAsString();
        reader.ReadToFollowing("objectBaseKeyB");
        string objectBaseKeyB = reader.ReadElementContentAsString();
        pmx.baseKey = new Color(Int32.Parse(objectBaseKeyR), Int32.Parse(objectBaseKeyG), Int32.Parse(objectBaseKeyB));

        reader.ReadToFollowing("objectHeightKeyR");
        string objectHeightKeyR = reader.ReadElementContentAsString();
        reader.ReadToFollowing("objectHeightKeyG");
        string objectHeightKeyG = reader.ReadElementContentAsString();
        reader.ReadToFollowing("objectHeightKeyB");
        string objectHeightKeyB = reader.ReadElementContentAsString();
        pmx.yKey = new Color(Int32.Parse(objectHeightKeyR), Int32.Parse(objectHeightKeyG), Int32.Parse(objectHeightKeyB));

        reader.Close();
        return pmx;
    }

    //Copys Base and Height Map from bitmaps
    private void populateBaseAndHeightMaps(Color[] objectMask, bool[,] objectBaseMask, Color baseKey, bool[,] objectHeightMask, Color yKey)
    {
        for (int x = 0; x != baseMap.Width; x++)
        {
            for (int y = 0; y != baseMap.Height; y++)
            {
                Color c = objectMask[x + (y * baseMap.Width)];
                objectBaseMask[x, y] = c.Equals(baseKey);
                objectHeightMask[x, y] = c.Equals(yKey);
            }
        }
    }

    public void renderRoofs(SpriteBatch sb, Texture2D los, int camX, int camY, int w, int h)
    {

        Renderer.graphicsDevice.SetRenderTarget(roofs);
        Renderer.graphicsDevice.Clear(new Color(0, 0, 0, 1f));

        BlendState bl = new BlendState();

        //Once I am zero, i stay zero.
        bl.AlphaDestinationBlend = Blend.Zero;
        bl.AlphaSourceBlend = Blend.DestinationAlpha; 

        bl.ColorSourceBlend = Blend.DestinationAlpha;
        bl.ColorDestinationBlend = Blend.Zero;

        roofBatch.Begin(SpriteSortMode.Deferred, bl, null, null, null);

        roofBatch.Draw(los, new Rectangle(0, 0, w, h), new Rectangle(0, 0, w, h), Color.White);
        roofBatch.Draw(roofMap, new Rectangle(0, 0, w, h), new Rectangle(camX, camY, w, h), Color.White);

        roofBatch.End();
        Renderer.graphicsDevice.SetRenderTarget(null);

        Renderer.drawTexture(sb,roofs, 0, 0);
    }
}


