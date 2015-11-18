using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

// A Occlusion Map is a Collision Map that adds support for Line Of Sight and occluded areas.
// Use "Wall" to designate things players cannot see through.
// Use "Roof" to designate what is drawn if players cannot see things.
 
public class OcclusionMap : CollisionMap
{
    // What is drawn if obscured by a wall.
    private Texture2D OccludeMap;
    // A boolean array where there are walls.
    private bool[,] Walls;
    // A special RenderTarget that we draw occulded porions to.
    private RenderTarget2D Roofs;
    // A batch for ORing the OccludeMap with the LineOfSight Map.
    private SpriteBatch RoofBatch;

    public OcclusionMap(string pathToMapXML, int screenWidth, int screenHeight) : base(pathToMapXML,screenWidth, screenHeight)
    {
        Dictionary<string, LinkedList<string>> mapXML = Utils.GetXMLEntriesHash(pathToMapXML);
        RoofBatch = new SpriteBatch(Renderer.GraphicsDevice);
        Roofs = new RenderTarget2D(Renderer.GraphicsDevice, ScreenWidth, screenHeight);

        Color wallKey;
        try
        {
            // load the texture of the "occlude" element.
            OccludeMap = Utils.TextureLoader(Path.GetDirectoryName(pathToMapXML) + @"\" + mapXML["occlude"].First.Value);
            // load in the color key.
            wallKey = new Color(Int32.Parse(mapXML["wallKeyR"].First.Value), Int32.Parse(mapXML["wallKeyG"].First.Value), Int32.Parse(mapXML["wallKeyB"].First.Value));
        }
        catch
        {
            Utils.Log("OccludeMap " + pathToMapXML + " is missing an element or has a bad color value");
            return;
        }
        
        //fill the boolean array for walls.
        Walls = GetCollisionArray(CollisionMaskMap, wallKey);
    }

    //Render the roof layer to this spritebatch, based on the LOS, which is a texture containing pixels that have a=0 for places you can see.
    public void RenderRoofs(AD2SpriteBatch sb, Texture2D los, int camX, int camY)
    {
        //Target Roofs.
        Renderer.GraphicsDevice.SetRenderTarget(Roofs);
        //Clear it to 100% opaque
        Renderer.GraphicsDevice.Clear(new Color(0, 0, 0, 1f));

        //Begin Drawing.
        RoofBatch.Begin(SpriteSortMode.Deferred, DrawIfDestinationOpaque(), null, null, null);

        //If the LOS has any transparent (a=0) pixels, they will overwrite the the opaque map.
        RoofBatch.Draw(los, new Rectangle(0, 0, ScreenWidth, ScreenHeight), new Rectangle(0, 0, ScreenWidth, ScreenHeight), Color.White);
        //If there are any transparent pixels, the Occlude map will not draw over it.
        RoofBatch.Draw(OccludeMap, new Rectangle(0, 0, ScreenWidth, ScreenHeight), new Rectangle(camX, camY, ScreenWidth, ScreenHeight), Color.White);

        RoofBatch.End();
        Renderer.GraphicsDevice.SetRenderTarget(null);

        sb.DrawTexture(Roofs, 0, 0);
    }

    //A Blendstate that only draws to a texture if the desination is opaque.
    private BlendState DrawIfDestinationOpaque()
    {
        BlendState bl = new BlendState();
        bl.AlphaSourceBlend = Blend.DestinationAlpha;
        bl.ColorSourceBlend = Blend.DestinationAlpha;

        bl.AlphaDestinationBlend = Blend.Zero;
        bl.ColorDestinationBlend = Blend.Zero;
        return bl;
    }

    //Given coordinates and a camera, generates a transparent texture for all the places that the coords can 'see'
    public Texture2D getLOS(LinkedList<int[]> coords, int cameraX, int cameraY)
    {
        Color[] LOSTemp = new Color[ScreenHeight * ScreenWidth];
        Texture2D LOSOverlay = new Texture2D(Renderer.GraphicsDevice, ScreenWidth, ScreenHeight);
        Color opaque = new Color(0, 0, 0, 1f);
    
        //TODO: maybe faster using GPU?
        for (int i = 0; i != ScreenHeight * ScreenWidth; i++)
        {
            LOSTemp[i] = opaque;
        }

        //Now, when we raycast, we update the the associated player's LOS if it is their turn.
        Raycast(LOSTemp, coords, cameraX, cameraY);

        LOSOverlay.SetData<Color>(LOSTemp);

        return LOSOverlay;
    }

    //clean up HARD
    //Given a LOS data array, colors the areas transparent that are visible.
    public void Raycast(Color[] losData, LinkedList<int[]> coords, int cameraX, int cameraY)
    {
        foreach(int[] coord in coords)
        {
            int destY = cameraY;

            for (int ddestX = cameraX; ddestX != cameraX + ScreenWidth; ddestX++)
            {
                raycastLinear(losData, coord[0], coord[1], ddestX, destY, cameraX, cameraY);
            }

            //for bottom
            destY = cameraY + ScreenHeight;
            for (int ddestX = cameraX; ddestX != cameraX + ScreenWidth; ddestX++)
            {
                raycastLinear(losData, coord[0], coord[1], ddestX, destY, cameraX, cameraY);
            }

            //for left
            int destX = cameraX;
            for (int ddestY = cameraY; ddestY != cameraY + ScreenHeight; ddestY++)
            {
                raycastLinear(losData, coord[0], coord[1], destX, ddestY, cameraX, cameraY);
            }

            //for right
            destX = cameraX + ScreenWidth;
            for (int ddestY = cameraY; ddestY != cameraY + ScreenHeight; ddestY++)
            {
                raycastLinear(losData, coord[0], coord[1], destX, ddestY, cameraX, cameraY);
            }
        }
    }

    //Given an LOS data array and a line, draws that line transparently as far as it can see.
    private void raycastLinear(Color[] losData, int startX, int startY, int destX, int destY, int cameraX, int cameraY)
    {

        int distToDest = (int)(1 + Utils.Dist(startX, destX, startY, destY));

        double dx = destX - startX;
        double dy = destY - startY;


        double xDouble = startX;
        double yDouble = startY;
        double deltaSize = .5;
        double incX = deltaSize * (dx / distToDest);
        double incY = deltaSize * (dy / distToDest);

        for (double delta = 0; delta < distToDest; delta += deltaSize)
        {
            xDouble += incX;
            yDouble += incY;
            int x = (int)xDouble;
            int y = (int)yDouble;


            int index = ((x - cameraX) + (ScreenWidth * (y - cameraY)));

    
            //TODO: Improve Walls[x,y] like boolean [x,y]
            if (x < 0 || y < 0 || x >= BaseMap.Width || y >= BaseMap.Height || Walls[x, y])
                return;
            else if (index >= 0 && index < ScreenWidth * ScreenHeight)
                losData[index] = Color.Transparent;
        }


    }

    public override bool Collide(int x, int y)
    {
        return base.Collide(x, y) || Walls[x, y];
    }

}
