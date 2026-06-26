namespace TelegramBotMinecraft
{
    partial class UserControl_Settings
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
            gb_Main = new GroupBox();
            chk_PushNotifications = new CheckBox();
            lbl_PushNotifications = new Label();
            btn_SaveMain = new Button();
            chk_AutoStartup = new CheckBox();
            lbl_AutoStartup = new Label();
            chk_StartToTray = new CheckBox();
            lbl_StartToTray = new Label();
            chk_AutoRestartBot = new CheckBox();
            lbl_AutoRestartBot = new Label();
            gb_Telegram = new GroupBox();
            gb_TelegramBot = new GroupBox();
            btn_ShowBotToken = new Button();
            lbl_BotToken = new Label();
            tb_BotToken = new TextBox();
            lbl_AutoStartBot = new Label();
            chk_AutoStartBot = new CheckBox();
            gb_TelegramProxy = new GroupBox();
            btn_ShowPassProxy = new Button();
            lbl_ProxyPassword = new Label();
            tb_ProxyPassword = new TextBox();
            lbl_ProxyUsername = new Label();
            tb_ProxyUsername = new TextBox();
            lbl_ProxyPort = new Label();
            tb_ProxyPort = new TextBox();
            lbl_ProxyHost = new Label();
            tb_ProxyHost = new TextBox();
            btn_OffBot = new Button();
            btn_OnBot = new Button();
            btn_SaveAndReconnect = new Button();
            gb_ConsoleBot = new GroupBox();
            rtb_ConsoleBot = new RichTextBox();
            gb_Main.SuspendLayout();
            gb_Telegram.SuspendLayout();
            gb_TelegramBot.SuspendLayout();
            gb_TelegramProxy.SuspendLayout();
            gb_ConsoleBot.SuspendLayout();
            SuspendLayout();
            // 
            // gb_Main
            // 
            gb_Main.Controls.Add(chk_PushNotifications);
            gb_Main.Controls.Add(lbl_PushNotifications);
            gb_Main.Controls.Add(btn_SaveMain);
            gb_Main.Controls.Add(chk_AutoStartup);
            gb_Main.Controls.Add(lbl_AutoStartup);
            gb_Main.Controls.Add(chk_StartToTray);
            gb_Main.Controls.Add(lbl_StartToTray);
            gb_Main.Enabled = false;
            gb_Main.Location = new Point(491, 3);
            gb_Main.Name = "gb_Main";
            gb_Main.Size = new Size(695, 203);
            gb_Main.TabIndex = 3;
            gb_Main.TabStop = false;
            gb_Main.Text = "Основные";
            // 
            // chk_PushNotifications
            // 
            chk_PushNotifications.AutoSize = true;
            chk_PushNotifications.ImeMode = ImeMode.NoControl;
            chk_PushNotifications.Location = new Point(10, 76);
            chk_PushNotifications.Name = "chk_PushNotifications";
            chk_PushNotifications.Size = new Size(15, 14);
            chk_PushNotifications.TabIndex = 17;
            chk_PushNotifications.TabStop = false;
            chk_PushNotifications.UseVisualStyleBackColor = true;
            // 
            // lbl_PushNotifications
            // 
            lbl_PushNotifications.AutoSize = true;
            lbl_PushNotifications.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_PushNotifications.ImeMode = ImeMode.NoControl;
            lbl_PushNotifications.Location = new Point(31, 70);
            lbl_PushNotifications.Name = "lbl_PushNotifications";
            lbl_PushNotifications.Size = new Size(237, 21);
            lbl_PushNotifications.TabIndex = 16;
            lbl_PushNotifications.Text = "Всплывающие уведомления";
            // 
            // btn_SaveMain
            // 
            btn_SaveMain.AutoSize = true;
            btn_SaveMain.ImeMode = ImeMode.NoControl;
            btn_SaveMain.Location = new Point(328, 174);
            btn_SaveMain.Name = "btn_SaveMain";
            btn_SaveMain.Size = new Size(84, 25);
            btn_SaveMain.TabIndex = 10;
            btn_SaveMain.Text = "Сохранить";
            btn_SaveMain.UseVisualStyleBackColor = true;
            // 
            // chk_AutoStartup
            // 
            chk_AutoStartup.AutoSize = true;
            chk_AutoStartup.ImeMode = ImeMode.NoControl;
            chk_AutoStartup.Location = new Point(10, 48);
            chk_AutoStartup.Name = "chk_AutoStartup";
            chk_AutoStartup.Size = new Size(15, 14);
            chk_AutoStartup.TabIndex = 15;
            chk_AutoStartup.TabStop = false;
            chk_AutoStartup.UseVisualStyleBackColor = true;
            // 
            // lbl_AutoStartup
            // 
            lbl_AutoStartup.AutoSize = true;
            lbl_AutoStartup.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_AutoStartup.ImeMode = ImeMode.NoControl;
            lbl_AutoStartup.Location = new Point(31, 42);
            lbl_AutoStartup.Name = "lbl_AutoStartup";
            lbl_AutoStartup.Size = new Size(151, 21);
            lbl_AutoStartup.TabIndex = 14;
            lbl_AutoStartup.Text = "Запуск с Windows";
            // 
            // chk_StartToTray
            // 
            chk_StartToTray.AutoSize = true;
            chk_StartToTray.ImeMode = ImeMode.NoControl;
            chk_StartToTray.Location = new Point(10, 19);
            chk_StartToTray.Name = "chk_StartToTray";
            chk_StartToTray.Size = new Size(15, 14);
            chk_StartToTray.TabIndex = 11;
            chk_StartToTray.TabStop = false;
            chk_StartToTray.UseVisualStyleBackColor = true;
            // 
            // lbl_StartToTray
            // 
            lbl_StartToTray.AutoSize = true;
            lbl_StartToTray.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_StartToTray.ImeMode = ImeMode.NoControl;
            lbl_StartToTray.Location = new Point(31, 13);
            lbl_StartToTray.Name = "lbl_StartToTray";
            lbl_StartToTray.Size = new Size(160, 21);
            lbl_StartToTray.TabIndex = 10;
            lbl_StartToTray.Text = "В трей при запуске";
            // 
            // chk_AutoRestartBot
            // 
            chk_AutoRestartBot.AutoSize = true;
            chk_AutoRestartBot.Enabled = false;
            chk_AutoRestartBot.ImeMode = ImeMode.NoControl;
            chk_AutoRestartBot.Location = new Point(8, 87);
            chk_AutoRestartBot.Name = "chk_AutoRestartBot";
            chk_AutoRestartBot.Size = new Size(15, 14);
            chk_AutoRestartBot.TabIndex = 17;
            chk_AutoRestartBot.TabStop = false;
            chk_AutoRestartBot.UseVisualStyleBackColor = true;
            // 
            // lbl_AutoRestartBot
            // 
            lbl_AutoRestartBot.AutoSize = true;
            lbl_AutoRestartBot.Enabled = false;
            lbl_AutoRestartBot.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_AutoRestartBot.ImeMode = ImeMode.NoControl;
            lbl_AutoRestartBot.Location = new Point(29, 81);
            lbl_AutoRestartBot.Name = "lbl_AutoRestartBot";
            lbl_AutoRestartBot.Size = new Size(147, 21);
            lbl_AutoRestartBot.TabIndex = 16;
            lbl_AutoRestartBot.Text = "Авторестарт бота";
            // 
            // gb_Telegram
            // 
            gb_Telegram.Controls.Add(gb_TelegramBot);
            gb_Telegram.Controls.Add(gb_TelegramProxy);
            gb_Telegram.Controls.Add(btn_OffBot);
            gb_Telegram.Controls.Add(btn_OnBot);
            gb_Telegram.Controls.Add(btn_SaveAndReconnect);
            gb_Telegram.Location = new Point(6, 3);
            gb_Telegram.Name = "gb_Telegram";
            gb_Telegram.Size = new Size(479, 654);
            gb_Telegram.TabIndex = 2;
            gb_Telegram.TabStop = false;
            gb_Telegram.Text = "Telegram";
            // 
            // gb_TelegramBot
            // 
            gb_TelegramBot.Controls.Add(btn_ShowBotToken);
            gb_TelegramBot.Controls.Add(lbl_BotToken);
            gb_TelegramBot.Controls.Add(tb_BotToken);
            gb_TelegramBot.Controls.Add(lbl_AutoStartBot);
            gb_TelegramBot.Controls.Add(lbl_AutoRestartBot);
            gb_TelegramBot.Controls.Add(chk_AutoRestartBot);
            gb_TelegramBot.Controls.Add(chk_AutoStartBot);
            gb_TelegramBot.Location = new Point(6, 22);
            gb_TelegramBot.Name = "gb_TelegramBot";
            gb_TelegramBot.Size = new Size(467, 181);
            gb_TelegramBot.TabIndex = 20;
            gb_TelegramBot.TabStop = false;
            gb_TelegramBot.Text = "Настройки бота";
            // 
            // btn_ShowBotToken
            // 
            btn_ShowBotToken.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_ShowBotToken.AutoSize = true;
            btn_ShowBotToken.ImeMode = ImeMode.NoControl;
            btn_ShowBotToken.Location = new Point(431, 26);
            btn_ShowBotToken.Name = "btn_ShowBotToken";
            btn_ShowBotToken.Size = new Size(29, 25);
            btn_ShowBotToken.TabIndex = 21;
            btn_ShowBotToken.Text = "👁";
            btn_ShowBotToken.UseVisualStyleBackColor = true;
            btn_ShowBotToken.Click += ShowHideTokenPass_Click;
            // 
            // lbl_BotToken
            // 
            lbl_BotToken.AutoSize = true;
            lbl_BotToken.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_BotToken.ImeMode = ImeMode.NoControl;
            lbl_BotToken.Location = new Point(8, 24);
            lbl_BotToken.Name = "lbl_BotToken";
            lbl_BotToken.Size = new Size(86, 21);
            lbl_BotToken.TabIndex = 3;
            lbl_BotToken.Text = "BotToken:";
            // 
            // tb_BotToken
            // 
            tb_BotToken.Location = new Point(112, 26);
            tb_BotToken.Name = "tb_BotToken";
            tb_BotToken.PlaceholderText = "123456789:ABCdefGHIjklMNOpqrSTUvwxYZ";
            tb_BotToken.Size = new Size(320, 23);
            tb_BotToken.TabIndex = 4;
            tb_BotToken.UseSystemPasswordChar = true;
            // 
            // lbl_AutoStartBot
            // 
            lbl_AutoStartBot.AutoSize = true;
            lbl_AutoStartBot.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_AutoStartBot.ImeMode = ImeMode.NoControl;
            lbl_AutoStartBot.Location = new Point(29, 53);
            lbl_AutoStartBot.Name = "lbl_AutoStartBot";
            lbl_AutoStartBot.Size = new Size(140, 21);
            lbl_AutoStartBot.TabIndex = 5;
            lbl_AutoStartBot.Text = "Автозапуск бота";
            // 
            // chk_AutoStartBot
            // 
            chk_AutoStartBot.AutoSize = true;
            chk_AutoStartBot.ImeMode = ImeMode.NoControl;
            chk_AutoStartBot.Location = new Point(8, 59);
            chk_AutoStartBot.Name = "chk_AutoStartBot";
            chk_AutoStartBot.Size = new Size(15, 14);
            chk_AutoStartBot.TabIndex = 8;
            chk_AutoStartBot.UseVisualStyleBackColor = true;
            // 
            // gb_TelegramProxy
            // 
            gb_TelegramProxy.Controls.Add(btn_ShowPassProxy);
            gb_TelegramProxy.Controls.Add(lbl_ProxyPassword);
            gb_TelegramProxy.Controls.Add(tb_ProxyPassword);
            gb_TelegramProxy.Controls.Add(lbl_ProxyUsername);
            gb_TelegramProxy.Controls.Add(tb_ProxyUsername);
            gb_TelegramProxy.Controls.Add(lbl_ProxyPort);
            gb_TelegramProxy.Controls.Add(tb_ProxyPort);
            gb_TelegramProxy.Controls.Add(lbl_ProxyHost);
            gb_TelegramProxy.Controls.Add(tb_ProxyHost);
            gb_TelegramProxy.Location = new Point(6, 209);
            gb_TelegramProxy.Name = "gb_TelegramProxy";
            gb_TelegramProxy.Size = new Size(467, 153);
            gb_TelegramProxy.TabIndex = 19;
            gb_TelegramProxy.TabStop = false;
            gb_TelegramProxy.Text = "Прокси";
            // 
            // btn_ShowPassProxy
            // 
            btn_ShowPassProxy.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btn_ShowPassProxy.AutoSize = true;
            btn_ShowPassProxy.ImeMode = ImeMode.NoControl;
            btn_ShowPassProxy.Location = new Point(431, 105);
            btn_ShowPassProxy.Name = "btn_ShowPassProxy";
            btn_ShowPassProxy.Size = new Size(29, 25);
            btn_ShowPassProxy.TabIndex = 22;
            btn_ShowPassProxy.Text = "👁";
            btn_ShowPassProxy.UseVisualStyleBackColor = true;
            btn_ShowPassProxy.Click += ShowHideTokenPass_Click;
            // 
            // lbl_ProxyPassword
            // 
            lbl_ProxyPassword.AutoSize = true;
            lbl_ProxyPassword.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_ProxyPassword.ImeMode = ImeMode.NoControl;
            lbl_ProxyPassword.Location = new Point(8, 103);
            lbl_ProxyPassword.Name = "lbl_ProxyPassword";
            lbl_ProxyPassword.Size = new Size(74, 21);
            lbl_ProxyPassword.TabIndex = 11;
            lbl_ProxyPassword.Text = "Пароль:";
            // 
            // tb_ProxyPassword
            // 
            tb_ProxyPassword.Location = new Point(168, 105);
            tb_ProxyPassword.Name = "tb_ProxyPassword";
            tb_ProxyPassword.PlaceholderText = "123456789";
            tb_ProxyPassword.Size = new Size(264, 23);
            tb_ProxyPassword.TabIndex = 12;
            tb_ProxyPassword.UseSystemPasswordChar = true;
            // 
            // lbl_ProxyUsername
            // 
            lbl_ProxyUsername.AutoSize = true;
            lbl_ProxyUsername.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_ProxyUsername.ImeMode = ImeMode.NoControl;
            lbl_ProxyUsername.Location = new Point(8, 74);
            lbl_ProxyUsername.Name = "lbl_ProxyUsername";
            lbl_ProxyUsername.Size = new Size(126, 21);
            lbl_ProxyUsername.TabIndex = 9;
            lbl_ProxyUsername.Text = "Пользователь:";
            // 
            // tb_ProxyUsername
            // 
            tb_ProxyUsername.Location = new Point(168, 76);
            tb_ProxyUsername.Name = "tb_ProxyUsername";
            tb_ProxyUsername.PlaceholderText = "Admin";
            tb_ProxyUsername.Size = new Size(286, 23);
            tb_ProxyUsername.TabIndex = 10;
            // 
            // lbl_ProxyPort
            // 
            lbl_ProxyPort.AutoSize = true;
            lbl_ProxyPort.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_ProxyPort.ImeMode = ImeMode.NoControl;
            lbl_ProxyPort.Location = new Point(8, 45);
            lbl_ProxyPort.Name = "lbl_ProxyPort";
            lbl_ProxyPort.Size = new Size(53, 21);
            lbl_ProxyPort.TabIndex = 7;
            lbl_ProxyPort.Text = "Порт:";
            // 
            // tb_ProxyPort
            // 
            tb_ProxyPort.Location = new Point(168, 47);
            tb_ProxyPort.Name = "tb_ProxyPort";
            tb_ProxyPort.PlaceholderText = "1080";
            tb_ProxyPort.Size = new Size(286, 23);
            tb_ProxyPort.TabIndex = 8;
            // 
            // lbl_ProxyHost
            // 
            lbl_ProxyHost.AutoSize = true;
            lbl_ProxyHost.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_ProxyHost.ImeMode = ImeMode.NoControl;
            lbl_ProxyHost.Location = new Point(8, 18);
            lbl_ProxyHost.Name = "lbl_ProxyHost";
            lbl_ProxyHost.Size = new Size(79, 21);
            lbl_ProxyHost.TabIndex = 5;
            lbl_ProxyHost.Text = "Хост / IP:";
            // 
            // tb_ProxyHost
            // 
            tb_ProxyHost.Location = new Point(168, 20);
            tb_ProxyHost.Name = "tb_ProxyHost";
            tb_ProxyHost.PlaceholderText = "127.0.0.1";
            tb_ProxyHost.Size = new Size(286, 23);
            tb_ProxyHost.TabIndex = 6;
            // 
            // btn_OffBot
            // 
            btn_OffBot.AutoSize = true;
            btn_OffBot.ImeMode = ImeMode.NoControl;
            btn_OffBot.Location = new Point(382, 621);
            btn_OffBot.Name = "btn_OffBot";
            btn_OffBot.Size = new Size(84, 25);
            btn_OffBot.TabIndex = 18;
            btn_OffBot.Text = "Выкл бот";
            btn_OffBot.UseVisualStyleBackColor = true;
            // 
            // btn_OnBot
            // 
            btn_OnBot.AutoSize = true;
            btn_OnBot.ImeMode = ImeMode.NoControl;
            btn_OnBot.Location = new Point(382, 590);
            btn_OnBot.Name = "btn_OnBot";
            btn_OnBot.Size = new Size(84, 25);
            btn_OnBot.TabIndex = 16;
            btn_OnBot.Text = "Вкл бот";
            btn_OnBot.UseVisualStyleBackColor = true;
            // 
            // btn_SaveAndReconnect
            // 
            btn_SaveAndReconnect.AutoSize = true;
            btn_SaveAndReconnect.ImeMode = ImeMode.NoControl;
            btn_SaveAndReconnect.Location = new Point(120, 621);
            btn_SaveAndReconnect.Name = "btn_SaveAndReconnect";
            btn_SaveAndReconnect.Size = new Size(200, 25);
            btn_SaveAndReconnect.TabIndex = 9;
            btn_SaveAndReconnect.Text = "Сохранить и переподключить ";
            btn_SaveAndReconnect.UseVisualStyleBackColor = true;
            // 
            // gb_ConsoleBot
            // 
            gb_ConsoleBot.Controls.Add(rtb_ConsoleBot);
            gb_ConsoleBot.Location = new Point(491, 212);
            gb_ConsoleBot.Name = "gb_ConsoleBot";
            gb_ConsoleBot.Size = new Size(695, 445);
            gb_ConsoleBot.TabIndex = 10;
            gb_ConsoleBot.TabStop = false;
            gb_ConsoleBot.Text = "Консоль бота";
            // 
            // rtb_ConsoleBot
            // 
            rtb_ConsoleBot.Font = new Font("Segoe UI", 12F);
            rtb_ConsoleBot.Location = new Point(6, 20);
            rtb_ConsoleBot.Name = "rtb_ConsoleBot";
            rtb_ConsoleBot.ReadOnly = true;
            rtb_ConsoleBot.Size = new Size(683, 417);
            rtb_ConsoleBot.TabIndex = 0;
            rtb_ConsoleBot.Text = "";
            // 
            // UserControl_Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(gb_ConsoleBot);
            Controls.Add(gb_Main);
            Controls.Add(gb_Telegram);
            Name = "UserControl_Settings";
            Size = new Size(1190, 660);
            gb_Main.ResumeLayout(false);
            gb_Main.PerformLayout();
            gb_Telegram.ResumeLayout(false);
            gb_Telegram.PerformLayout();
            gb_TelegramBot.ResumeLayout(false);
            gb_TelegramBot.PerformLayout();
            gb_TelegramProxy.ResumeLayout(false);
            gb_TelegramProxy.PerformLayout();
            gb_ConsoleBot.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox gb_Main;
        private CheckBox chk_AutoRestartBot;
        private Label lbl_AutoRestartBot;
        private CheckBox chk_AutoStartup;
        private Label lbl_AutoStartup;
        private CheckBox chk_StartToTray;
        private Label lbl_StartToTray;
        private GroupBox gb_Telegram;
        private Button btn_SaveAndReconnect;
        private CheckBox chk_AutoStartBot;
        private Label lbl_AutoStartBot;
        private TextBox tb_BotToken;
        private Label lbl_BotToken;
        private Button btn_SaveMain;
        private Button btn_OnBot;
        private Button btn_OffBot;
        private CheckBox chk_PushNotifications;
        private Label lbl_PushNotifications;
        private GroupBox gb_TelegramBot;
        private GroupBox gb_TelegramProxy;
        private Label lbl_ProxyPassword;
        private TextBox tb_ProxyPassword;
        private Label lbl_ProxyUsername;
        private TextBox tb_ProxyUsername;
        private Label lbl_ProxyPort;
        private TextBox tb_ProxyPort;
        private Label lbl_ProxyHost;
        private TextBox tb_ProxyHost;
        private GroupBox gb_ConsoleBot;
        private RichTextBox rtb_ConsoleBot;
        private Button btn_ShowBotToken;
        private Button btn_ShowPassProxy;
    }
}
