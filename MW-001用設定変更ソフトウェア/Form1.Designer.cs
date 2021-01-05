namespace MW_001用設定変更ソフトウェア
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.button_connect = new System.Windows.Forms.Button();
            this.comboBox_com = new System.Windows.Forms.ComboBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button_write = new System.Windows.Forms.Button();
            this.textBox_csv = new System.Windows.Forms.TextBox();
            this.textBox_tell = new System.Windows.Forms.TextBox();
            this.textBox_city = new System.Windows.Forms.TextBox();
            this.textBox_num = new System.Windows.Forms.TextBox();
            this.button_end = new System.Windows.Forms.Button();
            this.button_reset = new System.Windows.Forms.Button();
            this.button_info = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_step1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox_tell2 = new System.Windows.Forms.TextBox();
            this.button_file = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button_before = new System.Windows.Forms.Button();
            this.button_next = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_connect
            // 
            this.button_connect.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_connect.Location = new System.Drawing.Point(203, 76);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(105, 29);
            this.button_connect.TabIndex = 0;
            this.button_connect.Text = "接続";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // comboBox_com
            // 
            this.comboBox_com.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.comboBox_com.FormattingEnabled = true;
            this.comboBox_com.Location = new System.Drawing.Point(6, 41);
            this.comboBox_com.Name = "comboBox_com";
            this.comboBox_com.Size = new System.Drawing.Size(302, 29);
            this.comboBox_com.TabIndex = 0;
            this.comboBox_com.TabStop = false;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(15, 163);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(314, 40);
            this.progressBar1.TabIndex = 5;
            this.progressBar1.Value = 50;
            // 
            // button_write
            // 
            this.button_write.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_write.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.button_write.Location = new System.Drawing.Point(203, 76);
            this.button_write.Name = "button_write";
            this.button_write.Size = new System.Drawing.Size(105, 29);
            this.button_write.TabIndex = 0;
            this.button_write.Text = "書込";
            this.button_write.UseVisualStyleBackColor = true;
            this.button_write.Click += new System.EventHandler(this.button_write_Click);
            // 
            // textBox_csv
            // 
            this.textBox_csv.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox_csv.Location = new System.Drawing.Point(102, 41);
            this.textBox_csv.Name = "textBox_csv";
            this.textBox_csv.ReadOnly = true;
            this.textBox_csv.Size = new System.Drawing.Size(206, 29);
            this.textBox_csv.TabIndex = 12;
            this.textBox_csv.TabStop = false;
            this.textBox_csv.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_tell
            // 
            this.textBox_tell.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox_tell.Location = new System.Drawing.Point(86, 76);
            this.textBox_tell.Name = "textBox_tell";
            this.textBox_tell.ReadOnly = true;
            this.textBox_tell.Size = new System.Drawing.Size(111, 29);
            this.textBox_tell.TabIndex = 13;
            this.textBox_tell.TabStop = false;
            this.textBox_tell.Text = "02032456598";
            this.textBox_tell.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_tell.TextChanged += new System.EventHandler(this.textBox_tell_TextChanged);
            // 
            // textBox_city
            // 
            this.textBox_city.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox_city.Location = new System.Drawing.Point(102, 41);
            this.textBox_city.Name = "textBox_city";
            this.textBox_city.ReadOnly = true;
            this.textBox_city.Size = new System.Drawing.Size(54, 29);
            this.textBox_city.TabIndex = 14;
            this.textBox_city.TabStop = false;
            this.textBox_city.Text = "98765";
            this.textBox_city.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_num
            // 
            this.textBox_num.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox_num.Location = new System.Drawing.Point(258, 41);
            this.textBox_num.Name = "textBox_num";
            this.textBox_num.ReadOnly = true;
            this.textBox_num.Size = new System.Drawing.Size(48, 29);
            this.textBox_num.TabIndex = 15;
            this.textBox_num.TabStop = false;
            this.textBox_num.Text = "1265";
            this.textBox_num.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button_end
            // 
            this.button_end.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_end.Location = new System.Drawing.Point(257, 209);
            this.button_end.Name = "button_end";
            this.button_end.Size = new System.Drawing.Size(72, 40);
            this.button_end.TabIndex = 0;
            this.button_end.Text = "終了";
            this.button_end.UseVisualStyleBackColor = true;
            this.button_end.Click += new System.EventHandler(this.button_end_Click);
            // 
            // button_reset
            // 
            this.button_reset.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_reset.Location = new System.Drawing.Point(6, 76);
            this.button_reset.Name = "button_reset";
            this.button_reset.Size = new System.Drawing.Size(105, 29);
            this.button_reset.TabIndex = 17;
            this.button_reset.TabStop = false;
            this.button_reset.Text = "リセット";
            this.button_reset.UseVisualStyleBackColor = true;
            this.button_reset.Click += new System.EventHandler(this.button_reset_Click);
            // 
            // button_info
            // 
            this.button_info.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_info.Location = new System.Drawing.Point(15, 209);
            this.button_info.Name = "button_info";
            this.button_info.Size = new System.Drawing.Size(60, 40);
            this.button_info.TabIndex = 18;
            this.button_info.TabStop = false;
            this.button_info.Text = "INFO";
            this.button_info.UseVisualStyleBackColor = true;
            this.button_info.Click += new System.EventHandler(this.button_info_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.comboBox_com);
            this.groupBox1.Controls.Add(this.button_connect);
            this.groupBox1.Controls.Add(this.button_reset);
            this.groupBox1.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 114);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "1　ツールの接続";
            // 
            // label_step1
            // 
            this.label_step1.AutoSize = true;
            this.label_step1.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_step1.Location = new System.Drawing.Point(8, 139);
            this.label_step1.Name = "label_step1";
            this.label_step1.Size = new System.Drawing.Size(54, 21);
            this.label_step1.TabIndex = 6;
            this.label_step1.Text = "label4";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.textBox_tell2);
            this.groupBox2.Controls.Add(this.button_file);
            this.groupBox2.Controls.Add(this.textBox_csv);
            this.groupBox2.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(314, 114);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "2　ファイル選択と電源ON";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(6, 44);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(90, 21);
            this.label5.TabIndex = 21;
            this.label5.Text = "水位計一覧";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(6, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 21);
            this.label4.TabIndex = 20;
            this.label4.Text = "電話番号";
            // 
            // textBox_tell2
            // 
            this.textBox_tell2.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.textBox_tell2.Location = new System.Drawing.Point(86, 76);
            this.textBox_tell2.Name = "textBox_tell2";
            this.textBox_tell2.ReadOnly = true;
            this.textBox_tell2.Size = new System.Drawing.Size(111, 29);
            this.textBox_tell2.TabIndex = 19;
            this.textBox_tell2.TabStop = false;
            this.textBox_tell2.Text = "02032456598";
            this.textBox_tell2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button_file
            // 
            this.button_file.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_file.Location = new System.Drawing.Point(203, 76);
            this.button_file.Name = "button_file";
            this.button_file.Size = new System.Drawing.Size(105, 29);
            this.button_file.TabIndex = 2;
            this.button_file.Text = "選択";
            this.button_file.UseVisualStyleBackColor = true;
            this.button_file.Click += new System.EventHandler(this.button_file_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.textBox_tell);
            this.groupBox3.Controls.Add(this.textBox_city);
            this.groupBox3.Controls.Add(this.textBox_num);
            this.groupBox3.Controls.Add(this.button_write);
            this.groupBox3.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(314, 114);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "3　水位計IDの書込";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(6, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 21);
            this.label3.TabIndex = 18;
            this.label3.Text = "電話番号";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(162, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 21);
            this.label2.TabIndex = 17;
            this.label2.Text = "水位計番号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(6, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 21);
            this.label1.TabIndex = 16;
            this.label1.Text = "市町村コード";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(320, 124);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(320, 124);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox3);
            this.panel3.Location = new System.Drawing.Point(12, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(320, 124);
            this.panel3.TabIndex = 3;
            // 
            // button_before
            // 
            this.button_before.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_before.Location = new System.Drawing.Point(85, 209);
            this.button_before.Name = "button_before";
            this.button_before.Size = new System.Drawing.Size(80, 40);
            this.button_before.TabIndex = 20;
            this.button_before.TabStop = false;
            this.button_before.Text = "<戻る";
            this.button_before.UseVisualStyleBackColor = true;
            this.button_before.Click += new System.EventHandler(this.button_before_Click);
            // 
            // button_next
            // 
            this.button_next.Font = new System.Drawing.Font("Yu Gothic UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button_next.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.button_next.Location = new System.Drawing.Point(171, 209);
            this.button_next.Name = "button_next";
            this.button_next.Size = new System.Drawing.Size(80, 40);
            this.button_next.TabIndex = 21;
            this.button_next.TabStop = false;
            this.button_next.Text = "次へ>";
            this.button_next.UseVisualStyleBackColor = true;
            this.button_next.Click += new System.EventHandler(this.button_next_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar1,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 192);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(440, 25);
            this.statusStrip1.TabIndex = 22;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Yu Gothic UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(151, 20);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 217);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.button_next);
            this.Controls.Add(this.button_before);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button_info);
            this.Controls.Add(this.button_end);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MW-001用設定変更ソフトウェア";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_connect;
        private System.Windows.Forms.ComboBox comboBox_com;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button_write;
        private System.Windows.Forms.TextBox textBox_csv;
        private System.Windows.Forms.TextBox textBox_tell;
        private System.Windows.Forms.TextBox textBox_city;
        private System.Windows.Forms.TextBox textBox_num;
        private System.Windows.Forms.Button button_end;
        private System.Windows.Forms.Button button_reset;
        private System.Windows.Forms.Button button_info;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Label label_step1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button button_before;
        private System.Windows.Forms.Button button_next;
        private System.Windows.Forms.Button button_file;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox_tell2;
        private System.Windows.Forms.Label label5;
    }
}

