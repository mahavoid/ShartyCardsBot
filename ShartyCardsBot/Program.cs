using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShartyCardsBot.Extensions;
using ShartyCardsBot.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace ShartyCardsBot
{
    internal class Program
    {
        static ITelegramBotClient TgBot;

        static ILogger Logger;
        static IConfiguration Config;

        static IPictureGenerator PictureGenerator;

        static void Main()
        {
            initializeDependancies();
            initializeBot();

            while (true)
            {
                Thread.Sleep(1 * 60 * 1000);
            }
        }

        private static void initializeDependancies()
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile("appsettings.json", false);
            IConfigurationRoot config = configBuilder.Build();

            var botToken = config.GetValue(typeof(string), "BotToken");

            var builder = new ContainerBuilder();

            //this adds logging to console
            #region console logging
            IServiceCollection services = new ServiceCollection();
            services.AddLogging((c) => c.AddSimpleConsole(
                (conf) =>
                {
                    conf.ColorBehavior = Microsoft.Extensions.Logging.Console.LoggerColorBehavior.Disabled;
                    conf.TimestampFormat = "yyyy-MM-dd HH:mm:ss:fff";
                })
            );
            builder.Populate(services);
            #endregion

            builder.Register(context => config).As<IConfiguration>();

            builder.RegisterType<TelegramBotClient>().As<ITelegramBotClient>().WithParameter("token", config["BotToken"]).SingleInstance();

            builder.RegisterType<PictureGenerator>().As<IPictureGenerator>().SingleInstance();

            var container = builder.Build();

            TgBot = container.Resolve<ITelegramBotClient>();
            Logger = container.Resolve<ILogger<Program>>();
            Config = container.Resolve<IConfiguration>();
            PictureGenerator = container.Resolve<IPictureGenerator>();
        }

        private static void initializeBot()
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = new[] { UpdateType.Message }
            };

            TgBot.StartReceiving(
                tgBotUpdateAsync,
                tgBotErrorAsync,
                receiverOptions);
        }

        private static Task tgBotErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Logger.LogInformation(ErrorMessage);

            return Task.CompletedTask;
        }

        private static async Task tgBotUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (!update.IsValidForBot())
                return;

            PictureGenerationOptions pgo = new PictureGenerationOptions(update);
            InputOnlineFile iof = PictureGenerator.Generate(pgo);

            await botClient.SendPhotoAsync(update.Message.Chat.Id, iof, replyToMessageId: update.Id);
        }
    }
}
