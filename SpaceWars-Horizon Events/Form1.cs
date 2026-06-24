using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Projeto_Space_War_V2_;


// vitor esta dificill nada vai no vini

namespace SpaceWars_Horizon_Events
{
    public partial class MainForm : Form
    {
        Timer gameTimer = new Timer(); 
        PictureBox fundo = new PictureBox();
        NaveJogador naveJogador;
        NaveInimigo naveInimigo; 

        Keys _teclaAnterior = Keys.None;

        public MainForm()
        {
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            KeyPreview = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DefineTamanhoForm();
            fundo.Parent = this;
            fundo.Width = Width;
            fundo.Height = Height;
            fundo.Load("fundo.jpg");
            fundo.SizeMode = PictureBoxSizeMode.StretchImage;
            fundo.BackColor = Color.Transparent;

            gameTimer.Interval = 16;
            gameTimer.Enabled = true;
            gameTimer.Tick += gameTimerTick;

            naveJogador = new NaveJogador(fundo, "nave.png");
            naveInimigo = new NaveInimigo(fundo, "naveInimigo.png", naveJogador);
        }
        // dijdaisjdsa
        void DefineTamanhoForm()
        {
            Rectangle resolucao = Screen.PrimaryScreen.Bounds;
            this.Size = new Size(resolucao.Width, resolucao.Height);
            this.Location = new Point(0, 0);
        }

        void gameTimerTick(object sender, EventArgs e)
        {

            naveJogador.MoverNave();

            if (Input.KeyDown(Keys.Space) && _teclaAnterior != Keys.Space)
            {
                Tiro tiro = new Tiro(fundo, "tiro.png", naveJogador.DirecaoX, naveInimigo);
                tiro.Left = naveJogador.Left + naveJogador.Width - 5;
                tiro.Top = naveJogador.Top + (int)naveJogador.Height / 3;

                _teclaAnterior = Keys.Space;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Input.KeyPressed(e.KeyCode);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Input.KeyReleased(e.KeyCode);
            _teclaAnterior = Keys.None;
        }

        void MainFormLoad(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }


    }

    public static class Input
    {

        static HashSet<Keys> keysDown = new HashSet<Keys>();

        public static bool KeyDown(Keys key)
        {
            return keysDown.Contains(key);
        }

        public static void KeyPressed(Keys key)
        {
            keysDown.Add(key);
        }

        public static void KeyReleased(Keys key)
        {
            keysDown.Remove(key);
        }
    }
}