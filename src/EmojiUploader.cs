using Newtonsoft.Json.Linq;
using Microsoft.Toolkit.Uwp.Notifications;

namespace Program {
    class EmojiUploader {
        private const int UPLOAD_RATE_LIMIT = 20;
        private static int rateLimit = UPLOAD_RATE_LIMIT;
        private static List<FileInfo> queue = new List<FileInfo>();

        // This method is hit by multiple threads simultaneously when multiple files are added into the folder.
        // Any modification to rateLimit must be done with the Interlocked class, as thread-safe operations are a must.
        public static async Task upload(FileInfo file) {
            int currentLimitCount = Interlocked.Decrement(ref rateLimit);

            if (currentLimitCount == UPLOAD_RATE_LIMIT - 1) {
                Task.Delay(30000).ContinueWith(rateLimitComplete); // Do not await - schedules code for execution 30 seconds into the future, but does not block
            }
            else if (currentLimitCount < 0) {
                queue.Add(file);
                return;
            }
            await handleUpload(file);
        }

        private static async Task handleUpload(FileInfo file) {
            string name = Path.GetFileNameWithoutExtension(file.FullName);
            string base64ImageUri = "data:image/" + file.Extension.Replace(".", "") + ";base64," + Convert.ToBase64String(await File.ReadAllBytesAsync(file.FullName));
            ToastContentBuilder somethingHappenedToast = new ToastContentBuilder().AddText("Unable to Create Emoji").AddText($"Something happened while trying to create the emoji '{name}'.");
            JObject guild;

            try {
                guild = await Guild.getGuildFromApi();
            }
            catch {
                somethingHappenedToast.Show();
                return;
            }

            int availableSlots = Guild.getNumberOfEmojiSlots(guild) - ((JArray) guild["emojis"]).Count;

            if (availableSlots < 1) {
                new ToastContentBuilder().AddText("Unable to Create Emoji").AddText($"{guild["name"]} does not have any emoji slots left.").Show();
                return;
            }

            try {
                JObject bodyJson = new JObject();
                bodyJson.Add("name", name);
                bodyJson.Add("image", base64ImageUri);
                bodyJson.Add("roles", new JArray());

                StringContent body = new StringContent(bodyJson.ToString(), System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await HTTPClientInstance.instance.PostAsync($"https://discord.com/api/v10/guilds/{Config.GUILD_ID}/emojis", body);
                response.EnsureSuccessStatusCode();
            }
            catch {
                somethingHappenedToast.Show();
                return;
            }

            new ToastContentBuilder()
                .AddText("Emoji Created")
                .AddText($":{name}: has been added to {guild["name"]}.")
                .AddInlineImage(new Uri(file.FullName))
            .Show();
        }

        private static async void rateLimitComplete(Task _t) {
            Interlocked.Exchange(ref rateLimit, UPLOAD_RATE_LIMIT);
            List<FileInfo> queuedItems = new List<FileInfo>(queue); // Copy for thread safety

            for (int i = 0; i < queuedItems.Count; i++) {
                await upload(queuedItems[0]);
                queue.RemoveAt(0); // 0, not i - The uploaded item will always be at the beginning of the list
            }
        }
    }
}
