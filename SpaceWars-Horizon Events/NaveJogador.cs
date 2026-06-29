using System;
using System.Drawing;
using System.Windows.Forms;
using SpaceWars_Horizon_Events;

namespace Projeto_Space_War_V2_
{
    public class NaveJogador : Entidade
    {

        public int cenario = 0;
        public int tipoElemental = 0;
        public int bossKills = 0;
        PictureBox Fundo;

        public NaveJogador(PictureBox fundo, string imagemNave) : base(fundo)
        {
            Fundo = fundo;

            Load(imagemNave);
            Left = 300;
            Top = 420;
            Width = 150;
            Height = 90;
            Speed = 4;

            lblHP.Left = 100;
            lblHP.Top = 100;
            lblHP.Text = "HP Jogador: 100";

            // Config da barra de vida
            barraVida.Parent = this;
            barraVida.Left = 80;
            barraVida.Top = 20;
            barraVida.BackColor = Color.OrangeRed;
            barraVida.ProgressColor = Color.DarkGreen;
            barraVida.Height = 4;
            barraVida.Width = 30;
            barraVida.Minimum = 0;
            barraVida.Maximum = 100;
            barraVida.Value = HP;
        }

        public override void Dano(int valorDano, int tipoTiroAlvo)
        {
            if (tipoElemental == 1 && tipoTiroAlvo == 4 ||
                tipoElemental == 2 && tipoTiroAlvo == 1 ||
                tipoElemental == 3 && tipoTiroAlvo == 2 ||
                tipoElemental == 4 && tipoTiroAlvo == 3)
            {
                HP -= valorDano * 2;
            }
            else
            {
                HP -= valorDano;
            }
            if (HP <= 0)
            {
                Left = 3000;
                Dispose();
                HP = 0;
            }

            lblHP.Text = "HP Jogador: " + HP;
            barraVida.Value = HP;
        }

        public void MoverNave()
        {
            if (Input.KeyDown(Keys.Escape)) Application.Exit();

            if (Input.KeyDown(Keys.A))
            {
                Left -= Speed;
                Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\refazer\naveJogador-desativado.png");

                //clamp
                if (Left < 0)
                {
                    Left = X_max - this.Width - 30;
                    cenario--;
                    if (cenario < 0) cenario = 2;
                    Fundo.Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo" + cenario + ".png"); 
                }
            }

            if (Input.KeyDown(Keys.D))
            {
                Left += Speed;
                Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\refazer\naveJogador-ativado.png");

                //clamp
                if (Left >= X_max)
                {
                    Left = 0;
                    cenario++;
                    if (cenario > 2) cenario = 0;
                    Fundo.Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo" + cenario + ".png");
                }
            }

            if (Input.KeyDown(Keys.W)) Top -= Speed;
            if (Input.KeyDown(Keys.S)) Top += Speed;

        }
    }
}