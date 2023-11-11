namespace Nuvia.StateMachine;

using Automatonymous;
using Automatonymous.Binders;
using Nuvia.Messages;
using Nuvia.StateMachine.Managers;

public abstract class Process<TProcessInstanceState> : AutomatonymousStateMachine<TProcessInstanceState>, IProcess
        where TProcessInstanceState : ProcessInstanceState, new()
{
        private List<IProcessEffectManager> processEffectManagers =
                new List<IProcessEffectManager>();

	public async Task<ProcessResponse> Handle(TProcessInstanceState processInstanceState, IEvent @event,
            ProcessExecutionContext context)
        {
                if (processInstanceState.LastEventApplied >= context.EventSerial)
                {
                        return new ProcessResponse(ProcessResponseStatus.ReplayedEvent, 
                                                        processInstanceState.LastEventApplied);
                }

                this.TrackSessionChanges();

                var stateTextAtStart = processInstanceState.StateText;
                if (!String.IsNullOrWhiteSpace(processInstanceState.StateText))
                {
                        var state = this.GetState(processInstanceState.StateText);
                        await this.TransitionToState(processInstanceState, state);
                }

                var eventTypes
                    = this.GetType()
                        .GetProperties()
                        .Where(p => p.PropertyType.IsGenericType &&
                                    p.PropertyType.GetGenericTypeDefinition() == typeof(Event<>) &&
                                    p.PropertyType.GetGenericArguments().Count() == 1 &&
                                    p.PropertyType.GetGenericArguments().First().Equals(@event.GetType())
                                )
                        .ToList();
                if (eventTypes.Count == 1)
                {
                        var eventInstance =
                                eventTypes.First()
                                        .GetValue(this, null);

                        //TODO: Fragile! Revisit.
                        var method =
                        typeof(RaiseEventExtensions).GetMethods()
                                                        .Where(m => m.Name == "RaiseEvent")
                                                        .Where(m => m.IsGenericMethodDefinition)
                                                        .Where(m => m.GetGenericArguments().Count() == 3)
                                                        .Where(m => m.GetGenericArguments()[0].Name == "T")
                                                        .Where(m => m.GetGenericArguments()[1].Name == "TData")
                                                        .Where(m => m.GetGenericArguments()[2].Name == "TInstance")
                                                        .FirstOrDefault();

                        try
                        {
                                //https://stackoverflow.com/questions/39674988/how-to-call-a-generic-async-method-using-reflection
                                var task = (Task)method.MakeGenericMethod(this.GetType(), @event.GetType(), processInstanceState.GetType())
                                        .Invoke(null, new object[] { this, processInstanceState, eventInstance!, @event, null });  

                                await task.ConfigureAwait(false);                                                                                                                                

                        // processInstanceState.StateText = processInstanceState.State.Name;

                                var stateTextChanged =
                                        stateTextAtStart != processInstanceState.State.Name;

                                var response =
                                        this.GetSessionChanges(processInstanceState, context, stateTextChanged);

                                return response;
                        }
                        catch(Automatonymous.UnhandledEventException ex)
                        {
                                //TODO: Log.
                                //TODO: Also catch Automatonymous.UnknownStateException
                                //... and/or pre-validate the valid states against the incoming state text.

                                //The event is part of what the process has a contract to handle but in the state that the
                                //process is currently in, it is not expecting the event.                        
                                return new ProcessResponse(ProcessResponseStatus.UnexpectedEvent, 
                                                                processInstanceState.LastEventApplied);                                
                        }
                }
                else //Matching event sinks not equal to 1. This should only happen if 
                    //it is zero i.e. unknown event sent in since duplicate event sink types would have been checked for
                    //when the process was uploaded
                    //TODO: Create another response status for when matching event sinks > 1.
                    //We shouldn't simply rely on the fact that a downstream process would have checked
                    //that case...
                {
                    return new ProcessResponse(ProcessResponseStatus.UnknownEvent,
                                                processInstanceState.LastEventApplied);
                }                
        }

        public async Task<ProcessResponse> Handle(ProcessInstanceState processInstanceState, IEvent @event,
                ProcessExecutionContext context)
        {
                var state = processInstanceState as TProcessInstanceState;
                if(state == default(TProcessInstanceState))
                {
                        return new ProcessResponse(ProcessResponseStatus.InvalidInitiationState,
                                                        processInstanceState.LastEventApplied);                    
                }
                else
                {
                        return await this.Handle(state, @event, context);
                }
        }

        private void TrackSessionChanges()
        {
            this.processEffectManagers.Clear();
        }   

        private ProcessResponse GetSessionChanges(ProcessInstanceState processInstanceState,
            ProcessExecutionContext context, bool stateTextChanged)
        {
                var changes =
                        this.processEffectManagers
                                .SelectMany(m => m.GetChanges())
                                .ToArray();

                //For some reason, when we use extension methods we created like ThenEnd to 
                //end the process, the transition manager doesn't pick it (putting a break point in it, it appears it gets called BEFORE the Handle method is even called at all), 
                //hence HasChanges below could resolve to
                //false if there is no other logic that executed e.g. if there was an if clause that didn't resolve to true
                //so the only activity was the transition/end. For this reason, we're also checking if the state text changed
                //because even in such scenarios, the state text actually changes after the call to invoke above. TODO: Investigate...
                if (!stateTextChanged && changes.Count() == 0)
                {
                        //The event is part of what the process has a contract to handle but in the state that the
                        //process is currently in, it is not expecting the event.                        
                        return new ProcessResponse(ProcessResponseStatus.UnexpectedEvent, 
                                                        processInstanceState.LastEventApplied);
                }
                else
                {

                        return new ProcessResponse(ProcessResponseStatus.Successful, 
                                                context.EventSerial,
                                                changes);
                }
        } 

        protected CommandBuilder<TCommand> Do<TCommand>()
            where TCommand : ICommand, new()
        {
            var builder =
                new CommandBuilder<TCommand>();

            this.processEffectManagers.Add(builder);

            return builder;
        } 

        protected EventBuilder<TEvent> Raise<TEvent>()
            where TEvent : IEvent, new()
        {
            var builder =
                new EventBuilder<TEvent>();

            this.processEffectManagers.Add(builder);

            return builder;
        }     

        //Replacing 'During' with 'While' to make it read more fluently.
        protected void While(State state, params EventActivities<TProcessInstanceState>[] activities)
        {
            During(state, activities);
        }

        internal void TrackTransition(string toState)
        {
            var transitionManager =
                new TransitionManager();

            transitionManager.TrackTransition(toState);

            this.processEffectManagers.Add(transitionManager);
        }

        //TODO: Shouldn't we use the same transition manager throughout?
        //Or just dispense with the managers altogether and track the transition effects directly here?
        internal void TrackCompletion()
        {
            var transitionManager =
                new TransitionManager();

            transitionManager.TrackCompletion();

            this.processEffectManagers.Add(transitionManager);
        }                              
}
