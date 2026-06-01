using ActionsList;
using BoardTools;
using Content;
using Movement;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PrimedOverdriveThruster : GenericUpgrade
    {
        public PrimedOverdriveThruster() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Primed Overdrive Thruster",
                UpgradeType.Tech,
                cost: 0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.PrimedOverdriveThrusterAbility),
                legalityInfo: new() { Legality.XWA }
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/poedameron-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PrimedOverdriveThrusterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates += UpdateBoostTemplate;
            HostShip.OnGetAvailableBarrelRollTemplates += UpdateBarrelRollTemplate;
            HostShip.OnGetAvailableSlamTemplates += UpdateSlamTemplate;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBoostTemplates -= UpdateBoostTemplate;
            HostShip.OnGetAvailableBarrelRollTemplates -= UpdateBarrelRollTemplate;
            HostShip.OnGetAvailableSlamTemplates -= UpdateSlamTemplate;
        }

        private void UpdateBoostTemplate(List<BoostMove> availableTemplates, GenericAction action)
        {
            if (action.IsRed)
            {
                List<BoostMove> newTemplates = new();

                foreach (BoostMove template in availableTemplates)
                {
                    BoostMove newMove = new(
                        BoostMove.GetBoostTemplateFromName(template.Name.Replace('1', '2')),
                        template.IsRed,
                        template.IsPurple,
                        template.IsForced);

                    newTemplates.Add(newMove);
                }

                availableTemplates.Clear();
                availableTemplates.AddRange(newTemplates);

                AddAskSwapStressForStrainTriggers();
            }
        }

        private void UpdateBarrelRollTemplate(List<ManeuverTemplate> availableTemplates, GenericAction action)
        {
            if (action.IsRed)
            {
                foreach (ManeuverTemplate template in availableTemplates)
                {
                    template.TryIncreaseSpeed();
                }

                AddAskSwapStressForStrainTriggers();
            }
        }

        private void UpdateSlamTemplate(ref List<ManeuverHolder> availableTemplates, GenericAction action)
        {
            if (action.IsRed)
            {
                for (int i = 0; i < availableTemplates.Count; i++)
                {
                    bool result = false;
                    switch (availableTemplates[i].Speed)
                    {
                        case ManeuverSpeed.Speed1:
                        case ManeuverSpeed.Speed2:
                            result = true;
                            break;
                        case ManeuverSpeed.Speed3:
                        case ManeuverSpeed.Speed4:
                            result = availableTemplates[i].Direction == ManeuverDirection.Forward;
                            break;
                        case ManeuverSpeed.Speed5:
                            break;
                    }
                    if (result)
                    {
                        ManeuverHolder temp = availableTemplates[i];
                        temp.Speed++;
                        availableTemplates[i] = temp;
                    }
                }

                AddAskSwapStressForStrainTriggers();
            }
        }

        private void AddAskSwapStressForStrainTriggers()
        {
            HostShip.OnActionIsPerformed += RegisterTrigger1;
            HostShip.OnActionIsReallyFailed += RegisterTrigger2;
        }

        private void RegisterTrigger1(GenericAction action)
        {
            HostShip.OnActionIsPerformed -= RegisterTrigger1;
            HostShip.OnActionIsReallyFailed -= RegisterTrigger2;
            RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed,AskToSwapStressForStrain);
        }

        private void RegisterTrigger2(GenericAction action)
        {
            HostShip.OnActionIsPerformed -= RegisterTrigger1;
            HostShip.OnActionIsReallyFailed -= RegisterTrigger2;
            RegisterAbilityTrigger(TriggerTypes.OnActionIsReallyFailed,AskToSwapStressForStrain);
        }

        private void AskToSwapStressForStrain(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                SwapStressForStrain,
                descriptionLong: "Do you want to gain 1 Strain Token to remove 1 Stress Token?",
                imageHolder: HostUpgrade
            );
        }

        private void SwapStressForStrain(object sender, EventArgs e)
        {
            HostShip.Tokens.RemoveToken(typeof(StressToken), delegate
            {
                HostShip.Tokens.AssignToken(new StrainToken(HostShip),DecisionSubPhase.ConfirmDecision);
            });
        }
    }
}