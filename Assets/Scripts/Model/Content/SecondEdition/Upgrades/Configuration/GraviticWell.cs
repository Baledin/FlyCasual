
using Abilities.SecondEdition;
using Content;
using Ship;
using Ship.SecondEdition.NantexClassStarfighter;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class GraviticWell : GenericUpgrade
    {
        public GraviticWell() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                name: "Gravitic Well",
                type: UpgradeType.Configuration,
                cost: 5,
                isStandardized: true,
                abilityType: typeof(GraviticWellAbility),
                restriction: new ShipRestriction(typeof(NantexClassStarfighter)),
                legalityInfo: new List<Legality>() { Legality.XWA }
            );

            NameCanonical = "graviticwell-legendsandrelics";
        }
    }
}

namespace Abilities.SecondEdition
{
    // At the start of the Engagement Phase, if you are tractored, each other small ship at range 0-1 gains 1 strain token.
    // Replace any instance of 'if the defender is tractored' in your pilot ability with 'if you are tractored'
    public class GraviticWellAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
            HostShip.OnSetupPlaced += ReplacePilotAbilities;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
            HostShip.OnSetupPlaced -= ReplacePilotAbilities;
        }

        private void CheckAbility()
        {
            if (HostShip.IsTractored)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, UseAbility);
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            List<GenericShip> nearbyShips = new();

            foreach (GenericShip ship in Roster.AllShips.Values.Except(new List<GenericShip> { HostShip }).ToList())
            {
                if (HostShip.GetRangeToShip(ship) < 2)
                {
                    ship.Tokens.AssignToken(typeof(StrainToken), delegate { });
                }
            }

            Triggers.FinishTrigger();
        }

        private void ReplacePilotAbilities(GenericShip ship)
        {
            List<GenericAbility> pilotAbilities = ship.PilotAbilities.ToList();

            foreach (GenericAbility ability in pilotAbilities)
            {
                switch (ability)
                {
                    case SunFacAbility:
                        ReplaceAbility(ability, typeof(SunFacGraviticWellAbility));
                        break;

                    case ChertekAbility:
                        ReplaceAbility(ability, typeof(ChertekGraviticWellAbility));
                        break;
                }
            }
        }

        private void ReplaceAbility(GenericAbility ability, Type newAbilityType)
        {
            ability.DeactivateAbility();
            HostShip.PilotAbilities.Remove(ability);
            GenericAbility newAbility = (GenericAbility)Activator.CreateInstance(newAbilityType);

            newAbility.Initialize(HostShip);
            HostShip.PilotAbilities.Add(newAbility);
        }
    }

    public class SunFacGraviticWellAbility : SunFacAbility
    {
        protected override void CheckAbility(ref int count)
        {
            if (HostShip.IsTractored)
            {
                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} is tractored and may roll an additional attack die");
                count++;
            }
        }
    }

    public class ChertekGraviticWellAbility : ChertekAbility
    {
        protected override bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                && HostShip.IsTractored;
        }
    }
}