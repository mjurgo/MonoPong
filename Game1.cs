using System;
using System.Diagnostics;
using System.Security.Cryptography.Pkcs;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;

namespace MonoPong;

public class Game1 : Game
{
    private const int Width = 640;
    private const int Height = 480;
    private const int Velocity = 500;
    private const int BallSpeed = 300;
    private const int CompSpeed = 250;
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private SpriteFont _font;

    private readonly Point _centralPoint = new Point(Width / 2, Height / 2);

    private Rectangle _playerRect;
    private Rectangle _compRect;
    private Rectangle _ballRect;
    private Texture2D _tPaddle;

    private int _ballXDir = -1;
    private int _ballYDir = -1;

    private int _playerScore = 0;
    private int _compScore = 0;
    private bool _ballMoving = true;
    private Stopwatch _scoreStopwatch = new Stopwatch();

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
        _compRect = new Rectangle(20, 200, 20, 80);
        _playerRect = new Rectangle(600, 200, 20, 80);
        _ballRect = new Rectangle(310, 230, 20, 20);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _tPaddle = new Texture2D(GraphicsDevice, 1, 1);
        _tPaddle.SetData(new[] { Color.White });

        _font = Content.Load<SpriteFont>("MainFont");
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
        if (_ballMoving)
        {
            _ballRect.X = (int)MathHelper.Clamp(_ballRect.X + BallSpeed * _ballXDir * delta, 0f, Width - _ballRect.Width);
            _ballRect.Y = (int)MathHelper.Clamp(_ballRect.Y + BallSpeed * _ballYDir * delta, 0f, Height - _ballRect.Height);    
        }
        
        if (_ballRect.X <= 0 || _ballRect.X >= Width - _ballRect.Width)
        {
            _ballXDir *= -1;
        }
        if (_ballRect.Y <= 0 || _ballRect.Y >= Height - _ballRect.Height)
        {
            _ballYDir *= -1;
        }

        CheckBallPaddleIntersection(_playerRect);
        CheckBallPaddleIntersection(_compRect);
        
        // ==== COMPUTER LOGIC =====//
        if (_ballRect.Center.Y < _compRect.Center.Y)
        {
            _compRect.Y = (int)MathHelper.Clamp(_compRect.Y - CompSpeed * delta, 0f, Height - _compRect.Height);
        }
        if (_ballRect.Center.Y > _compRect.Center.Y)
        {
            _compRect.Y = (int)MathHelper.Clamp(_compRect.Y + CompSpeed * delta, 0f, Height - _compRect.Height);
        }

        if (_ballRect.Left < _compRect.Left)
        {
            _playerScore += 1;
            restartAfterPointScored();
        }

        if (_ballRect.Right > _playerRect.Right)
        {
            _compScore += 1;
            restartAfterPointScored();
        }

        if (_scoreStopwatch.ElapsedMilliseconds >= 3000)
        {
            _scoreStopwatch.Reset();
            _ballMoving = true;
        }

        base.Update(gameTime);
    }

    private void restartAfterPointScored()
    {
        _ballRect.Location = _centralPoint;
        _ballXDir *= -1;
        _ballMoving = false;
        _scoreStopwatch.Start();
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();
        
        _spriteBatch.Draw(_tPaddle, _playerRect, Color.White);
        _spriteBatch.Draw(_tPaddle, _compRect, Color.White);
        _spriteBatch.Draw(_tPaddle, _ballRect, Color.White);
        
        _spriteBatch.DrawString(_font, _compScore.ToString(), new Vector2(280, 0), Color.White);
        _spriteBatch.DrawString(_font, _playerScore.ToString(), new Vector2(360, 0), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
    
    private void CheckBallPaddleIntersection(Rectangle paddle)
    {
        if (_ballRect.Intersects(paddle))
        {
            if (_ballRect.Center.Y < paddle.Center.Y)
            {
                _ballYDir = -1;
            }
            else if (_ballRect.Center.Y > paddle.Center.Y)
            {
                _ballYDir = 1;
            }
            else
            {
                _ballYDir = 0;
            }
            _ballXDir *= -1;
            _ballRect.X = paddle.X + paddle.Width * _ballXDir + _ballXDir;
        }
    }
}