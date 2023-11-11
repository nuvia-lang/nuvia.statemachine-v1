namespace Nuvia.StateMachine;

//TODO: Implement in F#, dispense with the enum and use a discriminated union
public enum ProcessResponseStatus
{
    ///The state machine is aware of the event but in the state the machine was in, it was not
    ///expecting the event.
    UnexpectedEvent = 1,

    ///The event is unknown to the state machine.
    UnknownEvent = 2,

    ///The event is a replay of a prior event.
    ReplayedEvent = 3,

    ///The state the state machine was instantiated with is invalid.
    InvalidInitiationState = 4,

    ///The state machine execution was successful.
    Successful = 9999
}