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
                        new Commands.DealDamageToPlayer
                        {
                            Target = OpponentPlayer,
                            DamageToDeal = attackerWarriorBhv.Attack,
                            Cause = attackerWarriorBhv
                        },
                        new Commands.SendBehaviorMessage
                        {
                            Target = attackerWarriorBhv,
                            Message = "GoCoolingDown"
                        });
                }
                else if (blockers.Count == 1)
                {
                    var blocker = blockers[0];
                    var blockerWarriorBhv = blocker.Behaviors.Get<Behaviors.Warrior>();

                    var preDamageOnAttacker = new Triggers.PreCardDamageContext(this, attacker, blockerWarriorBhv.Attack, blockerWarriorBhv);
                    var preDamageOnBlocker = new Triggers.PreCardDamageContext(this, blocker, attackerWarriorBhv.Attack, attackerWarriorBhv);

                    TriggerGlobal(preDamageOnAttacker);
                    TriggerGlobal(preDamageOnBlocker);

                    attackerWarriorBhv.AccumulatedDamage += preDamageOnAttacker.DamageToDeal;
                    blockerWarriorBhv.AccumulatedDamage += preDamageOnBlocker.DamageToDeal;

                    TriggerGlobal(new Triggers.PostCardDamagedContext(this, attacker, preDamageOnAttacker.DamageToDeal, blockerWarriorBhv));
                    TriggerGlobal(new Triggers.PostCardDamagedContext(this, blocker, preDamageOnBlocker.DamageToDeal, attackerWarriorBhv));

                    IssueCommands(
                        new Commands.SendBehaviorMessage
                        {
                            Target = attackerWarriorBhv,
                            Message = "GoCoolingDown"
                        },
                        new Commands.SendBehaviorMessage
                        {
                            Target = blockerWarriorBhv,
                            Message = "GoCoolingDown"
                        });
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

        private void ResolveBattlefieldCards()
        {
            foreach (var player in Players)
            {
                for (int i = 0; i < player.m_battlefieldCards.Count; i++)
                {
                    var card = player.m_battlefieldCards[i];
                    if (!card.Behaviors.Has<Behaviors.Warrior>())
                        continue;
                    var warrior = card.Behaviors.Get<Behaviors.Warrior>();
                    if (warrior.Defense - warrior.AccumulatedDamage <= 0)
                    {
                        DestroyCard(card);
                        i--;
                    }
                }
            }
        }

        private void ResetAccumulatedDamage()
        {
            foreach (var player in Players)
            {
                foreach (var card in player.m_battlefieldCards)
                {
                    if (!card.Behaviors.Has<Behaviors.Warrior>())
                        continue;
                    var warrior = card.Behaviors.Get<Behaviors.Warrior>();
                    warrior.AccumulatedDamage = 0;
                }
            }
        }
    }
}
