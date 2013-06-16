using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Commands
{
    public enum PlayerModifier
    {
        AddMana,
        SubtractMana,
        AddLife,
        SubtractLife
    }

    public class AddPlayerModifier : BaseCommand
    {
        public Player Player
        {
            get; private set;
        }

        public PlayerModifier ModifierType
        {
            get; private set;
        }

        public ValueModifier Modifier
        {
            get; private set;
        }

        public AddPlayerModifier(Player player, PlayerModifier modifierType, ValueModifier modifier)
        {
            if (player == null)
            {
                throw new ArgumentNullException("player");
            }
            else if (modifier == null)
            {
                throw new ArgumentNullException("modifier");
            }

            Player = player;
            ModifierType = modifierType;
            Modifier = modifier;
        }

        internal override void ValidateOnIssue()
        {
            Validate(Player);
        }

        internal override bool ValidateOnRun()
        {
            return Modifier != null;
        }

        internal override void RunMain()
        {
            switch (ModifierType)
            {
                case PlayerModifier.AddMana:
                    Player.m_manaAddModifiers.Add(Modifier);
                    break;
                case PlayerModifier.SubtractMana:
                    Player.m_manaSubtractModifiers.Add(Modifier);
                    break;
                case PlayerModifier.AddLife:
                    Player.m_lifeAddModifiers.Add(Modifier);
                    break;
                case PlayerModifier.SubtractLife:
                    Player.m_lifeSubtractModifiers.Add(Modifier);
                    break;
                default:
                    break;
            }
        }
    }
}
