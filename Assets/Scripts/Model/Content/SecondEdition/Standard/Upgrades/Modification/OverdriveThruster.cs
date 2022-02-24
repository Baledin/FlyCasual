﻿using ActionsList;
using BoardTools;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class OverdriveThruster : GenericUpgrade, IVariableCost
    {
        public OverdriveThruster() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Overdrive Thruster",
                UpgradeType.Modification,
                cost: 2,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.T70XWing.T70XWing)),
                abilityType: typeof(Abilities.SecondEdition.OverdriveThrusterAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/21/3e/213e1b33-19c8-4081-9ed2-e30b0fe345dd/swz68_overdrive-thruster.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 2},
                {1, 3},
                {2, 4},
                {3, 5},
                {4, 6},
                {5, 7},
                {6, 8}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}

namespace Abilities.SecondEdition
{
    public class OverdriveThrusterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnUpdateChosenBoostTemplate += UpdateBoostTemplate;
            HostShip.OnUpdateChosenBarrelRollTemplate += UpdateBarrelRollTemplate;
            HostShip.OnUpdateChosenSlamTemplate += UpdateSlamTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnUpdateChosenBoostTemplate -= UpdateBoostTemplate;
            HostShip.OnUpdateChosenBarrelRollTemplate -= UpdateBarrelRollTemplate;
            HostShip.OnUpdateChosenSlamTemplate -= UpdateSlamTemplate;
        }

        private void UpdateBoostTemplate(ref string name)
        {
            if (ActionsHolder.CurrentAction.IsRed)
            {
                IncreaseSpeedOfTemplateByName(ref name);
            }
        }

        private void IncreaseSpeedOfTemplateByName(ref string name)
        {
            bool isChanged = false;

            if (name.Contains("1"))
            {
                name = name.Replace('1', '2');
                isChanged = true;
            }
            
            if (isChanged)
            {
                Messages.ShowInfo("Overdrive Thursters: Template of 1 speed higher is used");
            }
        }

        private void UpdateBarrelRollTemplate(ref ManeuverTemplate maneuverTemplate)
        {
            if (ActionsHolder.CurrentAction.IsRed)
            {
                if (maneuverTemplate.TryIncreaseSpeed())
                {
                    Messages.ShowInfo("Overdrive Thursters: Template of 1 speed higher is used");
                }
            }
        }

        private void UpdateSlamTemplate(GenericMovement movement)
        {
            if (ActionsHolder.CurrentAction.IsRed)
            {
                if (movement.TryIncreaseSpeed())
                {
                    Messages.ShowInfo("Overdrive Thursters: Template of 1 speed higher is used");
                }
            }
        }
    }
}