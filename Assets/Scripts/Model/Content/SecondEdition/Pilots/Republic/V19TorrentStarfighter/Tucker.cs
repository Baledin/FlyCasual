using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.V19TorrentStarfighter
{
    public class Tucker : V19TorrentStarfighter
    {
        public Tucker()
        {
            PilotInfo = new PilotCardInfo25
            (
                "\"Tucker\"",
                "Blue Five",
                Faction.Republic,
                2,
                4,
                13,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.TuckerAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                },
                tags: new List<Tags>
                {
                    Tags.Clone
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class TuckerXWA : Tucker
    {
        public TuckerXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 9;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 8;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Modification,
                UpgradeType.Modification,
                UpgradeType.Missile,
                UpgradeType.Missile
            };
        }
    }
}

namespace Abilities.SecondEdition
{
    //After a friendly ship at range 1-2 performs an attack against an enemy ship in your front arc, you may perform a focus action.
    public class TuckerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            var range = new BoardTools.DistanceInfo(HostShip, Combat.Attacker).Range;

            if (Combat.Attacker.Owner == HostShip.Owner
                && Combat.Defender.Owner != HostShip.Owner
                && HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Front)
                && range >= 1 && range <= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskPerformFocusAction);
            }
        }

        private void AskPerformFocusAction(object sender, EventArgs e)
        {
            GenericShip previousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);

            HostShip.AskPerformFreeAction(
                new FocusAction(),
                () =>
                {
                    Selection.ChangeActiveShip(previousActiveShip);
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "After a friendly ship at range 1-2 performs an attack against an enemy ship in your Standard Front Arc, you may perform a Focus action",
                HostShip
            );
        }
    }
}
