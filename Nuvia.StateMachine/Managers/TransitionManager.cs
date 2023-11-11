using Nuvia.StateMachine.Effects;

namespace Nuvia.StateMachine.Managers;

public class TransitionManager : IProcessEffectManager
{
    private bool completionTracked = false;

    private List<IProcessEffect> effects =
        new List<IProcessEffect>();

    internal void TrackTransition(string toState)
    {
        if(completionTracked)
        {
            //TODO: return or throw exception that transition cannot be tracked when completion has already been tracked.
        }

        effects.Add(new StateTransitioned(To: toState));
    }

    internal void TrackCompletion()
    {
        if(completionTracked)
        {
            //TODO: return or throw exception that completion cannot be tracked multiple times.
        }

        effects.Add(new Completed());

        completionTracked = true;        
    }

    public IProcessEffect[] GetChanges()
    {
        return effects.ToArray();
    }
}