using System.Drawing;

namespace SpaceInvaders
{
    /// <summary>
    /// Ceci est la classe abstraite de tout objet de jeu 
    /// </summary>
    abstract class GameObject
    {
        /// <summary>
        /// Ceci est l'énumération pour les camp de jeu (Allié, Ennemi, Neutre)
        /// </summary>
        public enum Side
        {
            Ally,
            Enemy,
            Neutral
        }

        /// <summary>
        /// Camp de l'objet de jeu
        /// </summary>
        protected Side _side;

        /// <summary>
        /// Constructeur protégé de la classe <see cref="T:SpaceInvaders.GameObject"/>.
        /// </summary>
        protected GameObject()
        {

        }

        /// <summary>
        /// Met à jour l'état de l'objet de jeu
        /// </summary>
        /// <param name="gameInstance">l'instance du jeu actuel</param>
        /// <param name="deltaT">temps en secondes depuis le dernier appel de la mise à jour Update()</param>
        public abstract void Update(Game gameInstance, double deltaT);

        /// <summary>
        /// Dessine l'objet de jeu (affiche à l'écran)
        /// </summary>
        /// <param name="gameInstance">l'instance du jeu actuel</param>
        /// <param name="graphics">l'objet graphique où il faut dessiner</param>
        public abstract void Draw(Game gameInstance, Graphics graphics);

        /// <summary>
        /// Détermine si l'objet de jeu est vivant. Si cela retourne faux, l'objet est automatiquement supprimé.
        /// </summary>
        /// <returns>Suis-je vivant ?</returns>
        public abstract bool IsAlive();

        /// <summary>
        /// Détermine si l'objet de jeu rentre en collision avec le missile
        /// </summary>
        public abstract void Collision(Missile m);
    }
}
