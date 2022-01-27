using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace SpaceInvaders
{
    /// <summary>
    /// Cette classe représente le bloc d'ennemis
    /// </summary>
    class EnemyBlock : GameObject
    {
        /// <summary>
        /// Liste des vaisseaux ennemis contenu
        /// </summary>
        private HashSet<SpaceShip> _enemyShips;
        /// <summary>
        /// Largeur de base du bloc
        /// </summary>
        private int _baseWidth;
        /// <summary>
        /// Marge du bas des vaisseaux
        /// </summary>
        private int _bottomShipMargin;
        /// <summary>
        /// Taille du bloc
        /// </summary>
        private Size _size;
        /// <summary>
        /// Position du bloc
        /// </summary>
        private Vector2 _position;
        /// <summary>
        /// Direction du bloc
        /// </summary>
        private int _direction;
        /// <summary>
        /// Vitesse horizontale du bloc
        /// </summary>
        private double _horSpeed;
        /// <summary>
        /// Vitesse verticale du bloc
        /// </summary>
        private double _verSpeed;
        /// <summary>
        /// Taux de probabilité de tir
        /// </summary>
        private double _randomShootProbability;

        /// <summary>
        /// Getter et Setter de la taille du bloc
        /// </summary>
        public Size Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Getter et Setter de la position du bloc
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        /// <summary>
        /// Constructeur publique
        /// </summary>
        /// <param name="spawnPos">position d'instanciation</param>
        /// <param name="blockWidth">largeur de base du bloc</param>
        /// <param name="direction">direction de base du bloc</param>
        public EnemyBlock(Vector2 spawnPos, int blockWidth, int direction)
        {
            _position = new Vector2(spawnPos.X, spawnPos.Y);
            _bottomShipMargin = 16;
            _size.Width = blockWidth;
            _size.Height = _bottomShipMargin * -1;
            _baseWidth = _size.Width;
            _enemyShips = new HashSet<SpaceShip>();
            _direction = direction;
            _side = Side.Enemy;
            _randomShootProbability = 0.02f;
            _horSpeed = 50;
            _verSpeed = 4000;
        }

        public override void Update(Game gameInstance, double deltaT)
        {
            // Si le jeu est en GameState.Play il exécute
            if (gameInstance.state == GameState.Play)
            {
                if (_position.X + _size.Width >= gameInstance.gameSize.Width)
                {
                    _direction = -1; // va à Gauche

                    _position.Y += _verSpeed * deltaT;
                    _horSpeed += 2;
                    foreach (SpaceShip spaceship in _enemyShips)
                    {
                      spaceship.Position.Y += _verSpeed * deltaT;
                      _randomShootProbability += 0.005f;
                    }

                }
                else if (_position.X <= 0)
                {
                    _direction = 1; // va à Droite

                    _position.Y += _verSpeed * deltaT;
                    _horSpeed += 2;
                    foreach (SpaceShip spaceship in _enemyShips)
                    {
                       spaceship.Position.Y += _verSpeed * deltaT;
                        _randomShootProbability += 0.005f;
                    }
                }

                // pour chaque spaceship dans les vaisseaux ennemis on crée un chiffre aléatoire 
                foreach (SpaceShip spaceship in _enemyShips.ToList())
                {
                    spaceship.Position.X += _horSpeed * deltaT * _direction;

                    // crée un chiffre(seed) aléatoire
                    Random randSeed = new Random();
                    // prend le prochain double du seed randSeed
                    double r = randSeed.NextDouble();

                    // tir aléatoire
                    if (r <= _randomShootProbability * deltaT) // tir aléatoire
                    {
                        spaceship.Shoot(gameInstance);
                    }

                    // enlève le vaisseau de la liste si il est mort
                    if (!spaceship.IsAlive())
                    {
                        _enemyShips.Remove(spaceship);
                    }
                }
                _position.X += _horSpeed * deltaT * _direction;

                if (_position.Y + _size.Height >= gameInstance.playerShip.Position.Y)
                {
                    gameInstance.playerShip.Kill();
                }

                UpdateSize();
            }
        }

        public override void Draw(Game gameInstance, Graphics graphics)
        {
            foreach (SpaceShip spaceship in _enemyShips)
            {
                spaceship.Draw(gameInstance, graphics);
            }
            /*// Permet de dessiner le rectangle entourant l'enemyblock(tous les ennemis)
            Pen pen = new Pen(Color.Red, 2);
            int x = (int)_position.X;
            int y = (int)_position.Y;

            graphics.DrawLine(pen, x, y, x + _size.Width, y);
            graphics.DrawLine(pen, x, y, x, _size.Height + y);
            graphics.DrawLine(pen, x + _size.Width, y, x + _size.Width, _size.Height + y);
            graphics.DrawLine(pen, x, y + _size.Height, x + _size.Width, y + _size.Height);*/
        }

        public override bool IsAlive()
        {
            foreach(SpaceShip spaceship in _enemyShips)
            {
                if (spaceship.IsAlive())
                {
                    return true;
                }
            }
            return false;
        }

        public override void Collision(Missile m)
        {
            foreach (SpaceShip spaceship in _enemyShips)
            {
                spaceship.Collision(m);
            }
        }

        /// <summary>
        /// Ajoute une ligne d'ennemis.
        /// </summary>
        /// <param name="nbShips">nombre de vaisseaux</param>
        /// <param name="nbLives">nombre de vies</param>
        /// <param name="shipImage">image du vaisseau</param>
        public void AddLine(int nbShips, int nbLives, Bitmap shipImage)
        {
            for (int ship = 0; ship < nbShips; ship++)
            {
                SpaceShip enemyship = new SpaceShip(new Vector2(_position.X + (_baseWidth / nbShips * (ship + 1)) - (_baseWidth / nbShips) + (_baseWidth / (nbShips * 2)) - (shipImage.Width/2), _position.Y + _size.Height + _bottomShipMargin), shipImage, nbLives, Side.Enemy);
                _enemyShips.Add(enemyship);
            }
            _size.Height += shipImage.Height + _bottomShipMargin;
        }

        /// <summary>
        /// Mets à jour la taille du bloc.
        /// </summary>
        private void UpdateSize()
        {
            double minX = 1000;
            double maxX = 0;
            // permet de mettre à jour la taille du bloc par rapport aux vaisseaux ennemis restants
            foreach (SpaceShip enemyShip in _enemyShips)
            {
                if (enemyShip.Position.X < minX)
                {
                    minX = enemyShip.Position.X;
                }
                else if (enemyShip.Position.X + enemyShip.Image.Width > maxX)
                {
                    maxX = enemyShip.Position.X + enemyShip.Image.Width;
                }
            }

            if (_enemyShips.Count() > 1)
            {
                _size.Width = Convert.ToInt32(maxX - minX);
                _position.X = minX;
            }
            else
            {
                _size.Width = _enemyShips.First().Image.Width;
                _position.X = _enemyShips.First().Position.X;
            }
        }
        /* // Permet de tuer tous les ennemis de _enemyShips
        public void KillAllShips()
        {
            foreach (SpaceShip enemyShip in _enemyShips)
            {
                enemyShip.Kill();
            }
        }*/
    }
}
