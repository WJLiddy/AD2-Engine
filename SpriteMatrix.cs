using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

//A SpriteMatrix is basically a Texture2d divided into frames.
//The intent is that the frames can be called like a matrix for ease of use in animations.
//To use, put the frames next to each other and create a matching XML.
public class SpriteMatrix
{
    //The sheet itself.
    private Texture2D Sheet;
    //The width of a frame.
    public int FrameWidth { get; private set; }
    //The height of a frame.
    public int FrameHeight { get; private set; }
    //How many x-direction frames there are in the animation.
    public int FrameCountX { get; private set; }
    //How many y-direction frames there are in this animation.
    public int FrameCountY { get; private set; }

    //When drawing a frame, this is an offset to draw it off by when rendering.
    //In AD2, this refers to the top left corner of someone's sprite. Other isometrics may use this system as well.
    //TODO Convert to OffsetX, Offset Y.
    public int XOffset{ get; private set; }
    //When drawing a frame, this is an offset to draw it off by when rendering.
    public int YOffset { get; private set; }

    //the path to the texture representing this sprite.
    private string SpritePath;

    //The constructor takes a path to an XML, decodes it, and looks for the png to fetch the actual texture.
    //Pass the XML relative to the assets folder.
    public SpriteMatrix(String pathToXML)
    {
        ReadParameters(pathToXML);
        SpritePath = Path.ChangeExtension(pathToXML, ".png");
        Sheet = Utils.TextureLoader(SpritePath);
    }
    
    //draw a given frame at a given place at a given size. Allows for streching/resizing
    public void Draw(SpriteBatch sb,int xFrame, int yFrame, int x, int y, int w, int h)
    {
        CheckIfValidFrame(xFrame, yFrame);
        sb.Draw(Sheet, new Rectangle(x - XOffset, y - YOffset, w, h), new Rectangle(xFrame*FrameWidth, yFrame*FrameHeight, FrameWidth, FrameHeight), Color.White);
    }

    //draw a given frame at a given place with the exact pixel size.
    public void Draw(SpriteBatch sb, int xFrame, int yFrame, int x, int y)
    {
        //calls color version
        Draw(sb, xFrame, yFrame, x, y, Color.White);
    }

    //draw a given frame at a given place with a tinted color..
    public void Draw(SpriteBatch sb, int xFrame, int yFrame, int x, int y, Color tint)
    {
        CheckIfValidFrame(xFrame, yFrame);
        sb.Draw(Sheet, new Rectangle(x - XOffset, y - YOffset, FrameWidth, FrameHeight), new Rectangle(xFrame * FrameWidth, yFrame * FrameHeight, FrameWidth, FrameHeight), tint);
    }

    public void ReadParameters(String pathToSheetXML)
    {
        Dictionary<string,LinkedList<string>> xml = Utils.GetXMLEntriesHash(pathToSheetXML);

        try
        {
            FrameWidth = Int32.Parse(xml["frameWidth"].First.Value);
            FrameHeight = Int32.Parse(xml["frameHeight"].First.Value);
            FrameCountX = Int32.Parse(xml["frameCountX"].First.Value);
            FrameCountY = Int32.Parse(xml["frameCountY"].First.Value);
        } catch ( KeyNotFoundException)
        {
            //We did not find vital spritematrix info.
            Utils.Log("Animation " + pathToSheetXML + " was missing an XML parameter");
            FrameWidth = FrameHeight = FrameCountX = FrameCountY = 1;
        }

        XOffset = (xml.ContainsKey("offsetX")) ? Int32.Parse(xml["offsetX"].First.Value) : 0;
        YOffset = (xml.ContainsKey("offsetY")) ? Int32.Parse(xml["offsetY"].First.Value) : 0;
    }

    void CheckIfValidFrame(int xFrame, int yFrame)
    {
        if (xFrame >= FrameCountX && yFrame >= FrameCountY)
            Utils.Log("Tried to play animation (" + xFrame +"," + yFrame +") which has path " + SpritePath);
    }
       
}

