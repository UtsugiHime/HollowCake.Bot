namespace HollowCake.Bot.Core.Exceptions;

public sealed class BotPermissionException(string message = "Insufficient permissions.", Exception? inner = null)
    : BotException(message, inner)
{
    protected override string DefaultUserMessage => "You do not have permission to perform this operation.";
}