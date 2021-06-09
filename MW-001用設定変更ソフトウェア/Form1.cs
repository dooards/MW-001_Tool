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
using System.Diagnostics;


namespace MW_001用設定変更ソフトウェア
{
    public partial class Form1 : Form
    {
        string[] ports;
        string[] Rxline;
        string filePath;
        string dataIN;
        bool TestReading;  //起動中
        bool TestWriting;　//確認中
        bool TestSetting;　//書込中
        bool Tout;
        bool CITY;
        bool TERM;
        bool ATCH;
        bool COMREADY;　//COM接続済み
        bool CSVREADY;  //CSV読込済み
        bool IDREADY;   //全終了
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
            //画面表示
            panel2.Visible = false;
            panel3.Visible = false;


            PortSearch();
        }
        
        //リセットボタン
        private void button_reset_Click(object sender, EventArgs e)
        {
            TestReading = false;

            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                toolStripProgressBar1.Value = 0;
                toolStripStatusLabel1.Text = "切断済み";
                System.Threading.Thread.Sleep(3000);
            }

            testReset();
            PortSearch();
        }

        //終了ボタン
        private void button_end_Click(object sender, EventArgs e)
        {
            TestReading = false;

            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

            System.Threading.Thread.Sleep(1000);

            testReset();
            LOG.WriteLine(" ");
            LOG.Close();
            Application.Exit();
        }

        //接続ボタン
        private void button_connect_Click(object sender, EventArgs e)
        {
            if (COMREADY == false)
            {
                try
                {
                    //serialport設定
                    serialPort1.PortName = comboBox_com.Text;
                    serialPort1.BaudRate = 115200; // Convert.ToInt32(cBoxBAUDRATE.Text);
                    serialPort1.DataBits = 8; // Convert.ToInt32(cBoxDATABITS.Text);
                    serialPort1.StopBits = (StopBits)Enum.Parse(typeof(StopBits), "One");
                    serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), "None");
                    //serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);

                    serialPort1.Open();
                    COMREADY = true;　//接続中

                    button_connect.Text = "切断";
                    toolStripStatusLabel1.Text = "接続完了";
                   // toolStripStatusLabel1.Update();



                    //次のステップを表示
                    toolStripProgressBar1.Value = 10;

                    //log
                    LOG.WriteLine(comboBox_com.Text);
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    System.Threading.Thread.Sleep(1000);
                    next_action();

                }
                catch
                {
                    //COMポート処理中ににエラーがある場合
                    COMREADY = false;
                    ForPushReset("ケーブルがありません。(2) リセットしてください。");

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);
                }
            }

            /*
            //切断ボタンの場合
            else if (COMREADY == true)
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();

                    toolStripProgressBar1.Value = 0;
                    toolStripStatusLabel1.Text = "切断済み";
                    System.Threading.Thread.Sleep(1000);
                }


                //未接続とする
                COMREADY = false;
                button_connect.Text = "接続";
                testReset();
                PortSearch();

                //log
                LOG.WriteLine(toolStripStatusLabel1.Text);
            }
            */
        }

        //CSVファイルボタン
        private void button_file_Click(object sender, EventArgs e)
        {
            TestReading = false;
            button_file.Enabled = false;

            OpenFileDialog SF = new OpenFileDialog();

            SF.FileName = "*.csv";
            //SF.InitialDirectory = @"C\:";
            SF.Filter = "CSVファイル|*.csv";
            SF.FilterIndex = 1;
            SF.Title = "水位計一覧ファイルを選択してください";
            SF.RestoreDirectory = true;
            SF.CheckFileExists = true;
            SF.CheckPathExists = true;


            if (SF.ShowDialog() == DialogResult.OK)
            {
                CSVREADY = true;
                textBox_csv.Text = Path.GetFileName(SF.FileName);
                filePath = SF.FileName;
                toolStripProgressBar1.Value = 20;
                toolStripStatusLabel1.Text = "水位計をテストモードで起動して下さい。";
                // toolStripStatusLabel1.Update();

                //log
                LOG.WriteLine(SF.FileName);

                //END button cancel
                //button_end.Enabled = false;
               

                //COMポート受信開始
                TestReading = true;
                TestRead();

            }
        }

        //書込みボタン
        private void button_write_Click(object sender, EventArgs e)
        {
            button_write.Enabled = false;
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

                                toolStripProgressBar1.Value = 100;
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
                        toolStripStatusLabel1.Text = "ID書込か動作確認で失敗しました。最初から実施願います。";
                        // toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        button_write.Enabled = true;
                        return;
                    }

                }
                else
                {
                    TestReading = false;
                    COMREADY = false;
                    ForPushReset("途中停止しました(3)。最初から実施願います。");

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                }
            }
            catch
            {
                TestReading = false;
                COMREADY = false;
                ForPushReset("途中停止しました(4)。最初から実施願います。");

                //log
                LOG.WriteLine(toolStripStatusLabel1.Text);

            }


        }

        //Version　ボタン
        private void button_info_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MW-001用設定変更ソフトウェア V1.40");
        }

        private void next_action()
        {
            if (panel1.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = true;
                panel3.Visible = false;

                if (CSVREADY == false)
                {
                    toolStripStatusLabel1.Text = "接続完了 水位計一覧ファイルを[選択]";
                }
                else
                {
                    toolStripStatusLabel1.Text = "水位計をテストモードで起動して下さい。";
                    // toolStripStatusLabel1.Update();
                    toolStripProgressBar1.Value = 20;

                    //COMポート受信開始
                    TestReading = true;
                    TestRead();
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
                        toolStripProgressBar1.Value = 50;

                        toolStripStatusLabel1.Text = "書込みできます。";
                        // toolStripStatusLabel1.Update();


                        //水位計ID書込み可
                        button_write.Enabled = true;
                        TestWriting = true;

                        break;

                    }
                    else
                    {

                        toolStripStatusLabel1.Text = "このSIMは登録がありません。最初から実施願います。";
                        //toolStripStatusLabel1.Update();
                        
                        button_write.Enabled = false;

                    }

                }
                sr.Close();

                //log
                LOG.WriteLine(textBox_city.Text);
                LOG.WriteLine(textBox_num.Text);
                LOG.WriteLine(toolStripStatusLabel1.Text);
            }
            catch
            {
                return;

            }
        }



        //リセット処理
        private void testReset()
        {
            //プログレスバー初期値
            toolStripProgressBar1.Value = 0;

            button_connect.Text = "接続";
            button_connect.Enabled = false;
            button_write.Enabled = false;

            //toolStripStatusLabel1.ResetText();

            textBox_tell2.ResetText();

            comboBox_com.Items.Clear();

            TestReading = false;
            TestWriting = false;
            TestSetting = false;
            Tout = false;
            CITY = false;
            TERM = false;
            ATCH = false;
            COMREADY = false;
            IDREADY = false;
        }

        //COMポート検索
        private void PortSearch()
        {

            do
            {
                //使用可能なCOMポートをコンボボックスへ表示
                ports = SerialPort.GetPortNames();
                comboBox_com.Items.AddRange(ports);


            } while (ports.Length > 0);

            //ケーブルが見つかった後の処理
            button_connect.Enabled = true;
            toolStripProgressBar1.Value = 5;
            Console.WriteLine(ports);

            //次のアクションの表示
            toolStripStatusLabel1.Text = "ケーブルを選択し、接続ボタンを押して下さい。";



/*
            //COMポートの有無で接続ボタンの表示。無い場合は処理を止める。
            if (ports.Length > 0)
            {
                button_connect.Enabled = true;
                toolStripProgressBar1.Value = 5;

                //次のアクションの表示
                toolStripStatusLabel1.Text = "ケーブルを選択し、接続ボタンを押して下さい。";
            }
            else
            {
                ForPushReset("ケーブルがありません。(1)　ケーブルを確認しリセットして下さい。");
                //log
                LOG.WriteLine(toolStripStatusLabel1.Text);



            }
*/

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
                        button_end.Enabled = false;
                        this.Activate();
                        Application.DoEvents();

                        if (Tout == true)
                        {
                            DateTime endDT = DateTime.Now;
                            TimeSpan ts = endDT - startDT;

                            if (ts.TotalSeconds > 60)
                            {
                                toolStripStatusLabel1.Text = "タイムアウト　最初から実施してください。";
                                //  toolStripStatusLabel1.Update();

                                //log
                                LOG.WriteLine(toolStripStatusLabel1.Text);

                                Tout = false;
                                TestReading = false;
                                button_end.Enabled = true;

                                toolStripProgressBar1.Value = 20;

                                //COMポート受信開始
                                TestReading = true;
                                TestRead();
                                return;
                            }
                        }

                        if (serialPort1.BytesToRead > 0)
                        {
                            dataIN = serialPort1.ReadExisting();
                            Rxline = dataIN.Split('\n');
                            this.Invoke(new EventHandler(Booting));
                        }
                    }
                
                   
                }
                catch //ケーブルが抜けた場合
                {
                    ForPushReset("途中停止しました(1)。最初から実施してください。");

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    COMREADY = false;
                    return;

                }

                Tout = false;

            }
            else
            {
                ForPushReset("途中停止しました(2)。最初から実施してください。");

                //log
                LOG.WriteLine(toolStripStatusLabel1.Text);

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

                        if (ts.TotalSeconds > 30)
                        {
                            toolStripStatusLabel1.Text = "タイムアウト　再度書き込むか、最初から実施して下さい。";
                            //toolStripStatusLabel1.Update();

                            //log
                            LOG.WriteLine(toolStripStatusLabel1.Text);

                            Tout = false;
                            TestWriting = true;　//

                            button_write.Enabled=true;

                            return;
                        }
                    }

                    if (ATCH == true)
                    {
                        DateTime endDTATCH = DateTime.Now;
                        TimeSpan ts = endDTATCH - startDT;

                        if (ts.TotalSeconds > 30)
                        {
                            toolStripStatusLabel1.Text = "LTE接続テスト失敗(1)";
                            //toolStripStatusLabel1.Update();

                            //log
                            LOG.WriteLine(toolStripStatusLabel1.Text);

                            ATCH = false;
                            TestWriting = true;
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
                        
                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                    }


                }
                Tout = false;

            }
        }



        //Boot時　合否判定
        private void Booting(object sender, EventArgs e)
        {
            foreach (string s in Rxline)
            {
                if(s.Contains("LTE_power off"))
                {
                    toolStripProgressBar1.Value = 20;
                    
                    toolStripStatusLabel1.Text = "テストモード起動失敗　装置を再起動して下さい。";
                    //toolStripStatusLabel1.Update();

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    return; //受信待ち継続
                }

                /*
                if (s.Contains("6666"))
                {
                    toolStripProgressBar1.Value = 20;
                    
                    toolStripStatusLabel1.Text = "テストモード起動失敗(2)　[装置再起動]";
                    // toolStripStatusLabel1.Update();

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    return;　//受信待ち継続
                }
                */

                if(s.Contains("START TEST"))
                {
                    toolStripProgressBar1.Value = 25;
                    startDT = DateTime.Now;
                    Tout = true;
                    toolStripStatusLabel1.Text = "起動中..";
                   // toolStripStatusLabel1.Update();
                    return;
                }

                if (s.Contains("WAKEUP"))
                {
                    toolStripProgressBar1.Value = 30;
                    
                    toolStripStatusLabel1.Text = "起動中....";
                  //  toolStripStatusLabel1.Update();
                    return;
                }

                if (s.Contains("NUM=0"))
                {
                    int len = s.Length;

                    if (len <= 14)
                    {
                        toolStripProgressBar1.Value = 35;
                        
                        toolStripStatusLabel1.Text = "SIMエラー　SIMを確認し最初からやり直してください。";
                        //toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        TestReading = false;
                        button_end.Enabled = true;

                    }
                    else if(len == 15)
                    {
                        toolStripProgressBar1.Value = 40;
                        toolStripStatusLabel1.Text = "電話番号取得";

                        //電話番号をテキストボックスへ入れる
                        textBox_tell2.ResetText();
                        textBox_tell2.Text = s.Substring(len - 11);
                        textBox_tell2.Update();

                        System.Threading.Thread.Sleep(1000);

                        //log
                        LOG.WriteLine(textBox_tell2.Text);

                        /*
                        //ADCコマンド
                        string ADC;
                        ADC = "!!VATT" + Environment.NewLine;
                        serialPort1.WriteLine(ADC);

                        //次へ進む
                        //TestReading = false;
                        */

                        TestReading = false;
                        button_end.Enabled = true;
                        next_action();

                    }

                }

                if (s.Contains("ADC="))
                {
                    
                    toolStripProgressBar1.Value = 50;

                    toolStripStatusLabel1.Text = "起動完了　電池電圧：" + s.Substring(4) + "　[次へ]";
                    // toolStripStatusLabel1.Update();

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);


                    //END button enable
                    button_end.Enabled = true;

                    //次へ進む
                    TestReading = false;
                    return;
                }
            }
        }

        //書込み時　合否判定
        private void WriteLog(object sender, EventArgs e)
        {

            foreach (string s in Rxline)
            {
                Console.WriteLine(s);

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
                    return;
                }

                if (s.Contains("OK"))
                {
                    if (CITY == true)
                    {
                        toolStripProgressBar1.Value = 60;

                        toolStripStatusLabel1.Text = "市町村コード書込";
                        //toolStripStatusLabel1.Update();

                        this.Activate();
                        Application.DoEvents();

                        System.Threading.Thread.Sleep(1000);
                        CITY = false;
                        TestWriting = false;
                        return;
                    }
                    else if (TERM == true)
                    {
                        toolStripProgressBar1.Value = 70;

                        toolStripStatusLabel1.Text = "水位計番号書込";
                       // toolStripStatusLabel1.Update();

                        this.Activate();
                        Application.DoEvents();

                        System.Threading.Thread.Sleep(1000);
                        TERM = false;
                        TestWriting = false;
                        return;
                    }
                    //else if()
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
                        toolStripProgressBar1.Value = 80;

                        toolStripStatusLabel1.Text = "市町村コード確認済";
                        // toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        this.Activate();
                        Application.DoEvents();

                        System.Threading.Thread.Sleep(1000);
                        TestWriting = false;

                    }
                    else
                    {
                        toolStripStatusLabel1.Text = "市町村コード確認エラー";
                        // toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);


                        TestWriting = false;
                        TestSetting = false;
                        return;
                    }


                }
                if (s.StartsWith("SENSORNO="))
                {
                    string WORD;
                    //int WORDLEN = s.Length;
                    WORD = s.Substring(9);

                    if (WORD == textBox_num.Text)
                    {
                        toolStripProgressBar1.Value = 90;

                        toolStripStatusLabel1.Text = "水位計番号確認済";
                        //  toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        this.Activate();
                        Application.DoEvents();

                        System.Threading.Thread.Sleep(1000);
                        TestWriting = false;
                        //TestSetting = false; //TEST
                        return;
                    }
                    else
                    {
                        toolStripStatusLabel1.Text = "水位計番号確認エラー";
                        // toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        TestWriting = false;
                        TestSetting = false;
                        return;
                    }


                }
                if (s.StartsWith("ATTACH"))
                {
                    string WORD;
                    WORD = s.Substring(7);


                    if (WORD == "OK")
                    {
                        toolStripProgressBar1.Value = 95;

                        toolStripStatusLabel1.Text = "LTE接続テスト合格";
                        // toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        TestWriting = false;
                        //TestSetting = false; //TEST
                        return;
                    }
                    else if (WORD == "ERROR")
                    {
                        toolStripStatusLabel1.Text = "LTE接続テスト失敗(2)　最初から実施して下さい。";
                        //  toolStripStatusLabel1.Update();

                        //log
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
            Console.WriteLine(ATT);
        }

        //テストコマンド　!!INFO
        private void CHECKID()
        {
            TestWriting = true;
            string INFO;
            INFO = "!!INFO" + Environment.NewLine;
            serialPort1.WriteLine(INFO);
            Console.WriteLine(INFO);
        }

        //テストコマンド　!!SENSNO
        private void WRITESENS()
        {
            TestWriting = true;
            string SENSORCODE;
            SENSORCODE = "!!SENSORNO=" + textBox_num.Text + Environment.NewLine;
            serialPort1.WriteLine(SENSORCODE);
            Console.WriteLine(SENSORCODE);

        }

        //テストコマンド　!!CITYID
        private void WRITECITY()
        {
            TestWriting = true;
            string CITYCODE;
            CITYCODE = "!!CITYCODE=" + textBox_city.Text + Environment.NewLine;
            serialPort1.WriteLine(CITYCODE);
            Console.WriteLine(CITYCODE);


        }

    }
}
