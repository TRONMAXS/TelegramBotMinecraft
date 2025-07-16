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
            TextBox textBox2;
            Button button4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            textBox1 = new TextBox();
            notifyIcon1 = new NotifyIcon(components);
            notifyIconMenu = new ContextMenuStrip(components);
            exitMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripComboBox1 = new ToolStripComboBox();
            toolStripTextBox1 = new ToolStripTextBox();
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            label2 = new Label();
            textBox3 = new TextBox();
            button6 = new Button();
            button7 = new Button();
            button8 = new Button();
            textBox2 = new TextBox();
            button4 = new Button();
            notifyIconMenu.SuspendLayout();
            SuspendLayout();
            // 
            // textBox2
            // 
            textBox2.CausesValidation = false;
            textBox2.Enabled = false;
            textBox2.HideSelection = false;
            textBox2.Location = new Point(7, 336);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Введите команду";
            textBox2.ShortcutsEnabled = false;
            textBox2.Size = new Size(193, 23);
            textBox2.TabIndex = 1;
            textBox2.TabStop = false;
            textBox2.Visible = false;
            textBox2.WordWrap = false;
            // 
            // button4
            // 
            button4.CausesValidation = false;
            button4.Enabled = false;
            button4.Location = new Point(7, 365);
            button4.Name = "button4";
            button4.Size = new Size(133, 27);
            button4.TabIndex = 6;
            button4.TabStop = false;
            button4.Text = "Выполнить команду";
            button4.UseMnemonic = false;
            button4.UseVisualStyleBackColor = true;
            button4.Visible = false;
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
            notifyIcon1.Text = "TelegramBotMinecraft";
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
            // label1
            // 
            label1.CausesValidation = false;
            label1.Enabled = false;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.Location = new Point(7, 395);
            label1.Name = "label1";
            label1.Size = new Size(166, 27);
            label1.TabIndex = 2;
            label1.Text = "Сервер - выключен";
            label1.UseMnemonic = false;
            label1.Visible = false;
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
            button2.Location = new Point(626, 336);
            button2.Name = "button2";
            button2.Size = new Size(81, 27);
            button2.TabIndex = 4;
            button2.Text = "Сервера";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Enabled = false;
            button3.Location = new Point(645, 369);
            button3.Name = "button3";
            button3.Size = new Size(132, 60);
            button3.TabIndex = 5;
            button3.Text = "Проверить и перезапустить";
            button3.UseVisualStyleBackColor = true;
            button3.Visible = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F);
            label2.Location = new Point(12, 422);
            label2.Name = "label2";
            label2.Size = new Size(0, 21);
            label2.TabIndex = 7;
            // 
            // textBox3
            // 
            textBox3.CausesValidation = false;
            textBox3.Enabled = false;
            textBox3.Font = new Font("Segoe UI", 12F);
            textBox3.HideSelection = false;
            textBox3.Location = new Point(7, 420);
            textBox3.Name = "textBox3";
            textBox3.ReadOnly = true;
            textBox3.ShortcutsEnabled = false;
            textBox3.Size = new Size(193, 29);
            textBox3.TabIndex = 8;
            textBox3.TabStop = false;
            textBox3.Visible = false;
            textBox3.WordWrap = false;
            // 
            // button6
            // 
            button6.Enabled = false;
            button6.Location = new Point(458, 377);
            button6.Name = "button6";
            button6.Size = new Size(132, 39);
            button6.TabIndex = 10;
            button6.Text = "Восстановить файл Servers.json";
            button6.UseVisualStyleBackColor = true;
            button6.Visible = false;
            // 
            // button7
            // 
            button7.Enabled = false;
            button7.Location = new Point(458, 422);
            button7.Name = "button7";
            button7.Size = new Size(132, 39);
            button7.TabIndex = 11;
            button7.Text = "Восстановить файл UserSettings.json";
            button7.UseVisualStyleBackColor = true;
            button7.Visible = false;
            // 
            // button8
            // 
            button8.Enabled = false;
            button8.Location = new Point(458, 331);
            button8.Name = "button8";
            button8.Size = new Size(132, 40);
            button8.TabIndex = 12;
            button8.Text = "Восстановить файл Settings.json";
            button8.UseVisualStyleBackColor = true;
            button8.Visible = false;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(803, 465);
            Controls.Add(button8);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(textBox3);
            Controls.Add(label2);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(textBox2);
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
        public TextBox textBox2;
        public NotifyIcon notifyIcon1;
        private Label label1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Label label2;
        private TextBox textBox3;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripTextBox toolStripTextBox1;
        private Button button6;
        private Button button7;
        private Button button8;
    }
}
