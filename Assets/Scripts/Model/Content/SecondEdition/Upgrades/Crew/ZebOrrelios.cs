using ActionsList;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ZebOrrelios : GenericUpgrade
    {
        public ZebOrrelios() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Zeb\" Orrelios",
                UpgradeType.Crew,
                cost: 1,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.ZebOrreliosCrewAbility),
                seImageNumber: 94
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(450, 5),
                new Vector2(150, 150)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ZebOrreliosCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Rules.DiceModification.OnAllowRangeZeroAttackModifications += AllowRange0FocusModification;
        }

        public override void DeactivateAbility()
        {
            Rules.DiceModification.OnAllowRangeZeroAttackModifications -= AllowRange0FocusModification;
        }

        private void AllowRange0FocusModification(GenericAction action, ref bool allowed)
        {
            allowed = allowed ||
                action is FocusAction &&
                (Combat.Attacker == HostShip || Combat.Defender == HostShip);
        }
    }
}
