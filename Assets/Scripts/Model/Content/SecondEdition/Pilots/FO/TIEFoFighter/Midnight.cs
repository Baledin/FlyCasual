using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEFoFighter
{
    public class Midnight : TIEFoFighter
    {
        public Midnight() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Midnight\"",
                "Omega Leader",
                Faction.FirstOrder,
                6,
                3,
                7,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.MidnightAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class MidnightXWA : Midnight
    {
        public MidnightXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 9;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Missile
            };
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MidnightAbility : GenericAbility
    {
        GenericShip LockedShip;

        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker += AddOmegaLeaderPilotAbility;
            HostShip.OnAttackStartAsDefender += AddOmegaLeaderPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= AddOmegaLeaderPilotAbility;
            HostShip.OnAttackStartAsDefender -= AddOmegaLeaderPilotAbility;

            if (LockedShip != null) RemoveOmegaLeaderPilotAbility(LockedShip);
        }

        private void AddOmegaLeaderPilotAbility()
        {
            if (Combat.Defender.ShipId == HostShip.ShipId)
            {
                LockedShip = Combat.Attacker;
            }
            else
            {
                LockedShip = Combat.Defender;
            }

            if (ActionsHolder.HasTargetLockOn(HostShip, LockedShip))
            {
                LockedShip.OnTryAddAvailableDiceModification += UseOmegaLeaderRestriction;
                LockedShip.OnTryAddDiceModificationOpposite += UseOmegaLeaderRestriction;
                LockedShip.OnAttackFinish += RemoveOmegaLeaderPilotAbility;
            }
        }

        private void UseOmegaLeaderRestriction(GenericShip ship, ActionsList.GenericAction diceModification, ref bool canBeUsed)
        {
            if (!diceModification.IsNotRealDiceModification)
            {
                Messages.ShowErrorToHuman(HostShip.PilotInfo.PilotName + ": The target is unable to modify dice");
                canBeUsed = false;
            }
        }

        private void RemoveOmegaLeaderPilotAbility(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= UseOmegaLeaderRestriction;
            ship.OnTryAddDiceModificationOpposite -= UseOmegaLeaderRestriction;
            ship.OnAttackFinish -= RemoveOmegaLeaderPilotAbility;
        }
    }
}