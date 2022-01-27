using System;
using System.Drawing;
using System.Collections.Generic;

namespace SpaceInvaders
{
    /// <summary>
    /// Classe de SpaceShip
    /// hérité de SimpleObject
    /// </summary>
    class SpaceShip : SimpleObject
    {
        /// <summary>
        /// Le missile
        /// </summary>
        private Missile _missile;

        /// <summary>
        /// Constructeur public de la classe <see cref="T:SpaceInvaders.SpaceShip"/>.
        /// </summary>
        /// <param name="spawnPos">position d'apparution</param>
        /// <param name="img">image</param>
        /// <param name="nbLives">nombre de vies</param>
        /// <param name="s">camp</param>
        public SpaceShip(Vector2 spawnPos, Bitmap img, int nbLives, Side s) : base(spawnPos, img, nbLives, s)
        {

        }

        public override void Update(Game gameInstance, double deltaT)
        {

        }

        /// <summary>
        /// Tir un missile dans l'instance de jeu.
        /// </summary>
        /// <param name="gameIns">instance de jeu</param>
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

        /// <summary>
        /// Tue cet instance.
        /// </summary>
        public void Kill()
        {
            _nbLives = 0;
        }
    }
}
