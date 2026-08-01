using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Wolfpack : GenericUpgrade
    {
        public Wolfpack() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Wolfpack",
                types: new List<UpgradeType>() { UpgradeType.Crew, UpgradeType.Gunner },
                cost: 4,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.WolfpackAbility),
                restriction: new FactionRestriction(Faction.Republic)
            );

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(329, 0),
                new Vector2(75, 75)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WolfpackAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Tools.IsFriendly(Combat.Defender, HostShip)
                && IsRangeFrom0To3()
                && IsAttackerInArc()
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskToGainStrainToLock);
            }
        }

        private void AskToGainStrainToLock(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                GainStrainFirst,
                descriptionLong: "Do you want to gain Strain token to acquire a lock on the attacker?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void GainStrainFirst(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Combat.Defender.Tokens.AssignToken(
                typeof(Tokens.StrainToken),
                AcquireLockOnAttacker
            );
        }

        private void AcquireLockOnAttacker()
        {
            ActionsHolder.AcquireTargetLock(
                Combat.Defender,
                Combat.Attacker,
                Triggers.FinishTrigger,
                Triggers.FinishTrigger
            );
        }

        private bool IsRangeFrom0To3()
        {
            DistanceInfo distInfo = new(HostShip, Combat.Defender);
            return distInfo.Range < 4;
        }

        private bool IsAttackerInArc()
        {
            ShotInfo shotInfo = new(HostShip, Combat.Attacker, HostShip.PrimaryWeapons);
            return shotInfo.InArc;
        }
    }
}