using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SpaceInvaders
{
    class EnemyBlock : GameObject
    {
        private HashSet<SpaceShip> _enemyShips;
        private int _baseWidth;
        public Size _size;
        public Vector2 _position;
        private int _direction;
        private double _speed;

        public EnemyBlock(Vector2 spawnPos, int blockWidth, int direction)
        {
            _position = new Vector2(spawnPos.X, spawnPos.Y);
            _size.Width = blockWidth;
            _size.Height = 0;
            _baseWidth = _size.Width;
            _enemyShips = new HashSet<SpaceShip>();
            _direction = direction;
        }

        public override void Update(Game gameInstance, double deltaT)
        {
            if (_position.X + _size.Width >= gameInstance.gameSize.Width)
            {
                _direction = -1; // goes Left
                _position.Y += 20;
                _speed += 10;
                foreach (SpaceShip spaceship in _enemyShips)
                {
                    spaceship.Position.Y += 20;
                }
            }
            else if (_position.X <= 0)
            {
                _direction = 1; // goes Right
                _position.Y += 20;
                _speed += 10;
                foreach (SpaceShip spaceship in _enemyShips)
                {
                    spaceship.Position.Y += 20;
                }
            }


            foreach (SpaceShip spaceship in _enemyShips.ToList())
            {
                spaceship.Position.X += _speed * deltaT * _direction;

                if (spaceship.IsAlive() == false)
                {
                    _enemyShips.Remove(spaceship);
                }
            }
            _position.X += _speed * deltaT * _direction;
        }

        public override void Draw(Game gameInstance, Graphics graphics)
        {
            foreach(SpaceShip spaceship in _enemyShips)
            {
                spaceship.Draw(gameInstance, graphics);
            }

            Pen pen = new Pen(Color.Red, 2);
            int x = (int)_position.X;
            int y = (int)_position.Y;

            graphics.DrawLine(pen, x, y, x + _size.Width, y);
            graphics.DrawLine(pen, x, y, x, _size.Height + y);
            graphics.DrawLine(pen, x + _size.Width, y, x + _size.Width, _size.Height + y);
            graphics.DrawLine(pen, x, y + _size.Height, x + _size.Width, y + _size.Height);
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

        public void AddLine(int nbShips, int nbLives, Bitmap shipImage)
        {
            for (int ship = 0; ship < nbShips; ship++)
            {
                SpaceShip enemyship = new SpaceShip(new Vector2(_position.X + (_baseWidth / nbShips * (ship + 1)) - (_baseWidth / nbShips) + (_baseWidth / (nbShips * 2)) - (shipImage.Width/2), _position.Y + _size.Height), shipImage, nbLives);
                _speed = enemyship.SpeedPixelPerSecond;
                _enemyShips.Add(enemyship);
            }
            _size.Height += shipImage.Height;
        }

        private void UpdateSize()
        {

        }
    }
}
