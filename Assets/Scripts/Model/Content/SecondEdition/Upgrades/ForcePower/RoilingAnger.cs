using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class RoilingAnger : GenericUpgrade
    {
        public RoilingAnger() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo
            (
                "Roiling Anger",
                UpgradeType.ForcePower,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.RoilingAngerAbility),
                legalityInfo: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            NameCanonical = "roilinganger-legendsandrelics";
        }
    }

    public class RoilingAngerXwa : RoilingAnger
    {
        public RoilingAngerXwa() : base()
        {
            UpgradeInfo.Cost = 2;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
            UpgradeInfo.Restrictions.AddRestriction(new TagRestriction(Tags.DarkSide));
            IsHidden = false;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RoilingAngerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            foreach (GenericShip enemyShip in HostShip.Owner.AnotherPlayer.Ships.Values)
            {
                if (enemyShip.SectorsInfo.IsShipInSector(HostShip, Arcs.ArcType.Front))
                {
                    RegisterAbilityTrigger
                    (
                        TriggerTypes.OnCombatPhaseStart,
                        AskToRecoverCharge,
                        customTriggerName: $"{HostShip.PilotInfo.PilotName}: {HostUpgrade.UpgradeInfo.Name}"
                    );
                }
            }
        }

        private void AskToRecoverCharge(object sender, EventArgs e)
        {
            AskToUseAbility
            (
                HostUpgrade.UpgradeInfo.Name,
                ShouldAiUseAbility,
                UseRoilingAngerAbility,
                descriptionLong: "You may gain 1 strain token to recover 1 force charge",
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private bool ShouldAiUseAbility()
        {
            return HostShip.State.Charges <= HostShip.State.MaxCharges - 2;
        }

        private void UseRoilingAngerAbility(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} gains 1 strain token to recover 1 force charge");
            HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), RecoverForce);
        }

        private void RecoverForce()
        {
            HostShip.State.RestoreForce();
            Triggers.FinishTrigger();
        }
    }
}