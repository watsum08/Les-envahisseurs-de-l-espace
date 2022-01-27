using System;
namespace SpaceInvaders
{
    /// <summary>
    /// Classe permettant de créer des vecteurs pour la position, etc..
    /// </summary>
    public class Vector2
    {
        /// <summary>
        /// la position X
        /// </summary>
        private double _x;
        /// <summary>
        /// la position Y
        /// </summary>
        private double _y;

        /// <summary>
        /// Getter et Setter de la position X
        /// </summary>
        /// <value>la position X</value>
        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Getter et Setter de la position Y
        /// </summary>
        /// <value>la position Y</value>
        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// Getter de la norme
        /// </summary>
        /// <value>la norme</value>
        public double Norme
        {
            get { return Math.Sqrt((_x * _x) * (_y * _y)); }
        }

        /// <summary>
        /// Constructeur public de la classe<see cref="T:SpaceInvaders.Vector2"/>.
        /// </summary>
        /// <param name="x">la position x</param>
        /// <param name="y">la position y</param>
        public Vector2(double x, double y)
        {
            _x = x;
            _y = y;
        }

    }
}
