using Nuvia.Snippets;
using Nuvia.StateMachine;

using System.Text.Json;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var process = new TestProcess();
var state = new ProcessInstanceState()
{
    StateText = "AwaitingStage3"
};
var @event = new ProcessStarted();
var @event2 = new Stage2Processed();
var context = new ProcessExecutionContext(Guid.Empty, new EventSerial(0, 1));

var response = await process.Handle(state, @event2, context);
var responseJson = JsonSerializer.Serialize(response);
Console.WriteLine(responseJson);
