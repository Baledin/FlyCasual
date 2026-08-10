using Abilities.Parameters;
using Content;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class DGS286 : HMPDroidGunship
        {
            public DGS286() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "DGS-286",
                    "Ambush Protocols",
                    Faction.Separatists,
                    3,
                    3,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DGS286Ability),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.TacticalRelay,
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

        public class DGS286XWA : DGS286
        {
            public DGS286XWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 10;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 13;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Modification,
                    UpgradeType.Device,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                    UpgradeType.Torpedo,
                    UpgradeType.Configuration,
                    UpgradeType.TacticalRelay,
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DGS286Ability : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new BeforeYouEngage();

        public override AbilityPart Action => new SelectShipAction
        (
            abilityDescription: new AbilityDescription
            (
                name: "DGS-286",
                description: "Choose another friendly ship to transfer Calculate token to you",
                imageSource: HostShip
            ),
            conditions: new ConditionsBlock
            (
                new RangeToHostCondition(0, 1),
                new TeamCondition(ShipTypes.OtherFriendly),
                new HasTokenCondition(tokenType: typeof(CalculateToken))
            ),
            action: new TransferTokenFromTargetAction
            (
                tokenType: typeof(CalculateToken),
                showMessage: GetMessageToShow
            ),
            aiSelectShipPlan: new AiSelectShipPlan
            (
                aiSelectShipTeamPriority: AiSelectShipTeamPriority.Friendly,
                aiSelectShipSpecial: AiSelectShipSpecial.Worst
            )
        );

        private string GetMessageToShow()
        {
            return "DGS-286: Calculate Token is transfered from " + TargetShip.PilotInfo.PilotName;
        }
    }
}