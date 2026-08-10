using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class QuinlanVos : Eta2Actis
    {
        public QuinlanVos() : base()
        {
            PilotInfo = new PilotCardInfo25(
                pilotName: "Quinlan Vos",
                pilotTitle: "Darkness Within",
                faction: Faction.Republic,
                initiative: 4,
                cost: 11,
                loadoutValue: 14,
                force: 2,
                regensForce: 1,
                isLimited: true,
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Cannon
                },
                tags: new List<Tags>()
                {
                    Tags.Jedi,
                    Tags.LightSide,
                    Tags.DarkSide
                },
                abilityType: typeof(QuinlanVosAbility),
                legality: new List<Legality>() { Legality.XWA }
            );

            PilotNameCanonical = "quinlanvos-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // After you perform a barrel roll or boost action, you may acquire a target lock on a ship in your bullseye.

    public class QuinlanVosAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= RegisterAbility;
        }

        private void RegisterAbility(GenericAction action)
        {
            if (HasAvailableTargets() && (action is BarrelRollAction || action is BoostAction))
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, GetTargetLock);
            }
        }

        private void GetTargetLock(object sender, EventArgs e)
        {
            HostShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                HostShip,
                IsValidTargetForLock
            );
        }

        private bool HasAvailableTargets()
        {
            return Roster.AllShips.Values.Any(s => IsValidTargetForLock(s));
        }

        private bool IsValidTargetForLock(GenericShip ship)
        {
            return HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Bullseye);
        }
    }
}