using Nuvia.Messages;
using Nuvia.StateMachine.Effects;

namespace Nuvia.StateMachine;


public record ProcessResponse(
    ProcessResponseStatus Status,
    EventSerial LastEventApplied
)
{
    private readonly IProcessEffect[] appliedEffects = new IProcessEffect[] {};

    //Explicit constructors are expected to use camelCase. Positional properties are expected to use PascalCase
    //https://github.com/DotNetAnalyzers/StyleCopAnalyzers/issues/3168
    public ProcessResponse(ProcessResponseStatus status,
                            EventSerial lastEventApplied, IProcessEffect[] appliedEffects)
                            : this(status, lastEventApplied)
    {
        this.appliedEffects = appliedEffects ?? new IProcessEffect[] {};
    }  

    public IProcessEffect[] AppliedEffects => appliedEffects;

    public ICommand[] EmittedCommands
    {
        get
        {
            return AppliedEffects
                    .OfType<CommandGenerated>()
                    .Select(e => e.Command)
                    .ToArray();
        }
    }

    public IEvent[] EmittedEvents
    {
        get
        {
            return AppliedEffects
                    .OfType<EventRaised>()
                    .Select(e => e.Event)
                    .ToArray();
        }
    }

    public string[] EmittedTransitions
    {
        get
        {
            return AppliedEffects
                    .OfType<StateTransitioned>()
                    .Select(t => t.To)
                    .ToArray();
        }
    }    

    public bool HasChanges
    {
        get
        {
            return AppliedEffects.Count() > 0;
        }
    }

    public bool IsComplete
    {
        get
        {
            return AppliedEffects
                    .OfType<Completed>()
                    .Count() > 0;
        }
    }      
}