using Nuvia.StateMachine.Effects;

namespace Nuvia.StateMachine.Managers;

public interface IProcessEffectManager
{
    IProcessEffect[] GetChanges();
}