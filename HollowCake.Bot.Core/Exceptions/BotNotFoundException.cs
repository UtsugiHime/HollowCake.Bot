namespace HollowCake.Bot.Core.Exceptions;

public sealed class BotNotFoundException(string message = "The requested resource was not found.", Exception? inner = null)
    : BotException(message, inner)
{
    protected override string DefaultUserMessage => "The requested resource could not be found.";
}