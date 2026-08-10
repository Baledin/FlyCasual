using Abilities.SecondEdition;
using Actions;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class FirstOrderOrdnanceTechEoD : GenericUpgrade
    {
        public FirstOrderOrdnanceTechEoD() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "First Order Ordnance Tech",
                UpgradeType.Gunner,
                cost: 0,
                restriction: new FactionRestriction(Faction.FirstOrder),
                addAction: new ActionInfo(typeof(ReloadAction)),
                addActionLink: new LinkedActionInfo(typeof(ReloadAction), typeof(TargetLockAction), linkedColor: ActionColor.White),
                abilityType: typeof(FirstOrderOrdnanceTechEoDAbility),
                legalityInfo: new List<Legality> { Legality.XWA }
            );

            IsHidden = true;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class FirstOrderOrdnanceTechEoDAbility : GenericAbility
    {
        // After you gain a disarm token, gain 1 calculate token.
        // While you have exactly 1 disarm token, you can still perform front primary attacks.

        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += RegisterGainCalculateToken;
            HostShip.OnWeaponsDisabledCheck += AllowPrimaryFrontArcAttacks;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= RegisterGainCalculateToken;
            HostShip.OnWeaponsDisabledCheck -= AllowPrimaryFrontArcAttacks;
        }

        private void RegisterGainCalculateToken(GenericShip ship, GenericToken token)
        {
            if (token is WeaponsDisabledToken)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, GainCalculateToken);
            }
        }

        private void GainCalculateToken(object sender, EventArgs e)
        {
            HostShip.Tokens.AssignToken(new CalculateToken(HostShip), Triggers.FinishTrigger);
        }

        private void AllowPrimaryFrontArcAttacks(ref bool isDisabled)
        {
            ShotInfo shotInformation = new(HostShip, Selection.AnotherShip, HostShip.PrimaryWeapons.First());

            if (HostShip.Tokens.GetTokens<WeaponsDisabledToken>().Count == 1
                && Combat.ChosenWeapon is PrimaryWeaponClass
                && shotInformation.InArcByType(Arcs.ArcType.Front))
            {
                Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: Primary weapon attacks in the front arc are allowed while disarmed.");
                isDisabled = false;
            }
        }
    }
}