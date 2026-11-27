namespace HollowCake.Bot.Core.Exceptions;

public sealed class BotExternalServiceException(string message = "An external service failed.", Exception? inner = null)
    : BotException(message, inner)
{
    protected override string DefaultUserMessage => "An external service encountered an error. Please try again later.";
}