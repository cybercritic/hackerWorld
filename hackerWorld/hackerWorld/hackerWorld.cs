using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using hackerWorldNS.HackerWorldService;
using System.Security.Cryptography;

namespace hackerWorldNS
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class hackerWorld : Microsoft.Xna.Framework.Game
    {
        enum hex { A = 10, B, C, D, E, F }

        const string passwordSalt = ".passwordHashSalt";

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        PrimitiveBatch primitiveBatch;
        KeyboardState oldKeyboardState;
        MouseState mouseState;
        Point screenSize = new Point();
        
        SystemObjects sysObjects = new SystemObjects();

        World gameWorld;

        //should probably move this into gameworld
        IhackerWorldService svrClient;

        Object activeInputHolder = null;
        Object activeDrag = null;

        Texture2D backgroundTexture;
        Texture2D blankTexture;
        Color tmpCol = Color.DarkBlue;

        List<Texture2D> programTextures = new List<Texture2D>();

        bool mouseDownLeft;
        double tenthTimer = 0.1;
        double secondTimer = 1;
        int minuteTimer = 0;

        private string userDirectory = "";
        private string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        
        string bufferSTR = "";

        public hackerWorld()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            userDirectory += "/cybercritics";
            Directory.CreateDirectory(userDirectory);
            userDirectory += "/hackerWorld";
            Directory.CreateDirectory(userDirectory);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.Window.Title = "Hacker World Online (" + assemblyVersion + ")";

            //adjust window size
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 576;
            graphics.ApplyChanges();

            screenSize.X = 1024;
            screenSize.Y = 576;

            //init gameworld
            gameWorld = new World();

            try
            {
                gameWorld.Username = (string)System.Windows.Forms.Application.UserAppDataRegistry.GetValue("userName");
            }
            catch { }

            //show mouse
            this.IsMouseVisible = true;

            //limit update interval to ease CPU load
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan(1000);

            //server connection
            svrClient = new IhackerWorldServiceClient();
            //svrClient.Open();
            
            //tmp col
            tmpCol.A = 32;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            primitiveBatch = new PrimitiveBatch(GraphicsDevice);

            try
            {

                // TODO: use this.Content to load your game content here
                font = Content.Load<SpriteFont>("fontImport");

                backgroundTexture = Content.Load<Texture2D>("textures/back576");
                blankTexture = Content.Load<Texture2D>("textures/2x2");
                
                //textures
                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[0] = Content.Load<Texture2D>("textures/uploader");
                programTextures[0].Name = "uploader";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[1] = Content.Load<Texture2D>("textures/downloader");
                programTextures[1].Name = "downloader";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[2] = Content.Load<Texture2D>("textures/hddBlue");
                programTextures[2].Name = "hddBlue";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[3] = Content.Load<Texture2D>("textures/help");
                programTextures[3].Name = "help";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[4] = Content.Load<Texture2D>("textures/market");
                programTextures[4].Name = "market";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[5] = Content.Load<Texture2D>("textures/noImage");
                programTextures[5].Name = "noImage";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[6] = Content.Load<Texture2D>("textures/SQL injector");
                programTextures[6].Name = "SQL injector";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[7] = Content.Load<Texture2D>("textures/Service Scanner");
                programTextures[7].Name = "Service Scanner";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[8] = Content.Load<Texture2D>("textures/FTP server");
                programTextures[8].Name = "FTP server";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[9] = Content.Load<Texture2D>("textures/HTTP server");
                programTextures[9].Name = "HTTP server";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[10] = Content.Load<Texture2D>("textures/IRC server");
                programTextures[10].Name = "IRC server";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[11] = Content.Load<Texture2D>("textures/deleter");
                programTextures[11].Name = "deleter";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[12] = Content.Load<Texture2D>("textures/remote exploit");
                programTextures[12].Name = "remote exploit";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[13] = Content.Load<Texture2D>("textures/local exploit");
                programTextures[13].Name = "local exploit";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[14] = Content.Load<Texture2D>("textures/compressor");
                programTextures[14].Name = "compressor";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[15] = Content.Load<Texture2D>("textures/SQL server");
                programTextures[15].Name = "SQL server";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[16] = Content.Load<Texture2D>("textures/logger");
                programTextures[16].Name = "logger";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[17] = Content.Load<Texture2D>("textures/encryptor");
                programTextures[17].Name = "encryptor";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[18] = Content.Load<Texture2D>("textures/virus");
                programTextures[18].Name = "virus";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[19] = Content.Load<Texture2D>("textures/backdoor");
                programTextures[19].Name = "backdoor";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[20] = Content.Load<Texture2D>("textures/key logger");
                programTextures[20].Name = "key logger";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[21] = Content.Load<Texture2D>("textures/Dictionary Cracker");
                programTextures[21].Name = "Dictionary Cracker";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[22] = Content.Load<Texture2D>("textures/ftp mapper");
                programTextures[22].Name = "ftp mapper";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[23] = Content.Load<Texture2D>("textures/http mapper");
                programTextures[23].Name = "http mapper";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[24] = Content.Load<Texture2D>("textures/Brute force");
                programTextures[24].Name = "Brute force";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[25] = Content.Load<Texture2D>("textures/root kit");
                programTextures[25].Name = "root kit";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[26] = Content.Load<Texture2D>("textures/cash");
                programTextures[26].Name = "cash";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[27] = Content.Load<Texture2D>("textures/gold");
                programTextures[27].Name = "gold";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[28] = Content.Load<Texture2D>("textures/anti-virus");
                programTextures[28].Name = "anti-virus";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[29] = Content.Load<Texture2D>("textures/trash");
                programTextures[29].Name = "trash";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[30] = Content.Load<Texture2D>("textures/tasks");
                programTextures[30].Name = "tasks";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[31] = Content.Load<Texture2D>("textures/login");
                programTextures[31].Name = "login";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[32] = Content.Load<Texture2D>("textures/compiler");
                programTextures[32].Name = "compiler";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[33] = Content.Load<Texture2D>("textures/time");
                programTextures[33].Name = "time";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[34] = Content.Load<Texture2D>("textures/hacker");
                programTextures[34].Name = "hacker";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[35] = Content.Load<Texture2D>("textures/hardware");
                programTextures[35].Name = "hardware";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[36] = Content.Load<Texture2D>("textures/memory");
                programTextures[36].Name = "memory";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[37] = Content.Load<Texture2D>("textures/addhdd");
                programTextures[37].Name = "addhdd";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[38] = Content.Load<Texture2D>("textures/proxy");
                programTextures[38].Name = "proxy";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[39] = Content.Load<Texture2D>("textures/network");
                programTextures[39].Name = "network";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[40] = Content.Load<Texture2D>("textures/mission");
                programTextures[40].Name = "mission";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[41] = Content.Load<Texture2D>("textures/whiteHat");
                programTextures[41].Name = "whiteHat";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[42] = Content.Load<Texture2D>("textures/blackHat");
                programTextures[42].Name = "blackHat";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[43] = Content.Load<Texture2D>("textures/forward");
                programTextures[43].Name = "forward";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[44] = Content.Load<Texture2D>("textures/back");
                programTextures[44].Name = "back";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[45] = Content.Load<Texture2D>("textures/noPass");
                programTextures[45].Name = "noPass";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[46] = Content.Load<Texture2D>("textures/yesPass");
                programTextures[46].Name = "yesPass";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[47] = Content.Load<Texture2D>("textures/unknown");
                programTextures[47].Name = "unknown";

                programTextures.Add(new Texture2D(GraphicsDevice, 48, 48));
                programTextures[48] = Content.Load<Texture2D>("textures/stats");
                programTextures[48].Name = "stats";
            }
            catch 
            { 
                System.Windows.Forms.MessageBox.Show("Error loading textures.");
                this.Exit();
            }

            //init windows
            createLoginMenu();
            createRegisterMenu();
            createHelpMenu();
            createHDDwin();
            createHelpWin();
            createMarketWin();
            createCPUwin();
            createCompilerWin();
            createHardwareWin();
            createNetworkWin();
            createMissionWin();
            createStatsWin();
            createDonationsWin();

            //init sys icons
            createSystemIcons();

            checkClientVersion();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            
            // TODO: Add your update logic here
            checkInputKeyboard();
            checkMouseInput();

            tenthTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            tenthUpdate();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            
            drawBackground();

            drawSystemIcons();
            drawUserInfo();
            drawLink();
            gameWorld.Draw(spriteBatch, primitiveBatch);

            foreach (Window win in sysObjects.systemWindows)
                win.Draw(spriteBatch, primitiveBatch);

            foreach (Window win in sysObjects.systemWindows)
                if (win.Focus)
                {
                    win.Draw(spriteBatch, primitiveBatch);
                    break;
                }

            spriteBatch.Begin();
            spriteBatch.DrawString(font, bufferSTR, new Vector2(2, screenSize.Y - 15), Color.Black);
            spriteBatch.End();

            

            drawDragging(spriteBatch);
            

            base.Draw(gameTime);
        }

        private void checkInputKeyboard()
        {
            try
            {
                //get states
                KeyboardState keyboardState = Keyboard.GetState();

                Keys[] oldPressed = oldKeyboardState.GetPressedKeys();
                Keys[] newPressed = keyboardState.GetPressedKeys();
                if (newPressed.Count() != oldPressed.Count())
                {
                    if (oldPressed.Count() > 1 && (oldPressed[0] == Keys.LeftShift || oldPressed[0] == Keys.RightShift))
                    {
                        bufferSTR = convertKeyToChar(oldPressed[1], oldPressed.Contains(Keys.LeftShift) || oldPressed.Contains(Keys.RightShift));
                        relayInput(bufferSTR);
                        if (activeInputHolder != null && activeInputHolder.GetType() == typeof(TextBox) && ((TextBox)activeInputHolder).Masked)
                            bufferSTR = "*";
                    }
                    else if (oldPressed.Count() > 0)
                    {
                        bufferSTR = convertKeyToChar(oldPressed[0], oldPressed.Contains(Keys.LeftShift) || oldPressed.Contains(Keys.RightShift));
                        relayInput(bufferSTR);
                        if (activeInputHolder != null && activeInputHolder.GetType() == typeof(TextBox) && ((TextBox)activeInputHolder).Masked)
                            bufferSTR = "*";
                    }
                }
                oldKeyboardState = keyboardState;
            }
            catch { }
        }

        /// <summary>
        /// check for mouse activity
        /// </summary>
        private void checkMouseInput()
        {
            //mouse
            mouseState = Mouse.GetState();

            //left mouse pressed
            if (mouseState.LeftButton == ButtonState.Pressed &&
                mouseState.X >= 0 && mouseState.X <= screenSize.X && mouseState.Y >= 0 && mouseState.Y <= screenSize.Y)
            {
                mouseDownLeft = true;
                if (activeDrag == null)//check if we are dragging, so we don't trigger other events
                {
                    //only one window should be moving at a time
                    bool found = false;
                    foreach (Window win in sysObjects.systemWindows)
                        if (win.Moving)
                        {
                            win.mouseDown(mouseState);
                            found = true;
                            break;
                        }

                    if(!found)
                        foreach (Window win in sysObjects.systemWindows)
                            win.mouseDown(mouseState);
                }
            }
            //left mouse button pressed/released
            else if (mouseState.LeftButton == ButtonState.Released && mouseDownLeft)
            {
                foreach (Window win in sysObjects.systemWindows)
                {
                    win.click(new Vector2(mouseState.X, mouseState.Y));
                    activeInputHolder = win.getActiveObject();
                    win.mouseUp(mouseState);
                }

                foreach (DropBox db in sysObjects.systemIcons)
                {
                    if (mouseState.X >= db.Position.X && mouseState.X <= db.Position.X + db.Size.X &&
                        mouseState.Y >= db.Position.Y && mouseState.Y <= db.Position.Y + db.Size.Y)
                        db.onMouseUp();
                }
                foreach (DropBox box in sysObjects.systemIcons)
                    box.click(new Vector2(mouseState.X, mouseState.Y));

                //drop the drag
                activeDrag = null;

                mouseDownLeft = false;
                //visit cybercritics pressed
                if (mouseState.X >= screenSize.X - 100 && mouseState.X <= screenSize.X - 5 &&
                    mouseState.Y >= screenSize.Y - 15 && mouseState.Y <= screenSize.Y)
                    startProcess("http://sites.google.com/site/cybercritics/", "");
            }
            //mouse over
            else
            {
                foreach (DropBox box in sysObjects.systemIcons)
                    box.onMouseOver(new Vector2(mouseState.X, mouseState.Y));
            }
        }

        #region windows

        /// <summary>
        /// creates layout of login menu
        /// </summary>
        private void createLoginMenu()
        {
            Window menuLogin = new Window(blankTexture, new Vector2(300, 100), new Point(250, 150), tmpCol, font, "LOGIN");
            menuLogin.RaiseClearFocus += handleChangeFocus;
            menuLogin.Name = "menuLogin";

            Label lbUserName = new Label("lbUsername", "Username:", Color.Black, new Vector2(5, 45), font);
            menuLogin.addObject(lbUserName);
            TextBox tbUserName = new TextBox("tbUsername", new Vector2(65, 40), font, new Point(185, 20), tmpCol, blankTexture);
            
            tbUserName.Active = true;
            activeInputHolder = tbUserName;
            if (gameWorld.Username != null)
                tbUserName.Text = gameWorld.Username;

            menuLogin.addObject(tbUserName);

            Label lbPassword = new Label("lbPassword", "Password:", Color.Black, new Vector2(5, 70), font);
            menuLogin.addObject(lbPassword);
            TextBox tbPassword = new TextBox("tbPassword", new Vector2(65, 65), font, new Point(185, 20), tmpCol, blankTexture);
            tbPassword.Masked = true;
            menuLogin.addObject(tbPassword);

            Button btLogin = new Button("btLogin", "LOGIN", Color.Black, new Vector2(90, 100), new Point(75, 20), font, tmpCol, blankTexture);
            btLogin.RaiseButtonClick += handleButtonClick;
            menuLogin.addObject(btLogin);

            Button btRegisterOP = new Button("btRegisterOP", "REGISTER", Color.Black, new Vector2(170, 100), new Point(75, 20), font, tmpCol, blankTexture);
            btRegisterOP.RaiseButtonClick += handleButtonClick;
            menuLogin.addObject(btRegisterOP);

            Button btHelpOP = new Button("btHelpOP", "HELP", Color.Black, new Vector2(10, 100), new Point(75, 20), font, tmpCol, blankTexture);
            btHelpOP.RaiseButtonClick += handleButtonClick;
            menuLogin.addObject(btHelpOP);

            Label lbResponse = new Label("lbResponse", "Info: ", Color.Black, new Vector2(5, 130), font);
            menuLogin.addObject(lbResponse);

            menuLogin.Open = true;
            menuLogin.Focus = true;
            sysObjects.systemWindows.Add(menuLogin);
        }

        /// <summary>
        /// creates layout of regiter menu
        /// </summary>
        private void createRegisterMenu()
        {
            Window menuRegister = new Window(blankTexture, new Vector2(100, 100), new Point(250, 200), tmpCol, font, "REGISTER");
            menuRegister.RaiseClearFocus += handleChangeFocus;
            menuRegister.Name = "menuRegister";

            Label lbUserName = new Label("lbUsernameReg", "Username:", Color.Black, new Vector2(5, 45), font);
            menuRegister.addObject(lbUserName);
            TextBox tbUserName = new TextBox("tbUsernameReg", new Vector2(65, 40), font, new Point(185, 20), tmpCol, blankTexture);
            tbUserName.Active = true;
            menuRegister.addObject(tbUserName);

            Label lbEmail = new Label("lbEmailRg", "Email:", Color.Black, new Vector2(5, 70), font);
            menuRegister.addObject(lbEmail);
            TextBox tbEmail = new TextBox("tbEmailRg", new Vector2(65, 65), font, new Point(185, 20), tmpCol, blankTexture);
            menuRegister.addObject(tbEmail);

            Label lbPassword = new Label("lbPasswordRg", "Password:", Color.Black, new Vector2(5, 95), font);
            menuRegister.addObject(lbPassword);
            TextBox tbPassword = new TextBox("tbPasswordRg", new Vector2(65, 90), font, new Point(185, 20), tmpCol, blankTexture);
            tbPassword.Masked = true;
            menuRegister.addObject(tbPassword);

            Label lbPasswordCon = new Label("lbPasswordRgCon", "Password:", Color.Black, new Vector2(5, 120), font);
            menuRegister.addObject(lbPasswordCon);
            TextBox tbPasswordCon = new TextBox("tbPasswordRgCon", new Vector2(65, 115), font, new Point(185, 20), tmpCol, blankTexture);
            tbPasswordCon.Masked = true;
            menuRegister.addObject(tbPasswordCon);

            Button btRegister = new Button("btRegister", "REGISTER", Color.Black, new Vector2(170, 150), new Point(75, 20), font, tmpCol, blankTexture);
            btRegister.RaiseButtonClick += handleButtonClick;
            menuRegister.addObject(btRegister);

            Label lbResponse = new Label("lbResponseRg", "Info: ", Color.Black, new Vector2(5, 175), font);
            menuRegister.addObject(lbResponse);

            sysObjects.systemWindows.Add(menuRegister);
        }

        /// <summary>
        /// creates layout of regiter menu
        /// </summary>
        private void createHelpMenu()
        {
            Window menuHelp = new Window(blankTexture, new Vector2(100, 100), new Point(250, 200), tmpCol, font, "HELP");
            menuHelp.RaiseClearFocus += handleChangeFocus;
            menuHelp.Name = "menuHelp";

            Label lbUsernameHL = new Label("lbUsernameHL", "Username:", Color.Black, new Vector2(5, 45), font);
            menuHelp.addObject(lbUsernameHL);
            TextBox tbUserName = new TextBox("tbUsernameHL", new Vector2(65, 40), font, new Point(185, 20), tmpCol, blankTexture);
            tbUserName.Active = true;
            tbUserName.Text = gameWorld.Username;
            menuHelp.addObject(tbUserName);

            Label lbEmailHL = new Label("lbEmailHL", "Email:", Color.Black, new Vector2(5, 70), font);
            menuHelp.addObject(lbEmailHL);
            TextBox tbEmailHL = new TextBox("tbEmailHL", new Vector2(65, 65), font, new Point(185, 20), tmpCol, blankTexture);
            menuHelp.addObject(tbEmailHL);

            Label lbPasswordHL = new Label("lbPasswordHL", "Password:", Color.Black, new Vector2(5, 95), font);
            menuHelp.addObject(lbPasswordHL);
            TextBox tbPassword = new TextBox("tbPasswordHL", new Vector2(65, 90), font, new Point(185, 20), tmpCol, blankTexture);
            tbPassword.Masked = true;
            menuHelp.addObject(tbPassword);

            Label lbNewPasswordHL = new Label("lbNewPasswordHL", "New pass:", Color.Black, new Vector2(5, 120), font);
            menuHelp.addObject(lbNewPasswordHL);
            TextBox tbNewPasswordHL = new TextBox("tbNewPasswordHL", new Vector2(65, 115), font, new Point(185, 20), tmpCol, blankTexture);
            tbNewPasswordHL.Masked = true;
            menuHelp.addObject(tbNewPasswordHL);

            Button btRequestHL = new Button("btRequestHL", "REQUEST ACC INFO", Color.Black, new Vector2(5, 150), new Point(125, 20), font, tmpCol, blankTexture);
            btRequestHL.RaiseButtonClick += handleButtonClick;
            menuHelp.addObject(btRequestHL);

            Button btResetHL = new Button("btResetHL", "CHANGE PASSWD", Color.Black, new Vector2(140, 150), new Point(105, 20), font, tmpCol, blankTexture);
            btResetHL.RaiseButtonClick += handleButtonClick;
            menuHelp.addObject(btResetHL);

            Label lbResponse = new Label("lbResponseHL", "Info: ", Color.Black, new Vector2(5, 175), font);
            menuHelp.addObject(lbResponse);

            sysObjects.systemWindows.Add(menuHelp);
        }

        private void createHDDwin()
        {
            Window winHDD = new Window(blankTexture, new Vector2(screenSize.X - 155,screenSize.Y - 285), new Point(150, 205), tmpCol, font, "Local HDD");
            winHDD.Name = "winHDD";
            winHDD.RaiseClearFocus += handleChangeFocus;

            sysObjects.systemWindows.Add(winHDD);
        }

        private void createCPUwin()
        {
            Window winCPU = new Window(blankTexture, new Vector2(screenSize.X - 390, screenSize.Y - 180), new Point(220, 100), tmpCol, font, "CPU monitor");
            winCPU.Name = "winCPU";
            winCPU.RaiseClearFocus += handleChangeFocus;

            sysObjects.systemWindows.Add(winCPU);
        }

        private void createHelpWin()
        {
            Window winHelp = new Window(blankTexture, new Vector2(5, screenSize.Y - 110), new Point(280, 90), tmpCol, font, "Help");
            winHelp.Name = "winHelp";
            winHelp.RaiseClearFocus += handleChangeFocus;

            //icon box
            Vector2 position = new Vector2(10, 30);
            DropBox tmpBox = new DropBox("helpIcon", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(48, 48);
            tmpBox.HasBorder = true;

            winHelp.addObject(tmpBox);

            position = new Vector2(65, 30);
            Label tmpLab = new Label("helpInfo", "Click on item to get help", Color.Black, position, font);
            winHelp.addObject(tmpLab);

            sysObjects.systemWindows.Add(winHelp);
        }

        private void createMarketWin()
        {
            Window winMarket = new Window(blankTexture, new Vector2(5, 100), new Point(280, 250), tmpCol, font, "Software Market");
            winMarket.Name = "winMarket";
            winMarket.RaiseClearFocus += handleChangeFocus;

            Vector2 position = new Vector2(5, winMarket.Size.Y - 20);
            Label tmpLab = new Label("lbMarket", "info: to purchase drag item to empty hdd slot", Color.Black, position, font);
            tmpLab.TextBreak = 65;
            winMarket.addObject(tmpLab);

            sysObjects.systemWindows.Add(winMarket);
        }

        private void createSystemIcons()
        {
            //HDD button
            Vector2 position = new Vector2(this.screenSize.X - 65, this.screenSize.Y - 65);
            DropBox sysIcon = new DropBox("systemHDD", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(48, 48);
            sysIcon.SetNewItem("systemHDD", getTexture("hddblue"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //help button
            position = new Vector2(this.screenSize.X - 130, this.screenSize.Y - 65);
            sysIcon = new DropBox("systemHelp", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(48, 48);
            sysIcon.SetNewItem("systemHelp", getTexture("help"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //market button
            position = new Vector2(this.screenSize.X - 195, this.screenSize.Y - 65);
            sysIcon = new DropBox("systemMarket", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(48, 48);
            sysIcon.SetNewItem("systemMarket", getTexture("market"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //cash
            position = new Vector2(this.screenSize.X / 2 - 20, 5);
            sysIcon = new DropBox("systemCash", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(24, 24);
            sysIcon.SetNewItem("systemCash", getTexture("cash"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //gold
            position = new Vector2(this.screenSize.X / 2 - 100, 5);
            sysIcon = new DropBox("systemGold", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(24, 24);
            sysIcon.SetNewItem("systemGold", getTexture("gold"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //trash
            position = new Vector2(15, 15);
            sysIcon = new DropBox("systemTrash", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(48, 48);
            sysIcon.SetNewItem("systemTrash", getTexture("trash"));
            sysIcon.RaiseDropItem += handleDragDrop;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //tasks
            position = new Vector2(this.screenSize.X - 260, this.screenSize.Y - 65);
            sysIcon = new DropBox("systemTasks", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(48, 48);
            sysIcon.SetNewItem("systemTasks", getTexture("tasks"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //login small
            position = new Vector2(this.screenSize.X - 30, 5);
            sysIcon = new DropBox("systemLogin", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(24, 24);
            sysIcon.SetNewItem("systemLogin", getTexture("login"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //stats small
            position = new Vector2(this.screenSize.X - 60, 5);
            sysIcon = new DropBox("systemStats", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(24, 24);
            sysIcon.SetNewItem("systemStats", getTexture("stats"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //compiler
            position = new Vector2(this.screenSize.X - 325, this.screenSize.Y - 65);
            sysIcon = new DropBox("systemCompiler", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(48, 48);
            sysIcon.SetNewItem("systemCompiler", getTexture("compiler"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //hardware
            position = new Vector2(this.screenSize.X - 390, this.screenSize.Y - 65);
            sysIcon = new DropBox("systemHardware", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(48, 48);
            sysIcon.SetNewItem("systemHardware", getTexture("hardware"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //network
            position = new Vector2(this.screenSize.X - 520, this.screenSize.Y - 65);
            sysIcon = new DropBox("systemNetwork", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(48, 48);
            sysIcon.SetNewItem("systemNetwork", getTexture("network"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);

            //mission
            position = new Vector2(this.screenSize.X - 455, this.screenSize.Y - 65);
            sysIcon = new DropBox("systemMission", position, tmpCol, blankTexture);
            sysIcon.Size = new Point(48, 48);
            sysIcon.SetNewItem("systemMission", getTexture("mission"));
            sysIcon.RaiseButtonClick += handleButtonClick;
            sysIcon.HasBorder = false;
            sysObjects.systemIcons.Add(sysIcon);
        }

        private void createCompilerWin()
        {
            Window winCompiler = new Window(blankTexture, new Vector2(5, screenSize.Y - 215), new Point(280, 95), tmpCol, font, "Compiler");
            winCompiler.Name = "winCompiler";
            winCompiler.RaiseClearFocus += handleChangeFocus;

            //base box
            Vector2 position = new Vector2(10, 30);
            DropBox tmpBox = new DropBox("baseComProg", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(32, 32);
            tmpBox.HasBorder = true;
            tmpBox.Holding = "";
            tmpBox.RaiseDropItem += handleDragDrop;
            winCompiler.addObject(tmpBox);

            position = new Vector2(12, 65);
            Label tmpLab = new Label("labCompBase", "BASE", Color.Black, position, font);
            winCompiler.addObject(tmpLab);

            //v2.0 box
            position = new Vector2(50, 30);
            tmpBox = new DropBox("compV2", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(32, 32);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("compV2", getTexture("time"));
            tmpBox.BackColor = Color.LimeGreen;
            tmpBox.RaiseButtonClick += handleButtonClick;
            winCompiler.addObject(tmpBox);

            position = new Vector2(52, 65);
            tmpLab = new Label("labCompV2", "v(2.0)", Color.Black, position, font);
            winCompiler.addObject(tmpLab);

            //v3.0 box
            position = new Vector2(90, 30);
            tmpBox = new DropBox("compV3", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(32, 32);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("compV3", getTexture("cash"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winCompiler.addObject(tmpBox);

            position = new Vector2(92, 65);
            tmpLab = new Label("labCompV3", "v(3.0)\n $500", Color.Black, position, font);
            winCompiler.addObject(tmpLab);

            //v4.0 box
            position = new Vector2(130, 30);
            tmpBox = new DropBox("compV4", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(32, 32);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("compV4", getTexture("hacker"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winCompiler.addObject(tmpBox);

            position = new Vector2(132, 65);
            tmpLab = new Label("labCompV4", "v(4.0)\n$1000", Color.Black, position, font);
            winCompiler.addObject(tmpLab);

            //v5.0 box
            position = new Vector2(170, 30);
            tmpBox = new DropBox("compV5", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(32, 32);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("compV5", getTexture("gold"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winCompiler.addObject(tmpBox);

            position = new Vector2(172, 65);
            tmpLab = new Label("labCompV5", "v(5.0)\n  [1]", Color.Black, position, font);
            winCompiler.addObject(tmpLab);

            //create start/claim button
            Button btTmp = new Button("btCompiler", "START", Color.Black, new Vector2(210, 70), new Point(65, 20), font, tmpCol, blankTexture);
            btTmp.RaiseButtonClick += handleButtonClick;
            winCompiler.addObject(btTmp);

            //info
            position = new Vector2(210, 27);
            tmpLab = new Label("labCompInfo", "NA", Color.Black, position, font);
            winCompiler.addObject(tmpLab);

            sysObjects.systemWindows.Add(winCompiler);
        }

        private void createHardwareWin()
        {
            Window winHardware = new Window(blankTexture, new Vector2(screenSize.X - 390, screenSize.Y - 285), new Point(220, 100), tmpCol, font, "Hardware Market");
            winHardware.Name = "winHardware";
            winHardware.RaiseClearFocus += handleChangeFocus;

            //buy cpu slots
            Vector2 position = new Vector2(5, 30);
            DropBox tmpBox = new DropBox("hardCPU", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(32, 32);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("hardCPU", getTexture("memory"));
            winHardware.addObject(tmpBox);

            position = new Vector2(45, 30);
            Label tmpLab = new Label("labHardCPU", "NA", Color.Black, position, font);
            winHardware.addObject(tmpLab);

            //buy button
            Button btTmp = new Button("btHardCPU", "BUY", Color.Black, new Vector2(5, 70), new Point(80, 20), font, tmpCol, blankTexture);
            btTmp.RaiseButtonClick += handleButtonClick;
            winHardware.addObject(btTmp);

            //buy hdd slots
            position = new Vector2(115, 30);
            tmpBox = new DropBox("hardHDD", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(32, 32);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("hardHDD", getTexture("addhdd"));
            winHardware.addObject(tmpBox);

            position = new Vector2(155, 30);
            tmpLab = new Label("labHardHDD", "NA", Color.Black, position, font);
            winHardware.addObject(tmpLab);

            //buy button
            btTmp = new Button("btHardHDD", "BUY", Color.Black, new Vector2(115, 70), new Point(80, 20), font, tmpCol, blankTexture);
            btTmp.RaiseButtonClick += handleButtonClick;
            winHardware.addObject(btTmp);

            sysObjects.systemWindows.Add(winHardware);
        }

        private void createMissionWin()
        {
            Window winMission = new Window(blankTexture, new Vector2(screenSize.X - 390, screenSize.Y - 470), new Point(220, 180), tmpCol, font, "Missions");
            winMission.Name = "winMission";
            winMission.RaiseClearFocus += handleChangeFocus;

            //white hat
            Vector2 position = new Vector2(winMission.Size.X - 30, 25);
            DropBox tmpBox = new DropBox("missWhite", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(24, 24);
            tmpBox.HasBorder = true;
            tmpBox.BackColor = Color.Green;
            tmpBox.SetNewItem("missWhite", getTexture("whiteHat"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winMission.addObject(tmpBox);

            //black hat
            position = new Vector2(winMission.Size.X - 30, 55);
            tmpBox = new DropBox("missBlack", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(24, 24);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("missBlack", getTexture("blackHat"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winMission.addObject(tmpBox);

            //back
            position = new Vector2(5, winMission.Size.Y - 20);
            tmpBox = new DropBox("missBack", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(16, 16);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("missBack", getTexture("back"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winMission.addObject(tmpBox);

            //forward
            position = new Vector2(winMission.Size.X - 21, winMission.Size.Y - 20);
            tmpBox = new DropBox("missForward", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(16, 16);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("missForward", getTexture("forward"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winMission.addObject(tmpBox);

            //target program
            position = new Vector2(5, 25);
            tmpBox = new DropBox("missProg", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(39, 39);
            tmpBox.HasBorder = true;
            tmpBox.RaiseButtonClick += handleButtonClick;
            winMission.addObject(tmpBox);

            //accept/drop button
            Button btTmp = new Button("btMissOK", "ACCEPT", Color.Black, new Vector2(winMission.Size.X / 2 - 33, winMission.Size.Y - 21), new Point(65, 18), font, tmpCol, blankTexture);
            btTmp.RaiseButtonClick += handleButtonClick;
            winMission.addObject(btTmp);

            //info label
            position = new Vector2(50, 25);
            Label tmpLab = new Label("labMissInfo", "NA", Color.Black, position, font);
            tmpLab.TextBreak = 25;
            winMission.addObject(tmpLab);

            //income, hat label and others
            position = new Vector2(5, 70);
            tmpLab = new Label("labMissIcHt", "NA", Color.Black, position, font);
            winMission.addObject(tmpLab);

            sysObjects.systemWindows.Add(winMission);
        }

        private void createNetworkWin()
        {
            Window winNetwork = new Window(blankTexture, new Vector2(297, 35), new Point(325, 470), tmpCol, font, "Attack Vector");
            winNetwork.Name = "winNetwork";
            winNetwork.RaiseClearFocus += handleChangeFocus;

            //slaves window
            Window slaves = new Window(blankTexture, new Vector2(312, 65), new Point(295, 320), tmpCol, font, "Slaves");
            slaves.Name = "winNetSlaves";
            slaves.Open = true;
            slaves.Slave = true;
            slaves.RaiseCustomDraw += handleCustomDraw;

            for (int c = 0; c < 4; c++)
            {
                //target slots
                int spacing2 = 5;
                for (int i = 0; i < 6; i++, spacing2 += 30)
                {
                    //seperate user from admin
                    if (i == 3)
                        spacing2 += 25;

                    Vector2 positionB = new Vector2(spacing2, 25 + (c * 75));
                    DropBox tmpBoxB = new DropBox("slaveSlot" + c.ToString() + "." + i.ToString(), positionB, tmpCol, blankTexture);
                    tmpBoxB.Size = new Point(24, 24);
                    tmpBoxB.HasBorder = true;
                    tmpBoxB.SetNewItem("unknown", getTexture("unknown"));
                    tmpBoxB.RaiseDropItem += handleDragDrop;
                    tmpBoxB.RaiseDragDown += handleDragDown;
                    tmpBoxB.Visible = false;
                    slaves.addObject(tmpBoxB);

                    positionB = new Vector2(spacing2, 50 + (75 * c));
                    Label tmpLabC = new Label("slaveVer" + c.ToString() + "." + i.ToString(), "  (?)", Color.Black, positionB, font);
                    tmpLabC.TextBreak = 16;
                    tmpLabC.Visible = false;
                    slaves.addObject(tmpLabC);
                }

                //user password
                Vector2 position2 = new Vector2(95, 33 + (75 * c));
                DropBox tmpBox2 = new DropBox("slaveUser" + c.ToString(), position2, tmpCol, blankTexture);
                tmpBox2.Size = new Point(16, 16);
                tmpBox2.HasBorder = true;
                tmpBox2.SetNewItem("noPass", getTexture("noPass"));
                tmpBox2.Visible = false;
                slaves.addObject(tmpBox2);

                //admin password
                position2 = new Vector2(210, 33 + (75 * c));
                tmpBox2 = new DropBox("slaveAdmin" + c.ToString(), position2, tmpCol, blankTexture);
                tmpBox2.Size = new Point(16, 16);
                tmpBox2.HasBorder = true;
                tmpBox2.SetNewItem("noPass", getTexture("noPass"));
                tmpBox2.Visible = false;
                slaves.addObject(tmpBox2);

                //drop button
                Button btTmp = new Button("btSlaveDrop" + c.ToString(), "DROP", Color.Black, new Vector2(slaves.Size.X - 65, 70 + (75 * c)), new Point(60, 18), font, tmpCol, blankTexture);
                btTmp.RaiseButtonClick += handleButtonClick;
                btTmp.Visible = false;
                slaves.addObject(btTmp);

                position2 = new Vector2(10, 75 + (75 * c));
                Label tmpLab = new Label("labSlaveTime" + c.ToString(), "time until detection: ", Color.Black, position2, font);
                tmpLab.TextBreak = 100;
                tmpLab.Visible = false;
                slaves.addObject(tmpLab);
            }

            winNetwork.addObject(slaves);
            //end of slaves win

            //target window
            Window target = new Window(blankTexture, new Vector2(312, 395), new Point(295, 100), tmpCol, font, "Client/Target");
            target.Name = "winNetTarget";
            target.Open = true;
            target.Slave = true;
            
            //immediate
            Vector2 position = new Vector2(15, 30);
            DropBox tmpBox = new DropBox("targImm", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(48, 48);
            tmpBox.HasBorder = true;
            tmpBox.RaiseButtonClick += handleButtonClick;
            tmpBox.RaiseDropItem += handleDragDrop;
            target.addObject(tmpBox);

            //progress
            position = new Vector2(15, 85);
            ProgressBar tmpBar = new ProgressBar("targProg", position, new Vector2(48,7), tmpCol, Color.LightGreen, blankTexture);
            tmpBar.HasBorder = true;
            tmpBar.setValues(300, 0);
            target.addObject(tmpBar);

            //target slots
            int spacing = 75;
            for (int i = 0; i < 6; i++, spacing += 30)
            {
                //seperate user from admin
                if (i == 3)
                    spacing += 25;

                position = new Vector2(spacing, 30);
                tmpBox = new DropBox("targSlot" + i.ToString(), position, tmpCol, blankTexture);
                tmpBox.Size = new Point(24, 24);
                tmpBox.HasBorder = true;
                tmpBox.SetNewItem("unknown", getTexture("unknown"));
                tmpBox.RaiseDropItem += handleDragDrop;
                tmpBox.RaiseDragDown += handleDragDown;
                target.addObject(tmpBox);

                position = new Vector2(spacing, 55);
                Label tmpLab = new Label("targVer" + i.ToString(),"  (?)", Color.Black, position, font);
                tmpLab.TextBreak = 16;
                target.addObject(tmpLab);
            }

            //user password
            position = new Vector2(75, 76);
            tmpBox = new DropBox("targUser", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(16, 16);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("targUser", getTexture("noPass"));
            target.addObject(tmpBox);

            position = new Vector2(95, 76);
            Label tmpLabB = new Label("labTargUPS", "usr psw lvl: ", Color.Black, position, font);
            tmpLabB.TextBreak = 25;
            target.addObject(tmpLabB);

            //admin password
            position = new Vector2(190, 76);
            tmpBox = new DropBox("targAdmin", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(16, 16);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("targAdmin", getTexture("noPass"));
            target.addObject(tmpBox);

            position = new Vector2(210, 76);
            tmpLabB = new Label("labTargAPS", "adm psw lvl: ", Color.Black, position, font);
            tmpLabB.TextBreak = 25;
            target.addObject(tmpLabB);

            winNetwork.addObject(target);

            sysObjects.systemWindows.Add(winNetwork);
        }

        private void createStatsWin()
        {
            Window winStats = new Window(blankTexture, new Vector2(screenSize.X - 155, 50), new Point(150, 235), tmpCol, font, "Server Stats");
            winStats.Name = "winStats";
            winStats.RaiseClearFocus += handleChangeFocus;
            
            //white hat
            Vector2 position = new Vector2(35, winStats.Size.Y - 30);
            DropBox tmpBox = new DropBox("statWhite", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(24, 24);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("statWhite", getTexture("whiteHat"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winStats.addObject(tmpBox);

            //black hat
            position = new Vector2(65, winStats.Size.Y - 30);
            tmpBox = new DropBox("statBlack", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(24, 24);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("statBlack", getTexture("blackHat"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winStats.addObject(tmpBox);

            //cash ico
            position = new Vector2(95, winStats.Size.Y - 30);
            tmpBox = new DropBox("statCash", position, tmpCol, blankTexture);
            tmpBox.Size = new Point(24, 24);
            tmpBox.HasBorder = true;
            tmpBox.SetNewItem("statCash", getTexture("cash"));
            tmpBox.RaiseButtonClick += handleButtonClick;
            winStats.addObject(tmpBox);

            //label totUsers
            position = new Vector2(5, 25);
            Label tmpLab = new Label("labUserStat", "Total Users: ", Color.Black, position, font);
            tmpLab.TextBreak = 25;
            winStats.addObject(tmpLab);

            //label top10 static
            position = new Vector2(55, 45);
            tmpLab = new Label("labStatStatic", "TOP 10", Color.Black, position, font);
            tmpLab.TextBreak = 25;
            winStats.addObject(tmpLab);

            //label top10 name
            position = new Vector2(5, 45);
            tmpLab = new Label("labStatTop10n", "", Color.Black, position, font);
            tmpLab.TextBreak = 25;
            winStats.addObject(tmpLab);

            //label top10 points
            position = new Vector2(winStats.Size.X - 65, 45);
            tmpLab = new Label("labStatTop10a", "", Color.Black, position, font);
            tmpLab.TextBreak = 25;
            winStats.addObject(tmpLab);

            sysObjects.systemWindows.Add(winStats);
        }

        private void createDonationsWin()
        {
            Window winDonations = new Window(blankTexture, new Vector2(screenSize.X - 390, 5), new Point(220, 95), tmpCol, font, "Donations");
            winDonations.Name = "winDonations";
            winDonations.RaiseClearFocus += handleChangeFocus;

            //label totUsers
            string message = "For any donation your gold will be set to 150,\nproceeds will be used for bandwidth and\neventually a dedicated server.";
            Vector2 position = new Vector2(5, 25);
            Label tmpLab = new Label("labDonInfo", message, Color.Black, position, font);
            tmpLab.TextBreak = 50;
            winDonations.addObject(tmpLab);

            //paypal button
            Button btTmp = new Button("btDonPay", "PAYPAL", Color.Black, new Vector2(winDonations.Size.X / 2 - 70, winDonations.Size.Y - 21), new Point(65, 18), font, tmpCol, blankTexture);
            btTmp.RaiseButtonClick += handleButtonClick;
            winDonations.addObject(btTmp);

            //paypal button
            btTmp = new Button("btDonCan", "CANCEL", Color.Black, new Vector2(winDonations.Size.X / 2, winDonations.Size.Y - 21), new Point(65, 18), font, tmpCol, blankTexture);
            btTmp.RaiseButtonClick += handleButtonClick;
            winDonations.addObject(btTmp);

            sysObjects.systemWindows.Add(winDonations);
        }

        #endregion

        #region events

        /// <summary>
        /// focus has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleChangeFocus(object sender, SimpleEventArgs e)
        {
            foreach (Window win in sysObjects.systemWindows)
                win.Focus = false;

            //foreach (Window win in sysObjects.systemWindows)
            //    if (win.Moving)
            //        return;

            if (sender.GetType() == typeof(Window))
                updateHelpInfo("window", ((Window)sender).Name);
        }

        /// <summary>
        /// user clicked button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleButtonClick(object sender, SimpleEventArgs e)
        {
            //login user
            if (e.Message == "btLogin")
            {
                bufferSTR = "LOGIN";
                loginUser();
            }
            //open register menu
            else if (e.Message == "btRegisterOP")
            {
                bufferSTR = "REGISTER";
                getWindowByName("menuLogin").Open = false;
                getWindowByName("menuRegister").Open = true;
            }
            //register user
            else if (e.Message == "btRegister")
            {
                ((Button)sender).onMouseUp();
                registerUser();
            }
            //open help menu
            else if (e.Message == "btHelpOP")
            {
                getWindowByName("menuLogin").Open = false;
                getWindowByName("menuHelp").Open = true;
            }
            //request account info
            else if (e.Message == "btRequestHL")
            {
                requestAccountInfo();

                Window tmpWin = getWindowByName("winHelp");
                tmpWin.Open = false;
            }
            //reset password
            else if (e.Message == "btResetHL")
            {
                changePassword();
            }
            //toggle hddwin
            else if (e.Message == "systemHDD")
            {
                Window tmpWin = getWindowByName("winHDD");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }
            //toggle helpwin
            else if (e.Message == "systemHelp")
            {
                Window tmpWin = getWindowByName("winHelp");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }
            //toggle sysMarket
            else if (e.Message == "systemMarket")
            {
                Window tmpWin = getWindowByName("winMarket");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }
            //toggle sysTasksWin
            else if (e.Message == "systemTasks")
            {
                Window tmpWin = getWindowByName("winCPU");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }
            //toggle loginMenu
            else if (e.Message == "systemLogin")
            {
                Window tmpWin = getWindowByName("menuLogin");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }//toggle statsWin
            else if (e.Message == "systemStats")
            {
                Window tmpWin = getWindowByName("winStats");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }
            //toggle compilerWin
            else if (e.Message == "systemCompiler")
            {
                Window tmpWin = getWindowByName("winCompiler");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }//toggle hardwareWin
            else if (e.Message == "systemHardware")
            {
                Window tmpWin = getWindowByName("winHardware");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }//toggle networkWin
            else if (e.Message == "systemNetwork")
            {
                Window tmpWin = getWindowByName("winNetwork");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;

                getWindowByName("winNetSlaves").Open = true;
                getWindowByName("winNetTarget").Open = true;

            }//toggle missionWin
            else if (e.Message == "systemMission")
            {
                Window tmpWin = getWindowByName("winMission");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }
            //systemGold
            else if (e.Message == "systemGold" || e.Message == "btDonCan")
            {
                Window tmpWin = getWindowByName("winDonations");
                tmpWin.Open = !tmpWin.Open;
                handleChangeFocus(this, new SimpleEventArgs("none"));
                tmpWin.Focus = true;
            }
            //help message for hdd
            else if (e.Message.IndexOf("hddASlot") != -1)
            {
                Window tmpWin = getWindowByName("winHDD");
                DropBox tmpBox = (DropBox)tmpWin.getObjectByName(e.Message);
                updateHelpInfo(tmpBox.Holding, e.Message);
            }
            //help message for market
            else if (e.Message.IndexOf("market") != -1)
            {
                Window tmpWin = getWindowByName("winMarket");
                DropBox tmpBox = (DropBox)tmpWin.getObjectByName(e.Message);
                updateHelpInfo(tmpBox.Holding, e.Message);
            }
            //help message for cpu
            else if (e.Message.IndexOf("cpuASlot") != -1)
            {
                Window tmpWin = getWindowByName("winCPU");
                DropBox tmpBox = (DropBox)tmpWin.getObjectByName(e.Message);
                updateHelpInfo(tmpBox.Holding, e.Message);
            }
            //compiler version change
            else if (e.Message.IndexOf("compV") != -1)
            {
                if (gameWorld.UserCompiler.Active)
                {
                    bufferSTR = "error: job already active";
                    return;
                }
                
                Window tmpWin = getWindowByName("winCompiler");
                if (((DropBox)tmpWin.getObjectByName("baseComProg")).Holding == "")
                    return;

                DropBox tmpBox = (DropBox)tmpWin.getObjectByName(e.Message);
                gameWorld.UserCompiler.ProgramVersion = Convert.ToInt32(e.Message.Substring(5, 1));
                refreshCompilerVer();
            }
            else if (e.Message.IndexOf("btCompiler") != -1)
            {
                if (!gameWorld.UserCompiler.Active)
                    startCompilerJob();
                else
                    claimCompilerJob();
            }
            else if (e.Message == "btHardHDD")
            {
                try
                {
                    bufferSTR = svrClient.addHDDslot(gameWorld.SessionID);
                    if (bufferSTR.IndexOf("error") == -1)
                    {
                        updateUserInfo();
                        updateHDDwin();
                        updateHardwareWin();
                    }
                }
                catch { bufferSTR = "error communicating with server"; }
            }
            else if (e.Message == "btHardCPU")
            {
                try
                {
                    bufferSTR = svrClient.addCPUslot(gameWorld.SessionID);
                    if (bufferSTR.IndexOf("error") == -1)
                    {
                        updateUserInfo();
                        updateCPUwin();
                        updateHardwareWin();
                    }
                }
                catch { bufferSTR = "error communicating with server"; }
            }
            //handle mission hats
            else if (e.Message == "missWhite" || e.Message == "missBlack")
            {
                gameWorld.MissionHat = !gameWorld.MissionHat;
                if (gameWorld.MissionHat)
                {
                    DropBox tmpBox = (DropBox)getWindowByName("winMission").getObjectByName("missWhite");
                    tmpBox.BackColor = Color.Green;

                    tmpBox = (DropBox)getWindowByName("winMission").getObjectByName("missBlack");
                    tmpBox.BackColor = tmpCol;
                }
                else if (!gameWorld.MissionHat)
                {
                    DropBox tmpBox = (DropBox)getWindowByName("winMission").getObjectByName("missWhite");
                    tmpBox.BackColor = tmpCol;

                    tmpBox = (DropBox)getWindowByName("winMission").getObjectByName("missBlack");
                    tmpBox.BackColor = Color.Green;
                }

                //refresh missions here
                gameWorld.MissionInd = 0;
                updateMissionWin();
            }
            //Stat buttons
            else if (e.Message == "statWhite" || e.Message == "statBlack" || e.Message == "statCash")
            {
                if (e.Message == "statWhite")
                    gameWorld.StatInd = World.StatChoice.White;
                else if (e.Message == "statBlack")
                    gameWorld.StatInd = World.StatChoice.Black;
                else if (e.Message == "statCash")
                    gameWorld.StatInd = World.StatChoice.Cash;
                updateStatWin();
            }
            else if (e.Message == "missForward" || e.Message == "missBack")
            {
                //check for active mission
                if (gameWorld.MissionActive != World.MissionState.NoMission)
                    return;

                //goto next in line
                if (e.Message == "missForward")
                    gameWorld.MissionInd = getNextMissionByHat(gameWorld.MissionHat, true);
                else
                    gameWorld.MissionInd = getNextMissionByHat(gameWorld.MissionHat, false);
                updateMissionWin();
            }//mission accepted/canceled
            else if (e.Message == "btMissOK")
            {
                try
                {
                    if (gameWorld.MissionActive == World.MissionState.NoMission)
                    {
                        bufferSTR = svrClient.addMission(gameWorld.SessionID, gameWorld.UserMission);
                        if (bufferSTR.IndexOf("error") == -1)
                            gameWorld.MissionActive = World.MissionState.MissionActive;
                    }
                    else if (gameWorld.MissionActive == World.MissionState.MissionActive)
                    {
                        bufferSTR = svrClient.deleteMission(gameWorld.SessionID);
                        if (bufferSTR.IndexOf("error") == -1)
                            gameWorld.MissionActive = World.MissionState.NoMission;
                    }
                    else if (gameWorld.MissionActive == World.MissionState.MissionComplete)
                    {
                        bufferSTR = svrClient.claimMission(gameWorld.SessionID);
                        if (bufferSTR.IndexOf("error") == -1)
                            gameWorld.MissionActive = World.MissionState.NoMission;
                        refreshSlaveTime();
                    }

                    gameWorld.MissionInd = getNextMissionByHat(gameWorld.MissionHat, true);
                    updateMissionButton();
                    updateMissionWin();
                    updateNetworkWin();
                    updateUserInfo();

                    if (gameWorld.UserMission.MissionID == 0 && gameWorld.MissionActive == World.MissionState.MissionComplete)
                        bufferSTR = "info: upload viruses, backdoors and root-kits to extend slave time";
                }
                catch { bufferSTR = "error: unable to contact server"; }
            }
            else if (e.Message.Length >= 11 && e.Message.Substring(0, 11) == "btSlaveDrop")
            {
                int slaveID = Convert.ToInt32(e.Message.Remove(0, 11));

                try { bufferSTR = svrClient.deleteSlave(gameWorld.SessionID, slaveID); }
                catch
                {
                    bufferSTR = "error communicating with server";
                }

                updateNetworkWin();
            }
            //donation clicked
            else if (e.Message == "btDonPay")
            {
                Window tmpWin = getWindowByName("winDonations");
                tmpWin.Open = false;

                startProcess("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=76UZZHKWEQ8DG", "");
                try { bufferSTR = svrClient.claimDonation(gameWorld.SessionID); }
                catch { bufferSTR = "error: unable to communicate with server"; }

                updateUserInfo();
                bufferSTR = "thank you for your support, enjoy";
            }
            if(sender.GetType() == typeof (Button))
                ((Button)sender).onMouseUp();
        }

        /// <summary>
        /// user is dragging something
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void handleDragDown(object sender, SimpleEventArgs e)
        {
            activeDrag = sender;
        }

        private void handleDragDrop(object sender, SimpleEventArgs e)
        {
            if (activeDrag == null)
                return;
            if (activeDrag.GetType() == typeof(DropBox) && sender.GetType() == typeof (DropBox))
            {
                DropBox dragFrom = (DropBox)activeDrag;
                DropBox dragTo = (DropBox)sender;

                //buying item from software market
                if (dragFrom.Parent == "winMarket" && dragTo.Parent == "winHDD")
                {
                    if (dragTo.Holding != "")
                    {
                        bufferSTR = "error: hdd slot already full";
                        return;
                    }
                    //get program info
                    ProgramHW software = getProgramByName(dragFrom.Holding);
                    //parse hddASlot
                    software.HddSlot = Convert.ToInt32(dragTo.Name.Remove(0, 8));
                    
                    try
                    {
                        string tmp = svrClient.purchaseFromMarket(gameWorld.SessionID, software);
                        if (tmp.IndexOf("error") != -1)
                        {
                            bufferSTR = tmp;
                            return;
                        }
                        bufferSTR = tmp;
                    }
                    catch
                    {
                        bufferSTR = "error communicating with server";
                        return;
                    }

                    bool ok = true;
                    try
                    {
                        gameWorld.UserHDD = svrClient.getHDDInfo(gameWorld.SessionID);
                    }
                    catch { bufferSTR = "error communicating with server"; ok = false; }

                    updateHDDwin();
                    updateUserInfo();
                    if (ok)
                        bufferSTR = "info: transaction successful";
                }
                //swapping two programs on hdd
                else if (dragFrom.Parent == "winHDD" && dragTo.Parent == "winHDD")
                {
                    string res = "";
                    try
                    {
                        //hddASlot
                        res = svrClient.swapProgsHDD(gameWorld.SessionID, Convert.ToInt32(dragFrom.Name.Remove(0, 8)),
                                               Convert.ToInt32(dragTo.Name.Remove(0, 8)));
                    }
                    catch { bufferSTR = "error communicating with server"; }
                    updateHDDwin();
                    bufferSTR = res;
                }
                //trash item 
                else if (dragFrom.Parent == "winHDD" && dragTo.Name == "systemTrash")
                {
                    int slot = Convert.ToInt32(dragFrom.Name.Remove(0, 8));
                    if(!hasProgramAtSlot(slot))
                        return;
                    bool ok = true;
                    try
                    {
                        svrClient.deleteHDDItem(gameWorld.SessionID, slot);
                        //gameWorld.UserHDD = svrClient.getHDDInfo(gameWorld.SessionID);
                    }
                    catch { bufferSTR = "error communicating with server"; ok = false; }
                    updateHDDwin();
                    if (ok)
                        bufferSTR = "info: deleted program";
                }
                //HDD to CPU
                else if (dragFrom.Parent == "winHDD" && dragTo.Parent == "winCPU")
                {
                    int slotCPU = Convert.ToInt32(dragTo.Name.Remove(0, 8));
                    int slotHDD = Convert.ToInt32(dragFrom.Name.Remove(0, 8));
                    bool ok = true;
                    try
                    {
                        ProgramHW tmpProg = getProgramAtSlot(slotHDD);
                        if (tmpProg == null)
                        {
                            bufferSTR = "error: slot empty";
                            return;
                        }
                        tmpProg.ProgramName = getProgramName(tmpProg.ProgramType, tmpProg.ProgramSubType);
                        svrClient.addCPUprogram(gameWorld.SessionID, tmpProg, slotCPU);
                        //gameWorld.UserCPU = svrClient.getCPUInfo(gameWorld.SessionID);
                    }
                    catch { bufferSTR = "error communicating with server"; ok = false; }
                    updateHDDwin();
                    updateCPUwin();
                    updateHelpInfo(dragFrom.Holding, dragFrom.Name);
                    if(ok)
                        bufferSTR = "info: program started";
                }
                //trash cpu item 
                else if (dragFrom.Parent == "winCPU" && dragTo.Name == "systemTrash")
                {
                    int slot = Convert.ToInt32(dragFrom.Name.Remove(0, 8));
                    if (!hasProgramAtCPUSlot(slot))
                        return;
                    bool ok = true;
                    try
                    {
                        svrClient.deleteCPUItem(gameWorld.SessionID, slot);
                        //gameWorld.UserHDD = svrClient.getHDDInfo(gameWorld.SessionID);
                    }
                    catch { bufferSTR = "error communicating with server"; ok = false; }
                    updateCPUwin();
                    if (ok)
                        bufferSTR = "info: deactivated program";
                }
                //HDD to Compiler
                else if (dragFrom.Parent == "winHDD" && dragTo.Parent == "winCompiler")
                {
                    if (gameWorld.UserCompiler.Active)
                    {
                        bufferSTR = "error: job already active";
                        return;
                    }
                    int slotHDD = Convert.ToInt32(dragFrom.Name.Remove(0, 8));
                    ProgramHW tmpProg = getProgramAtSlot(slotHDD);
                    if (tmpProg == null)
                    {
                        bufferSTR = "error: slot empty";
                        return;
                    }
                    tmpProg.ProgramName = getProgramName(tmpProg.ProgramType, tmpProg.ProgramSubType);

                    dragTo.SetNewItem(tmpProg.ProgramName, getTexture(tmpProg.ProgramName));

                    //set compiler options
                    gameWorld.UserCompiler = new CompilerJob();
                    gameWorld.UserCompiler.ProgramType = tmpProg.ProgramType;
                    gameWorld.UserCompiler.ProgramSubType = tmpProg.ProgramSubType;
                    gameWorld.UserCompiler.ProgramVersion = 2;
                    gameWorld.UserCompiler.StartTime = DateTime.UtcNow.Ticks;
                    gameWorld.UserCompiler.EndTime = (TimeSpan.TicksPerMinute * 1) + gameWorld.UserCompiler.StartTime;

                    refreshCompilerVer();
                    updateCompilerWin();
                    updateHelpInfo(dragFrom.Holding, dragFrom.Name);
                    bufferSTR = "info: select version and confirm";
                }
                else if ((dragFrom.Parent == "winCPU" || dragFrom.Parent == "winNetSlaves") && dragTo.Name == "targImm")
                {
                    if (gameWorld.MissionActive == World.MissionState.NoMission)
                        return;

                    int slave = dragFrom.Parent == "winCPU" ? -1 : -2;
                    int slot = 0;
                    if (slave == -1)
                        slot = Convert.ToInt32(dragFrom.Name.Substring(8, dragFrom.Name.Length - 8));//"cpuASlot??"
                    else
                    {
                        slave = Convert.ToInt32(dragFrom.Name.Substring(9, 1));//slaveSlot?.?
                        slot = Convert.ToInt32(dragFrom.Name.Substring(11, 1));//slaveSlot?.?
                    }

                    if (slave == -1)
                        gameWorld.RunningProg = getProgramAtCPUSlot(slot);
                    else
                    {
                        Slave tmpS = getSlaveByID(slave);
                        SlaveSlot tmpSlot = getSlotByID(tmpS, slot);
                        gameWorld.RunningProg.CpuSlot = -1;
                        gameWorld.RunningProg.ProgramType = tmpSlot.ProgramGroup;
                        gameWorld.RunningProg.ProgramSubType = tmpSlot.ProgramSubGroup;
                        gameWorld.RunningProg.ProgramVersion = tmpSlot.ProgramVersion;
                    }
                    
                    dragTo.Holding = dragFrom.Holding;
                    dragTo.Texture = dragFrom.Texture;
                    gameWorld.TimerActive = true;
                }
                else if (dragFrom.Parent == "winNetTarget" && dragTo.Name == "systemTrash")
                {
                    if (gameWorld.MissionActive == World.MissionState.NoMission)
                        return;

                    int slave = 999;
                    int slot = Convert.ToInt32(dragFrom.Name.Substring(8, 1));//targSlot?
                    Slave tmpS = getSlaveByID(slave);
                    if (tmpS == null)
                        return;
                    SlaveSlot tmpSlot = getSlotByID(tmpS, slot);
                    if (tmpSlot == null)
                        return;

                    if(tmpS.SlaveID != 999)
                        return;
                    else if(slot < 3 && !tmpS.UserPass)  
                    {
                        bufferSTR = "info: you need user privlages to delete this file";
                        return;
                    }
                    else if (slot >= 3 && !tmpS.AdminPass)
                    {
                        bufferSTR = "info: you need admin privlages to delete this file";
                        return;
                    }

                    int maxDelete = getMaxProgramVer(1, 3);
                    if (tmpSlot.ProgramVersion <= maxDelete)
                    {
                        Slave missionSlave = getSlaveByID(999);
                        for (int i = 0; i < missionSlave.SlaveFiles.Count(); i++)
                        {
                            SlaveSlot slotA = missionSlave.SlaveFiles[i];
                            if (slotA.SlotID == slot)
                                missionSlave.SlaveFiles.RemoveAt(i);
                        }

                        try { svrClient.setSlave(gameWorld.SessionID, missionSlave); }
                        catch
                        {
                            bufferSTR = "error communicating with server";
                            return;
                        }
                        updateNetworkWin();
                    }
                    else
                    {
                        bufferSTR = "info: deleter version too low to delete this file";
                    }

                    //check mission end
                    if (validateMissionEnd(gameWorld.UserMission, gameWorld.SlavesList))
                    {
                        gameWorld.MissionActive = World.MissionState.MissionComplete;
                        updateMissionButton();
                    }
                }
                //upload from HDD to target
                else if (dragFrom.Parent == "winHDD" && dragTo.Parent == "winNetTarget")
                {
                    if (gameWorld.MissionActive == World.MissionState.NoMission)
                        return;
                    else if (dragTo.Name == "targImm")
                    {
                        bufferSTR = "info: you can only run programs on target from CPU or Slaves";
                        return;
                    }

                    int slave = 999;
                    int slot = Convert.ToInt32(dragTo.Name.Substring(8, 1));//targSlot?
                    Slave tmpS = getSlaveByID(slave);
                    SlaveSlot tmpSlot = getSlotByID(tmpS, slot);

                    if (tmpSlot != null)
                    {
                        bufferSTR = "info: slot already full, delete file first";
                        return;
                    }

                    ProgramHW prog = getProgramAtSlot(Convert.ToInt32(dragFrom.Name.Remove(0, 8)));

                    if (tmpS.SlaveID != 999)
                        return;
                    else if (slot < 3 && !tmpS.UserPass)
                    {
                        bufferSTR = "info: you need user privlages to upload to this slot";
                        return;
                    }
                    else if (slot >= 3 && !tmpS.AdminPass)
                    {
                        bufferSTR = "info: you need admin privlages to upload to this slot";
                        return;
                    }
                    else if (getMaxProgramVer(1, 1) < prog.ProgramVersion)
                    {
                        bufferSTR = "info: uploader version too low for upload";
                        return;
                    }

                    //create slot
                    SlaveSlot newSlot = new SlaveSlot();
                    newSlot.ProgramGroup = prog.ProgramType;
                    newSlot.ProgramSubGroup = prog.ProgramSubType;
                    newSlot.ProgramVersion = prog.ProgramVersion;
                    newSlot.SlaveID = 999;
                    newSlot.SlotID = slot;

                    //add new slot
                    tmpS.SlaveFiles.Add(newSlot);
                    try { svrClient.setSlave(gameWorld.SessionID, tmpS); }
                    catch
                    {
                        bufferSTR = "error communicating with server";
                        return;
                    }

                    //decrement uses
                    try { bufferSTR = svrClient.decrementHDDuses(gameWorld.SessionID, prog); }
                    catch
                    {
                        bufferSTR = "error communicating with server";
                        return;
                    }

                    bufferSTR = "info: uploaded program";

                    //check mission end
                    if (validateMissionEnd(gameWorld.UserMission, gameWorld.SlavesList))
                    {
                        gameWorld.MissionActive = World.MissionState.MissionComplete;
                        updateMissionButton();
                    }

                    updateHDDwin();
                    updateNetworkWin();
                }
                //upload from Slave to target
                else if (dragFrom.Parent == "winNetSlaves" && dragTo.Parent == "winNetTarget")
                {
                    try
                    {
                        Slave parentSlave = getSlaveByID(Convert.ToInt32(dragFrom.Name.Substring(9, 1)));
                        SlaveSlot parentSlot = getSlotByID(parentSlave, Convert.ToInt32(dragFrom.Name.Substring(11, 1)));//"slaveSlot" + c.ToString() + "." + i.ToString()

                        if (parentSlave == null || parentSlot == null)
                            return;

                        //get target slot
                        int slave = 999;
                        int slot = Convert.ToInt32(dragTo.Name.Substring(8, 1));//targSlot?
                        Slave tmpS = getSlaveByID(slave);
                        SlaveSlot tmpSlot = getSlotByID(tmpS, slot);

                        if (tmpSlot != null)
                        {
                            bufferSTR = "info: slot already full, delete file first";
                            return;
                        }

                        if (tmpS.SlaveID != 999)
                            return;
                        else if (slot < 3 && !tmpS.UserPass)
                        {
                            bufferSTR = "info: you need user privlages to upload to this slot";
                            return;
                        }
                        else if (slot >= 3 && !tmpS.AdminPass)
                        {
                            bufferSTR = "info: you need admin privlages to upload to this slot";
                            return;
                        }
                        else if (getMaxProgramVer(1, 1) < parentSlot.ProgramVersion)
                        {
                            bufferSTR = "info: uploader version too low for upload";
                            return;
                        }

                        //create slot
                        SlaveSlot newSlot = new SlaveSlot();
                        newSlot.ProgramGroup = parentSlot.ProgramGroup;
                        newSlot.ProgramSubGroup = parentSlot.ProgramSubGroup;
                        newSlot.ProgramVersion = parentSlot.ProgramVersion;
                        newSlot.SlaveID = 999;
                        newSlot.SlotID = slot;

                        //add new slot
                        tmpS.SlaveFiles.Add(newSlot);
                        try { svrClient.setSlave(gameWorld.SessionID, tmpS); }
                        catch
                        {
                            bufferSTR = "error communicating with server";
                            return;
                        }

                        //remove slot from parent
                        for (int i = 0; i < parentSlave.SlaveFiles.Count; i++)
                        {
                            if (parentSlave.SlaveFiles[i].SlotID == parentSlot.SlotID)
                            {
                                parentSlave.SlaveFiles.RemoveAt(i);
                                break;
                            }
                        }

                        try { bufferSTR = svrClient.setSlave(gameWorld.SessionID, parentSlave); }
                        catch
                        {
                            bufferSTR = "error communicating with server";
                            return;
                        }

                        bufferSTR = "info: uploaded program";

                        //check mission end
                        if (validateMissionEnd(gameWorld.UserMission, gameWorld.SlavesList))
                        {
                            gameWorld.MissionActive = World.MissionState.MissionComplete;
                            updateMissionButton();
                        }

                        updateHDDwin();
                        updateNetworkWin();
                    }
                    catch { bufferSTR = "error: exception, try action again"; }
                }
            }
            
            activeDrag = null;
        }

        private void handleCustomDraw(object sender, CustomDrawEventArgs e)
        {
            if (e.Who == "winNetSlaves")
            {
                for (int i = 1; i < 4; i++)
                    drawLine(new Vector2(e.Position.X, e.Position.Y + (75 * i) + 20), new Vector2(e.Position.X + 295, e.Position.Y + (75 * i) + 20), Color.Black);
            }
        }

        #endregion

        private void loadAllData()
        {
            //get file types
            try
            {
                gameWorld.FileTypes = svrClient.getProgramTypes(gameWorld.SessionID);
                if (gameWorld.FileTypes == null)
                {
                    bufferSTR = "error: server error, can not get filetypes";
                    return;
                }
            }
            catch 
            {
                bufferSTR = "error: server seems offline";
                return;
            }

            //get HDDinfo
            try
            {
                gameWorld.UserHDD = svrClient.getHDDInfo(gameWorld.SessionID);
                if (gameWorld.UserHDD == null)
                {
                    bufferSTR = "error: no HDD info on server";
                    return;
                }
                else
                    bufferSTR = "info: got HDD filelist";
            }
            catch
            {
                bufferSTR = "error: failed to retrieve HDD info";
                return;
            }

            //get userInfo
            try
            {
                gameWorld.UserInfo = svrClient.getUserInfo(gameWorld.SessionID);
                if (gameWorld.UserInfo == null)
                {
                    bufferSTR = "error: no User info on server";
                    return;
                }
                else
                    bufferSTR = "info: got User info";
            }
            catch
            {
                bufferSTR = "error: failed to retrieve User info";
                return;
            }

            //get CPU info
            try
            {
                gameWorld.UserCPU = svrClient.getCPUInfo(gameWorld.SessionID);
            }
            catch
            {
                if (gameWorld.UserInfo == null)
                {
                    bufferSTR = "error: no CPU load info on server";
                    return;
                }
                else
                    bufferSTR = "info: got CPU load info";
            }

            //get Mission Types List
            try
            {
                gameWorld.MissionTypes = svrClient.getMissionList(gameWorld.SessionID);
            }
            catch
            {
                if (gameWorld.MissionTypes == null)
                {
                    bufferSTR = "error: could not get mission types list";
                    return;
                }
                else
                    bufferSTR = "info: got Mission Types info";
            }

            //get UserStats
            try
            {
                gameWorld.Top10Stats = svrClient.getStats(gameWorld.SessionID);
            }
            catch
            {
                if (gameWorld.Top10Stats == null)
                {
                    bufferSTR = "error: could not get stat list";
                    return;
                }
                else
                    bufferSTR = "info: got Stat List";
            }

            updateHDDwin();
            updateCPUwin();
            updateMarketWin();
            updateCompilerWin();
            updateHardwareWin();
            updateMissionWin();
            updateNetworkWin();
            updateStatWin();

            if (validateMissionEnd(gameWorld.UserMission, gameWorld.SlavesList))
                gameWorld.MissionActive = World.MissionState.MissionComplete;

            updateMissionButton();
        }

        #region Update

        private void updateUserInfo()
        {
            //get userInfo
            try
            {
                gameWorld.UserInfo = svrClient.getUserInfo(gameWorld.SessionID);
                if (gameWorld.UserInfo == null)
                {
                    bufferSTR = "error: no User info on server";
                    return;
                }
                else
                    bufferSTR = "info: got User info";
            }
            catch
            {
                bufferSTR = "error: failed to retrieve User info";
                return;
            }
        }

        private void updateHDDwin()
        {
            getWindowByName("winHDD").dumpObjects();
            try { gameWorld.UserHDD = svrClient.getHDDInfo(gameWorld.SessionID); }
            catch { bufferSTR = "error: unable to get HDD info"; };

            for (int i = 0; i < gameWorld.UserHDD.DriveSize; i++)
            {
                Vector2 position = new Vector2(16 + (i % 4) * 32, 32 + (i / 4) * 32);
                DropBox hddSlot = new DropBox("hddASlot" + i.ToString(), position, tmpCol, blankTexture);
                hddSlot.RaiseDropItem += handleDragDrop;

                if (hasProgramAtSlot(i))
                {

                    string fileName = getProgramName(getProgramAtSlot(i).ProgramType, getProgramAtSlot(i).ProgramSubType);
                    hddSlot.SetNewItem(fileName, getTexture(fileName));
                    hddSlot.RaiseDragDown += handleDragDown;
                    hddSlot.RaiseButtonClick += handleButtonClick;
                }

                Window winHDD = getWindowByName("winHDD");
                winHDD.addObject(hddSlot);
            }

            /*DropBox hddSlotA = new DropBox("hddSlotA", new Vector2(16, 32), tmpCol, blankTexture);
            hddSlotA.SetNewItem("downloader", getProgramTexture("downloader"));
            hddSlotA.RaiseDragDown += handleDragDown;
            hddWin.addObject(hddSlotA);*/
        }

        private void updateCPUwin()
        {
            getWindowByName("winCPU").dumpObjects();
            try { gameWorld.UserCPU = svrClient.getCPUInfo(gameWorld.SessionID); }
            catch { bufferSTR = "error: unable to get CPU info"; };

            for (int i = 0; i < gameWorld.UserInfo.CpuSlots; i++)
            {
                Vector2 position = new Vector2(16 + (i % 6) * 32, 32 + (i / 6) * 32);
                DropBox cpuSlot = new DropBox("cpuASlot" + i.ToString(), position, tmpCol, blankTexture);
                cpuSlot.RaiseDropItem += handleDragDrop;
                cpuSlot.RaiseDragDown += handleDragDown;

                if (hasProgramAtCPUSlot(i))
                {
                    string fileName = getProgramName(getProgramAtCPUSlot(i).ProgramType, getProgramAtCPUSlot(i).ProgramSubType);
                    cpuSlot.SetNewItem(fileName, getTexture(fileName));
                    cpuSlot.RaiseDragDown += handleDragDown;
                    cpuSlot.RaiseButtonClick += handleButtonClick;
                }

                Window winCPU = getWindowByName("winCPU");
                winCPU.addObject(cpuSlot);
            }
        }

        private void updateMarketWin()
        {
            int end = gameWorld.FileTypes.ProgramTypesLst.Count();
            for (int i = 0; i < end; i++)
            {
                ProgramHW file = gameWorld.FileTypes.ProgramTypesLst[i];
                Vector2 position = new Vector2(16 + (i % 4) * 64, 32 + (i / 4) * 32);
                DropBox marketSlot = new DropBox("market" + i.ToString(), position, tmpCol, blankTexture);
                //marketSlot.RaiseDropItem += handleDragDown;
                position.X += 32;
                Label price = new Label("price" + i.ToString(), file.BasePrice.ToString("N0"), Color.Black, position, font);

                string fileName = file.ProgramName;
                marketSlot.SetNewItem(fileName, getTexture(fileName));
                marketSlot.RaiseDragDown += handleDragDown;
                marketSlot.RaiseButtonClick += handleButtonClick;

                Window winMarket = getWindowByName("winMarket");
                winMarket.addObject(marketSlot);
                winMarket.addObject(price);
            }
        }

        private void updateHelpInfo(string itemName, string sender)
        {
            Window winHelp = getWindowByName("winHelp");
            DropBox tmpBox = (DropBox)winHelp.getObjectByName("helpIcon");
            Label tmpLab = (Label)winHelp.getObjectByName("helpInfo");

            //handle hdd help
            if (sender.Length > 7 && sender.Substring(0,8) == "hddASlot")
            {
                //find filetype info
                foreach (ProgramHW fileType in gameWorld.FileTypes.ProgramTypesLst)
                    if (fileType.ProgramName.ToLower() == itemName.ToLower())
                    {
                        tmpLab.Text = fileType.ProgramName + "\n" + fileType.ProgramDescription + "\n";
                        tmpBox.SetNewItem(fileType.ProgramName, getTexture(fileType.ProgramName));

                        //more info
                        string slot = sender.Remove(0, 8);//hddASlot
                        ProgramHW file = getProgramAtSlot(Convert.ToInt32(slot));
                        if (file == null)
                            return;

                        tmpLab.Text = fileType.ProgramName + " v(" + file.ProgramVersion.ToString() + ".0)" +
                            " [" + (file.UsesLeft == -1 ? "~" : file.UsesLeft.ToString()) + "] " + "uses left" + "\n" + fileType.ProgramDescription;
                    }
            }
            //handle cpu help
            else if (sender.Length > 7 && sender.Substring(0, 8) == "cpuASlot")
            {
                //find filetype info
                foreach (ProgramHW fileType in gameWorld.FileTypes.ProgramTypesLst)
                    if (fileType.ProgramName.ToLower() == itemName.ToLower())
                    {
                        tmpLab.Text = fileType.ProgramName + "\n" + fileType.ProgramDescription + "\n";
                        tmpBox.SetNewItem(fileType.ProgramName, getTexture(fileType.ProgramName));

                        //more info
                        string slot = sender.Remove(0, 8);//cpuASlot
                        CPUslotHW file = getProgramAtCPUSlot(Convert.ToInt32(slot));
                        if (file == null)
                            return;

                        tmpLab.Text = fileType.ProgramName + " v(" + file.ProgramVersion.ToString() + ".0) " + fileType.ProgramDescription;
                    }
            }
            //handle market help
            else if (sender.Length > 5 && sender.Substring(0, 6) == "market")
            {
                string slot = sender.Remove(0, 6);//market
                ProgramHW file = gameWorld.FileTypes.ProgramTypesLst[Convert.ToInt32(slot)];
                tmpLab.Text = file.ProgramName + " v(" + file.ProgramVersion.ToString() + ".0)" +
                            " [5] " + "uses left" + "\n" + file.ProgramDescription;

                tmpBox.SetNewItem(file.ProgramName, getTexture(file.ProgramName));
            }
            //handle window help
            else if (itemName == "window")
            {
                if(sender == "winCompiler")
                {
                    tmpLab.Text = "Allows coding higher versions of selected program. Drag item from HDD to \"BASE\", select version and commit.";
                    tmpBox.SetNewItem("winCompiler", getTexture("compiler"));
                }
                else if (sender == "menuLogin")
                {
                    tmpLab.Text = "Login to HackerWorld server. Registration, password recovery is also provided.";
                    tmpBox.SetNewItem("winLogin", getTexture("login"));
                }
                else if (sender == "winMarket")
                {
                    tmpLab.Text = "Marketplace that provides basic software.";
                    tmpBox.SetNewItem("winMarket", getTexture("market"));
                }
                else if (sender == "winHDD")
                {
                    tmpLab.Text = "Local hard-drive used for storing software.";
                    tmpBox.SetNewItem("winHDD", getTexture("hddBlue"));
                }
                else if (sender == "winCPU")
                {
                    tmpLab.Text = "Currently running programs are listed here, you have to be running a program to use it. Drag software from HDD to start it.";
                    tmpBox.SetNewItem("winCPU", getTexture("tasks"));
                }
                else if (sender == "winHardware")
                {
                    tmpLab.Text = "You can purchase new HDD and CPU slots\nhere.";
                    tmpBox.SetNewItem("winHardware", getTexture("hardware"));
                }
                else if (sender == "winNetwork")
                {
                    tmpLab.Text = "Drag programs from CPU window \nor use your slaves to accomplish mission hint: service scan, break pass,exploit\nextend slave time with malicious software";
                    tmpBox.SetNewItem("winNetwork", getTexture("network"));
                }
                else if (sender == "winMission")
                {
                    tmpLab.Text = "Choose your alignment, find and commit to a mission.";
                    tmpBox.SetNewItem("winMission", getTexture("mission"));
                }
            }
        }

        private void updateCompilerWin()
        {
            try 
            { 
                CompilerJob job = svrClient.getCompilerJob(gameWorld.SessionID);
                if (job != null)
                    gameWorld.UserCompiler = job;
                else
                    bufferSTR = "info: no compiler job on server";
            }
            catch { bufferSTR = "error: unable to get compiler job"; }
            refreshCompilerVer();
        }

        private void updateHardwareWin()
        {
            Window winCPU = getWindowByName("winHardware");
            
            Label labCPU = (Label)winCPU.getObjectByName("labHardCPU");
            int cpuSlots = gameWorld.UserCPU.TotalCPUslots;
            int cpuCost = (cpuSlots / 4 - 1) * 1000 + (cpuSlots % 4 + 1) * 250;
            int cpuGold = cpuSlots >= 8 ? 1 : 0;

            labCPU.Text = "$" + cpuCost.ToString("N0") + " [" + cpuGold.ToString() + "]\n";
            labCPU.Text += "cpu slot " + cpuSlots.ToString();

            Label labHDD = (Label)winCPU.getObjectByName("labHardHDD");
            int hddSlots = gameWorld.UserHDD.DriveSize;
            int hddCost = (hddSlots / 4 - 1) * 1000 + (hddSlots % 4) * 250;
            int hddGold = hddSlots >= 16 ? 1 : 0;

            labHDD.Text = "$" + hddCost.ToString("N0") + " [" + hddGold.ToString() + "]\n";
            labHDD.Text += "hdd slot " + hddSlots.ToString();
        }

        private void updateMissionWin()
        {
            Window winMiss = getWindowByName("winMission");
            Label labInfo = (Label)winMiss.getObjectByName("labMissInfo");
            Label labIcHt = (Label)winMiss.getObjectByName("labMissIcHt");
            DropBox boxProg = (DropBox)winMiss.getObjectByName("missProg");

            MissionType mission = getMissionByID(gameWorld.MissionInd);
            UserMission userMission = null;
            try
            {
                userMission = svrClient.getUserMission(gameWorld.SessionID);
            }
            catch
            {
                bufferSTR = "error communicating with server";
                return;
            }

            //setup userMission, no active mission
            if (userMission == null)
            {
                gameWorld.MissionActive = World.MissionState.NoMission;
                updateMissionButton();

                if (mission == null)
                    return;

                userMission = gameWorld.createUserMission(mission.MissionID);
            }
            else//mission already there
            {
                mission = getMissionByID(userMission.MissionID);
                gameWorld.MissionActive = World.MissionState.MissionActive;
                updateMissionButton();
            }

            string desc = mission.Description;
            string progName = getProgramName(userMission.ProgramGroup, userMission.ProgramSubGroup);
            desc = desc.Replace("%1", progName);
            desc = desc.Replace("%2", "v(" + userMission.ProgramVersion.ToString("N1") + ")");
            labInfo.Text = desc;

            int income = mission.MissionPay;
            income += userMission.PassStrength * 75;
            income += userMission.ProgramVersion * 100;
            income += userMission.NeedAdmin ? 0 : 250;

            labIcHt.Text = "Income $" + income.ToString("N0");
            labIcHt.Text += "\nHat points " + mission.HatPoints.ToString("N0");
            labIcHt.Text += "\nPassword strength: " + userMission.PassStrength.ToString("N0");
            labIcHt.Text += "\nNeed admin: " + userMission.NeedAdmin.ToString();
            labIcHt.Text += "\nHave admin: " + userMission.HaveAdmin.ToString();
            labIcHt.Text += "\nHave password: " + userMission.HavePass.ToString();

            boxProg.SetNewItem("miss" + progName, getTexture(progName));

            gameWorld.UserMission = userMission;
        }

        private void updateNetworkWin()
        {
            //get slaves
            try
            {
                gameWorld.SlavesList = svrClient.getSlaves(gameWorld.SessionID);
                if (gameWorld.SlavesList == null)
                {
                    bufferSTR = "error: invalid slaves list";
                    return;
                }
            }
            catch { bufferSTR = "error communicating with server"; }

            refreshSlaveTime();

            //populate mission slave
            Slave missionSlave = getSlaveByID(999);
            if (missionSlave != null && gameWorld.MissionActive != World.MissionState.NoMission)
            {
                //populate mission slave slots
                Window tmpWin = (Window)getWindowByName("winNetwork").getObjectByName("winNetTarget");
                for (int i = 0; i < 6; i++)
                {
                    SlaveSlot slot = getSlotByID(missionSlave, i);

                    DropBox tmpBox = (DropBox)tmpWin.getObjectByName("targSlot" + i.ToString());
                    Label tmpLab = (Label)tmpWin.getObjectByName("targVer" + i.ToString());

                    if (slot == null)
                    {
                        tmpBox.SetNewItem("unknown", getTexture("unknown"));
                        tmpLab.Text = "  NA";
                    }
                    else if (slot.ProgramGroup == 2 || (slot.SlotID < 3 && i < 3 && missionSlave.UserPass) || (slot.SlotID >= 3 && i >= 3 && missionSlave.AdminPass))
                    {
                        tmpLab.Text = "v" + slot.ProgramVersion.ToString() + ".0";
                        string programName = getProgramName(slot.ProgramGroup, slot.ProgramSubGroup);
                        tmpBox.SetNewItem(programName, getTexture(programName));
                    }
                    
                }

                Label tmpLab2 = (Label)tmpWin.getObjectByName("labTargUPS");
                tmpLab2.Text = "usr psw lvl: " + gameWorld.UserMission.PassStrength.ToString();

                if (missionSlave.UserPass)
                {
                    tmpLab2 = (Label)tmpWin.getObjectByName("labTargAPS");
                    tmpLab2.Text = "adm psw lvl: " + (gameWorld.UserMission.PassStrength + 1); ToString();
                }

                DropBox tmpBox2 = (DropBox)tmpWin.getObjectByName("targUser");
                if (missionSlave.UserPass)
                    tmpBox2.SetNewItem("userPass", getTexture("yesPass"));
                else
                    tmpBox2.SetNewItem("userPass", getTexture("noPass"));

                tmpBox2 = (DropBox)tmpWin.getObjectByName("targAdmin");
                if (missionSlave.AdminPass)
                    tmpBox2.SetNewItem("adminPass", getTexture("yesPass"));
                else
                    tmpBox2.SetNewItem("adminPass", getTexture("noPass"));
            }
            else if (gameWorld.MissionActive == World.MissionState.NoMission)
            {
                //populate mission slave slots
                Window tmpWin = (Window)getWindowByName("winNetwork").getObjectByName("winNetTarget");
                for (int i = 0; i < 6; i++)
                {
                    DropBox tmpBox = (DropBox)tmpWin.getObjectByName("targSlot" + i.ToString());
                    Label tmpLab = (Label)tmpWin.getObjectByName("targVer" + i.ToString());

                    tmpLab.Text = "  (?)";
                    tmpBox.SetNewItem("slot", getTexture("unknown"));
                }

                Label tmpLab2 = (Label)tmpWin.getObjectByName("labTargUPS");
                tmpLab2.Text = "usr psw lvl: ";

                tmpLab2 = (Label)tmpWin.getObjectByName("labTargAPS");
                tmpLab2.Text = "adm psw lvl: ";

                DropBox tmpBox2 = (DropBox)tmpWin.getObjectByName("targUser");
                tmpBox2.SetNewItem("userPass", getTexture("noPass"));

                tmpBox2 = (DropBox)tmpWin.getObjectByName("targAdmin");
                tmpBox2.SetNewItem("adminPass", getTexture("noPass"));
            }

            //////////////////////////////////////////////////////////////////////////////////////////////
            //slaves section
            //////////////////////////////////////////////////////////////////////////////////////////////

            for (int c = 0; c < 4; c++)
            {
                Slave slave = getSlaveByID(c);
                Window tmpWin = (Window)getWindowByName("winNetSlaves");

                //clear slave details
                if (slave != null)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        SlaveSlot slot = getSlotByID(slave, i);
                        
                        //prog icon
                        DropBox tmpBox = (DropBox)tmpWin.getObjectByName("slaveSlot" + c.ToString() + "." + i.ToString());
                        tmpBox.Visible = true;

                        //prog version
                        Label tmpLab = (Label)tmpWin.getObjectByName("slaveVer" + c.ToString() + "." + i.ToString());
                        tmpLab.Visible = true;

                        if (slot == null)
                        {
                            tmpBox.SetNewItem("unknown", getTexture("unknown"));
                            tmpLab.Text = "  NA";
                        }
                        else if (slot.ProgramGroup == 2 || (slot.SlotID < 3 && i < 3 && slave.UserPass) || (slot.SlotID >= 3 && i >= 3 && slave.AdminPass))
                        {
                            tmpLab.Text = "v" + slot.ProgramVersion.ToString() + ".0";
                            string programName = getProgramName(slot.ProgramGroup, slot.ProgramSubGroup);
                            tmpBox.SetNewItem(programName, getTexture(programName));
                        }
                        else
                        {
                            tmpBox.SetNewItem("unknown", getTexture("unknown"));
                            tmpLab.Text = "  (?)";
                        }

                        //time left
                        tmpLab = (Label)tmpWin.getObjectByName("labSlaveTime" + c.ToString());
                        tmpLab.Visible = true;

                        //user password
                        tmpBox = (DropBox)tmpWin.getObjectByName("slaveUser" + c.ToString());
                        tmpBox.Visible = true;

                        if (slave.UserPass)
                            tmpBox.SetNewItem("userPass", getTexture("yesPass"));
                        else
                            tmpBox.SetNewItem("userPass", getTexture("noPass"));

                        //admin password
                        tmpBox = (DropBox)tmpWin.getObjectByName("slaveAdmin" + c.ToString());
                        tmpBox.Visible = true;
                        if (slave.AdminPass)
                            tmpBox.SetNewItem("adminPass", getTexture("yesPass"));
                        else
                            tmpBox.SetNewItem("adminPass", getTexture("noPass"));

                        //drop button
                        Button btTmp = (Button)tmpWin.getObjectByName("btSlaveDrop" + c.ToString());
                        btTmp.Visible = true;
                    }
                }
                //populate slave
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        //prog icon
                        DropBox tmpBox = (DropBox)tmpWin.getObjectByName("slaveSlot" + c.ToString() + "." + i.ToString());
                        tmpBox.Visible = false;

                        //prog version
                        Label tmpLab = (Label)tmpWin.getObjectByName("slaveVer" + c.ToString() + "." + i.ToString());
                        tmpLab.Visible = false;

                        //time left
                        tmpLab = (Label)tmpWin.getObjectByName("labSlaveTime" + c.ToString());
                        tmpLab.Visible = false;

                        //user password
                        tmpBox = (DropBox)tmpWin.getObjectByName("slaveUser" + c.ToString());
                        tmpBox.Visible = false;

                        //admin password
                        tmpBox = (DropBox)tmpWin.getObjectByName("slaveAdmin" + c.ToString());
                        tmpBox.Visible = false;

                        //drop button
                        Button btTmp = (Button)tmpWin.getObjectByName("btSlaveDrop" + c.ToString());
                        btTmp.Visible = false;
                    }
                }
            }
        }

        private void updateMissionButton()
        {
            Button tmpBut = (Button)getWindowByName("winMission").getObjectByName("btMissOK");
            if (gameWorld.MissionActive == World.MissionState.MissionActive)
                tmpBut.Text = "DROP";
            else if (gameWorld.MissionActive == World.MissionState.MissionComplete)
                tmpBut.Text = "CLAIM";
            else if (gameWorld.MissionActive == World.MissionState.NoMission)
                tmpBut.Text = "ACCEPT";
        }

        private void updateMissionAction()
        {
            if (gameWorld.MissionActive == World.MissionState.NoMission || gameWorld.RunningProg == null || gameWorld.RunningProg.ProgramType == 0)
                return;

            ProgressBar bar = (ProgressBar)getWindowByName("winNetTarget").getObjectByName("targProg");
            bar.setValues(300, 0);

            //do nothing
            if (gameWorld.RunningProg.ProgramType == 1 && gameWorld.RunningProg.ProgramSubType != 7)
                bufferSTR = "info: no effect on target, to upload a program drag directly to empty slot, to delete drag to bin";
            //antivirus
            else if (gameWorld.RunningProg.ProgramType == 1 && gameWorld.RunningProg.ProgramSubType == 7)
            {
                Slave missionSlave = getSlaveByID(999);
                for (int i = 0; i < missionSlave.SlaveFiles.Count(); i++)
                {
                    SlaveSlot slot = missionSlave.SlaveFiles[i];
                    if (slot.ProgramGroup == 4 && slot.ProgramVersion <= gameWorld.RunningProg.ProgramVersion)
                        missionSlave.SlaveFiles.RemoveAt(i);
                }
                try { svrClient.setSlave(gameWorld.SessionID, missionSlave); }
                catch
                {
                    bufferSTR = "error communicating with server";
                    return;
                }
                updateNetworkWin();
            }
            //service scanner
            else if (gameWorld.RunningProg.ProgramType == 3 && gameWorld.RunningProg.ProgramSubType == 1)
            {
                Slave missionSlave = getSlaveByID(999);
                foreach(SlaveSlot slot in missionSlave.SlaveFiles)
                {
                    if (slot.ProgramVersion <= gameWorld.RunningProg.ProgramVersion)
                    {
                        Window tmpWin = getWindowByName("winNetTarget");
                        DropBox tmpBox = (DropBox)tmpWin.getObjectByName("targSlot" + slot.SlotID.ToString());
                        Label tmpLab = (Label)tmpWin.getObjectByName("targVer" + slot.SlotID.ToString());

                        tmpLab.Text = "v" + slot.ProgramVersion.ToString() + ".0";
                        string programName = getProgramName(slot.ProgramGroup, slot.ProgramSubGroup);
                        tmpBox.SetNewItem(programName, getTexture(programName));
                    }
                }
                updateNetworkWin();
            }
            //dictionary cracker
            else if (gameWorld.RunningProg.ProgramType == 4 && gameWorld.RunningProg.ProgramSubType == 8)
            {
                Slave missionSlave = getSlaveByID(999);
                if (missionSlave.UserPass)
                {
                    bufferSTR = "info: you already have user access, use brute-force for admin pass";
                    return;
                }

                if (gameWorld.UserMission.PassStrength <= gameWorld.RunningProg.ProgramVersion)
                    missionSlave.UserPass = true;
                else
                    bufferSTR = "info: program version too low";

                try { svrClient.setSlave(gameWorld.SessionID, missionSlave); }
                catch
                {
                    bufferSTR = "error communicating with server";
                    return;
                }
                updateNetworkWin();
            }
            //brute force
            else if (gameWorld.RunningProg.ProgramType == 4 && gameWorld.RunningProg.ProgramSubType == 7)
            {
                Slave missionSlave = getSlaveByID(999);
                if (!missionSlave.UserPass)
                {
                    if (gameWorld.UserMission.PassStrength <= gameWorld.RunningProg.ProgramVersion)
                        missionSlave.UserPass = true;
                    else
                        bufferSTR = "info: program version too low for user access";

                    try { svrClient.setSlave(gameWorld.SessionID, missionSlave); }
                    catch
                    {
                        bufferSTR = "error communicating with server";
                        return;
                    }

                    updateNetworkWin();
                }
                else if (missionSlave.AdminPass)
                {
                    bufferSTR = "info: you already have admin access";
                    return;
                }

                if (gameWorld.UserMission.PassStrength + 1 <= gameWorld.RunningProg.ProgramVersion)
                {
                    missionSlave.AdminPass = true;
                    bufferSTR = "info: this would take a lot longer in the real world :)";
                }
                else
                    bufferSTR = "info: program version too low for admin hack";

                try { svrClient.setSlave(gameWorld.SessionID, missionSlave); }
                catch
                {
                    bufferSTR = "error communicating with server";
                    return;
                }
                updateNetworkWin();
            }
            //remote exploit
            else if (gameWorld.RunningProg.ProgramType == 4 && gameWorld.RunningProg.ProgramSubType == 1)
            {
                Slave missionSlave = getSlaveByID(999);
                if (!missionSlave.UserPass)
                {
                    bool verOK = false;
                    for (int i = 0; i < 3; i++)
                    {
                        SlaveSlot slot = getSlotByID(missionSlave, i);
                        if (slot.ProgramVersion <= gameWorld.RunningProg.ProgramVersion)
                            verOK = true;
                    }

                    if (verOK)
                        missionSlave.UserPass = true;
                    else
                        bufferSTR = "info: program version too low for successful hack";

                    try { svrClient.setSlave(gameWorld.SessionID, missionSlave); }
                    catch
                    {
                        bufferSTR = "error communicating with server";
                        return;
                    }
                }
                else if (missionSlave.UserPass)
                {
                    bufferSTR = "info: you already have user access, use local exploit to lift privlages";
                    return;
                }

                updateNetworkWin();
            }
            //local exploit
            else if (gameWorld.RunningProg.ProgramType == 4 && gameWorld.RunningProg.ProgramSubType == 2)
            {
                Slave missionSlave = getSlaveByID(999);
                if (!missionSlave.UserPass)
                {
                    bufferSTR = "info: you need local user access to run this exploit";
                    return;
                }
                else if (!missionSlave.AdminPass)
                {
                    bool verOK = false;
                    for (int i = 3; i < 6; i++)
                    {
                        SlaveSlot slot = getSlotByID(missionSlave, i);
                        if (slot.ProgramVersion <= gameWorld.RunningProg.ProgramVersion)
                            verOK = true;
                    }

                    if (verOK)
                        missionSlave.AdminPass = true;
                    else
                        bufferSTR = "info: program version too low for successful hack";

                    try { svrClient.setSlave(gameWorld.SessionID, missionSlave); }
                    catch
                    {
                        bufferSTR = "error communicating with server";
                        return;
                    }
                }
                else if (missionSlave.AdminPass)
                    bufferSTR = "info: you already have admin access";

                updateNetworkWin();
            }
            //ftp map
            else if (gameWorld.RunningProg.ProgramType == 3 && gameWorld.RunningProg.ProgramSubType == 2)
            {
                Slave missionSlave = getSlaveByID(999);
                bool verOK = false;
                foreach (SlaveSlot slot in missionSlave.SlaveFiles)
                    if (slot.ProgramGroup == 2 && slot.ProgramSubGroup == 1 && slot.ProgramVersion <= gameWorld.RunningProg.ProgramVersion)
                        verOK = true;
                
                if (!verOK)
                {
                    bufferSTR = "info: program version too low";
                    return;
                }

                if (gameWorld.UserMission.MissionID == 21 && validateMissionEnd(gameWorld.UserMission, gameWorld.SlavesList) && verOK)
                {
                    gameWorld.MissionActive = World.MissionState.MissionComplete;
                    updateMissionButton();
                }   
                updateNetworkWin();
            }//http map
            else if (gameWorld.RunningProg.ProgramType == 3 && gameWorld.RunningProg.ProgramSubType == 3)
            {
                Slave missionSlave = getSlaveByID(999);
                bool verOK = false;
                foreach (SlaveSlot slot in missionSlave.SlaveFiles)
                    if (slot.ProgramGroup == 2 && slot.ProgramSubGroup == 2 && slot.ProgramVersion <= gameWorld.RunningProg.ProgramVersion)
                        verOK = true;

                if (!verOK)
                {
                    bufferSTR = "info: program version too low";
                    return;
                }

                if (gameWorld.UserMission.MissionID == 20 && validateMissionEnd(gameWorld.UserMission, gameWorld.SlavesList) && verOK)
                {
                    gameWorld.MissionActive = World.MissionState.MissionComplete;
                    updateMissionButton();
                }
                updateNetworkWin();
            }//sql injection
            else if (gameWorld.RunningProg.ProgramType == 3 && gameWorld.RunningProg.ProgramSubType == 3)
            {
                Slave missionSlave = getSlaveByID(999);
                bool verOK = false;
                foreach (SlaveSlot slot in missionSlave.SlaveFiles)
                    if (slot.ProgramGroup == 2 && slot.ProgramSubGroup == 2 && slot.ProgramVersion <= gameWorld.RunningProg.ProgramVersion)
                        verOK = true;

                if (!verOK)
                {
                    bufferSTR = "info: program version too low";
                    return;
                }

                if (gameWorld.UserMission.MissionID == 10 && validateMissionEnd(gameWorld.UserMission, gameWorld.SlavesList) && verOK)
                {
                    gameWorld.MissionActive = World.MissionState.MissionComplete;
                    updateMissionButton();
                }
                updateNetworkWin();
            }

            //check mission end
            if (gameWorld.UserMission.MissionID < 20 && gameWorld.UserMission.MissionID != 10 && validateMissionEnd(gameWorld.UserMission,gameWorld.SlavesList))
            {
                gameWorld.MissionActive = World.MissionState.MissionComplete;
                updateMissionButton();
            }
        }

        private void checkMission()
        {

        }

        private void updateStatWin()
        {
            //gather needed objects
            Window winMiss = getWindowByName("winStats");
            Label labNames = (Label)winMiss.getObjectByName("labStatTop10n");
            Label labAmounts = (Label)winMiss.getObjectByName("labStatTop10a");
            Label labUsers = (Label)winMiss.getObjectByName("labUserStat");
            DropBox boxWhite = (DropBox)winMiss.getObjectByName("statWhite");
            DropBox boxBlack = (DropBox)winMiss.getObjectByName("statBlack");
            DropBox boxCash = (DropBox)winMiss.getObjectByName("statCash");

            labUsers.Text = "Total Users: " + gameWorld.Top10Stats.RegisteredUsers.ToString("N0");

            boxWhite.BackColor = tmpCol;
            boxBlack.BackColor = tmpCol;
            boxCash.BackColor = tmpCol;

            if (gameWorld.Top10Stats.Top10white == null)
                return;

            string textNames = "";
            string textAmounts = "";
            if (gameWorld.StatInd == World.StatChoice.White)
            {
                boxWhite.BackColor = Color.Green;
                int c = 0;
                foreach (UserStatOne stat in gameWorld.Top10Stats.Top10white)
                {
                    textNames += "\n" + stat.UserName;
                    textAmounts += "\n" + stat.Amount.ToString("N0");
                    c++;
                    if (c >= 10)
                        break;
                }
            }
            else if (gameWorld.StatInd == World.StatChoice.Black)
            {
                boxBlack.BackColor = Color.Green;
                int c = 0;
                foreach (UserStatOne stat in gameWorld.Top10Stats.Top10black)
                {
                    textNames += "\n" + stat.UserName;
                    textAmounts += "\n" + stat.Amount.ToString("N0");
                    c++;
                    if (c >= 10)
                        break;
                }
            }
            else if (gameWorld.StatInd == World.StatChoice.Cash)
            {
                boxCash.BackColor = Color.Green;
                int c = 0;
                foreach (UserStatOne stat in gameWorld.Top10Stats.Top10cash)
                {
                    textNames += "\n" + stat.UserName;
                    textAmounts += "\n" + stat.Amount.ToString("N0");
                    c++;
                    if (c >= 10)
                        break;
                }
            }

            labNames.Text = textNames;
            labAmounts.Text = textAmounts;
        }

        private void tenthUpdate()
        {
            if (tenthTimer < 0)
            {
                secondTimer -= 0.1;
                tenthTimer = 0.01;
                if (gameWorld.TimerActive)
                {
                    gameWorld.ActionTimer--;
                    ProgressBar bar = (ProgressBar)getWindowByName("winNetTarget").getObjectByName("targProg");
                    bar.makeStep();
                    if (bar.Value >= bar.Target)
                    {
                        updateMissionAction();
                        gameWorld.TimerActive = false;
                    }
                }
            }

            if (secondTimer < 0)
            {
                secondTimer = 1;
                minuteTimer++;
            }

            if (minuteTimer >= 60)
            {
                minuteTimer = 0;
                if (gameWorld.UserCompiler.Active)
                    refreshCompilerVer();
                refreshSlaveTime();
            }
        }

        #endregion

        #region server requests

        private void loginUser()
        {
            Window menuLogin = getWindowByName("menuLogin");
            Label response = ((Label)menuLogin.getObjectByName("lbResponse"));
            string password = ((TextBox)menuLogin.getObjectByName("tbPassword")).Text;

            if (password.Length == 0)
            {
                response.Text = "error: enter password";
                return;
            }

            gameWorld.PasswordHash = getSHA1hash(password);
            gameWorld.Username = ((TextBox)menuLogin.getObjectByName("tbUsername")).Text;
            try
            {
                gameWorld.SessionID = svrClient.Login(gameWorld.Username.ToLower(), gameWorld.PasswordHash);
                bufferSTR = gameWorld.SessionID;
                if (gameWorld.SessionID.IndexOf("error") != -1)
                {
                    response.Text = gameWorld.SessionID;
                    return;
                }
            }
            catch
            {
                response.Text = "error: can't connect to server";
                flushDNS();
                return;
            }

            menuLogin.Open = false;
            gameWorld.Connected = true;
            bufferSTR = "WELCOME " + gameWorld.Username;

            getWindowByName("winHDD").Open = true;
            //winHDD.Open = true;

            try
            {
                System.Windows.Forms.Application.UserAppDataRegistry.SetValue("userName", gameWorld.Username);
            }
            catch { }

            //load everyting
            loadAllData();
        }

        private void registerUser()
        {
            Window menuRegister = getWindowByName("menuRegister");
            Label response = ((Label)menuRegister.getObjectByName("lbResponseRg"));

            string username = ((TextBox)menuRegister.getObjectByName("tbUsernameReg")).Text;
            string passwordA = ((TextBox)menuRegister.getObjectByName("tbPasswordRg")).Text;
            string passwordB = ((TextBox)menuRegister.getObjectByName("tbPasswordRgCon")).Text;
            string email = ((TextBox)menuRegister.getObjectByName("tbEmailRg")).Text;

            if(passwordA != passwordB || passwordA.IndexOf(" ") != -1)
            {
                response.Text = "Info: passwords do not match";
                return;
            }
            else if(!checkSyntax(email))
            {
                response.Text = "Info: invalid email";
                return;
            }

            try
            {
                string passHash = getSHA1hash(passwordA);
                response.Text = "Info: sending request to server";
                string servres = svrClient.RegisterUser(username, passHash, email);
                response.Text = servres;
                if (servres.IndexOf("error") == -1)
                {
                    menuRegister.Open = false;
                    Window menuLogin = getWindowByName("menuLogin");
                    ((TextBox)menuLogin.getObjectByName("tbUsername")).Text = username;
                    ((TextBox)menuLogin.getObjectByName("tbPassword")).Text = passwordA;
                    menuLogin.Open = true;
                }
            }
            catch 
            { 
                response.Text = "Info: server seems down";
                flushDNS();
            }
        }

        private void requestAccountInfo()
        {
            Window menuHelp = getWindowByName("menuHelp");
            Label response = ((Label)menuHelp.getObjectByName("lbResponseHL"));

            string username = ((TextBox)menuHelp.getObjectByName("tbUsernameHL")).Text;
            string email = ((TextBox)menuHelp.getObjectByName("tbEmailHL")).Text;

            if (!checkSyntax(email))
            {
                response.Text = "error: invalid email";
                return;
            }

            try
            {
                response.Text = svrClient.sendPassword(email);
            }
            catch 
            { 
                response.Text = "error: server not responding";
                flushDNS();
            }

            if (response.Text.IndexOf("error") == -1)
            {
                getWindowByName("menuRegister").Open = false;
                Window menuLogin = getWindowByName("menuLogin");
                ((TextBox)menuLogin.getObjectByName("tbUsername")).Text = username;
                menuLogin.Open = true;
                bufferSTR = response.Text;
            }
        }

        private void changePassword()
        {
            Window menuHelp = getWindowByName("menuHelp");
            Label response = ((Label)menuHelp.getObjectByName("lbResponseHL"));
            string username = ((TextBox)menuHelp.getObjectByName("tbUsernameHL")).Text;
            string oldPassword = ((TextBox)menuHelp.getObjectByName("tbPasswordHL")).Text;
            string newPassword = ((TextBox)menuHelp.getObjectByName("tbNewPasswordHL")).Text;

            if (oldPassword.Length == 0 || newPassword.Length == 0 || newPassword.IndexOf(" ") != -1)
            {
                response.Text = "error: invalid password";
                return;
            }
            else if (username.IndexOf(" ") != -1)
            {
                response.Text = "error: invalid username";
            }

            string newPasswordHash = getSHA1hash(newPassword);

            try
            {
                response.Text = svrClient.changePassword(username, newPasswordHash);
                menuHelp.Open = false;
                getWindowByName("menuLogin").Open = true;
                bufferSTR = response.Text;
            }
            catch 
            { 
                response.Text = "error: server not responding";
                flushDNS();
            }
        }

        private void checkClientVersion()
        {
            //check client version
            try
            {
                string verChk = svrClient.checkClientVersion(assemblyVersion);
                if (verChk.IndexOf("error") != -1)
                {
                    if (verChk == "error: client version too low")
                    {
                        if(System.Windows.Forms.MessageBox.Show("Please update your client software. This version (" + assemblyVersion + ") is not supported. \nVisit http://sites.google.com/site/cybercritics/ for latest version."+
                                                                "\n\nWould you like to go to update site?", "Incompatible version",System.Windows.Forms.MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                            startProcess("http://sites.google.com/site/cybercritics/", "");
                    }
                    else
                        System.Windows.Forms.MessageBox.Show("Server seems down. Please try again later.");
                    this.Exit();
                }
                bufferSTR = verChk;
            }
            catch
            {
                flushDNS();
                try
                {
                    string verChk = svrClient.checkClientVersion(assemblyVersion);
                    if (verChk == "error: client version too low")
                    {
                        if (System.Windows.Forms.MessageBox.Show("Please update your client software. This version (" + assemblyVersion + ") is not supported. \nVisit http://sites.google.com/site/cybercritics/ for latest version." +
                                                                "\n\nWould you like to go to update site?", "Incompatible version", System.Windows.Forms.MessageBoxButtons.YesNoCancel) == System.Windows.Forms.DialogResult.Yes)
                            startProcess("http://sites.google.com/site/cybercritics/", "");
                        else
                            System.Windows.Forms.MessageBox.Show("Server seems down. Please try again later.");

                        this.Exit();
                    }
                    bufferSTR = verChk;
                }
                catch
                {
                    System.Windows.Forms.MessageBox.Show("Server seems down. Please try again later.");
                    this.Exit();
                }
            }
        }

        private void startCompilerJob()
        {
            try
            {
                bufferSTR = svrClient.addCompilerJob(gameWorld.SessionID, gameWorld.UserCompiler);
                
                if (bufferSTR.IndexOf("error") == -1)
                { 
                    updateUserInfo();
                    gameWorld.UserCompiler = new CompilerJob();
                    updateCompilerWin();
                }

            }
            catch { bufferSTR = "error: could not submit job"; }
        }

        private void claimCompilerJob()
        {
            try 
            { 
                bufferSTR = svrClient.claimCompilerJob(gameWorld.SessionID);
                gameWorld.UserCompiler = new CompilerJob();
            }
            catch { bufferSTR = "error: claiming job"; }
            updateCompilerWin();
            updateHDDwin();
        }

        #endregion

        #region drawRegion
        /// <summary>
        /// helper, draws line
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="color"></param>
        private void drawLine(Vector2 p0, Vector2 p1, Color color)
        {
            primitiveBatch.Begin(PrimitiveType.LineList);
            primitiveBatch.AddVertex(p0, color);
            primitiveBatch.AddVertex(p1, color);
            primitiveBatch.End();
        }

        private void drawUserInfo()
        {
            spriteBatch.Begin();
            if (gameWorld.UserInfo != null)
            {
                spriteBatch.DrawString(font, gameWorld.UserInfo.UserGold.ToString("N0"), new Vector2(screenSize.X / 2 - 70, 10), Color.Black);
                spriteBatch.DrawString(font, gameWorld.UserInfo.UserCash.ToString("N0"), new Vector2(screenSize.X / 2 + 10, 10), Color.Black);
            }
            spriteBatch.End();
        }

        private void drawBackground()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);
            spriteBatch.End();
        }

        private void drawSystemIcons()
        {
            //draw texture
            foreach (DropBox db in sysObjects.systemIcons)
                db.Draw(spriteBatch, primitiveBatch, new Vector2(0, 0));
        }

        private void drawDragging(SpriteBatch spriteBatch)
        {
            if (activeDrag == null)
                return;

            Texture2D dragging = null;
            if(activeDrag.GetType() == typeof (DropBox))
            {
                dragging = getTexture(((DropBox)activeDrag).Holding);
            }

            if (dragging == null)
                return;

            //draw texture
            spriteBatch.Begin();
            spriteBatch.Draw(dragging, new Rectangle(mouseState.X, mouseState.Y, 24, 24), Color.White);
            spriteBatch.End();
        }

        private void drawLink()
        {
            spriteBatch.Begin();
            if (mouseState.X >= screenSize.X - 100 && mouseState.X <= screenSize.X - 5 &&
                mouseState.Y >= screenSize.Y - 15 && mouseState.Y <= screenSize.Y)
                spriteBatch.DrawString(font, "visit cyberCritics", new Vector2(screenSize.X - 100, screenSize.Y - 15), Color.White);
            else
                spriteBatch.DrawString(font, "visit cyberCritics", new Vector2(screenSize.X - 100, screenSize.Y - 15), Color.Black);
            spriteBatch.End();
        }

        private void drawSlaveInfo(SpriteBatch spriteBatch, Vector2 position)
        {
            /*primitiveBatch.Begin(PrimitiveType.LineList);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y + 20), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y + 20), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X + size.X, position.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y + size.Y), Color.Black);
            primitiveBatch.AddVertex(new Vector2(position.X, position.Y), Color.Black);
            primitiveBatch.End();*/
        }

        #endregion

        #region helpers

        private void refreshCompilerVer()
        {
            Window tmpWin = getWindowByName("winCompiler");
            for (int i = 2; i <= 5; i++)
            {
                DropBox tmpBox = (DropBox)tmpWin.getObjectByName("compV" + i.ToString());
                if (i <= gameWorld.UserCompiler.ProgramVersion)
                    tmpBox.BackColor = Color.LightGreen;
                else
                    tmpBox.BackColor = tmpCol;
            }

            if (!gameWorld.UserCompiler.Active)
            {
                gameWorld.UserCompiler.StartTime = DateTime.UtcNow.Ticks;
                gameWorld.UserCompiler.EndTime = ((TimeSpan.TicksPerMinute * 1) * (gameWorld.UserCompiler.ProgramVersion - 0)) +
                                                 gameWorld.UserCompiler.StartTime;
            }

            //setup label
            Label tmpLab = (Label)tmpWin.getObjectByName("labCompInfo");
            int cost = (gameWorld.UserCompiler.ProgramVersion - 1) * 500;
            if (gameWorld.UserCompiler.ProgramVersion != 4)
                cost -= 500;
            tmpLab.Text = "$" + cost.ToString("N0");
            if (gameWorld.UserCompiler.ProgramVersion == 5)
                tmpLab.Text += " [1]";
            long timeDiff = (gameWorld.UserCompiler.EndTime - DateTime.UtcNow.Ticks) / TimeSpan.TicksPerMinute;
            
            if (timeDiff < 0)
                timeDiff = 0;

            if (gameWorld.UserCompiler.EndTime - DateTime.UtcNow.Ticks < 0)
                tmpLab.Text += "\n" + "ready";
            else if(timeDiff < 1)
                tmpLab.Text += "\n" + "<1min";
            else 
                tmpLab.Text += "\n" + timeDiff.ToString() + "min";

            tmpLab.Text += "\n" + gameWorld.UserCompiler.BuddyName;

            //setup button
            Button tmpBut = (Button)tmpWin.getObjectByName("btCompiler");
            if (tmpBut.Text == "START" && gameWorld.UserCompiler.Active)
                tmpBut.Text = "CLAIM";
            else if (tmpBut.Text == "CLAIM" && !gameWorld.UserCompiler.Active)
                tmpBut.Text = "START";

            //setup image
            DropBox tmpBoxA = (DropBox)tmpWin.getObjectByName("baseComProg");
            tmpBoxA.Holding = getProgramName(gameWorld.UserCompiler.ProgramType, gameWorld.UserCompiler.ProgramSubType);
            if (tmpBoxA.Holding == "2x2")
                tmpBoxA.Holding = "";

            if (tmpBoxA.Holding != "")
                tmpBoxA.SetNewItem(tmpBoxA.Holding, getTexture(tmpBoxA.Holding));
            else
            {
                tmpLab.Text = "NA";
                tmpBoxA.Texture = null;
            }
        }

        private void refreshSlaveTime()
        {
            Window tmpWin = (Window)getWindowByName("winNetSlaves");
            for (int i = 0; i < 4; i++)
            {
                //time left
                Label tmpLab = (Label)tmpWin.getObjectByName("labSlaveTime" + i.ToString());

                if (!tmpLab.Visible)
                    continue;

                Slave slave = getSlaveByID(i);
                if (slave == null)
                    return;

                long time = slave.EndTime - DateTime.UtcNow.Ticks;
                if (time < 0)
                {
                    try { svrClient.deleteSlave(gameWorld.SessionID, slave.SlaveID); }
                    catch { bufferSTR = "error communicating with server"; return; }
                    updateNetworkWin();
                }

                string timeLeft = "";
                if (time > TimeSpan.TicksPerDay)
                    timeLeft = (time / TimeSpan.TicksPerDay).ToString("N0") + " days";
                else if (time > TimeSpan.TicksPerHour)
                    timeLeft = (time / TimeSpan.TicksPerHour).ToString("N0") + " hours";
                else if (time > TimeSpan.TicksPerMinute)
                    timeLeft = (time / TimeSpan.TicksPerMinute).ToString("N0") + " minutes";
                else
                    timeLeft = " <1min";

                tmpLab.Text = "time until detection:" + timeLeft;
            }
        }

        private int getNextMissionByHat(bool whiteHat, bool forward)
        {
            if (gameWorld.MissionTypes.Missions == null)
                return 0;

            int end = gameWorld.MissionTypes.Missions.Count();
            if (forward)
                for (int i = gameWorld.MissionInd + 1; i < end; i++)
                {
                    if (whiteHat && gameWorld.MissionTypes.Missions[i].HatPoints >= 0)
                        return i;
                    else if (!whiteHat && gameWorld.MissionTypes.Missions[i].HatPoints <= 0)
                        return i;
                }
            else
                for (int i = gameWorld.MissionInd - 1; i >= 0; i--)
                {
                    if (whiteHat && gameWorld.MissionTypes.Missions[i].HatPoints >= 0)
                        return i;
                    else if (!whiteHat && gameWorld.MissionTypes.Missions[i].HatPoints <= 0)
                        return i;
                }

            if (forward)
                return 0;
            else
            {
                gameWorld.MissionInd = end;
                return getNextMissionByHat(whiteHat, false);
            }
        }

        /// <summary>
        /// list trough program textures and return match
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Texture2D getTexture(string name)
        {
            if (name == null)
                return null;

            name = name.ToLower();
            foreach (Texture2D tex in programTextures)
                if (tex.Name.ToLower() == name)
                    return tex;

            if (name == "noImage")
                return null;

            return getTexture("noImage");
        }

        private Window getWindowByName(string name)
        {
            foreach (Window win in sysObjects.systemWindows)
            {
                if (win.Name == name)
                    return win;
                if (win.getObjectByName(name) != null)
                    return (Window)win.getObjectByName(name);
            }
            return null;
        }

        private ProgramHW getProgramByName(string name)
        {
            ProgramHW result = null;
            foreach (ProgramHW prog in gameWorld.FileTypes.ProgramTypesLst)
                if (prog.ProgramName == name)
                {
                    result = new ProgramHW();
                    result.ProgramType = prog.ProgramType;
                    result.HddSlot = prog.HddSlot;
                    result.ProgramVersion = prog.ProgramVersion;
                    result.ProgramSubType = prog.ProgramSubType;
                    result.ProgramDescription = prog.ProgramDescription;
                    result.ProgramName = prog.ProgramName;
                    result.UsesLeft = prog.UsesLeft;
                    result.BasePrice = prog.BasePrice;
                    return result;
                }

            return null;
        }

        private MissionType getMissionByID(int missionID)
        {
            if (gameWorld.MissionTypes.Missions == null)
                return null;

            foreach (MissionType mis in gameWorld.MissionTypes.Missions)
                if (mis.MissionID == missionID)
                    return mis;
            return null;
        }

        /// <summary>
        /// check email syntax
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool checkSyntax(string email)
        {
            if (email.IndexOf("@") == -1 || email.IndexOf("@") != email.LastIndexOf("@") || email.IndexOf(".") == -1 || email.IndexOf(" ") != -1)
                return false;

            return true;
        }

        /// <summary>
        /// server is on dynamic DNS, flush dns if needed
        /// </summary>
        private void flushDNS()
        {
            if (System.Windows.Forms.MessageBox.Show(" It seems that the server is down or it's IP has changed,\n hackerWorld server uses dynamic DNS,\n flushing your DNS cache might solve this problem.","Flush DNS cache?",System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                bufferSTR = "flushing DNS cache";
                startProcess("ipconfig", "/flushdns");
                bufferSTR = "flushed DNS cache, try again";
            }
        }

        /// <summary>
        /// computes sha1 hash from given string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string getSHA1hash(string input)
        {
            string result = "";

            input += passwordSalt;
            byte[] data = new byte[input.Length];

            int i = 0;
            foreach (char ch in input)
            {
                data[i] = (byte)ch;
                i++;
            }

            SHA1 sha = new SHA1CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(data);

            foreach (byte by in hash)
                result += showHex(by);

            return result;
        }

        /// <summary>
        /// shows hex of given byte
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string showHex(byte input)
        {
            string result = "";
            int a = input >> 4;
            int b = input & 15;
            if (a < 10)
                result += a.ToString();
            else
                result += ((hex)a).ToString();

            if (b < 10)
                result += b.ToString();
            else
                result += ((hex)b).ToString();


            return result;
        }

        /// <summary>
        /// start process
        /// </summary>
        /// <param name="filename">proccess name to start</param>
        private void startProcess(string filename, string args)
        {
            try
            {
                ProcessStartInfo ProcessInfo;
                Process Process;
                ProcessInfo = new ProcessStartInfo(filename);
                ProcessInfo.Arguments += args;
                ProcessInfo.CreateNoWindow = true;
                Process = Process.Start(ProcessInfo);
            }
            catch { bufferSTR = "failed to start process"; }
        }

        /// <summary>
        /// relay keyboard input to active object
        /// </summary>
        /// <param name="input"></param>
        private void relayInput(string input)
        {
            try
            {
                foreach (Window win in sysObjects.systemWindows)
                    if (win.Name.IndexOf("menu") != -1 && win.Open)
                        activeInputHolder = win.getActiveObject();

                if (activeInputHolder != null)
                {
                    if (activeInputHolder.GetType() == typeof(TextBox))
                    {

                        if (input == "tb" || input == "\n")
                        {
                            foreach (Window win in sysObjects.systemWindows)
                                if (win.Name.IndexOf("menu") != -1 && win.Open)
                                    win.getNextControl(((TextBox)activeInputHolder).Name);
                            //pass on login click
                            if (input == "\n" && ((TextBox)activeInputHolder).Name == "tbPassword")
                                handleButtonClick(activeInputHolder, new SimpleEventArgs("btLogin"));
                        }
                        else
                            ((TextBox)activeInputHolder).addText(input);
                    }
                }
            }
            catch { }
        }

        /// <summary> 
        /// Convert a key to it's respective character or escape sequence. 
        /// </summary> 
        /// <param name="key">The key to convert.</param> 
        /// <param name="shift">Is the shift key pressed or caps lock down.</param> 
        /// <returns>The char for the key that was pressed or string.Empty if it doesn't have a char representation.</returns> 
        public static string convertKeyToChar(Keys key, bool shift) 
        { 
            switch (key) 
            { 
                case Keys.Space: return " "; 
 
                // Escape Sequences 
                case Keys.Enter: return "\n";                         // Create a new line 
                case Keys.Tab: return "tb";                           // Tab to the right 
                case Keys.Back: return "bs";

                // D-Numerics (strip above the alphabet) 
                case Keys.D0: return shift ? ")" : "0"; 
                case Keys.D1: return shift ? "!" : "1"; 
                case Keys.D2: return shift ? "@" : "2"; 
                case Keys.D3: return shift ? "#" : "3"; 
                case Keys.D4: return shift ? "$" : "4"; 
                case Keys.D5: return shift ? "%" : "5"; 
                case Keys.D6: return shift ? "^" : "6"; 
                case Keys.D7: return shift ? "&" : "7"; 
                case Keys.D8: return shift ? "*" : "8"; 
                case Keys.D9: return shift ? "(" : "9"; 
 
                // Numpad 
                case Keys.NumPad0: return "0"; 
                case Keys.NumPad1: return "1"; 
                case Keys.NumPad2: return "2"; 
                case Keys.NumPad3: return "3"; 
                case Keys.NumPad4: return "4"; 
                case Keys.NumPad5: return "5"; 
                case Keys.NumPad6: return "6"; 
                case Keys.NumPad7: return "7"; 
                case Keys.NumPad8: return "8"; 
                case Keys.NumPad9: return "9"; 
                case Keys.Add: return "+"; 
                case Keys.Subtract: return "-"; 
                case Keys.Multiply: return "*"; 
                case Keys.Divide: return "/"; 
                case Keys.Decimal: return ".";

                // Alphabet 
                case Keys.A: return shift ? "A" : "a";
                case Keys.B: return shift ? "B" : "b";
                case Keys.C: return shift ? "C" : "c";
                case Keys.D: return shift ? "D" : "d";
                case Keys.E: return shift ? "E" : "e";
                case Keys.F: return shift ? "F" : "f";
                case Keys.G: return shift ? "G" : "g";
                case Keys.H: return shift ? "H" : "h";
                case Keys.I: return shift ? "I" : "i";
                case Keys.J: return shift ? "J" : "j";
                case Keys.K: return shift ? "K" : "k";
                case Keys.L: return shift ? "L" : "l";
                case Keys.M: return shift ? "M" : "m";
                case Keys.N: return shift ? "N" : "n";
                case Keys.O: return shift ? "O" : "o";
                case Keys.P: return shift ? "P" : "p";
                case Keys.Q: return shift ? "Q" : "q";
                case Keys.R: return shift ? "R" : "r";
                case Keys.S: return shift ? "S" : "s";
                case Keys.T: return shift ? "T" : "t";
                case Keys.U: return shift ? "U" : "u";
                case Keys.V: return shift ? "V" : "v";
                case Keys.W: return shift ? "W" : "w";
                case Keys.X: return shift ? "X" : "x";
                case Keys.Y: return shift ? "Y" : "y";
                case Keys.Z: return shift ? "Z" : "z";

                // Oem 
                case Keys.OemOpenBrackets: return shift ? "{" : "[";
                case Keys.OemCloseBrackets: return shift ? "}" : "]";
                case Keys.OemComma: return shift ? "<" : ",";
                case Keys.OemPeriod: return shift ? ">" : ".";
                case Keys.OemMinus: return shift ? "_" : "-";
                case Keys.OemPlus: return shift ? "+" : "=";
                case Keys.OemQuestion: return shift ? "?" : "/";
                case Keys.OemSemicolon: return shift ? ":" : ";";
                case Keys.OemQuotes: return shift ? "\"" : "'";
                case Keys.OemPipe: return shift ? "|" : "\\";
                case Keys.OemTilde: return shift ? "~" : "`";
            }

            return string.Empty;
        }

        /// <summary>
        /// is there a file at given slot
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool hasProgramAtSlot(int pos)
        {
            foreach (ProgramHW file in gameWorld.UserHDD.Programs)
                if (file.HddSlot == pos)
                    return true;

            return false;
        }

        /// <summary>
        /// is there a file at given slot
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private bool hasProgramAtCPUSlot(int pos)
        {
            foreach (CPUslotHW slot in gameWorld.UserCPU.Programs)
                if (slot.CpuSlot == pos)
                    return true;

            return false;
        }

        /// <summary>
        /// is there a file at given slot
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private ProgramHW getProgramAtSlot(int pos)
        {
            foreach (ProgramHW file in gameWorld.UserHDD.Programs)
                if (file.HddSlot == pos)
                    return file;

            return null;
        }

        /// <summary>
        /// is there a file at given slot
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private CPUslotHW getProgramAtCPUSlot(int pos)
        {
            foreach (CPUslotHW slot in gameWorld.UserCPU.Programs)
                if (slot.CpuSlot == pos)
                    return slot;

            return null;
        }

        /// <summary>
        /// get name of filetype
        /// </summary>
        /// <param name="fileType"></param>
        /// <param name="fileSubType"></param>
        /// <returns></returns>
        private string getProgramName(int fileType, int fileSubType)
        {
            foreach (ProgramHW type in gameWorld.FileTypes.ProgramTypesLst)
                if (type.ProgramType == fileType && type.ProgramSubType == fileSubType)
                    return type.ProgramName;

            return "2x2";
        }

        /// <summary>
        /// find slave with given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Slave getSlaveByID(int id)
        {
            foreach (Slave sl in gameWorld.SlavesList.SlaveList)
                if (sl.SlaveID == id)
                    return sl;
            return null;
        }

        /// <summary>
        /// find a slaveSlot in given slave
        /// </summary>
        /// <param name="slave"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private SlaveSlot getSlotByID(Slave slave, int id)
        {
            foreach (SlaveSlot slot in slave.SlaveFiles)
                if (slot.SlotID == id)
                    return slot;
            return null;
        }

        /// <summary>
        /// check all slaves for program version
        /// </summary>
        /// <param name="progGroup"></param>
        /// <param name="progSubGroup"></param>
        /// <returns></returns>
        private int getMaxProgramVer(int progGroup, int progSubGroup)
        {
            int max = -1;
            foreach (Slave slave in gameWorld.SlavesList.SlaveList)
            {
                if (slave.SlaveID == 999)
                    continue;

                foreach (SlaveSlot slot in slave.SlaveFiles)
                {
                    if (slot.ProgramGroup == progGroup && slot.ProgramSubGroup == progSubGroup && slot.ProgramVersion > max)
                        max = slot.ProgramVersion;
                }
            }

            foreach(CPUslotHW slot in gameWorld.UserCPU.Programs)
                if (slot.ProgramType == progGroup && slot.ProgramSubType == progSubGroup && slot.ProgramVersion > max)
                    max = slot.ProgramVersion;

            return max;
        }

        private bool validateMissionEnd(UserMission mission, SlaveListHW slaves)
        {
            Slave missionSlave = getSlaveByID(999);

            if (missionSlave == null)
                return false;

            //check mission requirements
            //check mission requirements
            switch (mission.MissionID)
            {
                case 0:
                    if (missionSlave.UserPass || missionSlave.AdminPass)
                        return true;
                    break;
                case 1:
                case 2:
                case 3:
                case 4:
                    if (slaveHasProgram(slaves, 999, mission.ProgramGroup, mission.ProgramSubGroup, mission.ProgramVersion, mission.ProgramVersion))
                        return true;
                    break;
                case 9:
                case 11:
                case 12:
                case 14:
                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                    if (slaveHasProgram(slaves, 999, mission.ProgramGroup, mission.ProgramSubGroup, 0, 5))
                        return true;
                    break;
                case 5:
                    if (!slaveHasProgram(slaves, 999, mission.ProgramGroup, mission.ProgramSubGroup, mission.ProgramVersion, mission.ProgramVersion))
                        return true;
                    break;
                case 6:
                    if (missionSlave.SlaveFiles.Count == 0)
                        return true;
                    break;
                case 10:
                case 20:
                case 21:
                    return true;
                case 7:
                case 8:
                    if (missionSlave.UserPass)
                        return true;
                    break;
                case 13:
                    if (missionSlave.AdminPass)
                        return true;
                    break;

                default:
                    return false;
            }

            return false;
        }

        private bool slaveHasProgram(SlaveListHW slaves, int slaveID, int progGroup, int progSub, int minProgVer, int maxProgVer)
        {
            Slave slave = getSlaveByID(slaveID);
            if (slave == null)
                return false;

            foreach (SlaveSlot slot in slave.SlaveFiles)
                if (slot.ProgramGroup == progGroup && slot.ProgramSubGroup == progSub && slot.ProgramVersion >= minProgVer && slot.ProgramVersion <= maxProgVer)
                    return true;

            return false;
        }

        //donations
        //https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=76UZZHKWEQ8DG

        #endregion
    }
}
