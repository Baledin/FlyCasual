using ActionsList;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.NantexClassStarfighter
    {
        public class BerwerKret : NantexClassStarfighter
        {
            public BerwerKret() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Berwer Kret",
                    "Hive Guard Captain",
                    Faction.Separatists,
                    5,
                    4,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BerwerKretAbility),
                    abilityText: "After you perform an attack that hits, each friendly ship with Calculate on its action bar and a lock on the defender may perform a red Calculate action.",
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent
                    },
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );
            }
        }

        public class BerwerKretXWA : BerwerKret
        {
            public BerwerKretXWA() : base()
            {
                (PilotInfo as PilotCardInfo25).Cost = 9;
                (PilotInfo as PilotCardInfo25).LoadoutValue = 9;
                (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                };
                (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BerwerKretAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += RegisterOwnAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= RegisterOwnAbility;
        }

        private void RegisterOwnAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, TryToStartMultiSelectionSubphase);
        }

        private void TryToStartMultiSelectionSubphase(object sender, EventArgs e)
        {
            if (HostShip.Owner.Ships.Values.Any(n => CanUseBerwerKretBonus(n)))
            {
                StartMultiSelectionSubphase();
            }
            else
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": No ships that can use this ability");
                Triggers.FinishTrigger();
            }
        }

        private void StartMultiSelectionSubphase()
        {
            BerwerKretMultiSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<BerwerKretMultiSelectionSubphase>(HostShip.PilotInfo.PilotName, Triggers.FinishTrigger);

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Filter = CanUseBerwerKretBonus;
            subphase.GetAiPriority = GetAiPriority;
            subphase.MaxToSelect = int.MaxValue;
            subphase.WhenDone = PerformRedCalculateActionRecursive;

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Select what ships will perform a red Calculate action";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }

        private void PerformRedCalculateActionRecursive(Action callback)
        {
            GenericShip currentShip = Selection.MultiSelectedShips.FirstOrDefault();

            if (currentShip == null)
            {
                Selection.ChangeActiveShip(HostShip);
                callback();
            }
            else
            {
                Selection.MultiSelectedShips.Remove(currentShip);

                Selection.ChangeActiveShip(currentShip);

                currentShip.AskPerformFreeAction(
                    new CalculateAction() { Color = Actions.ActionColor.Red },
                    delegate
                    {
                        PerformRedCalculateActionRecursive(callback);
                    },
                    HostShip.PilotInfo.PilotName,
                    "You may perform red Calculate action",
                    HostShip
                );
            }
        }

        private int GetAiPriority(GenericShip ship)
        {
            // AI don't use this ability
            return 0;
        }

        private bool CanUseBerwerKretBonus(GenericShip ship)
        {
            return ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && ship.ActionBar.HasAction(typeof(CalculateAction))
                && ActionsHolder.HasTargetLockOn(ship, Combat.Defender);
        }

        private class BerwerKretMultiSelectionSubphase : MultiSelectionSubphase { }
    }
}