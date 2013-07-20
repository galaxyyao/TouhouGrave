using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Interactions
{
    public class SelectCardModel : BaseInteraction, IQuickInteraction
    {
        public Player Player
        {
            get; private set;
        }

        public string Message
        {
            get; private set;
        }

        public IIndexable<ICardModel> Candidates
        {
            get; private set;
        }

        public SelectCardModel(Player player, IEnumerable<ICardModel> candidates, string message)
            : base(player.Game)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (candidates == null)
            {
                throw new ArgumentNullException("candidates");
            }

            Player = player;
            Candidates = candidates.ToList().ToIndexable();
            Message = message ?? String.Empty;
        }

        public ICardModel Run()
        {
            var result = Candidates.Count != 0
                         ? NotifyAndWait<ICardModel>()
                         : null;
            Validate(result);
            return result;
        }

        public void Respond(ICardModel selectedCard)
        {
            Validate(selectedCard);
            RespondBack(selectedCard);
        }

        object IQuickInteraction.Run()
        {
            return Run();
        }

        bool IQuickInteraction.HasCandidates()
        {
            return Candidates.Count != 0;
        }

        private void Validate(ICardModel selectedCard)
        {
            if (selectedCard != null && !Candidates.Contains(selectedCard))
            {
                throw new InteractionValidationFailException("Selection is not candidate.");
            }
        }
    }
}
