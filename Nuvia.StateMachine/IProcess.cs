namespace Nuvia.StateMachine;

using Nuvia.Messages;

public interface IProcess
{
    Task<ProcessResponse> Handle(ProcessInstanceState processInstanceState, IEvent @event, 
                            ProcessExecutionContext context);
}