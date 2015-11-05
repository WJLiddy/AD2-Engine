using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.IO;
using System.Xml;

//An AnimationSet is a set of sprite matrix animations that are all mutually exclusive for one character.
//For example, A village could have an eat sprite matrix, and a walk sprite matrix, never to be drawn at the same time. 
//Each sprite matrix is to represent one animation (such as walk, eat, etc).

//our implemetaion of a single animation is a sprite matrix.ons.

//play once finction
public class AnimationSet
{
    //The name of the animation that is defaulted to on load.
    private readonly String DEFAULT_ANIMATION_NAME = "idle";

    //All of the sprite matrixes in this set.
    private Hashtable animations;

    //ticks until the next frame is triggered. May be changed at any time.
    public int speed = 1;

    //The current sprite matrix being shown.
    private SpriteMatrix currentAnimation = null;

    //The current x-frame.
    private int xFrame = 0;
    //The current y-frame.
    private int yFrame = 0;

    //Ticks until next xframe. Remember all animations run left to right.
    private int ticksLeftToNextFrame = 0;
    //If true, automatically animate on tick. Otherwise, the animation will stay at the x and y frame.
    private bool animate = false;

    //Creates a new animation set relative to assets
    public AnimationSet(String pathToXML)
    {
        //Loads in all the sprite matrix animations, picking the last one to be the default, or the one marked "idle"
        animations = loadAnimations(pathToXML); 

        //Overrides default if the default animation name is in the set.
        if (animations[DEFAULT_ANIMATION_NAME] != null)
            hold(DEFAULT_ANIMATION_NAME, 0, 0);
    }

    //freezes to this animation at this x and y frame.
    public void hold(string anim, int x, int y)
    {
        currentAnimation = (SpriteMatrix)animations[anim];
        //TODO error handling
        animate = false;
        this.xFrame = x;
        this.yFrame = y;
    }

    //starts animation. If animation is already in progress, does nothing
    public void autoAnimate(string anim, int y)
    {
        //TODO Error Handling
        currentAnimation = (SpriteMatrix)animations[anim];
        this.yFrame = y;

        if (!animate)
        {
            ticksLeftToNextFrame = speed;
            animate = true;
            xFrame = 0;
        }
    }

    //call on every clock tick.
    public void update()
    {
        if (animate)
        {
            ticksLeftToNextFrame--;

            if (ticksLeftToNextFrame == 0)
            {
                xFrame = (xFrame + 1) % currentAnimation.frameCountX;
                ticksLeftToNextFrame = speed;
            }
        }
    }

    //draw the frame, but can be stretched
    public void draw(SpriteBatch sb, int x, int y, int w, int h)
    {
        currentAnimation.draw(sb, xFrame,yFrame,x,y,w,h);
    }

    //draw the frame, no stretching.
    public void draw(SpriteBatch sb, int x, int y)
    {
        currentAnimation.draw(sb, xFrame, yFrame, x, y);
    }

    //load in aniamation set.
    private Hashtable loadAnimations(String pathToXML)
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

