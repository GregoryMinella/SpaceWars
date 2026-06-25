using System;
using System.Windows.Forms;
using System.Drawing;

namespace Projeto_Space_War_V2_
{
    public class NaveInimigo : Entidade
    {
        public Timer inimigoTimer = new Timer();
        int sentidoInimigo = 1;
        int tiroCont = 0;
        Entidade Alvo;
        PictureBox Fundo;
        int sent = 0;


        public NaveInimigo(PictureBox fundo, string imagemNave, Entidade alvo) : base(fundo)
        {
            Fundo = fundo;
            Alvo = alvo;
            Load(imagemNave);
            Left = 1350;
            Top = 420;
            Width = 120;
            Height = 40;
            Speed = 5;

            lblHP.Text = "HP Inimigo: 100";
            lblHP.Left = 1680;
            lblHP.Top = 1020;

            inimigoTimer.Start();
            inimigoTimer.Interval = 16;
            inimigoTimer.Tick += InimigoTimerTick;

            // Config da barra de vida
            barraVida.Parent = this;
            barraVida.Left = 40;
            barraVida.Top = 36;
            barraVida.BackColor = Color.OrangeRed;
            barraVida.ProgressColor = Color.DarkGreen;
            barraVida.Height = 4;
            barraVida.Width = 30;
            barraVida.Minimum = 0;
            barraVida.Maximum = 100;
            barraVida.Value = HP;
        }

        void InimigoTimerTick(object sender, EventArgs e)
        {
            Top += Speed * sentidoInimigo;
            if (Top >= Y_max - Height || Top <= 0)
            {
                sentidoInimigo = -sentidoInimigo;
                sent++;
                if (sent > 1) sent = 0;
                Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 1 – Eco-do-Vazio\EcoVazio" + sent + ".png");
            }

            tiroCont++;
            if (tiroCont == 30)
            {
                Tiro tiro = new Tiro(Fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\tiro(temporario)\tiro.png", -1, Alvo);
                tiro.Left = Left - tiro.Width;
                tiro.Top = Top + (Height / 2) - (tiro.Height / 2);
                tiroCont = 0;
            }
        }

        public override void Dano(int valorDano)
        {
            HP -= valorDano;
            if (HP <= 0)
            {
                inimigoTimer.Stop();
                Left = 3000;
                Dispose();
                HP = 0;
            }
            lblHP.Text = "HP Inimigo: " + HP;
            barraVida.Value = HP;
        }
    }
}