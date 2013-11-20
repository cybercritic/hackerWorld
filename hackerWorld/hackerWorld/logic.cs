using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using hackerWorldNS.HackerWorldService;

namespace hackerWorldNS
{

    class World
    {
        public enum MissionState { NoMission, MissionActive, MissionComplete }
        public enum StatChoice { White, Black, Cash }

        private bool connected = false;
        private bool missionHat = true;
        private bool timerActive = false;
        private string username = "";
        private string passwordHash = "";
        private string sessionID = "";
        private int missionInd = 0;
        private int actionTimer = 300;
        private StatChoice statInd = StatChoice.White;
        private SpriteFont font;
        private HardDrive userHDD;
        private ProgramTypes fileTypes;
        private UserInfo userInfo = new UserInfo();
        private CPUload userCPU = new CPUload();
        private CompilerJob userCompiler = new CompilerJob();
        private MissionList missionTypes = new MissionList();
        private Random rnd = new Random((int)DateTime.Now.Ticks);
        private MissionState missionActive;
        private UserMission userMission = new UserMission();
        private SlaveListHW slavesList = new SlaveListHW();
        private CPUslotHW runningProg = new CPUslotHW();
        private UserStats top10Stats = new UserStats();

        public bool Connected { set { connected = value; } get { return connected; } }
        public bool MissionHat { set { missionHat = value; } get { return missionHat; } }
        public bool TimerActive { set { timerActive = value; } get { return timerActive; } }
        public string Username { set { username = value; } get { return username; } }
        public string PasswordHash { set { passwordHash = value; } get { return passwordHash; } }
        public string SessionID { set { sessionID = value; } get { return sessionID; } }
        public int MissionInd { set { missionInd = value; } get { return missionInd; } }
        public int ActionTimer { set { actionTimer = value; } get { return actionTimer; } }
        public SpriteFont Font { set { font = value; } get { return font; } }
        public HardDrive UserHDD { set { userHDD = value; } get { return userHDD; } }
        public ProgramTypes FileTypes { set { fileTypes = value; } get { return fileTypes; } }
        public UserInfo UserInfo { set { userInfo = value; } get { return userInfo; } }
        public CPUload UserCPU { set { userCPU = value; } get { return userCPU; } }
        public CompilerJob UserCompiler { set { userCompiler = value; } get { return userCompiler; } }
        public MissionList MissionTypes { set { missionTypes = value; } get { return missionTypes; } }
        public MissionState MissionActive { set { missionActive = value; } get { return missionActive; } }
        public UserMission UserMission { set { userMission = value; } get { return userMission; } }
        public SlaveListHW SlavesList { set { slavesList = value; } get { return slavesList; } }
        public CPUslotHW RunningProg { set { runningProg = value; } get { return runningProg; } }
        public UserStats Top10Stats { set { top10Stats = value; } get { return top10Stats; } }
        public StatChoice StatInd { set { statInd = value; } get { return statInd; } }

        public World() { }

        public void Draw(SpriteBatch spriteBatch, PrimitiveBatch primitiveBatch)
        {
            
        }

        public UserMission createUserMission(int missionID)
        {
            UserMission result = new UserMission();
            switch (missionID)
            {
                //Hack a random target and slave it.
                case 0:
                    result.MissionID = 0;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 4;
                    result.ProgramSubGroup = 7;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Install %1 %2 on client computer.
                case 1:
                    result.MissionID = 1;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = true;
                    result.ProgramGroup = 1;
                    result.ProgramSubGroup = rnd.Next(7) + 1;
                    result.ProgramVersion = rnd.Next(4) + 1;
                    break;
                //Break in and install %1 %2 on victim computer.
                case 2:
                    result.MissionID = 2;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 1;
                    result.ProgramSubGroup = rnd.Next(7) + 1;
                    result.ProgramVersion = rnd.Next(4) + 1;
                    break;
                //Upgrade client's %1 to %2.
                case 3:
                    result.MissionID = 3;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = true;
                    result.HaveAdmin = true;
                    result.ProgramGroup = 2;
                    result.ProgramSubGroup = rnd.Next(5) + 1;
                    result.ProgramVersion = rnd.Next(3) + 2;
                    break;
                //Break in and downgrade target's %1 to %2
                case 4:
                    result.MissionID = 4;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 2;
                    result.ProgramSubGroup = rnd.Next(5) + 1;
                    result.ProgramVersion = 1;
                    break;
                //Remove unwanted %1 from client computer.
                case 5:
                    result.MissionID = 5;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = true;
                    result.HavePass = true;
                    result.HaveAdmin = true;
                    result.ProgramGroup = 4;
                    result.ProgramSubGroup = rnd.Next(6) + 1;
                    result.ProgramVersion = rnd.Next(4) + 1;
                    break;
                //Break in and format target HDD.
                case 6:
                    result.MissionID = 6;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = true;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 1;
                    result.ProgramSubGroup = 3;
                    result.ProgramVersion = -1;
                    break;
                //Client forgot his password, crack it for him.
                case 7:
                    result.MissionID = 7;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 4;
                    result.ProgramSubGroup = 7 + rnd.Next(2);
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Crack victim password.
                case 8:
                    result.MissionID = 8;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 4;
                    result.ProgramSubGroup = 7 + rnd.Next(2);
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Install a backdoor for client.
                case 9:
                    result.MissionID = 9;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 4;
                    result.ProgramSubGroup = 4;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Alter victim database.
                case 10:
                    result.MissionID = 10;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 2;
                    result.ProgramSubGroup = 4;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Develop and deliver %1 %2 for client.
                case 11:
                    result.MissionID = 11;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = true;
                    result.HaveAdmin = true;
                    result.ProgramGroup = 4;
                    result.ProgramSubGroup = rnd.Next(9) + 1;
                    result.ProgramVersion = 5;
                    break;
                //Develop and deliver %1 %2 for client.
                case 12:
                    result.MissionID = 12;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = true;
                    result.HaveAdmin = true;
                    result.ProgramGroup = 3;
                    result.ProgramSubGroup = rnd.Next(3) + 1;
                    result.ProgramVersion = 5;
                    break;
                //Gain admin privlages on victim computer.
                case 13:
                    result.MissionID = 13;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = true;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 4;
                    result.ProgramSubGroup = 7 + rnd.Next(2);
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Setup a scam website.
                case 14:
                    result.MissionID = 14;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = true;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 2;
                    result.ProgramSubGroup = 2;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Setup IRC bot-net controller.
                case 15:
                    result.MissionID = 15;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = true;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 2;
                    result.ProgramSubGroup = 3;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Setup SQL database for client.
                case 16:
                    result.MissionID = 16;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = true;
                    result.HavePass = true;
                    result.HaveAdmin = true;
                    result.ProgramGroup = 2;
                    result.ProgramSubGroup = 4;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Setup a web-server for client.
                case 17:
                    result.MissionID = 17;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = true;
                    result.HavePass = true;
                    result.HaveAdmin = true;
                    result.ProgramGroup = 2;
                    result.ProgramSubGroup = 2;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Install a keylogger on victim PC.
                case 18:
                    result.MissionID = 18;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = true;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 4;
                    result.ProgramSubGroup = 6;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Install root-kit on victim computer.
                case 19:
                    result.MissionID = 19;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = true;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 4;
                    result.ProgramSubGroup = 5;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Map web-server.
                case 20:
                    result.MissionID = 20;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 2;
                    result.ProgramSubGroup = 2;
                    result.ProgramVersion = result.PassStrength;
                    break;
                //Map ftp-server.
                case 21:
                    result.MissionID = 21;
                    result.PassStrength = rnd.Next(4) + 1;
                    result.NeedAdmin = false;
                    result.HavePass = false;
                    result.HaveAdmin = false;
                    result.ProgramGroup = 2;
                    result.ProgramSubGroup = 1;
                    result.ProgramVersion = result.PassStrength;
                    break;
                default:
                    
                    break;
            }

            return result;
        }
    }

    /*class Trash
    {
        private Texture2D trashTexture;
        private Vector2 position;
        private SpriteFont font;

        public Texture2D TrashTexture { set { trashTexture = value; } get { return trashTexture; } }
        public Vector2 Position { get { return position; } set { position = value; } }
        public SpriteFont Font { set { font = value; } get { return font; } }

        public Trash(Texture2D trashTexture, Vector2 position, SpriteFont font)
        {
            this.trashTexture = trashTexture;
            this.position = position;
            this.font = font;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(trashTexture, position, Color.White);
            spriteBatch.DrawString(font, "Trash Can" , new Vector2(position.X,position.Y + 48) , Color.Black);
            spriteBatch.End();
        }
    }*/

    class SystemObjects
    {
        public List<DropBox> systemIcons = new List<DropBox>();
        public List<Window> systemWindows = new List<Window>();
    }
}
