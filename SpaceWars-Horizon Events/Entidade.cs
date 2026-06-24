using System;
using System.Drawing;
using System.Windows.Forms;
using Projeto_Space_War_V2_;

namespace Projeto_Space_War_V2_  // <-- REMOVA O PONTO FINAL
{
    public class Entidade : PictureBox
    {
        public int HP = 100;
        public int Speed = 5;
        public int X_max, Y_max;
        public int DirecaoX = 1;

        public Label lblHP = new Label();

        public ProgressBarCustomizada barraVida = new ProgressBarCustomizada();


        public Entidade(PictureBox fundo) // construtor
        {
            Parent = fundo;
            X_max = fundo.Width;
            Y_max = fundo.Height;

            SizeMode = PictureBoxSizeMode.StretchImage;
            BackColor = Color.Transparent;

            //config do labelHP
            lblHP.Parent = fundo;
            lblHP.Font = new Font("Arial", 14f);
            lblHP.ForeColor = Color.Aqua;
            lblHP.AutoSize = true;
        }

        public virtual void Dano(int valorDano)
        {

        }
    }
}