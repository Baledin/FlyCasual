﻿using Abilities.Parameters;
using ActionsList;
using Arcs;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class SeftinVanik : RZ2AWing
        {
            public SeftinVanik() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Seftin Vanik",
                    5,
                    37,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SeftinVanikAbility),
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent }
                );

                ModelInfo.SkinName = "Green (HoH)";
            }
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