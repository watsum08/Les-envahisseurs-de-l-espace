using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace SpaceInvaders
{
    /// <summary>
    /// Classe bunker pour se mettre à l'abri
    /// hérité de SimpleObject
    /// </summary>
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
            // chaque pixel en collision avec le missile transforme le pixel touché du bunker en blanc
            foreach(Vector2 point in collidingPixelsPoints)
            {
                _image.SetPixel(Convert.ToInt32(point.X), Convert.ToInt32(point.Y), Color.FromArgb(0, 0, 0, 0));
            }
            // décremente la vie du missile par rapport au nb de pixels en collision
            m.DecrementLives(numberOfPixelsInCollision);
        }
    }
}
