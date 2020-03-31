using System;
using System.Collections.Generic;
using System.IO;
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

namespace PPT_user_filter
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        int selecteduser = 1;//1 right
        long hostid = 0;
        long clientid = 0;
        string hostname = "";
        string clientname = "";
        Thread worker = null;
        public MainWindow()
        {
            InitializeComponent();
            worker = new Thread(() => Run());
            worker.Start();
        }

        public void Run() {
            while (true) {
                string taghost = "";
                string tagclient = "";
                getGameData();
                //load db
                FileStream fs = new FileStream("list.txt", FileMode.OpenOrCreate);
                StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
                while (!sr.EndOfStream)
                {
                    string s = sr.ReadLine();
                    string[] temp = s.Split(',');
                    try
                    {
                        if (hostid == long.Parse(temp[1]))
                        {
                            taghost += "" + temp[2];
                        }
                        if (clientid == long.Parse(temp[1]))
                        {
                            tagclient += "" + temp[2];
                        }
                    }
                    catch
                    {
                        //무시
                    }

                }
                sr.Close();
                fs.Close();
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    //사용할 메서드 및 동작
                    leftradio.Content = hostname + "" + taghost;
                    rightradio.Content = clientname + "" + tagclient;
                }));
                
                Thread.Sleep(500);
            }
        }

        /*right*/
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            selecteduser = 1;
        }

        /*left*/
        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            selecteduser = 0;
        }

        //새로고침
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        public void refresh() {
            string taghost = "";
            string tagclient = "";
            getGameData();
            //load db
            FileStream fs = new FileStream("list.txt", FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                string[] temp = s.Split(',');
                try
                {
                    if (hostid == long.Parse(temp[1]))
                    {
                        taghost += "" + temp[2];
                    }
                    if (clientid == long.Parse(temp[1]))
                    {
                        tagclient += "" + temp[2];
                    }
                }
                catch
                {
                    //무시
                }

            }
            sr.Close();
            fs.Close();
            leftradio.Content = hostname + "" + taghost;
            rightradio.Content = clientname + "" + tagclient;
        }

        public void getGameData()
        {
            ProcessMemory Game = new ProcessMemory("puyopuyotetris");

            byte[] hostnametmp;
            hostnametmp = Game.ReadByteArray(new IntPtr(
               Game.ReadInt32(new IntPtr(
                    Game.ReadInt32(new IntPtr(
                             Game.ReadInt32(new IntPtr(
                                Game.ReadInt32(new IntPtr(
                                        0x140473760
                                    )) + 0x20
                                )) + 0x8
                            )) + 0x10
                        )) + 0x80 + 0x58
               ), 28);
            int index = 0;
            int getzeromemory = 0;
            foreach (int element in hostnametmp) {
                if (getzeromemory == 2)
                {
                    hostnametmp[index] = 0;
                }
                else {
                    if (element == 0)
                    {
                        getzeromemory+=1;
                    }
                    else
                    {
                        getzeromemory = 0;
                    }
                }
                index++;
            }
            hostname = Encoding.Unicode.GetString(hostnametmp);
            hostname = Regex.Replace(hostname, @"\0+", "");

            byte[] clientnametmp;
            clientnametmp = Game.ReadByteArray(new IntPtr(
               Game.ReadInt32(new IntPtr(
                    Game.ReadInt32(new IntPtr(
                             Game.ReadInt32(new IntPtr(
                                Game.ReadInt32(new IntPtr(
                                        0x140473760
                                    )) + 0x20
                                )) + 0x8
                            )) + 0x10
                        )) + 0x80 - 0x10 + 0xB8
               ), 28);
            index = 0;
            getzeromemory = 0;
            foreach (int element in clientnametmp)
            {
                if (getzeromemory == 2)
                {
                    clientnametmp[index] = 0;
                }
                else
                {
                    if (element == 0)
                    {
                        getzeromemory += 1;
                    }
                    else
                    {
                        getzeromemory = 0;
                    }
                }
                index++;
            }
            clientname = Encoding.Unicode.GetString(clientnametmp);
            clientname = Regex.Replace(clientname, @"\0+", "");

            clientid = Game.ReadInt64(new IntPtr(
               Game.ReadInt32(new IntPtr(
                   Game.ReadInt32(new IntPtr(
                    Game.ReadInt32(new IntPtr(
                        Game.ReadInt32(new IntPtr(
                                0x140473760
                            )) + 0x20
                        )) + 0x8
                    )) + 0x10
                )) + 0x80
           ));


            hostid = Game.ReadInt64(new IntPtr(
               Game.ReadInt32(new IntPtr(
                   Game.ReadInt32(new IntPtr(
                    Game.ReadInt32(new IntPtr(
                        Game.ReadInt32(new IntPtr(
                                0x140473760
                            )) + 0x20
                        )) + 0x8
                    )) + 0x10
                )) + 0x80 - 0x10
            ));
        }
        

        //open selected steam profile
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            System.Diagnostics.Process.Start("https://steamcommunity.com/profiles/"+steamid);
        }

        public void addtag(long id, String tagname, String nickname)
        {
            //load db
            FileStream fs = new FileStream("list.txt", FileMode.OpenOrCreate);
            StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("UTF-8"));

            //checking if already written.
            int linenumber = 0;
            int isexist = 0;
            while (!sr.EndOfStream)
            {
                string s = sr.ReadLine();
                string[] temp = s.Split(',');
                try
                {
                    if (id == long.Parse(temp[1])){
                        isexist = 1;
                        break;
                    }
                }
                catch
                {
                    //무시
                }
                linenumber += 1;
            }
            sr.Close();
            fs.Close();

            //write
            if (isexist == 1) {
                //
                // Read the old file.
                string[] lines = File.ReadAllLines("list.txt");

                // Write the new file over the old file.
                using (StreamWriter writer = new StreamWriter("list.txt"))
                {
                    for (int currentLine = 1; currentLine <= lines.Length; ++currentLine)
                    {
                        if (currentLine == (linenumber+1))
                        {
                            writer.WriteLine("" + nickname + "," + id + "," + tagname);

                        }
                        else
                        {
                            writer.WriteLine(lines[currentLine - 1]);
                        }
                    }
                    writer.Close();
                }

            }
            else {
                //
                StreamWriter sw = new StreamWriter("list.txt", append: true);
                sw.WriteLine(nickname + "," + id + "," + tagname+"");
                sw.Close();
            }

            refresh();
        }

        public long getSelectSteamId() {
            long steamid = 0;
            switch (selecteduser)
            {
                case 0:
                    //left user
                    steamid = hostid;
                    break;
                case 1:
                    //right user
                    steamid = clientid;
                    break;
            }
            return steamid;
        }

        public String getSelectedNickname()
        {
            String result = "";
            switch (selecteduser)
            {
                case 0:
                    //left user
                    result = hostname;
                    break;
                case 1:
                    //right user
                    result = clientname;
                    break;
            }
            return result;
        }

        private void Custom_Click(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            addtag(steamid, "(" + customtag.Text + ")", getSelectedNickname());
        }

        private void ai_Click(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            addtag(steamid, "(" + "ai" + ")", getSelectedNickname());
        }

        private void lag_Click(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            addtag(steamid, "(" + "lag" + ")", getSelectedNickname());
        }

        private void c4w_Click(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            addtag(steamid, "(" + "c4w" + ")", getSelectedNickname());
        }

        private void normal_Click(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            addtag(steamid, "(" + "normal" + ")", getSelectedNickname());
        }

        private void disconnet_Click(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            addtag(steamid, "(" + "disconnet" + ")", getSelectedNickname());
        }

        private void a4w_Click(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            addtag(steamid, "(" + "4w" + ")", getSelectedNickname());
        }

        private void puyo_Click(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            addtag(steamid, "(" + "puyo" + ")", getSelectedNickname());
        }

        private void tetris_Click(object sender, RoutedEventArgs e)
        {
            long steamid = getSelectSteamId();
            addtag(steamid, "(" + "tetris" + ")", getSelectedNickname());
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (worker != null) worker.Abort();
        }
    }
}
