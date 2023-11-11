using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Automatonymous.Binders;

using Nuvia.StateMachine;

namespace Automatonymous
{
    public static class AutomatonymousExtensions
    {
        //TODO: .End works (i.e. adds the ProcessEffect) but this as .ThenEnd doesn't for some reason. Investigate.
/*         public static EventActivityBinder<TInstance, TData> ThenEnd<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source) where TInstance : ProcessInstanceState, new()
        {
            source =
                source.TransitionTo(source.StateMachine.Final);

            var stateMachine =
                source.StateMachine as Process<TInstance>;   
                Console.WriteLine("Here 111");          
            stateMachine.TrackCompletion();

            return source;
        } */

        //This will be translated from WaitFor [State without the 'Awaiting' Prefix] in the DSL e.g. WaitFor UserRegistration
        //in the DSL will be translated to WaitAt(AwaitingUserRegistration) in code.
  /*       public static EventActivityBinder<TInstance, TData> WaitAt<TInstance, TData>(
            this EventActivityBinder<TInstance, TData> source, State state) where TInstance : ProcessInstanceState, new()
        {
            var newSource =
                source.TransitionTo(state);

            var stateMachine =
                newSource.StateMachine as Process<TInstance>;
            stateMachine.TrackTransition(state.Name);

            return newSource;
        } */

        public static void TransitionTo<TInstance>(this StateMachine<TInstance> stateMachine, TInstance instance, State state,
            CancellationToken cancellationToken = default(CancellationToken)) where TInstance : ProcessInstanceState, new()
        {
            (stateMachine as Process<TInstance>).TrackTransition(state.Name);
            stateMachine.TransitionToState(instance, state, cancellationToken).Wait();
        }

        public static void End<TInstance>(this StateMachine<TInstance> stateMachine, TInstance instance,
            CancellationToken cancellationToken = default(CancellationToken)) where TInstance : ProcessInstanceState, new()
        {
            var sm =
                stateMachine as Process<TInstance>;

            sm.TransitionToState(instance, stateMachine.Final, cancellationToken).Wait();
            sm.TrackCompletion();
        }
    }
}
