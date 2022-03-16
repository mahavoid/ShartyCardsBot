using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ShartyCardsBot.Extensions
{
    public static class UpdateExtensions
    {
        public static bool IsValidForBot(this Update update)
        {
            if (update.Type != UpdateType.Message
                || update.Message == null
                || update.Message.Type != MessageType.Text
                || update.Message.Text == null
                || update.Message.From.IsBot
                || update.Message.Chat.Type != ChatType.Private)
                return false;

            return true;
        }
    }
}
