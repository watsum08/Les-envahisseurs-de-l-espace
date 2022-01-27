using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace SpaceInvaders
{
    public partial class GameForm : Form
    {

        #region fields
        /// <summary>
        /// L'instance de jeu
        /// </summary>
        private Game game;

        #region time management
        /// <summary>
        /// Chronomètre de jeu
        /// </summary>
        Stopwatch watch = new Stopwatch();

        /// <summary>
        /// Temps de dernier Update()
        /// </summary>
        long lastTime = 0;
        #endregion
           
        #endregion

        #region constructor
        /// <summary>
        /// Créer form et créer jeu
        /// </summary>
        public GameForm()
        {
            InitializeComponent();
            game = Game.CreateGame(this.ClientSize);
            watch.Start();
            WorldClock.Start();

        }
        #endregion

        #region events
        /// <summary>
        /// Peindre l'événement de forme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            BufferedGraphics bg = BufferedGraphicsManager.Current.Allocate(e.Graphics, e.ClipRectangle);
            Graphics g = bg.Graphics;
            g.Clear(Color.White);

            game.Draw(g);

            bg.Render();
            bg.Dispose();

        }

        /// <summary>
        /// A chaque "tick" => update jeu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorldClock_Tick(object sender, EventArgs e)
        {
            // 5ms update pour éviter des effets quantique
            int maxDelta = 5;

            // prends le temps avec précision au millisecondes
            long nt = watch.ElapsedMilliseconds;
            // calcule le temps passé de la dernière mise à jour
            double deltaT = (nt - lastTime);

            for (; deltaT >= maxDelta; deltaT -= maxDelta)
                game.Update(maxDelta / 1000.0);

            game.Update(deltaT / 1000.0);

            // remember the time of this update
            // se rappelle du dernier temps de mise à jour
            lastTime = nt;

            Invalidate();

        }

        /// <summary>
        /// événement Key down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            game.keyPressed.Add(e.KeyCode);
        }

        /// <summary>
        /// événement Key up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GameForm_KeyUp(object sender, KeyEventArgs e)
        {
            game.keyPressed.Remove(e.KeyCode);
        }

        #endregion

        private void GameForm_Load(object sender, EventArgs e)
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }
    }
}
