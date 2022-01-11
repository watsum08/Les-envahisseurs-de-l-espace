using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvaders
{
    class SpaceShip : SimpleObject
    {
        private double _speedPixelPerSecond = 300;

        private Missile _missile;

        public Missile Missile
        {
            get { return _missile; }
            set { _missile = value; }
        }

        public SpaceShip(Vector2 spawnPos, Bitmap img, int nbLives) : base(spawnPos, img, nbLives)
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

        public void Shoot(Game gameIns)
        {
            if (_missile == null || !_missile.IsAlive())
            {
                int missileLives = 10;
                _missile = new Missile(new Vector2(_position.X + _image.Width / 2 - 1, _position.Y), SpaceInvaders.Properties.Resources.shoot1, missileLives);
                gameIns.AddNewGameObject(_missile);
            }
        }

        public override void Collision(Missile m)
        {

        }
    }
}
