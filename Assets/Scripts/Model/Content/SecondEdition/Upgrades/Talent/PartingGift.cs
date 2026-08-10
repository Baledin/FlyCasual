using Abilities.SecondEdition;
using BoardTools;
using Bombs;
using Content;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class PartingGift : GenericUpgrade
    {
        public PartingGift() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Parting Gift",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(PartingGiftAbility)
            );

            IsHidden = true;

            NameCanonical = "partinggift-legendsandrelics";
        }
    }

    public class PartingGiftXwa : PartingGift
    {
        public PartingGiftXwa() : base()
        {
            UpgradeInfo.Cost = 2;
            UpgradeInfo.LegalityInfo = new List<Legality> { Legality.XWA };
            IsHidden = false;
        }
    }
}

namespace Abilities.SecondEdition
{

    public class PartingGiftAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShipIsDestroyed += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShipIsDestroyed -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship, bool isFled)
        {
            if (!isFled)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, UseAbility);
            }
        }

        private void UseAbility(object sender, EventArgs e)
        {
            HostShip.OnGetAvailableBombDropTemplatesTwoConditions += GetBombTemplates;
            BombsManager.RegisterBombDropTriggerIfAvailable(HostShip, TriggerTypes.OnAbilityDirect);
            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private void GetBombTemplates(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            HostShip.OnGetAvailableBombDropTemplatesTwoConditions -= GetBombTemplates;
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left,
                ManeuverSpeed.Speed1, isBombTemplate: true));
            availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right,
                ManeuverSpeed.Speed1, isBombTemplate: true));
        }
    }
}