using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using TelegramBot.Api.Domain;
using TelegramBot.Api.Extensions;
using TelegramBot.Api.Handlers.Abstractions;
using TelegramBot.Api.Models;
using TelegramBot.Api.Options;
using TelegramBot.Api.UpdateSpecifications;

namespace TelegramBot.Api.Handlers;

public class RegistrationHandler : IFsmHandler
{
    private const string RegisterWithUsernameAndPassword = "RegisterWithUsernameAndPassword";

    private readonly IHttpClientFactory _httpClientFactory;

    public RegistrationHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public void AddTransitionsToFsmOptions(FsmOptions config)
    {
        config.From(GlobalStates.Any).With<RegistrationUpdateSpecification>().To<RegistrationHandler>();
        config.From(RegisterWithUsernameAndPassword).With<NotCommandUpdateSpecification>().To<RegistrationHandler>();
    }

    public async Task<string> HandleAsync(UpdateContext context)
    {
        string? messageText = context.Update.Message?.Text;
        long? userId = context.SafeUserId;

        switch (context.Key)
        {
            case (GlobalStates.Any, RegistrationUpdateSpecification):
                await context.BotClient.SendTextMessageAsync(userId!, "Enter <i>username:email:password</i> to register", ParseMode.Html);
                return RegisterWithUsernameAndPassword;
            case (RegisterWithUsernameAndPassword, NotCommandUpdateSpecification):
                if (messageText != null)
                {
                    string[] msgTextParts = messageText.Split(':');
                    var registrationModel = new
                    {
                        username = msgTextParts[0].Trim(),
                        email = msgTextParts[1].Trim(),
                        password = msgTextParts[2].Trim(),
                        passwordConfirmation = msgTextParts[2].Trim()
                    };

                    // todo: validate registrationModel

                    StringContent content = new(JsonConvert.SerializeObject(registrationModel), Encoding.UTF8, MediaTypeNames.Application.Json);

                    var authClient = _httpClientFactory.CreateClient();
                    authClient.BaseAddress = new System.Uri("https://idsrv");
                    await authClient.PostAsync("/auth/register", content);
                    await context.BotClient.SendTextMessageAsync(userId!, $"Registration succeeded", ParseMode.MarkdownV2);
                }
                return GlobalStates.Any;
        }

        return GlobalStates.Any;
    }
}