//TODO: Change the versioning scheme to use major and minor versions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nuvia.StateMachine;
using Nuvia.Messages;

using Automatonymous;

namespace Nuvia.Snippets;

public class TestProcess : Process<ProcessInstanceState>
{
    private (ProcessInstanceState, IEvent) Extract
        (BehaviorContext<ProcessInstanceState, IEvent> context) => (context.Instance, context.Data);

    public TestProcess()
    {
        Event(() => ProcessStarted);
        Event(() => Stage2Processed);

        State(() => AwaitingStage2);

        Initially(
            When(ProcessStarted)
                .Then((context) =>
                {
                    var (p, e) = Extract(context);

                   /*  Do<CreateTaskForRole>()
                            .With(c => c.Role = "TL")
                            .With(c => c.ForBusinessUnit = p.OriginatingBusinessUnit)
                            .With(c => c.TaskTemplate = "commercial-loan-review-request")
                            .With(c => c.WorkItem = p.WorkItem); */

             /*        Do<SendEmail>()
                            .With(c => c.To = Group("TL").For(p.OriginatingBusinessUnit))
                            .With(c => c.Cc = Group("RO").For(p.OriginatingBusinessUnit))
                            .With(c => c.MailTemplate = "pending-approval-notification")
                            .With(c => c.MailInfo = new
                            {
                                WorkItemID = p.WorkItemID,
                                WorkItemName = p.WorkItemName,
                                BusinessProcess = ProcessDisplayName,
                                AccountName = p.AccountName,
                                AccountNumber = p.AccountNumber,
                                Comments = e.Comments
                            }); */

                    this.TransitionTo(p, AwaitingStage2);
                })
        );

        While(AwaitingStage2,

            When(Stage2Processed)
                .Then((context) =>
                {
                    var (p, e) = Extract(context);

                    /* Do<CreateTaskForRole>()
                            .With(c => c.Role = "TL")
                            .With(c => c.ForBusinessUnit = p.OriginatingBusinessUnit)
                            .With(c => c.TaskTemplate = "agric-loan-review-request")
                            .With(c => c.WorkItem = p.WorkItem); */

                   // this.TransitionTo(p, Awaiting_Team_Lead_Review);

                   this.End(p);

                })//.ThenEnd()
        );        
    }

    #region States

    public Automatonymous.State AwaitingStage2 { get; private set; }

    #endregion

    #region Events

    public Event<ProcessStarted> ProcessStarted { get; private set; }

    public Event<Stage2Processed> Stage2Processed { get; private set; }    

    #endregion
}