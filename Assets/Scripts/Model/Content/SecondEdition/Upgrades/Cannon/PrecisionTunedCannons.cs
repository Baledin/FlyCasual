using Abilities.SecondEdition;
using Arcs;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PrecisionTunedCannons : GenericSpecialWeapon
    {
        public PrecisionTunedCannons() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Precision-Tuned Cannons",
                UpgradeType.Cannon,
                cost: 0,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 2,
                    maxRange: 3,
                    arc: ArcType.Front
                ),
                abilityType: typeof(PrecisionTunedCannonsAbility),
                legalityInfo: new List<Legality>() { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            NameCanonical = "precisiontunedcannons-legendsandrelics";
        } 
    }

    public class PrecisionTunedCannonsXWA : PrecisionTunedCannons
    {
        public PrecisionTunedCannonsXWA() : base()
        {
            UpgradeInfo.Cost = 3;
            UpgradeInfo.LegalityInfo = new List<Legality>() { Legality.XWA };
            IsHidden = false;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PrecisionTunedCannonsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Add,
                1,
                sideCanBeChangedTo: DieSide.Focus
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && Combat.ChosenWeapon is UpgradesList.SecondEdition.PrecisionTunedCannons
                && Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye));
        }

        public int GetDiceModificationAiPriority()
        {
            return 110;
        }
    }
}