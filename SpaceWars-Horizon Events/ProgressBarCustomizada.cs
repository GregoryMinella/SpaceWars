using System;
using System.Windows.Forms;
using System.Drawing;

namespace Projeto_Space_War_V2_
{
    public class ProgressBarCustomizada : ProgressBar
    {
        // 1. Definição da nova propriedade de cor
        private Color progressColor = Color.LimeGreen;

        public Color ProgressColor
        {
            get { return progressColor; }
            set
            {
                progressColor = value;
                this.Invalidate(); // Força o controle a redesenhar com a nova cor
            }
        }

        // Construir para forçar o estilo "UserPaint" e "Continuous"
        public ProgressBarCustomizada()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
            this.Style = ProgressBarStyle.Continuous;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // 2. Garante que o progresso seja desenhado apenas se for o estilo Continuous
            if (this.Style != ProgressBarStyle.Continuous)
            {
                // Se o estilo não for Continuous (ex: Blocks ou Marque),
                // permite que o controle padrão do Windows faça a renderização.
                base.OnPaint(e);
                return;
            }

            // --- Lógica de Desenho Personalizado ---

            // Retângulo de toda a área do controle
            // Rectangle rect = e.ClippedRectangle;
            Rectangle rect = this.ClientRectangle;

            // 3. Calcula a largura do progresso
            int progressWidth = (int)(((double)this.Value / this.Maximum) * rect.Width);

            // 4. Cria o pincel usando a nova propriedade ProgressColor
            using (SolidBrush brush = new SolidBrush(this.ProgressColor))
            {
                // Preenche o fundo (opcional, pode ser feito com BackColor)
                using (SolidBrush backgroundBrush = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillRectangle(backgroundBrush, 0, 0, rect.Width, rect.Height);
                }

                // Desenha a barra de progresso preenchida
                e.Graphics.FillRectangle(brush, 0, 0, progressWidth, rect.Height);
            }
        }
    }
}