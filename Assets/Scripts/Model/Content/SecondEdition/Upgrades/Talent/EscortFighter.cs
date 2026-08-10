using ActionsList.SecondEdition;
using BoardTools;
using Content;
using Ship;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class EscortFighter : GenericUpgrade
    {
        public EscortFighter() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Escort Fighter",
                UpgradeType.Talent,
                abilityType: typeof(Abilities.SecondEdition.EscortFighterAbility),
                legalityInfo: new() { Legality.XWA },
                charges: 1,
                regensCharges: true
            );

            IsHidden = true;
            ImageUrl = "https://infinitearenas.com/xw2xwa/images/pilots/ronithblario-evacuationofdqar.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EscortFighterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += GenerateEscortFighterDiceModifications;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= GenerateEscortFighterDiceModifications;
        }

        private void GenerateEscortFighterDiceModifications(GenericShip ship)
        {
            Combat.Defender.AddAvailableDiceModification(new EscortFighterEffect(HostUpgrade), HostShip);
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class EscortFighterEffect : GenericAction
    {
        private GenericUpgrade HostUpgrade;

        public EscortFighterEffect(GenericUpgrade hostUpgrade) : base()
        {
            Name = DiceModificationName =  "Escort Fighter";
            HostUpgrade = hostUpgrade;
        }

        public override void ActionEffect(Action callBack)
        {
            HostUpgrade.State.SpendCharge();
            Combat.DiceRollDefence.AddDiceAndShow(DieSide.Focus);
            HostShip.Tokens.AssignToken(new StrainToken(HostShip),callBack);
        }

        public override bool IsDiceModificationAvailable()
        {
            ShotInfoArc arcInfo = new ShotInfoArc(
                Combat.Attacker,
                HostShip,
                Combat.ArcForShot
            );
            
            return Tools.IsSameTeam(Combat.Defender, HostShip)
                && Combat.Defender.ShipBase.Size != BaseSize.Small
                && arcInfo.InArc
                && HostUpgrade.State.Charges > 0;
        }

        public override int GetDiceModificationPriority()
        {
            return 89;
        }
    }
}
