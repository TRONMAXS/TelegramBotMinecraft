namespace TelegramBotMinecraft
{
    partial class Form3
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form3));
            checkBox1 = new CheckBox();
            label1 = new Label();
            textBox1 = new TextBox();
            button1 = new Button();
            label3 = new Label();
            listBox1 = new ListBox();
            textBox2 = new TextBox();
            button2 = new Button();
            button3 = new Button();
            textBox3 = new TextBox();
            button4 = new Button();
            label2 = new Label();
            textBox4 = new TextBox();
            button5 = new Button();
            SuspendLayout();
            // 
            // checkBox1
            // 
            checkBox1.Font = new Font("Segoe UI", 12F);
            checkBox1.Location = new Point(12, 12);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(239, 23);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Всплывающие уведомления";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.Location = new Point(117, 38);
            label1.Name = "label1";
            label1.Size = new Size(157, 21);
            label1.TabIndex = 1;
            label1.Text = "BotToken Telegram";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(12, 62);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(374, 23);
            textBox1.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(145, 91);
            button1.Name = "button1";
            button1.Size = new Size(106, 23);
            button1.TabIndex = 3;
            button1.Text = "Обновить токен";
            button1.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label3.Location = new Point(85, 191);
            label3.Name = "label3";
            label3.Size = new Size(235, 42);
            label3.TabIndex = 5;
            label3.Text = "               Разрешенные \r\nпользователи, чаты, группы";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.HorizontalScrollbar = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(12, 265);
            listBox1.Name = "listBox1";
            listBox1.ScrollAlwaysVisible = true;
            listBox1.Size = new Size(262, 169);
            listBox1.TabIndex = 6;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(12, 236);
            textBox2.Name = "textBox2";
            textBox2.PlaceholderText = "Имя";
            textBox2.Size = new Size(187, 23);
            textBox2.TabIndex = 7;
            // 
            // button2
            // 
            button2.Location = new Point(280, 265);
            button2.Name = "button2";
            button2.Size = new Size(106, 23);
            button2.TabIndex = 8;
            button2.Text = "Добавить";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(280, 294);
            button3.Name = "button3";
            button3.Size = new Size(106, 23);
            button3.TabIndex = 9;
            button3.Text = "Удалить";
            button3.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(205, 236);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "id";
            textBox3.Size = new Size(181, 23);
            textBox3.TabIndex = 10;
            // 
            // button4
            // 
            button4.Location = new Point(280, 323);
            button4.Name = "button4";
            button4.Size = new Size(106, 23);
            button4.TabIndex = 11;
            button4.Text = "Настроить";
            button4.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label2.Location = new Point(12, 132);
            label2.Name = "label2";
            label2.Size = new Size(137, 21);
            label2.TabIndex = 12;
            label2.Text = "Admin Password";
            // 
            // textBox4
            // 
            textBox4.Location = new Point(155, 143);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(231, 23);
            textBox4.TabIndex = 13;
            // 
            // button5
            // 
            button5.Location = new Point(12, 156);
            button5.Name = "button5";
            button5.Size = new Size(137, 23);
            button5.TabIndex = 14;
            button5.Text = "Обновить пароль";
            button5.UseVisualStyleBackColor = true;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(398, 444);
            Controls.Add(button5);
            Controls.Add(textBox4);
            Controls.Add(label2);
            Controls.Add(button4);
            Controls.Add(textBox3);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(textBox2);
            Controls.Add(listBox1);
            Controls.Add(label3);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(checkBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form3";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Настройки";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckBox checkBox1;
        private Label label1;
        private TextBox textBox1;
        private Button button1;
        private Label label3;
        private ListBox listBox1;
        private ListView listView1;
        private TextBox textBox2;
        private Button button2;
        private Button button3;
        private TextBox textBox3;
        private Button button4;
        private Label label2;
        private TextBox textBox4;
        private Button button5;
    }
}