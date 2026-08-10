using BoardTools;
using Content;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class AaylaSecura : Eta2Actis
    {
        public AaylaSecura()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Aayla Secura",
                "Confident Warrior",
                Faction.Republic,
                5,
                5,
                15,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.AaylaSecuraActisAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Modification,
                    UpgradeType.Cannon
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Blue",
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );
        }
    }

    public class AaylaSecuraXWA : AaylaSecura
    {
        public AaylaSecuraXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 11;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 14;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AaylaSecuraActisAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Aayla Secura",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus,
                isGlobal: true
            );
        }

        private bool IsAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                ShotInfoArc arcInfo = new ShotInfoArc(
                    HostShip,
                    Combat.Attacker,
                    HostShip.SectorsInfo.Arcs.First(n => n.Facing == Arcs.ArcFacing.Front)
                );

                if (arcInfo.InArc && arcInfo.Range <= 1) result = true;
            }

            return result;
        }

        private int GetAiPriority()
        {
            return 100;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
