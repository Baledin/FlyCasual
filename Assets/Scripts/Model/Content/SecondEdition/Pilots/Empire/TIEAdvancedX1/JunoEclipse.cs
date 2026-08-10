using ActionsList;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class JunoEclipse : TIEAdvancedX1
        {
            public JunoEclipse() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Juno Eclipse",
                    "Corulag's Finest",
                    Faction.Imperial,
                    5,
                    4,
                    8,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JunoEclipseAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Sensor,
                        UpgradeType.Missile,
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

        public class JunoEclipseXWA : JunoEclipse
        {
            public JunoEclipseXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 11;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 13;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Modification,
                    UpgradeType.Missile
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JunoEclipseAbility : GenericAbility
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
            RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToPerformRedBoost);
        }

        private void AskToPerformRedBoost(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new BoostAction() { HostShip = TargetShip, Color = Actions.ActionColor.Red },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "You may perform a red Boost action",
                HostShip
            );
        }
    }
}