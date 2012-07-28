using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Commands;

namespace TouhouSpring
{
    public partial class BaseController
        : IEpilogTrigger<DrawCard>
    {
        void IEpilogTrigger<DrawCard>.Run(CommandContext context)
        {
            new Interactions.NotifyCardEvent(this, "OnCardDrawn", (context.Command as DrawCard).CardDrawn).Run();
        }
    }
}
