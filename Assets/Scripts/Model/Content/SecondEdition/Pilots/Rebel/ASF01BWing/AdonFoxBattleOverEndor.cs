using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.ASF01BWing
{
    public class AdonFoxBattleOverEndor : ASF01BWing
    {
        public AdonFoxBattleOverEndor() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                pilotName: "Adon Fox",
                pilotTitle: "Battle Over Endor",
                faction: Faction.Rebel,
                initiative: 1,
                cost: 5,
                loadoutValue: 0,
                isLimited: true,
                abilityType: typeof(AdonFoxBattleOverEndorAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Talent,
                    UpgradeType.Missile,
                    UpgradeType.Device
                },
                tags: new List<Tags>
                {
                    Tags.BWing
                },
                charges: 2,
                regensCharges: 1,
                isStandardLayout: true,
                legality: new List<Legality>() { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ItsATrap));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.PartingGift));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ProtonRockets));
            MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.ProtonBombs));

            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReloadAction), ActionColor.Red));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(TargetLockAction)));

            ShipAbilities.Add(new GyroCockpit());

            PilotNameCanonical = "adonfox-battleoverendor";

            DefaultUpgrades.Remove(typeof(UpgradesList.SecondEdition.StabilizedSFoilsOpen));
        }
    }

    public class AdonFoxBoEXWA : AdonFoxBattleOverEndor
    {
        public AdonFoxBoEXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 12;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AdonFoxBattleOverEndorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsDefender += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsDefender -= CheckAbility;
        }
        private void CheckAbility()
        {
            if (HostShip.IsStressed)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDefenseStart, UseAbility);
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            if (Combat.Defender == HostShip)
            {
                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} adds an extra defense die");
                HostShip.AfterGotNumberOfDefenceDice += AddDefenseDie;
                Triggers.FinishTrigger();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AddDefenseDie(ref int dieCount)
        {
            HostShip.AfterGotNumberOfDefenceDice -= AddDefenseDie;
            dieCount++;
        }
    }
}