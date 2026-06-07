using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DynamicSpriteFontSample.Core;

/// <summary>
/// Demonstrates loading and drawing a <see cref="DynamicSpriteFont"/> from a file and a stream.
/// </summary>
public sealed class DynamicSpriteFontSampleGame : Game
{
    private const float LabelFontSize = 22f;
    private const float SmallFontSize = 20f;
    private const float MediumFontSize = 32f;
    private const float LargeFontSize = 52f;

    private readonly GraphicsDeviceManager _graphics;

    private SpriteBatch _spriteBatch;
    private DynamicSpriteFont _fontFromFile;
    private DynamicSpriteFont _fontFromStream;

    public DynamicSpriteFontSampleGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 960;
        _graphics.PreferredBackBufferHeight = 540;

        _spriteBatch = null!;
        _fontFromFile = null!;
        _fontFromStream = null!;

        IsMouseVisible = true;
        Window.Title = "DynamicSpriteFont Sample";
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // ------------------------------------------------------------------------------
        // Load at runtime with FromFile
        //
        string fromFilePath = Path.Combine(AppContext.BaseDirectory, "Content", "JetBrainsMono-Regular.ttf");
        _fontFromFile = DynamicSpriteFont.FromFile(GraphicsDevice, fromFilePath, 32.0f);


        // ------------------------------------------------------------------------------
        // Load at runtime with FromStream using TitleContainer
        //
        string streamPath = Path.Combine("Content", "JetBrainsMono-Regular.ttf");
        using Stream stream = TitleContainer.OpenStream(streamPath);
        _fontFromStream = DynamicSpriteFont.FromStream(GraphicsDevice, stream, 32.0f);

    }

    protected override void UnloadContent()
    {
        _fontFromStream.Dispose();
        _fontFromFile.Dispose();
        _spriteBatch.Dispose();

        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        {
            Exit();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(20, 24, 32));

        _fontFromFile.Size = 32.0f;

        _spriteBatch.Begin();

        // ------------------------------------------------------------------------------
        // Drawing text using the font that was loaded from DynamicSpriteFont.FromFile
        //
        _spriteBatch.DrawString(_fontFromFile, "Loaded from file", new Vector2(40.0f, 40.0f), Color.White);

        // ------------------------------------------------------------------------------
        // Drawing text using the font that was loaded from DynamicSpriteFont.FromFile
        //
        _spriteBatch.DrawString(_fontFromStream, "Loaded from stream", new Vector2(40.0f, 150.0f), Color.White);

        // ------------------------------------------------------------------------------
        // Drawing text using different font sizes
        //
        _fontFromFile.Size = 16.0f;
        _spriteBatch.DrawString(_fontFromFile, "Small text (16px)", new Vector2(40.0f, 314.0f), Color.White);

        _fontFromFile.Size = 32.0f;
        _spriteBatch.DrawString(_fontFromFile, "Medium text (32px)", new Vector2(40.0f, 354.0f), Color.White);

        _fontFromFile.Size = 52.0f;
        _spriteBatch.DrawString(_fontFromFile, "Large text (52px)", new Vector2(40.0f, 408.0f), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
