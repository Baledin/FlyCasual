using Actions;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class LieutenantHebslyBoE : TIELnFighter
        {
            public LieutenantHebslyBoE() : base()
            {
                PilotInfo = new PilotCardInfo25(
                    "Lieutenant Hebsly",
                    "Battle Over Endor",
                    Faction.Imperial,
                    3,
                    3,
                    loadoutValue: 0,
                    isStandardLayout: true,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    abilityType: typeof(Abilities.SecondEdition.LieutenantHebsly),
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                ShipAbilities.Add(new Abilities.SecondEdition.FormedUpAbility());
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BoostAction), ActionColor.Red));
                ShipInfo.Hull++;

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Collected));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Elusive));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.NoEscape));

                PilotNameCanonical = "lieutenanthebsly-battleoverendor";
            }
        }

        public class LieutenantHebslyBoEXWA : LieutenantHebslyBoE
        {
            public LieutenantHebslyBoEXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 8;
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you defend, you may perform a red boost action, even while stressed.
    public class LieutenantHebsly : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += CheckAttackFinishCondition;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= CheckAttackFinishCondition;
        }

        private void CheckAttackFinishCondition(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskUseAbility);
        }
        private void AskUseAbility(object sender, EventArgs e)
        {
            Selection.ThisShip = HostShip;
            Selection.ChangeActiveShip(HostShip);
            CameraScript.RestoreCamera();
            HostShip.AskPerformFreeAction(
                new BoostAction() { CanBePerformedWhileStressed = true, Color = ActionColor.Red },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "After you defend, you may perform a red boost action, even while stressed.",
                HostShip
            );
        }
    }
}