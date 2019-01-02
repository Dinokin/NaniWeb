using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace NaniWeb.Others.Services
{
    public class DiscordBot
    {
        private readonly SettingsKeeper _settingsKeeper;
        private DiscordSocketClient _discordSocketClient;

        public DiscordBot(SettingsKeeper settingsKeeper)
        {
            _settingsKeeper = settingsKeeper;
        }

        public async Task SendMessage(string message)
        {
            if (bool.Parse(_settingsKeeper.GetSetting("EnableDiscordBot").Value))
            {
                await Connect();
                var channel = _discordSocketClient?.GetChannel(ulong.Parse(_settingsKeeper.GetSetting("DiscordChannelId").Value)) as SocketTextChannel;
                channel?.SendMessageAsync(message);
            }
        }

        private async Task Connect()
        {
            if (_discordSocketClient == null || _discordSocketClient.ConnectionState != ConnectionState.Connected)
            {
                _discordSocketClient?.Dispose();
                _discordSocketClient = new DiscordSocketClient();
                await _discordSocketClient.LoginAsync(TokenType.Bot, _settingsKeeper.GetSetting("DiscordToken").Value);
                await _discordSocketClient.StartAsync();
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}