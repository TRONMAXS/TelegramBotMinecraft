namespace TelegramBotMinecraft
{
    partial class App
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(App));
            tabPage4 = new TabPage();
            userControl_Settings1 = new UserControl_Settings();
            tabPage3 = new TabPage();
            userControl_Users1 = new UserControl_Users();
            tabPage2 = new TabPage();
            userControl_Servers1 = new UserControl_Servers();
            tabPage1 = new TabPage();
            userControl_Console1 = new UserControl_Console();
            tabControl1 = new TabControl();
            tabPage4.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage1.SuspendLayout();
            tabControl1.SuspendLayout();
            SuspendLayout();
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(userControl_Settings1);
            resources.ApplyResources(tabPage4, "tabPage4");
            tabPage4.Name = "tabPage4";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // userControl_Settings1
            // 
            resources.ApplyResources(userControl_Settings1, "userControl_Settings1");
            userControl_Settings1.Name = "userControl_Settings1";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(userControl_Users1);
            resources.ApplyResources(tabPage3, "tabPage3");
            tabPage3.Name = "tabPage3";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // userControl_Users1
            // 
            resources.ApplyResources(userControl_Users1, "userControl_Users1");
            userControl_Users1.Name = "userControl_Users1";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(userControl_Servers1);
            resources.ApplyResources(tabPage2, "tabPage2");
            tabPage2.Name = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // userControl_Servers1
            // 
            resources.ApplyResources(userControl_Servers1, "userControl_Servers1");
            userControl_Servers1.Name = "userControl_Servers1";
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(userControl_Console1);
            resources.ApplyResources(tabPage1, "tabPage1");
            tabPage1.Name = "tabPage1";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // userControl_Console1
            // 
            resources.ApplyResources(userControl_Console1, "userControl_Console1");
            userControl_Console1.Name = "userControl_Console1";
            // 
            // tabControl1
            // 
            resources.ApplyResources(tabControl1, "tabControl1");
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            // 
            // App
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = SystemColors.Control;
            resources.ApplyResources(this, "$this");
            Controls.Add(tabControl1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "App";
            tabPage4.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TabPage tabPage4;
        private TabPage tabPage3;
        private TabPage tabPage2;
        private TabPage tabPage1;
        private TabControl tabControl1;
        private UserControl_Console userControl_Console1;
        private UserControl_Servers userControl_Servers1;
        private UserControl_Users userControl_Users1;
        private UserControl_Settings userControl_Settings1;
    }
}