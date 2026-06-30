using System;
using System.Windows.Forms;

namespace Projeto_Space_War_V2_
{
    public class Tiro : Entidade
    {
        Random rnd = new Random();
        public Timer tiroTimer = new Timer();
        Entidade Alvo;
        private int elementoDoTiro; // <--- Guarda o elemento de quem disparou

        // Atualizamos o construtor para receber "int tipoTiroOrigem" no final
        public Tiro(PictureBox fundo, string imagem, int direcaoX, Entidade alvo, int tipoTiroOrigem) : base(fundo)
        {
            Load(imagem);
            Width = 50;
            Height = 15;
            Speed = 18;
            Alvo = alvo;
            DirecaoX = direcaoX;

            elementoDoTiro = tipoTiroOrigem; // <--- Registra o elemento do jogador neste tiro

            tiroTimer.Start();
            tiroTimer.Interval = 16;
            tiroTimer.Tick += TiroTimerTick;
        }

        void TiroTimerTick(object sender, EventArgs e)
        {
            Left += Speed * DirecaoX;

            if (Left > X_max || Left < 0)
            {
                tiroTimer.Stop();
                Dispose();
                return;
            }

            if (Alvo != null && !Alvo.IsDisposed && Bounds.IntersectsWith(Alvo.Bounds))
            {
                tiroTimer.Stop(); // Para o timer para evitar múltiplos ticks na colisão
                Dispose();

                // CORRIGIDO: Passa o elemento real do tiro, não o do Alvo!
                Alvo.Dano(rnd.Next(11, 27), elementoDoTiro);
            }
        }
    }
}