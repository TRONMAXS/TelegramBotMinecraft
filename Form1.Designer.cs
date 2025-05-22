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
            textBox2 = new TextBox();
            notifyIcon1 = new NotifyIcon(components);
            label1 = new Label();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
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
            // textBox2
            // 
            textBox2.Location = new Point(7, 336);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Введите команду";
            textBox2.Size = new Size(787, 23);
            textBox2.TabIndex = 1;
            textBox2.WordWrap = false;
            // 
            // notifyIcon1
            // 
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "notifyIcon1";
            notifyIcon1.Visible = true;
            // 
            // label1
            // 
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.Location = new Point(7, 372);
            label1.Name = "label1";
            label1.Size = new Size(166, 27);
            label1.TabIndex = 2;
            label1.Text = "Сервер - выключен";
            // 
            // button1
            // 
            button1.Location = new Point(713, 372);
            button1.Name = "button1";
            button1.Size = new Size(81, 27);
            button1.TabIndex = 3;
            button1.Text = "Настройки";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(713, 405);
            button2.Name = "button2";
            button2.Size = new Size(81, 27);
            button2.TabIndex = 4;
            button2.Text = "Сервера";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(575, 405);
            button3.Name = "button3";
            button3.Size = new Size(132, 27);
            button3.TabIndex = 5;
            button3.Text = "Обновить Json-файл";
            button3.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            KeyPreview = true;
            Name = "Form1";
            Text = "TelegramBotMinecraft";
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
    }
}
