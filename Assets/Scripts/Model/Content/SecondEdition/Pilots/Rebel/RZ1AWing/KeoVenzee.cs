using Content;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.RZ1AWing
{
    public class KeoVenzee : RZ1AWing
    {
        public KeoVenzee() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Keo Venzee",
                "Auspicious Ace",
                Faction.Rebel,
                3,
                3,
                7,
                force: 1,
                regensForce: 0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.KeoVenzeeAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Blue",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class KeoVenzeeXWA : KeoVenzee
    {
        public KeoVenzeeXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 15;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KeoVenzeeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += CheckRevealedManeuved;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= CheckRevealedManeuved;
        }

        private void CheckRevealedManeuved(GenericShip ship)
        {
            if (ship.State.Force == 0
                && ship.RevealedManeuver != null
                && (ship.RevealedManeuver.Bearing == Movement.ManeuverBearing.Bank || ship.RevealedManeuver.Bearing == Movement.ManeuverBearing.Turn))
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, RegisterKeoVenziAbility);
            }
        }

        private void RegisterKeoVenziAbility(object sender, EventArgs e)
        {
            AskToUseAbility
            (
                HostShip.PilotInfo.PilotName,
                NeverUseByDefault,
                DoSideSlip,
                descriptionLong: "Do you want to perform sideslip instead?",
                imageHolder: HostShip
            );
        }

        private void DoSideSlip(object sender, EventArgs e)
        {
            GenericMovement movement = null;
            if (HostShip.RevealedManeuver.Bearing == ManeuverBearing.Bank)
            {
                movement = new SideslipBankMovement(
                    HostShip.RevealedManeuver.Speed,
                    HostShip.RevealedManeuver.Direction,
                    ManeuverBearing.SideslipBank,
                    GenericMovement.IncreaseComplexity(HostShip.RevealedManeuver.ColorComplexity)
                    );
            }

            if (HostShip.RevealedManeuver.Bearing == ManeuverBearing.Turn)
            {
                movement = new SideslipTurnMovement(
                    HostShip.RevealedManeuver.Speed,
                    HostShip.RevealedManeuver.Direction,
                    ManeuverBearing.SideslipTurn,
                    GenericMovement.IncreaseComplexity(HostShip.RevealedManeuver.ColorComplexity)
                    );
            }

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: Maneuver is changed to Sideslip");
            HostShip.SetAssignedManeuver(movement);

            HostShip.State.RestoreForce();

            Triggers.FinishTrigger();
        }
    }
}