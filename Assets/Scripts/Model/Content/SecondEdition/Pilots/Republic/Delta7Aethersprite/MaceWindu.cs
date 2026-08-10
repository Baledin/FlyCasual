using BoardTools;
using Content;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class MaceWindu : Delta7Aethersprite
    {
        public MaceWindu()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Mace Windu",
                "Harsh Traditionalist",
                Faction.Republic,
                4,
                4,
                7,
                isLimited: true,
                force: 3,
                abilityType: typeof(Abilities.SecondEdition.MaceWinduAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Astromech,
                    UpgradeType.Configuration,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Mace Windu",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class MaceWinduXWA : MaceWindu
    {
        public MaceWinduXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 8;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Configuration,
            };
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a red maneuver, recover 1 force.
    public class MaceWinduAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == MovementComplexity.Complex && !(Board.IsOffTheBoard(HostShip) || HostShip.IsBumped))
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AssignTokens);
            }
        }

        private void AssignTokens(object sender, EventArgs e)
        {
            HostShip.State.RestoreForce();
            Triggers.FinishTrigger();
        }
    }
}
