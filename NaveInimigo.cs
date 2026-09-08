using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Projeto_Space_War_V2_
{
    public class NaveInimigo : Entidade
    {
        public Timer inimigoTimer = new Timer();
        int sentidoInimigo = 1;
        public PictureBox conquista = new PictureBox();
        public Timer TickConquista = new Timer();
        int tiroCont = 0;
        Entidade Alvo;
        PictureBox Fundo;
        int sent = 0;

        public string[] imagensInimigo =
        {
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 1 – Eco-do-Vazio\EcoVazio",
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 2 – Sentinela Sombria\SentinelaSombria",
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 3 – Caçador Gravitacional\CaçadorGravitacional",
            @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 4 – Executor do Horizonte\ExecutorHorizonte"
        };

        public NaveInimigo(PictureBox fundo, string imagemNave, Entidade alvo, int hp, int tipoElemento) : base(fundo) 
        { 
           Fundo = fundo;
            Alvo = alvo;
            HP = hp;
            Load(imagemNave);
            Left = 1350;
            Top = 420;
            Width = 180;
            Height = 170;
            Speed = 5;
            tipoElemental = tipoElemento;

            lblHP.Text = "HP Inimigo: " + HP;
            lblHP.Left = 1680;
            lblHP.Top = 1020;

            inimigoTimer.Interval = 16;
            inimigoTimer.Tick += InimigoTimerTick;
            inimigoTimer.Start();

            conquista.Parent = fundo;
            conquista.SizeMode = PictureBoxSizeMode.StretchImage;
            conquista.Width = 370; 
            conquista.Height = 80;
            conquista.Left = 3100; // mais 3000
            conquista.Top = 20;
            conquista.BackColor = Color.Transparent;

            // Config da barra de vida
            barraVida.Parent = this;
            barraVida.Left = 40;
            barraVida.Top = 20;
            barraVida.BackColor = Color.OrangeRed;
            barraVida.ProgressColor = Color.DarkGreen;
            barraVida.Height = 4;
            barraVida.Width = 30;
            barraVida.Minimum = 0;
            barraVida.Maximum = hp;
            barraVida.Value = HP;


            TickConquista.Interval = 5000;
            TickConquista.Tick += tempoconquista_Tick;
        }

        void InimigoTimerTick(object sender, EventArgs e)
        {
            Top += Speed * sentidoInimigo;
            if (Top >= Y_max - Height || Top <= 0)
            {
                sentidoInimigo = -sentidoInimigo;
                sent++;
                if (sent > 1) sent = 0;

                int indexImagem = tipoElemental - 1;
                if (indexImagem >= 0 && indexImagem < imagensInimigo.Length)
                {
                    Load(imagensInimigo[indexImagem] + sent + ".png");
                }
            }

            tiroCont++;
            if (tiroCont == 30)
            {
                Tiro tiro = new Tiro(Fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\tiro\tiro"+ tipoElemental +".png", -1, Alvo, 4);
                
                tiro.Left = Left - tiro.Width;
                tiro.Top = Top + (Height / 2) - (tiro.Height / 2);
                tiroCont = 0;
            }
        }
        public void tempoconquista_Tick(object sender, EventArgs e)
        {
            TickConquista.Stop();
            
            conquista.Left += 3000;
        }
        public override void Dano(int valorDano, int tipoTiroAlvo)
        {
           
            if (tipoElemental == 1 && tipoTiroAlvo == 4)
                HP -= valorDano * 2;
            else if (tipoElemental == 2 && tipoTiroAlvo == 1)
                HP -= valorDano * 2;
            else if (tipoElemental == 3 && tipoTiroAlvo == 2)
                HP -= valorDano * 2;
            else if (tipoElemental == 4 && tipoTiroAlvo == 3)
                HP -= valorDano * 2;
            else
                HP -= valorDano;

            if (HP <= 0)
            {
                inimigoTimer.Stop();
                conquista.Left -= 3000;
                Left = 3000;

                // ALTERADO: Converte o Alvo para NaveJogador para conseguir pegar a propriedade .cenario correta
                if (Alvo is NaveJogador jogador)
                {
                    SpaceWars_Horizon_Events.MainForm.Instance.RegistrarInimigoDerrotado(jogador.cenario);
                    
                }

                
                conquista.Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\GIF\C_"+ tipoElemental +".gif");
                
                Dispose();
                HP = 0;
            }
            lblHP.Text = "HP Inimigo: " + HP;
            barraVida.Value = HP;
        }

        public bool EstaMorto()
        {
            return HP <= 0;
        }
    }
}