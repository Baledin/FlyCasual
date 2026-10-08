using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.VCX100LightFreighter
{
    public class HeraSyndullaLAR : VCX100LightFreighter
    {
        public HeraSyndullaLAR() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Hera Syndulla",
                pilotTitle: "New Republic General",
                faction: Faction.Rebel,
                initiative: 6,
                cost: 18,
                loadoutValue: 20,
                isLimited: true,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Crew,
                    UpgradeType.Sensor,
                    UpgradeType.Gunner,
                    UpgradeType.Modification,
                    UpgradeType.Turret,
                    UpgradeType.Torpedo,
                    UpgradeType.Title
                },
                tags: new List<Tags>()
                {
                    Tags.Freighter,
                    Tags.Spectre
                },
                abilityType: typeof(HeraSyndullaLARAbility),
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "herasyndulla-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HeraSyndullaLARAbility : GenericAbility
    {
        // After you fully execute a red maneuver, you may coordinate a friendly ship at range 1-3.

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            if (!HostShip.Owner.Ships.Values.Any(s => HostShip.GetRangeToShip(s) >= 1 && HostShip.GetRangeToShip(s) <= 3))
            {
                Triggers.FinishTrigger();
                return;
            }

            HeraSyndullaLegendsAndRelicsCoordinateAction action = new()
            {
                IsRealAction = false
            };

            HostShip.OnCheckCoordinateModeModification += SetCustomCoordinateMode;

            HostShip.AskPerformFreeAction(
                action,
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                descriptionLong: $"You may coordinate a friendly ship at range 1-3."
            );
        }


        private void SetCustomCoordinateMode(ref CoordinateActionData coordinateActionData)
        {
            coordinateActionData.MaxRange = 3;
            HostShip.OnCheckCoordinateModeModification -= SetCustomCoordinateMode;
        }


        private class HeraSyndullaLegendsAndRelicsCoordinateAction : CoordinateAction { }
    }
}