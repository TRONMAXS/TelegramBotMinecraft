namespace TelegramBotMinecraft
{
    partial class Form5
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form5));
            labelVersion = new Label();
            labelBuild = new Label();
            buttonGitHub = new Button();
            SuspendLayout();
            // 
            // labelVersion
            // 
            labelVersion.AutoSize = true;
            labelVersion.Font = new Font("Segoe UI", 10F);
            labelVersion.Location = new Point(66, 19);
            labelVersion.Name = "labelVersion";
            labelVersion.Size = new Size(97, 19);
            labelVersion.TabIndex = 0;
            labelVersion.Text = "Версия: v0.4.4";
            // 
            // labelBuild
            // 
            labelBuild.AutoSize = true;
            labelBuild.Font = new Font("Segoe UI", 10F);
            labelBuild.Location = new Point(30, 50);
            labelBuild.Name = "labelBuild";
            labelBuild.Size = new Size(165, 19);
            labelBuild.TabIndex = 1;
            labelBuild.Text = "Дата сборки: 18.07.2025";
            // 
            // buttonGitHub
            // 
            buttonGitHub.Location = new Point(30, 90);
            buttonGitHub.Name = "buttonGitHub";
            buttonGitHub.Size = new Size(180, 30);
            buttonGitHub.TabIndex = 2;
            buttonGitHub.Text = "Открыть GitHub";
            buttonGitHub.UseVisualStyleBackColor = true;
            buttonGitHub.Click += buttonGitHub_Click;
            // 
            // Form5
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(250, 134);
            Controls.Add(buttonGitHub);
            Controls.Add(labelBuild);
            Controls.Add(labelVersion);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form5";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "О программе";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelBuild;
        private System.Windows.Forms.Button buttonGitHub;
    }
}
