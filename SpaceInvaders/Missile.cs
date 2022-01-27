using System;
using System.Drawing;
using System.Collections.Generic;

namespace SpaceInvaders
{
    /// <summary>
    /// Classe permettant d'instancier un missile
    /// hérité de SimpleObject
    /// </summary>
    class Missile : SimpleObject
    {
        /// <summary>
        /// La vitesse du missile
        /// </summary>
        private double _vitesse = 500;

        /// <summary>
        /// Créer une nouvelle instance de la classe <see cref="T:SpaceInvaders.Missile"/>.
        /// </summary>
        /// <param name="spawnPos">position d'apparution</param>
        /// <param name="img">image</param>
        /// <param name="nbLives">nombre de vie</param>
        /// <param name="s">camp</param>
        public Missile(Vector2 spawnPos, Bitmap img, int nbLives, Side s) : base(spawnPos, img, nbLives, s)
        {

        }

        public override void Update(Game gameInstance, double deltaT)
        {
            // si le jeu est en GameState.Play il exécute
            if (gameInstance.state == GameState.Play)
            {
                if (_side == Side.Ally)
                {
                    _position.Y -= _vitesse * deltaT; // missile va vers le haut car c'est le joueur
                }
                else
                {
                    _position.Y += _vitesse * deltaT; // missile va vers le bas car c'est l'ennemi
                }

                // enlève toutes les vies(détruit) missile si il sort du cadre Y
                if (_position.Y + _image.Height < 0 || _position.Y > gameInstance.gameSize.Height)
                {
                    _nbLives = 0;
                }
            }

            // vérifie la collision avec chaque gameobject
            foreach(GameObject gameObject in gameInstance.gameObjects)
            {
                gameObject.Collision(this);
            }
        }

        /// <summary>
        /// Décremente le nombre de vies
        /// </summary>
        /// <param name="n">nombre Int</param>
        public void DecrementLives(int n)
        {
            _nbLives -= n;
        }

        protected override void OnCollision(Missile m, int numberOfPixelsInCollision, List<Vector2> collidingPixelsPoints)
        {
            if (m != this)
            {
                _nbLives = 0;
                m.DecrementLives(m.NbLives);
            }
        }
    }
}
