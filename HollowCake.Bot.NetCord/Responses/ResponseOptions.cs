using NetCord;

namespace HollowCake.Bot.NetCord.Responses;

public sealed class ResponseOptions
{
    public Color SuccessColor { get; set; } = new(0x57F287);
    public Color ErrorColor { get; set; } = new(0xED4245);
    public Color WarningColor { get; set; } = new(0xFEE75C);
    public Color InfoColor { get; set; } = new(0x5865F2);
}