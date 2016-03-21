using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

// A Map contains "objects" which have heights and therefore players can go in front/behind them.
// Ordered drawing is a must in this case, so
// Requires you to draw things on the screen in order of y.
public class ObliqueMap : OcclusionMap
{
    // The maximum height an object can be. 
    //TODO: Change this to allow objects of varying heights without penalty.
    private static readonly int MAX_OBJECT_HEIGHT = 90;

    // The texture to be rendered at a "y" coordinate.
    private class MapObjectLine
    {
        public RenderTarget2D Texture;
        public int Height;
    }

    // All of the objects, ordered by y=0, y=1... y=Base.Height-1
    private MapObjectLine[] AllObjects;

    public ObliqueMap(string pathToMap, int w, int h) : base (pathToMap,w,h)
    {
        Dictionary<string, LinkedList<string>> mapXML = Utils.GetXMLEntriesHash(pathToMap);

        // Has the "mask" which designates where items are and how tall they are.
        Texture2D objectMask;
        // Has the "map" which contains actual items/
        Texture2D objectMap;
        // Keys to tell where an item starts and how tall it is.
        Color objectBaseKey;
        Color objectHeightKey;

        try
        {
            // load the textures of the object and it's mask.
            objectMask = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + mapXML["objectMask"].First.Value);
            objectMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMap) + @"\" + mapXML["object"].First.Value);

            // load in the color keys.
            objectBaseKey = new Color(Int32.Parse(mapXML["objectBaseKeyR"].First.Value), Int32.Parse(mapXML["objectBaseKeyB"].First.Value), Int32.Parse(mapXML["objectBaseKeyG"].First.Value));
            objectHeightKey = new Color(Int32.Parse(mapXML["objectHeightKeyR"].First.Value), Int32.Parse(mapXML["objectHeightKeyB"].First.Value), Int32.Parse(mapXML["objectHeightKeyG"].First.Value));
        }
        catch
        {
            Utils.Log("ObliqueMap " + pathToMap + " is missing an element or has a bad color value");
            return;
        }

        //Now all we need to do is populate allObjects, so we know what to draw at each y.
        createObjectMap(objectMap, objectMask, objectBaseKey, objectHeightKey);
    }

    // Draws the object line at the the position 'x' 'y' with the 
    public void DrawObjectLine(SpriteBatch sb, int x, int y, int mapY)
    {
        // Draw if the object line is on screen AND the object line is in bounds of the array.
        if (mapY + y >= 0 && mapY + y < (BaseMap.Height))
        {
            if (AllObjects[mapY + y] != null)
            {
                MapObjectLine l = AllObjects[mapY + y];
                sb.Draw(l.Texture, new Rectangle(-x, -y + (y + mapY) +- (l.Height - 1), BaseMap.Width, l.Height), new Rectangle(0, 0, BaseMap.Width, l.Height), Color.White);
            }
        }
    }

   
    //Create a list of objects to render
    public void createObjectMap(Texture2D objectLayer, Texture2D objectMask, Color baseKey, Color heightKey)
    {
        //first thing to do is convert the objectLayer and objectMask to colors.
        Color[] objectLayerColor = new Color[BaseMap.Height * BaseMap.Width];
        objectLayer.GetData(objectLayerColor);

        Color[] objectMaskColor = new Color[BaseMap.Height * BaseMap.Width];
        objectMask.GetData(objectMaskColor);

        //Now, set up the object base and height masks as boolean arrays.
        bool[,] objectBaseMask = new bool[BaseMap.Width, BaseMap.Height];
        bool[,] objectHeightMask = new bool[BaseMap.Width, BaseMap.Height];

        //ObjectBaseMask and ObjectHeightMask are filled.
        PopulateBaseAndHeightMaps(objectMaskColor, baseKey, heightKey, objectBaseMask, objectHeightMask);

        //Create a list of freestanding objects. on each Y coordinate.
        AllObjects = new MapObjectLine[BaseMap.Height];

        //Now we have the base mask and height mask figured out. We need to find all of the bases, climb up the height, and map them to objects.
        for (int y = 0; y != BaseMap.Height; y++)
        {

            //we say that each map object line is a bitmap that towers above the "base" y in the array.
            AllObjects[y] = new MapObjectLine();
            AllObjects[y].Height = MAX_OBJECT_HEIGHT;
            AllObjects[y].Texture = new RenderTarget2D(Renderer.GraphicsDevice, BaseMap.Width, MAX_OBJECT_HEIGHT);

            // Then, we set up the a spritebatch so we can render objects from the texture based on the mask to this.
            SpriteBatch objRender = new SpriteBatch(Renderer.GraphicsDevice);
            Renderer.GraphicsDevice.SetRenderTarget(AllObjects[y].Texture);
            Renderer.GraphicsDevice.Clear(Color.Transparent);

            objRender.Begin();

            bool drewSomething = false;
            for (int x = 0; x != BaseMap.Width ; x++)
            {
                //Climb up the pixel at "x,y", rendering whatever the mask specifies to the object line texture.
                if (objectBaseMask[x, y])
                {
                    drewSomething = true;
                    renderObjectSliver(x, y, objectHeightMask, objectLayerColor, objRender);
                    
                }
            }
            objRender.End();
            //Setting as null means skip the rendering.
            if (!drewSomething)
                AllObjects[y] = null;
        }
    }
        
    //At the position 'x' and 'y' render the object onto the object renderer.
    private void renderObjectSliver(int x, int y, bool[,] objectHeightMask, Color[] objectLayerColor, SpriteBatch objRender)
    {
        //find the total height of the object, h.
        int totalObjectHeight = 1;
        while (objectHeightMask[x, y - totalObjectHeight])
        {
            totalObjectHeight++;
        }
        
        //Get the color data of the object, ascending.
        Color[] colorData = new Color[totalObjectHeight];

        for (int dh = 0; dh != totalObjectHeight; dh++)
        {
            //we know the object is 'h' pixels high from the base.
            //we also know that the top is at y = 0.
            colorData[dh] = objectLayerColor[x + ((y + -((totalObjectHeight - 1) - dh)) * BaseMap.Width)];
        }

        Texture2D newObject = new Texture2D(Renderer.GraphicsDevice, 1, totalObjectHeight);
        newObject.SetData(colorData);
        //Draw the rendered object onto the renderer.
        objRender.Draw(newObject, new Rectangle(x, (AllObjects[y].Height - 1) - (totalObjectHeight - 1), 1, totalObjectHeight), new Rectangle(0, 0, 1, totalObjectHeight), Color.White);
    }
  
    //Copys Base and Height Map from bitmaps
    private void PopulateBaseAndHeightMaps(Color[] objectMask, Color baseKey, Color heightKey, bool[,] objectHeightMask, bool[,] objectBaseMask)
    {
        for (int x = 0; x != BaseMap.Width; x++)
        {
            for (int y = 0; y != BaseMap.Height; y++)
            {
                Color c = objectMask[x + (y * BaseMap.Width)];
                objectBaseMask[x, y] = c.Equals(baseKey);
                objectHeightMask[x, y] = c.Equals(heightKey);
            }
        }
    }


}


