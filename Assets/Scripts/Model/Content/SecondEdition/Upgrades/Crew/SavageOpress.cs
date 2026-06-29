using Abilities.SecondEdition;
using Ship;
using SubPhases;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SavageOpress : GenericUpgrade
    {
        public SavageOpress() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Savage Opress",
                UpgradeType.Crew,
                cost: 10,
                isLimited: true,
                addForce: 1,
                restriction: new FactionRestriction(Faction.Scum, Faction.Separatists),
                abilityType: typeof(SavageOpressAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(590, 0),
                new Vector2(200, 200)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SavageOpressAbility : GenericAbility
    {
        // After a friendly ship in your front arc at range 1-2 gains a stress or strain token, you may spend 1 force. If you do, that ship gains 1 focus token.
        GenericShip friendlyShip;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal += RegisterSavageOpressAbility;
            HostShip.OnCheckForceRecurring += SetForceRecurring;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsAssignedGlobal -= RegisterSavageOpressAbility;
            HostShip.OnCheckForceRecurring -= SetForceRecurring;
        }

        private void RegisterSavageOpressAbility(GenericShip ship, GenericToken token)
        {
            if (ship == HostShip || Tools.IsAnotherTeam(HostShip, ship)) return;

            if ((token is StressToken || token is StrainToken)
                && HostShip.State.Force > 0
                && HostShip.SectorsInfo.IsShipInSector(Selection.ActiveShip, Arcs.ArcType.Front)
                && HostShip.GetRangeToShip(Selection.ActiveShip) is >= 1 and <= 2)
            {
                friendlyShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AskGainFocusToken);
            }
        }

        private void AskGainFocusToken(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                SpendForceToGainFocusToken,
                callback: Triggers.FinishTrigger,
                descriptionLong: $"Spend 1 force to give {friendlyShip.PilotInfo.PilotName} a focus token?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void SpendForceToGainFocusToken(object sender, System.EventArgs e)
        {
            HostShip.State.SpendForce(1, GainFocusToken);
        }

        private void GainFocusToken()
        {            
            friendlyShip.Tokens.AssignToken(new FocusToken(HostShip), DecisionSubPhase.ConfirmDecision);
        }

        private void SetForceRecurring(ref bool isRecurring)
        {
            isRecurring = true;
        }
    }
}