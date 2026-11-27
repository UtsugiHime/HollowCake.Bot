namespace HollowCake.Bot.Core.Exceptions;

public sealed class BotInvalidInputException(string message = "Invalid input was provided.", Exception? inner = null)
    : BotValidationException(message, inner)
{
    protected override string DefaultUserMessage => "The provided input is invalid.";
}