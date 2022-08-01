#pragma warning disable CS8625
#pragma warning disable CS8603
#pragma warning disable CS8604

using Newtonsoft.Json.Linq;

namespace Program {

    class Guild {
        public static async Task<JObject> getGuildFromApi() {
            HttpResponseMessage response = await HTTPClientInstance.instance.GetAsync($"https://discord.com/api/v10/guilds/{Config.GUILD_ID}");
            response.EnsureSuccessStatusCode();
            return JObject.Parse(await response.Content.ReadAsStringAsync());
        }

        public static int getNumberOfEmojiSlots(JObject guild) {
            int premiumTier = (int) guild["premium_tier"];

            switch (premiumTier) {
                case 0: {
                    return 50;
                }
                case 1: {
                    return 100;
                }
                case 2: {
                    return 150;
                }
                case 3: {
                    return 250;
                }
                default: {
                    return 50;
                }
            }
        }
    }
}
