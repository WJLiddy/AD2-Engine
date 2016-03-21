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

    // For handling controllers.
    private ControllerManager ControllerManager;

    int BaseHeight;
    int BaseWidth;

    bool RunningSlowly;


    // Create a game with the passed in resolution and the msPerFrame
    public AD2Game(int baseWidth, int baseHeight, int msPerFrame)
    {

        #if DEBUG
            Utils.SetAssetDirectory(@"..\..\..\assets\");
        #else
            Utils.SetAssetDirectory(@"assets\");
        #endif
        this.BaseWidth = baseWidth;
        this.BaseHeight = baseHeight;

        // Set the games graphics Manager.
        Renderer.GraphicsDeviceManager = new GraphicsDeviceManager(this);

        // Set up the fixed FPS here.
        IsFixedTimeStep = true;
        TargetElapsedTime = TimeSpan.FromMilliseconds(msPerFrame);

        ControllerManager = new ControllerManager();
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

    protected override void Update(GameTime gameTime)
    {
        RunningSlowly = gameTime.IsRunningSlowly;
        AD2Logic(gameTime.ElapsedGameTime.Milliseconds, Keyboard.GetState(), ControllerManager.GetState());
    }

    protected abstract void AD2Logic(int ms, KeyboardState keyboardState, SlimDX.DirectInput.JoystickState[] joyStickState);

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

    protected bool IsRunningSlowly()
    {
        return RunningSlowly;
    }
}