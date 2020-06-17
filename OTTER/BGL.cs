#define My_Debug
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OTTER
{
    /// <summary>
    /// -
    /// </summary>
    public partial class BGL : Form
    {
        /* ------------------- */
        #region Environment Variables

        List<Func<int>> GreenFlagScripts = new List<Func<int>>();

        /// <summary>
        /// Uvjet izvršavanja igre. Ako je <c>START == true</c> igra će se izvršavati.
        /// </summary>
        /// <example><c>START</c> se često koristi za beskonačnu petlju. Primjer metode/skripte:
        /// <code>
        /// private int MojaMetoda()
        /// {
        ///     while(START)
        ///     {
        ///       //ovdje ide kod
        ///     }
        ///     return 0;
        /// }</code>
        /// </example>
        public static bool START = true;

        //sprites
        /// <summary>
        /// Broj likova.
        /// </summary>
        public static int spriteCount = 0, soundCount = 0;

        /// <summary>
        /// Lista svih likova.
        /// </summary>
        //public static List<Sprite> allSprites = new List<Sprite>();
        public static SpriteList<Sprite> allSprites = new SpriteList<Sprite>();

        //sensing
        int mouseX, mouseY;
        Sensing sensing = new Sensing();

        //background
        List<string> backgroundImages = new List<string>();
        int backgroundImageIndex = 0;
        string ISPIS = "";

        SoundPlayer[] sounds = new SoundPlayer[1000];
        TextReader[] readFiles = new StreamReader[1000];
        TextWriter[] writeFiles = new StreamWriter[1000];
        bool showSync = false;
        int loopcount;
        DateTime dt = new DateTime();
        String time;
        double lastTime, thisTime, diff;

        #endregion
        /* ------------------- */
        #region Events
#if My_Debug
        public int _cursX = 0;
        public int _cursY = 0;
        //https://www.youtube.com/watch?v=hmkQvMDN4no
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics dc = e.Graphics;

            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.EndEllipsis;
            Font _font = new Font("Cooper Black", 30,FontStyle.Bold);

            //TextRenderer.DrawText(dc, "X = " + _cursX.ToString() + " : " + "Y = " + _cursY.ToString(), _font,
              // new Rectangle(0, 0, 250, 20), Color.Black, flags);

            TextRenderer.DrawText(dc, "Score: " + player.Score, _font,
                new Rectangle(0, 50, 300, 50), Color.DarkCyan, flags);

            TextRenderer.DrawText(dc, "Timer:\n " + seconds, _font,
                new Rectangle(650, 0, 250, 100), Color.Brown, flags);

            //TextRenderer.DrawText(dc, "trigger: " + player.Trigger, _font,
            //   new Rectangle(0, 80, 180, 20), Color.Red, flags);

            //TextRenderer.DrawText(dc, "player " +"X: "+ player.X + " Y: " + player.Y , _font,
            //   new Rectangle(0, 110, 250, 20), Color.Red, flags);

            TextRenderer.DrawText(dc, "Floor: " + player.FloorCount, _font,
               new Rectangle(0, 130, 300, 50), Color.DarkCyan, flags);

            base.OnPaint(e);
        }
#endif
        private void Draw(object sender, PaintEventArgs e)
        {
           
            Graphics g = e.Graphics;

            try
            {
                foreach (Sprite sprite in allSprites)
                {
                    if (sprite != null)
                        if (sprite.Show == true)
                        {
                            g.DrawImage(sprite.CurrentCostume, new Rectangle(sprite.X, sprite.Y, sprite.Width, sprite.Heigth));
                        }
                    if (allSprites.Change)
                        break;
                }
                if (allSprites.Change)
                    allSprites.Change = false;
            }
            catch
            {
                //ako se doda sprite dok crta onda se mijenja allSprites
                MessageBox.Show("Greška!");
            }
        }

        private void startTimer(object sender, EventArgs e)
        {
            START = true;
            timer1.Start();
            timer2.Start();
            timer3.Start();
            timerMoveFloors.Start();
          
            Init();
        }

        private void updateFrameRate(object sender, EventArgs e)
        {
            updateSyncRate();
        }

        /// <summary>
        /// Crta tekst po pozornici.
        /// </summary>
        /// <param name="sender">-</param>
        /// <param name="e">-</param>
        public void DrawTextOnScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var brush = new SolidBrush(Color.WhiteSmoke);
            string text = ISPIS;

            SizeF stringSize = new SizeF();
            Font stringFont = new Font("Arial", 14);
            stringSize = e.Graphics.MeasureString(text, stringFont);

            using (Font font1 = stringFont)
            {
                RectangleF rectF1 = new RectangleF(0, 0, stringSize.Width, stringSize.Height);
                e.Graphics.FillRectangle(brush, Rectangle.Round(rectF1));
                e.Graphics.DrawString(text, font1, Brushes.Black, rectF1);
            }
        }

        private void mouseClicked(object sender, MouseEventArgs e)
        {

            sensing.MouseDown = true;
        }

        private void mouseDown(object sender, MouseEventArgs e)
        {
    
            sensing.MouseDown = true;
        }

        private void mouseUp(object sender, MouseEventArgs e)
        {

            sensing.MouseDown = false;
        }

        private void mouseMove(object sender, MouseEventArgs e)
        {
            mouseX = e.X;
            mouseY = e.Y;

            sensing.Mouse.X = e.X;
            sensing.Mouse.Y = e.Y;
#if My_Debug
            _cursX = e.X;
            _cursY = e.Y;
            this.Refresh();
#endif
        }
        

        //https://youtu.be/X1lfRIi1G6g
        bool _right;
        bool _left;
        bool _jump;

        const int G = 30;//tezina tijela
        int Force;//sila

        private void keyDown(object sender, KeyEventArgs e)
        {
            sensing.Key = e.KeyCode.ToString();
            sensing.KeyPressedTest = true;

            if (e.KeyCode == Keys.P)
            {

                pauza = !pauza;
                if (pauza)//ako je pauza true,tj pritisnuta tipka P
                {
                    timerMoveFloors.Stop();
                    Menu mn = new Menu();//otvori menu pauze
                    mn.Pause = pauza;
                    mn.ShowDialog();
                    pauza = mn.Pause;
                    timerMoveFloors.Start();
               
                }
            }


            if (e.KeyCode == Keys.Right) { _right = true; }//ako je pritisnuta strelica desno 
            if (e.KeyCode == Keys.Left) { _left = true; }//ako je pritisnuta strelica lijevo
           
            if (_jump == false)//ako player nije u skoku,ovime omogucujemo da jednim pritiskom space tipke player skoci za G
            {
               
                if (e.KeyCode == Keys.Space)//ako pritisnemo space 
                {
                    Force = G;//sila se postavlja na G
                    _jump = true;//jump je true - znaci da smo skocili

                }
            }


        }

        private void keyUp(object sender, KeyEventArgs e)
        {
            sensing.Key = "";
            sensing.KeyPressedTest = false;

            if (e.KeyCode == Keys.Right) { _right = false; }//kada pustimo tipku strelica desno
            if (e.KeyCode == Keys.Left) { _left = false; }//kada pustimo tipku strelica lijevo
        }

        private void Update(object sender, EventArgs e)
        {
            if (sensing.KeyPressed(Keys.Escape))
            {
                START = false;
            }

            if (START)
            {
                this.Refresh();
            }

        }

        private void timerMoveFloors_Tick(object sender, EventArgs e)
        {
            //timer koji odbrojava sekunde do ubrzanja kojim se micu platforme
            if (player.Trigger == true )//player ovaj trigger postavi na true kada prvi put dolazi do pozicije s koje se pocimaju micati platforme
            {
                seconds--;//stoperica koja odbrojava od 30s do 0s
                if (seconds == 0)//ako je odbrojio
                {
                    floorSpeed += 1;//povecava se brzina platformi
                    seconds = 30;//stoperica se ponovno postavi na 30s
                }

            }
             
        }

        public string IMG = "BLUE"; //poslan odabir iz Options forme

        private void timer3_Tick(object sender, EventArgs e)
        {
            #region movingANDjumping
            if (_right == true)//pritisnuta je strelica desno
            {
                if (IMG == "BLUE")
                {
                    player.ChangeImage("sprites\\ballBLUE_R.png");
                }
                else if (IMG == "RED")
                {
                    player.ChangeImage("sprites\\ballRED_R.png");
                }
                player.X += 10;
            }
            if (_left == true)//pritisnuta je strelica lijevo
            {
                if (IMG == "BLUE")
                {
                    player.ChangeImage("sprites\\ballBLUE_L.png");
                }
                else if (IMG == "RED")
                {
                    player.ChangeImage("sprites\\ballRED_L.png");
                }
                player.X -= 10;
            }

            if (_jump == true)//ako je player u skoku ( space )
            {
                player.Y -= Force;//player ide gore za iznos sile
                Force -= 1;//sila se smanjuje
   
                if (player.Y <= 0)
                {
                    player.Y = 0;
                }
                
            }

            if (player.Y + player.Heigth >= (GameOptions.DownEdge - player.Heigth) && player.Trigger == false)//ako player stigne do dna ekrana i tu je jos uvijek floor
            {
                player.Y = (GameOptions.DownEdge - player.Heigth) - player.Heigth;//zaustavi playera na flooru
                _jump = false;//vise ne skace
                Force = 0;//sila kojom skace je 0
            }
            else
            {
                player.Y += 10;//ako player nije dosao do dna nastavi padati
            }
            #endregion

            #region jumpingONplatforms
            foreach (Brick b in bricks)
            {
                if (player.TouchingSprite(b))//ako je player dodirnuo platformu
                {
                    if (b.Y - player.Heigth <= 0)
                    {
                        player.Y = 0;
                    }
                    else
                        player.Y = b.Y - player.Heigth;//postavi playera na platformu
                    Force = 0;//postavi silu na nulu da prestane skok
                    _jump = false;//vise ne skace
                    //bricksHopOn je lista u kojoj su sve platforme na koje je vec player skocio
                    if (!bricksHopOnDown.Contains(b))//ako player nije vec prije skocio na tu platformu
                    {
                        player.Score += 100;//dodaj playeru bodove za skok 
                        player.FloorCount++;//brojac platformi
                        bricksHopOnDown.Add(b);//dodaj tu platformu u listu onih na koje je skocio

                    }
                    if (bricksHopOnDown.Last().Y < player.Y)//ako je posljednja platforma na koju je player skocio iznad samog playera,znaci da je pao sa te platfrome
                    {
                        player.Score -= 100;//oduzmu se bodovi zbog pada
                        player.FloorCount--;
                        bricksHopOnDown.RemoveAt(bricksHopOnDown.Count - 1);//platforma s koje je pao se makne iz liste na koje je skocio da bi mu se kada opet na nju skoci dali bodovi
                    }

                }
            }

            foreach (Brick b in bricksU)
            {
                if (player.TouchingSprite(b))//ako je player dodirnuo platformu
                {
                    if (b.Y - player.Heigth <= 0)
                    {
                        player.Y += 100;
                    }
                    else
                        player.Y = b.Y - player.Heigth;//postavi playera na platformu
                    Force = 0;//postavi silu na nulu da prestane skok
                    _jump = false;//vise ne skace
                }
            }
            #endregion

            #region jumpOUT
            if (player.Y >= (GameOptions.DownEdge - player.Heigth) || player.Score < 0)//ako player ispadne iz ekrana ili postigne negativne bodove
            {
                FinishMenu mnu = new FinishMenu();//nova menu forma
                mnu.Score = player.Score;//poslati bodove na menu svojstvo Score
                BGL.allSprites.Clear();//pocisti listu svih spriteova
                GC.Collect();//pokupi smece

                Wait(0.4);
                START = false;//zaustavi sve skripte
                this.Close();//zatvori trenutni bgl
                mnu.Show();//otvori menu formu
            }
            #endregion

        }
        #endregion
      
        /* ------------------- */
        #region Start of Game Methods

        //my
        #region my

        //private void StartScriptAndWait(Func<int> scriptName)
        //{
        //    Task t = Task.Factory.StartNew(scriptName);
        //    t.Wait();
        //}

        //private void StartScript(Func<int> scriptName)
        //{
        //    Task t;
        //    t = Task.Factory.StartNew(scriptName);
        //}

        private int AnimateBackground(int intervalMS)
        {
            while (START)
            {
                setBackgroundPicture(backgroundImages[backgroundImageIndex]);
                Game.WaitMS(intervalMS);
                backgroundImageIndex++;
                if (backgroundImageIndex == 3)
                    backgroundImageIndex = 0;
            }
            return 0;
        }

        private void KlikNaZastavicu()
        {
            foreach (Func<int> f in GreenFlagScripts)
            {
                Task.Factory.StartNew(f);
            }
        }

#endregion

        /// <summary>
        /// BGL
        /// </summary>
        public BGL()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Pričekaj (pauza) u sekundama.
        /// </summary>
        /// <example>Pričekaj pola sekunde: <code>Wait(0.5);</code></example>
        /// <param name="sekunde">Realan broj.</param>
        public void Wait(double sekunde)
        {
            int ms = (int)(sekunde * 1000);
            Thread.Sleep(ms);
        }

        //private int SlucajanBroj(int min, int max)
        //{
        //    Random r = new Random();
        //    int br = r.Next(min, max + 1);
        //    return br;
        //}

        /// <summary>
        /// -
        /// </summary>
        public void Init()
        {
            if (dt == null) time = dt.TimeOfDay.ToString();
            loopcount++;
            //Load resources and level here
            this.Paint += new PaintEventHandler(DrawTextOnScreen);
            SetupGame();
        }

        /// <summary>
        /// -
        /// </summary>
        /// <param name="val">-</param>
        public void showSyncRate(bool val)
        {
            showSync = val;
            if (val == true) syncRate.Show();
            if (val == false) syncRate.Hide();
        }

        /// <summary>
        /// -
        /// </summary>
        public void updateSyncRate()
        {
            if (showSync == true)
            {
                thisTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
                diff = thisTime - lastTime;
                lastTime = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;

                double fr = (1000 / diff) / 1000;

                int fr2 = Convert.ToInt32(fr);

                syncRate.Text = fr2.ToString();
            }

        }

        //stage
#region Stage

        /// <summary>
        /// Postavi naslov pozornice.
        /// </summary>
        /// <param name="title">tekst koji će se ispisati na vrhu (naslovnoj traci).</param>
        public void SetStageTitle(string title)
        {
            this.Text = title;
        }

        /// <summary>
        /// Postavi boju pozadine.
        /// </summary>
        /// <param name="r">r</param>
        /// <param name="g">g</param>
        /// <param name="b">b</param>
        public void setBackgroundColor(int r, int g, int b)
        {
            this.BackColor = System.Drawing.Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Postavi boju pozornice. <c>Color</c> je ugrađeni tip.
        /// </summary>
        /// <param name="color"></param>
        public void setBackgroundColor(Color color)
        {
            this.BackColor = color;
        }

        /// <summary>
        /// Postavi sliku pozornice.
        /// </summary>
        /// <param name="backgroundImage">Naziv (putanja) slike.</param>
        public void setBackgroundPicture(string backgroundImage)
        {
            this.BackgroundImage = new Bitmap(backgroundImage);
        }

        /// <summary>
        /// Izgled slike.
        /// </summary>
        /// <param name="layout">none, tile, stretch, center, zoom</param>
        public void setPictureLayout(string layout)
        {
            if (layout.ToLower() == "none") this.BackgroundImageLayout = ImageLayout.None;
            if (layout.ToLower() == "tile") this.BackgroundImageLayout = ImageLayout.Tile;
            if (layout.ToLower() == "stretch") this.BackgroundImageLayout = ImageLayout.Stretch;
            if (layout.ToLower() == "center") this.BackgroundImageLayout = ImageLayout.Center;
            if (layout.ToLower() == "zoom") this.BackgroundImageLayout = ImageLayout.Zoom;
        }

#endregion

        //sound
#region sound methods

        /// <summary>
        /// Učitaj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        /// <param name="file">-</param>
        public void loadSound(int soundNum, string file)
        {
            soundCount++;
            sounds[soundNum] = new SoundPlayer(file);
        }

        /// <summary>
        /// Sviraj zvuk.
        /// </summary>
        /// <param name="soundNum">-</param>
        public void playSound(int soundNum)
        {
            sounds[soundNum].Play();
        }

        /// <summary>
        /// loopSound
        /// </summary>
        /// <param name="soundNum">-</param>
        public void loopSound(int soundNum)
        {
            sounds[soundNum].PlayLooping();
        }

        /// <summary>
        /// Zaustavi zvuk.
        /// </summary>
        /// <param name="soundNum">broj</param>
        public void stopSound(int soundNum)
        {
            sounds[soundNum].Stop();
        }

#endregion

        //file
#region file methods

        /// <summary>
        /// Otvori datoteku za čitanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToRead(string fileName, int fileNum)
        {
            readFiles[fileNum] = new StreamReader(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToRead(int fileNum)
        {
            readFiles[fileNum].Close();
        }

        /// <summary>
        /// Otvori datoteku za pisanje.
        /// </summary>
        /// <param name="fileName">naziv datoteke</param>
        /// <param name="fileNum">broj</param>
        public void openFileToWrite(string fileName, int fileNum)
        {
            writeFiles[fileNum] = new StreamWriter(fileName);
        }

        /// <summary>
        /// Zatvori datoteku.
        /// </summary>
        /// <param name="fileNum">broj</param>
        public void closeFileToWrite(int fileNum)
        {
            writeFiles[fileNum].Close();
        }

        /// <summary>
        /// Zapiši liniju u datoteku.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <param name="line">linija</param>
        public void writeLine(int fileNum, string line)
        {
            writeFiles[fileNum].WriteLine(line);
        }

        /// <summary>
        /// Pročitaj liniju iz datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća pročitanu liniju</returns>
        public string readLine(int fileNum)
        {
            return readFiles[fileNum].ReadLine();
        }

        /// <summary>
        /// Čita sadržaj datoteke.
        /// </summary>
        /// <param name="fileNum">broj datoteke</param>
        /// <returns>vraća sadržaj</returns>
        public string readFile(int fileNum)
        {
            return readFiles[fileNum].ReadToEnd();
        }

#endregion

        //mouse & keys
#region mouse methods

        /// <summary>
        /// Sakrij strelicu miša.
        /// </summary>
        public void hideMouse()
        {
            Cursor.Hide();
        }

        /// <summary>
        /// Pokaži strelicu miša.
        /// </summary>
        public void showMouse()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Provjerava je li miš pritisnut.
        /// </summary>
        /// <returns>true/false</returns>
        public bool isMousePressed()
        {
            //return sensing.MouseDown;
            return sensing.MouseDown;
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">naziv tipke</param>
        /// <returns></returns>
        public bool isKeyPressed(string key)
        {
            if (sensing.Key == key)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Provjerava je li tipka pritisnuta.
        /// </summary>
        /// <param name="key">tipka</param>
        /// <returns>true/false</returns>
        public bool isKeyPressed(Keys key)
        {
            if (sensing.Key == key.ToString())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
          
        }

        #endregion

        #endregion
        /* ------------------- */

        /* ------------ GAME CODE START ------------ */

        /* Game variables *///----------------------------------------------------------------------------------
        
        Sprite floor;
        Sprite magnet;
       
        Gem gem1;
        Gem gem2;
        Gem gem3;
        Gem gem4;
        List<Gem> gems;

        ExtraGem egem;
        FalseGem fgem;

        Brick brick1;
        Brick brick2;
        Brick brick3;
        Brick brick4;

        Brick brickU1;
        Brick brickU2;
        Brick brickU3;
        Brick brickU4;

        List<Brick> bricks;//u ovoj listi su sve 4 platforme koje idu dolje
        List<Brick> bricksU;//platforme koje idu prema gore
        List<Brick> bricksHopOnDown;//u ovoj listi su platforme na koje player skoci
        List<Brick> bricksHopOnUp;//u ovoj listi su platforme na koje player skoci koje idu gore
        Brick[] brickArray;//ovo je niz svih platformi,lakse je s nizom mijenjati koordinate i slike svake pojedine platforme
        Brick[] brickArrayUp;//niz platformi koje idu gore
        
        string[] brickImg;//niz u kojem su 3 slike koje platfroma moze imati,nasumicno se mijenjaju svakim novim ulaskom platforme u igru
        int floorSpeed;//brzina kojom ce se pomicati platfrome,svako 30 sekundi ona se povecava za odredjeni iznos
        int seconds;//stoperica kojom odbrojavamo 30 sekundi za ubrzanje platformi
        bool pauza = false;
 
        Player player;
        
        Random g;

       
        /* Initialization */
        public void SetupGame()
        {
            ////1. setup stage-----------------------------------------------------------------------
           // this.CenterToScreen();
            SetStageTitle("Ball");
            setBackgroundColor(Color.SkyBlue);            
            //setBackgroundPicture("backgrounds\\back.png");
            //setPictureLayout("stretch");////none, tile, stretch, center, zoom

            ////2. add sprites----------------------------------------------------------------------
     
            g = new Random();
            brickImg = new string[3];
            seconds = 30;
            floorSpeed = 3;
            bricks = new List<Brick>();
            bricksU = new List<Brick>();
            bricksHopOnDown = new List<Brick>();
           //bricksHopOnUp = new List<Brick>();
            brickArray = new Brick[4];
            brickArrayUp = new Brick[4];
            gems = new List<Gem>();

            
            magnet = new Sprite("sprites\\magnet.png",750,150);
            magnet.SetVisible(false);
            Game.AddSprite(magnet);

            if (IMG == "RED")
            {
                player = new Player("sprites\\ballRED_R.png", 15, 700);
            }
            else
            {
                player = new Player("sprites\\ballBLUE_R.png", 15, 700);
            }
            
            Game.AddSprite(player);

            //------------------------------------------------------------------------------BRICKS
            #region BRICKS
            floor = new Sprite("sprites\\oneB4.png", 2, 750);
            floor.Width = GameOptions.RightEdge;
            Game.AddSprite(floor);

            brickImg[0] = "sprites\\oneB4.png"; 
            brickImg[1] = "sprites\\twoB4.png";
            brickImg[2] = "sprites\\threeB4.png";

            brickU1 = new Brick("sprites\\oneB4C.png", 40, 200);
            Game.AddSprite(brickU1);

            brickU2 = new Brick("sprites\\oneB4C.png", 640, 400);
            Game.AddSprite(brickU2);

            brickU3 = new Brick("sprites\\oneB4C.png", 240, 600);
            Game.AddSprite(brickU3);

            brickU4 = new Brick("sprites\\oneB4C.png", 690, 800);
            Game.AddSprite(brickU4);

            int r1 = g.Next(0,3);//nasumicno odabrana slika 
            int x1 = g.Next(0, 600);//gameoptions.rightedge(900)-najduzaplatforma.width(300)=600
            brick1 = new Brick(brickImg[r1], x1, 625);//
            Game.AddSprite(brick1);

            int r2 = g.Next(0, 3);//nasumicno odabrana slika 
            int x2 = g.Next(0, 600);
            brick2 = new Brick(brickImg[r2], x2, 450);
            Game.AddSprite(brick2);

            int r3 = g.Next(0, 3);//nasumicno odabrana slika 
            int x3 = g.Next(0, 600);
            brick3 = new Brick(brickImg[r3], x3, 225);
            Game.AddSprite(brick3);

            int r4 = g.Next(0, 3);//nasumicno odabrana slika 
            int x4 = g.Next(0, 600);
            brick4 = new Brick(brickImg[r4], x4, 0);
            Game.AddSprite(brick4);

            bricks.Add(brick1);
            bricks.Add(brick2);
            bricks.Add(brick3);
            bricks.Add(brick4);

            bricksU.Add(brickU1);
            bricksU.Add(brickU2);
            bricksU.Add(brickU3);
            bricksU.Add(brickU4);

            brickArray[0] = brick1;
            brickArray[1] = brick2;
            brickArray[2] = brick3;
            brickArray[3] = brick4;

            brickArrayUp[0] = brickU1;
            brickArrayUp[1] = brickU2;
            brickArrayUp[2] = brickU3;
            brickArrayUp[3] = brickU4;



            #endregion
            //------------------------------------------------------------------------------BRICKS  

            //------------------------------------------------------------------------------GEMS
            #region GEMS
            egem = new ExtraGem("sprites\\gemH.png", 100, -100);
            fgem = new FalseGem("sprites\\bomb.png", 100, -100); 

            gem1 = new Gem("sprites\\gem.png", 0, 550);
            Game.AddSprite(gem1);

            gem2 = new Gem("sprites\\gem.png", 200, 500);
            Game.AddSprite(gem2);

            gem3 = new Gem("sprites\\gem.png", 450, 125);
            Game.AddSprite(gem3);

            gem4 = new Gem("sprites\\gem.png", 300, 0);
            Game.AddSprite(gem4);

            gems.Add(gem1);
            gems.Add(gem2);
            gems.Add(gem3);
            gems.Add(gem4);

            Game.AddSprite(egem);
            Game.AddSprite(fgem);
            #endregion
            //-----------------------------------------------------------------------------GEMS


            ////3. scripts that start
            Game.StartScript(GemCollect);
            Game.StartScript(SpecialGemCollect);
            Game.StartScript(PlatformMoveDown);
            Game.StartScript(PlatformMoveUp);
            Game.StartScript(ColorChange);
         
            

         
        }


        /* Scripts *///--------------------------------------------------------------------------------------------
        #region SCRIPTS
        public int GemCollect()
        {
            while (START)
            {
                if (pauza)
                {
                    Wait(1);
                    continue;
                }
                if (gems.Count() != 0)
                {
                    foreach (Gem g in gems)
                    {
                        if (player.TouchingSprite(g))//ako player pokupi gem
                        {
                            player.Score += g.Points;
                            g.GotoXY();
                        }
                        if (g.Y >= GameOptions.DownEdge)//ako gem izadje iz ekrana,oduzmi bodove playeru
                        {
                            player.Score -= g.Points;
                            g.GotoXY();
                        }
                       
                    }
                }

                //ako player pokupi dijamant,dobije extra 500 bodova
                if ( seconds <= 10 && player.Trigger == true || egem.Trigger == true)
                {//dijamant pada dok je njegov triger true ili dok ne izadje iz ekrana ili ga player ne pokupi
                    egem.Trigger = true;
                    egem.Y += 8;
                    if (egem.Y >= GameOptions.DownEdge)
                    {
                        egem.GotoXY(g.Next(0, 750), -100);
                        egem.Trigger = false;
                    }
                    else if (player.TouchingSprite(egem))
                    {
                        player.Score += egem.Points;
                        egem.GotoXY(g.Next(0, 750), -100);
                        egem.Trigger = false;
                    }
                }
                //ako player pokupi crni gem,gubi 1000 bodova
                if (seconds >= 20 && player.Trigger == true || fgem.Trigger == true)
                {
                    fgem.Trigger = true;
                    fgem.Y += 8;
                    if (fgem.Y >= GameOptions.DownEdge)
                    {
                        fgem.GotoXY(g.Next(0, 750), -100);
                        fgem.Trigger = false;
                    }
                    else if (player.TouchingSprite(fgem))
                    {
                        player.Score += fgem.Points;
                        fgem.GotoXY(g.Next(0, 750), -100);
                        fgem.Trigger = false;
                    }
                }
                Wait(0.02);
            }
            return 0;
        }

        public int SpecialGemCollect()
        { 
            while (START)
            {
                if (pauza)
                {
                    Wait(1);
                    continue;
                }
                while (seconds <= 10)
                {
                    magnet.SetVisible(true);
                    Gem cg = ClosestGem(gems);
                    while (true)
                    {
                        cg.PointToSprite(player);
                        cg.MoveSteps(50);
                        if (player.TouchingSprite(cg))
                        {
                            player.Score += cg.Points/2;
                            cg.GotoXY();
                            break;
                        }
                        Wait(0.05);
                    }

                }
                magnet.SetVisible(false);

            }
            return 0;
        }
       
        public int PlatformMoveDown()
        {
            while (START)
            {
                if (pauza)
                {
                    Wait(1);
                    continue;
                }
                //platforme se tek pocmu micati kada se player popne na Y=225,tada pokrenem triger koji osigurava da se nastave micati
                if (player.Y <= 225 || player.Trigger == true)
                {
                    player.Trigger = true;
                    floor.GotoXY(0, -100);
                    for (int i = 0; i < brickArray.Length; i++)
                    {//4 platforme,4 gema
                      
                        brickArray[i].Y += floorSpeed;
                        gems[i].Y += floorSpeed;
                        if (brickArray[i].Y >= GameOptions.DownEdge)//kada platforma izadje iz ekrana
                        {
                            brickArray[i].BrickChange(brickImg,ref bricksHopOnDown);
                        }
                    }
                }
               
                Wait(0.01);
            }
            return 0;
        }


        public int PlatformMoveUp()
        {
            while (START)
            {
                if (pauza)
                {
                    Wait(1);
                    continue;
                }
                //platforme se tek pocmu micati kada se player popne na Y=225,tada pokrenem triger koji osigurava da se nastave micati
                if (player.Y <= 225 || player.Trigger == true)
                {
                    player.Trigger = true;
                    floor.GotoXY(0, -100);//maknem pod 

                    for (int i = 0; i < brickArrayUp.Length; i++)
                    {//4 platforme

                        brickArrayUp[i].Y -= floorSpeed;
                       
                        if (brickArrayUp[i].Y + brickArrayUp[i].Heigth <= 0)//kada platforma izadje iz ekrana
                        {
                              brickArrayUp[i].Y = 800;
                        }
                    }
                }
                Wait(0.1);
               

            }
            return 0;
        }


        public int ColorChange()
        {
            while (START)
            {
                if (pauza)
                {
                    Wait(1);
                    continue;
                }
                if (player.FloorCount == 50)
                {
                    brickImg[0] = "sprites\\oneB2.png";
                    brickImg[1] = "sprites\\twoB2.png";
                    brickImg[2] = "sprites\\threeB2.png";

                }
                if (player.FloorCount == 150)
                {
                    brickImg[0] = "sprites\\oneB3.png";
                    brickImg[1] = "sprites\\twoB3.png";
                    brickImg[2] = "sprites\\threeB3.png";
                    
                }
                if (player.FloorCount == 300)
                {
                    brickImg[0] = "sprites\\oneB.png";
                    brickImg[1] = "sprites\\twoB.png";
                    brickImg[2] = "sprites\\threeB.png";

                }

            }
            return 0;
        }


        private Gem ClosestGem(List<Gem> gems)
        {
            double min = double.MaxValue;
            Gem closestG = gems[0];
            for (int i = 0; i < gems.Count; i++)
            {
                if (Dist(player, gems[i]) <= min)
                {
                    min = Dist(player, gems[i]);
                    closestG = gems[i];
                }
            }
            return closestG;
        }
        private double Dist(Player p, Gem g)
        {
            return Math.Sqrt((p.X - g.X) * (p.X - g.X) + (p.Y - g.Y) * (p.Y - g.Y));
        }


        #endregion

        /* --------------------------------------------------- GAME CODE END ---------------------------------------------- */


    }
}
