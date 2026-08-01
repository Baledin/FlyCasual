using Arcs;
using BoardTools;
using Bombs;
using Movement;
using Players;
using Remote;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Remote
{
    public class CommandoTeam : GenericRemote
    {
        public CommandoTeam(GenericPlayer owner) : base(owner)
        {
            RemoteInfo = new RemoteInfo25(
                name: "Commando Team",
                initiative: 2,
                arcInfo: new ShipArcsInfo(ArcType.Front, 2),
                agility: 2,
                hull: 2,
                imageUrl: "https://infinitearenas.com/xw2xwa/images/remotes/imperialsupercommandos.png",
                abilityType: typeof(Abilities.SecondEdition.CommandoTeam),
                charges: 2,
                soundInfo: new ShipSoundInfo(
                    new List<string>()
                    { },
                    "TIE-Fire", 2
                )
            );
        }

        public override Dictionary<string, Vector3> BaseEdges
        {
            get
            {
                return new Dictionary<string, Vector3>()
                {
                    { "R0", new Vector3(1.004529f, 0, 3.072494f) },
                    { "R1", new Vector3(0.9982725f, 0, 3.007827f) },
                    { "R2", new Vector3(1.022992f, 0, 3.133916f) },
                    { "R3", new Vector3(1.05272f, 0, 3.18902f) },
                    { "R4", new Vector3(1.09224f, 0, 3.23503f) },
                    { "R5", new Vector3(1.139557f, 0, 3.269648f) },
                    { "R6", new Vector3(1.192303f, 0, 3.291141f) },
                    { "R7", new Vector3(1.247838f, 0, 3.298428f) },
                    { "R8", new Vector3(1.507493f, 0, 3.298428f) },
                    { "R9", new Vector3(1.79818f, 0, 3.007737f) },
                    { "R10", new Vector3(1.798184f, 0, -0.007309198f) },
                    { "R11", new Vector3(0.9982721f, 0, -0.007408261f) },
                    { "R12", new Vector3(1.507542f, 0, -0.2979491f) },
                    { "R13", new Vector3(1.247837f, 0, -0.297949f) },
                    { "R14", new Vector3(1.192303f, 0, -0.2906724f) },
                    { "R15", new Vector3(1.139557f, 0, -0.2691795f) },
                    { "R16", new Vector3(1.092239f, 0, -0.2345611f) },
                    { "R17", new Vector3(1.052719f, 0, -0.1885616f) },
                    { "R18", new Vector3(1.022991f, 0, -0.1334678f) },
                    { "R19", new Vector3(1.004528f, 0, -0.07205403f) },
                    { "R20", new Vector3(-0.9982678f, 0, 3.008066f) },
                    { "R21", new Vector3(-0.9982682f, 0, -0.007648349f) },
                    { "R22", new Vector3(-1.798195f, 0, 3.008166f) },
                    { "R23", new Vector3(-1.507934f, 0, 3.298428f) },
                    { "R24", new Vector3(-1.798195f, 0, -0.007747412f) },
                    { "R25", new Vector3(-1.507992f, 0, -0.2979496f) },
                    { "R26", new Vector3(-1.247833f, 0, 3.298428f) },
                    { "R27", new Vector3(-1.247833f, 0, -0.2979497f) },
                    { "R28", new Vector3(-1.192295f, 0, 3.291151f) },
                    { "R29", new Vector3(-1.192295f, 0, -0.2906722f) },
                    { "R30", new Vector3(-1.139549f, 0, 3.269678f) },
                    { "R31", new Vector3(-1.139549f, 0, -0.2691993f) },
                    { "R32", new Vector3(-1.092231f, 0, 3.235078f) },
                    { "R33", new Vector3(-1.052711f, 0, 3.189109f) },
                    { "R34", new Vector3(-1.022983f, 0, 3.134056f) },
                    { "R35", new Vector3(-1.004528f, 0, 3.072683f) },
                    { "R36", new Vector3(-1.092232f, 0, -0.23462f) },
                    { "R37", new Vector3(-1.052712f, 0, -0.18865f) },
                    { "R38", new Vector3(-1.022984f, 0, -0.1336068f) },
                    { "R39", new Vector3(-1.004524f, 0, -0.07225502f) },
                };
            }
        }

        public override bool HasCombatActivation
        {
            get
            {
                return State.Charges > 0 && !IsAttackPerformed && !IsOverlapped();
            }
        }

        private bool IsOverlapped()
        {
            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (ship.RemotesOverlapped.Contains(this))
                {
                    return true;
                }
            }

            return false;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CommandoTeam : GenericAbility
    {
        private readonly int SelectedJointIndex = 2;
        private ManeuverTemplate SelectedManeuverTemplate;

        public override void ActivateAbility()
        {
            Phases.Events.OnActivationPhaseStart += RegisterRepositionTrigger;

            GenericShip.OnPositionFinishGlobal += CheckRemoteOverlapping;
            GenericShip.OnRemoteWasDroppedGlobal += CheckOverlap;

            HostShip.OnAttackFinishAsAttacker += SpendCharge;

            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Change,
                    1,
                    new List<DieSide>() { DieSide.Focus },
                    DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnActivationPhaseStart -= RegisterRepositionTrigger;

            GenericShip.OnPositionFinishGlobal -= CheckRemoteOverlapping;
            GenericShip.OnRemoteWasDroppedGlobal -= CheckOverlap;

            HostShip.OnAttackFinishAsAttacker -= SpendCharge;

            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            return HostShip.IsAttacking && Combat.CurrentDiceRoll.Focuses > 0;
        }

        private int GetDiceModificationAiPriority()
        {
            return 90;
        }

        private void SpendCharge(GenericShip ship)
        {
            HostShip.SpendCharge();
        }

        private void CheckOverlap()
        {
            GameManagerScript.Instance.StartCoroutine(ReCheckOverlap(delegate { }));
        }

        private IEnumerator ReCheckOverlap(Action callback)
        {
            ObstaclesStayDetectorForced collisionDetector = HostShip.Model.GetComponentInChildren<ObstaclesStayDetectorForced>();

            collisionDetector.ReCheckCollisionsStart();
            collisionDetector.TheShip = HostShip;
            yield return new WaitForFixedUpdate();

            bool overlapsShip = collisionDetector.OverlapsShipNow;
            bool outsidePlayArea = collisionDetector.OffTheBoardNow;
            collisionDetector.ReCheckCollisionsFinish();

            if (outsidePlayArea)
            {
                Messages.ShowInfo($"{HostShip.PilotInfo.PilotName} fled the play area.");
                HostShip.DestroyShipForced(callback, true);
            }
            else if (overlapsShip)
            {
                foreach (GenericShip ship in collisionDetector.OverlappedShipsNow)
                {
                    if (!ship.RemotesOverlapped.Contains((GenericRemote)HostShip))
                    {
                        ship.RemotesOverlapped.Add((GenericRemote)HostShip);
                        if (!Tools.IsFriendly(ship, HostShip))
                        {
                            Messages.ShowInfo($"{ship.PilotInfo.PilotName} gains 1 strain due to overlapping {HostShip.PilotInfo.PilotName}");
                            ship.Tokens.AssignToken(new Tokens.StrainToken(ship), callback);
                        }
                    }
                }
            }
        }

        private void CheckRemoteOverlapping(GenericShip ship)
        {
            if (ship is GenericRemote) return;

            if (Tools.IsSameTeam(ship, HostShip)) return;

            if (ship.RemotesOverlapped.Contains(HostShip))
            {
                Messages.ShowInfo($"{ship.PilotInfo.PilotName} gains 1 strain due to overlapping {HostShip.PilotInfo.PilotName}");
                ship.Tokens.AssignToken(new Tokens.StrainToken(ship), delegate { });
            }
        }

        private void RegisterRepositionTrigger()
        {
            // Always register
            RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseStart, AskToPerformReposition);
        }

        private void AskToPerformReposition(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);
            AskToUseAbility(
                "Do you want to relocate",
                NeverUseByDefault,
                AskSelectTemplate,
                descriptionLong: "You may relocate using a 2 straight or turn template",
                imageHolder: HostShip,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void AskSelectTemplate(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            TemplateDecisionSubphase templateDecisionSubphase = Phases.StartTemporarySubPhaseNew<TemplateDecisionSubphase>(
                "Template decision",
                StartReposition
            );

            templateDecisionSubphase.DescriptionShort = "Select template";
            templateDecisionSubphase.DecisionOwner = HostShip.Owner;

            templateDecisionSubphase.ShowSkipButton = false;

            templateDecisionSubphase.AddDecision(
                "Straight 2",
                delegate { SelectTemplate(new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed2)); },
                isCentered: true
            );

            templateDecisionSubphase.AddDecision(
                "Turn 1 Left",
                delegate { SelectTemplate(new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Left, ManeuverSpeed.Speed1)); }
            );

            templateDecisionSubphase.AddDecision(
                "Turn 1 Right",
                delegate { SelectTemplate(new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Right, ManeuverSpeed.Speed1)); }
            );

            templateDecisionSubphase.DefaultDecisionName = "Straight 2";

            templateDecisionSubphase.Start();
        }

        private void SelectTemplate(ManeuverTemplate maneuverTemplate)
        {
            SelectedManeuverTemplate = maneuverTemplate;
            DecisionSubPhase.ConfirmDecision();
        }

        private void StartReposition()
        {
            GenericRemote remote = HostShip as GenericRemote;
            SelectedManeuverTemplate.ApplyTemplate(remote, SelectedJointIndex);

            GameManagerScript.Wait(1, ContinueReposition);
        }

        private void ContinueReposition()
        {
            GenericRemote remote = HostShip as GenericRemote;
            HostShip.SetPosition(SelectedManeuverTemplate.GetFinalPosition());
            HostShip.SetAngles(SelectedManeuverTemplate.GetFinalAngles());
            GameManagerScript.Wait(1, FinishReposition);
        }

        private void FinishReposition()
        {
            SelectedManeuverTemplate.DestroyTemplate();
            CheckOverlap();
            Triggers.FinishTrigger();
        }

        private class TemplateDecisionSubphase : DecisionSubPhase { }
    }

    public class DeployCommandoTeam : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAllowDrop;
            HostShip.OnSystemsAbilityActivation += RegisterAskDropCommandos;
            HostShip.OnAllowBombDropFrontGuides += AllowFrontGuides;
            HostShip.OnRemoteWasDroppedUpgrade += SpendCharge;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAllowDrop;
            HostShip.OnSystemsAbilityActivation -= RegisterAskDropCommandos;
            HostShip.OnAllowBombDropFrontGuides -= AllowFrontGuides;
            HostShip.OnRemoteWasDroppedUpgrade -= SpendCharge;
        }

        private void CheckAllowDrop(GenericShip ship, ref bool isAllowed)
        {
            isAllowed = HostUpgrade.State.Charges > 0;
        }

        private void RegisterAskDropCommandos(GenericShip ship)
        {
            if (HostUpgrade.State.Charges < 1) return;

            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, DeployRemote);
        }

        private void DeployRemote(object sender, EventArgs e)
        {
            BombsManager.RegisterBombDropTriggerIfAvailable(
                HostShip,
                TriggerTypes.OnAbilityDirect,
                subType: UpgradeSubType.Remote,
                type: HostUpgrade.UpgradeInfo.RemoteType
            );

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private void AllowFrontGuides(GenericUpgrade upgrade, ref bool isAllowed)
        {
            if (upgrade == HostUpgrade)
                isAllowed = true;
        }

        private void SpendCharge(GenericUpgrade upgrade)
        {
            if (upgrade == HostUpgrade)
                upgrade.State.SpendCharge();
        }
    }
}