using Content;
using Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class UnhingedAstromech : GenericUpgrade
    {
        public UnhingedAstromech() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Unhinged Astromech",
                UpgradeType.Astromech,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.UnhingedAstromechAbility),
                legalityInfo: new() { Legality.XWA }
            );

            NameCanonical = "unhingedastromech-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    //Reduce the difficulty of your speed 3 basic maneuvers.
    public class UnhingedAstromechAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetManeuvers += ReduceSpeedThreeDifficulty;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetManeuvers -= ReduceSpeedThreeDifficulty;
        }

        private void ReduceSpeedThreeDifficulty(Dictionary<string, MovementComplexity> maneuvers)
        {
            //"1.L.B", MovementComplexity.Easy
            string[] basicManuevers = new string[] { "S", "B", "T" };

            Dictionary<string, MovementComplexity> speedThreeMoves = maneuvers.Where(m => m.Key.Contains("3")).ToDictionary(m => m.Key, m => m.Value);

            foreach (KeyValuePair<string, MovementComplexity> movement in speedThreeMoves)
            {
                string[] movementParts = movement.Key.Split('.');

                if (basicManuevers.Contains(movementParts[2]))
                {
                    maneuvers[movement.Key] = GenericMovement.ReduceComplexity(movement.Value);
                }
            }
        }
    }
}