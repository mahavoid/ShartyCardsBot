using Telegram.Bot.Types.InputFiles;

namespace ShartyCardsBot.Services
{
    public interface IPictureGenerator
    {
        InputOnlineFile Generate(PictureGenerationOptions pgo);
    }
}