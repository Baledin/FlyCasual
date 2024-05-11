﻿using ActionsList;
using Arcs;
using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class Vennie : Mg100StarFortress
        {
            public Vennie() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Vennie",
                    2,
                    51,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VennieAbility)
                );

                ModelInfo.SkinName = "Crimson";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VennieAbility : GenericAbility
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
            ship.AddAvailableDiceModificationOwn(new VennieDiceModification() 
            {
                ImageUrl = HostShip.ImageUrl
            });
        }

        private class VennieDiceModification : GenericAction
        {
            public VennieDiceModification()
            {
                Name = DiceModificationName = "Vennie";
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.AddDiceAndShow(DieSide.Focus);
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                if (Combat.AttackStep == CombatStep.Defence)
                {
                    foreach (GenericShip friendlyShip in Combat.Defender.Owner.Ships.Values)
                    {
                        ShotInfo shotInfo = new ShotInfo(friendlyShip, Combat.Attacker, friendlyShip.PrimaryWeapons);
                        if (shotInfo.InArcByType(ArcType.SingleTurret) && Tools.IsFriendly(friendlyShip, HostShip)) return true;
                    }
                }
                return false;
            }

            public override int GetDiceModificationPriority()
            {
                return 110;
            }
        }

    }
}