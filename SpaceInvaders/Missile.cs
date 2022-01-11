using System;
using System.Drawing;
namespace SpaceInvaders
{
    class Missile : SimpleObject
    {
        private double _vitesse = 200;

        public Missile(Vector2 spawnPos, Bitmap img, int nbLives) : base(spawnPos, img, nbLives)
        {

        }

        public override void Update(Game gameInstance, double deltaT)
        {
            // Si le jeu est en GameState.Play il exécute
            if (gameInstance.state == GameState.Play)
            {
                _position.Y -= _vitesse * deltaT; // missile va vers le haut

                // Enlève toutes les vies(détruit) missile si il sort du cadre Y
                if (_position.Y + _image.Height < 0)
                {
                    _nbLives = 0;
                }
            }

            foreach(GameObject gameObject in gameInstance.gameObjects)
            {
                gameObject.Collision(this);
            }
        }

        public override void Collision(Missile m)
        {

        }

        public void DecrementLives(int n)
        {
            _nbLives -= n;
        }
    }
}
