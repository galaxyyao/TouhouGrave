using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public partial class Game
    {
        private void ResolveCombat(IIndexable<BaseCard> declaredAttackers, IIndexable<IIndexable<BaseCard>> declaredBlockers)
        {
            for (int i = 0; i < declaredAttackers.Count; ++i)
            {
                BaseCard attacker = declaredAttackers[i];
                var attackerWarriorBhv = attacker.Behaviors.Get<Behaviors.Warrior>();

                IIndexable<BaseCard> blockers = declaredBlockers[i];

                if (blockers.Count == 0)
                {
                    IssueCommands(
                        // TODO: attackers be declared on some target player
                        new Commands.DealDamageToPlayer(ActingPlayerEnemies.First(), attackerWarriorBhv, attackerWarriorBhv.Attack),
                        new Commands.SendBehaviorMessage(attackerWarriorBhv, "GoCoolingDown", null));
                }
                else if (blockers.Count == 1)
                {
                    var blocker = blockers[0];
                    var blockerWarriorBhv = blocker.Behaviors.Get<Behaviors.Warrior>();

                    IssueCommands(
                        new Commands.DealDamageToCard(attacker, blockerWarriorBhv, blockerWarriorBhv.Attack),
                        new Commands.DealDamageToCard(blocker, attackerWarriorBhv, attackerWarriorBhv.Attack),
                        new Commands.SendBehaviorMessage(attackerWarriorBhv, "GoCoolingDown", null),
                        new Commands.SendBehaviorMessage(blockerWarriorBhv, "GoCoolingDown", null));
                }
                //else if (blockers.Count == 2)
                //{
                //    var blocker1 = blockers[0];
                //    var blocker1WarriorBhv = blocker1.Behaviors.Get<Behaviors.Warrior>();
                //    int damageOnBlocker1 = Math.Min(attackerWarriorBhv.Attack, blocker1WarriorBhv.Defense);

                //    var blocker2 = blockers[1];
                //    var blocker2WarriorBhv = blocker2.Behaviors.Get<Behaviors.Warrior>();
                //    int damageOnBlocker2 = attackerWarriorBhv.Attack - damageOnBlocker1;

                //    {
                //        var preDamageOnAttacker = new Triggers.PreCardDamageContext(this, attacker, blocker1WarriorBhv.Attack, blocker1WarriorBhv);
                //        var preDamageOnBlocker = new Triggers.PreCardDamageContext(this, blocker1, damageOnBlocker1, attackerWarriorBhv);

                //        TriggerGlobal(preDamageOnAttacker);
                //        TriggerGlobal(preDamageOnBlocker);

                //        attackerWarriorBhv.AccumulatedDamage += preDamageOnAttacker.DamageToDeal;
                //        blocker1WarriorBhv.AccumulatedDamage += preDamageOnBlocker.DamageToDeal;

                //        TriggerGlobal(new Triggers.PostCardDamagedContext(this, attacker, preDamageOnAttacker.DamageToDeal, blocker1WarriorBhv));
                //        TriggerGlobal(new Triggers.PostCardDamagedContext(this, blocker1, preDamageOnBlocker.DamageToDeal, attackerWarriorBhv));

                //        blocker1.State = CardState.CoolingDown;
                //    }

                //    if (attackerWarriorBhv.AccumulatedDamage <= attackerWarriorBhv.Defense)
                //    {
                //        var preDamageOnAttacker = new Triggers.PreCardDamageContext(this, attacker, blocker2WarriorBhv.Attack, blocker2WarriorBhv);
                //        var preDamageOnBlocker = new Triggers.PreCardDamageContext(this, blocker2, damageOnBlocker2, attackerWarriorBhv);

                //        TriggerGlobal(preDamageOnAttacker);
                //        TriggerGlobal(preDamageOnBlocker);

                //        attackerWarriorBhv.AccumulatedDamage += preDamageOnAttacker.DamageToDeal;
                //        blocker2WarriorBhv.AccumulatedDamage += preDamageOnBlocker.DamageToDeal;

                //        TriggerGlobal(new Triggers.PostCardDamagedContext(this, attacker, preDamageOnAttacker.DamageToDeal, blocker2WarriorBhv));
                //        TriggerGlobal(new Triggers.PostCardDamagedContext(this, blocker2, preDamageOnBlocker.DamageToDeal, attackerWarriorBhv));

                //        blocker2.State = CardState.CoolingDown;
                //    }

                //    attacker.State = CardState.CoolingDown;
                //}
                else
                {
                    throw new NotSupportedException("Currently only two blockers can be declared on one single attacker.");
                }
            }

            FlushCommandQueue();
        }
    }
}
