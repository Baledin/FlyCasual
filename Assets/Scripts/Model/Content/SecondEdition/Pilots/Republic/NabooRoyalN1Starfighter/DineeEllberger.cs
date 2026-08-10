using Abilities.SecondEdition;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class DineeEllberger : NabooRoyalN1Starfighter
        {
            public DineeEllberger() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Dineé Ellberger",
                    "Bravo Five",
                    Faction.Republic,
                    3,
                    4,
                    14,
                    isLimited: true,
                    abilityText: "While you defend or perform an attack, if the speed of your revealed maneuver is the same as the enemy ship's, that ship's dice cannot be modified.",
                    abilityType: typeof(DineeEllbergerAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Astromech,
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class DineeEllbergerXWA : DineeEllberger
        {
            public DineeEllbergerXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 16;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Torpedo,
                };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DineeEllbergerAbility : GenericAbility
    {
        GenericShip EnemyShip;
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker += CheckDineeEllbergerAbility;
            HostShip.OnAttackStartAsDefender += CheckDineeEllbergerAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= CheckDineeEllbergerAbility;
            HostShip.OnAttackStartAsDefender -= CheckDineeEllbergerAbility;
        }

        private void CheckDineeEllbergerAbility()
        {
            if (Combat.Defender.ShipId == HostShip.ShipId)
            {
                EnemyShip = Combat.Attacker;
            }
            else
            {
                EnemyShip = Combat.Defender;
            }
            if (HostShip.RevealedManeuver == null || EnemyShip.RevealedManeuver == null) return;

            if (HostShip.RevealedManeuver.Speed == EnemyShip.RevealedManeuver.Speed)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Enemy's dice cannot be modified.");
                EnemyShip.OnTryAddAvailableDiceModification += UseDiceRestriction;
                HostShip.OnTryAddDiceModificationOpposite += UseDiceRestriction;
                EnemyShip.OnAttackFinish += RemoveOmegaLeaderPilotAbility;
            }
        }

        private void UseDiceRestriction(GenericShip ship, ActionsList.GenericAction diceModification, ref bool canBeUsed)
        {
            if (!diceModification.IsNotRealDiceModification)
            {
                Messages.ShowErrorToHuman(HostShip.PilotInfo.PilotName + ": Enemy's dice cannot be modified.");
                canBeUsed = false;
            }
        }

        private void RemoveOmegaLeaderPilotAbility(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= UseDiceRestriction;
            HostShip.OnTryAddDiceModificationOpposite -= UseDiceRestriction;
            ship.OnAttackFinish -= RemoveOmegaLeaderPilotAbility;
        }
    }
}
