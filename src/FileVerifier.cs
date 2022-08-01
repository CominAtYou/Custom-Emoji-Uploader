#pragma warning disable CS8600

using Microsoft.Toolkit.Uwp.Notifications;
using System.Text.RegularExpressions;

namespace Program {
    class FileVerifier {
        private static readonly string[] validExtensions = { ".png", ".webp", ".jpg", ".jpeg", ".gif" };
        private static readonly Regex exp = new Regex("^([A-Za-z0-9]*[A-Za-z]){3,32}[A-Za-z0-9]*$");
        public static async Task verify(FileInfo file) {
            /*
                Some people may save images from their web browser directly into the folder.
                Chrome downloads them as .crdownload files, then renames them to the correct name once completed.
                Firefox creates two files - one, the actual file with a zero-byte size, and a .part file
                that the data gets downloaded into and then copied into the zero-byte file.
                If these kinds of files are detected, don't do anything.
            */
            if (file.Extension == ".crdownload" || file.Extension == ".part") {
                throw new Exception("File is not an iamge file");
            }

            await Task.Delay(300); // This is needed for some reason. I don't know why. It just is.

            if (!validExtensions.Contains(file.Extension)) { // The best I can do
                new ToastContentBuilder()
                    .AddText("Unable to Create Emoji")
                    .AddText($"{file.Name} is not an image file.")
                .Show();
                throw new Exception("File is not an image file");
            }

            if (!exp.IsMatch(Path.GetFileNameWithoutExtension(file.FullName))) {
                new ToastContentBuilder()
                    .AddText("Unable to Create Emoji")
                    .AddText($"The name provided for {file.Name} is invalid. Enter a new name to try again.")
                    .AddInputTextBox("newname", "New name")
                    .AddButton(new ToastButton()
                        .SetContent("Submit")
                        .SetTextBoxId("newname")
                        .AddArgument("action", "rename"))
                .Show();

                ToastNotificationManagerCompat.OnActivated += async (e) => {
                    ToastArguments toastArgs = ToastArguments.Parse(e.Argument);
                    string directory = file.DirectoryName;

                    if (toastArgs["action"] == "rename") {
                        string newFilePath = directory + @"\" + e.UserInput.First().Value + file.Extension;
                        System.IO.File.Move(file.FullName, newFilePath);

                        await EmojiHandler.createEmoji(newFilePath);
                    }
                };
                throw new Exception("Name of file is invalid");
            }
        }
    }
}
