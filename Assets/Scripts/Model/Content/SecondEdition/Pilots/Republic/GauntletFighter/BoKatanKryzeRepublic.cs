using ActionsList;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.GauntletFighter
    {
        public class BoKatanKryzeRepublic : GauntletFighter
        {
            public BoKatanKryzeRepublic() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Bo-Katan Kryze",
                    "Nite Owl Commander",
                    Faction.Republic,
                    4,
                    6,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BoKatanKryzeRepublicAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification,
                        UpgradeType.Device,
                        UpgradeType.Torpedo,
                        UpgradeType.Configuration,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian
                    },
                    skinName: "Dark Blue",
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class BoKatanKryzeRepublicXWA : BoKatanKryzeRepublic
        {
            public BoKatanKryzeRepublicXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 16;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 22;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Configuration,
                    UpgradeType.Title
                };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BoKatanKryzeRepublicAbility : GenericAbility
    {
        private GenericShip LockedShip;
        private GenericShip PreviousCurrentShip;

        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterAbility;
        }
        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= RegisterAbility;
        }

        // SELECT A LOCKED TARGET

        private void RegisterAbility(GenericShip ship)
        {
            if (Board.GetShipsInArcAtRange(HostShip, Arcs.ArcType.Front, new Vector2(1, 2), Team.Type.Enemy).Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToChooseLockedTarget);
            }
        }

        private void AskToChooseLockedTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AskToSelectAnotherFriendlyShip,
                FilterTargets,
                GetLockedTargetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "You may gain a deplete token to choose an object in your front arc at range 1-2...",
                imageSource: HostShip
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return Board.GetShipsInArcAtRange(HostShip, Arcs.ArcType.Front, new Vector2(1, 2), Team.Type.Enemy).Contains(ship);
        }

        private int GetLockedTargetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        // SELECT ANOTHER FRIENDLY SHIP

        private void AskToSelectAnotherFriendlyShip()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            LockedShip = TargetShip;

            SelectTargetForAbility(
                FriendlyShipIsSelected,
                FilterFriendlyTargets,
                GetFriednlyShipAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "Choose another friendly ship it may acquire a lock on that object",
                imageSource: HostShip
            );
        }

        private bool FilterFriendlyTargets(GenericShip ship)
        {
            return Tools.CheckShipsTeam(HostShip, ship, TargetTypes.OtherFriendly);
        }

        private int GetFriednlyShipAiPriority(GenericShip ship)
        {
            int priority = ship.PilotInfo.Cost;

            DistanceInfo distInfo = new DistanceInfo(ship, LockedShip);
            if (distInfo.Range < 4) priority += 100;

            ShotInfo shotInfo = new ShotInfo(ship, LockedShip, ship.PrimaryWeapons);
            if (shotInfo.IsShotAvailable) priority += 50;

            if (!ship.Tokens.HasToken<BlueTargetLockToken>('*')) priority += 100;

            if (!ship.ActionBar.HasAction(typeof(TargetLockAction))) priority = 0;

            return priority;
        }

        // ABILITY IS RESOLVED

        private void FriendlyShipIsSelected()
        {
            HostShip.Tokens.AssignToken(typeof(DepleteToken), SelectShipSubPhase.FinishSelectionNoCallback);

            RulesList.TargetLocksRule.OnCheckTargetLockIsDisallowed += CanPerformTargetLock;

            Selection.ChangeActiveShip(TargetShip);
            TargetShip.AskPerformFreeAction(
                new TargetLockAction(),
                FinishAbility,
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "You may perform a Lock action",
                imageHolder: HostShip
            );
        }

        private void CanPerformTargetLock(ref bool result, GenericShip ship, ITargetLockable defender)
        {

            if (ship != TargetShip) return;

            if (defender is GenericShip)
            {
                result = (defender as GenericShip) == LockedShip;
            }
        }

        private void FinishAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsDisallowed -= CanPerformTargetLock;
            Selection.ChangeActiveShip(HostShip);
            Triggers.FinishTrigger();
        }
    }
}