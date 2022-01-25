using System;
using System.Drawing;
using System.Collections.Generic;

namespace SpaceInvaders
{
    class SpaceShip : SimpleObject
    {
        private Missile _missile;

        private double _speedPixelPerSecond = 50;

        public double SpeedPixelPerSecond
        {
            get { return _speedPixelPerSecond; }
            set { _speedPixelPerSecond = value; }
        }

        public SpaceShip(Vector2 spawnPos, Bitmap img, int nbLives, Side s) : base(spawnPos, img, nbLives, s)
        {

        }

        public override void Update(Game gameInstance, double deltaT)
        {

        }

        public void Shoot(Game gameIns)
        {
            if (_missile == null || !_missile.IsAlive())
            {
                int missileLives = 20;
                Bitmap missileImg = SpaceInvaders.Properties.Resources.shoot1;
                _missile = new Missile(new Vector2(_position.X + _image.Width / 2 - 1, _position.Y - missileImg.Height), missileImg, missileLives, _side);
                gameIns.AddNewGameObject(_missile);
            }
        }

        protected override void OnCollision(Missile m, int numberOfPixelsInCollision, List<Vector2> collidingPixelsPoints)
        {
            if (m.NbLives <= _nbLives)
            {
                _nbLives -= m.NbLives;
                m.DecrementLives(m.NbLives);
            }
            else
            {
                m.DecrementLives(_nbLives);
                _nbLives = 0;
            }
        }

        public void Kill()
        {
            _nbLives = 0;
        }
    }
}
