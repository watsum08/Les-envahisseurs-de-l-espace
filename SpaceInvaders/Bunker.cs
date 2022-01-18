using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SpaceInvaders
{
    class Bunker : SimpleObject
    {
        public Bunker(Vector2 spawnPos, Side s) : base(spawnPos, SpaceInvaders.Properties.Resources.croissant, 1, s)
        {

        }

        public override void Update(Game gameInstance, double deltaT)
        {

        }

        protected override void OnCollision(Missile m, int numberOfPixelsInCollision, List<Vector2> collidingPixelsPoints)
        {
            foreach(Vector2 point in collidingPixelsPoints)
            {
                _image.SetPixel(Convert.ToInt32(point.X), Convert.ToInt32(point.Y), Color.FromArgb(0, 0, 0, 0));
            }

            m.DecrementLives(numberOfPixelsInCollision);
        }
    }
}
