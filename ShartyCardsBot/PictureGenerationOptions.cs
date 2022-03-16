using Telegram.Bot.Types;

namespace ShartyCardsBot
{
    public class PictureGenerationOptions
    {
        private Update update;

        public PictureGenerationOptions(Update update)
        {
            this.update = update;
        }
    }
}