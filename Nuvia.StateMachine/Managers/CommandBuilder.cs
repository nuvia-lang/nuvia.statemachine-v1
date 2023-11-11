using Nuvia.Messages;
using Nuvia.StateMachine.Effects;

namespace Nuvia.StateMachine.Managers;

public abstract class CommandBuilder : IProcessEffectManager
{
    protected ICommand command;

    protected CommandBuilder(ICommand command)
    {
        this.command = command;
    }

    public IProcessEffect[] GetChanges()
    {
        return new CommandGenerated[] {
            new CommandGenerated(this.command)
        };
    }
}

public class CommandBuilder<TCommand> : CommandBuilder
    where TCommand : ICommand, new()
{
    internal CommandBuilder()
        : base(new TCommand())
    {
        
    }

    //TODO: Use a convention-based approach to automatically set properties that have names matching with 
    //the event first then the process second to reduce boiler plate code and hence the cognitive load when reading the code.
    //Only use this ('With') method to explicitly set some properties. If a command property has a similarly-named
    //event AND process property or if a command property has NEITHER a similarly-named event or process
    //property, show a build warning or so ...
    public CommandBuilder<TCommand> With(Action<TCommand> action)
    {
        action((TCommand)this.command);

        return this;
    }
}