namespace TelegramBotMinecraft
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            textBox1 = new TextBox();
            notifyIcon1 = new NotifyIcon(components);
            notifyIconMenu = new ContextMenuStrip(components);
            exitMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripComboBox1 = new ToolStripComboBox();
            toolStripTextBox1 = new ToolStripTextBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            label1 = new Label();
            notifyIconMenu.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 12F);
            textBox1.Location = new Point(7, 7);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Консоль";
            textBox1.ReadOnly = true;
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.Size = new Size(787, 323);
            textBox1.TabIndex = 0;
            textBox1.WordWrap = false;
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = notifyIconMenu;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "TelegramBotMinecraft v0.4.1";
            notifyIcon1.Visible = true;
            // 
            // notifyIconMenu
            // 
            notifyIconMenu.Items.AddRange(new ToolStripItem[] { exitMenuItem, toolStripSeparator1, toolStripMenuItem1, toolStripComboBox1, toolStripTextBox1 });
            notifyIconMenu.Name = "notifyIconMenu";
            notifyIconMenu.Size = new Size(182, 106);
            notifyIconMenu.Text = "TelegramBotMinecraft";
            // 
            // exitMenuItem
            // 
            exitMenuItem.Name = "exitMenuItem";
            exitMenuItem.Size = new Size(181, 22);
            exitMenuItem.Text = "Выход";
            exitMenuItem.Click += ExitMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(178, 6);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(181, 22);
            // 
            // toolStripComboBox1
            // 
            toolStripComboBox1.Name = "toolStripComboBox1";
            toolStripComboBox1.Size = new Size(121, 23);
            // 
            // toolStripTextBox1
            // 
            toolStripTextBox1.Name = "toolStripTextBox1";
            toolStripTextBox1.Size = new Size(100, 23);
            // 
            // button1
            // 
            button1.Location = new Point(713, 336);
            button1.Name = "button1";
            button1.Size = new Size(81, 27);
            button1.TabIndex = 3;
            button1.Text = "Настройки";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(713, 369);
            button2.Name = "button2";
            button2.Size = new Size(81, 27);
            button2.TabIndex = 4;
            button2.Text = "Сервера";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Enabled = false;
            button3.Location = new Point(662, 402);
            button3.Name = "button3";
            button3.Size = new Size(132, 39);
            button3.TabIndex = 5;
            button3.Text = "Проверить и перезапустить";
            button3.UseVisualStyleBackColor = true;
            button3.Visible = false;
            // 
            // button4
            // 
            button4.Enabled = false;
            button4.Location = new Point(12, 336);
            button4.Name = "button4";
            button4.Size = new Size(132, 40);
            button4.TabIndex = 12;
            button4.Text = "Восстановить файл Settings.json";
            button4.UseVisualStyleBackColor = true;
            button4.Visible = false;
            // 
            // button5
            // 
            button5.Enabled = false;
            button5.Location = new Point(150, 337);
            button5.Name = "button5";
            button5.Size = new Size(132, 39);
            button5.TabIndex = 10;
            button5.Text = "Восстановить файл Servers.json";
            button5.UseVisualStyleBackColor = true;
            button5.Visible = false;
            // 
            // button6
            // 
            button6.Enabled = false;
            button6.Location = new Point(288, 337);
            button6.Name = "button6";
            button6.Size = new Size(132, 39);
            button6.TabIndex = 11;
            button6.Text = "Восстановить файл UserSettings.json";
            button6.UseVisualStyleBackColor = true;
            button6.Visible = false;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI Light", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 204);
            label1.Location = new Point(7, 423);
            label1.Name = "label1";
            label1.Size = new Size(34, 18);
            label1.TabIndex = 2;
            label1.Text = "v0.4.4";
            label1.UseCompatibleTextRendering = true;
            label1.UseMnemonic = false;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(803, 450);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(textBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            KeyPreview = true;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Telegram Bot Minecraft";
            notifyIconMenu.ResumeLayout(false);
            notifyIconMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public TextBox textBox1;
        public NotifyIcon notifyIcon1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private Label label1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripTextBox toolStripTextBox1;
    }
}
