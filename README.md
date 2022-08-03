# Custom Emoji Uploader
Drop an image into a folder, and have it uploaded to Discord as a custom emoji.

https://user-images.githubusercontent.com/35669235/181942405-c413a035-1df2-45a1-8c06-c6356d315cad.mp4

## What It Does
Uploading emojis to Discord can be a hassle. You've gotta download the emoji, fumble through some folders to find the right server, and then get to the emoji page in server settings, and browse through the file upload window to find the image.

This app aim to simplify this process into one step. Download or copy an image into a designated folder, and you're done. If something goes wrong, the app will give you a heads up and offer some steps to help.

It's incredibly light and fast, and works with every image type Discord supports, which currently consists of PNG, JPEG, and GIF images.

## Getting Started
There's a few steps you'll need to take in order to be able to use the app, but they're quite simple. They're listed below.

### Requirements
- Windows 10 or later
- [.NET 6.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0/runtime)
- An existent computer
- An internet connection that can access Discord

First, download the executable from the [releases](https://github.com/CominAtYou/Custom-Emoji-Uploader/releases/latest) page, and save it to wherever you want - preferably somewhere where it won't be accidentally deleted.

### 1. Create a Discord bot
This app uses a Discord bot to upload emojis to your server, as that is the only way Discord permits programs to upload emojis. If you don't know how to create and add a bot to a server, follow [this guide](https://gist.github.com/CominAtYou/f2bdb20f36799914474dc270a19553bd). If you do already know, create a bot, add it to the server you want, and then jump to [step 2](#2-config-file).

### 2. Config File
To use the app, you'll need a config file. To create it, create a file named `customemojiuploader-config.json` in the same folder as the app executable.

Next, open up the newly-created file add the following JSON, and fill out the values for them.
```
{
    "emojiFolderPath": "C:\\path\\to\\the\\folder\\you\\want",
    "guildId": id_of_the_server_to_upload_to,
    "token": "YOUR_DISCORD_BOT_TOKEN"
}
```
Where:

`emojiFilderPath` Is the path to the folder where you will put images that will be made into emojis. You will need to use double backslashes `\\` when typing out the path, as JSON requires it.

`guildId` Is the ID of the server where you wish to upload the emojis to. (Your bot must be present in this server)

`token` Is the token for your bot. (If you followed the guide above, you copied and noted down your token. Put it in between the quotes.)

### 3. Run the bot
Double-click the executable. If everything went right, you should see an icon in the tray and no error message displayed. Congrats! You're ready to go.

## Building
1. Install .NET 6 SDK
2. Clone the repo
3. Install the necessary packages if they aren't already present: `dotnet add package Microsoft.Toolkit.Uwp.Notifications && dotnet add package Discord.NET && dotnet add package Newtonsoft.Json`
4. Run `dotnet publish`
5. Find the outputted executable in `./bin/Debug/net6.0-windows10.0.22000.0/win-x64/publish`
## Frequently Asked Questions
### How do I make the app not run on startup?
Open Task Manager. Click the startup tab. Find Custom Emoji Uploader. Right-click it, and hit disable. Alternatively, go to Settings > Apps > Startup, and turn off Custom Emoji Uploader.

### How do I close the app?
Right-click the tray icon, and hit exit.

### How do I turn off notifications?
You can turn them off in Windows Settings, under the "Notifications" option.

## Attribution
The icon for the executable and tray icon is from the [Twemoji](https://twemoji.twitter.com) project and is licensed under CC-BY 4.0.

This program uses the [Windows Community Toolkit](https://github.com/CommunityToolkit/WindowsCommunityToolkit) and [Newtonsoft JSON](https://www.newtonsoft.com/json), which are both licensed under the [MIT](https://licenses.nuget.org/MIT) license.
