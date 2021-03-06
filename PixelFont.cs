﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;

// ADFont handles usage of an ASCII pixel font.
public class PixelFont
{
    // Height of the font, including spaces on the top and bottom.
    private int Height;

    // A long strip containing the characters. Ordered by ascii number.
    private Texture2D Font;

    // Widths of an indiviual ASCII character.
    // For example the number "1", which might be represented as a vertical line plus one pixel to the left, has the spacing value of 2.
    // You can get the width of any character by passing it as the index of this array. Unsupported characters are '0'.
    static private int[] Widths;

    // Amount of space after each character.
    private int SpaceBetweenLetters;

    public PixelFont(string pathToFile)
    {
        LoadFontFromXML(pathToFile);
    }

    public void Draw(SpriteBatch sb, string s, int x, int y, Color col, int scale = 1, bool outline = false, Color ? outlineCol = null){

        //where we are putting the next letter relaive to the x
        int xCursor = 0; 

        foreach(char c in s)
        {
            int value = (int)c;

            //Draws a black underline of the letter.
            if (outline)
            {
                sb.Draw(Font, new Rectangle(x + scale + xCursor, y, scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), outlineCol ?? Color.Black);
                sb.Draw(Font, new Rectangle(x + -scale + xCursor, y, scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), outlineCol ?? Color.Black);
                sb.Draw(Font, new Rectangle(x + xCursor, y + scale, scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), outlineCol ?? Color.Black);
                sb.Draw(Font, new Rectangle(x + xCursor, y - scale , scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), outlineCol ?? Color.Black);
            }

            //Draws the actual letter.
            sb.Draw(Font, new Rectangle(x+xCursor, y, scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), col);

            //increment the xCursor by the spacing and the widths.
            xCursor += SpaceTakenByCharacter(scale, value, outline);
        }
    }

    //return width of a given string. 
    public int GetWidth(string s,bool outline)
    {
        int width = 0;
        foreach (char c in s)
        {
            width += SpaceTakenByCharacter(1, c, outline);
        }
        //ignore last space between letter.
        return (width - SpaceBetweenLetters);
   }

    private int SpaceTakenByCharacter(int scale, int value, bool outline)
    {
        //outline takes up two additional pixels.
        return scale * (SpaceBetweenLetters + Widths[value] + (outline ? 2 : 0));
    }

    private void LoadFontFromXML(string pathToFile)
    {
        //read the associated xml for the font. Fetch all the letter spacings.
        //Require: Height, and Space.
        //Then the ascii codes 33 to 126.
        Widths = new int[128];
        Dictionary<string, LinkedList<string>> fontdata = Utils.GetXMLEntriesHash(pathToFile);
        for (int i = 33; i != 127; i++)
        {
            string character = ((char)i).ToString();

            //Two ways to input width:
            // _ASCIINUMBER_
            // and _ALPHANUMBERIC

            if (fontdata.ContainsKey("_"+character))
                Widths[i] = Int32.Parse(fontdata["_"+character].First.Value);

            else if (fontdata.ContainsKey("_" + i.ToString() + "_"))
                Widths[i] = Int32.Parse(fontdata["_" + i.ToString() + "_"].First.Value);
        }

        //fetch the font.
        Font = Utils.TextureLoader(Path.ChangeExtension(pathToFile,".png"));

        if (!fontdata.ContainsKey("height") || !fontdata.ContainsKey("space") || !fontdata.ContainsKey("spaceBetweenLetters"))
        {
            Height = 1;
            Utils.Log("Error in XML while loading font " + pathToFile);
        } else
        {
            Height = Int32.Parse(fontdata["height"].First.Value);
            Widths[' '] = Int32.Parse(fontdata["space"].First.Value);
            SpaceBetweenLetters = Int32.Parse(fontdata["spaceBetweenLetters"].First.Value);
        }
    }
}