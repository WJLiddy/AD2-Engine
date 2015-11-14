using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.IO;
using System.Xml;

//An AnimationSet is a set of sprite matrix animations that are all mutually exclusive for one character.
//For example, A villager could have an eat sprite matrix, and a walk sprite matrix, never to be drawn at the same time. 
//Each sprite matrix is to represent one animation (such as walk, eat, etc).

//our implemetaion of a single animation is a sprite matrix.ons.

//TODO play once animations
public class AnimationSet
{
    //All of the sprite matrixes in this set.
    private Hashtable Animations;

    //Speed of an animation. May be changed at any time.
    public int Speed = 1;

    //The current sprite matrix being shown.
    private SpriteMatrix CurrentAnimation = null;

    //The current x-frame.
    private int XFrame = 0;
    //The current y-frame.
    private int YFrame = 0;

    //Ticks until next xframe. Remember all animations run left to right.
    private int TicksLeftToNextFrame = 0;
    //If true, automatically animate on tick. Otherwise, the animation will stay at the x and y frame.
    private bool Animate = false;
    //If true, once the animation reaches it's end, then it will simply stop.
    private bool AnimateOnce = false;

    //Creates a new animation set relative to assets
    public AnimationSet(String pathToXML)
    {
        //Loads in all the sprite matrix animations, picking the last one to be the current Animation.
        Animations = LoadAnimations(pathToXML); 
    }

    //freezes to this animation at this x and y frame.
    public void Hold(string anim, int x, int y)
    {
        CurrentAnimation = (SpriteMatrix)Animations[anim];
        //TODO error handling
        Animate = false;
        XFrame = x;
        YFrame = y;
    }

    //starts looping animation. If animation is already in progress, does nothing
    public void AutoAnimate(string anim, int y)
    {
        //TODO Error Handling
        CurrentAnimation = (SpriteMatrix)Animations[anim];
        YFrame = y;
        XFrame = 0;
        TicksLeftToNextFrame = Speed;
        Animate = true;
        XFrame = 0;
    }

    public void AutoAnimateOnce(string anim, int y)
    {
        AutoAnimate(anim, y);
        AnimateOnce = true;
    }

    //call on every clock tick.
    public void Update()
    {
        if (Animate)
        {
            TicksLeftToNextFrame--;

            if (TicksLeftToNextFrame == 0)
            {
                //If we are on the last animation, simply stop playing it.
                //Off by one: XFrame indexes to zero.
                if (AnimateOnce && XFrame == (CurrentAnimation.FrameCountX - 1))
                {
                    return;
                }

                XFrame = (XFrame + 1) % CurrentAnimation.FrameCountX;
                TicksLeftToNextFrame = Speed;
            }
        }
    }

    //draw the frame, but can be stretched
    public void Draw(AD2SpriteBatch sb, int x, int y, int w, int h)
    {
        CurrentAnimation.Draw(sb, XFrame,YFrame,x,y,w,h);
    }

    //draw the frame, no stretching.
    public void Draw(AD2SpriteBatch sb, int x, int y)
    {
        CurrentAnimation.Draw(sb, XFrame, YFrame, x, y);
    }

    //draw the frame, no stretching.
    public void Draw(AD2SpriteBatch sb, int x, int y, Color tint)
    {
        CurrentAnimation.Draw(sb, XFrame, YFrame, x, y, tint);
    }

    //load in aniamation set. TODO: Use utils.XML
    private Hashtable LoadAnimations(String pathToXML)
    {
        Hashtable loadedAnimations = new Hashtable();
        XmlReader reader = XmlReader.Create(pathToXML);
        //load each Animation.
        reader.ReadToFollowing("Animation");
        while (!reader.EOF)
        {
            String animationName = reader.ReadElementContentAsString();
            //add the animation to the hash, using the name.
            loadedAnimations.Add(animationName, new SpriteMatrix(Path.GetDirectoryName(pathToXML) + @"\" + animationName + ".xml"));
            //order the animation to hold at this animation's 0,0.
            reader.ReadToFollowing("Animation");
        }
        reader.Close();
        return loadedAnimations;
    }
}

