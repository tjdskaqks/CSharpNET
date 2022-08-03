namespace hospAsmInfoService_v1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lv_HospitalList = new System.Windows.Forms.ListView();
            this.wv2_Location = new Microsoft.Web.WebView2.WinForms.WebView2();
            ((System.ComponentModel.ISupportInitialize)(this.wv2_Location)).BeginInit();
            this.SuspendLayout();
            // 
            // lv_HospitalList
            // 
            this.lv_HospitalList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lv_HospitalList.Location = new System.Drawing.Point(12, 12);
            this.lv_HospitalList.MultiSelect = false;
            this.lv_HospitalList.Name = "lv_HospitalList";
            this.lv_HospitalList.Size = new System.Drawing.Size(1599, 333);
            this.lv_HospitalList.TabIndex = 0;
            this.lv_HospitalList.UseCompatibleStateImageBehavior = false;
            // 
            // wv2_Location
            // 
            this.wv2_Location.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wv2_Location.CreationProperties = null;
            this.wv2_Location.DefaultBackgroundColor = System.Drawing.Color.White;
            this.wv2_Location.Location = new System.Drawing.Point(15, 365);
            this.wv2_Location.Name = "wv2_Location";
            this.wv2_Location.Size = new System.Drawing.Size(1599, 507);
            this.wv2_Location.TabIndex = 1;
            this.wv2_Location.ZoomFactor = 1D;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1626, 885);
            this.Controls.Add(this.wv2_Location);
            this.Controls.Add(this.lv_HospitalList);
            this.Font = new System.Drawing.Font("맑은 고딕", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.wv2_Location)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ListView lv_HospitalList;
        private Microsoft.Web.WebView2.WinForms.WebView2 wv2_Location;
    }
}