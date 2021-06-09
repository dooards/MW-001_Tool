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
            this.Update();
            PortSearch();
        }

        //COMポート検索
        private void PortSearch()
        {
            button_connect.Text = "接続";
            toolStripStatusLabel1.Text = "ケーブルのUSB端をパソコンへ接続して下さい。";

            do
            {
                Application.DoEvents();
                if (END == true)
                {
                    Console.WriteLine("END");
                    //Application.Exit();
                    break;
                }
                statusStrip1.Update();

                //使用可能なCOMポートをコンボボックスへ表示
                ports = SerialPort.GetPortNames();
            } while (ports.Length == 0);

            //次のアクションの表示
            if(END == false)
            {
                comboBox_com.Items.AddRange(ports);
                foreach (string port in ports)
                {
                    Console.WriteLine(port);
                }
                button_connect.Enabled = true;
                toolStripProgressBar1.Value = 5;
                toolStripStatusLabel1.Text = "ケーブルを選択し、接続ボタンを押して下さい。";
                LOG.WriteLine(toolStripStatusLabel1.Text);
            }
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
                System.Threading.Thread.Sleep(1000);
            }

            testReset();
            PortSearch();
        }

        //終了ボタン
        private void button_end_Click(object sender, EventArgs e)
        {
            END = true;

            TestReading = false;

            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

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

                    //button_connect.Text = "切断";
                    toolStripStatusLabel1.Text = "接続完了";
                    //toolStripStatusLabel1.Update();
                    button_next.Select();


                    //次のステップを表示
                    toolStripProgressBar1.Value = 10;
                    //button_next.Enabled = true;

                    //log
                    LOG.WriteLine(comboBox_com.Text);
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    //次へ遷移
                    next_action();

                }
                catch
                {
                    //COMポート処理中ににエラーがある場合
                    COMREADY = false;
                    ForPushReset("ケーブルがありません。(2) [リセット]");

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
                  //  toolStripStatusLabel1.Update();
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
                toolStripProgressBar1.Value = 24;
                toolStripStatusLabel1.Text = "水位計をテストモードで起動して下さい。[ON]";
                // toolStripStatusLabel1.Update();

                //log
                LOG.WriteLine(SF.FileName);

                //END button cancel
                button_end.Enabled = false;
               

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

                            //case 4:
                            //    ATTACH();
                            //    IDWrite();
                            //    break;

                            case 5:
                                //終了処理
                                TestWriting = false;
                                TestSetting = false;
                                IDREADY = true;

                                


                                toolStripProgressBar1.Value = 100;
                                toolStripStatusLabel1.Text = "水位計ID書込み完了 続けての書込みは[戻る]";
                                // toolStripStatusLabel1.Update();

                                //log
                                LOG.WriteLine(toolStripStatusLabel1.Text);

                                COMREADY = true;
                                button_before.Enabled = true;
                                break;

                        }
                        num = num + 1;

                    }
                    if (IDREADY == false)
                    {
                        toolStripStatusLabel1.Text = "ID書込か動作確認で失敗しました。[書込][<戻る]";
                        // toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        button_write.Enabled = true;
                        button_before.Enabled = true;
                        return;
                    }

                }
                else
                {
                    TestReading = false;
                    COMREADY = false;
                    ForPushReset("途中停止しました(3)。[戻る]>[リセット]");

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    button_before.Enabled = true;

                }
            }
            catch
            {
                TestReading = false;
                COMREADY = false;
                ForPushReset("途中停止しました(4)。[戻る]>[リセット]");

                //log
                LOG.WriteLine(toolStripStatusLabel1.Text);

                button_before.Enabled = true;

            }


        }

        //Version　ボタン
        private void button_info_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MW-001用設定変更ソフトウェア V1.40");
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
                        toolStripStatusLabel1.Text = "接続完了 水位計一覧ファイルを[選択]";
                        // toolStripStatusLabel1.Update();

                        button_next.Enabled = false;

                    }
                    /*
                    else if (CSVREADY == true)
                    {
                        toolStripStatusLabel1.Text = "水位計をテストモードで起動して下さい。[ON]";
                        // toolStripStatusLabel1.Update();
                        toolStripProgressBar1.Value = 24;
                        button_next.Enabled = false;
                        button_end.Enabled = false;

                        //COMポート受信開始
                        TestReading = true;
                        TestRead();
                    }
                    */
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

                button_next.Enabled = false;
                return;
            }
        }

        //次へボタン
        private void button_next_Click(object sender, EventArgs e)
        {
            if (panel1.Visible == true)
            {
                panel1.Visible = false;
                panel2.Visible = true;
                panel3.Visible = false;

                button_before.Enabled = true;

                if (COMREADY == true)
                {
                    button_next.Enabled = true;
                    button_before.Enabled = true;
                }
                else
                {
                    button_next.Enabled = false;
                    button_before.Enabled = true;
                }

                if (CSVREADY == false)
                {
                    toolStripStatusLabel1.Text = "接続完了 水位計一覧ファイルを[選択]";
                   // toolStripStatusLabel1.Update();

                    button_next.Enabled = false;

                }
                else
                {
                    toolStripStatusLabel1.Text = "水位計をテストモードで起動して下さい。[ON]";
                   // toolStripStatusLabel1.Update();
                    toolStripProgressBar1.Value = 24;
                    button_next.Enabled = false;
                    button_end.Enabled = false;

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

                button_next.Enabled = false;
                return;

            }
        }

        //戻るボタン
        private void button_before_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == true)
            {
                panel1.Visible = true;
                panel2.Visible = false;
                panel3.Visible = false;

                button_before.Enabled = false; //前は1だから
                button_end.Enabled = true; //ENDボタンは復活
                TestReading = false; //読込中は止める

                if (COMREADY == true)
                {
                    toolStripStatusLabel1.Text = "接続完了　[次へ]";
                   // toolStripStatusLabel1.Update();
                    toolStripProgressBar1.Value = 16;
                    button_next.Enabled = true;
                    button_next.Select();

                }
                else
                {
                    toolStripStatusLabel1.Text = "リセットして下さい。[リセット]";
                   // toolStripStatusLabel1.Update();
                    toolStripProgressBar1.Value = 0;
                }

                return;

            }
            if (panel3.Visible == true)
            {
                if (COMREADY == true)
                {
                    panel1.Visible = false;
                    panel2.Visible = true;
                    panel3.Visible = false;

                    if (IDREADY == true)
                    {
                        toolStripProgressBar1.Value = 24;
                        toolStripStatusLabel1.Text = "次の水位計をテストモードで起動して下さい。[ON]";
                        //   toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        //button_before.Enabled = false;

                        //COMポート受信開始
                        TestReading = true;
                        TestRead();
                    }
                    else
                    {
                        //SIMが間違っていた　ID一覧にない　COMはつながっている　途中停止
                        panel1.Visible = false;
                        panel2.Visible = true;
                        panel3.Visible = false;

                        toolStripProgressBar1.Value = 32;
                        toolStripStatusLabel1.Text = "機器確認後　[再起動]";
                        //  toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        //COMポート受信開始
                        TestReading = true;
                        TestRead();

                    }

                }
                else
                {
                    //1へ飛ぶ
                    panel1.Visible = true;
                    panel2.Visible = false;
                    panel3.Visible = false;

                    toolStripProgressBar1.Value = 8;
                    toolStripStatusLabel1.Text = "リセットして下さい。[リセット]";
                  //  toolStripStatusLabel1.Update();

                    button_before.Enabled = false;

                }

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
                        toolStripProgressBar1.Value = 56;

                        toolStripStatusLabel1.Text = "書込みできます。 [書込]";
                        // toolStripStatusLabel1.Update();


                        //水位計ID書込み可
                        button_write.Enabled = true;
                        TestWriting = true;

                        break;

                    }
                    else
                    {

                        toolStripStatusLabel1.Text = "このSIMは登録がありません。[<戻る]";
                        //toolStripStatusLabel1.Update();

                        
                        button_write.Enabled = false;
                        button_before.Enabled = true;

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
            //初期値
            toolStripProgressBar1.Value = 0;

            button_connect.Text = "接続";
            button_connect.Enabled = false;
            button_write.Enabled = false;
            button_before.Enabled = false;
            button_next.Enabled = false;

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

                        if (Tout == true)
                        {
                            DateTime endDT = DateTime.Now;
                            TimeSpan ts = endDT - startDT;

                            if (ts.TotalSeconds > 50)
                            {
                                toolStripStatusLabel1.Text = "起動できませんでした。[タイムアウト][再起動]";
                                //  toolStripStatusLabel1.Update();

                                //log
                                LOG.WriteLine(toolStripStatusLabel1.Text);

                                Tout = false;
                                TestReading = false;

                                toolStripProgressBar1.Value = 32;
                                button_next.Enabled = false;

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
                            this.Invoke(new EventHandler(SerialLog));
                        }
                    }
                
                   
                }
                catch //ケーブルが抜けた場合
                {

                    ForPushReset("途中停止しました(1)。[戻る]>[リセット]");

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    COMREADY = false;
                    button_before.Enabled = true;
                    return;

                }

                Tout = false;

            }
            else
            {
                ForPushReset("途中停止しました(2)。[戻る]>[リセット]");

                //log
                LOG.WriteLine(toolStripStatusLabel1.Text);

                COMREADY = false;
                button_before.Enabled = true;
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

                        if (ts.TotalSeconds > 10)
                        {
                            toolStripStatusLabel1.Text = "書込みできませんでした。[タイムアウト]";
                            //toolStripStatusLabel1.Update();

                            //log
                            LOG.WriteLine(toolStripStatusLabel1.Text);

                            Tout = false;
                            TestWriting = true;　//

                            toolStripProgressBar1.Value = 32;
                            button_next.Enabled = false;
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
                            toolStripStatusLabel1.Text = "LTE接続テスト失敗(1)";
                            //toolStripStatusLabel1.Update();

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
                        button_before.Enabled = true;
                        ForPushReset("書き込みエラー　[戻る]>[リセット]");
                        
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
                    toolStripProgressBar1.Value = 24;
                    
                    toolStripStatusLabel1.Text = "テストモード起動失敗　[再起動して下さい]";
                    //toolStripStatusLabel1.Update();

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    return; //受信待ち継続
                }

                if (s.Contains("6666"))
                {
                    toolStripProgressBar1.Value = 24;
                    
                    toolStripStatusLabel1.Text = "テストモード起動失敗(2)　[再起動して下さい]";
                    // toolStripStatusLabel1.Update();

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    return;　//受信待ち継続
                }

                if(s.Contains("START TEST"))
                {
                    toolStripProgressBar1.Value = 32;
                    startDT = DateTime.Now;
                    Tout = true;
                    toolStripStatusLabel1.Text = "起動中.";
                   // toolStripStatusLabel1.Update();
                    return;
                }

                if (s.Contains("WAKEUP"))
                {
                    toolStripProgressBar1.Value = 40;
                    
                    toolStripStatusLabel1.Text = "起動中....";
                  //  toolStripStatusLabel1.Update();
                    return;
                }

                if (s.Contains("NUM="))
                {
                    int len = s.Length;

                    if (len < 5)
                    {
                        toolStripProgressBar1.Value = 32;
                        
                        toolStripStatusLabel1.Text = "SIMエラー。OFFしてSIMを確認後[戻る][再起動]";
                        //toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        return;　//受信待ち継続

                    }
                    else
                    {
                        toolStripProgressBar1.Value = 44;
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
                        //TestReading = false;
                        //button_next.Enabled = true;
                        return;
                    }

                }

                if (s.Contains("ADC="))
                {
                    
                    toolStripProgressBar1.Value = 48;

                    toolStripStatusLabel1.Text = "起動完了　電池電圧：" + s.Substring(4) + "　[次へ]";
                    // toolStripStatusLabel1.Update();

                    //log
                    LOG.WriteLine(toolStripStatusLabel1.Text);

                    button_next.Select();


                    //END button enable
                    button_end.Enabled = true;

                    //次へ進む
                    TestReading = false;
                    button_next.Enabled = true;
                    return;


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
                    //toolStripStatusLabel1.Update();
                    return;
                }

                if (s.Contains("OK"))
                {
                    if (CITY == true)
                    {
                        toolStripProgressBar1.Value = 64;

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
                        toolStripProgressBar1.Value = 72;

                        toolStripStatusLabel1.Text = "水位計番号書込";
                       // toolStripStatusLabel1.Update();

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
                    int WORDLEN = s.Length;
                    WORD = s.Substring(9);

                    if (WORD == textBox_num.Text)
                    {
                        toolStripProgressBar1.Value = 88;

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
                    WORD = s;

                    if (WORD == "ATTACH OK")
                    {
                        toolStripProgressBar1.Value = 96;

                        toolStripStatusLabel1.Text = "LTE接続テスト合格";
                        // toolStripStatusLabel1.Update();

                        //log
                        LOG.WriteLine(toolStripStatusLabel1.Text);

                        System.Threading.Thread.Sleep(1000);
                        TestWriting = false;
                        //TestSetting = false; //TEST
                        return;
                    }
                    else if (WORD == "ATTACH ERROR")
                    {
                        toolStripStatusLabel1.Text = "LTE接続テスト失敗(2)";
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


    }
}
