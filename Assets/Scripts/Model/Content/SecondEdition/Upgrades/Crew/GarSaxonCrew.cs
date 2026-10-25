using Abilities.SecondEdition;
using Content;
using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class GarSaxonCrew : GenericUpgrade
    {
        public GarSaxonCrew() : base()
        {
            UpgradeInfo = new(
                name: "Gar Saxon",
                type: UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                abilityType: typeof(GarSaxonCrewAbility),
                restriction: new FactionRestriction(Faction.Imperial),
                legalityInfo: new() { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class GarSaxonCrewXWA : GarSaxonCrew
    {
        public GarSaxonCrewXWA() : base()
        {
            UpgradeInfo.Restrictions.AddRestriction(new BaseSizeRestriction(Ship.BaseSize.Large, BaseSize.Huge));
            UpgradeInfo.LegalityInfo = new() { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    /* While a friendly unit at range 1-3 with an initiative of 4 or lower performs an attack against a defender you have locked, the attacker may change 1 eye result to a hit result.*/

    public class GarSaxonCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                name: HostUpgrade.UpgradeInfo.Name,
                isAvailable: IsAvailable,
                count: 1,
                aiPriority: AiPriority,
                modificationType: DiceModificationType.Change,
                sidesCanBeSelected: new() { DieSide.Focus },
                sideCanBeChangedTo: DieSide.Success,
                isGlobal: true
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            bool result = Tools.IsSameTeam(Combat.Attacker, HostShip);

            result = result && HostShip.GetRangeToShip(Combat.Attacker) >= 1 && HostShip.GetRangeToShip(Combat.Attacker) <= 3;

            result = result && HostShip.State.Initiative <= 4;

            result = result && HostShip.Tokens.GetTokens<BlueTargetLockToken>('*').Any(t => t.OtherTargetLockTokenOwner == Combat.Defender);

            result = result && Combat.DiceRollAttack.Focuses > 0;

            return result;
        }

        private int AiPriority()
        {
            return 100;
        }
    }
}