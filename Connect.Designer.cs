namespace Chinese_Chess
{
    partial class Connect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Connect));
            this.Listen_btn = new System.Windows.Forms.Button();
            this.Connect_btn = new System.Windows.Forms.Button();
            this.Local_IP_notice = new System.Windows.Forms.Label();
            this.Remote_IP_notice = new System.Windows.Forms.Label();
            this.Local_IP_input = new System.Windows.Forms.TextBox();
            this.Remote_IP_input = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Listen_btn
            // 
            this.Listen_btn.BackColor = System.Drawing.Color.Transparent;
            this.Listen_btn.FlatAppearance.BorderSize = 0;
            this.Listen_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Listen_btn.Location = new System.Drawing.Point(306, 284);
            this.Listen_btn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Listen_btn.Name = "Listen_btn";
            this.Listen_btn.Size = new System.Drawing.Size(73, 33);
            this.Listen_btn.TabIndex = 0;
            this.Listen_btn.Text = "监   听";
            this.Listen_btn.UseVisualStyleBackColor = false;
            this.Listen_btn.Click += new System.EventHandler(this.Listen_btn_Click);
            this.Listen_btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Listen_btn_MouseMove);
            // 
            // Connect_btn
            // 
            this.Connect_btn.BackColor = System.Drawing.Color.Transparent;
            this.Connect_btn.FlatAppearance.BorderSize = 0;
            this.Connect_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Connect_btn.Location = new System.Drawing.Point(410, 284);
            this.Connect_btn.Name = "Connect_btn";
            this.Connect_btn.Size = new System.Drawing.Size(73, 33);
            this.Connect_btn.TabIndex = 1;
            this.Connect_btn.Text = "连  接";
            this.Connect_btn.UseVisualStyleBackColor = false;
            this.Connect_btn.Click += new System.EventHandler(this.Connect_btn_Click);
            this.Connect_btn.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Connect_btn_MouseMove);
            // 
            // Local_IP_notice
            // 
            this.Local_IP_notice.AutoSize = true;
            this.Local_IP_notice.BackColor = System.Drawing.Color.Transparent;
            this.Local_IP_notice.Location = new System.Drawing.Point(290, 173);
            this.Local_IP_notice.Name = "Local_IP_notice";
            this.Local_IP_notice.Size = new System.Drawing.Size(64, 20);
            this.Local_IP_notice.TabIndex = 2;
            this.Local_IP_notice.Text = "本地IP：";
            // 
            // Remote_IP_notice
            // 
            this.Remote_IP_notice.AutoSize = true;
            this.Remote_IP_notice.BackColor = System.Drawing.Color.Transparent;
            this.Remote_IP_notice.Location = new System.Drawing.Point(290, 221);
            this.Remote_IP_notice.Name = "Remote_IP_notice";
            this.Remote_IP_notice.Size = new System.Drawing.Size(64, 20);
            this.Remote_IP_notice.TabIndex = 3;
            this.Remote_IP_notice.Text = "目的IP：";
            // 
            // Local_IP_input
            // 
            this.Local_IP_input.Location = new System.Drawing.Point(361, 173);
            this.Local_IP_input.Name = "Local_IP_input";
            this.Local_IP_input.Size = new System.Drawing.Size(139, 26);
            this.Local_IP_input.TabIndex = 4;
            // 
            // Remote_IP_input
            // 
            this.Remote_IP_input.Location = new System.Drawing.Point(361, 218);
            this.Remote_IP_input.Name = "Remote_IP_input";
            this.Remote_IP_input.Size = new System.Drawing.Size(139, 26);
            this.Remote_IP_input.TabIndex = 5;
            // 
            // Connect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Remote_IP_input);
            this.Controls.Add(this.Local_IP_input);
            this.Controls.Add(this.Remote_IP_notice);
            this.Controls.Add(this.Local_IP_notice);
            this.Controls.Add(this.Connect_btn);
            this.Controls.Add(this.Listen_btn);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Connect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "连接到另一台设备";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Connect_FormClosing);
            this.Load += new System.EventHandler(this.Connect_Load);
            this.Click += new System.EventHandler(this.Connect_Click);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Connect_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Listen_btn;
        private System.Windows.Forms.Button Connect_btn;
        private System.Windows.Forms.Label Local_IP_notice;
        private System.Windows.Forms.Label Remote_IP_notice;
        private System.Windows.Forms.TextBox Local_IP_input;
        private System.Windows.Forms.TextBox Remote_IP_input;
    }
}