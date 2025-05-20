namespace TelegramBotMinecraft
{
    partial class Form2
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
            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();
            textBox1 = new TextBox();
            button2 = new Button();
            button1 = new Button();
            checkedListBox1 = new CheckedListBox();
            SuspendLayout();
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 12F);
            textBox1.Location = new Point(3, 14);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.PlaceholderText = "Текствое поле для Json ";
            textBox1.ScrollBars = ScrollBars.Both;
            textBox1.Size = new Size(602, 432);
            textBox1.TabIndex = 0;
            textBox1.WordWrap = false;
            // 
            // button2
            // 
            button2.Location = new Point(707, 14);
            button2.Name = "button2";
            button2.Size = new Size(81, 27);
            button2.TabIndex = 6;
            button2.Text = "Сохранить";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(620, 14);
            button1.Name = "button1";
            button1.Size = new Size(81, 27);
            button1.TabIndex = 5;
            button1.Text = "Открыть";
            button1.UseVisualStyleBackColor = true;
            // 
            // checkedListBox1
            // 
            checkedListBox1.Font = new Font("Segoe UI", 12F);
            checkedListBox1.FormattingEnabled = true;
            checkedListBox1.HorizontalScrollbar = true;
            checkedListBox1.Location = new Point(620, 64);
            checkedListBox1.Name = "checkedListBox1";
            checkedListBox1.Size = new Size(165, 364);
            checkedListBox1.TabIndex = 7;
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(checkedListBox1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Name = "Form2";
            Text = "Form2";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private OpenFileDialog openFileDialog1;
        private SaveFileDialog saveFileDialog1;
        private TextBox textBox1;
        private Button button2;
        private Button button1;
        private CheckedListBox checkedListBox1;
    }
}