using System;
using System.Security.Cryptography.Pkcs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoPong;

public class Game1 : Game
{
    private const int Width = 640;
    private const int Height = 480;
    private const int Velocity = 500;
    private const int BallSpeed = 200; 
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Rectangle _playerRect;
    private Rectangle _compRect;
    private Rectangle _ballRect;
    private Texture2D _tPaddle;

    private int _ballXDir = -1;
    private int _ballYDir = -1;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        _graphics.PreferredBackBufferWidth = Width;
        _graphics.PreferredBackBufferHeight = Height;
    }

    protected override void Initialize()
    {
        _playerRect = new Rectangle(20, 200, 20, 80);
        _compRect = new Rectangle(600, 200, 20, 80);
        _ballRect = new Rectangle(310, 230, 20, 20);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _tPaddle = new Texture2D(GraphicsDevice, 1, 1);
        _tPaddle.SetData(new[] { Color.White });
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

        KeyboardState kState = Keyboard.GetState();

        if (kState.IsKeyDown(Keys.Up))
        {
            _playerRect.Y = (int)MathHelper.Clamp(_playerRect.Y - Velocity * delta, 0f, Height - _playerRect.Height);
        }
        else if (kState.IsKeyDown(Keys.Down))
        {
            _playerRect.Y = (int)MathHelper.Clamp(_playerRect.Y + Velocity * delta, 0f, Height - _playerRect.Height);
        }
        
        //===== BALL LOGIC =====//
        _ballRect.X = (int)MathHelper.Clamp(_ballRect.X + BallSpeed * _ballXDir * delta, 0f, Width - _ballRect.Width);
        _ballRect.Y = (int)MathHelper.Clamp(_ballRect.Y + BallSpeed * _ballYDir * delta, 0f, Height - _ballRect.Height);

        if (_ballRect.X <= 0 || _ballRect.X >= Width - _ballRect.Width)
        {
            _ballXDir *= -1;
        }
        if (_ballRect.Y <= 0 || _ballRect.Y >= Height - _ballRect.Height)
        {
            _ballYDir *= -1;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        
        _spriteBatch.Draw(_tPaddle, _playerRect, Color.White);
        _spriteBatch.Draw(_tPaddle, _compRect, Color.White);
        _spriteBatch.Draw(_tPaddle, _ballRect, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}