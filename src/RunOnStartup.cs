#pragma warning disable CS8600
#pragma warning disable CS8602

using Microsoft.Win32;

namespace Program {
    class RunOnStartup {
        public static void runOnStartup() {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key.GetSubKeyNames().Contains("CustomEmojiUploader") && File.Exists(key.GetValue("CustomEmojiUploader") as string)) return;

            key.SetValue("CustomEmojiUploader", Application.ExecutablePath);
        }
    }
}
