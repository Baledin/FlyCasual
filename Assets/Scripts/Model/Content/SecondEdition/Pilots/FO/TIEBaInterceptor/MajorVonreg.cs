using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.TIEBaInterceptor
{
    public class MajorVonreg : TIEBaInterceptor
    {
        public MajorVonreg() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Major Vonreg",
                "Red Baron",
                Faction.FirstOrder,
                6,
                5,
                14,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.MajorVonregAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Tech,
                    UpgradeType.Missile,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }

            );
        }
    }

    public class MajorVonregXWA : MajorVonreg
    {
        public MajorVonregXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>()
            {
                UpgradeType.Talent,
                UpgradeType.Talent,
                UpgradeType.Modification,
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
    public class MajorVonregAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckForAbility;
            HostShip.OnSystemsAbilityActivation += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckForAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterTrigger;
        }

        private void CheckForAbility(GenericShip ship, ref bool isAbilityActive)
        {
            isAbilityActive = Roster.AllShips.Values.Any(n => FilterTargets(n));
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, StartSelectShip);
        }

        private void StartSelectShip(object sender, EventArgs e)
        {
            if (Roster.AllShips.Values.Any(s => FilterTargets(s)))
            {
                SelectTargetForAbility(
                    SelectShip,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "You may choose a ship in your bullseye arc to assign Strain or Deplete token to it",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SelectShip()
        {
            SelectDebuffDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<SelectDebuffDecisionSubphase>(
                "Select debuff subphase",
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "You may assign Strain or Deplete token to target";
            subphase.ImageSource = HostShip;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = false;

            subphase.AddDecision(
                "Assign Strain token",
                SelectStrainToken
            );

            subphase.AddDecision(
                "Assign Deplete token",
                SelectDepleteToken
            );

            subphase.DefaultDecisionName = "Assign Strain token";

            subphase.Start();
        }

        private void SelectDepleteToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            TargetShip.Tokens.AssignToken(
                typeof(DepleteToken),
                Triggers.FinishTrigger
            );
        }

        private void SelectStrainToken(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            TargetShip.Tokens.AssignToken(
                typeof(StrainToken),
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            return Tools.IsAnotherTeam(HostShip, ship)
                && HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Bullseye);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private class SelectDebuffDecisionSubphase : DecisionSubPhase { };
    }
}