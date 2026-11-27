namespace HollowCake.Bot.Core.Exceptions;

public sealed class BotConflictException(string message = "A resource conflict occurred.", Exception? inner = null)
    : BotException(message, inner)
{
    protected override string DefaultUserMessage => "This operation conflicts with the current state.";
}