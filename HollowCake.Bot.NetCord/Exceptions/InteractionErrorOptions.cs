using NetCord;

namespace HollowCake.Bot.NetCord.Exceptions;

public class InteractionErrorOptions
{
    public string Title { get; set; } = "Error";
    public Color Color { get; set; } = new(0xED4245);
    
    public string UnexpectedErrorMessage { get; set; } = "An unexpected error occurred. Please try again later.";
    public string ExecutionFailedMessage { get; set; } = "Unable to complete this interaction. Please try again later.";
}