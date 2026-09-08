using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Projeto_Space_War_V2_;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Microsoft.Win32;
using System.Media;

namespace SpaceWars_Horizon_Events
{
    public partial class MainForm : Form
    {
        public static MainForm Instance { get; private set; }

        Timer gameTimer = new Timer();
        PictureBox fundo = new PictureBox();
        NaveJogador naveJogador;
        SoundPlayer somfundo = new SoundPlayer();
        public PictureBox itemDropado;
        int cenario = 0;

        public NaveInimigo naveInimigo;
        Label lblUpgrade = new Label();
        bool jogoPausado = false;
        bool cutscenePlaying = false;
        Process videoProcess = null;
        Timer cutsceneTimer;
        bool upgradeAguardEnter;

        // Lista para salvar quais cenários já tiveram seus chefes derrotados
        private List<int> cenariosDerrotados = new List<int>();

        Keys _teclaAnterior = Keys.None;

        // ==========================================
        // SISTEMA DE INÉRCIA E FÍSICA INTEGRADO
        // ==========================================
        bool usarInercia = true;
        bool usarDeltaTime = true;
        Stopwatch cronometro = new Stopwatch();
        float deltaTime = 0f;

        float aceleracao = 900f;          // Taxa de aceleração (pixels/s²)
        float desaceleracaoAuto = 600f;   // Força da frenagem automática
        float deadzone = 50f;            // Limiar de parada e impulso inicial
        float velocidadeAtualX = 0f;
        float velocidadeAtualY = 0f;
        float velocidadeNave = 300f;      // Teto máximo de velocidade (pixels/s)

        public MainForm()
        {
            InitializeComponent();

            Instance = this;
            BackColor = Color.Black;
            KeyPreview = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.FormBorderStyle = FormBorderStyle.None;
            DefineTamanhoForm();

            fundo.Parent = this;
            fundo.Width = Width;
            fundo.Height = Height;
            fundo.Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo0.png");
            fundo.SizeMode = PictureBoxSizeMode.StretchImage;
            fundo.BackColor = Color.Transparent;

            gameTimer.Interval = 16;
            gameTimer.Enabled = false; // Mantido false para a introdução
            gameTimer.Tick += gameTimerTick;

            naveJogador = new NaveJogador(fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Jogador\refazer\naveJogador-desativado.png");

            naveInimigo = null;
            this.KeyPreview = true;

            // Inicia o cronômetro de precisão para a inércia
            cronometro.Start();

            PlayCutscene("introducao.mp4", 11);
            somfundo = new SoundPlayer(@"Assets\GDD_Immeasurable Chasm Event Horizon\Som\Fundo\SomFundo0.wav");
            somfundo.Play();
        }

        void DefineTamanhoForm()
        {
            Rectangle resolucao = Screen.PrimaryScreen.Bounds;
            this.Size = new Size(resolucao.Width, resolucao.Height);
            this.Location = new Point(0, 0);
        }

        public void CriarInimigoPorCenario(int cenarioAtual)
        {
            cenario = cenarioAtual - 1;
            if (itemDropado != null)
            {
                itemDropado.Dispose();
                itemDropado = null;
            }

            if (cenariosDerrotados.Contains(cenarioAtual))
            {
                if (naveInimigo != null)
                {
                    naveInimigo.inimigoTimer.Stop();
                    naveInimigo.Dispose();
                    naveInimigo = null;
                }
                return;
            }

            if (naveInimigo != null)
            {
                naveInimigo.inimigoTimer.Stop();
                naveInimigo.Dispose();
                naveInimigo = null;
            }

            switch (cenarioAtual)
            {
                case 1:
                    PlayCutscene("Eco.mp4", 8);
                    naveInimigo = new NaveInimigo(fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 1 – Eco-do-Vazio\EcoVazio0.png", naveJogador, 100, 1);
                    somfundo = new SoundPlayer(@"Assets\GDD_Immeasurable Chasm Event Horizon\Som\Fundo\SomFundo1.wav");
                    somfundo.Play();
                    break;
                case 2:
                    PlayCutscene("Sentinela.mp4", 8);
                    naveInimigo = new NaveInimigo(fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 2 – Sentinela Sombria\SentinelaSombria0.png", naveJogador, 150, 2);
                    somfundo = new SoundPlayer(@"Assets\GDD_Immeasurable Chasm Event Horizon\Som\Fundo\SomFundo2.wav");
                    somfundo.Play();
                    break;
                case 3:
                    naveInimigo = new NaveInimigo(fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 3 – Caçador Gravitacional\CaçadorGravitacional0.png", naveJogador, 200, 3);
                    somfundo = new SoundPlayer(@"Assets\GDD_Immeasurable Chasm Event Horizon\Som\Fundo\SomFundo3.wav");
                    somfundo.Play();
                    break;
                case 4:
                    PlayCutscene("Executor.mp4", 6);
                    naveInimigo = new NaveInimigo(fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Inimigo 4 – Executor do Horizonte\ExecutorHorizonte0.png", naveJogador, 250, 4);
                    somfundo = new SoundPlayer(@"Assets\GDD_Immeasurable Chasm Event Horizon\Som\Fundo\SomFundo4.wav");
                    somfundo.Play();
                    break;
                case 5:
                    naveInimigo = new NaveInimigo(fundo, @"Assets\GDD_Immeasurable Chasm Event Horizon\Personagens\Chefe Final – Manfredo", naveJogador, 300, 5);
                    somfundo = new SoundPlayer(@"Assets\GDD_Immeasurable Chasm Event Horizon\Som\Fundo\SomFundo5.wav");
                    somfundo.Play();
                    break;
                default:
                    somfundo = new SoundPlayer(@"Assets\GDD_Immeasurable Chasm Event Horizon\Som\Fundo\SomFundo0.wav");
                    somfundo.Play();
                    naveInimigo = null;
                    break;
            }
        }

        public void RegistrarInimigoDerrotado(int cenarioAtual)
        {
            if (!cenariosDerrotados.Contains(cenarioAtual))
            {
                cenariosDerrotados.Add(cenarioAtual);
                naveJogador.HP += naveJogador.barraVida.Maximum / 10;
                naveJogador.lblHP.Text = "HP Jogador: " + naveJogador.HP;
                naveJogador.HP = Math.Min(naveJogador.HP + (naveJogador.barraVida.Maximum / 10), naveJogador.barraVida.Maximum);

                naveJogador.barraVida.Value = naveJogador.HP;
                naveJogador.Speed += naveJogador.Speed / 3;
                velocidadeNave += velocidadeNave * 0.2f; // Incrementa também a velocidade máxima com inércia
                DropItem();
                naveInimigo.TickConquista.Start();
                naveJogador.bossKills++;
                if (naveJogador.bossKills >= 4)
                {
                    fundo.Load(@"Assets\GDD_Immeasurable Chasm Event Horizon\Fundo\fundo5.png");
                }
            }
        }

        public void DropItem()
        {
            if (naveInimigo.EstaMorto())
            {
                itemDropado = new PictureBox();
                itemDropado.Parent = fundo;
                itemDropado.Width = 180;
                itemDropado.Height = 180;
                itemDropado.SizeMode = PictureBoxSizeMode.StretchImage;
                itemDropado.BackColor = Color.Transparent;
                itemDropado.Top = 420;
                itemDropado.Left = 1350;

                itemDropado.Load(naveInimigo.imagensInimigo[cenario] + "2.png");
                itemDropado.BringToFront();
            }
        }

        void gameTimerTick(object sender, EventArgs e)
        {
            // 1. Atualização do Delta Time para suavizar a inércia
            if (usarDeltaTime)
            {
                deltaTime = (float)cronometro.Elapsed.TotalSeconds;
                cronometro.Restart();
            }
            else
            {
                deltaTime = 0.016f;
            }

            // 2. Movimento com inércia física aplicada
            AtualizarMovimentoJogador();

            // Verifica se o boss morreu
            if (naveInimigo != null && naveInimigo.EstaMorto())
            {
                RegistrarInimigoDerrotado(naveJogador.cenario);
            }

            // Coleta de item
            if (itemDropado != null && naveJogador.Bounds.IntersectsWith(itemDropado.Bounds))
            {
                SendKeys.Send("D" + naveInimigo.tipoElemental);
                itemDropado.Dispose();
                itemDropado = null;
            }

            // Disparo
            if (Input.KeyDown(Keys.Space) && _teclaAnterior != Keys.Space)
            {
                if (naveInimigo != null && !naveInimigo.EstaMorto())
                {
                    Tiro tiro = new Tiro(fundo, $@"Assets\GDD_Immeasurable Chasm Event Horizon\tiro\tiro" + naveJogador.tipoElemental + ".png",
                                         naveJogador.DirecaoX, naveInimigo, naveJogador.tipoElemental);

                    tiro.Left = naveJogador.Left + naveJogador.Width - 5;
                    tiro.Top = naveJogador.Top + (int)naveJogador.Height / 3;
                }
                _teclaAnterior = Keys.Space;
            }
        }

        // ==========================================
        // MOTOR FÍSICO: ACELERAÇÃO, FRENAGEM E CLAMP
        // ==========================================
        void AtualizarMovimentoJogador()
        {
            if (usarInercia)
            {
                float velocidadeInicial = deadzone + 1f;

                // 1. ARRANCADA E ACELERAÇÃO NO EIXO X
                if (Input.KeyDown(Keys.D))
                {
                    if (velocidadeAtualX == 0f) velocidadeAtualX = velocidadeInicial;
                    velocidadeAtualX += aceleracao * deltaTime;
                }
                else if (Input.KeyDown(Keys.A))
                {
                    if (velocidadeAtualX == 0f) velocidadeAtualX = -velocidadeInicial;
                    velocidadeAtualX -= aceleracao * deltaTime;
                }
                else
                {
                    // FRENAGEM AUTOMÁTICA X (Quando nenhuma tecla horizontal está pressionada)
                    if (velocidadeAtualX > 0) velocidadeAtualX = Math.Max(0, velocidadeAtualX - desaceleracaoAuto * deltaTime);
                    if (velocidadeAtualX < 0) velocidadeAtualX = Math.Min(0, velocidadeAtualX + desaceleracaoAuto * deltaTime);

                    // DEADZONE X: Corta tremor residual
                    if (Math.Abs(velocidadeAtualX) <= deadzone) velocidadeAtualX = 0f;
                }

                // 2. ARRANCADA E ACELERAÇÃO NO EIXO Y
                if (Input.KeyDown(Keys.S))
                {
                    if (velocidadeAtualY == 0f) velocidadeAtualY = velocidadeInicial;
                    velocidadeAtualY += aceleracao * deltaTime;
                }
                else if (Input.KeyDown(Keys.W))
                {
                    if (velocidadeAtualY == 0f) velocidadeAtualY = -velocidadeInicial;
                    velocidadeAtualY -= aceleracao * deltaTime;
                }
                else
                {
                    // FRENAGEM AUTOMÁTICA Y (Quando nenhuma tecla vertical está pressionada)
                    if (velocidadeAtualY > 0) velocidadeAtualY = Math.Max(0, velocidadeAtualY - desaceleracaoAuto * deltaTime);
                    if (velocidadeAtualY < 0) velocidadeAtualY = Math.Min(0, velocidadeAtualY + desaceleracaoAuto * deltaTime);

                    // DEADZONE Y: Corta tremor residual
                    if (Math.Abs(velocidadeAtualY) <= deadzone) velocidadeAtualY = 0f;
                }

                // 3. CLAMP DE VELOCIDADE (Impede que a nave acelere infinitamente)
                velocidadeAtualX = Math.Max(-velocidadeNave, Math.Min(velocidadeNave, velocidadeAtualX));
                velocidadeAtualY = Math.Max(-velocidadeNave, Math.Min(velocidadeNave, velocidadeAtualY));

                // 4. DESLOCAMENTO FÍSICO
                naveJogador.Left += (int)(velocidadeAtualX * deltaTime);
                naveJogador.Top += (int)(velocidadeAtualY * deltaTime);
            }

                // Modo legado sem inércia (usa MoverNave)
             naveJogador.MoverNave();
            

            // Atualiza Direção/Inversão de sprite
            AtualizarSpriteJogador();
        }
        void AtualizarSpriteJogador()
        {
            if (velocidadeAtualX > 0f && naveJogador.DirecaoX != 1)
            {
                naveJogador.DirecaoX = 1;

                naveJogador.Load(naveJogador.imagensJogador[naveJogador.tipoElemental] + naveJogador.tipoElemental + "-ativado.png");
            }
            else if (velocidadeAtualX < 0f && naveJogador.DirecaoX != -1)
            {
                naveJogador.DirecaoX = -1;

                naveJogador.Load(naveJogador.imagensJogador[naveJogador.tipoElemental] + naveJogador.tipoElemental + "-desativado.png");
            }
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            Input.KeyPressed(e.KeyCode);

            if (upgradeAguardEnter && e.KeyCode == Keys.Enter)
            {
                upgradeAguardEnter = false;
                retomarTudo();
                lblUpgrade.Dispose();
                return;
            }

            if (Keys.K == e.KeyCode)
                naveJogador.bossKills = 3;

            if (e.KeyCode == Keys.Enter)
            {
                if (!jogoPausado) pausarTudo();
                else retomarTudo();
                return;
            }

            switch (e.KeyCode)
            {
                case Keys.D1:
                    if (cenariosDerrotados.Contains(1) && itemDropado == null)
                    {
                        naveJogador.Load(naveJogador.imagensJogador[1] + 1 + "-desativado.png");
                        naveJogador.tipoElemental = 1;
                    }
                    break;

                case Keys.D2:
                    if (cenariosDerrotados.Contains(2) && itemDropado == null)
                    {
                        naveJogador.Load(naveJogador.imagensJogador[2] + 2 + "-desativado.png");
                        naveJogador.tipoElemental = 2;
                    }
                    break;

                case Keys.D3:
                    if (cenariosDerrotados.Contains(3) && itemDropado == null)
                    {
                        naveJogador.Load(naveJogador.imagensJogador[3] + 3 + "-desativado.png");
                        naveJogador.tipoElemental = 3;
                    }
                    break;

                case Keys.D4:
                    if (cenariosDerrotados.Contains(4) && itemDropado == null)
                    {
                        naveJogador.Load(naveJogador.imagensJogador[4] + 4 + "-desativado.png");
                        naveJogador.tipoElemental = 4;
                    }
                    break;

                case Keys.D0:
                    naveJogador.tipoElemental = 0;
                    naveJogador.Load(naveJogador.imagensJogador[0] + 0 + "-desativado.png");
                    break;
            }
        }

        public void pausarTudo()
        {
            jogoPausado = true;
            gameTimer.Stop();
            cronometro.Stop();
            if (naveInimigo?.inimigoTimer != null) naveInimigo.inimigoTimer.Stop();
        }

        public void retomarTudo()
        {
            jogoPausado = false;
            cronometro.Restart();
            gameTimer.Start();
            if (naveInimigo?.inimigoTimer != null) naveInimigo.inimigoTimer.Start();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            Input.KeyReleased(e.KeyCode);
            _teclaAnterior = Keys.None;
        }

        public void PlayCutscene(string videoFileName, int durationSeconds)
        {
            if (cutscenePlaying) return;
            InitializeCutscenePlayer(videoFileName, durationSeconds);
        }

        void InitializeCutscenePlayer(string videoFileName, int durationSeconds)
        {
            string videoRelative = Path.Combine(@"Assets\GDD_Immeasurable Chasm Event Horizon\Video", videoFileName);
            string videoPath = Path.Combine(Application.StartupPath, videoRelative);

            if (!File.Exists(videoPath))
                videoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, videoRelative);

            if (!File.Exists(videoPath))
            {
                DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                while (dir != null && !File.Exists(videoPath))
                {
                    videoPath = Path.Combine(dir.FullName, videoRelative);
                    if (!File.Exists(videoPath)) dir = dir.Parent;
                    else break;
                }
            }

            if (!File.Exists(videoPath))
            {
                MessageBox.Show($"Vídeo '{videoFileName}' não encontrado em:\n{videoPath}", "Erro - Vídeo não localizado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                StopCutscene();
                return;
            }

            try
            {
                cutscenePlaying = true;
                gameTimer.Enabled = false;
                cronometro.Stop();
                if (naveInimigo?.inimigoTimer != null) naveInimigo.inimigoTimer.Stop();

                string wmplayerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Windows Media Player\wmplayer.exe");
                if (!File.Exists(wmplayerPath)) wmplayerPath = "wmplayer.exe";

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = wmplayerPath;
                psi.Arguments = $"\"{videoPath}\" /play /fullscreen";
                psi.UseShellExecute = true;
                psi.CreateNoWindow = false;

                videoProcess = Process.Start(psi);
                videoProcess.EnableRaisingEvents = true;

                cutsceneTimer = new Timer();
                cutsceneTimer.Interval = durationSeconds * 1000;
                cutsceneTimer.Tick += (s, e) =>
                {
                    cutsceneTimer.Stop();
                    StopCutscene();
                };
                cutsceneTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir vídeo '{videoFileName}':\n{ex.Message}", "Erro ao executar wmplayer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                StopCutscene();
            }
        }

        void StopCutscene()
        {
            if (!cutscenePlaying) return;
            cutscenePlaying = false;

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

            try
            {
                
                ProcessStartInfo killPsi = new ProcessStartInfo();
                killPsi.FileName = "cmd.exe";
                killPsi.Arguments = "/c taskkill /IM wmplayer.exe /F /T";
                killPsi.UseShellExecute = false;
                killPsi.CreateNoWindow = true;
                Process killProcess = Process.Start(killPsi);
                killProcess.WaitForExit(2000);
                killProcess.Dispose();
            }
            catch { }

            try
            {
                if (videoProcess != null)
                {
                    videoProcess.Dispose();
                    videoProcess = null;
                }
            }
            catch { }

            System.Threading.Thread.Sleep(200);
            cronometro.Restart();
            gameTimer.Enabled = true;

            if (naveInimigo?.inimigoTimer != null) naveInimigo.inimigoTimer.Start();
        }

        void MainFormLoad(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED contra flickering
                return cp;
            }
        }
    }

    public static class Input
    {
        static HashSet<Keys> keysDown = new HashSet<Keys>();
        public static bool KeyDown(Keys key) => keysDown.Contains(key);
        public static void KeyPressed(Keys key) => keysDown.Add(key);
        public static void KeyReleased(Keys key) => keysDown.Remove(key);
    }
}