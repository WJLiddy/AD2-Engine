using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

// ADFont handles usage of an ASCII pixel font.
public class PixelFont
{
    // Height of the font, including spaces on the top and bottom.
    private int Height;

    // A long strip containing the characters. Ordered by ascii number.
    private Texture2D Font;

    // Widths of an indiviual ASCII character, including post-spacing after character print. 
    // For example the number "1", which might be represented as a vertical line, has the spacing value of 2.
    // You can get the width of any character by passing it as the index of this array. Unsupported characters are '0'
    static private int[] Widths;

    // Amount of space after each character. One pixel is fine.
    private readonly int LetterSpacing = 1;

    public PixelFont(string pathToFile)
    {
        loadFontFromXML(pathToFile);
    }

    public void draw(SpriteBatch sb, string s, int x, int y, Color col, int scale = 1, bool outline = false){
        //Support upcase to get correct character codes.
        s = s.ToUpper();

        //where we are putting the next letter relaive to the x
        int xCursor = 0; 

        foreach(char c in s)
        {
            int value = (int)c;

            //Draws a black underline of the letter.
            if (outline)
            {
                sb.Draw(Font, new Rectangle(x + scale + xCursor, y, scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), Color.Black);
                sb.Draw(Font, new Rectangle(x + -scale + xCursor, y, scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), Color.Black);
                sb.Draw(Font, new Rectangle(x + xCursor, y + scale, scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), Color.Black);
                sb.Draw(Font, new Rectangle(x + xCursor, y - scale , scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), Color.Black);
            }

            //Draws the actual letter.
            sb.Draw(Font, new Rectangle(x+xCursor, y, scale * Widths[value], scale * Height), new Rectangle(0, value * Height, Widths[value], Height), col);

            //increment the xCursor by the spacing and the widths.
            xCursor += SpaceTakenByCharacter(scale, value, outline);
        }
    }

    //return width of a given string. 
    public int GetWidth(String s,bool outline)
    {
        s = s.ToUpper();
        int width = 0;
        foreach (char c in s)
        {
            width += SpaceTakenByCharacter(1, c, outline);
        }
        return width;
   }

    public static int[] DefaultSpacing()
    {
        return new int[128]{
            0,0,0,0,0,0,0,0,0,0, //0 - 9
            0,0,0,0,0,0,0,0,0,0, //10 - 9
            0,0,0,0,0,0,0,0,0,0, //20 - 9
            0,0,2,0,0,0,0,0,0,0, //30 - 9
            0,0,0,0,0,0,0,0,3,2, //40 - 9
            3,3,3,3,3,3,3,3,1,0, //50 - 9
            0,0,0,0,0,3,3,3,3,2, //60 - 9
            2,3,3,3,4,3,3,5,3,3, //70 - 9
            3,4,3,3,3,3,3,5,3,3, //80 - 9
            3,0,0,0,0,0,0,0,0,0, //90 - 9
            0,0,0,0,0,0,0,0,0,0, //100 - 9
            0,0,0,0,0,0,0,0,0,0, //110 - 9
            0,0,0,0,0,0,0,0};
    }

    private int SpaceTakenByCharacter(int scale, int value, bool outline)
    {
        //outline takes up two additional pixels
        return scale * (LetterSpacing + Widths[value] + (outline ? 2 : 0));
    }

    private void loadFontFromXML(string pathToFile)
    {
        Font = Utils.TextureLoader(Path.ChangeExtension(pathToFile,".png"));
        Height = 7;
        Widths = DefaultSpacing();

    }
}