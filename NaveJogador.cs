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
        PictureBox Fundo;
        // criação botão ESC

        Panel fundoEsc = new Panel();
        Button Esc1 = new Button();
        Button Esc2 = new Button();
        Button Esc3 = new Button();

        PictureBox Morreu = new PictureBox();
        public string[] imagensJogador =
        {
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\naveJogador-",
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\naveJogador_agua",
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\naveJogador_terra",
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\naveJogador_vento",
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\naveJogador_fogo",
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\naveJogador_total",
        };
        
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
            Morreu.Parent = fundo;

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

            fundoEsc.Parent = fundo;
            fundoEsc.Width = fundo.Width;
            fundoEsc.Height = fundo.Height;
            fundoEsc.BringToFront();
            fundoEsc.BackColor = Color.FromArgb(120, 0, 0, 0);
            fundoEsc.Visible = false;
            fundoEsc.Enabled = false;

            Esc2.Parent = fundoEsc;
            Esc2.Width = 200;
            Esc2.Height = 100;
            Esc2.Top = 400;
            Esc2.Left = (fundo.Width/2) - 100;
            Esc2.Text = "Retomar";
            Esc2.Click += retomarbutton;

            Esc1.Parent = fundoEsc;
            Esc1.Width = 200;
            Esc1.Height = 100;
            Esc1.Top = 500;
            Esc1.Left = (fundo.Width / 2) - 100;
            Esc1.Text = "Sair";
            Esc1.Click += sairbutton;

            Esc3.Parent = fundoEsc;
            Esc3.Width = 200;
            Esc3.Height = 100;
            Esc3.Top = 600;
            Esc3.Left = (fundo.Width / 2) - 100;
            Esc3.Text = "Reiniciar";
            Esc3.Click += recomeçarbutton;


        }
        void sairbutton(object sender, EventArgs e)
        {
            Application.Exit();
        }
        void recomeçarbutton(object sender, EventArgs e)
        {
            Application.Restart();
        }
        void retomarbutton (object sender, EventArgs e)
        {
            MainForm.Instance.retomarTudo();
            fundoEsc.Visible = false;
            fundoEsc.Enabled = false;
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
                
                SendKeys.Send("Enter");
                Morreu.Width = 1920;
                Morreu.Height = 1080;
                Morreu.BackColor = Color.Black;
                Left = 3000;
                Dispose();
                HP = 0;
            }

            lblHP.Text = "HP Jogador: " + HP;
            barraVida.Value = HP;
        }

        public void MoverNave()
        {
            if (Input.KeyDown(Keys.Escape))
            {
                fundoEsc.Visible = true;
                fundoEsc.Enabled = true;
                MainForm.Instance.pausarTudo();
            }

                // Application.Exit();
            
            // Jogador pode mudar se estiver no cenário 0 ou se o inimigo da tela atual foi derrotado/não existir
            bool podeMudarDeCenario = (cenario == 0) || (MainForm.Instance.naveInimigo == null) || (MainForm.Instance.naveInimigo.EstaMorto());

            // Esquerda
            if (Input.KeyDown(Keys.A))
            {
                Left -= Speed;

                if (Left < 0)
                {
                    if (podeMudarDeCenario && bossKills < 4)
                    {
                        // Se o boss morreu antes de você sair, avisa o MainForm para salvar o progresso da tela
                        if (cenario != 0) MainForm.Instance.RegistrarInimigoDerrotado(cenario);

                        Left = X_max - Width - 30;

                        if (cenario == 0) cenario = 1;
                        else if (cenario == 3) cenario = 0;

                        Fundo.Load($@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo{cenario}.png");
                        MainForm.Instance.CriarInimigoPorCenario(cenario);
                    }
                    else
                    {
                        Left = 0;
                    }
                }
            }

            // Direita
            if (Input.KeyDown(Keys.D))
            {
                Left += Speed;
                if (Left >= X_max - Width)
                {
                    if (podeMudarDeCenario && bossKills < 4)
                    {
                        if (cenario != 0) MainForm.Instance.RegistrarInimigoDerrotado(cenario);

                        Left = 0;

                        if (cenario == 0) cenario = 3;
                        else if (cenario == 1) cenario = 0;

                        Fundo.Load($@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo{cenario}.png");
                        MainForm.Instance.CriarInimigoPorCenario(cenario);
                    }

                    else
                    {
                        Left = X_max - Width - 5;
                    }
                }
            }

            // Cima
            if (Input.KeyDown(Keys.W))
            {
                Top -= Speed;
                if (Top < 0)
                {
                    if (podeMudarDeCenario && bossKills < 4)
                    {
                        if (cenario != 0) MainForm.Instance.RegistrarInimigoDerrotado(cenario);

                        Top = Y_max - Height - 30;

                        if (cenario == 0) cenario = 2;
                        else if (cenario == 4) cenario = 0;

                        Fundo.Load($@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo{cenario}.png");
                        MainForm.Instance.CriarInimigoPorCenario(cenario);
                    }
                    else
                    {
                        Top = 0;
                    }
                }
            }

            // Baixo
            if (Input.KeyDown(Keys.S))
            {
                Top += Speed;
                if (Top >= Y_max - Height)
                {
                    if (podeMudarDeCenario && bossKills < 4)
                    {
                        if (cenario != 0) MainForm.Instance.RegistrarInimigoDerrotado(cenario);

                        Top = 0;

                        if (cenario == 0) cenario = 4;
                        else if (cenario == 2) cenario = 0;

                        Fundo.Load($@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo{cenario}.png");
                        MainForm.Instance.CriarInimigoPorCenario(cenario);
                    }
                    else
                    {
                        Top = Y_max - Height - 5;
                    }
                }
            }
        }
        
    }
}