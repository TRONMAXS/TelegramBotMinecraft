namespace TelegramBotMinecraft
{
    partial class UserControl_Console
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox5 = new GroupBox();
            listBox1 = new ListBox();
            groupBox6 = new GroupBox();
            richTextBox1 = new RichTextBox();
            button1 = new Button();
            textBox1 = new TextBox();
            groupBox5.SuspendLayout();
            groupBox6.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(listBox1);
            groupBox5.Location = new Point(729, 0);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(240, 527);
            groupBox5.TabIndex = 6;
            groupBox5.TabStop = false;
            groupBox5.Text = "Сервера";
            // 
            // listBox1
            // 
            listBox1.CausesValidation = false;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(7, 18);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(222, 499);
            listBox1.TabIndex = 1;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(richTextBox1);
            groupBox6.Controls.Add(button1);
            groupBox6.Controls.Add(textBox1);
            groupBox6.Location = new Point(0, 0);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(723, 527);
            groupBox6.TabIndex = 7;
            groupBox6.TabStop = false;
            groupBox6.Text = "Консоль";
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(6, 18);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(708, 463);
            richTextBox1.TabIndex = 0;
            richTextBox1.Text = "";
            // 
            // button1
            // 
            button1.ImeMode = ImeMode.NoControl;
            button1.Location = new Point(627, 491);
            button1.Name = "button1";
            button1.Size = new Size(87, 23);
            button1.TabIndex = 3;
            button1.Text = "Enter";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(6, 491);
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Поле для команд";
            textBox1.Size = new Size(615, 23);
            textBox1.TabIndex = 2;
            // 
            // UserControl_Console
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox5);
            Controls.Add(groupBox6);
            Name = "UserControl_Console";
            Size = new Size(969, 527);
            Load += UserControl_Console_Load;
            groupBox5.ResumeLayout(false);
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox5;
        private ListBox listBox1;
        private GroupBox groupBox6;
        private RichTextBox richTextBox1;
        private Button button1;
        private TextBox textBox1;
    }
}
