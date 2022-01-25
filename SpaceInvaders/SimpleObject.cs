using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows;

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

        protected SimpleObject(Vector2 spawnPos, Bitmap img, int nbLives, Side s)
        {
            _position = new Vector2(spawnPos.X, spawnPos.Y);
            _nbLives = nbLives;
            _image = img;
            _side = s;
        }

        public override void Draw(Game gameInstance, Graphics graphics)
        {
            graphics.DrawImage(_image, (float)_position.X, (float)_position.Y, _image.Width, _image.Height);
        }

        public override bool IsAlive()
        {
            if (_nbLives > 0) { return true; }
            return false;
        }

        public override void Collision(Missile m)
        {
            if (m != null && m.IsAlive() && m._side != _side)
            {
                if (IsRectColliding(this, m))
                {
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

        protected abstract void OnCollision(Missile m, int numberOfPixelsInCollision, List<Vector2> collidingPixelsPoints);
    }
}
