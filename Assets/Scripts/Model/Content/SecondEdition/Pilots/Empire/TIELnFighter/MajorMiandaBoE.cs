using Actions;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class MajorMiandaBoE : TIELnFighter
        {
            public MajorMiandaBoE() : base()
            {
                PilotInfo = new PilotCardInfo25(
                    "Major Mianda",
                    "Battle Over Endor",
                    Faction.Imperial,
                    5,
                    4,
                    loadoutValue: 0,
                    isStandardLayout: true,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    abilityType: typeof(Abilities.SecondEdition.MajorMiandaAbility),
                    legality: new List<Legality> { Legality.StandardLegal, Legality.ExtendedLegal }
                );

                ShipAbilities.Add(new Abilities.SecondEdition.FormedUpAbility());
                ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(EvadeAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(CoordinateAction), ActionColor.Red));
                ShipInfo.Shields++;

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.NoEscape));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Ruthless));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.SwarmTactics));

                PilotNameCanonical = "majormianda-battleoverendor";
            }
        }

        public class MajorMiandaBoEXWA : MajorMiandaBoE
        {
            public MajorMiandaBoEXWA() : base()
            {
                var pilotInfo = PilotInfo as PilotCardInfo25;
                pilotInfo.Cost = 10;
                pilotInfo.LegalityInfo = new List<Legality> { Legality.XWA };
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //During the End Phase, you may choose up to 2 friendly small ships at range 0-2, You and the chosen ships may perform a red barrel roll or red boost action.
    public class MajorMiandaAbility : GenericAbility
    {
        //private List<GenericShip> abilityTargets = new List<GenericShip>();
        private bool selfSelected = false;
        private List<GenericShip> selectedShips = new List<GenericShip>();
        private List<GenericShip> eligibleShips = new List<GenericShip>();

        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, HandleTrigger);
        }

        private void HandleTrigger(object sender, EventArgs e)
        {
            selfSelected = false;
            selectedShips.Clear();
            eligibleShips = BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(0, 2), Team.Type.Friendly).Where(n => n.ShipBase.Size == BaseSize.Small).ToList();
            SelectTargetRecursive();
        }

        private void SelectTargetRecursive()
        {
            Selection.ChangeActiveShip(HostShip);
            if (HostShip.Owner.Ships.Any(s => FilterTargets(s.Value)))
            {
                SelectTargetForAbility(
                    GrantAction,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "You may select yourself and/or up to 2 friendly small ships at range 0-2 to perform a red Barrel Roll or red Boost action",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void GrantAction()
        {
            if (TargetShip.Equals(HostShip))
            {
                selfSelected = true;
            }
            else
            {
                selectedShips.Add(TargetShip);
            }
            Selection.ChangeActiveShip(TargetShip);
            List<GenericAction> actions = new List<GenericAction>();
            actions.Add(new BoostAction() { Color = Actions.ActionColor.Red });
            actions.Add(new BarrelRollAction() { Color = Actions.ActionColor.Red });
            Selection.ThisShip.AskPerformFreeAction(
                actions,
                delegate
                {
                    SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();
                    SelectTargetRecursive();
                },
                HostShip.PilotInfo.PilotName,
                "You may perform a Red Barrel Roll or Red Boost action.",
                HostShip
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            //if (!Tools.IsFriendly(ship, HostShip)) return false;
            //if (ship.ShipBase.Size != BaseSize.Small) return false;
            //DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (!eligibleShips.Contains(ship)) return false;
            if (ship.Equals(HostShip) && selfSelected) return false;
            if (!ship.Equals(HostShip) && selectedShips.Count >= 2) return false;
            if (selectedShips.Contains(ship)) return false;
            return true;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}