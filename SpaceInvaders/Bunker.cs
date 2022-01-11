using System;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvaders
{
    class Bunker : SimpleObject
    {
        public Bunker(Vector2 spawnPos) : base(spawnPos, SpaceInvaders.Properties.Resources.bunker, 1)
        {

        }

        public override void Update(Game gameInstance, double deltaT)
        {

        }

        public override void Collision(Missile m)
        {
            if (m != null && m.IsAlive())
            {
                if (IsRectColliding(this, m))
                {
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
                                _image.SetPixel(Convert.ToInt32(x), Convert.ToInt32(y), Color.FromArgb(0, 0, 0, 0));
                                m.DecrementLives(1);
                            }
                        }
                    }
                }
            }
        }

        private bool IsRectColliding(SimpleObject r1, SimpleObject r2)
        {
            if (r1.Position.X < r2.Position.X + r2.Image.Width &&
                r1.Position.X + r1.Image.Width > r2.Position.X &&
                r1.Position.Y < r2.Position.Y + r2.Image.Height &&
                r1.Position.Y + r1.Image.Height > r2.Position.Y)
            {
                return true;
            }
            return false;
        }
    }
}
