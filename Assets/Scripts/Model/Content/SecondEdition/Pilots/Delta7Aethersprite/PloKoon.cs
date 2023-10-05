﻿using Abilities.Parameters;
using Tokens;
using Upgrade;
using Content;
using System.Collections.Generic;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class PloKoon : Delta7Aethersprite
    {
        public PloKoon()
        {
            PilotInfo = new PilotCardInfo(
                "Plo Koon",
                5,
                45,
                true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.PloKoonAbility),
                tags: new List<Tags>
                {
                    Tags.LightSide,
                    Tags.Jedi
                },
                extraUpgradeIcon: UpgradeType.ForcePower
            );

            ModelInfo.SkinName = "Plo Koon";
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagement Phase, you may spend 1 force and choose another friendly ship at range 0-2. 
    //If you do, you may transfer 1 green token to it or transfer one orange token from it to you.
    public class PloKoonAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AtTheStartOfPhase(typeof(SubPhases.CombatStartSubPhase));

        public override AbilityPart Action => new AskToUseAbilityAction
        (
            description: new AbilityDescription
            (
                name: "Plo Koon",
                description: "Do you want to spend Force to transfer tokens?",
                imageSource: HostShip
            ),
            conditions: new ConditionsBlock
            (
                new CanSpendForceCondition(),
                new OrCondition
                (
                    new AndCondition
                    (
                        new HasTokenCondition(tokenColor: TokenColors.Green),
                        new HasAnyShipAtRange
                        (
                            new ConditionsBlock
                            (
                                new RangeToHostCondition(0, 2),
                                new TeamCondition(ShipTypes.OtherFriendly)
                            )
                        )
                    ),
                    new HasAnyShipAtRange
                    (
                        new ConditionsBlock
                        (
                            new RangeToHostCondition(0, 2),
                            new HasTokenCondition(tokenColor: TokenColors.Orange),
                            new TeamCondition(ShipTypes.OtherFriendly)
                        )
                    )
                )
            ),
            onYes: new SelectShipAction
            (
                new AbilityDescription
                (
                    name: "Plo Koon",
                    description: "Choose another friendly ship to transfer token",
                    imageSource: HostShip
                ),
                new ConditionsBlock
                (
                    new CanSpendForceCondition(),
                    new AndCondition
                    (
                        new RangeToHostCondition(0, 2),
                        new TeamCondition(ShipTypes.OtherFriendly),
                        new OrCondition
                        (
                            new HasTokenCondition(tokenColor: TokenColors.Orange),
                            new HasTokenCondition(tokenColor: TokenColors.Green, shipRoleToCheck: ShipRole.HostShip)
                        )
                    )
                ),
                new ExchangeToken
                (
                    getByColor: TokenColors.Orange,
                    giveByColor: TokenColors.Green,
                    showMessage: ShowMessage,
                    doNext: new SpendForceAction()
                ),
                aiSelectShipPlan: new AiSelectShipPlan(AiSelectShipTeamPriority.Friendly, AiSelectShipSpecial.None)
            )
        );

        private string ShowMessage()
        {
            return "Message";
        }
    }
}
