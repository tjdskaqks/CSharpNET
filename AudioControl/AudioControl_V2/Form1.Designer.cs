
namespace AudioControl_V2
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.pb_ForegroundImage = new System.Windows.Forms.PictureBox();
            this.pb_BackgroudImage = new System.Windows.Forms.PictureBox();
            this.pb_Microphone = new System.Windows.Forms.PictureBox();
            this.tb_VoluneControl = new System.Windows.Forms.TrackBar();
            this.cbb_MicrophoneList = new System.Windows.Forms.ComboBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.elementHost1 = new System.Windows.Forms.Integration.ElementHost();
            this.userControl21 = new WpfControlLibrary1.UserControl2();
            this.uc_AudioControl1 = new AudioControlLibrary.uc_AudioControl();
            this.elementHost2 = new System.Windows.Forms.Integration.ElementHost();
            this.userControl31 = new WpfControlLibrary1.UserControl3();
            ((System.ComponentModel.ISupportInitialize)(this.pb_ForegroundImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_BackgroudImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Microphone)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_VoluneControl)).BeginInit();
            this.SuspendLayout();
            // 
            // pb_ForegroundImage
            // 
            this.pb_ForegroundImage.BackColor = System.Drawing.Color.Aqua;
            this.pb_ForegroundImage.Location = new System.Drawing.Point(239, 53);
            this.pb_ForegroundImage.Name = "pb_ForegroundImage";
            this.pb_ForegroundImage.Size = new System.Drawing.Size(84, 32);
            this.pb_ForegroundImage.TabIndex = 8;
            this.pb_ForegroundImage.TabStop = false;
            // 
            // pb_BackgroudImage
            // 
            this.pb_BackgroudImage.BackColor = System.Drawing.Color.Black;
            this.pb_BackgroudImage.Location = new System.Drawing.Point(239, 53);
            this.pb_BackgroudImage.Name = "pb_BackgroudImage";
            this.pb_BackgroudImage.Size = new System.Drawing.Size(164, 32);
            this.pb_BackgroudImage.TabIndex = 7;
            this.pb_BackgroudImage.TabStop = false;
            // 
            // pb_Microphone
            // 
            this.pb_Microphone.BackColor = System.Drawing.Color.Green;
            this.pb_Microphone.Location = new System.Drawing.Point(12, 53);
            this.pb_Microphone.Name = "pb_Microphone";
            this.pb_Microphone.Size = new System.Drawing.Size(36, 32);
            this.pb_Microphone.TabIndex = 6;
            this.pb_Microphone.TabStop = false;
            // 
            // tb_VoluneControl
            // 
            this.tb_VoluneControl.AutoSize = false;
            this.tb_VoluneControl.BackColor = System.Drawing.Color.Black;
            this.tb_VoluneControl.Location = new System.Drawing.Point(54, 53);
            this.tb_VoluneControl.Name = "tb_VoluneControl";
            this.tb_VoluneControl.Size = new System.Drawing.Size(179, 32);
            this.tb_VoluneControl.TabIndex = 5;
            this.tb_VoluneControl.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // cbb_MicrophoneList
            // 
            this.cbb_MicrophoneList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbb_MicrophoneList.DropDownWidth = 410;
            this.cbb_MicrophoneList.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cbb_MicrophoneList.FormattingEnabled = true;
            this.cbb_MicrophoneList.Location = new System.Drawing.Point(12, 12);
            this.cbb_MicrophoneList.Name = "cbb_MicrophoneList";
            this.cbb_MicrophoneList.Size = new System.Drawing.Size(391, 26);
            this.cbb_MicrophoneList.TabIndex = 9;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 189);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(1361, 689);
            this.textBox1.TabIndex = 10;
            // 
            // elementHost1
            // 
            this.elementHost1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.elementHost1.Location = new System.Drawing.Point(54, 92);
            this.elementHost1.Name = "elementHost1";
            this.elementHost1.Size = new System.Drawing.Size(179, 32);
            this.elementHost1.TabIndex = 11;
            this.elementHost1.Text = "elementHost1";
            this.elementHost1.Child = this.userControl21;
            // 
            // uc_AudioControl1
            // 
            this.uc_AudioControl1.Location = new System.Drawing.Point(424, 13);
            this.uc_AudioControl1.Name = "uc_AudioControl1";
            this.uc_AudioControl1.Size = new System.Drawing.Size(414, 116);
            this.uc_AudioControl1.TabIndex = 13;
            // 
            // elementHost2
            // 
            this.elementHost2.Location = new System.Drawing.Point(54, 131);
            this.elementHost2.Name = "elementHost2";
            this.elementHost2.Size = new System.Drawing.Size(253, 52);
            this.elementHost2.TabIndex = 12;
            this.elementHost2.Text = "elementHost2";
            this.elementHost2.Child = this.userControl31;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1385, 890);
            this.Controls.Add(this.uc_AudioControl1);
            this.Controls.Add(this.elementHost2);
            this.Controls.Add(this.elementHost1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.cbb_MicrophoneList);
            this.Controls.Add(this.pb_ForegroundImage);
            this.Controls.Add(this.pb_BackgroudImage);
            this.Controls.Add(this.pb_Microphone);
            this.Controls.Add(this.tb_VoluneControl);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pb_ForegroundImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_BackgroudImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Microphone)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tb_VoluneControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_ForegroundImage;
        private System.Windows.Forms.PictureBox pb_BackgroudImage;
        private System.Windows.Forms.PictureBox pb_Microphone;
        private System.Windows.Forms.TrackBar tb_VoluneControl;
        private System.Windows.Forms.ComboBox cbb_MicrophoneList;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Integration.ElementHost elementHost1;
        private WpfControlLibrary1.UserControl2 userControl21;
        private AudioControlLibrary.uc_AudioControl uc_AudioControl1;
        private System.Windows.Forms.Integration.ElementHost elementHost2;
        private WpfControlLibrary1.UserControl3 userControl31;
    }
}

