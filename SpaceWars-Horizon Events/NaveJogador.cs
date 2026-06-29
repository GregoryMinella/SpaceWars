using System;
using System.Drawing;
using System.Windows.Forms;
using SpaceWars_Horizon_Events;

namespace Projeto_Space_War_V2_
{
    public class NaveJogador : Entidade
    {

        public int cenario = 0;
        public int tipoTiro = 0;
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

        public override void Dano(int valorDano)
        {
            HP -= valorDano;
            if (HP <= 0)
            {
                Left = 3000;
                Dispose();
                HP = 0;
            }

            if (tipoTiro == 1)
            {

            }
            lblHP.Text = "HP Jogador: " + HP;
            if (HP > barraVida.Value) // verifica se o ja subiu de level (+HP) e então, pode aumentar o maximum antes de crashar
            {
                barraVida.Maximum = HP;
            }
            barraVida.Value = HP;
        }

        public void MoverNave()
        {
            if (Input.KeyDown(Keys.Escape)) Application.Exit();

            if (Input.KeyDown(Keys.A))
            {
                Left -= Speed;

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

            if (Input.KeyDown(Keys.A)) Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\refazer\naveJogador-desativado.png");
            if (Input.KeyDown(Keys.D)) Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\refazer\naveJogador-ativado.png");
        }
    }
}