using System;
using System.IO;
using System.Linq;
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
    private string _asciiString;
    private bool _reloadFont;
    private KeyboardState _currentKeyboardState;
    private KeyboardState _previousKeyboardState;

    public DynamicSpriteFontSampleGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 800;

        _spriteBatch = null!;
        _fontFromFile = null!;

        IsMouseVisible = true;
        Window.Title = "DynamicSpriteFont Sample";
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        LoadFont();
        _asciiString = GenerateAsciiString();
    }

    protected override void UnloadContent()
    {
        _fontFromFile.Dispose();
        _spriteBatch.Dispose();

        base.UnloadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        _currentKeyboardState = Keyboard.GetState();
        if (_currentKeyboardState.IsKeyDown(Keys.Escape) || GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
        {
            Exit();
        }

        if (_currentKeyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
        {
            _reloadFont = !_reloadFont;
        }

        if (_reloadFont)
        {
            // We create a new font each frame to start with a new texture atlas.
            LoadFont();
        }

        _previousKeyboardState = _currentKeyboardState;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(20, 24, 32));

        _fontFromFile.Size = 32.0f;

        _spriteBatch.Begin();

        // We start with a small texture atlas.
        _spriteBatch.DrawString(_fontFromFile, "Small texture atlas: This line will bork out on rebuild.", new Vector2(40, 40), Color.Yellow);

        var textureBeforeRebuild = _fontFromFile.GetTexture(0);

        // Much larger atlas is required for all the visible ASCII characters.
        _spriteBatch.DrawString(_fontFromFile, _asciiString, new Vector2(40, 280), Color.White);

        _spriteBatch.DrawString(_fontFromFile, "Large texture atlas: These lines will be drawn after rebuild.", new Vector2(40, 240), Color.White);

        var textureAfterRebuild = _fontFromFile.GetTexture(0);
        
        var isTextureAtlasRebuilt = textureBeforeRebuild != textureAfterRebuild;
        var isOldTextureDisposed = textureBeforeRebuild.IsDisposed;

        _spriteBatch.DrawString(_fontFromFile, "Everything after rebuild works fine.", new Vector2(40, 360), Color.White);

        _spriteBatch.DrawString(_fontFromFile, "Press SPACE to flip texture atlas rebuild on every frame.", new Vector2(40, 600), Color.Yellow);

        _fontFromFile.Size = 16.0f;
        var atlasRebuildMessageColor = isTextureAtlasRebuilt
            ? Color.Red
            : Color.LimeGreen;
        var atlasSizeMessageColor = isTextureAtlasRebuilt
            ? Color.MonoGameOrange
            : Color.LimeGreen;

        if (isTextureAtlasRebuilt)
        {
            var newAtlasSizeMessage = $"New size: {textureAfterRebuild.Width}x{textureAfterRebuild.Height}";
            _spriteBatch.DrawString(_fontFromFile, newAtlasSizeMessage, new Vector2(40, 720), atlasSizeMessageColor);
        }

        var oldAtlasSizeMessage = $"Old size: {textureBeforeRebuild.Width}x{textureBeforeRebuild.Height} (IsDisposed={isOldTextureDisposed})";
        _spriteBatch.DrawString(_fontFromFile, oldAtlasSizeMessage, new Vector2(40, 740), atlasSizeMessageColor);

        var atlasRebuildMessage = isTextureAtlasRebuilt
            ? "Texture atlas is rebuilt."
            : "No rebuild.";
        _spriteBatch.DrawString(_fontFromFile, atlasRebuildMessage, new Vector2(40, 760), atlasRebuildMessageColor);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void LoadFont()
    {
        _fontFromFile?.Dispose();

        string fromFilePath = Path.Combine(AppContext.BaseDirectory, "Content", "JetBrainsMono-Regular.ttf");
        _fontFromFile = DynamicSpriteFont.FromFile(GraphicsDevice, fromFilePath, 32.0f);
    }

    private string GenerateAsciiString()
    {
        var asciiCharacters = Enumerable.Range(0, 255)
            .Select(i => (char)i)
            .Where(c => !char.IsControl(c))
            .ToArray();

        return string.Join(string.Empty, asciiCharacters);
    }
}

