namespace Program {
    class EmojiHandler {
        private static readonly FileSystemWatcher watcher = new FileSystemWatcher(Config.PATH);
        public static readonly List<string> inProgressFiles = new List<string>();
        public static void createWatcher() {
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

        public static async Task createEmoji(string path) {
            FileInfo file = new FileInfo(path);

            try {
                await FileVerifier.verify(file);
            }
            catch {
                inProgressFiles.Remove(path);
                return;
            }

            await EmojiUploader.upload(file);
            inProgressFiles.Remove(path);
        }
    }
}
