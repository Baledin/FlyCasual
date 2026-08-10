using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.T65XWing
{
    public class LukeSkywalkerBoY : T65XWing
    {
        public LukeSkywalkerBoY() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Luke Skywalker",
                "Battle of Yavin",
                Faction.Rebel,
                5,
                5,
                0,
                isLimited: true,
                abilityType: typeof(LukeSkywalkerAbility),
                force: 2,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Torpedo,
                    UpgradeType.Astromech,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.LightSide,
                    Tags.XWing
                },
                seImageNumber: 2,
                skinName: "Luke Skywalker",
                isStandardLayout: true,
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            ShipAbilities.Add(new HopeAbility());

            MustHaveUpgrades.Add(typeof(AttackSpeed));
            MustHaveUpgrades.Add(typeof(InstinctiveAim));
            MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
            MustHaveUpgrades.Add(typeof(R2D2BoY));

            PilotNameCanonical = "lukeskywalker-battleofyavin";
        }
    }

    public class LukeSkywalkerBoYXWA : LukeSkywalkerBoY
    {
        public LukeSkywalkerBoYXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 15;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HopeAbility : GenericAbility
    {
        private GenericShip PreviousCurrentShip { get; set; }

        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool flag)
        {
            if (!Tools.IsAnotherFriendly(HostShip, ship)) return;

            DistanceInfo distanceInfo = new DistanceInfo(HostShip, ship);
            if (distanceInfo.Range > 3) return;

            RegisterAbilityTrigger(
                TriggerTypes.OnShipIsDestroyed,
                AskWhatToDo,
                customTriggerName: $"Hope (ID: {HostShip.ShipId})"
            );
        }

        private void AskWhatToDo(object sender, EventArgs e)
        {
            PreviousCurrentShip = Selection.ThisShip;

            Selection.ChangeActiveShip(HostShip);
            Selection.ThisShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new FocusAction(){ HostShip = HostShip },
                    new BoostAction(){ HostShip = HostShip },
                },
                FinishAbility,
                descriptionShort: "Hope",
                descriptionLong: "You may perform a Focus or Boost action"
            );
        }

        private void FinishAbility()
        {
            Selection.ChangeActiveShip(PreviousCurrentShip);
            Triggers.FinishTrigger();
        }
    }
}