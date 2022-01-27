using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvaders
{
    /// <summary>
    /// Classe permettant d'instancier le vaisseau joueur
    /// hérité de SpaceShip
    /// </summary>
    class PlayerSpaceship : SpaceShip
    {
        /// <summary>
        /// Vitesse de déplacement (px par seconde)
        /// </summary>
        private double _speedPixelPerSecond = 300;

        /// <summary>
        /// Créer une nouvelle instance de la classe <see cref="T:SpaceInvaders.PlayerSpaceship"/>.
        /// </summary>
        /// <param name="spawnPos">position d'apparution</param>
        /// <param name="img">image</param>
        /// <param name="nbLives">nombre de vies</param>
        /// <param name="s">camp</param>
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
            // reprend la méthode du parent
            base.Draw(gameInstance, graphics);

            // dessine "VIE:" à l'écran
            graphics.DrawString(
                    "VIE:",
                    Game.defaultFont,
                    Game.blackBrush,
                    new RectangleF(40, gameInstance.gameSize.Height - 59, Game.m_SubHeaderTextFieldSize.Width, Game.m_SubHeaderTextFieldSize.Height));

            // crée le stylo
            Pen pen;
            if (_nbLives > _nbLives * 0.4) // change la couleur du stylo par rapport au nombre de vies
            {
                pen = new Pen(Color.LightGreen, 16);
            } 
            else if (_nbLives <= _nbLives * 0.4 && _nbLives > _nbLives * 0.15)
            {
                pen = new Pen(Color.Yellow, 16);
            }
            else
            {
                pen = new Pen(Color.Red, 16);
            }

            // dessine la barre de vie du joueur
            graphics.DrawLine(pen, 95, gameInstance.gameSize.Height - 46, 95 + _nbLives * 2, gameInstance.gameSize.Height - 46);

        }
    }
}
