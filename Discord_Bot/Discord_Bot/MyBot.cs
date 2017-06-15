using System;

using Discord;
using Discord.Commands;
using Discord.Audio;
using System.IO;
using System.Collections;

namespace Discord_Bot
{
    class MyBot
    {

        DiscordClient discord;
        private Boolean trolling = false;
        private Boolean deleting = false;

        public MyBot()
        {

            discord = getClient();
            setUp(discord);
            setUpCommands(discord);

            //discord.SetGame("Nooob noooob");

            Connect(discord);        

        }

        private void log(Object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private DiscordClient getClient()
        {
            //Create new Client
            DiscordClient discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = log;

            });

            discord.UsingAudio(x => // Opens an AudioConfigBuilder so we can configure our AudioService
            {
                x.Mode = AudioMode.Outgoing; // Tells the AudioService that we will only be sending audio
            });

            Console.WriteLine("Client initialized!");
            return discord;
        } 

        private void Connect(DiscordClient discord)
        {
            //Connect
            discord.ExecuteAndWait(async () =>
            {
                //Security Stuff
                String prepath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                String path = prepath + @"\key\api_key.txt";
                String key = "";
                StreamReader reader = null;

                Console.WriteLine(path);

                if (File.Exists(path))
                {
                    reader = new StreamReader(path);
                    Console.WriteLine("Reader active");
                }
                await discord.Connect(reader.ReadLine(), TokenType.Bot);   //No connect
                reader.Close();
            });
            Console.WriteLine("Client connected!");
        }

        private void setUp(DiscordClient discord)
        {
            //SETUP Command prefix
            discord.UsingCommands(x =>
            {
                x.AllowMentionPrefix = true;
                x.HelpMode = HelpMode.Public;

            });
            Console.WriteLine("Command Prefix set!");
        }

        private void setUpCommands(DiscordClient discord)
        {
            //Command List
            CommandService commands = discord.GetService<CommandService>();

            RegisterMemeCommand(commands);
            RegisterPrugeCommand(commands);
            RegisterTrollCommand(commands, discord);
            RegisterDeleteCommand(commands, discord);
            RegisterStopCommand(commands, discord);

            Console.WriteLine("Commands initialized!");
        }

        private void RegisterMemeCommand(CommandService commands)
        {
            commands.CreateCommand("lotr")
                .Alias(new String[]{"LOTR", "Lord of the Rings", "lord of the rings"})
                .Do(async (e) => {

                    if (deleting) DeleteCalling(e);

                    Console.WriteLine("Sending meme!");
                    String[] files = Directory.GetFiles("memes");

                    foreach(String s in files)
                    {
                        Console.WriteLine("Files " + s);
                    }
                    
                    ArrayList arraylist = new ArrayList(files);

                    foreach (String file in arraylist)
                    {
                        if (!file.ToLower().EndsWith(".jpg") || !file.ToLower().EndsWith(".png"))
                        {
                            //arraylist.Remove(file);
                            //Console.WriteLine("File removed " + file);
                        }
                    }

                    Random random = new Random();
                    int memeindex = random.Next(arraylist.Capacity);
                    Console.WriteLine("Index: " + memeindex + " Size: " + arraylist.Capacity);

                    String fileToSend = (String)arraylist[memeindex];
                    Console.WriteLine("Sending " + fileToSend);
                    await e.Channel.SendFile(fileToSend);
                });
        }

        private void RegisterPrugeCommand(CommandService commands)
        {
            commands.CreateCommand("purge")
                .Do(async (e) =>
                {
                    if(deleting) DeleteCalling(e);

                    Message[] messagesToDelete;

                    //Standard Limit of 100 Messages
                    messagesToDelete = await e.Channel.DownloadMessages(5);

                    await e.Channel.DeleteMessages(messagesToDelete);
                });
        }

        private void RegisterYoutubeCommand(CommandService commands, DiscordClient client)
        {
            commands.CreateCommand("join")
                .Do(async (e) =>
                {

                    //var voiceChannels = client.FindServers("Music Bot Server");
                    //foreach(Server server in voiceChannels)
                    //{

                    //}
                    // Finds the first VoiceChannel on the server ˓→'Music Bot Server'

                    // We use GetService to find˓→the AudioService that we installed earlier. In previous versions, this was˓→equivelent to _client.Audio()
                    //var _vClient = await client.GetService<AudioService>()       
                    //.Join(voiceChannel); // Join the Voice Channel, and return the IAudioClient.
                    await e.Channel.SendMessage("Hi");
                });
        }

        private void RegisterTrollCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("troll")
                .Alias(new string[] { "trolling", "trolled" })
                .Description("trolls everywhere and everyone on this server")
                .Parameter("BoolSwitch", ParameterType.Optional)
                .Do(async (e) =>
                {
                    if (deleting) DeleteCalling(e);

                    if (e.GetArg("BoolSwitch") != null) {
                        String args = e.GetArg("BoolSwitch").ToLower();
                        if (args.Equals("true"))
                        {
                            trolling = true;
                            ActivateTrollingMode(discord);
                            await e.Channel.SendMessage("Trolling activated!");
                        }
                        else if (args.Equals("false"))
                        {
                            trolling = false;
                            DeactivateTrollingMode(discord);
                            await e.Channel.SendMessage("Trolling deactivated!");
                        }
                    }
                    else
                    {
                        switch (trolling)
                        {
                            case true:
                                trolling = false;
                                DeactivateTrollingMode(discord);
                                await e.Channel.SendMessage("Trolling deactivated!");
                                break;

                            case false:
                                trolling = true;
                                ActivateTrollingMode(discord);
                                await e.Channel.SendMessage("Trolling activated!");
                                break;

                            default:
                                trolling = false;
                                DeactivateTrollingMode(discord);
                                break;
                        }
                    }

                    
                });
        }

        private void RegisterDeleteCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("delete")
                .Parameter("BoolSwitch", ParameterType.Optional)
                .Do(async (e) =>
                {
                    //if (deleting) DeleteCalling(e);

                    if (e.GetArg("BoolSwitch") != null)
                    {
                        String args = e.GetArg("BoolSwitch").ToLower();
                        if (args.Equals("true"))
                        {
                            deleting = true;
                            await e.Channel.SendMessage("Deleting activated!");
                        }
                        else if (args.Equals("false"))
                        {
                            deleting = false;
                            await e.Channel.SendMessage("Deleting deactivated!");
                        }
                    }
                    else
                    {
                        switch (deleting)
                        {
                            case true:
                                deleting = false;
                                await e.Channel.SendMessage("Deleting deactivated!");
                                break;

                            case false:
                                deleting = true;
                                await e.Channel.SendMessage("Deleting activated!");
                                break;

                            default:
                                deleting = false;
                                break;
                        }
                    }


                });
        }

        private void RegisterStopCommand(CommandService commands, DiscordClient discord)
        {
            commands.CreateCommand("stop")
                .Do(async (e) =>
                {
                    await e.Channel.SendMessage("Bye!");
                    await discord.Disconnect();
                    Environment.Exit(Environment.ExitCode);
                });
        }

        private void ActivateTrollingMode(DiscordClient discord)
        {
            discord.MessageReceived += async (s, e) =>
            {
                if (!e.Message.IsAuthor && trolling)
                {
                    await e.Channel.SendMessage(e.Message.User.Mention + " said " + e.Message.Text);
                }
            };
        }

        private void DeactivateTrollingMode(DiscordClient discord)
        {
            discord.MessageReceived -= (s, e) => {};
        }

        private async void DeleteCalling(CommandEventArgs e)
        {
            Message[] messageToDelete = await e.Channel.DownloadMessages(1);
            await e.Channel.DeleteMessages(messageToDelete);
        }


    }
}