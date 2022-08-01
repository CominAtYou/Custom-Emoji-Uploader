namespace Program {
    class HTTPClientInstance {
        public static readonly HttpClient instance = new HttpClient();

        static HTTPClientInstance() {
            instance.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bot", Config.TOKEN);
        }
    }
}
