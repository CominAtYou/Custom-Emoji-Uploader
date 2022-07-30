#pragma warning disable CS8600

using Discord;
using Discord.WebSocket;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Text.RegularExpressions;

namespace Program {
    class EmojiHandler {
        private DiscordSocketClient client;
        private readonly FileSystemWatcher watcher = new FileSystemWatcher(Config.PATH);
        public EmojiHandler() {
            client = new DiscordSocketClient();

            client.Ready += () => {
                Console.WriteLine($"Logged in as {client.CurrentUser}");
                return Task.CompletedTask;
            };
        }

        public async Task MainAsync() {
            try {
                await client.LoginAsync(TokenType.Bot, Config.TOKEN);
                await client.StartAsync();
            }
            catch {
                new ToastContentBuilder()
                    .AddText("Unable to Connect to Discord")
                    .AddText("We were unable to connect to Discord. This could be because of an issue with your internet connection, or your bot token might be invalid.")
                .Show();

                Application.Exit();
            }
            createWatcher();
        }

        public void createWatcher() {
            watcher.EnableRaisingEvents = true;
            watcher.NotifyFilter = NotifyFilters.Attributes |
                NotifyFilters.CreationTime |
                NotifyFilters.FileName |
                NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.Size;

            watcher.Created += async (sender, args) => {
                await createEmoji(args.FullPath);
            };
        }

        private async Task createEmoji(string path) {
            string filenameWithExtension = Path.GetFileName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string extension = Path.GetExtension(path).ToLower();
            Regex exp = new Regex("^([A-Za-z0-9]*[A-Za-z]){3,32}[A-Za-z0-9]*$");

            if (extension != ".png" && extension != ".jpg" && extension != ".jpeg" && extension != ".webp" && extension != ".gif") { // The best I can do
                new ToastContentBuilder()
                    .AddText("Unable to Create Emoji")
                    .AddText($"{filenameWithExtension} is not an image file.")
                .Show();
                return;
            }


            if (!exp.IsMatch(name)) {
                new ToastContentBuilder()
                    .AddText("Unable to Create Emoji")
                    .AddText($"The name provided for {filenameWithExtension} is invalid. Enter a new name to try again.")
                    .AddInputTextBox("newname", "New name")
                    .AddButton(new ToastButton()
                        .SetContent("Submit")
                        .SetTextBoxId("newname")
                        .AddArgument("action", "rename"))
                .Show();

                ToastNotificationManagerCompat.OnActivated += async (e) => {
                    ToastArguments toastArgs = ToastArguments.Parse(e.Argument);
                    string directory = Path.GetDirectoryName(path);

                    if (toastArgs["action"] == "rename") {
                        string newFilePath = directory + @"\" + e.UserInput.First().Value + extension;
                        System.IO.File.Move(path, newFilePath);

                        await createEmoji(newFilePath);
                    }
                };
                return;
            }

            SocketGuild guild = client.GetGuild(Config.GUILD_ID);

            try {
                using (Discord.Image image = new Discord.Image(path)) {
                    await guild.CreateEmoteAsync(name, image);
                }
            }
            catch {
                new ToastContentBuilder()
                    .AddText("Unable to Create Emoji")
                    .AddText($"Something happened while trying to create the emoji '{name}'.")
                .Show();
                return;
            }

            new ToastContentBuilder()
                .AddText("Emoji Created")
                .AddText($":{name}: has been added to {guild.Name}")
                .AddInlineImage(new Uri(path))
            .Show();
        }
    }
}
