namespace Nuvia.StateMachine;

using Automatonymous;

public class ProcessInstanceState
{
    public State State
    {
        get;
        set;
    }        

    public string StateText
    {
        get;
        set;
    }      

    public EventSerial LastEventApplied
    {
        get;
        set;
    }      
}