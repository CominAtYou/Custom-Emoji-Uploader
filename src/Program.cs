#pragma warning disable CS8600
#pragma warning disable CS8604

using Microsoft.Toolkit.Uwp.Notifications;
using System.Reflection;

namespace Program {
    class Program {
        static void Main(string[] args) {
            if (args.Contains("--unregisternotifications")) {
                ToastNotificationManagerCompat.Uninstall();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new CustomEmojiUploader());
        }
    }

    public class CustomEmojiUploader : ApplicationContext {
        private NotifyIcon trayIcon = new NotifyIcon();
        public CustomEmojiUploader() {
            InitializeConfig.initialize();
            RunOnStartup.runOnStartup();

            bool validWatcherPath = Directory.Exists(Config.PATH);
            if (!validWatcherPath) {
                new ToastContentBuilder()
                    .AddText("Invalid Path")
                    .AddText("The emoji folder path does not exist or is invalid. Please create it or set a new path.")
                .Show();
                Environment.Exit(1);
            }

            try {
                Guild.getGuildFromApi().GetAwaiter().GetResult();
            }
            catch {
                new ToastContentBuilder()
                    .AddText("Invalid Guild ID")
                    .AddText($"The Guild ID provided in the config is not valid. Either the ID is incorrect, or the bot is not in that guild. Double check the ID, then try again.")
                    .AddButton("Edit config", ToastActivationType.Protocol, new Uri(@$"{Path.GetDirectoryName(Application.ExecutablePath)}\customemojiuploader-config.json").ToString())
                .Show();
                Environment.Exit(1);
            }

            // Handle the "exit" button tooltip menu icon
            EventHandler exit = (sender, args) => {
                trayIcon.Visible = false; // Allow the tray icon to disappear before exiting
                Application.Exit();
            };

            // Get the icon from embedded resources and set it
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Custom_Emoji_Uploader.resources.trayicon.ico")) {
                trayIcon.Icon = new System.Drawing.Icon(stream);
            }

            trayIcon.Text = "Custom Emoji Uploader";

            trayIcon.ContextMenuStrip = new ContextMenuStrip(); // Create  a context menu

            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Version 1.3.0", null, null, "version"));
            trayIcon.ContextMenuStrip.Items[0].Enabled = false;

            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Icon from Twemoji", null, null, "attribution")); // Attribution for Twemoji icon
            trayIcon.ContextMenuStrip.Items[1].Enabled = false; // Make the attribution non-clickable

            trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null , exit, "exit")); // Add an 'Exit' option to the menu
            trayIcon.Visible = true; // Make the icon visible

            EmojiHandler.createWatcher();
        }

    }
}
