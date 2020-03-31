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

namespace PPT_core_AI_button
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread worker;
        ProcessMemory Game = new ProcessMemory("puyopuyotetris");
        public MainWindow()
        {
            InitializeComponent();
            worker = new Thread(() => Run(0));
            worker.Start();

        }

        private void Run(int val)
        {
            while (true) { 
            Game.WriteByte(new IntPtr(0x140598C24), 1);
            Game.WriteByte(new IntPtr(0x140598C8C), 1);
            Game.WriteByte(new IntPtr(0x140598CF4), 1);
            Game.WriteByte(new IntPtr(0x140598D5C), 1);
            }
            Thread.Sleep(100);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (worker != null) worker.Abort();
            Game.WriteByte(new IntPtr(0x140598C24), 0);
            Game.WriteByte(new IntPtr(0x140598C8C), 0);
            Game.WriteByte(new IntPtr(0x140598CF4), 0);
            Game.WriteByte(new IntPtr(0x140598D5C), 0);
        }

    }
}
