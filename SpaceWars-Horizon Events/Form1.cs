using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Projeto_Space_War_V2_;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SpaceWars_Horizon_Events
{
    public partial class MainForm : Form
        {
            public static MainForm Instance { get; private set; } // criando a instancia do metodo (playCine)

            Timer gameTimer = new Timer(); 
            PictureBox fundo = new PictureBox();
            NaveJogador naveJogador;
            NaveInimigo naveInimigo;
        bool jogoPausado = false;
            bool cutscenePlaying = false;
            Process videoProcess = null;
            Timer cutsceneTimer;
            bool xpEntregue = false;

        Keys _teclaAnterior = Keys.None;

        public MainForm()
        {
            InitializeComponent();

            Instance = this; // registra a instancia (playCine)

            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Black;
            KeyPreview = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            DefineTamanhoForm();
            fundo.Parent = this;
            fundo.Width = Width;
            fundo.Height = Height;
            fundo.Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo0.png");
            fundo.SizeMode = PictureBoxSizeMode.StretchImage;
            fundo.BackColor = Color.Transparent;

            gameTimer.Interval = 16;
            gameTimer.Enabled = false; // Não inicia até a cutscene terminar
            gameTimer.Tick += gameTimerTick;

            naveJogador = new NaveJogador(fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\refazer\naveJogador-desativado.png");
            naveInimigo = new NaveInimigo(fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 1 – Eco-do-Vazio\EcoVazio0.png", naveJogador);
            xpEntregue = false; // IMPORTANTE! precisa disso para funcionar a função de upar o lvl, toda vez que vier um inimigo novo (lembrem por favor)
            // Dispara a cutscene de introdução automaticamente ao iniciar
            // Parâmetro: nome do arquivo e duração em segundos
            PlayCutscene("introducao.mp4", 11); // 11 segundos de duração <---- Guys esse cara é o foda, é so chamar ele em outra classe se precisar, ex:  MainForm.Instance.PlayCutscene("boss_jao.mp4", 20);
           
        }
        void DefineTamanhoForm()
        {
            Rectangle resolucao = Screen.PrimaryScreen.Bounds;
            this.Size = new Size(resolucao.Width, resolucao.Height);
            this.Location = new Point(0, 0);
        }

        void gameTimerTick(object sender, EventArgs e)
        {

            naveJogador.MoverNave();

            if (Input.KeyDown(Keys.Space) && _teclaAnterior != Keys.Space)
            {
                Tiro tiro = new Tiro(fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\tiro\tiro.png", naveJogador.DirecaoX, naveInimigo);
                tiro.Left = naveJogador.Left + naveJogador.Width - 5;
                tiro.Top = naveJogador.Top + (int)naveJogador.Height / 3;

                _teclaAnterior = Keys.Space;
            }

            uparLvl(); // roda a função a cada gameTimerTick 
        }

        void uparLvl()
        {
            if (naveInimigo.EstaMorto() && xpEntregue == false) //verifica se pode entregar o xp toda vez que o bixo toma dano
            {
                xpEntregue = true;
                naveJogador.bossKills++;
                naveJogador.Speed += 2 + (naveJogador.Speed / 3);
                naveJogador.HP += 10 + (naveJogador.HP / 4);
                //dano precisa ser tipo.tiro novo
                MessageBox.Show($"HP atual:{naveJogador.HP} bosses já derrotados:{naveJogador.bossKills} speed atual:{naveJogador.Speed}");
            }
            
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // comportamento original do jogo
            Input.KeyPressed(e.KeyCode);

            // permitir mudar de tiro só se ja derrotou certo tipo de boss
            Input.KeyPressed(e.KeyCode);

            if (e.KeyCode == Keys.Enter)
            {
                if (!jogoPausado) // Se NÃO (!) estiver pausado...
                {
                    pausarTudo(); // Pausa!
                }
                else // Se já estiver pausado...
                {
                    retomarTudo(); // Despausa!
                }
                return; // <--- Muito importante!
            }

            switch (e.KeyCode)
            {
                case Keys.D1:
                    naveJogador.tipoTiro = 1;
                    break;
                case Keys.D2:
                    naveJogador.tipoTiro = 2;
                    break;
                case Keys.D3:
                    naveJogador.tipoTiro = 3;
                    break;
                case Keys.D4:
                    naveJogador.tipoTiro = 4;
                    break;
                case Keys.D0:
                    naveJogador.tipoTiro = 0;
                    break;
            }
        }

        void pausarTudo()
        {
            jogoPausado = true; // Avisa o jogo que mudamos o estado
            gameTimer.Stop();   // Para o timer principal (sua nave, tiros, etc.)

            // O "?." checa se o inimigo existe antes de tentar parar o timer dele (evita crash)
            if (naveInimigo?.inimigoTimer != null) naveInimigo.inimigoTimer.Stop();
        }
        void retomarTudo()
        {
            jogoPausado = false; // Avisa o jogo que voltamos
            gameTimer.Start();   // Reativa o timer principal

            // CORRIGIDO: Agora dá .Start() no inimigo para ele voltar a se mexer!
            if (naveInimigo?.inimigoTimer != null) naveInimigo.inimigoTimer.Start();
        }

        //falta ainda colocar uma função para mudar o que o tiro irá fazer quando tipo.tiro for igual a algo

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Input.KeyReleased(e.KeyCode);
            _teclaAnterior = Keys.None;
        }

        public void PlayCutscene(string videoFileName, int durationSeconds)
        {
            // Se já há uma cutscene rodando, não permite iniciar outra
            if (cutscenePlaying) return;

            InitializeCutscenePlayer(videoFileName, durationSeconds);
        }

        void InitializeCutscenePlayer(string videoFileName, int durationSeconds)
        {
            // Caminho do vídeo - usando diferentes formas de procurar
            string videoRelative = Path.Combine(@"Assets\GDD_Immeasurable Chasm Event Horizon\Video", videoFileName);
            string videoPath = Path.Combine(Application.StartupPath, videoRelative);

            // Se não encontrar, tenta no diretório do projeto
            if (!File.Exists(videoPath))
            {
                videoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, videoRelative);
            }

            // Se ainda não encontrar, tenta procurar em diretórios pai
            if (!File.Exists(videoPath))
            {
                DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                while (dir != null && !File.Exists(videoPath))
                {
                    videoPath = Path.Combine(dir.FullName, videoRelative);
                    if (!File.Exists(videoPath))
                    {
                        dir = dir.Parent;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            // Debug: mostrar se o arquivo existe
            if (!File.Exists(videoPath))
            {
                MessageBox.Show($"Vídeo '{videoFileName}' não encontrado em:\n{videoPath}", 
                    "Erro - Vídeo não localizado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                StopCutscene();
                return;
            }

            try
            {
                cutscenePlaying = true;
               
                gameTimer.Enabled = false; // Pausa o jogo durante cutscene
                naveInimigo.inimigoTimer.Stop();// Pausa o inimigo durante cutscene
                // Obter caminho completo do wmplayer
                string wmplayerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), 
                    @"Windows Media Player\wmplayer.exe");

                // Se não encontrar, tentar localização alternativa
                if (!File.Exists(wmplayerPath))
                {
                    wmplayerPath = "wmplayer.exe"; // Tenta usar PATH do sistema
                }

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = wmplayerPath;
                psi.Arguments = $"\"{videoPath}\" /play /fullscreen";
                psi.UseShellExecute = true;
                psi.CreateNoWindow = false;

                videoProcess = Process.Start(psi);
                videoProcess.EnableRaisingEvents = true;

                // Timer que vai disparar taskkill AUTOMATICAMENTE após durationSeconds
                cutsceneTimer = new Timer();
                cutsceneTimer.Interval = durationSeconds * 1000; // Converte segundos para milissegundos
                cutsceneTimer.Tick += (s, e) =>
                {
                    cutsceneTimer.Stop();
                    StopCutscene();
                };
                cutsceneTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir vídeo '{videoFileName}':\n{ex.Message}", 
                    "Erro ao executar wmplayer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StopCutscene();
            }
        }

        void StopCutscene()
        {
            if (!cutscenePlaying) return;

            cutscenePlaying = false;

            // Parar o timer
            try
            {
                if (cutsceneTimer != null)
                {
                    cutsceneTimer.Stop();
                    cutsceneTimer.Dispose();
                    cutsceneTimer = null;
                }
            }
            catch { }

            // Usar taskkill para GARANTIR que wmplayer fecha
            try
            {
                ProcessStartInfo killPsi = new ProcessStartInfo();
                killPsi.FileName = "cmd.exe";
                killPsi.Arguments = "/c taskkill /IM wmplayer.exe /F /T"; // /T = mata processo e filhos
                killPsi.UseShellExecute = false;
                killPsi.CreateNoWindow = true;
                killPsi.RedirectStandardOutput = true;
                Process killProcess = Process.Start(killPsi);
                killProcess.WaitForExit(2000);
                killProcess.Dispose();
            }
            catch { }

            // Limpar a referência do processo anterior
            try
            {
                if (videoProcess != null)
                {
                    videoProcess.Dispose();
                    videoProcess = null;
                }
            }
            catch { }

            // Aguarda um pouco e inicia o jogo
            System.Threading.Thread.Sleep(200);
            gameTimer.Enabled = true;
            naveInimigo.inimigoTimer.Start();
        }

        void MainFormLoad(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }

    
    }
        
    public static class Input
    {

        static HashSet<Keys> keysDown = new HashSet<Keys>();

        public static bool KeyDown(Keys key)
        {
            return keysDown.Contains(key);
        }

        public static void KeyPressed(Keys key)
        {
            keysDown.Add(key);
        }

        public static void KeyReleased(Keys key)
        {
            keysDown.Remove(key);
        }
    }
}