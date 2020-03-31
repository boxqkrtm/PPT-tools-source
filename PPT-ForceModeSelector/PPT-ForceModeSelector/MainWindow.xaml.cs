using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PPT_ForceModeSelector
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread worker = null;
        ProcessMemory Game;
        public MainWindow()
        {
            InitializeComponent();
            none.Click += radioButtons_CheckedChanged;
            end.Click += radioButtons_CheckedChanged;
            mini.Click += radioButtons_CheckedChanged;
            a6w.Click += radioButtons_CheckedChanged;
            a10w.Click += radioButtons_CheckedChanged;
            nodamage.Click += radioButtons_CheckedChanged;
        }

        public void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            //RadioButton radioButton = sender as RadioButton;
            if (worker != null) worker.Abort();
            if (none.IsChecked == true)
            {

            }
            else if (mini.IsChecked == true)
            {
                worker = new Thread(() => Run(0));
                worker.Start();
            }
            else if (end.IsChecked == true)
            {
                worker = new Thread(() => Run(1));
                worker.Start();
            }
            else if (a6w.IsChecked == true)
            {
                worker = new Thread(() => Run(2));
                worker.Start();
            }
            else if (a10w.IsChecked == true)
            {
                worker = new Thread(() => Run(3));
                worker.Start();
            }
            else if (nodamage.IsChecked == true)
            {
                worker = new Thread(() => Run(4));
                worker.Start();
            }
        }

        private void Run(int val)
        {
            Game = new ProcessMemory("puyopuyotetris");
            while (true)
            {
                switch (val)
                {
                    case 0:
                        //minipuyo
                        Game.WriteInt32(new IntPtr(0x140598BB8), 8);//set mode
                        break;
                    case 1:
                        //endless fever
                        Game.WriteInt32(new IntPtr(0x140598BB8), 0);
                        Game.WriteInt32(new IntPtr(0x140598BB0), 3);//set fever
                        break;
                    case 2:
                        //6w fusion
                        //Game.WriteInt32(new IntPtr(0x140598BB8), 
                        //    InPreGame()
                        //        ? 3 //If in pregame, force gamemode to 6w
                        //        : 4 //else, force gamemode to regular fusion
                        //    );
                        if (InPreGame())
                        {
                            Game.WriteInt32(new IntPtr(0x140598BB8), 3);
                        }
                        else
                        {
                            Game.WriteInt32(new IntPtr(0x140598BB8), 4);
                        }
                        break;
                    case 3:
                        //10w fusion
                        Game.WriteInt32(new IntPtr(0x140598BB8),
                            InPreGame()
                                ? 2
                                : 4
                            );
                        break;
                    case 4:
                        //no damage tetris
                        Game.WriteInt32(new IntPtr(0x140598BB0), 0);
                        break;
                    case 5:
                        //you have been disconnected from the internet
                        Game.WriteInt32(new IntPtr(0x1404606A4), 20);
                        Game.WriteInt32(new IntPtr(0x1404606A0), 20);
                        break;
                    case 6:
                        //puzzle league loadding
                        Game.WriteInt32(new IntPtr(0x1404606A4), 15);
                        Game.WriteInt32(new IntPtr(0x1404606A0), 15);
                        break;
                    case 7:
                        //online
                        Game.WriteInt32(new IntPtr(0x1404606A4), 13);
                        Game.WriteInt32(new IntPtr(0x1404606A0), 13);
                        break;
                    case 8:
                        // nd + vs
                        Game.WriteInt32(new IntPtr(0x140598BB8), 1);
                        Game.WriteInt32(new IntPtr(0x140598BB0), 0);
                        break;
                }
                Thread.Sleep(1);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (worker != null) worker.Abort();
        }

        public bool InPreGame()
        {
            Game.TrustProcess = true;
            long boffset = Game.ReadInt32(new IntPtr(0x1405989B0));
            if (boffset == 0)
            {
                return false;
            }
            bool ret = Game.ReadByte((IntPtr)boffset + 0x9A) == 0x0
                && (Game.ReadByte(new IntPtr(0x14046069C)) == 7) || (Game.ReadByte(new IntPtr(0x14046069C)) == 8); //Check for specific scenario that makes sure you are in the right position for gamemode to be forced without crash
            Game.TrustProcess = false;
            return ret;
        }
    }
}
