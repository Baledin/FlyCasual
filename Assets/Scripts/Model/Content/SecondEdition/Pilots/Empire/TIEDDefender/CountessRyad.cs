using Content;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class CountessRyad : TIEDDefender
        {
            public CountessRyad() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Countess Ryad",
                    "Cutthroat Politico",
                    Faction.Imperial,
                    4,
                    7,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CountessRyadAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Sensor,
                        UpgradeType.Cannon,
                        UpgradeType.Missile,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 124,
                    skinName: "Crimson",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class CountessRyadXWA : CountessRyad
        {
            public CountessRyadXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 16;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 11;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Cannon,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you would execute a straight maneuver, you may increase difficulty of the maeuver. If you do, execute it as a koiogran turn maneuver instead.
    public class CountessRyadAbility : Abilities.FirstEdition.CountessRyadAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeMovementIsExecuted += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeMovementIsExecuted -= RegisterAskChangeManeuver;
        }

        protected override void RegisterAskChangeManeuver(GenericShip ship)
        {
            //I have assumed that you can not use this ability if you execute a red maneuver
            if (HostShip.AssignedManeuver.ColorComplexity != MovementComplexity.Complex
                && HostShip.AssignedManeuver.Bearing == ManeuverBearing.Straight
                && !HostShip.AssignedManeuver.IsIonManeuver)
            {
                RegisterAbilityTrigger(TriggerTypes.BeforeMovementIsExecuted, AskChangeManeuver);
            }
        }

        protected override MovementComplexity GetNewManeuverComplexity()
        {
            if (HostShip.AssignedManeuver.ColorComplexity == MovementComplexity.Complex)
                throw new Exception("Can't increase difficulty of red maneuvers");

            return HostShip.AssignedManeuver.ColorComplexity + 1;
        }
    }
}

namespace Abilities.FirstEdition
{
    public class CountessRyadAbility : GenericAbility
    {
        string maneuverKey;
        MovementComplexity originalColor;

        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterAskChangeManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterAskChangeManeuver;
        }

        protected virtual void RegisterAskChangeManeuver(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.Bearing == ManeuverBearing.Straight)
            {
                RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, AskChangeManeuver);
            }
        }

        protected virtual MovementComplexity GetNewManeuverComplexity()
        {
            return HostShip.AssignedManeuver.ColorComplexity;
        }

        protected void AskChangeManeuver(object sender, System.EventArgs e)
        {
            Messages.ShowInfoToHuman("Countess Ryad: You can change your maneuver to Koiogran turn");
            maneuverKey = HostShip.AssignedManeuver.Speed + ".F.R";
            originalColor = (HostShip.Maneuvers.ContainsKey(maneuverKey)) ? HostShip.Maneuvers[maneuverKey] : MovementComplexity.None;
            HostShip.Maneuvers[maneuverKey] = GetNewManeuverComplexity();

            HostShip.Owner.ChangeManeuver(
                (maneuverCode) =>
                {
                    ShipMovementScript.SendAssignManeuverCommand(maneuverCode);
                    HostShip.OnMovementFinish += RestoreManuvers;
                },
                Triggers.FinishTrigger,
                StraightOrKoiogran
            );
        }

        private void RestoreManuvers(GenericShip ship)
        {
            HostShip.OnMovementFinish -= RestoreManuvers;

            if (originalColor != MovementComplexity.None)
            {
                HostShip.Maneuvers[maneuverKey] = originalColor;
            }
            else
            {
                HostShip.Maneuvers.Remove(maneuverKey);
            }
        }

        private bool StraightOrKoiogran(string maneuverString)
        {
            bool result = false;
            ManeuverHolder movementStruct = new ManeuverHolder(maneuverString);
            if (movementStruct.Speed == Selection.ThisShip.AssignedManeuver.ManeuverSpeed &&
                (movementStruct.Bearing == ManeuverBearing.Straight ||
                movementStruct.Bearing == ManeuverBearing.KoiogranTurn))
            {
                result = true;
            }
            return result;
        }
    }
}