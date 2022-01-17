using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

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
        public static Brush blackBrush = new SolidBrush(Color.Black);

        public static Size m_SubHeaderTextFieldSize = new Size(300, 100); //Taille du texte

        /// <summary>
        /// A shared simple font
        /// </summary>
        public static Font defaultFont = new Font("Comic Sans MS", 24, FontStyle.Bold, GraphicsUnit.Pixel);
        #endregion


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
            gameObjects.Clear(); //supprime tous les objets de jeu

            playerShip = new PlayerSpaceship(new Vector2(gameSize.Width / 2, gameSize.Height - 150), SpaceInvaders.Properties.Resources.ship3, 100, GameObject.Side.Ally); // Instancie le vaisseau du joueur
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
            for (int i = 0; i < 3; i++)
            {
                bunkers.Add(new Bunker(new Vector2(gameSize.Width / 3 * i + 80, gameSize.Height - 200), GameObject.Side.Neutral));
                AddNewGameObject(bunkers[i]);
            }
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
            foreach (GameObject gameObject in gameObjects)
                gameObject.Draw(this, graphics);

            //Dessine l'état pausé du jeu(GameState.Pause) au canvas
            if (state == GameState.Pause)
            {
                graphics.DrawString(
                    state.ToString(),
                    defaultFont,
                    blackBrush,
                    new RectangleF(gameSize.Width/2 - 50, gameSize.Height/2 - m_SubHeaderTextFieldSize.Height, m_SubHeaderTextFieldSize.Width, m_SubHeaderTextFieldSize.Height));
            }

            if (state == GameState.Win)
            {
                graphics.DrawString(
                    "you win",
                    defaultFont,
                    blackBrush,
                    new RectangleF(gameSize.Width / 2 - 50, gameSize.Height / 2 - m_SubHeaderTextFieldSize.Height, m_SubHeaderTextFieldSize.Width, m_SubHeaderTextFieldSize.Height));
            }

            if (state == GameState.Lost)
            {
                graphics.DrawString(
                    "you lose",
                    defaultFont,
                    blackBrush,
                    new RectangleF(gameSize.Width / 2 - 50, gameSize.Height / 2 - m_SubHeaderTextFieldSize.Height, m_SubHeaderTextFieldSize.Width, m_SubHeaderTextFieldSize.Height));
            }
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
                if (keyPressed.Contains(Keys.Space))
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
        #endregion
    }
}
