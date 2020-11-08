using System.Drawing;
using System.Threading;

namespace Chinese_Chess
{
    partial class MainWnd
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWnd));
            this.chat_txb = new System.Windows.Forms.TextBox();
            this.chat_info = new System.Windows.Forms.RichTextBox();
            this.game_info = new System.Windows.Forms.RichTextBox();
            this.common_sentences = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // chat_txb
            // 
            this.chat_txb.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chat_txb.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chat_txb.Location = new System.Drawing.Point(1557, 922);
            this.chat_txb.Multiline = true;
            this.chat_txb.Name = "chat_txb";
            this.chat_txb.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.chat_txb.Size = new System.Drawing.Size(318, 50);
            this.chat_txb.TabIndex = 0;
            // 
            // chat_info
            // 
            this.chat_info.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.chat_info.Location = new System.Drawing.Point(1538, 641);
            this.chat_info.Name = "chat_info";
            this.chat_info.Size = new System.Drawing.Size(357, 232);
            this.chat_info.TabIndex = 1;
            this.chat_info.Text = "";
            this.chat_info.TextChanged += new System.EventHandler(this.chat_info_TextChanged);
            // 
            // game_info
            // 
            this.game_info.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.game_info.Location = new System.Drawing.Point(1540, 144);
            this.game_info.Name = "game_info";
            this.game_info.Size = new System.Drawing.Size(357, 247);
            this.game_info.TabIndex = 2;
            this.game_info.Text = "";
            this.game_info.TextChanged += new System.EventHandler(this.game_info_TextChanged);
            // 
            // common_sentences
            // 
            this.common_sentences.FormattingEnabled = true;
            this.common_sentences.ItemHeight = 20;
            this.common_sentences.Items.AddRange(new object[] {
            "快点啊，都等得我花都谢了。",
            "你是GG还是MM啊？",
            "不要走，决战到天亮！",
            "你玩得也太厉害了！"});
            this.common_sentences.Location = new System.Drawing.Point(1557, 1036);
            this.common_sentences.Name = "common_sentences";
            this.common_sentences.Size = new System.Drawing.Size(148, 24);
            this.common_sentences.TabIndex = 3;
            this.common_sentences.Visible = false;
            this.common_sentences.SelectedIndexChanged += new System.EventHandler(this.common_sentences_SelectedIndexChanged);
            // 
            // MainWnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Chinese_Chess.Properties.Resources.bg_img;
            this.ClientSize = new System.Drawing.Size(1920, 1061);
            this.Controls.Add(this.common_sentences);
            this.Controls.Add(this.game_info);
            this.Controls.Add(this.chat_info);
            this.Controls.Add(this.chat_txb);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainWnd";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "中国象棋";
            this.Click += new System.EventHandler(this.MainWnd_Click);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainWnd_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainWnd_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainWnd_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chat_txb;
        private System.Windows.Forms.RichTextBox chat_info;
        private System.Windows.Forms.RichTextBox game_info;
        private System.Windows.Forms.ListBox common_sentences;
    }
}

