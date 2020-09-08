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

namespace PPC_ArleSkin
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread worker;
        public MainWindow()
        {
           
            InitializeComponent();
            InitializeComponent();
            worker = new Thread(() => Run());
            worker.Start();
        }
        private void Run()
        {
            while (true)
            {
                ProcessMemory Game = new ProcessMemory("PuyoPuyoChampions");
                Game.WriteInt32(new IntPtr(0x7FF603333AF8), 26);
                Thread.Sleep(10);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (worker != null) worker.Abort();
        }
    }
}