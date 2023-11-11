namespace Nuvia.StateMachine.Effects;

public record StateTransitioned(string To) : IProcessEffect;