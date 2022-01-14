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
        Pause
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
        private static Brush blackBrush = new SolidBrush(Color.Black);

        /// <summary>
        /// A shared simple font
        /// </summary>
        private static Font defaultFont = new Font("Comic Sans MS", 24, FontStyle.Bold, GraphicsUnit.Pixel);
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

            playerShip = new PlayerSpaceship(new Vector2(gameSize.Width / 2, gameSize.Height - 150), SpaceInvaders.Properties.Resources.ship3, 3); // Instancie le vaisseau du joueur
            AddNewGameObject(playerShip); //Ajoute le vaisseau du joueur dans la liste des Game Objects

            _enemies = new EnemyBlock(new Vector2(200, 50), 500, -1);
            _enemies.AddLine(5, 3, SpaceInvaders.Properties.Resources.ship7);
            _enemies.AddLine(3, 2, SpaceInvaders.Properties.Resources.ship5);
            _enemies.AddLine(10, 1, SpaceInvaders.Properties.Resources.ship6);
            AddNewGameObject(_enemies);

            List<Bunker> bunkers = new List<Bunker>();
            for (int i = 0; i < 3; i++)
            {
                bunkers.Add(new Bunker(new Vector2(gameSize.Width/3*i+80, gameSize.Height - 200)));
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

        private Size m_SubHeaderTextFieldSize = new Size(300, 100); //Taille du texte

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

            graphics.DrawString(
                    "Vies : " + playerShip.NbLives,
                    defaultFont,
                    blackBrush,
                    new RectangleF(50, gameSize.Height - 100, m_SubHeaderTextFieldSize.Width, m_SubHeaderTextFieldSize.Height));
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
