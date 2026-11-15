using Abilities.SecondEdition;
using Actions;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.ASF01BWing
{
    public class BraylenStrammBattleOverEndor : ASF01BWing
    {
        public BraylenStrammBattleOverEndor() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                pilotName: "Braylen Stramm",
                pilotTitle: "Battle Over Endor",
                faction: Faction.Rebel,
                initiative: 4,
                cost: 5,
                loadoutValue: 0,
                isLimited: true,
                abilityType: typeof(BraylenStrammBattleOverEndorAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Device,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.BWing
                },
                skinName: "Braylen Stramm",
                charges: 2,
                regensCharges: 1,
                isStandardLayout: true,
                legality: new List<Legality>() { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipInfo.Shields++;

            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ItsATrap));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.HomingMissiles));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ProtonBombs));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.DelayedFuses));

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReloadAction), ActionColor.Red));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(TargetLockAction)));

            ShipAbilities.Add(new GyroCockpit());

            PilotNameCanonical = "braylenstramm-battleoverendor";

            DefaultUpgrades.Remove(typeof(UpgradesList.SecondEdition.StabilizedSFoilsOpen));

        }
    }

    public class BraylenStrammBoEXWA : BraylenStrammBattleOverEndor
    {
        public BraylenStrammBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 13;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BraylenStrammBattleOverEndorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            List<GenericShip> friendlyShipsAtRange = Board.GetShipsAtRange(HostShip, new Vector2(0, 2), Team.Type.Friendly);

            foreach (GenericShip ship in friendlyShipsAtRange)
            {
                if (ship.PilotInfo.PilotName.Equals("Gina Moonsong") && ship.IsStressed)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, CheckUseAbility);
                    break;
                }
            }
        }

        private void CheckUseAbility(object sender, EventArgs e)
        {
            AskToUseAbility
            (
                descriptionShort: HostShip.PilotName,
                descriptionLong: "Gina Moonsong is stressed, do you want to gain a focus token?",
                useByDefault: AlwaysUseByDefault,
                useAbility: GainFocusToken,
                callback: Triggers.FinishTrigger,
                showAlwaysUseOption: true,
                imageHolder: HostShip,
                showSkipButton: false
            );
        }

        private void GainFocusToken(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(new Tokens.FocusToken(HostShip), DecisionSubPhase.ConfirmDecision);
        }
    }
}