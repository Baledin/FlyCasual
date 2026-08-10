using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Arcs;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.Mg100StarFortress
{
    public class VennieEoD : Mg100StarFortress
    {
        public VennieEoD() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Vennie",
                "Evacuation of D'Qar",
                Faction.Resistance,
                2,
                17,
                0,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.VennieAbilityEoD),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Gunner,
                    UpgradeType.Gunner,
                    UpgradeType.Device,
                },
                legality: new List<Legality>() { Legality.XWA },
                isStandardLayout: true
            );

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(RotateArcAction), typeof(FocusAction)));

            ShipAbilities.Add(new ModularBombingMagazine());

            MustHaveUpgrades.Add(typeof(PerceptiveCopilot));
            MustHaveUpgrades.Add(typeof(ProtonBombs));
            MustHaveUpgrades.Add(typeof(DedicatedGunners));

            ModelInfo.SkinName = "Crimson";
            PilotNameCanonical = "vennie-evacuationofdqar";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VennieAbilityEoD : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddVennieAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddVennieAbility;
        }

        private void AddVennieAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new VennieEoDDiceModification()
            {
                ImageUrl = HostShip.ImageUrl
            });
        }

        private class VennieEoDDiceModification : GenericAction
        {
            public VennieEoDDiceModification()
            {
                Name = DiceModificationName = "Vennie";
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus,DieSide.Success);
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                if (Combat.AttackStep == CombatStep.Defence && Combat.CurrentDiceRoll.HasResult(DieSide.Focus))
                {
                    foreach (GenericShip friendlyShip in Combat.Defender.Owner.Ships.Values)
                    {
                        ShotInfo shotInfo = new ShotInfo(friendlyShip, Combat.Attacker, friendlyShip.PrimaryWeapons);
                        if (shotInfo.InArcByType(ArcType.SingleTurret)) return true;
                    }
                }

                return false;
            }

            public override int GetDiceModificationPriority()
            {
                return 80;
            }
        }
    }
}
