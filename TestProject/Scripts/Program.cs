﻿using Johwa.Common.Debug;
using Johwa.Core;

internal class Program
{
    public static JohwaLogger debugger = new();
    public static DiscordBot bot = new DiscordBot(
        Environment.GetEnvironmentVariable("DISCORD_BOTTOKEN") 
            ?? throw new Exception("DISCORD_BOTTOKEN is not set.")
    );

    static async Task Main(string[] args)
    {
        await bot.Connect();
        await Task.Delay(-1);
    }
}