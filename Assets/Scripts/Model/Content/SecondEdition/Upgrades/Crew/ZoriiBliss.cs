using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using MainPhases;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ZoriiBliss : GenericUpgrade
    {
        public ZoriiBliss() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Zorii Bliss",
                UpgradeType.Crew,
                cost: 5,
                charges: 1,
                regensChargesCount: 1,
                abilityType: typeof(ZoriiBlissCrewAbility),
                addAction: new ActionInfo(typeof(JamAction), ActionColor.White),
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Resistance),
                    new ActionBarRestriction(typeof(JamAction))),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "zoriibliss-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZoriiBlissCrewAbility : GenericAbility
    {
        GenericToken savedToken;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsRemovedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsRemovedGlobal -= CheckAbility;
        }

        public void CheckAbility(GenericShip ship, GenericToken token)
        {
            if (Phases.CurrentPhase is EndPhase) return;

            int rangeToShip = HostShip.GetRangeToShip(ship);

            if (Tools.IsAnotherTeam(HostShip, ship) && ship.IsJammed && HostUpgrade.State.Charges > 0 && token.TokenColor == TokenColors.Green && rangeToShip is >= 1 and <= 2)
            {
                savedToken = token;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsRemoved, AskGainDuplicateToken);
            }
        }

        public void AskGainDuplicateToken(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                GainDuplicateToken,
                callback: Triggers.FinishTrigger,
                descriptionLong: $"Spend 1 charge to gain a {savedToken.Name}?"
            );
        }

        public void GainDuplicateToken(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(savedToken.GetType(), DecisionSubPhase.ConfirmDecision);

            HostUpgrade.State.SpendCharge();
        }
    }
}