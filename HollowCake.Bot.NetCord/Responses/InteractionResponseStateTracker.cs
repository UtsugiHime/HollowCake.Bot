using System.Runtime.CompilerServices;
using NetCord;

namespace HollowCake.Bot.NetCord.Responses;

internal sealed class InteractionResponseStateTracker
{
    private sealed record Entry(InteractionResponseState State);

    private readonly ConditionalWeakTable<Interaction, Entry> _states = new();

    public InteractionResponseState GetState(Interaction interaction)
        => _states.TryGetValue(interaction, out var entry)
            ? entry.State
            : InteractionResponseState.Unknown;
    
    public void SetState(Interaction interaction, InteractionResponseState state)
        => _states.AddOrUpdate(interaction, new Entry(state));
}