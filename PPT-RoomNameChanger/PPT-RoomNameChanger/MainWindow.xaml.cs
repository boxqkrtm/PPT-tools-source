using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Windows.Threading;




namespace PPT_RoomNameChanger
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread worker = null;
        int loopc = 1;
        int customft = 100;
        ProcessMemory Game;
        public MainWindow()
        {
            InitializeComponent();
            worker = new Thread(() => Run(0));
            worker.Start();

            a5.Click += radioButtons_CheckedChanged;
            a10.Click += radioButtons_CheckedChanged;
            a15.Click += radioButtons_CheckedChanged;
            a20.Click += radioButtons_CheckedChanged;
            a25.Click += radioButtons_CheckedChanged;
            a30.Click += radioButtons_CheckedChanged;
            a50.Click += radioButtons_CheckedChanged;
            a100.Click += radioButtons_CheckedChanged;
        }

        #region
        public bool IsNumeric(string source)
        {
            Regex regex = new Regex("[^0-9.-]+");
            return !regex.IsMatch(source);
        }
        public void OnTextBoxPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                e.Handled = !IsNumeric(e.Text);
            }
            catch { 
            }
        }
        #endregion

        public void radioButtons_CheckedChanged(object sender, EventArgs e)
        {
            applyFT();
        }

        public void applyFT() {
            //RadioButton radioButton = sender as RadioButton;
            int ft = 1;
            if (a5.IsChecked == true)
            {
                ft = 5;
            }
            else if (a10.IsChecked == true)
            {
                ft = 10;
            }
            else if (a15.IsChecked == true)
            {
                ft = 15;
            }
            else if (a20.IsChecked == true)
            {
                ft = 20;
            }
            else if (a25.IsChecked == true)
            {
                ft = 25;
            }
            else if (a30.IsChecked == true)
            {
                ft = 30;
            }
            else if (a50.IsChecked == true)
            {
                ft = 50;
            }
            else if (a100.IsChecked == true)
            {
                ft = customft;
            }

            Game = new ProcessMemory("puyopuyotetris");
            Game.WriteInt32(new IntPtr(
            Game.ReadInt32(new IntPtr(
                Game.ReadInt32(new IntPtr(
                     Game.ReadInt32(new IntPtr(
                            Game.ReadInt32(new IntPtr(
                                Game.ReadInt32(new IntPtr(
                                    0x140460690
                                )) + 0x20
                            )) + 0x20
                        )) + 0x20
                    )) + 0x610
                )) + 0x4), ft);
        }



           private void Run(int val)
        {
            while (true)
            {
                if (loopc == 0)
                {
                    Game = new ProcessMemory("puyopuyotetris");
                    var roomtitle = Game.ReadStringUnicode(new IntPtr(
                               Game.ReadInt32(new IntPtr(
                                    Game.ReadInt32(new IntPtr(
                                         Game.ReadInt32(new IntPtr(
                                                Game.ReadInt32(new IntPtr(
                                                    Game.ReadInt32(new IntPtr(
                                                        0x140460690
                                                    )) + 0x20
                                                )) + 0x20
                                            )) + 0x20
                                        )) + 0x610
                                    )) + 0x4
                            + 0xAC), 32);
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        //사용할 메서드 및 동작
                        
                    textBox1.Text = ("" + Regex.Replace(roomtitle, @"\0+", ""));
                    }));
                }
                Thread.Sleep(1000);
                loopc -= 1;
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //if (worker != null) 
            worker.Abort();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Game = new ProcessMemory("puyopuyotetris");
            Game.WriteStringUnicode(new IntPtr(
            Game.ReadInt32(new IntPtr(
                Game.ReadInt32(new IntPtr(
                     Game.ReadInt32(new IntPtr(
                            Game.ReadInt32(new IntPtr(
                                Game.ReadInt32(new IntPtr(
                                    0x140460690
                                )) + 0x20
                            )) + 0x20
                        )) + 0x20
                    )) + 0x610
                )) + 0x4
                + 0xAC),
                "\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0\0");
            Game.WriteStringUnicode(new IntPtr(
            Game.ReadInt32(new IntPtr(
                Game.ReadInt32(new IntPtr(
                     Game.ReadInt32(new IntPtr(
                            Game.ReadInt32(new IntPtr(
                                Game.ReadInt32(new IntPtr(
                                    0x140460690
                                )) + 0x20
                            )) + 0x20
                        )) + 0x20
                    )) + 0x610
                )) + 0x4
                + 0xAC),
                textBox1.Text);
            loopc = 30;
        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            try
            {
                customft = int.Parse(ftbox.Text);
            }
            catch {
                customft = 1;
            }
            if (customft > 255) {
                customft = 255;
                ftbox.Text = "255";
            }
            if (customft <= 0) {
                customft = 1;
                ftbox.Text = "1";
            }
            applyFT();
        }




    }
}

