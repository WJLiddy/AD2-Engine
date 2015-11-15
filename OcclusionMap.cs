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
    private RenderTarget2D roofs;
    // A batch for ORing the OccludeMap with the LineOfSight Map.
    private SpriteBatch roofBatch;

    public OcclusionMap(string pathToMapXML, int screenWidth, int screenHeight) : base(pathToMapXML,screenWidth, screenHeight)
    {
        Dictionary<string, LinkedList<string>> mapXML = Utils.GetXMLEntriesHash(pathToMapXML);

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
        Walls = getCollisionArray(CollisionMaskMap, wallKey);
    }

    //Render the roof layer to this spritebatch, based on the LOS, which is a texture containing pixels that have a=0 for places you can see.
    public void renderRoofs(AD2SpriteBatch sb, Texture2D los, int camX, int camY, int w, int h)
    {
        //Target Roofs.
        Renderer.GraphicsDevice.SetRenderTarget(roofs);
        //Clear it to 100% opaque
        Renderer.GraphicsDevice.Clear(new Color(0, 0, 0, 1f));

        //Begin Drawing.
        roofBatch.Begin(SpriteSortMode.Deferred, DrawIfDestinationOpaque(), null, null, null);

        //If the LOS has any transparent (a=0) pixels, they will overwrite the the opaque map.
        roofBatch.Draw(los, new Rectangle(0, 0, w, h), new Rectangle(0, 0, w, h), Color.White);
        //If there are any transparent pixels, the Occlude map will not draw over it.
        roofBatch.Draw(OccludeMap, new Rectangle(0, 0, w, h), new Rectangle(camX, camY, w, h), Color.White);

        roofBatch.End();
        Renderer.GraphicsDevice.SetRenderTarget(null);

        sb.DrawTexture(roofs, 0, 0);
    }

    private BlendState DrawIfDestinationOpaque()
    {
        BlendState bl = new BlendState();
        bl.AlphaSourceBlend = Blend.DestinationAlpha;
        bl.ColorSourceBlend = Blend.DestinationAlpha;

        bl.AlphaDestinationBlend = Blend.Zero;
        bl.ColorDestinationBlend = Blend.Zero;
        return bl;
    }
}