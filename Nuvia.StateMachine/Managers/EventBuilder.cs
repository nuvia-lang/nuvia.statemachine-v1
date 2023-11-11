using Nuvia.Messages;
using Nuvia.StateMachine.Effects;

namespace Nuvia.StateMachine.Managers;

public abstract class EventBuilder : IProcessEffectManager
{
    protected IEvent @event;

    protected EventBuilder(IEvent @event)
    {
        this.@event = @event;
    }

    public IProcessEffect[] GetChanges()
    {
        return new EventRaised[] {
            new EventRaised(this.@event)
        };
    }
}

public class EventBuilder<TEvent> : EventBuilder
    where TEvent : IEvent, new()
{
    internal EventBuilder()
        : base(new TEvent())
    {
        
    }

    public EventBuilder<TEvent> With(Action<TEvent> action)
    {
        action((TEvent)this.@event);

        return this;
    }
}