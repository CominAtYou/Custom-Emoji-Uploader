#pragma warning disable CS8600
#pragma warning disable CS8604

using Discord;
using Discord.WebSocket;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Text.RegularExpressions;

namespace Program {
    class EmojiHandler {
        private DiscordSocketClient client = new DiscordSocketClient();
        private readonly FileSystemWatcher watcher = new FileSystemWatcher(Config.PATH);
        private readonly string[] validExtensions = { ".png", ".webp", ".jpg", ".jpeg", ".gif" };
        private List<string> inProgressFiles = new List<string>();

        public EmojiHandler() {
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
                NotifyFilters.FileName |
                NotifyFilters.CreationTime |
                NotifyFilters.FileName |
                NotifyFilters.LastWrite;

            watcher.Created += async (sender, args) => {
                if (inProgressFiles.Contains(args.FullPath)) return;
                inProgressFiles.Add(args.FullPath);

                await createEmoji(args.FullPath);
            };
        }

        public async Task createEmoji(string path) {
            Console.WriteLine(path);
            FileInfo file = new FileInfo(path);
            string name = Path.GetFileNameWithoutExtension(path);
            Regex exp = new Regex("^([A-Za-z0-9]*[A-Za-z]){3,32}[A-Za-z0-9]*$");

            inProgressFiles.Add(file.Name.ToLower());

            /*
                Some people may save images from their web browser directly into the folder.
                Chrome downloads them as .crdownload files, then renames them to the correct name once completed.
                Firefox creates two files - one, the actual file with a zero-byte size, and a .part file
                that the data gets downloaded into and then copied into the zero-byte file.
                If these kinds of files are detected, don't do anything.
            */
            if (file.Extension == ".crdownload" || file.Extension == ".part") return;
            await Task.Delay(300); // This is needed for some reason. I don't know why. It just is.

            if (!validExtensions.Contains(file.Extension)) { // The best I can do
                new ToastContentBuilder()
                    .AddText("Unable to Create Emoji")
                    .AddText($"{file.Name} is not an image file.")
                .Show();
                inProgressFiles.Remove(path);
                return;
            }

            if (!exp.IsMatch(name)) {
                new ToastContentBuilder()
                    .AddText("Unable to Create Emoji")
                    .AddText($"The name provided for {file.Name} is invalid. Enter a new name to try again.")
                    .AddInputTextBox("newname", "New name")
                    .AddButton(new ToastButton()
                        .SetContent("Submit")
                        .SetTextBoxId("newname")
                        .AddArgument("action", "rename"))
                .Show();
                inProgressFiles.Remove(path);

                ToastNotificationManagerCompat.OnActivated += async (e) => {
                    ToastArguments toastArgs = ToastArguments.Parse(e.Argument);
                    string directory = file.DirectoryName;

                    if (toastArgs["action"] == "rename") {
                        string newFilePath = directory + @"\" + e.UserInput.First().Value + file.Extension;
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
                inProgressFiles.Remove(path);
                return;
            }

            new ToastContentBuilder()
                .AddText("Emoji Created")
                .AddText($":{name}: has been added to {guild.Name}")
                .AddInlineImage(new Uri(path))
            .Show();

            inProgressFiles.Remove(path);
        }
    }
}
