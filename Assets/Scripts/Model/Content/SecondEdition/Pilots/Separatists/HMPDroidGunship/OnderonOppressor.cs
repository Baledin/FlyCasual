using ActionsList;
using Content;
using Movement;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class OnderonOppressor : HMPDroidGunship
        {
            public OnderonOppressor() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Onderon Oppressor",
                    "Atmospheric Attack Module",
                    Faction.Separatists,
                    3,
                    4,
                    15,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.OnderonOppressorAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Crew,
                        UpgradeType.Device,
                        UpgradeType.Modification,
                        UpgradeType.Configuration
                    },
                    tags: new List<Tags>
                    {
                        Tags.Droid
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class OnderonOppressorXWA : OnderonOppressor
        {
            public OnderonOppressorXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 9;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                    UpgradeType.Configuration
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class OnderonOppressorAbility : CombinedAbility
    {
        public override List<Type> CombinedAbilities => new List<Type>()
        {
            typeof(OnderonOppressorBarrelRollAbility),
            typeof(OnderonOppressorSideslipAbility),
        };

        private class OnderonOppressorBarrelRollAbility : TriggeredAbility
        {
            public override TriggerForAbility Trigger => new AfterYouPerformAction
            (
                actionType: typeof(BarrelRollAction),
                hasToken: typeof(StressToken)
            );

            public override AbilityPart Action => new AssignTokenAction
            (
                typeof(CalculateToken),
                targetShipRole: ShipRole.HostShip,
                showMessage: GetMessage
            );

            private string GetMessage()
            {
                return "Onderon Oppressor: Gained Calculate token";
            }
        }

        private class OnderonOppressorSideslipAbility : TriggeredAbility
        {
            public override TriggerForAbility Trigger => new AfterManeuver
            (
                onlyIfBearing: ManeuverBearing.SideslipAny,
                hasToken: typeof(StressToken)
            );

            public override AbilityPart Action => new AssignTokenAction
            (
                typeof(CalculateToken),
                targetShipRole: ShipRole.HostShip,
                showMessage: GetMessage
            );

            private string GetMessage()
            {
                return "Onderon Oppressor: Gained Calculate token";
            }
        }
    }
}