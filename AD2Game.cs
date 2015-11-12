using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

public abstract class AD2Game : Game
{
    // Puts our Rendering tools in a globally accessible place
    protected Renderer Renderer;

    // Spritebatch that gets put on screen
    private AD2SpriteBatch PrimarySpriteBatch;

    // Use this to play sounds and stuff
    protected SoundManager Manager;

    int BaseHeight;
    int BaseWidth;


    // Create a game with the passed in resolution and the msPerFrame
    public AD2Game(int baseWidth, int baseHeight, int msPerFrame)
    {
        // Set path to assets
        // TODO: Only works on debug releases
        Utils.SetAssetDirectory(@"..\..\..\assets\");
        this.BaseWidth = baseWidth;
        this.BaseHeight = baseHeight;

        // Set the games graphics Manager.
        Renderer.GraphicsDeviceManager = new GraphicsDeviceManager(this);

        // Set up the fixed FPS here.
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromMilliseconds(msPerFrame);
    }

    // Here we instantiate the Graphics Device, set the screen res, then allow the users to load stuff
    protected override void LoadContent()
    {
        Renderer.GraphicsDevice = Renderer.GraphicsDeviceManager.GraphicsDevice;
        PrimarySpriteBatch = new AD2SpriteBatch(Renderer.GraphicsDevice);
        Renderer.setResolution(BaseWidth, BaseHeight);
        Utils.Load();
        AD2LoadContent();
    }

    //Get the controller Status
    protected override void Update(GameTime gameTime)
    {
        // Check the device for Player One
        GamePadState[] gamepads = new GamePadState[4]{
            GamePad.GetState(PlayerIndex.One),
            GamePad.GetState(PlayerIndex.Two),
            GamePad.GetState(PlayerIndex.Three),
            GamePad.GetState(PlayerIndex.Four)
        };

        //If Keyboard.getState can be static then so can Renderer!
        AD2Logic(gameTime.ElapsedGameTime.Milliseconds, Keyboard.GetState(), gamepads);
    }

    protected abstract void AD2Logic(int ms, KeyboardState keyboardState, GamePadState[] gamePadState);

    protected abstract void AD2Draw(AD2SpriteBatch primarySpriteBatch);

    protected abstract void AD2LoadContent();

    protected override void Draw(GameTime gameTime)
    {
        Renderer.GraphicsDevice.Clear(Color.Black);

        //set the spritebatch to start.
        PrimarySpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Renderer.MatrixScale);
        AD2Draw(PrimarySpriteBatch);
        PrimarySpriteBatch.End();
    }
}