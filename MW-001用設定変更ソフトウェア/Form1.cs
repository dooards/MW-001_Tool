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
        bool CITY;
        bool TERM;
        bool COMREADY;　//COM接続済み
        bool CSVREADY; //CSV読込済み
        bool IDREADY; //全終了


        public Form1()
        {
            InitializeComponent();

            testReset();

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            panel2.Visible = false;
            panel3.Visible = false;

            PortSearch();

        }

        private void button_end_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }

            testReset();
            Application.Exit();
        }

        private void testReset()
        {
            //初期値
            progressBar1.Value = 0;

            button_connect.Text = "接続";
            button_connect.Enabled = false;
            button_write.Enabled = false;
            button_before.Enabled = false;
            button_next.Enabled = false;

            label_step1.ResetText();

            textBox_tell2.ResetText();

            comboBox_com.Items.Clear();

            TestReading = false;
            TestWriting = false;
            TestSetting = false;
            CITY = false;
            TERM = false;
            COMREADY = false;
            IDREADY = false;
        }

        private void PortSearch()
        {
            //使用可能なCOMポートをコンボボックスへ表示
            ports = SerialPort.GetPortNames();
            comboBox_com.Items.AddRange(ports);

            //COMポートの有無で接続ボタンの表示。無い場合は処理を止める。
            if (ports.Length > 0)
            {
                button_connect.Enabled = true;
                progressBar1.Value = 8;

                //次のアクションの表示
                label_step1.Text = "ケーブルを選択し、接続ボタンを押して下さい。";
                label_step1.Update();

            }
            else
            {
                ForPushReset("ケーブルがありません。(1)　リセットしてください。");

            }
        }

        private void button_reset_Click(object sender, EventArgs e)
        {
            TestReading = false;

            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                progressBar1.Value = 0;
                label_step1.Text = "切断済み";
                label_step1.Update();
                System.Threading.Thread.Sleep(1000);
            }

            testReset();
            PortSearch();
        }

        private void ForPushReset(string Mess)
        {
            label_step1.Text = Mess;
            label_step1.Update();
            progressBar1.Value = 0;
            button_connect.Enabled = false;
            TestReading = false;
            button_write.Enabled = false;
            button_connect.Enabled = false;

        }


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
                    label_step1.Text = "接続完了　[次へ]";
                    label_step1.Update();

                    //次のステップを表示
                    progressBar1.Value = 16;
                    button_next.Enabled = true;


                }
                catch
                {
                    //COMポート処理中ににエラーがある場合
                    COMREADY = false;
                    ForPushReset("ケーブルがありません。(2) [リセット]");
                }
            }
            //切断ボタンの場合
            else if(COMREADY == true)
            {
                if (serialPort1.IsOpen)
                {
                    serialPort1.Close();

                    progressBar1.Value = 0;
                    label_step1.Text = "切断済み";
                    label_step1.Update();
                    System.Threading.Thread.Sleep(1000);
                }
  

                //未接続とする
                COMREADY = false;
                button_connect.Text = "接続";
                testReset();
                PortSearch();
            }
        }

        private void button_file_Click(object sender, EventArgs e)
        {
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
                progressBar1.Value = 24;
                label_step1.Text = "水位計をテストモードで起動して下さい。[ON]";
                label_step1.Update();

                //COMポート受信開始
                TestReading = true;
                TestRead();

            }
        }

        private void TestRead()
        {


            if (serialPort1.IsOpen)
            {
                serialPort1.DiscardInBuffer();
                serialPort1.DiscardOutBuffer();

                while (TestReading == true)
                {
                    this.Activate();
                    Application.DoEvents();

                    try
                    {
                        if (serialPort1.BytesToRead > 0)
                        {
                            dataIN = serialPort1.ReadExisting();
                            Rxline = dataIN.Split('\n');
                            this.Invoke(new EventHandler(SerialLog));
                        }
                    }
                    catch //ケーブルが抜けた場合
                    {
                     
                        ForPushReset("途中停止しました(1)。[戻る]>[リセット]");
                        COMREADY = false;
                        button_before.Enabled = true;
                        return;

                    }
                }

            }
            else
            {
                ForPushReset("途中停止しました(2)。[戻る]>[リセット]");
                COMREADY = false;
                button_before.Enabled = true;
                return;

            }
        }

        private void SerialLog(object sender, EventArgs e)
        {
            foreach (string s in Rxline)
            {
                if(s.Contains("LTE_power off"))
                {
                    progressBar1.Value = 24;
                    
                    label_step1.Text = "テストモード起動失敗　[再起動]";
                    label_step1.Update();
                    return; //受信待ち継続
                }

                if (s.Contains("6666"))
                {
                    progressBar1.Value = 24;
                    
                    label_step1.Text = "テストモード起動失敗(2)　[再起動]";
                    label_step1.Update();
                    return;　//受信待ち継続
                }

                if(s.Contains("START TEST"))
                {
                    progressBar1.Value = 32;
                    
                    label_step1.Text = "起動中.";
                    label_step1.Update();
                    return;
                }

                if (s.Contains("WAKEUP"))
                {
                    progressBar1.Value = 40;
                    
                    label_step1.Text = "起動中....";
                    label_step1.Update();
                    return;
                }

                if (s.Contains("NUM="))
                {
                    int len = s.Length;

                    if (len < 5)
                    {
                        progressBar1.Value = 32;
                        
                        label_step1.Text = "SIMエラー。SIMを確認し[再起動]";
                        label_step1.Update();
                        return;　//受信待ち継続

                    }
                    else
                    {
                        progressBar1.Value = 48;
                        label_step1.Text = "起動完了 [次へ]";
                        label_step1.Update();


                        //電話番号をテキストボックスへ入れる
                        textBox_tell2.ResetText();
                        textBox_tell2.Text = s.Substring(len - 11);
                        textBox_tell2.Update();

                        //次へ進む
                        TestReading = false;
                        button_next.Enabled = true;
                        return;
                    }

                }
            }
        }

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
                        progressBar1.Value = 56;
                        
                        label_step1.Text = "書込みできます。 [書込]";
                        label_step1.Update();

                        //水位計ID書込み可
                        button_write.Enabled = true;
                        TestWriting = true;
                        
                        return;

                    }
                    else
                    {
                        
                        label_step1.Text = "このSIMは登録がありません。[再起動]";
                        //label_step1.Update();
                        button_write.Enabled = false;
                        button_before.Enabled = true;
                        
                    }

                }
                sr.Close();

            }
            catch
            {
                return;

            }
        }

        private void button_write_Click(object sender, EventArgs e)
        {
            button_write.Enabled = false;
            TestReading = false;
            TestSetting = true;
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
                                //終了処理
                                TestWriting = false;
                                TestSetting = false;
                                IDREADY = true;

                                progressBar1.Value = 100;
                                label_step1.Text = "水位計ID書込み完了 続けての書込みは[戻る]";
                                label_step1.Update();
                                COMREADY = true;
                                button_before.Enabled = true;
                                break;

                        }
                        num = num + 1;

                    }
                    if (IDREADY == false)
                    {
                        label_step1.Text = "IDの書込みに失敗しました。[書込][<戻る]";
                        label_step1.Update();
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
                    button_before.Enabled = true;

                }
            }
            catch
            {
                TestReading = false;
                COMREADY = false;
                ForPushReset("途中停止しました(4)。[戻る]>[リセット]");
                button_before.Enabled = true;

            }


        }

        private void CHECKID()
        {
            TestWriting = true;
            string INFO;
            INFO = "!!INFO" + Environment.NewLine;
            serialPort1.WriteLine(INFO);
        }

        private void WRITESENS()
        {
            TestWriting = true;
            string SENSORCODE;
            SENSORCODE = "!!SENSORNO=" + textBox_num.Text + Environment.NewLine;
            serialPort1.WriteLine(SENSORCODE);

        }

        private void WRITECITY()
        {
            TestWriting = true;
            string CITYCODE;
            CITYCODE = "!!CITYCODE=" + textBox_city.Text + Environment.NewLine;
            serialPort1.WriteLine(CITYCODE);


        }


        private void IDWrite()
        {
            if (serialPort1.IsOpen)
            {

                while (TestWriting == true)
                {
                    this.Activate();
                    Application.DoEvents();

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

                    }
                }
            }
        }

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

                if (s.Contains("OK"))
                {
                   if(CITY == true)
                    {
                        progressBar1.Value = 64;
                        
                        label_step1.Text = "市町村コード書込";
                        label_step1.Update();
                        System.Threading.Thread.Sleep(1000);
                        CITY = false;
                        TestWriting = false;
                        return;
                    }
                    else if (TERM == true)
                    {
                        progressBar1.Value = 72;
                        
                        label_step1.Text = "水位計番号書込";
                        label_step1.Update();
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
                        progressBar1.Value = 80;
                        
                        label_step1.Text = "市町村コード確認    ";
                        label_step1.Update();
                        System.Threading.Thread.Sleep(1000);
                        TestWriting = false;
                        
                    }
                    else
                    {
                        label_step1.Text = "市町村コード確認エラー";
                        label_step1.Update();
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
                        progressBar1.Value = 88;
                        
                        label_step1.Text = "水位計番号確認    ";
                        label_step1.Update();
                        System.Threading.Thread.Sleep(1000);
                        TestWriting = false;
                        //TestSetting = false; //TEST
                        return;
                    }
                    else
                    {                       
                        label_step1.Text = "水位計番号確認エラー";
                        label_step1.Update();
                        TestWriting = false;
                        TestSetting = false;
                        return;
                    }
                    

                }

            }


        }



        private void button_info_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MW-001用設定変更ソフトウェア V1.00");
        }

        private void button_next_Click(object sender, EventArgs e)
        {
            if(panel1.Visible==true)
            {
                panel1.Visible = false;
                panel2.Visible = true;
                panel3.Visible = false;

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
                    label_step1.Text = "接続完了 水位計一覧ファイルを[選択]";
                    label_step1.Update();

                    button_next.Enabled = false;

                }
                else
                {
                    label_step1.Text = "水位計をテストモードで起動して下さい。[ON]";
                    label_step1.Update();
                    progressBar1.Value = 32;
                    button_next.Enabled = false;

                    //COMポート受信開始
                    TestReading = true;
                    TestRead();

                }


                button_before.Enabled = true;
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

        private void button_before_Click(object sender, EventArgs e)
        {
            if (panel2.Visible == true)
            {
                panel1.Visible = true;
                panel2.Visible = false;
                panel3.Visible = false;

                button_before.Enabled = false; //前は1だから
                TestReading = false; //読込中は止める

                if(COMREADY == true)
                {
                    label_step1.Text = "接続完了　[次へ]";
                    label_step1.Update();
                    progressBar1.Value = 24;
                    button_next.Enabled = true;

                }
                else
                {
                    label_step1.Text = "リセットして下さい。[リセット]";
                    label_step1.Update();
                    progressBar1.Value = 0;
                }

                return;

            }
            if (panel3.Visible == true)
            {
                if(COMREADY == true)
                {
                    panel1.Visible = false;
                    panel2.Visible = true;
                    panel3.Visible = false;

                    if (IDREADY == true)
                    {
                        progressBar1.Value = 32;
                        label_step1.Text = "水位計をテストモードで起動して下さい。[ON]";
                        label_step1.Update();

                        //button_before.Enabled = false;

                        //COMポート受信開始
                        TestReading = true;
                        TestRead();
                    }
                    else
                    {
                        //1へ飛ぶ
                        panel1.Visible = true;
                        panel2.Visible = false;
                        panel3.Visible = false;

                        progressBar1.Value = 8;
                        label_step1.Text = "リセットして下さい。[リセット]";
                        label_step1.Update();

                        button_before.Enabled = false;
                    }

                }
                else
                {
                    //1へ飛ぶ
                    panel1.Visible = true;
                    panel2.Visible = false;
                    panel3.Visible = false;

                    progressBar1.Value = 8;
                    label_step1.Text = "リセットして下さい。[リセット]";
                    label_step1.Update();

                    button_before.Enabled = false;

                }

            }
        }
    }
}
