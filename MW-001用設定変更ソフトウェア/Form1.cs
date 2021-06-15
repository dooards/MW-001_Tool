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
        string filePath;
        string dataIN;
        bool TestReading; //起動中
        bool TestWriting;　//確認中
        bool TestSetting;　//書込中
        bool END;
        bool Tout;
        bool CITY;
        bool TERM;
        bool ATCH;
        bool COMREADY;　//COM接続済み
        bool CSVREADY; //CSV読込済み
        bool IDREADY; //全終了
        DateTime startDT;

        StreamWriter LOG;

        public Form1()
        {
            InitializeComponent();
            testReset();
            DateTime start_tool = DateTime.Now;
            string dtn = start_tool.ToString("yyyyMMdd");
            LOG = new StreamWriter(dtn + ".log", true, System.Text.Encoding.Default);
            LOG.WriteLine(start_tool);
        }

        //フォームの起動
        private void Form1_Load_1(object sender, EventArgs e)
        {

        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = false;
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.Update();
            PortSearch();
        }

        //COMポート検索
        private void PortSearch()
        {
            toolStripStatusLabel1.Text = "ケーブルのUSB端をパソコンへ接続して下さい。";
            LOG.WriteLine(toolStripStatusLabel1.Text); //ケーブル差し込み前
            do
            {
                Application.DoEvents();
                if (END == true)
                {
                    Console.WriteLine("END");
                    this.Close();
                    break;
                }
                statusStrip1.Update();

                //使用可能なCOMポートをコンボボックスへ表示
                ports = SerialPort.GetPortNames();
            } while (ports.Length == 0);

            //次のアクションの表示
            if(END == false)
            {
                GetDeviceNames();
                portNames = GetDeviceNames();
                if (portNames != null)
                {
                    foreach (string port in portNames)
                    {
                        Console.WriteLine(port);
                    }
                }
                comboBox_com.Items.AddRange(portNames);

                button_connect.Enabled = true;
                toolStripProgressBar1.Value = 5; //action-1
                toolStripStatusLabel1.Text = "ケーブルを選択し、接続ボタンを押して下さい。";
                LOG.WriteLine(toolStripStatusLabel1.Text);　//ケーブル差し込み済
            }
        }





        //終了ボタン
        private void button_end_Click(object sender, EventArgs e)
        {
            END = true;
            Console.WriteLine("END");
            TestReading = false;

            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

            //testReset();
            LOG.WriteLine("終了ボタン");
            LOG.Close();
            Application.Exit();
            this.Close();
        }

        //接続ボタン
        private void button_connect_Click(object sender, EventArgs e)
        {
            if (COMREADY == false)
            {
                try
                {
                    //serialport設定
                    string strValue = comboBox_com.Text.Remove(0, comboBox_com.Text.IndexOf("(") + 1);
                    strValue = strValue.Remove(strValue.IndexOf(")"));

                    serialPort1.PortName = strValue;
                    serialPort1.BaudRate = 115200; // Convert.ToInt32(cBoxBAUDRATE.Text);
                    serialPort1.DataBits = 8; // Convert.ToInt32(cBoxDATABITS.Text);
                    serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One");
                    serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), "None");
                    //serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

                    serialPort1.Open();
                    COMREADY = true;　//接続中FLAG
                    toolStripStatusLabel1.Text = "接続済み"; 
                    LOG.WriteLine(comboBox_com.Text);　//COM
                    LOG.WriteLine(toolStripStatusLabel1.Text);　//ケーブル選択済み

                    //次のステップを表示
                    toolStripProgressBar1.Value = 10; //action-2

                    //次へ遷移
                    next_action();

                }
                catch
                {
                    //COMポート処理中ににエラーがある場合
                    COMREADY = false;
                    ForPushReset("ケーブルがありません。(1)");

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);
                    comboBox_com.Items.Clear();
                    comboBox_com.Text = "";
                    PortSearch();
                }
            }           
        }

        //CSVファイルボタン
        private void button_file_Click(object sender, EventArgs e)
        {
            TestReading = false;

            OpenFileDialog SF = new OpenFileDialog();

            SF.FileName = "*.csv";
            //SF.InitialDirectory = @"C\:";
            SF.Filter = "CSVファイル|*.csv";
            SF.FilterIndex = 1;
            SF.Title = "水位計一覧ファイルを選択して下さい。";
            SF.RestoreDirectory = true;
            SF.CheckFileExists = true;
            SF.CheckPathExists = true;


            if (SF.ShowDialog() == DialogResult.OK)
            {
                CSVREADY = true;
                textBox_csv.Text = Path.GetFileName(SF.FileName);
                filePath = SF.FileName;
                toolStripProgressBar1.Value = 15; //action-3
                toolStripStatusLabel1.Text = "水位計をテストモードで起動して下さい。";

                //log
                LOG.WriteLine(SF.FileName);

                //END button true
                button_end.Enabled = true;

                //COMポート受信開始
                TestReading = true; //受信開始FLAG
                TestRead();
            }
            else
            {
                textBox_csv.Text = "";
                TestReading = false;
                toolStripStatusLabel1.Text = "水位計一覧ファイルを選択してください。";
                return;
            }
        }

        //書込みボタン
        private void button_write_Click(object sender, EventArgs e)
        {
            button_write.Enabled = false;
            button_write.Text = "書込み中";
            TestReading = false;
            TestSetting = true;
            Tout = true;
            startDT = DateTime.Now;

            int num = 0;

            try
            {
                if (serialPort1.IsOpen)
                {
                    while (TestSetting == true)
                    {
                        this.Activate();
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
                                break;

                            case 5:
                                //終了処理
                                TestWriting = false;
                                TestSetting = false;
                                IDREADY = true;

                                


                                toolStripProgressBar1.Value = 100;　//action-14
                                button_write.Text = "書込み済";
                                toolStripStatusLabel1.Text = "水位計ID書込み完了 アプリを終了して下さい。";
                                // toolStripStatusLabel1.Update();

                                //log
                                LOG.WriteLine(toolStripStatusLabel1.Text);

                                COMREADY = true;
                                break;

                        }
                        num = num + 1;

                    }
                    if (IDREADY == false)
                    {
                        toolStripStatusLabel1.Text = "ID書込か動作確認で失敗しました。最初から実施して下さい。";
                        toolStripProgressBar1.Value = 15; //action-3

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        button_write.Enabled = true;
                        return;
                    }

                }
                else
                {
                    //ケーブル抜け　書込み前
                    TestReading = false;
                    COMREADY = false;
                    ForPushReset("途中停止しました(3)。最初から実施して下さい。");
                    toolStripProgressBar1.Value = 15; //action-3
                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);
                }
            }
            catch
            {
                //ケーブル抜け　書込み中
                TestReading = false;
                COMREADY = false;
                ForPushReset("途中停止しました(4)。最初から実施して下さい。");
                toolStripProgressBar1.Value = 15; //action-3
                //log
                LOG.WriteLine(toolStripStatusLabel1.Text);

            }


        }

        //次への関数
        private void next_action()
        {
            if (panel1.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = true;
                panel3.Visible = false;

                if (serialPort1.IsOpen)
                {
                    if (CSVREADY == false)
                    {
                        toolStripStatusLabel1.Text = "接続完了 水位計一覧ファイルを選択して下さい。";

                    }
                }
                return;
            }
            if (panel2.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = true;

                //表示切替用コピー
                textBox_tell.ResetText();
                textBox_tell.Text = textBox_tell2.Text;
                return;
            }
        }


        //電話番号取得時処理
        private void textBox_tell_TextChanged(object sender, EventArgs e)
        {
            textBox_city.Clear();
            textBox_num.Clear();

            try
            {
                StreamReader sr = new StreamReader(filePath, Encoding.Default);

                string dat;
                while ((dat = sr.ReadLine()) != null)
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
                        // toolStripStatusLabel1.Update();

                        //水位計ID書込み可
                        button_write.Enabled = true;
                        TestWriting = true;
                        break;
                    }
                    else
                    {
                        button_write.Enabled = false;
                    }
                }
                sr.Close();

                //log
                LOG.WriteLine(textBox_city.Text);
                LOG.WriteLine(textBox_num.Text);
                LOG.WriteLine(toolStripStatusLabel1.Text); //起動済み
            }
            catch
            {
                toolStripStatusLabel1.Text = "このSIMは登録がありません。最初からやり直して下さい。";
                toolStripProgressBar1.Value = 15; //action-3
                button_write.Enabled = false;
                return;
            }
        }



        //リセット処理
        private void testReset()
        {
            //初期値
            toolStripProgressBar1.Value = 0; //action-0

            button_connect.Text = "接続";
            button_connect.Enabled = false;
            button_write.Enabled = false;

            textBox_tell2.ResetText();
            comboBox_com.Items.Clear();

            TestReading = false;
            TestWriting = false;
            TestSetting = false;
            END = false;
            Tout = false;
            CITY = false;
            TERM = false;
            ATCH = false;
            COMREADY = false;
            IDREADY = false;
        }

        
        //リセット前処理
        private void ForPushReset(string Mess)
        {
            toolStripStatusLabel1.Text = Mess;
           // toolStripStatusLabel1.Update();
           // toolStripProgressBar1.Value = 0;
            button_connect.Enabled = false;
            TestReading = false;
            button_write.Enabled = false;
            button_connect.Enabled = false;
        }

        //UART受信処理　BOOT時
        private void TestRead()
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();

                try
                {
                    while (TestReading == true)
                    {
                        this.Activate();
                        Application.DoEvents();

                        if(END == true)
                        {
                            break;
                        }

                        if (Tout == true)
                        {
                            DateTime endDT = DateTime.Now;
                            TimeSpan ts = endDT - startDT;

                            if (ts.TotalSeconds > 60)
                            {
                                //タイムアウトした
                                toolStripStatusLabel1.Text = "起動できませんでした。最初から実施して下さい。";

                                //log
                                LOG.WriteLine(toolStripStatusLabel1.Text);

                                Tout = false;
                                TestReading = false;

                                toolStripProgressBar1.Value = 15; //action-3

                                //COMポート受信開始
                                //TestReading = true;
                                //TestRead();
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
                        ForPushReset("途中停止しました(1)。最初から実施して下さい。");

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);
                        toolStripProgressBar1.Value = 0; //action-0
                    }
                    COMREADY = false;
                    return;
                }
                Tout = false;
            }
            else
            {
                //ケーブルが抜け　ファイル読み込み後
                ForPushReset("途中停止しました(2)。最初から実施して下さい。");

                //log
                LOG.WriteLine(toolStripStatusLabel1.Text);
                toolStripProgressBar1.Value = 0; //action-0

                COMREADY = false;
                return;
            }
        }

        //UART受信処理　書込み時
        private void IDWrite()
        {

            if (serialPort1.IsOpen)
            {

                while (TestWriting == true)
                {
                    this.Activate();
                    Application.DoEvents();

                    if (Tout == true)
                    {
                        DateTime endDT = DateTime.Now;
                        TimeSpan ts = endDT - startDT;

                        if (ts.TotalSeconds > 20)
                        {
                            toolStripStatusLabel1.Text = "書込みできませんでした。最初から実施して下さい。";
                            toolStripProgressBar1.Value = 15; //action-3

                            //log
                            LOG.WriteLine(toolStripStatusLabel1.Text);

                            Tout = false;
                            TestWriting = true;　//

                            toolStripProgressBar1.Value = 50;
                            button_write.Enabled=true;

                            return;
                        }
                    }

                    if (ATCH == true)
                    {
                        DateTime endDTATCH = DateTime.Now;
                        TimeSpan ts = endDTATCH - startDT;

                        if (ts.TotalSeconds > 20)
                        {
                            toolStripStatusLabel1.Text = "LTE接続テスト失敗(1)　最初から実施して下さい。";
                            toolStripProgressBar1.Value = 15; //action-3

                            //log
                            LOG.WriteLine(toolStripStatusLabel1.Text);

                            ATCH = false;
                            TestWriting = false;
                            TestSetting = false;
                            return;

                        }

                    }

                    try
                    {
                        if (serialPort1.BytesToRead > 0)
                        {
                            dataIN = serialPort1.ReadExisting();
                            Rxline = dataIN.Split('\n');
                            this.Invoke(new EventHandler(WriteLog));
                        }
                    }
                    catch
                    {

                        COMREADY = false;
                        button_write.Enabled = false;
                        ForPushReset("書き込みエラー　最初から実施して下さい。");
                        toolStripProgressBar1.Value = 15; //action-3

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                    }
                }
                Tout = false;
            }
        }

        //Boot時　合否判定
        private void SerialLog(object sender, EventArgs e)
        {
            foreach (string s in Rxline)
            {
                if(s.Contains("LTE_power off"))
                {
                    toolStripProgressBar1.Value = 20; //action-4
                    toolStripStatusLabel1.Text = "テストモード起動失敗　すぐ装置を再起動して下さい]";

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    return; //受信待ち継続
                }

                if(s.Contains("START TEST"))
                {
                    button_file.Enabled = false;
                    toolStripProgressBar1.Value = 25; //action-5
                    startDT = DateTime.Now;
                    Tout = true;
                    toolStripStatusLabel1.Text = "起動開始　[タイムアウト60秒]";
                    LOG.WriteLine(toolStripStatusLabel1.Text);　//テストモード起動確認

                    return;
                }

                if (s.Contains("WAKEUP"))
                {
                    toolStripProgressBar1.Value = 30; //action-6
                    toolStripStatusLabel1.Text = "起動中......";
                    LOG.WriteLine(toolStripStatusLabel1.Text);　//テストモード起動中
                    return;
                }

                if (s.Contains("NUM=0"))
                {
                    int len = s.Length;

                    if (len < 15)
                    {
                        toolStripProgressBar1.Value = 15; //action-3
                        toolStripStatusLabel1.Text = "SIMエラー。最初から実施して下さい。";

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);
                        break;　//終わり

                    }
                    else
                    {
                        toolStripProgressBar1.Value = 35; //action-7
                        toolStripStatusLabel1.Text = "電話番号取得";
                       // toolStripStatusLabel1.Update();

                        //電話番号をテキストボックスへ入れる
                        textBox_tell2.ResetText();
                        textBox_tell2.Text = s.Substring(len - 11);
                        textBox_tell2.Update();

                        //log
                        LOG.WriteLine(textBox_tell2.Text);

                        //ADCコマンド
                        string ADC;
                        ADC = "!!VATT" + Environment.NewLine;
                        serialPort1.WriteLine(ADC);

                        //次へ進む
                        TestReading = false;
                        next_action();
                        return;
                    }
                }
            }
        }

        //書込み時　合否判定
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
                    ATCH = true;
                    startDT = DateTime.Now;
                    toolStripStatusLabel1.Text = "LTE接続テスト開始[タイムアウト20秒]";
                    this.Activate();
                    Application.DoEvents();
                    return;
                }

                if (s.StartsWith("OK"))
                {
                    if (CITY == true)
                    {
                        toolStripProgressBar1.Value = 50; //action-9

                        toolStripStatusLabel1.Text = "市町村コード書込";
                        LOG.WriteLine(toolStripStatusLabel1.Text);　//CITYCODE

                        this.Activate();
                        Application.DoEvents();

                        System.Threading.Thread.Sleep(1000);
                        CITY = false;
                        TestWriting = false;
                        return;
                    }
                    else if (TERM == true)
                    {
                        toolStripProgressBar1.Value = 60; //action-10

                        toolStripStatusLabel1.Text = "水位計番号書込";
                        LOG.WriteLine(toolStripStatusLabel1.Text);　//SENSORNO

                        this.Activate();
                        Application.DoEvents();

                        System.Threading.Thread.Sleep(1000);
                        TERM = false;
                        TestWriting = false;
                        return;
                    }
                    else
                    {
                        TestWriting = false;
                        return;
                    }

                }
                if (s.StartsWith("CITYCODE="))
                {
                    string WORD;
                    int WORDLEN = s.Length;
                    WORD = s.Substring(9);

                    if (WORD == textBox_city.Text)
                    {
                        toolStripProgressBar1.Value = 70; //action-11
                        toolStripStatusLabel1.Text = "市町村コード確認";
                        LOG.WriteLine(toolStripStatusLabel1.Text);　//INFO

                        this.Activate();
                        Application.DoEvents();

                        System.Threading.Thread.Sleep(1000);
                        TestWriting = false;

                    }
                    else
                    {
                        toolStripProgressBar1.Value = 40; //action-8
                        toolStripStatusLabel1.Text = "市町村コード確認エラー　装置を見直し最初から実施して下さい。";
                        LOG.WriteLine(toolStripStatusLabel1.Text);　

                        TestWriting = false;
                        TestSetting = false;
                        return;
                    }


                }
                if (s.StartsWith("SENSORNO="))
                {
                    string WORD;
                    int WORDLEN = s.Length;
                    WORD = s.Substring(9);

                    if (WORD == textBox_num.Text)
                    {
                        toolStripProgressBar1.Value = 80; //action-12
                        toolStripStatusLabel1.Text = "水位計番号確認";
                        LOG.WriteLine(toolStripStatusLabel1.Text);　//INFO

                        this.Activate();
                        Application.DoEvents();

                        System.Threading.Thread.Sleep(1000);
                        TestWriting = false;
                        //TestSetting = false; //TEST
                        return;
                    }
                    else
                    {
                        toolStripProgressBar1.Value = 40; //action-8
                        toolStripStatusLabel1.Text = "水位計番号確認エラー　装置を見直し最初から実施して下さい。";
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        TestWriting = false;
                        TestSetting = false;
                        return;
                    }


                }
                if (s.StartsWith("ATTACH"))
                {
                    string WORD;
                    WORD = s;

                    if (WORD == "ATTACH OK")
                    {
                        toolStripProgressBar1.Value = 90; //action-13

                        toolStripStatusLabel1.Text = "LTE接続テスト合格";
                        this.Update();

                        LOG.WriteLine(toolStripStatusLabel1.Text);　//ATTACH

                        System.Threading.Thread.Sleep(1000);
                        TestWriting = false;
                        return;
                    }
                    else if (WORD == "ATTACH ERROR")
                    {
                        toolStripStatusLabel1.Text = "LTE接続テスト失敗 最初から実施して下さい。";
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        TestWriting = false;
                        TestSetting = false;
                        return;
                    }
                }
            }
        }



        //テストコマンド　!!ATTACH
        private void ATTACH()
        {
            TestWriting = true;
            string ATT;
            ATT = "!!ATTACH" + Environment.NewLine;
            serialPort1.WriteLine(ATT);
        }

        //テストコマンド　!!INFO
        private void CHECKID()
        {
            TestWriting = true;
            string INFO;
            INFO = "!!INFO" + Environment.NewLine;
            serialPort1.WriteLine(INFO);
        }

        //テストコマンド　!!SENSNO
        private void WRITESENS()
        {
            TestWriting = true;
            string SENSORCODE;
            SENSORCODE = "!!SENSORNO=" + textBox_num.Text + Environment.NewLine;
            serialPort1.WriteLine(SENSORCODE);

        }

        //テストコマンド　!!CITYID
        private void WRITECITY()
        {
            TestWriting = true;
            string CITYCODE;
            CITYCODE = "!!CITYCODE=" + textBox_city.Text + Environment.NewLine;
            serialPort1.WriteLine(CITYCODE);


        }

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
                    deviceNames[index++] = name.ToString();
                }
                return deviceNames;
            }
            else
            {
                return null;
            }
        }

        private void ヘルプHToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MW-001用設定変更ソフトウェア　1.00版" + Environment.NewLine + "Copyright ABIT Co.");
        }
    }
}
