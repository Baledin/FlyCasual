using Content;
using Movement;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.VCX100LightFreighter
{
    public class HeraSyndulla : VCX100LightFreighter
    {
        public HeraSyndulla() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Hera Syndulla",
                "Spectre-2",
                Faction.Rebel,
                5,
                7,
                20,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.HeraSyndullaAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
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
                tags: new List<Tags>
                {
                    Tags.Freighter,
                    Tags.Spectre
                },
                legality: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                },
                seImageNumber: 73
            );

            PilotNameCanonical = "herasyndulla-vcx100lightfreighter";
        }
    }

    public class HeraSyndullaXWA : HeraSyndulla
    {
        public HeraSyndullaXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 17;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 19;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
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
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HeraSyndullaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        private void RegisterAskChangeManeuver(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
        }

        private void AskChangeManeuver(object sender, System.EventArgs e)
        {
            if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Easy || HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
            {
                HostShip.Owner.ChangeManeuver(ShipMovementScript.SendAssignManeuverCommand, Triggers.FinishTrigger, IsSameComplexity);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool IsSameComplexity(string maneuverString)
        {
            bool result = false;
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if (movementStruct.ColorComplexity == HostShip.AssignedManeuver.ColorComplexity)
            {
                result = true;
            }
            return result;
        }
    }
}