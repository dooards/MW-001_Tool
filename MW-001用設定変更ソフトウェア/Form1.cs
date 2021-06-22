//*********************************************************
//
// MW-001用設定変更ソフトウェア
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Management;
using System.Diagnostics;


namespace MW_001用設定変更ソフトウェア
{
    public partial class Form1 : Form
    {
        string[] ports;
        string[] portNames;
        string[] Rxline;
        string tellnum;
        string filePath;
        string dataIN;
        string appPath;
        bool TestReading; //読込中 Rx
        bool TestWriting;　//書込中 Tx
        bool Findcom;
        bool END;
        bool Tout; //タイムインフラグ
        bool CITY;
        bool TERM;
        bool ATCH;　//タイムインフラグ2
        bool Retry;
        DateTime startDT;
        StreamWriter LOG;
        StreamReader SRead;

        //プログラム起動 0
        public Form1()
        {
            InitializeComponent();
            testReset();
            DateTime start_tool = DateTime.Now;
            string dtn = start_tool.ToString("yyyyMMdd");
            LOG = new StreamWriter(dtn + ".log", true, System.Text.Encoding.Default);
            LOG.WriteLine(Environment.NewLine + start_tool);
        }

        //1.フォーム起動
        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
        //2.フォーム起動時のファイル検索
        private void Form1_Shown(object sender, EventArgs e)
        {
            panel3.Visible = false;
            textBox_city.Clear();
            textBox_num.Clear();

            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;

            //IDファイルの取得
            appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            appPath = appPath.Replace("MW-001用設定変更ソフトウェア.exe", "");
            Console.WriteLine(appPath + Environment.NewLine);
            string[] CSVfiles = System.IO.Directory.GetFiles(@appPath, "*.csv", System.IO.SearchOption.AllDirectories);

            
            toolStripStatusLabel1.Text = "水位計IDファイルを読込んでいます。";
            this.Update();
            System.Threading.Thread.Sleep(500);
            if (CSVfiles.Length == 1)
            {
                filePath = CSVfiles[0];
                LOG.WriteLine(filePath);
                textBox_csv.Text = Path.GetFileName(CSVfiles[0]);
                toolStripProgressBar1.Value = 5; //action-0
                toolStripStatusLabel1.Text = "水位計IDファイルが読込まれました。";
                this.Update();
                System.Threading.Thread.Sleep(1000);
                PortSearch();
            }
            else
            {
                ForErrorStop("水位計IDファイルが不正です。", 0, false, false);
                return;
            }
        }

        //3.COMポート検索
        private void PortSearch()
        {
            toolStripStatusLabel1.Text = "ケーブルのUSB端をパソコンへ接続して下さい。";
            this.Update();
            System.Threading.Thread.Sleep(500);

            LOG.WriteLine(toolStripStatusLabel1.Text); //ケーブル差し込み前
            do
            {
                this.Update();
                Application.DoEvents();
                if (END == true)
                {
                    this.Close();
                    break;
                }
                statusStrip1.Update();

                //使用可能なCOMポートをコンボボックスへ表示
                ports = SerialPort.GetPortNames();

                if (ports.Length > 0)
                {
                    GetDeviceNames();
                    portNames = GetDeviceNames();
                    comboBox_com.Items.Clear();
                    for (int i = 0; i < portNames.Length; i++)
                    {
                        comboBox_com.Items.Add(portNames[i].Substring(0, portNames[i].IndexOf("(")));
                        if (portNames[i].StartsWith("USB Serial Port"))
                        {
                            comboBox_com.SelectedIndex = i;
                            Findcom = true;
                        }
                    }
                    if (Findcom == true)
                    {
                        //次のアクションの表示
                        button_connect.Enabled = true;
                        toolStripProgressBar1.Value = 10; //action-1
                        toolStripStatusLabel1.Text = "ケーブルを選択し、接続ボタンを押して下さい。";
                        button_connect.Focus();
                        LOG.WriteLine(toolStripStatusLabel1.Text);　//ケーブル差し込み済
                    }
                    else
                    {
                        comboBox_com.Items.Clear();
                        comboBox_com.Items.Add(""); 
                    }
                    System.Threading.Thread.Sleep(250);
                }
            } while (Findcom == false); //ports.Length == 0
        }

        //0.終了ボタン
        private void button_end_Click(object sender, EventArgs e)
        {
            END = true;
            TestReading = false;
            TestWriting = false;

            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
            LOG.WriteLine("終了ボタン");
            LOG.Close();
            Application.Exit();
            this.Close();
        }

        //4.接続ボタン
        private void button_connect_Click(object sender, EventArgs e)
        {
            if (Findcom == true)
            {
                try
                {
                    toolStripStatusLabel1.Text = "接続中";
                    this.Update();

                    //serialport設定
                    string strValue = portNames[comboBox_com.SelectedIndex];
                    strValue = strValue.Remove(0, strValue.IndexOf("(") + 1);
                    strValue = strValue.Remove(strValue.IndexOf(")"));

                    serialPort1.PortName = strValue;
                    serialPort1.BaudRate = 115200; // Convert.ToInt32(cBoxBAUDRATE.Text);
                    serialPort1.DataBits = 8; // Convert.ToInt32(cBoxDATABITS.Text);
                    serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One");
                    serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), "None");
                    //serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

                    serialPort1.Open();
                    toolStripStatusLabel1.Text = "接続済み"; 
                    LOG.WriteLine(portNames[comboBox_com.SelectedIndex]);　//COM名
                    LOG.WriteLine(toolStripStatusLabel1.Text);　//ケーブル選択済み

                    //次のステップを表示
                    toolStripProgressBar1.Value = 15; //action-2

                    //次へ遷移
                    next_action();
                }
                catch
                {
                    //COMポート処理中ににエラーがある場合
                    ForErrorStop("ケーブルがパソコンから抜けました。", 0, false, false);
                    this.Update();
                    System.Threading.Thread.Sleep(1000);
                    comboBox_com.Items.Clear();
                    comboBox_com.Text = "";
                    Findcom = false;
                    PortSearch();
                }
            }           
        }

        //7.書込みボタン
        private void button_write_Click(object sender, EventArgs e)
        {
            button_write.Enabled = false;
            TestReading = true;
            TestWriting = true;

            int num = 0;

            try
            {
                if (serialPort1.IsOpen)
                {
                    while (TestWriting == true && END == false)
                    {
                        this.Update();
                        Application.DoEvents();

                        switch (num)
                        {
                            case 1:
                                WRITECITY();
                                IDWrite();
                                break;

                            case 2:
                                WRITESENS();
                                IDWrite();
                                break;

                            case 3:
                                CHECKID();
                                IDWrite();
                                break;

                            case 4:
                                ATTACH();
                                IDWrite();
                                if(Retry == true)
                                {
                                    ATTACH();
                                    IDWrite();
                                }
                                break;

                            case 5:
                                //終了処理
                                ForErrorStop("水位計IDが書き込まれました。 アプリを終了して下さい。", 100, false, false);
                                button_end.Focus();
                                break;
                        }
                        num++;
                    }
                }
                else
                {
                    //ケーブル抜け　書込み前
                    ForErrorStop("途中停止しました(3)。アプリを一度閉じて最初から実施して下さい。", 15, false, false);
                    toolStripProgressBar1.Value = 20; //action-3
                    return;
                }
            }
            catch
            {
                //ケーブル抜け　書込み中
                ForErrorStop("途中停止しました(4)。アプリを一度閉じて最初から実施して下さい。", 15, false, false);
                toolStripProgressBar1.Value = 20; //action-3
                return;

            }
        }

        //5.次への関数
        private void next_action()
        {
            if (panel1.Visible == true)
            {
                panel1.Visible = false;
                panel3.Visible = true;

                if (serialPort1.IsOpen)
                {
                    toolStripStatusLabel1.Text = "水位計の所定の位置に磁石を近づけて、電源を入れてください。";
                }
                //COMポート受信開始
                TestReading = true; //受信開始FLAG
                TestRead();
                //電話番号を表示
                textBox_tell.Text = tellnum;
            }
        }


        //6-2.電話番号取得時処理
        private void textBox_tell_TextChanged(object sender, EventArgs e)
        {
            textBox_city.Clear();
            textBox_num.Clear();

            if(textBox_tell.Text.Length == 11)
            {
                try
                {
                    SRead = new StreamReader(@filePath, Encoding.Default);

                    string dat;
                    while ((dat = SRead.ReadLine()) != null)
                    {
                        string callnum;
                        string[] sbuf = dat.Split(',');
                        callnum = sbuf[0];

                        if (textBox_tell.Text == callnum)
                        {
                            textBox_city.Text = sbuf[1];
                            textBox_num.Text = sbuf[2];
                            toolStripProgressBar1.Value = 40; //action-8

                            toolStripStatusLabel1.Text = "IDの書込みが可能です。";

                            //水位計ID書込み可
                            button_write.Enabled = true;
                            TestWriting = true;
                            break;
                        }
                    }
                    SRead.Close();

                    if(TestWriting == false)
                    {
                        ForErrorStop("このSIMは登録がありません(1)。アプリを一度閉じて最初からやり直して下さい。", 15, false, false);
                        SRead.Close();
                        return;
                    }
                    else
                    {
                        //log
                        LOG.WriteLine(textBox_city.Text);
                        LOG.WriteLine(textBox_num.Text);
                        LOG.WriteLine(toolStripStatusLabel1.Text); //起動済み
                    }
                }
                catch
                {
                    ForErrorStop("このSIMは登録がありません(2)。アプリを一度閉じて最初からやり直して下さい。", 15, false, false);
                    return;
                }
            }
        }



        //0.リセット処理
        private void testReset()
        {
            //初期値
            toolStripProgressBar1.Value = 0; //action-0

            button_connect.Text = "接続";
            button_connect.Enabled = false;
            button_write.Enabled = false;
            comboBox_com.Items.Clear();

            TestReading = false;
            TestWriting = false;
            Findcom = false;
            END = false;
            Tout = false;
            CITY = false;
            TERM = false;
            ATCH = false;
            Retry = false;
        }

        
        //0.エラー時処理
        private void ForErrorStop(string Mess, int barnum, bool rx, bool tx)
        {
            TestReading = rx;
            TestWriting = tx;
            toolStripProgressBar1.Value = barnum;
            toolStripStatusLabel1.Text = Mess;
            LOG.WriteLine(toolStripStatusLabel1.Text);
        }

        //6.UART受信処理　BOOT時
        private void TestRead()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();

                try
                {
                    while (TestReading == true && END == false)
                    {
                        this.Activate();
                        button_end.Focus();
                        this.Update();
                        Application.DoEvents();
                        if (Tout == true) //タイマーが起動中
                        {
                            DateTime endDT = DateTime.Now;
                            TimeSpan ts = endDT - startDT;
                            //Console.WriteLine(ts);

                            if (ts.TotalSeconds > 60)
                            {
                                //タイムアウトした
                                ForErrorStop("起動できませんでした。アプリを一度閉じて最初から実施して下さい。", 15, false, false);
                                return;
                            }
                        }
                        if (serialPort1.BytesToRead > 0)
                        {
                            dataIN = serialPort1.ReadExisting();
                            Rxline = dataIN.Split('\n');
                            this.Invoke(new EventHandler(SerialLog));
                        }
                    } 
                }
                catch
                {
                    if(END != true)
                    {
                        //ケーブルが抜け　起動中
                        ForErrorStop("途中停止しました(1)。アプリを一度閉じて最初から実施して下さい。", 0, false, false);
                    }
                    return;
                }
                Tout = false;
            }
            else
            {
                //ケーブルが抜け　ファイル読み込み後
                ForErrorStop("途中停止しました(2)。アプリを一度閉じて最初から実施して下さい。", 0, false, false);
                return;
            }
        }

        //7-1.UART受信処理　書込み時
        private void IDWrite()
        {

            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();

                try
                {
                    while (TestReading == true && END == false)
                    {
                        this.Update();
                        Application.DoEvents();

                        if (Tout == true)
                        {
                            DateTime endDT = DateTime.Now;
                            TimeSpan ts = endDT - startDT;
                            //Console.WriteLine(ts);

                            if (ts.TotalSeconds > 10)
                            {
                                string eName = "書込みタイムアウト　アプリを閉じて最初から実施して下さい。";
                                ForErrorStop(eName, 40, false, false);
                                this.Update();
                                return;
                            }
                        }
                        if (ATCH == true)
                        {
                            DateTime endDTATCH = DateTime.Now;
                            TimeSpan ts = endDTATCH - startDT;
                            //Console.WriteLine(ts);

                            if (ts.TotalSeconds > 30)
                            {
                                if(Retry == true)
                                {
                                    toolStripStatusLabel1.Text = "LTE接続テスト失敗(1) アプリを閉じて最初から実施して下さい。";
                                    LOG.WriteLine(toolStripStatusLabel1.Text);
                                    this.Update();
                                    ATCH = false;
                                    TestReading = false;
                                    TestWriting = false;
                                    return;
                                }
                                else if(Retry == false)
                                {
                                    toolStripStatusLabel1.Text = "もう一度LTE接続テストを実施します。";
                                    LOG.WriteLine(toolStripStatusLabel1.Text);
                                    this.Update();
                                    System.Threading.Thread.Sleep(500);
                                    Retry = true;
                                    ATCH = false;
                                    TestReading = false;
                                    return;
                                }
                            }
                        }

                        if (serialPort1.BytesToRead > 0)
                        {
                            dataIN = serialPort1.ReadExisting();
                            Rxline = dataIN.Split('\n');
                            this.Invoke(new EventHandler(WriteLog));
                        }
                    }
                }
                catch
                {
                    if (END != true)
                    {
                        //ケーブルが抜け　起動中
                        ForErrorStop("途中停止しました(3)。アプリを閉じて最初から実施して下さい。", 0, false, false);
                    }
                    return;
                }
                Tout = false;
                ATCH = false;
            }
            else
            {
                //ケーブルが抜け　ファイル読み込み後
                ForErrorStop("途中停止しました(3)。アプリを一度閉じて最初から実施して下さい。", 0, false, false);
                return;
            }
        }

        //6-1.Boot時　合否判定
        private void SerialLog(object sender, EventArgs e)
        {
            foreach (string s in Rxline)
            {
                if(s.Contains("LTE_power off"))
                {
                    ForErrorStop("磁石が反応してません。このまま電源を入れ直して下さい。", 20, true, false);
                    return; //受信待ち継続
                }

                if(s.Contains("START TEST"))
                {
                    toolStripProgressBar1.Value = 25; //action-5
                    startDT = DateTime.Now; //時間取得
                    Console.WriteLine(startDT);
                    Tout = true; //タイマー起動
                    toolStripStatusLabel1.Text = "起動開始 [起動完了まで34秒] [強制終了60秒]";
                    LOG.WriteLine(toolStripStatusLabel1.Text);　//テストモード起動確認
                    return;
                }

                if (s.Contains("WAKEUP"))
                {
                    toolStripProgressBar1.Value = 30; //action-6
                    toolStripStatusLabel1.Text = "起動中....";
                    LOG.WriteLine(toolStripStatusLabel1.Text);　//テストモード起動中
                    return;
                }

                if (s.Contains("NUM=0"))
                {
                    int len = s.Length;
                    if (len < 15)
                    {
                        ForErrorStop("SIMが読めません。アプリを一度閉じて最初から実施して下さい。", 15, false, false);
                        break;　//終わり
                    }
                    else
                    {
                        toolStripProgressBar1.Value = 35; //action-7
                        toolStripStatusLabel1.Text = "電話番号取得";
                        System.Threading.Thread.Sleep(500);
                        this.Update();

                        //電話番号
                        tellnum = s.Substring(len - 11);
                        Console.WriteLine(tellnum + Environment.NewLine);

                        //log
                        LOG.WriteLine(tellnum);

                        //次へ進む
                        TestReading = false;
                        next_action();
                        return;
                    }
                }
            }
        }

        //7-2.書込み時　合否判定
        private void WriteLog(object sender, EventArgs e)
        {
            foreach (string s in Rxline)
            {
                if (s.StartsWith("!!CITYCODE"))
                {
                    CITY = true;
                    return;
                }
                if (s.StartsWith("!!SENSORNO"))
                {
                    TERM = true;
                    return;
                }
                if (s.StartsWith("!!ATTACH"))
                {
                    toolStripStatusLabel1.Text = "LTE接続テスト開始 [強制終了30秒]";
                    this.Update();
                    return;
                }
                if (s.StartsWith("OK"))
                {
                    if (CITY == true)
                    {
                        toolStripProgressBar1.Value = 50; //action-9
                        toolStripStatusLabel1.Text = "市町村コード書込";
                        LOG.WriteLine(toolStripStatusLabel1.Text); //CITYCODE
                        this.Update();
                        System.Threading.Thread.Sleep(500);
                        CITY = false;
                        TestReading = false;
                        return;
                    }
                    else if (TERM == true)
                    {
                        toolStripProgressBar1.Value = 60; //action-10
                        toolStripStatusLabel1.Text = "水位計番号書込";
                        LOG.WriteLine(toolStripStatusLabel1.Text); //SENSORNO
                        this.Update();
                        System.Threading.Thread.Sleep(500);
                        TERM = false;
                        TestReading = false;
                        return;
                    }
                }
                if (s.StartsWith("CITYCODE="))
                {
                    string WORD;
                    WORD = s.Substring(9);

                    if (WORD == textBox_city.Text)
                    {
                        toolStripProgressBar1.Value = 70; //action-11
                        toolStripStatusLabel1.Text = "市町村コード確認";
                        LOG.WriteLine(toolStripStatusLabel1.Text); //INFO
                        this.Update();
                        System.Threading.Thread.Sleep(500);
                        TestReading = false;
                        return;
                    }
                    else
                    {
                        ForErrorStop("市町村コード確認エラー　アプリを一度閉じて最初から実施して下さい。", 40, false, false);
                        return;
                    }
                }
                if (s.StartsWith("SENSORNO="))
                {
                    string WORD;
                    WORD = s.Substring(9);

                    if (WORD == textBox_num.Text)
                    {
                        toolStripProgressBar1.Value = 80; //action-12
                        toolStripStatusLabel1.Text = "水位計番号確認";
                        LOG.WriteLine(toolStripStatusLabel1.Text); //INFO
                        this.Update();
                        System.Threading.Thread.Sleep(500);
                        TestReading = false;
                        return;
                    }
                    else
                    {
                        ForErrorStop("水位計番号確認エラー　アプリを一度閉じて最初から実施して下さい。", 40, false, false);
                        return;
                    }
                }
                if (s.StartsWith("ATTACH"))
                {
                    string WORD;
                    WORD = s;

                    if (s.Contains("OK")) //(WORD == "ATTACH OK")
                    {
                        toolStripProgressBar1.Value = 90; //action-13
                        toolStripStatusLabel1.Text = "LTE接続テスト合格";
                        Console.WriteLine(WORD + Environment.NewLine);
                        this.Update();
                        LOG.WriteLine(toolStripStatusLabel1.Text);　//ATTACH
                        System.Threading.Thread.Sleep(750);
                        TestReading = false;
                        return;
                    }
                    else if (s.Contains("ERROR"))  //(WORD == "ATTACH ERROR")
                    {
                        if (Retry == true)
                        {
                            string eName = "LTE接続テスト失敗(1)　アプリを一度閉じて最初から実施して下さい。";
                            ForErrorStop(eName, 40, false, false);
                            return;
                        }
                        else if (Retry == false)
                        {
                            toolStripStatusLabel1.Text = "もう一度LTE接続テストを実施します。";
                            LOG.WriteLine(toolStripStatusLabel1.Text);
                            this.Update();
                            System.Threading.Thread.Sleep(500);
                            Retry = true;
                            return;
                        }

                    }
                }
            }
        }



        //7-3.テストコマンド　!!ATTACH
        private void ATTACH()
        {
            TestReading = true;
            string ATT;
            ATT = "!!ATTACH" + Environment.NewLine;
            Console.WriteLine(ATT);
            serialPort1.WriteLine(ATT);
            ATCH = true;
            startDT = DateTime.Now;
        }

        //テストコマンド　!!INFO
        private void CHECKID()
        {
            TestReading = true;
            string INFO;
            INFO = "!!INFO" + Environment.NewLine;
            Console.WriteLine(INFO);
            serialPort1.WriteLine(INFO);
            Tout = true;
            startDT = DateTime.Now;
        }

        //テストコマンド　!!SENSNO
        private void WRITESENS()
        {
            TestReading = true;
            string SENSORCODE;
            SENSORCODE = "!!SENSORNO=" + textBox_num.Text + Environment.NewLine;
            Console.WriteLine(SENSORCODE);
            serialPort1.WriteLine(SENSORCODE);
            Tout = true;
            startDT = DateTime.Now;
        }

        //テストコマンド　!!CITYID
        private void WRITECITY()
        {
            TestReading = true;
            string CITYCODE;
            CITYCODE = "!!CITYCODE=" + textBox_city.Text + Environment.NewLine;
            Console.WriteLine(CITYCODE);
            serialPort1.WriteLine(CITYCODE);
            Tout = true;
            startDT = DateTime.Now;
        }

        //2-1.COMPortのDeviceID取得
        public static string[] GetDeviceNames()
        {

            var deviceNameList = new System.Collections.ArrayList();
            var check = new System.Text.RegularExpressions.Regex("(COM[1-9][0-9]?[0-9]?)");

            ManagementClass mclass = new ManagementClass("Win32_PnPEntity");
            ManagementObjectCollection manageObjCol = mclass.GetInstances();

            //全てのPnPデバイスを探索しシリアル通信が行われるデバイスを随時追加する
            foreach (ManagementObject manageObj in manageObjCol)
            {
                //Nameプロパティを取得
                var namePropertyValue = manageObj.GetPropertyValue("Name");
                if (namePropertyValue == null)
                {
                    continue;
                }

                //Nameプロパティ文字列の一部が"(COM1)～(COM999)"と一致するときリストに追加"
                string name = namePropertyValue.ToString();
                if (check.IsMatch(name))
                {
                    deviceNameList.Add(name);
                }
            }

            //戻り値作成
            if (deviceNameList.Count > 0)
            {
                string[] deviceNames = new string[deviceNameList.Count];
                int index = 0;
                foreach (var name in deviceNameList)
                {
                    string dev = name.ToString();
                    //dev = dev.Substring(0, dev.IndexOf("("));
                    deviceNames[index++] = dev;
                }
                return deviceNames;
            }
            else
            {
                return null;
            }
        }

        //0.バージョン表示
        private void ヘルプHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MW-001用設定変更ソフトウェア　1.00版" + Environment.NewLine + "Copyright ABIT Co.");
        }

    }
}
