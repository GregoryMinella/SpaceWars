using System;
using System.Drawing;
using System.Windows.Forms;
using SpaceWars_Horizon_Events;

namespace Projeto_Space_War_V2_
{
    public class NaveJogador : Entidade
    {

        public int cenario = 0;
        public int bossKills = 0;
        NaveInimigo naveInimigo;
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

            // Esquerda
            if (Input.KeyDown(Keys.A))
            {
                Left -= Speed;
                Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\refazer\naveJogador-desativado.png");



                if (Left < 0)
                {
                    Left = X_max - Width - 30;
                     animaCont = 1;

                    if (cenario == 0)
                        cenario = 1;


                    else if (cenario == 3)
                        cenario = 0;
                    naveInimigo = new NaveInimigo(Fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 2 – Sentinela Sombria\SentinelaSombria0.png", this, 200, 4);
                    Fundo.Load($@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo{cenario}.png");
                }
            }

                // Direita
                if (Input.KeyDown(Keys.D))
                {
                    Left += Speed;
                    Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\refazer\naveJogador-ativado.png");

                    animaCont = 3;
                    if (Left >= X_max)
                    {
                        Left = 0;

                        if (cenario == 0)
                            cenario = 3;
                        else if (cenario == 1)
                            cenario = 0;
                        naveInimigo = new NaveInimigo(Fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 4 – Executor do Horizonte\ExecutorHorizonte0.png", this, 200, 4);
                        Fundo.Load($@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo{cenario}.png");
                    }
                }

                // Cima
                if (Input.KeyDown(Keys.W))
                {
                    Top -= Speed;
                    animaCont = 0;
                    if (Top < 0)
                    {
                        Top = Y_max - Height - 30;

                        if (cenario == 0)
                            cenario = 2;
                        else if (cenario == 4)
                            cenario = 0;

                        naveInimigo = new NaveInimigo(Fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 1 – Eco-do-Vazio\EcoVazio0.png", this, 200, 4);
                        Fundo.Load($@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo{cenario}.png");
                    }
                }

            // Baixo
            if (Input.KeyDown(Keys.S))
            {
                Top += Speed;
                animaCont = 2;
                if (Top >= Y_max)
                {
                    Top = 0;

                    if (cenario == 0)
                        cenario = 4;
                    else if (cenario == 2)
                        cenario = 0;
                    naveInimigo = new NaveInimigo(Fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 3 – Caçador Gravitacional\CaçadorGravitacional0.png", this, 200, 4);
                    Fundo.Load($@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo{cenario}.png");
                }
            }
        }
    }
}