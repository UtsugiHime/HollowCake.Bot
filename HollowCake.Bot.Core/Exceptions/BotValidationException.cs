namespace HollowCake.Bot.Core.Exceptions;

public class BotValidationException(string message = "Validation failed.", Exception? inner = null)
    : BotException(message, inner)
{
    protected override string DefaultUserMessage => "The requirements for this operation were not met.";
}