using Nuvia.Messages;

namespace Nuvia.StateMachine.Effects;

public record CommandGenerated(ICommand Command) : IProcessEffect;