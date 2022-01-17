using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvaders
{
    class PlayerSpaceship : SpaceShip
    {
        private double _speedPixelPerSecond = 300;

        public PlayerSpaceship(Vector2 spawnPos, Bitmap img, int nbLives, Side s) : base(spawnPos, img, nbLives, s)
        {

        }

        public override void Update(Game gameInstance, double deltaT)
        {
            // Si le jeu est en GameState.Play il exécute
            if (gameInstance.state == GameState.Play)
            {
                // if Left Arrow is pressed
                if (gameInstance.keyPressed.Contains(Keys.Left))
                {
                    _position.X -= _speedPixelPerSecond * deltaT;
                }


                // if Right Arrow is pressed
                if (gameInstance.keyPressed.Contains(Keys.Right))
                {
                    _position.X += _speedPixelPerSecond * deltaT;
                }


                // if Space Bar is pressed
                if (gameInstance.keyPressed.Contains(Keys.Space))
                {
                    Shoot(gameInstance);
                }


                //border player position block
                if (_position.X < 0)
                {
                    _position.X = 0;
                }
                else if (_position.X + _image.Width > gameInstance.gameSize.Width)
                {
                    _position.X = gameInstance.gameSize.Width - _image.Width;
                }
            }
        }

        public override void Draw(Game gameInstance, Graphics graphics)
        {
            base.Draw(gameInstance, graphics);

            graphics.DrawString(
                    "Vie : ",
                    Game.defaultFont,
                    Game.blackBrush,
                    new RectangleF(30, gameInstance.gameSize.Height - 70, Game.m_SubHeaderTextFieldSize.Width, Game.m_SubHeaderTextFieldSize.Height));

            Pen pen;
            if (_nbLives > 40)
            {
                pen = new Pen(Color.Green, 16);
            } 
            else if (_nbLives <= 40 && _nbLives > 20)
            {
                pen = new Pen(Color.Yellow, 16);
            }
            else
            {
                pen = new Pen(Color.Red, 16);
            }


            graphics.DrawLine(pen, 90, gameInstance.gameSize.Height - 50, 90 + _nbLives*2, gameInstance.gameSize.Height - 50);

        }

        public void Kill()
        {
            _nbLives = 0;
        }
    }
}
