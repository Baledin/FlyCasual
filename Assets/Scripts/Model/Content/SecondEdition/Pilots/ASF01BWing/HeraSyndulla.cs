﻿using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ASF01BWing
    {
        public class HeraSyndulla : ASF01BWing
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    6,
                    50,
                    pilotTitle: "Phoenix Leader",
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.HeraSyndullaABWingAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                PilotNameCanonical = "herasyndulla-asf01bwing";

                ModelInfo.SkinName = "Prototype";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HeraSyndullaABWingAbility : GenericAbility
    {
        private static readonly List<Type> TokenTypesTransferable = new List<Type>()
        {
            typeof(FocusToken),
            typeof(EvadeToken),
            typeof(BlueTargetLockToken)
        };

        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new HeraSyndullaDiceModification(), HostShip);
        }

        private class HeraSyndullaDiceModification : ActionsList.GenericAction
        {
            public HeraSyndullaDiceModification()
            {
                Name = DiceModificationName = "Get Hera's token";
                IsNotRealDiceModification = true;
            }

            public override bool IsDiceModificationAvailable()
            {
                return HasTokensToTransfter()
                    && Tools.IsAnotherFriendly(HostShip, DiceModificationShip)
                    && IsInRangeOfAbility();
            }

            private bool IsInRangeOfAbility()
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, DiceModificationShip);
                return (distInfo.Range >= 1 && distInfo.Range <= 2);
            }

            private bool HasTokensToTransfter()
            {
                foreach (Type tokenType in TokenTypesTransferable)
                {
                    if (HostShip.Tokens.HasToken(tokenType, '*')) return true;
                }

                return false;
            }

            public override int GetDiceModificationPriority()
            {
                return 0;
            }

            public override void ActionEffect(Action callBack)
            {
                Triggers.RegisterTrigger
                (
                    new Trigger()
                    {
                        Name = "Choose Hera's token",
                        EventHandler = AskToTransfterToken,
                        TriggerOwner = HostShip.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnAbilityDirect
                    }
                );

                Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, callBack);
            }

            private void AskToTransfterToken(object sender, EventArgs e)
            {
                HeraSyndullaDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<HeraSyndullaDecisionSubPhase>("Hera Syndulla Decision", Triggers.FinishTrigger);

                subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
                subphase.DescriptionLong = $"Choose which token do you want to transfer to {DiceModificationShip.PilotInfo.PilotName}";
                subphase.ImageSource = HostShip;

                foreach (GenericToken token in HostShip.Tokens.GetAllTokens())
                {
                    if (TokenTypesTransferable.Contains(token.GetType()))
                    {
                        string tokenName = (token is BlueTargetLockToken) ? $"Lock \"{(token as BlueTargetLockToken).Letter}\"" : token.Name;
                        subphase.AddDecision(tokenName, delegate { TransferToken(token); });
                    }
                }

                subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
                subphase.DecisionOwner = HostShip.Owner;
                subphase.ShowSkipButton = false;

                subphase.Start();
            }

            private void TransferToken(GenericToken token)
            {
                DecisionSubPhase.ConfirmDecisionNoCallback();

                ActionsHolder.ReassignToken(token, HostShip, DiceModificationShip, Triggers.FinishTrigger);
            }
        }

        private class HeraSyndullaDecisionSubPhase : DecisionSubPhase { };
    }
}