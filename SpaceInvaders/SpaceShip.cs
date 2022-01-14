using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SpaceInvaders
{
    class SpaceShip : SimpleObject
    {
        private Missile _missile;

        private double _speedPixelPerSecond = 30;

        public double SpeedPixelPerSecond
        {
            get { return _speedPixelPerSecond; }
            set { _speedPixelPerSecond = value; }
        }

        public SpaceShip(Vector2 spawnPos, Bitmap img, int nbLives) : base(spawnPos, img, nbLives)
        {

        }

        public override void Update(Game gameInstance, double deltaT)
        {
            // Si le jeu est en GameState.Play il exécute
            if (gameInstance.state == GameState.Play)
            {
               
            }
        }

        public void Shoot(Game gameIns)
        {
            if (_missile == null || !_missile.IsAlive())
            {
                int missileLives = 10;
                Bitmap missileImg = SpaceInvaders.Properties.Resources.shoot1;
                _missile = new Missile(new Vector2(_position.X + _image.Width / 2 - 1, _position.Y - missileImg.Height), missileImg, missileLives);
                gameIns.AddNewGameObject(_missile);
            }
        }

        protected override void OnCollision(Missile m, int numberOfPixelsInCollision, List<Vector2> collidingPixelsPoints)
        {
            _nbLives = 0;
        }
    }
}
