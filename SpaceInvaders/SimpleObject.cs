// Auteur : Marc Meynet
// Classe : CFC-24
// Date de création : 09.12.21
// Classe SimpleObject
using System;
using System.Drawing;
using System.Collections.Generic;

namespace SpaceInvaders
{
    /// <summary>
    /// Classe abstraite de SimpleObject
    /// hérité de GameObject
    /// </summary>
    abstract class SimpleObject : GameObject
    {
        /// <summary>
        /// La position
        /// </summary>
        protected Vector2 _position;
        /// <summary>
        /// L'image
        /// </summary>
        protected Bitmap _image;
        /// <summary>
        /// Le nombre de vies
        /// </summary>
        protected int _nbLives;

        /// <summary>
        /// Getter de Position
        /// </summary>
        /// <value>la position.</value>
        public Vector2 Position
        {
            get { return _position; }
        }
        /// <summary>
        /// Getter d'Image
        /// </summary>
        /// <value>l'image.</value>
        public Bitmap Image
        {
            get { return _image; }
        }
        /// <summary>
        /// Getter du nombre de vies
        /// </summary>
        /// <value>nombre de vies.</value>
        public int NbLives
        {
            get { return _nbLives; }
        }

        /// <summary>
        /// Constructeur protégé de la classe <see cref="T:SpaceInvaders.SimpleObject"/>.
        /// </summary>
        /// <param name="spawnPos">position d'apparution</param>
        /// <param name="img">image</param>
        /// <param name="nbLives">nombre de vies</param>
        /// <param name="s">camp</param>
        protected SimpleObject(Vector2 spawnPos, Bitmap img, int nbLives, Side s)
        {
            _position = new Vector2(spawnPos.X, spawnPos.Y);
            _nbLives = nbLives;
            _image = img;
            _side = s;
        }

        /// <summary>
        /// Dessine l'interface graphique sur l'instance de jeu
        /// </summary>
        /// <param name="gameInstance">instance de jeu</param>
        /// <param name="graphics">interface graphique</param>
        public override void Draw(Game gameInstance, Graphics graphics)
        {
            graphics.DrawImage(_image, (float)_position.X, (float)_position.Y, _image.Width, _image.Height);
        }

        /// <summary>
        /// Vérifie si l'objet est en vie
        /// </summary>
        /// <returns><c>true</c>, si l'objet est en vie, <c>false</c> sinon.</returns>
        public override bool IsAlive()
        {
            if (_nbLives > 0) { return true; }
            return false;
        }

        /// <summary>
        /// Vérifie la collision en vérifiant le rectangle englobant l'objet et si la transparence du pixel est 255
        /// </summary>
        /// <param name="m">missile</param>
        public override void Collision(Missile m)
        {
            if (m != null && m.IsAlive() && m._side != _side)
            {
                if (IsRectColliding(this, m))
                {
                    // crée une nouvelle liste Vector2 pour ajouter les pixels entré en collision
                    List<Vector2> points = new List<Vector2>();
                    int nbPixelsColliding = 0;
                    for (double x = 0; x < _image.Width; x++)
                    {
                        for (double y = 0; y < _image.Height; y++)
                        {
                            if (m.Position.X < _position.X + x &&
                                m.Position.X + m.Image.Width > _position.X + x &&
                                m.Position.Y < _position.Y + y &&
                                m.Position.Y + m.Image.Height > _position.Y + y &&
                                _image.GetPixel(Convert.ToInt32(x), Convert.ToInt32(y)).A == 255)
                            {
                                nbPixelsColliding++;
                                points.Add(new Vector2(x, y));
                            }
                        }
                    }
                    OnCollision(m, nbPixelsColliding, points);
                }
            }
        }

        /// <summary>
        /// Vérifie si un rectangle rentre en collision avec un autre rectangle
        /// </summary>
        /// <returns><c>true</c>, si le rectangle rentre en collision, <c>false</c> sinon.</returns>
        /// <param name="r1">simpleobject1/param>
        /// <param name="r2">simpleobject2</param>
        private bool IsRectColliding(SimpleObject r1, SimpleObject r2)
        {
            Rectangle rect1 = new Rectangle((int)r1.Position.X, (int)r1.Position.Y, r1.Image.Width, r1.Image.Height);
            Rectangle rect2 = new Rectangle((int)r2.Position.X, (int)r2.Position.Y, r2.Image.Width, r2.Image.Height);

            if (rect1.IntersectsWith(rect2))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Appelé en cas de collision
        /// </summary>
        /// <param name="m">missile</param>
        /// <param name="numberOfPixelsInCollision">nombre de pixels entré en  collision</param>
        /// <param name="collidingPixelsPoints">les pixels entrés en collision</param>
        protected abstract void OnCollision(Missile m, int numberOfPixelsInCollision, List<Vector2> collidingPixelsPoints);
    }
}
