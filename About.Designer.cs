namespace Chinese_Chess
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.about_tip = new System.Windows.Forms.Label();
            this.about_info = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // about_tip
            // 
            this.about_tip.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.about_tip.AutoSize = true;
            this.about_tip.BackColor = System.Drawing.Color.Transparent;
            this.about_tip.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.about_tip.Location = new System.Drawing.Point(357, 116);
            this.about_tip.Name = "about_tip";
            this.about_tip.Size = new System.Drawing.Size(80, 26);
            this.about_tip.TabIndex = 2;
            this.about_tip.Text = "关     于";
            // 
            // about_info
            // 
            this.about_info.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.about_info.AutoSize = true;
            this.about_info.BackColor = System.Drawing.Color.Transparent;
            this.about_info.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.about_info.Location = new System.Drawing.Point(237, 174);
            this.about_info.Name = "about_info";
            this.about_info.Size = new System.Drawing.Size(338, 175);
            this.about_info.TabIndex = 3;
            this.about_info.Text = "本程序系2020年秋季.net架构程序设计\r\n大作业，作者是许静宇，学号：\r\n2018302060052。\r\n版本：1.0.0.0。\r\n开发环境：Windows " +
    "10 1909，\r\nVisual Studio 2019(版本：16.7.5)，\r\n.Net Framework 4.6。";
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.about_info);
            this.Controls.Add(this.about_tip);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "关于";
            this.Click += new System.EventHandler(this.About_Click);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.About_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label about_tip;
        private System.Windows.Forms.Label about_info;
    }
}