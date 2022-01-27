using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace SpaceInvaders
{
    /// <summary>
    /// Ceci est l'énumération pour l'état de jeu (Play, Pause, Win, Lost)
    /// </summary>
    enum GameState
    {
        Play,
        Pause,
        Win,
        Lost
    }
    /// <summary>
    /// Cette classe représente le jeu entier, elle implémente schéma singleton
    /// </summary>
    class Game
    {

        #region GameObjects management
        /// <summary>
        /// Collection sans indexe de tous les objets de jeu actuellement dans le jeu
        /// </summary>
        public HashSet<GameObject> gameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Collection sans indexe de tous les objets de jeu en attente, à rajouter au jeu 
        /// </summary>
        private HashSet<GameObject> pendingNewGameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Planifie un nouvel objet de jeu à rajouter dans le jeu
        /// Le nouvel objet sera ajouté lors du début de la prochaine itération de la boucle Update()
        /// </summary>
        /// <param name="gameObject">objet à rajouter</param>
        public void AddNewGameObject(GameObject gameObject)
        {
            pendingNewGameObjects.Add(gameObject);
        }
        #endregion

        #region game technical elements
        /// <summary>
        /// La taille du jeu
        /// </summary>
        public Size gameSize;

        /// <summary>
        /// L'état de jeu(pause, en jeu, etc..)
        /// </summary>

        public GameState state = GameState.Play;

        /// <summary>
        /// L'état du clavier
        /// </summary>
        public HashSet<Keys> keyPressed = new HashSet<Keys>();

        #endregion

        #region static fields (helpers)

        /// <summary>
        /// Singleton pour accès facile (qu'une seule instance de Game => Singleton)
        /// </summary>
        public static Game game { get; private set; }

        /// <summary>
        /// Pinceau noir partagé
        /// </summary>
        public static Brush blackBrush = new SolidBrush(Color.FromArgb(250, 240, 240, 240));

        public static Size m_SubHeaderTextFieldSize = new Size(300, 100); //Taille du texte

        /// <summary>
        /// Police simple partagé
        /// </summary>
        public static Font defaultFont = new Font("Ubuntu Mono", 16);
        #endregion

        /// <summary>
        /// Police privé 24px Ubuntu Mono
        /// </summary>
        private Font font24px = new Font("Ubuntu Mono", 24);
        /// <summary>
        /// Police privé 32px Ubuntu Mono
        /// </summary>
        private Font font32px = new Font("Ubuntu Mono", 32);


        #region constructors
        /// <summary>
        /// Constructeur Singleton
        /// </summary>
        /// <param name="gameSize">Taille de jeu</param>
        /// 
        /// <returns></returns>
        public static Game CreateGame(Size gameSize)
        {
            if (game == null)
                game = new Game(gameSize);
            return game;
        }

        /// <summary>
        /// L'instance de vaisseau joueur
        /// </summary>
        public PlayerSpaceship playerShip;
        /// <summary>
        /// L'instance du bloc d'ennemi
        /// </summary>
        private EnemyBlock _enemies;
        /// <summary>
        /// L'instance du chronomètre de jeu
        /// </summary>
        private Stopwatch _stopwatch;
        /// <summary>
        /// L'instance du minuteur d'animation (texte)
        /// </summary>
        private int timer;
        /// <summary>
        /// Bool de la fin de jeu, si Vrai = jeu terminé sinon Faux = jeu en cours
        /// </summary>
        private bool _gameOver;
        /// <summary>
        /// Constructeur privé
        /// </summary>
        /// <param name="gameSize">Taille de jeu</param>
        private Game(Size gameSize)
        {
            this.gameSize = gameSize;

            GameStart();
        }

        private void GameStart()
        {
            timer = 0;
            _gameOver = false;
            gameObjects.Clear(); // supprime tous les objets de jeu pour quand on recommence le jeu

            playerShip = new PlayerSpaceship(new Vector2(gameSize.Width / 2 - SpaceInvaders.Properties.Resources.ship3.Width / 2, gameSize.Height - 120), SpaceInvaders.Properties.Resources.ship3, 120, GameObject.Side.Ally); // Instancie le vaisseau du joueur
            AddNewGameObject(playerShip); // Ajoute le vaisseau du joueur dans la liste des Game Objects

            Random r = new Random(); // crée un chiffre (seed) aléatoire
            double randDirection = r.NextDouble(); // prend le double suivant du seed
            Vector2 enemyBlockSpawnPos = new Vector2((gameSize.Width - 400) / 2, 50); //position du bloc d'ennemis
            int enemyBlockWidth = 400; // largeur du bloc d'ennemis
            if (randDirection > 0.5) // chance de 50% d'aller à gauche et à droite
            {
                _enemies = new EnemyBlock(enemyBlockSpawnPos, enemyBlockWidth, 1); // le bloc d'ennemis va à droite
            }
            else
            {
                _enemies = new EnemyBlock(enemyBlockSpawnPos, enemyBlockWidth, -1); // le bloc d'ennemis va à gauche
            }
            _enemies.AddLine(7, 24, SpaceInvaders.Properties.Resources.ship7); // ajoute la première ligne d'ennemis durs
            _enemies.AddLine(5, 20, SpaceInvaders.Properties.Resources.ship5); // rajoute une ligne d'ennemis moyens
            _enemies.AddLine(9, 10, SpaceInvaders.Properties.Resources.ship6); // rajoute une ligne d'ennemis faible
            AddNewGameObject(_enemies); // ajoute le bloc d'ennemis aux objets de jeu

            List<Bunker> bunkers = new List<Bunker>(); // liste de bunkers
            for (int i = 1; i < 4; i++)
            {
                bunkers.Add(new Bunker(new Vector2(gameSize.Width / 4 * i - SpaceInvaders.Properties.Resources.bunker.Width / 2, gameSize.Height - 240), GameObject.Side.Neutral)); // ajoute des instances de bunkers dans la liste de manière symétrique
                AddNewGameObject(bunkers[i - 1]); // ajoute chaque bunker aux objets de jeu
            }

            _stopwatch = new Stopwatch(); // instancie le chronomètre pour le temps de jeu
            _stopwatch.Start(); // commence le chronomètre
        }

        #endregion

        #region methods

        /// <summary>
        /// Force une touche à être ignoré lors des prochains Updates jusqu'as
        /// l'utilisateur le retouche de manière explicite.
        /// </summary>
        /// <param name="key">key to ignore</param>
        public void ReleaseKey(Keys key)
        {
            keyPressed.Remove(key);
        }

        /// <summary>
        /// Dessine le jeu entier
        /// </summary>
        /// <param name="g">Interface graphique à desinner dessus</param>
        public void Draw(Graphics graphics)
        {
            // dessine l'arrière plan
            graphics.DrawImage(SpaceInvaders.Properties.Resources.background, 0, 0, 706, 652);

            // dessine chaque objet de jeu
            foreach (GameObject gameObject in gameObjects)
                gameObject.Draw(this, graphics);

            CheckGameState(graphics);
        }

        /// <summary>
        /// Mets à jour le jeu
        /// </summary>
        /// <param name="deltaT">temps en ms depuis dernier Update</param>
        public void Update(double deltaT)
        {
            // ajoute les objets de jeu en attente
            gameObjects.UnionWith(pendingNewGameObjects);
            pendingNewGameObjects.Clear(); // efface les objets de jeu en attente


            // mets à jour chaque objet de jeu
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.Update(this, deltaT);
            }

            // si l'objet de jeu n'est plus vivant, le supprimer de la liste
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

            /* // Permet de tuer tous les ennemis d'une touche (mode développeur!!)
            if (keyPressed.Contains(Keys.K) && state == GameState.Play)
            {
                _enemies.KillAllShips();
            }
            ReleaseKey(Keys.K);*/

            // si le vaisseau joueur est mort, l'état de jeu = Perdu
            if (!playerShip.IsAlive())
            {
                state = GameState.Lost;
            }
            else if (!_enemies.IsAlive()) // si le bloc d'ennemis est mort, l'état de jeu = Gagné
            {
                state = GameState.Win;
            }

            // si l'état de jeu est Gagné ou Perdu
            if (state == GameState.Win || state == GameState.Lost)
            {
                 // vérifie si le chronomètre a bien été lancé pour éviter des erreurs
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Stop(); // arrête le chronomètre
                }
                timer++; // incrémente la minuterie d'animations

                // si le jeu est terminé et que l'on touche sur espace
                if (_gameOver && keyPressed.Contains(Keys.Space))
                {
                    state = GameState.Play;
                    GameStart(); // recommence/recrée l'instance de jeu
                }
            }
            ReleaseKey(Keys.Space); // pour éviter d'avoir la touche appuyé plusieurs fois

        }

        /// <summary>
        /// Vérifie l'état du jeu et dessine l'état sur l'interface graphique
        /// </summary>
        /// <param name="graphics">l'interface graphique</param>
        private void CheckGameState(Graphics graphics)
        {
            string text;
            // dessine l'état pausé du jeu(GameState.Pause, ...) au canvas
            if (state == GameState.Pause)
            {
                text = state.ToString().ToUpper();
                graphics.DrawString(
                    text,
                    font32px,
                    blackBrush,
                    new RectangleF(gameSize.Width / 2 - text.Length * 21 / 2, gameSize.Height / 2 - 50, text.Length * 21, m_SubHeaderTextFieldSize.Height));
            }

            // dessine l'état gagné du jeu au canvas
            if (state == GameState.Win)
            {
                text = "VICTOIRE!";
                graphics.DrawString(
                    text,
                    font32px,
                    blackBrush,
                    new RectangleF(gameSize.Width / 2 - text.Length * 21 / 2, gameSize.Height / 2 - 150, text.Length * 21, m_SubHeaderTextFieldSize.Height));
                // après 2 secondes affiche le temps de jeu
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

                    // choix du nombres d'étoiles à afficher selon le temps de jeu et le nombre de vies du joueur
                    if (ts.TotalSeconds < 42 && playerShip.NbLives >= 96)
                    {
                        DrawStars(graphics, 3);
                    }
                    else if (ts.TotalSeconds < 50 && playerShip.NbLives >= 72)
                    {
                        DrawStars(graphics, 2);
                    }
                    else if (ts.TotalSeconds < 60 && playerShip.NbLives >= 48)
                    {
                        DrawStars(graphics, 1);
                    }
                    else
                    {
                        DrawStars(graphics, 0);
                    }
                }

                // après environ 5 secondes afficher instruction et met fin au jeu (_gameOver = true)
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

            // si l'état du jeu 
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

        /// <summary>
        /// Permet de dessiner les étoiles de score sur l'interface graphique
        /// <summary>
        /// <param name="graphics">l'interface graphique</param>
        /// <param name="nbStars">les nombres d'étoiles</param>
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

        /// <summary>
        /// Permet de créer les étoiles à 5 points
        /// <summary>
        /// <param name="Orig">point d'origine</param>
        /// <param name="outerradius">rayon extérieur</param>
        /// <param name="innerradius">rayon intérieur</param>
        private PointF[] Calculate5StarPoints(PointF Orig, float outerradius, float innerradius)
        {
            // défini les variables pour éviter trop de calculs
            // conversions à des radians
            double Ang36 = Math.PI / 5.0;   // 36° x PI/180
            double Ang72 = 2.0 * Ang36;     // 72° x PI/180
            // des valeurs Sin et Cos qu'on a besoin
            float Sin36 = (float)Math.Sin(Ang36);
            float Sin72 = (float)Math.Sin(Ang72);
            float Cos36 = (float)Math.Cos(Ang36);
            float Cos72 = (float)Math.Cos(Ang72);
            // crée et rempli le tableau pnts avec 10 points d'origines
            PointF[] pnts = { Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig, Orig };
            pnts[0].Y -= outerradius;  // le point haut de l'étoile, 12h00 sur l'horloge
            pnts[1].X += innerradius * Sin36; pnts[1].Y -= innerradius * Cos36; // 0h06
            pnts[2].X += outerradius * Sin72; pnts[2].Y -= outerradius * Cos72; // 0h12
            pnts[3].X += innerradius * Sin72; pnts[3].Y += innerradius * Cos72; // 0h18
            pnts[4].X += outerradius * Sin36; pnts[4].Y += outerradius * Cos36; // 0h24 
            pnts[5].Y += innerradius;
            // j'utilise la symétrie de l'étoile
            pnts[6].X += pnts[6].X - pnts[4].X; pnts[6].Y = pnts[4].Y;  // point en miroir
            pnts[7].X += pnts[7].X - pnts[3].X; pnts[7].Y = pnts[3].Y;  // point en miroir
            pnts[8].X += pnts[8].X - pnts[2].X; pnts[8].Y = pnts[2].Y;  // point en miroir
            pnts[9].X += pnts[9].X - pnts[1].X; pnts[9].Y = pnts[1].Y;  // point en miroir
            return pnts;
        }
        #endregion
    }
}
