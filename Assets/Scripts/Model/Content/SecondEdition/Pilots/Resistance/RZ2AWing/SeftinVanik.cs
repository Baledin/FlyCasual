using Abilities.Parameters;
using ActionsList;
using Content;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.RZ2AWing
{
    public class SeftinVanik : RZ2AWing
    {
        public SeftinVanik() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Seftin Vanik",
                "Skillful Wingmate",
                Faction.Resistance,
                5,
                4,
                12,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.SeftinVanikAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Tech,
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.AWing
                },
                skinName: "Green (HoH)",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class SeftinVanikXWA : SeftinVanik
    {
        public SeftinVanikXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 8;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Talent,
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
    public class SeftinVanikAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterYouPerformAction
        (
            actionType: typeof(BoostAction),
            hasToken: typeof(EvadeToken)
        );

        public override AbilityPart Action => new SelectShipAction
        (
            abilityDescription: new AbilityDescription
            (
                name: "Seftin Vanik",
                description: "You may transfer Evade token to a friendly ship",
                imageSource: HostShip
            ),
            conditions: new ConditionsBlock
            (
                new RangeToHostCondition(1, 1),
                new TeamCondition(ShipTypes.OtherFriendly)
            ),
            action: new TransferTokenToTargetAction
            (
                tokenType: typeof(EvadeToken),
                showMessage: ShowTransferSuccessMessage
            ),
            aiSelectShipPlan: new AiSelectShipPlan
            (
                aiSelectShipTeamPriority: AiSelectShipTeamPriority.Friendly
            )
        );

        private string ShowTransferSuccessMessage()
        {
            return "Seftin Vanik: Evade Token is transfered to " + TargetShip.PilotInfo.PilotName;
        }
    }
}