using System;
namespace SpaceInvaders
{
    public class Vector2
    {
        private double _x;
        private double _y;

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Norme
        {
            get { return Math.Sqrt((_x * _x) * (_y * _y)); }
        }

        public Vector2(double x, double y)
        {
            _x = x;
            _y = y;
        }

    }
}
