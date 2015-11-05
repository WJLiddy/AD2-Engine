using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

public abstract class AD2Game : Game
{
    // Use this to access MonoGame render routines, as well as ad2 render routines
    // Handles all the drawing stuff. Simply tell it to render.
    protected Renderer renderer;

    // Use this to play sounds and stuff
    protected SoundManager manager;

    int baseHeight;
    int baseWidth;

    public AD2Game(int baseWidth, int baseHeight, int msPerFrame)
    {
        Utils.setAssetDirectory(@"..\..\..\assets\");
        this.baseWidth = baseWidth;
        this.baseHeight = baseHeight;


        //Info hiding later
        Renderer.graphicsDeviceManager = new GraphicsDeviceManager(this);

        //say fps here
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromMilliseconds(msPerFrame); // 20 milliseconds, or 50 FPS.

        manager = new SoundManager("sound\\");
        Utils.load();
    }

    protected override void LoadContent()
    {
        Renderer.graphicsDevice = Renderer.graphicsDeviceManager.GraphicsDevice;
        renderer = new Renderer(baseWidth, baseHeight);
        ad2LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        // Check the device for Player One
        GamePadState[] gamepads = new GamePadState[4]{
            GamePad.GetState(PlayerIndex.One),
            GamePad.GetState(PlayerIndex.Two),
            GamePad.GetState(PlayerIndex.Three),
            GamePad.GetState(PlayerIndex.Four)
        };

        ad2Logic(gameTime.ElapsedGameTime.Milliseconds, Keyboard.GetState(), gamepads);
    }

    protected abstract void ad2Logic(int ms, KeyboardState keyboardState, GamePadState[] gamePadState);

    protected abstract void ad2Draw();

    protected abstract void ad2LoadContent();

    protected override void Draw(GameTime gameTime)
    {
        Renderer.graphicsDevice.Clear(Color.Black);
        renderer.startDraw();
        ad2Draw();
        renderer.endDraw();
    }
}