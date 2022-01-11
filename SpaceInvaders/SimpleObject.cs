using System;
using System.Drawing;

namespace SpaceInvaders
{
    abstract class SimpleObject : GameObject
    {
        protected Vector2 _position;
        protected Bitmap _image;
        protected int _nbLives;

        public Vector2 Position
        {
            get { return _position; }
        }
        public Bitmap Image
        {
            get { return _image; }
        }
        public int NbLives
        {
            get { return _nbLives; }
        }

        protected SimpleObject(Vector2 spawnPos, Bitmap img, int nbLives)
        {
            _position = new Vector2(spawnPos.X, spawnPos.Y);
            _nbLives = nbLives;
            _image = img;
        }

        public override void Draw(Game gameInstance, Graphics graphics)
        {
            graphics.DrawImage(_image, (float)_position.X, (float)_position.Y, _image.Width, _image.Height);
        }

        public override bool IsAlive()
        {
            if (_nbLives > 0) { return true; } else return false;
        }

        public override void Collision(Missile m)
        {

        }
    }
}
