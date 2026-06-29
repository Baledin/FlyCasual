using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class AcceleratedSensorArray : GenericUpgrade
    {
        public AcceleratedSensorArray() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Accelerated Sensor Array",
                UpgradeType.Tech,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.AcceleratedSensorArrayAbility),
                legalityInfo: new List<Legality> { Legality.XWA }
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/stomeronistarck-evacuationofdqar.png";
        }
    }
}
namespace Abilities.SecondEdition
{
    // While you defend or perform a primary attack, if the speed of your revealed maneuver is 3-5, you may reroll 1 die.
    // If your revealed maneuver is an advanced maneuver, you may reroll up to 2 dice instead.
    public class AcceleratedSensorArrayAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AcceleratedSensorArrayEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AcceleratedSensorArrayEffect;
        }

        private void AcceleratedSensorArrayEffect(GenericShip host)
        {
            GenericAction newAction = new ActionsList.SecondEdition.AcceleratedSensorArrayEffect() {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl,
            };

            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class AcceleratedSensorArrayEffect : GenericAction
    {
        public AcceleratedSensorArrayEffect() : base()
        {
            Name = DiceModificationName =  "Accelerated Sensor Array";
        }

        public override void ActionEffect(Action callBack)
        {
            int diceRerollCount;
            if (HostShip.RevealedManeuver.IsAdvancedManeuver)
            {
                diceRerollCount = 2;
            }
            else
            {
                diceRerollCount = 1;
            }

            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = diceRerollCount,
                CallBack = callBack
            };

            diceRerollManager.Start();
        }

        public override bool IsDiceModificationAvailable()
        {
            return (HostShip.RevealedManeuver.Speed >= 3 || HostShip.RevealedManeuver.IsAdvancedManeuver)
                && Combat.ShotInfo.Range > 0
                && ((Combat.Attacker == HostShip && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
                    || Combat.Defender == HostShip);
        }

        public override int GetDiceModificationPriority()
        {
            return 90;
        }
    }
}
