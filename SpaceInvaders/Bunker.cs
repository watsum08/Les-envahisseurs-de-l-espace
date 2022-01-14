using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

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
