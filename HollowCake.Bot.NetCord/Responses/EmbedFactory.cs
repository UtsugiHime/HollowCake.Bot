using NetCord;
using NetCord.Rest;

namespace HollowCake.Bot.NetCord.Responses;

public sealed class EmbedFactory(ResponseOptions options)
{
    public EmbedProperties Success(string title, string description) 
        => Create(title, description, options.SuccessColor);
    
    public EmbedProperties Error(string title, string description) 
        => Create(title, description, options.ErrorColor);
    
    public EmbedProperties Warning(string title, string description) 
        => Create(title, description, options.WarningColor);
    
    public EmbedProperties Info(string title, string description) 
        => Create(title, description, options.InfoColor);
    
    public EmbedProperties Create(string title, string description, Color color)
        => new EmbedProperties().WithTitle(title).WithDescription(description).WithColor(color);
}