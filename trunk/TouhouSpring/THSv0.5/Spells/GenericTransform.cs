using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouhouSpring.Behaviors;

namespace TouhouSpring.THSv0_5.Spells
{
    public sealed class GenericTransform : BaseBehavior<GenericTransform.ModelType>,
        ICastableSpell,
        ILocalPrerequisiteTrigger<Commands.CastSpell>
    {
        CommandResult ILocalPrerequisiteTrigger<Commands.CastSpell>.RunLocalPrerequisite(Commands.CastSpell command)
        {
            Game.NeedMana(Model.ManaCost);
            IEnumerable<CardInstance> candidates = Host.Owner.CardsOnBattlefield;
            if (Model.OfCertainType.Value != null)
            {
                candidates = candidates.Where(card => card.Model == Model.OfCertainType.Value);
            }
            Game.NeedTargets(this, candidates, "选择一张卡转化。");
            if (Model.ToTypeChoiceA.Value != null
                && Model.ToTypeChoiceB.Value != null)
            {
                Game.NeedInteraction(this, 1, true,
                    new Interactions.SelectCardModel(Host.Owner,
                                                     new ICardModel[] { Model.ToTypeChoiceA.Value, Model.ToTypeChoiceB.Value },
                                                     "选择转化形态。"));
            }
            return CommandResult.Pass;
        }

        void ICastableSpell.RunSpell(Commands.CastSpell command)
        {
            var target = Game.GetTargets(this)[0];
            ICardModel toType = null;
            if (Model.ToTypeChoiceA.Value != null
                && Model.ToTypeChoiceB.Value != null)
            {
                toType = Game.GetInteractionResult(this, 1) as ICardModel;
            }
            else
            {
                toType = Model.ToTypeChoiceA.Value ?? Model.ToTypeChoiceB.Value;
            }
            Game.QueueCommands(new Commands.Transform(target, toType));
        }

        [BehaviorModel(typeof(GenericTransform), Category = "v0.5/Spell", DefaultName = "转化")]
        public class ModelType : BehaviorModel
        {
            public int ManaCost { get; set; }
            public CardModelReference OfCertainType { get; set; }
            public CardModelReference ToTypeChoiceA { get; set; }
            public CardModelReference ToTypeChoiceB { get; set; }

            public ModelType()
            {
                OfCertainType = new CardModelReference();
                ToTypeChoiceA = new CardModelReference();
                ToTypeChoiceB = new CardModelReference();
            }
        }
    }
}
