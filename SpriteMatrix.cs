using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Xml;

//A SpriteMatrix is basically a Texture2d divided into frames.
//The intent is that the frames can be called like a matrix for ease of use in animations.
//To use, put the frames next to each other and create a matching XML.
public class SpriteMatrix
{
    //The sheet itself.
    private Texture2D sheet;
    //The width of a frame.
    public int frameWidth { get; private set; }
    //The height of a frame.
    public int frameHeight { get; private set; }
    //How many x-direction frames there are in the animation.
    public int frameCountX { get; private set; }
    //How many y-direction frames there are in this animation.
    public int frameCountY { get; private set; }

    //When drawing a frame, this is an offset to draw it off by when rendering.
    //In AD2, this refers to the top left corner of someone's sprite. Other isometrics may use this system as well.
    //TODO Convert to OffsetX, Offset Y.
    public int xOffset{ get; private set; }
    //When drawing a frame, this is an offset to draw it off by when rendering.
    public int yOffset { get; private set; }

    //The constructor takes a path to an XML, decodes it, and looks for the png to fetch the actual texture.
    //Pass the XML relative to the assets folder.
    public SpriteMatrix(String pathToXML)
    {
        readParameters(pathToXML);
        sheet = Utils.TextureLoader(Path.ChangeExtension(pathToXML, ".png"));
    }
    
    //draw a given frame at a given place at a given size. Allows for streching/resizing
    public void draw(SpriteBatch sb,int xFrame, int yFrame, int x, int y, int w, int h)
    {
        checkIfValidFrame(xFrame, yFrame);
        sb.Draw(sheet, new Rectangle(x - xOffset, y - yOffset, w, h), new Rectangle(xFrame*frameWidth, yFrame*frameHeight, frameWidth, frameHeight), Color.White);
    }

    //draw a given frame at a given place with the exact pixel size.
    public void draw(SpriteBatch sb, int xFrame, int yFrame, int x, int y)
    {
        draw(sb, xFrame, yFrame, x, y, Color.White);
    }

    //draw a given frame at a given place with a tinted color..
    public void draw(SpriteBatch sb, int xFrame, int yFrame, int x, int y, Color tint)
    {
        checkIfValidFrame(xFrame, yFrame);
        sb.Draw(sheet, new Rectangle(x - xOffset, y - yOffset, frameWidth, frameHeight), new Rectangle(xFrame * frameWidth, yFrame * frameHeight, frameWidth, frameHeight), tint);
    }
    public void readParameters(String pathToSheetXML)
    {
        XmlReader reader = XmlReader.Create(pathToSheetXML);

        reader.ReadToFollowing("frameWidth");
        frameWidth = reader.ReadElementContentAsInt();

        reader.ReadToFollowing("frameHeight");
        frameHeight = reader.ReadElementContentAsInt();

        reader.ReadToFollowing("frameCountX");
        frameCountX = reader.ReadElementContentAsInt();

        reader.ReadToFollowing("frameCountY");
        frameCountY = reader.ReadElementContentAsInt();

        reader.ReadToFollowing("offsetX");
        xOffset = reader.ReadElementContentAsInt();

        reader.ReadToFollowing("offsetY");
        yOffset = reader.ReadElementContentAsInt();

        reader.Close();
    }

    void checkIfValidFrame(int xFrame, int yFrame)
    {
        if (xFrame >= frameCountX && yFrame >= frameCountY)
            Console.Out.WriteLine("WARNING: trying to draw frame not in animation set");
    }
       
}

