using Abilities.SecondEdition;
using Conditions;
using Content;
using Mods;
using Mods.ModsList;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.NabooRoyalN1Starfighter
{
    public class PadmeAmidala : NabooRoyalN1Starfighter
    {
        public PadmeAmidala() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Padmé Amidala",
                "Aggressive Negotiator",
                Faction.Republic,
                4,
                4,
                16,
                isLimited: true,
                abilityText: "While an enemy ship in your [Front Arc] defends or performs an attack, that ship can modify only 1 [Focus] result (other results can still be modified).",
                abilityType: typeof(PadmeAmidalaAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Sensor,
                    UpgradeType.Torpedo,
                },
                legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
            );

            if (ModsManager.Mods[typeof(LimitedEditionNabooRoyalN1StarfighterMod)].IsOn)
            {
                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e8/c1/e8c1866f-a83a-469f-b2c0-a144c166fced/swzp02_padme-amidala.jpg";
                ModelInfo.SkinName = "Silver";
            }
        }
    }

    public class PadmeAmidalaXWA : PadmeAmidala
    {
        public PadmeAmidalaXWA() : base()
        {
            (PilotInfo as PilotCardInfo25).Cost = 10;
            (PilotInfo as PilotCardInfo25).LoadoutValue = 13;
            (PilotInfo as PilotCardInfo25).LegalityInfo = new List<Legality> { Legality.XWA };
            (PilotInfo as PilotCardInfo25).ExtraUpgrades = new List<UpgradeType>
            {
                UpgradeType.Talent,
                UpgradeType.Astromech,
                UpgradeType.Sensor,
                UpgradeType.Modification,
                UpgradeType.Tech,
                UpgradeType.Torpedo,
            };

        }
    }
}

namespace Abilities.SecondEdition
{
    public class PadmeAmidalaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckPadmeAbilityAttacker;
            GenericShip.OnAttackStartAsDefenderGlobal += CheckPadmeAbilityDefender;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckPadmeAbilityAttacker;
            GenericShip.OnAttackStartAsDefenderGlobal -= CheckPadmeAbilityDefender;
        }

        public void CheckPadmeAbilityDefender()
        {
            CheckPadmeAbility(false);
        }

        public void CheckPadmeAbilityAttacker()
        {
            CheckPadmeAbility(true);
        }

        public void CheckPadmeAbility(bool isAttacker)
        {
            GenericShip target = isAttacker ? Combat.Attacker : Combat.Defender;

            if (target.Owner != HostShip.Owner &&
                HostShip.SectorsInfo.IsShipInSector(target, Arcs.ArcType.Front))
            {
                PadmeAmidalaCondition condition = new(target, HostShip);
                target.Tokens.AssignCondition(condition);
            }
        }
    }
}

namespace Conditions
{
    public class PadmeAmidalaCondition : GenericToken
    {
        bool SkippedFirst = false;

        public PadmeAmidalaCondition(GenericShip host, GenericShip source) : base(host)
        {
            Name = ImageName = "Debuff Token";
            TooltipType = source.GetType();
            Temporary = true;
        }

        public override void WhenAssigned()
        {
            Messages.ShowInfo($"Padmé Amidala: {Host.PilotInfo.PilotName} can only modify 1 focus result for this {(Host == Combat.Attacker ? "attack" : "defense")}.");

            Host.OnImmediatelyAfterRolling += LockDice;

            Host.OnAttackFinish += RemovePadmeAmidalaCondition;
        }

        private void LockDice(DiceRoll diceroll)
        {
            Host.OnImmediatelyAfterRolling -= LockDice;

            foreach (Die die in diceroll.DiceList)
            {
                if (die.Side == DieSide.Focus)
                {
                    if (SkippedFirst)
                    {
                        die.CannotBeModified = true;
                        die.ShowRerolledLock(true);
                    }
                    else
                        SkippedFirst = true;
                }
            }

            diceroll.OrganizeDicePositions();
        }

        public void RemovePadmeAmidalaCondition(GenericShip ship)
        {
            Host.Tokens.RemoveCondition(this);
        }

        public override void WhenRemoved()
        {
            Messages.ShowInfo($"Padmé Amidala: {Host.PilotInfo.PilotName}'s ability to modify focus results restored.");

            Host.OnAttackFinish -= RemovePadmeAmidalaCondition;
        }
    }
}