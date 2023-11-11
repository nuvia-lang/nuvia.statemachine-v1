using Nuvia.Messages;

namespace Nuvia.StateMachine.Effects;

public record EventRaised(IEvent Event) : IProcessEffect;