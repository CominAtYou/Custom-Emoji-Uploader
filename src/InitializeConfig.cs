#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8604

using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json.Linq;

namespace Program {
    class InitializeConfig {
        private static readonly ToastContentBuilder incompleteConfigNotification = new ToastContentBuilder()
            .AddText("Invalid Config")
            .AddText("Your config file is incomplete or missing some items. Please make sure everything has been specified and is complete.")
            .AddButton("More Info", ToastActivationType.Protocol, "https://github.com/CominAtYou/Custom-Emoji-Uploader/blob/master/README.md#2-config-file")
            .AddButton("Edit Config", ToastActivationType.Protocol, new Uri(@$"{Path.GetDirectoryName(Application.ExecutablePath)}\customemojiuploader-config.json").ToString());

        public static void initialize() {
            String workingDirectory = Path.GetDirectoryName(Application.ExecutablePath);

            if (!File.Exists(@$"{workingDirectory}\customemojiuploader-config.json")) {
                new ToastContentBuilder()
                    .AddText("Unable to Find Config File")
                    .AddText("We couldn't find the config file. Please make sure that it's in the same folder as the app.")
                    .AddButton("More Info", ToastActivationType.Protocol, "https://github.com/CominAtYou/Custom-Emoji-Uploader/blob/master/README.md#2-config-file")
                .Show();
                Environment.Exit(1);
            }

            string jsonString = File.ReadAllText(@$"{workingDirectory}\customemojiuploader-config.json");
            JObject json;

            try {
                json = JObject.Parse(jsonString);
            }
            catch {
                incompleteConfigNotification.Show();
                Environment.Exit(1);
                return; // The app will not compile without this.
            }

            if (!json.ContainsKey("token") || !json.ContainsKey("guildId") || !json.ContainsKey("emojiFolderPath") || json["token"].Type != JTokenType.String || json["guildId"].Type != JTokenType.Integer || json["emojiFolderPath"].Type != JTokenType.String) {
                incompleteConfigNotification.Show();
                Environment.Exit(1);
            }

            Config.PATH = (string) json["emojiFolderPath"];
            Config.GUILD_ID = (ulong) json["guildId"];
            Config.TOKEN = (string) json["token"];
        }

    }
}
