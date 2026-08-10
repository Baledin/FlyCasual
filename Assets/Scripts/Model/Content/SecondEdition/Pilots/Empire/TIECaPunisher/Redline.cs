using ActionsList;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIECaPunisher
    {
        public class Redline : TIECaPunisher
        {
            public Redline() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Redline\"",
                    "Adrenaline Junkie",
                    Faction.Imperial,
                    5,
                    7,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RedlineAbility),
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    seImageNumber: 139,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }

        public class RedlineXWA : Redline
        {
            public RedlineXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 14;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 20;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Device,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                    UpgradeType.Torpedo
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //You can maintain 2 locks. After you perform an action, you may acquire a lock.
    public class RedlineAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Add(HostShip);
            HostShip.TwoTargetLocksOnSameTargetsAreAllowed.Add(HostShip);
            HostShip.OnActionIsPerformed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Remove(HostShip);
            HostShip.TwoTargetLocksOnSameTargetsAreAllowed.Remove(HostShip);
            HostShip.OnActionIsPerformed -= RegisterAbility;
        }

        private void RegisterAbility(GenericAction action)
        {
            RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AcquireSecondTargetLock);
        }

        private void AcquireSecondTargetLock(object sender, EventArgs e)
        {
            HostShip.ChooseTargetToAcquireTargetLock(
                Triggers.FinishTrigger,
                "You may acquire a lock",
                HostShip
            );
        }
    }
}
