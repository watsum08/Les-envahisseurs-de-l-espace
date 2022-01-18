using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace SpaceInvaders
{
    enum GameState
    {
        Play,
        Pause,
        Win,
        Lost
    }
    /// <summary>
    /// This class represents the entire game, it implements the singleton pattern
    /// </summary>
    class Game
    {

        #region GameObjects management
        /// <summary>
        /// Set of all game objects currently in the game
        /// </summary>
        public HashSet<GameObject> gameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Set of new game objects scheduled for addition to the game
        /// </summary>
        private HashSet<GameObject> pendingNewGameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Schedule a new object for addition in the game.
        /// The new object will be added at the beginning of the next update loop
        /// </summary>
        /// <param name="gameObject">object to add</param>
        public void AddNewGameObject(GameObject gameObject)
        {
            pendingNewGameObjects.Add(gameObject);
        }
        #endregion

        #region game technical elements
        /// <summary>
        /// Size of the game area
        /// </summary>
        public Size gameSize;

        /// <summary>
        /// Game State(pause, en jeu, etc..)
        /// </summary>

        public GameState state = GameState.Play;

        /// <summary>
        /// State of the keyboard
        /// </summary>
        public HashSet<Keys> keyPressed = new HashSet<Keys>();

        #endregion

        #region static fields (helpers)

        /// <summary>
        /// Singleton for easy access
        /// </summary>
        public static Game game { get; private set; }

        /// <summary>
        /// A shared black brush
        /// </summary>
        public static Brush blackBrush = new SolidBrush(Color.FromArgb(250, 240, 240, 240));

        public static Size m_SubHeaderTextFieldSize = new Size(300, 100); //Taille du texte

        /// <summary>
        /// A shared simple font
        /// </summary>
        public static Font defaultFont = new Font("Ubuntu Mono", 16);
        #endregion

        private Font font24px = new Font("Ubuntu Mono", 24);
        private Font font32px = new Font("Ubuntu Mono", 32);


        #region constructors
        /// <summary>
        /// Singleton constructor
        /// </summary>
        /// <param name="gameSize">Size of the game area</param>
        /// 
        /// <returns></returns>
        public static Game CreateGame(Size gameSize)
        {
            if (game == null)
                game = new Game(gameSize);
            return game;
        }

        public PlayerSpaceship playerShip;
        private EnemyBlock _enemies;
        private Stopwatch _stopwatch;
        private int timer;
        private bool _gameOver;
        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="gameSize">Size of the game area</param>
        private Game(Size gameSize)
        {
            this.gameSize = gameSize;

            GameStart();
        }

        private void GameStart()
        {
            timer = 0;
            _gameOver = false;
            gameObjects.Clear(); //supprime tous les objets de jeu pour quand on recommence le jeu

            playerShip = new PlayerSpaceship(new Vector2(gameSize.Width / 2 - SpaceInvaders.Properties.Resources.ship3.Width / 2, gameSize.Height - 120), SpaceInvaders.Properties.Resources.ship3, 60, GameObject.Side.Ally); // Instancie le vaisseau du joueur
            AddNewGameObject(playerShip); //Ajoute le vaisseau du joueur dans la liste des Game Objects

            Random r = new Random();
            double randDirection = r.NextDouble();
            Vector2 enemyBlockSpawnPos = new Vector2((gameSize.Width - 400) / 2, 50);
            int enemyBlockWidth = 400;
            if (randDirection > 0.5)
            {
                _enemies = new EnemyBlock(enemyBlockSpawnPos, enemyBlockWidth, 1);
            }
            else
            {
                _enemies = new EnemyBlock(enemyBlockSpawnPos, enemyBlockWidth, -1);
            }
            _enemies.AddLine(5, 12, SpaceInvaders.Properties.Resources.ship7);
            _enemies.AddLine(3, 10, SpaceInvaders.Properties.Resources.ship5);
            _enemies.AddLine(10, 5, SpaceInvaders.Properties.Resources.ship6);
            AddNewGameObject(_enemies);

            List<Bunker> bunkers = new List<Bunker>();
            for (int i = 1; i < 4; i++)
            {
                bunkers.Add(new Bunker(new Vector2(gameSize.Width / 4 * i - SpaceInvaders.Properties.Resources.bunker.Width/2, gameSize.Height - 240), GameObject.Side.Neutral));
                AddNewGameObject(bunkers[i-1]);
            }

            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        #endregion

        #region methods

        /// <summary>
        /// Force a given key to be ignored in following updates until the user
        /// explicitily retype it or the system autofires it again.
        /// </summary>
        /// <param name="key">key to ignore</param>
        public void ReleaseKey(Keys key)
        {
            keyPressed.Remove(key);
        }

        /// <summary>
        /// Draw the whole game
        /// </summary>
        /// <param name="g">Graphics to draw in</param>
        public void Draw(Graphics graphics)
        {
            // Dessine l'arrière plan
            graphics.DrawImage(SpaceInvaders.Properties.Resources.background, 0, 0, 706, 652);

            foreach (GameObject gameObject in gameObjects)
                gameObject.Draw(this, graphics);

            CheckGameState(graphics);
        }

        /// <summary>
        /// Update game
        /// </summary>
        public void Update(double deltaT)
        {
            // add new game objects
            gameObjects.UnionWith(pendingNewGameObjects);
            pendingNewGameObjects.Clear();


            // update each game object
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update(this, deltaT);
            }

            // remove dead objects
            gameObjects.RemoveWhere(gameObject => !gameObject.IsAlive());
        
            // mettre en pause / play
            if (keyPressed.Contains(Keys.P) && state == GameState.Play)
            {
                state = GameState.Pause;
            }
            else if (keyPressed.Contains(Keys.P) && state == GameState.Pause)
            {
                state = GameState.Play;
            }
            ReleaseKey(Keys.P);

            if (keyPressed.Contains(Keys.K) && state == GameState.Play)
            {
                _enemies.KillAllShips();
            }
            ReleaseKey(Keys.K);

            if (!playerShip.IsAlive())
            {
                state = GameState.Lost;
            } 
            else if (!_enemies.IsAlive())
            {
                state = GameState.Win;
            }

            if (state == GameState.Win || state == GameState.Lost)
            {
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop();
                }
                timer++;

                if (keyPressed.Contains(Keys.Space) && _gameOver)
                {
                    state = GameState.Play;
                    GameStart();
                }
            }
            ReleaseKey(Keys.Space);

        }

        public bool IsRectColliding(SimpleObject r1, SimpleObject r2)
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

        private void CheckGameState(Graphics graphics)
        {
            string text;
            //Dessine l'état pausé du jeu(GameState.Pause, ...) au canvas
            if (state == GameState.Pause)
            {
                text = state.ToString().ToUpper();
                graphics.DrawString(
                    text,
                    font32px,
                    blackBrush,
                    new RectangleF(gameSize.Width / 2 - text.Length* 21 / 2, gameSize.Height / 2 - 50, text.Length * 21, m_SubHeaderTextFieldSize.Height));
            }

            if (state == GameState.Win)
            {
                text = "VICTOIRE!";
                graphics.DrawString(
                    text,
                    font32px,
                    blackBrush,
                    new RectangleF(gameSize.Width / 2 - text.Length * 21 / 2, gameSize.Height / 2 - 150, text.Length * 21, m_SubHeaderTextFieldSize.Height));
                if (timer > 120)
                {
                    TimeSpan ts = _stopwatch.Elapsed;
                    string elapsedTime = ts.ToString(@"m\ \m\i\n\ s\ \s\e\c");
                    text = "Jeu terminé en: " + elapsedTime;
                    graphics.DrawString(
                        text,
                        font24px,
                        blackBrush,
                        new RectangleF(gameSize.Width / 2 - text.Length * 16 / 2, gameSize.Height / 2 - 80, text.Length * 16, m_SubHeaderTextFieldSize.Height));

                    if (ts.TotalSeconds < 40 && playerShip.NbLives >= 48)
                    {
                        DrawStars(graphics, 3);
                    }
                    else if (ts.TotalSeconds < 50 && playerShip.NbLives >= 36)
                    {
                        DrawStars(graphics, 2);
                    }
                    else if (ts.TotalSeconds < 60 && playerShip.NbLives >= 24)
                    {
                        DrawStars(graphics, 1);
                    }
                    else
                    {
                        DrawStars(graphics, 0);
                    }
                }

                if (timer > 300)
                {
                    text = "Appuyez sur ESPACE pour rejouer.";
                    graphics.DrawString(
                        text,
                        font24px,
                        blackBrush,
                        new RectangleF(gameSize.Width / 2 - text.Length * 16 / 2, gameSize.Height / 2 + 50, text.Length * 16, m_SubHeaderTextFieldSize.Height));
                    _gameOver = true;
                }
            }

            if (state == GameState.Lost)
            {
                text = "DÉFAITE..";
                graphics.DrawString(
                    text,
                    font32px,
                    blackBrush,
                    new RectangleF(gameSize.Width / 2 - text.Length * 21 / 2, gameSize.Height / 2 - 150, text.Length * 21, m_SubHeaderTextFieldSize.Height));

                if (timer > 240)
                {
                    text = "Appuyez sur ESPACE pour rejouer.";
                    graphics.DrawString(
                        text,
                        font24px,
                        blackBrush,
                        new RectangleF(gameSize.Width / 2 - text.Length * 16 / 2, gameSize.Height / 2 - 60, text.Length * 16, m_SubHeaderTextFieldSize.Height));
                    _gameOver = true;
                }
            }
        }

        private void DrawStars(Graphics graphics, int nbStars)
        {
            List<PointF[]> stars = new List<PointF[]>();
            for (int i = 0; i < 3; i++)
            {
                stars.Add(Calculate5StarPoints(new PointF(280f + 70 * i, 320f), 25f, 12.5f));
            }

            SolidBrush FillBrush;
           
            for (int i = 0; i < stars.Count; i++)
            {
                if (i < nbStars)
                {
                    FillBrush = new SolidBrush(Color.Yellow);
                }
                else
                {
                    FillBrush = new SolidBrush(Color.LightGoldenrodYellow);
                }

                graphics.FillPolygon(FillBrush, stars[i]);
                graphics.DrawPolygon(new Pen(Color.Black, 2), stars[i]);
            }
        }

        private PointF[] Calculate5StarPoints(PointF Orig, float outerradius, float innerradius)
        {
            // Define some variables to avoid as much calculations as possible
            // conversions to radians
            double Ang36 = Math.PI / 5.0;   // 36Â° x PI/180
            double Ang72 = 2.0 * Ang36;     // 72Â° x PI/180
            // some sine and cosine values we need
            float Sin36 = (float)Math.Sin(Ang36);
            float Sin72 = (float)Math.Sin(Ang72);
            float Cos36 = (float)Math.Cos(Ang36);
            float Cos72 = (float)Math.Cos(Ang72);
            // Fill array with 10 origin points
            PointF[] pnts = { Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig };
            pnts[0].Y -= outerradius;  // top off the star, or on a clock this is 12:00 or 0:00 hours
            pnts[1].X += innerradius * Sin36; pnts[1].Y -= innerradius * Cos36; // 0:06 hours
            pnts[2].X += outerradius * Sin72; pnts[2].Y -= outerradius * Cos72; // 0:12 hours
            pnts[3].X += innerradius * Sin72; pnts[3].Y += innerradius * Cos72; // 0:18
            pnts[4].X += outerradius * Sin36; pnts[4].Y += outerradius * Cos36; // 0:24 
            // Phew! Glad I got that trig working.
            pnts[5].Y += innerradius;
            // I use the symmetry of the star figure here
            pnts[6].X += pnts[6].X - pnts[4].X; pnts[6].Y = pnts[4].Y;  // mirror point
            pnts[7].X += pnts[7].X - pnts[3].X; pnts[7].Y = pnts[3].Y;  // mirror point
            pnts[8].X += pnts[8].X - pnts[2].X; pnts[8].Y = pnts[2].Y;  // mirror point
            pnts[9].X += pnts[9].X - pnts[1].X; pnts[9].Y = pnts[1].Y;  // mirror point
            return pnts;
        }
        #endregion
    }
}
