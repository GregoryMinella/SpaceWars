using System;
using System.Windows.Forms;

namespace Projeto_Space_War_V2_
{
    public class Tiro : Entidade
    {
        Random rnd = new Random();
        public Timer tiroTimer = new Timer();
        Entidade Alvo;

        public Tiro(PictureBox fundo, string imagem, int direcaoX, Entidade alvo) : base(fundo)
        {
            Load(imagem);
            Width = 40;
            Height = 30;
            Speed = 18; Alvo = alvo;

            DirecaoX = direcaoX;

            tiroTimer.Start();
            tiroTimer.Interval = 16;
            tiroTimer.Tick += TiroTimerTick;
        }

        void TiroTimerTick(object sender, EventArgs e)
        {
            Left += Speed * DirecaoX;

            if (Left > X_max || Left < 0)
            {
                Left = 5000;
                Dispose();
            }

            if (Bounds.IntersectsWith(Alvo.Bounds))
            {
                Left = 5000;
                Dispose();

                Alvo.Dano(rnd.Next(10, 21));
            }
        }
    }
}