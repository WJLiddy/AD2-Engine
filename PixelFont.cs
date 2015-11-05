using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

//TODO XMLIZE! read from an XML file that points at a fontsheet and also text.

//ADFont handles usage of an ASCII pixel font.
public class PixelFont
{
    private Texture2D font;
    //Height of the font, including spaces.
    private int height;
    //widths of an indiviual ASCII character, including post-spacing after character print. 
    //For example the number "1", a vertical line, has the spacing value of "2".
    static private int[] widths;

    //Amount of space after each character.
    private readonly int LETTER_SPACING = 1;

    public PixelFont(string pathToFile)
    {
        font = Utils.TextureLoader(pathToFile);
        height = 7;
        widths= defaultSpacing();      
    }

    public void draw(string s, int x, int y, Color col, int scale = 1, bool outline = false){
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
                Utils.sb.Draw(font, new Rectangle(x + scale + xCursor, y, scale * widths[value], scale * height), new Rectangle(0, value * height, widths[value], height), Color.Black);
                Utils.sb.Draw(font, new Rectangle(x + -scale + xCursor, y, scale * widths[value], scale * height), new Rectangle(0, value * height, widths[value], height), Color.Black);
                Utils.sb.Draw(font, new Rectangle(x + xCursor, y + scale, scale * widths[value], scale * height), new Rectangle(0, value * height, widths[value], height), Color.Black);
                Utils.sb.Draw(font, new Rectangle(x + xCursor, y - scale , scale * widths[value], scale * height), new Rectangle(0, value * height, widths[value], height), Color.Black);
            }

            //Draws the actual letter.
            Utils.sb.Draw(font, new Rectangle(x+xCursor, y, scale * widths[value], scale * height), new Rectangle(0, value * height, widths[value], height), col);

            //increment the xCursor by the spacing and the widths.
            xCursor += spaceTakenByCharacter(scale, value, outline);
        }
    }

    //return width of a given string. 
    public int getWidth(String s,bool outline)
    {
        s = s.ToUpper();
        int width = 0;
        foreach (char c in s)
        {
            width += spaceTakenByCharacter(1, c, outline);
        }
        return width;
   }

    public static int[] defaultSpacing()
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

    private int spaceTakenByCharacter(int scale, int value, bool outline)
    {
        //outline takes up two additional pixels
        return scale * (LETTER_SPACING + widths[value] + (outline ? 2 : 0));
    }
}