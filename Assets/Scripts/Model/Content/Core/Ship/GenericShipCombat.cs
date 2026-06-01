using Abilities;
using ActionsList;
using BoardTools;
using Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    public enum WeaponTypes
    {
        PrimaryWeapon,
        Torpedo,
        Missile,
        Cannon,
        Turret,
        Illicit,
        Talent,
        Force
    }

    public partial class GenericShip
    {
        public List<PrimaryWeaponClass> PrimaryWeapons = new();

        public Damage Damage { get; protected set; }

        public DiceRoll AssignedDamageDiceroll = new(DiceKind.Attack, 0, DiceRollCheckType.Virtual);

        public bool IsCannotAttackSecondTime { get; set; }
        public bool CanAttackBumpedTargetAlways { get; set; }
        public bool IgnoresBombDetonationEffect { get; set; }
        public bool AttackIsAlwaysConsideredHit { get; set; }
        public int DiceRolledLastAttack { get; set; }

        // EVENTS
        public delegate void EventHandlerShipWeaponRefBool(GenericShip ship, IShipWeapon weapon, ref bool isAllowed);

        public event EventHandlerShip OnSystemsPhaseStart;

        public event EventHandlerShip OnActivationPhaseStart;
        public event EventHandlerShip OnActionSubPhaseStart;
        public event EventHandlerShip OnRoundEnd;

        public event EventHandlerBoolStringList OnTryPerformAttack;
        public static event EventHandlerBoolStringList OnTryPerformAttackGlobal;

        public event EventHandlerTokensList OnGenerateAvailableAttackPaymentList;

        public event EventHandlerShipWeaponTypeBool OnModifyWeaponAttackRequirement;
        public static event EventHandlerShipWeaponTypeBool OnModifyWeaponAttackRequirementGlobal;

        public event EventHandler OnAttackStartAsAttacker;
        public static event EventHandler OnAttackStartAsAttackerGlobal;
        public event EventHandler OnAttackStartAsDefender;
        public static event EventHandler OnAttackStartAsDefenderGlobal;

        public event EventHandler OnShotStartAsAttacker;
        public event EventHandler OnShotStartAsDefender;
        public static event EventHandler OnDiceAboutToBeRolled;

        public event EventHandlerShip OnCheckCancelCritsFirst;

        public event EventHandler OnDefenceStartAsAttacker;
        public event EventHandler OnDefenceStartAsDefender;

        public event EventHandler OnAtLeastOneCritWasCancelledByDefender;

        public event EventHandler OnShotHitAsAttacker;
        public event EventHandler OnShotHitAsDefender;
        public static event EventHandler OnShotHitAsDefenderGlobal;

        public static event EventHandlerShipDamage OnTryDamagePreventionGlobal;
        public event EventHandlerShipDamage OnTryDamagePrevention;

        public event EventHandler OnAttackHitAsAttacker;
        public static event EventHandler OnAttackHitAsAttackerGlobal;
        public event EventHandler OnAttackHitAsDefender;
        public static event EventHandler OnAttackHitAsDefenderGlobal;
        public event EventHandler OnAttackMissedAsAttacker;
        public event EventHandler OnAttackMissedAsDefender;
        public static event EventHandler OnAttackMissedAsAttackerGlobal;
        public event EventHandler OnShieldLost;

        public event EventHandlerShip OnCombatCheckExtraAttack;

        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponAttackDice;
        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponDefenceDice;
        public event EventHandlerInt AfterGotNumberOfAttackDice;
        public event EventHandlerInt AfterGotNumberOfAttackDiceCap;
        public event EventHandlerInt AfterGotNumberOfDefenceDice;
        public static event EventHandlerInt AfterGotNumberOfDefenceDiceGlobal;
        public event EventHandlerInt AfterGotNumberOfDefenceDiceCap;
        public event EventHandlerInt AfterNumberOfDefenceDiceConfirmed;

        public event EventHandlerShip AfterAssignedDamageIsChanged;

        public event EventHandlerBool OnCheckFaceupCrit;
        public event EventHandlerShipCritArgs OnFaceupCritCardReadyToBeDealt;
        public static event EventHandlerShipCritArgs OnFaceupCritCardReadyToBeDealtGlobal;
        public event EventHandlerShipCritArgs OnFaceupCritCardRevealed;
        public static event EventHandlerShipCritArgs OnFaceupCritCardRevealedGlobal;
        public event EventHandlerShipCritArgs OnAssignCrit;

        public event EventHandlerShipBool OnDamageWasSuccessfullyDealt;
        public static event EventHandlerShipBoolArgs OnSufferHullDamageGlobal;
        public event EventHandlerShip OnDamageCardSeverityIsChecked;
        public static event EventHandlerShip OnDamageCardSeverityIsCheckedGlobal;
        public event EventHandlerShip OnDamageCardIsDealt;
        public static event EventHandlerShip OnDamageCardIsDealtGlobal;
        public static event EventHandlerShipDamage OnDamageInstanceResolvedGlobal;
        public static event EventHandlerShipBomb OnAfterSufferBombEffect;

        public event EventHandlerShip OnBeforeCheckPreventDestruction;
        public event EventHandlerShipRefBool OnCheckPreventDestruction;
        public static event EventHandlerShipRefBool OnCheckPreventDestructionGlobal;
        public event EventHandlerShipBool OnShipIsDestroyed;
        public static event EventHandlerShipBool OnShipIsDestroyedGlobal;
        public event EventHandlerShip OnShipIsReadyToBeRemoved;
        public event EventHandlerShip OnShipIsRemoved_System;
        public static event EventHandlerShip OnShipIsReadyToBeRemovedGlobal;

        public event EventHandler AfterAttackWindow;

        public event EventHandlerShip OnAttackFinish;
        public event EventHandlerShip OnAttackFinishAsAttacker;
        public event EventHandlerShip OnAttackFinishAsDefender;
        public static event EventHandlerShip OnAttackFinishGlobal;

        public event EventHandlerUpgradeRefInt OnGetReloadChargesCount;
        public event EventHandlerBombDropTemplates OnGetAvailableBombDropTemplatesTwoConditions;
        public event EventHandlerBombDropTemplates OnGetAvailableBombDropTemplatesOneCondition;
        public event EventHandlerBombDropTemplates OnGetAvailableBombDropTemplatesNoConditions;
        public event EventHandlerBombDropTemplates OnGetAvailableBombDropTemplatesForbid;
        public event EventHandlerBombDropTemplates OnGetAvailableBombLaunchTemplates;
        public event EventHandlerBombDropTemplates OnGetAvailableBombLaunchTemplatesModifications;
        public event EventHandlerDirection OnGetBombTemplateDirection;

        public event EventHandlerBarrelRollTemplates OnGetAvailableBarrelRollTemplates;
        public event EventHandlerDecloakTemplates OnGetAvailableDecloakBarrelRollTemplates;
        public event EventHandlerDecloakTemplates OnGetAvailableDecloakBoostTemplates;
        public event EventHandlerBoostTemplates OnGetAvailableBoostTemplates;
        public event EventHandlerSlamTemplates OnGetAvailableSlamTemplates;
        public event EventHandlerRefString OnUpdateChosenBoostTemplate;
        public event EventHandlerRefManeuverTemplate OnUpdateChosenBarrelRollTemplate;
        public event EventHandlerMovement OnUpdateChosenSlamTemplate;

        public event EventHandlerDiceroll OnImmediatelyAfterRolling;
        public event EventHandlerDiceroll OnImmediatelyAfterReRolling;

        public event EventHandlerBool OnWeaponsDisabledCheck;

        public event EventHandlerBool2Ships OnCanAttackBumpedTarget;
        public static event EventHandlerBool2Ships OnCanAttackBumpedTargetGlobal;

        public event EventHandlerShipRefBool OnCanAttackWhileLandedOnObstacle;
        public static event EventHandlerShipRefBool OnCanAttackWhileLandedOnObstacleGlobal;

        public event EventHandlerShip OnCombatActivation;
        public static event EventHandlerShip OnCombatActivationGlobal;
        public event EventHandlerShip OnCombatDeactivation;

        public event EventHandlerShip OnCheckSufferBombDetonation;

        public event EventHandlerObjArgsBool OnSufferCriticalDamage;
        public event EventHandlerObjArgsBool OnSufferDamageDecidingSeverity;

        public event EventHandlerBool OnTryConfirmDiceResults;

        public event EventHandlerShip OnCombatCompareResults;
        public event EventHandler OnAfterNeutralizeResults;
        public event EventHandler OnAfterNeutralizeResultsAttacker;

        public event EventHandler AfterAttackDiceModification;
        public event EventHandlerModifyDice OnTryDiceResultModification;
        public event EventHandlerTrySelectDie OnTrySelectDie;

        public event EventHandler BeforeBombWillBeDropped;
        public event EventHandler OnBombWillBeDropped;
        public event EventHandler OnBombWasDropped;
        public event EventHandler OnBombWasLaunched;
        public event EventHandler OnRemoteWasDropped;
        public static event EventHandler OnRemoteWasDroppedGlobal;
        public event EventHandler OnRemoteWasLaunched;
        public static event EventHandler OnRemoteWasLaunchedGlobal;
        public event EventHandler OnCheckDropOfSecondDevice;

        public event EventHandelerWeaponRange OnUpdateWeaponRange;
        public static event EventHandelerWeaponRange OnUpdateWeaponRangeGlobal;

        public event EventHandlerShipRefInt OnShotObstructedByMe;

        public event EventHandlerShipRefBool OnTargetForAttackIsAllowed;

        public event EventHandlerShipWeaponRefBool OnCheckIsForbiddenWeapon;

        // TRIGGERS

        public void CallOnActivationPhaseStart()
        {
            OnActivationPhaseStart?.Invoke(this);
        }

        public void CallOnSystemsPhaseStart()
        {
            OnSystemsPhaseStart?.Invoke(this);
        }

        public void CallOnRoundEnd()
        {
            OnRoundEnd?.Invoke(this);
        }

        public void CallOnActionSubPhaseStart()
        {
            OnActionSubPhaseStart?.Invoke(this);
        }

        public bool CallCanPerformAttack(bool result = true, List<string> stringList = null, bool isSilent = false)
        {
            stringList ??= new List<string>();

            OnTryPerformAttack?.Invoke(ref result, stringList);

            OnTryPerformAttackGlobal?.Invoke(ref result, stringList);

            if (!isSilent && stringList.Count > 0)
            {
                foreach (string errorMessage in stringList)
                {
                    Messages.ShowErrorToHuman(errorMessage);
                }
            }

            return result;
        }

        public void CallAfterAttackDiceModification()
        {
            AfterAttackDiceModification?.Invoke();
        }

        public void CallAttackStart()
        {
            if (Combat.Attacker.ShipId == this.ShipId)
            {
                IsAttackPerformed = true;

                OnAttackStartAsAttacker?.Invoke();
                OnAttackStartAsAttackerGlobal?.Invoke();
            }
            else if (Combat.Defender.ShipId == this.ShipId)
            {
                OnAttackStartAsDefender?.Invoke();
                OnAttackStartAsDefenderGlobal?.Invoke();
            }
        }

        public void CallDiceAboutToBeRolled(Action callback)
        {
            OnDiceAboutToBeRolled?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.OnDiceAboutToBeRolled, callback);
        }

        public void CallShotStart()
        {
            if (Combat.Attacker.ShipId == this.ShipId)
            {
                OnShotStartAsAttacker?.Invoke();
            }
            else if (Combat.Defender.ShipId == this.ShipId)
            {
                OnShotStartAsDefender?.Invoke();
            }
        }

        public void CallCheckCancelCritsFirst()
        {
            OnCheckCancelCritsFirst?.Invoke(this);
        }

        public void CallDefenceStartAsAttacker()
        {
            OnDefenceStartAsAttacker?.Invoke();
        }

        public void CallDefenceStartAsDefender()
        {
            OnDefenceStartAsDefender?.Invoke();
        }

        public void CallShotHitAsAttacker()
        {
            OnShotHitAsAttacker?.Invoke();
        }

        public void CallShotHitAsDefender()
        {
            OnShotHitAsDefenderGlobal?.Invoke();

            OnShotHitAsDefender?.Invoke();
        }

        public void CallTryDamagePrevention(DamageSourceEventArgs e, Action callback)
        {
            OnTryDamagePreventionGlobal?.Invoke(this, e);
            OnTryDamagePrevention?.Invoke(this, e);

            Triggers.ResolveTriggers(TriggerTypes.OnTryDamagePrevention, callback);
        }

        public void CallOnAttackHitAsAttacker()
        {
            OnAttackHitAsAttackerGlobal?.Invoke();

            OnAttackHitAsAttacker?.Invoke();
        }

        public void CallOnAttackHitAsDefender()
        {
            OnAttackHitAsDefenderGlobal?.Invoke();

            OnAttackHitAsDefender?.Invoke();
        }

        public void CallOnAttackMissedAsAttacker()
        {
            OnAttackMissedAsAttacker?.Invoke();
            OnAttackMissedAsAttackerGlobal?.Invoke();
        }

        public void CallOnAttackMissedAsDefender()
        {
            OnAttackMissedAsDefender?.Invoke();
        }

        public void CallAfterAttackWindow()
        {
            AfterAttackWindow?.Invoke();
        }

        public void CallAttackFinish()
        {
            OnAttackFinish?.Invoke(this);
        }

        public void CallAttackFinishGlobal()
        {
            OnAttackFinishGlobal?.Invoke(this);
        }

        public void CallAttackFinishAsAttacker()
        {
            OnAttackFinishAsAttacker?.Invoke(this);
        }

        public void CallAttackFinishAsDefender()
        {
            OnAttackFinishAsDefender?.Invoke(this);
        }

        public void CallOnImmediatelyAfterRolling(DiceRoll diceroll, Action callBack)
        {
            OnImmediatelyAfterRolling?.Invoke(diceroll);

            Triggers.ResolveTriggers(TriggerTypes.OnImmediatelyAfterRolling, callBack);
        }

        public void CallOnImmediatelyAfterReRolling(DiceRoll diceroll, Action callBack)
        {
            OnImmediatelyAfterReRolling?.Invoke(diceroll);

            Triggers.ResolveTriggers(TriggerTypes.OnImmediatelyAfterReRolling, callBack);
        }

        public void CallOnAtLeastOneCritWasCancelledByDefender()
        {
            OnAtLeastOneCritWasCancelledByDefender?.Invoke();
        }

        public List<Type> GetWeaponAttackRequirement(GenericSpecialWeapon weapon, bool isSilent)
        {
            List<Type> tokenTypeAttackRequirement = weapon.WeaponInfo.RequiresTokens;

            GenericShip.OnModifyWeaponAttackRequirementGlobal?.Invoke(this, weapon, ref tokenTypeAttackRequirement, isSilent);
            OnModifyWeaponAttackRequirement?.Invoke(this, weapon, ref tokenTypeAttackRequirement, isSilent);

            return tokenTypeAttackRequirement;
        }

        public void CallOnGenerateAvailableAttackPaymentList(List<GenericToken> tokens)
        {
            OnGenerateAvailableAttackPaymentList?.Invoke(tokens);
        }

        public void CallOnDamageCardIsDealt(Action callBack)
        {
            OnDamageCardIsDealt?.Invoke(this);
            OnDamageCardIsDealtGlobal?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnDamageCardIsDealt, callBack);
        }

        public void CallOnDamageInstanceResolved(DamageSourceEventArgs dsource, Action callback)
        {
            if (this == Combat.Defender) Combat.DamageInfo.IsDefenderSufferedDamage = true;
            OnDamageInstanceResolvedGlobal?.Invoke(this, dsource);

            Triggers.ResolveTriggers(TriggerTypes.OnDamageInstanceResolved, callback);
        }

        public void CallOnShieldIsLost(Action callback)
        {
            OnShieldLost?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.OnShieldIsLost, callback);
        }

        public void CallCombatCheckExtraAttack()
        {
            OnCombatCheckExtraAttack?.Invoke(this);
        }

        public void CallCombatActivation(Action callback)
        {
            //Messages.ShowInfo("Ship is activated! " + this.ShipId);

            OnCombatActivation?.Invoke(this);
            OnCombatActivationGlobal?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnCombatActivation, callback);
        }

        public void CallCombatDeactivation(Action callback)
        {
            //Messages.ShowInfo("Ship is deactivated! " + this.ShipId);

            OnCombatDeactivation?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnCombatDeactivation, callback);
        }

        // DICE

        public int GetNumberOfAttackDice()
        {
            int result = Combat.ChosenWeapon.WeaponInfo.AttackValue;

            AfterGotNumberOfAttackDice?.Invoke(ref result);
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                AfterGotNumberOfPrimaryWeaponAttackDice?.Invoke(ref result);
            }

            AfterGotNumberOfAttackDiceCap?.Invoke(ref result);

            if (result < 0) result = 0;
            return result;
        }

        public int GetNumberOfDefenceDice(GenericShip attackerShip)
        {
            int result = State.Agility;

            AfterGotNumberOfDefenceDice?.Invoke(ref result);
            AfterGotNumberOfDefenceDiceGlobal?.Invoke(ref result);

            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                AfterGotNumberOfPrimaryWeaponDefenceDice?.Invoke(ref result);
            }

            AfterGotNumberOfDefenceDiceCap?.Invoke(ref result);

            if (result < 0) result = 0;

            int temporary = result;
            CallAfterNumberOfDefenceDiceConfirmed(temporary);

            return result;
        }

        public void CallAfterNumberOfDefenceDiceConfirmed(int numDefenceDice)
        {
            AfterNumberOfDefenceDiceConfirmed?.Invoke(ref numDefenceDice);
        }

        public bool TryDiceResultModification(Die die, GenericAbility.DiceModificationType modType, DieSide newResult, ref bool isAllowed)
        {
            OnTryDiceResultModification?.Invoke(die, modType, newResult, ref isAllowed);

            return isAllowed;
        }

        public bool TrySelectDie(Die die, ref bool isAllowed)
        {
            OnTrySelectDie?.Invoke(die, ref isAllowed);
            return isAllowed;
        }

        // REGEN

        public bool TryRegenShields()
        {
            bool result = false;

            if (State.ShieldsCurrent < State.ShieldsMax)
            {
                result = true;
                State.ShieldsCurrent++;
                AnimateShields();
                AfterAssignedDamageIsChanged(this);
            }

            return result;
        }

        // DAMAGE

        public void SufferDamage(object sender, EventArgs e)
        {
            if (DebugManager.DebugDamage) Debug.Log("+++ Source: " + (e as DamageSourceEventArgs).Source);
            if (DebugManager.DebugDamage) Debug.Log("+++ DamageType: " + (e as DamageSourceEventArgs).DamageType);

            bool isCritical = (AssignedDamageDiceroll.Successes > 0 && AssignedDamageDiceroll.RegularSuccesses == 0);

            OnSufferDamageDecidingSeverity?.Invoke(sender, e, ref isCritical);

            if (isCritical)
            {
                bool skipSufferDamage = false;
                OnSufferCriticalDamage?.Invoke(sender, e, ref skipSufferDamage);

                if (!skipSufferDamage)
                {
                    SufferDamageByType(sender, e, true);
                }
            }
            else
            {
                SufferDamageByType(sender, e, false);
            }
        }

        private void SufferDamageByType(object sender, EventArgs e, bool isCritical)
        {
            if (State.ShieldsCurrent > 0)
            {
                SufferShieldDamage(isCritical);
            }
            else
            {
                SufferHullDamage(CheckFaceupCrit(isCritical), e, Triggers.FinishTrigger);
            }
        }

        public void SufferHullDamage(bool isFaceup, EventArgs e, Action callback)
        {
            OnSufferHullDamageGlobal?.Invoke(this, ref isFaceup, e);
            if (DebugManager.DebugAllDamageIsCrits) isFaceup = true;

            DamageDecks.DrawDamageCard(Owner.PlayerNo, isFaceup, ProcessDrawnDamageCard, e, callback);
        }

        public void ProcessDrawnDamageCard(EventArgs e, Action callback)
        {
            AssignedDamageDiceroll.CancelHitsSpecial(1);
            AssignedDamageDiceroll.RemoveAllFailures();

            OnDamageCardSeverityIsChecked?.Invoke(this);
            OnDamageCardSeverityIsCheckedGlobal?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnDamageCardSeverityIsChecked, delegate { ProcessDrawnDamageCardContinue(e, callback); });
        }

        private void ProcessDrawnDamageCardContinue(EventArgs e, Action callback)
        {
            if (Combat.CurrentCriticalHitCard.IsFaceup)
            {
                OnFaceupCritCardReadyToBeDealt?.Invoke(this, Combat.CurrentCriticalHitCard);
                OnFaceupCritCardReadyToBeDealtGlobal?.Invoke(this, Combat.CurrentCriticalHitCard, e);

                Triggers.RegisterTrigger(new Trigger
                {
                    Name = "Information about faceup damage card",
                    TriggerOwner = this.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFaceupCritCardRevealed,
                    EventHandler = InformCrit.LoadAndShow
                });

                Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardReadyToBeDealt, delegate { SufferFaceupDamageCard(e, callback); });
            }
            else
            {
                CallOnDamageCardIsDealt(delegate { Damage.DealDrawnCard(callback); });
            }
        }

        private void SufferFaceupDamageCard(EventArgs e, Action callback)
        {
            OnFaceupCritCardRevealed?.Invoke(this, Combat.CurrentCriticalHitCard);
            OnFaceupCritCardRevealedGlobal?.Invoke(this, Combat.CurrentCriticalHitCard, e);

            Triggers.ResolveTriggers(
                TriggerTypes.OnFaceupCritCardRevealed,
                delegate { SufferFaceupDamageCardPart2(callback); }
            );
        }

        private void SufferFaceupDamageCardPart2(Action callback)
        {
            OnAssignCrit?.Invoke(this, Combat.CurrentCriticalHitCard);

            if (Combat.CurrentCriticalHitCard != null)
            {
                if (this == Combat.Defender) Combat.DamageInfo.IsDefenderDealtFaceUpDamageCard = true;
                CallOnDamageCardIsDealt(delegate { Damage.DealDrawnCard(callback); });
            }
            else
            {
                callback();
            }
        }

        public void CallHullValueIsDecreased(Action callBack)
        {
            CallAfterAssignedDamageIsChanged();

            CallOnDamageWasSuccessfullyDealt(
                Combat.CurrentCriticalHitCard.IsFaceup,
                delegate { IsHullDestroyedCheck(callBack); }
            );
        }

        public void CallAfterAssignedDamageIsChanged()
        {
            AfterAssignedDamageIsChanged?.Invoke(this);
        }

        private bool CheckFaceupCrit(bool result)
        {
            OnCheckFaceupCrit?.Invoke(ref result);
            return result;
        }

        public void SufferShieldDamage(bool isCritical)
        {
            AssignedDamageDiceroll.CancelHitsSpecial(1);
            AssignedDamageDiceroll.RemoveAllFailures();

            State.ShieldsCurrent--;
            CallAfterAssignedDamageIsChanged();

            CallOnShieldIsLost(
                delegate { CallOnDamageWasSuccessfullyDealt(isCritical, Triggers.FinishTrigger); }
            );
        }

        private void CallOnDamageWasSuccessfullyDealt(bool isCritical, Action callback)
        {
            OnDamageWasSuccessfullyDealt?.Invoke(this, isCritical);

            Triggers.ResolveTriggers(TriggerTypes.OnDamageWasSuccessfullyDealt, callback);
        }

        public void LoseShield()
        {
            State.ShieldsCurrent--;
            CallAfterAssignedDamageIsChanged();
        }

        public virtual void IsHullDestroyedCheck(Action callBack)
        {
            if (State.HullCurrent == 0 && !IsDestroyed)
            {
                CallBeforeCheckPreventDestruction(delegate { CallCheckPreventDestruction(callBack); });
            }
            else
            {
                callBack();
            }
        }

        private void CallBeforeCheckPreventDestruction(Action callBack)
        {
            OnBeforeCheckPreventDestruction?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipIsDestroyedCheck, callBack);
        }

        private void CallCheckPreventDestruction(Action callBack)
        {
            bool preventDestruction = false;

            OnCheckPreventDestruction?.Invoke(this, ref preventDestruction);

            OnCheckPreventDestructionGlobal?.Invoke(this, ref preventDestruction);

            if (!preventDestruction)
            {
                IsDestroyed = true;

                PlayDestroyedAnimSound(
                    delegate
                    {
                        CallShipDestruction(
                     delegate { PlanShipRemoval(callBack); },
                     isFled: false);
                    }
                );
            }
            else
            {
                callBack();
            }
        }

        private void CallShipDestruction(Action callback, bool isFled)
        {
            OnShipIsDestroyed?.Invoke(this, isFled);
            OnShipIsDestroyedGlobal?.Invoke(this, isFled);

            Triggers.ResolveTriggers(TriggerTypes.OnShipIsDestroyed, callback);
        }

        private void PlanShipRemoval(Action callback)
        {
            if (IsDestructionDuringCombat())
            {
                Phases.Events.OnEngagementInitiativeChanged += RegisterShipRemovalSimultaneous;
                callback();
            }
            else
            {
                RemoveDestroyedShip(callback);
            }
        }

        private bool IsDestructionDuringCombat()
        {
            return (Phases.CurrentPhase is MainPhases.CombatPhase);
        }

        public void DestroyShipForced(Action callback, bool isFled = false)
        {
            IsDestroyed = true;

            PlayDestroyedAnimSound(
                delegate
                {
                    CallShipDestruction(
                        delegate { RemoveDestroyedShip(callback); },
                        isFled: isFled
                    );
                }
            );
        }

        private void RegisterShipRemovalSimultaneous()
        {
            Phases.Events.OnEngagementInitiativeChanged -= RegisterShipRemovalSimultaneous;

            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Destruction of ship #" + this.ShipId,
                TriggerType = TriggerTypes.OnEngagementInitiativeChanged,
                TriggerOwner = this.Owner.PlayerNo,
                EventHandler = delegate { RemoveDestroyedShip(Triggers.FinishTrigger); }
            });
        }

        private void RemoveDestroyedShip(Action callback)
        {
            OnShipIsReadyToBeRemoved?.Invoke(this);
            OnShipIsReadyToBeRemovedGlobal?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipIsReadyToBeRemoved, delegate { RemoveDestroyedShip_System(callback); });
        }

        private void RemoveDestroyedShip_System(Action callback)
        {
            OnShipIsRemoved_System?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipIsRemoved, callback);
        }

        public void DeactivateAllAbilities()
        {
            foreach (GenericAbility shipAbility in ShipAbilities)
            {
                shipAbility.DeactivateAbility();
            }

            foreach (GenericAbility pilotAbility in PilotAbilities)
            {
                pilotAbility.DeactivateAbility();
            }

            foreach (GenericUpgrade upgrade in UpgradeBar.GetUpgradesOnlyFaceup())
            {
                foreach (GenericAbility upgradeAbility in upgrade.UpgradeAbilities)
                {
                    upgradeAbility.DeactivateAbility();
                }
            }
        }

        public int GetReloadChargesCount(GenericUpgrade upgrade)
        {
            int count = 1;
            OnGetReloadChargesCount?.Invoke(upgrade, ref count);
            return count;
        }

        public List<ManeuverTemplate> GetAvailableBombDropTemplates(GenericUpgrade upgrade)
        {
            List<ManeuverTemplate> availableTemplates = new();
            availableTemplates.AddRange(upgrade.GetDefaultDropTemplates());

            OnGetAvailableBombDropTemplatesNoConditions?.Invoke(availableTemplates, upgrade);
            OnGetAvailableBombDropTemplatesTwoConditions?.Invoke(availableTemplates, upgrade);
            OnGetAvailableBombDropTemplatesOneCondition?.Invoke(availableTemplates, upgrade);

            OnGetAvailableBombDropTemplatesForbid?.Invoke(availableTemplates, upgrade);

            return availableTemplates;
        }

        public List<ManeuverTemplate> GetAvailableDeviceLaunchTemplates(GenericUpgrade upgrade)
        {
            List<ManeuverTemplate> availableTemplates = new();
            availableTemplates.AddRange(upgrade.GetDefaultLaunchTemplates());

            OnGetAvailableBombLaunchTemplates?.Invoke(availableTemplates, upgrade);

            OnGetAvailableBombLaunchTemplatesModifications?.Invoke(availableTemplates, upgrade);

            return availableTemplates;
        }

        public void CallOnGetBombTemplateDirection(ref Direction direction)
        {
            OnGetBombTemplateDirection?.Invoke(ref direction);
        }

        public List<ManeuverTemplate> GetAvailableBarrelRollTemplates(GenericAction action)
        {
            List<ManeuverTemplate> availableTemplates = new(ShipBase.BarrelRollTemplatesAvailable);

            OnGetAvailableBarrelRollTemplates?.Invoke(availableTemplates, action);

            return availableTemplates;
        }

        public List<ManeuverTemplate> GetAvailableDecloakBarrelRollTemplates()
        {
            List<ManeuverTemplate> availableTemplates = new(ShipBase.DecloakBarrelRollTemplatesAvailable);

            OnGetAvailableDecloakBarrelRollTemplates?.Invoke(availableTemplates);

            return availableTemplates;
        }

        public List<ManeuverTemplate> GetAvailableDecloakBoostTemplates()
        {
            List<ManeuverTemplate> availableTemplates = new List<ManeuverTemplate>(ShipBase.DecloakBoostTemplatesAvailable);

            OnGetAvailableDecloakBoostTemplates?.Invoke(availableTemplates);

            return availableTemplates;
        }

        public List<BoostMove> GetAvailableBoostTemplates(GenericAction action)
        {
            List<BoostMove> availableMoves = new()
            {
                new BoostMove(ActionsHolder.BoostTemplates.Straight1),
                new BoostMove(ActionsHolder.BoostTemplates.LeftBank1),
                new BoostMove(ActionsHolder.BoostTemplates.RightBank1),
            };

            OnGetAvailableBoostTemplates?.Invoke(availableMoves, action);

            return availableMoves;
        }

        public List<ManeuverHolder> GetAvailableSlamTemplates(GenericAction action)
        {
            List<ManeuverHolder> availableTemplates = new();
            foreach (ManeuverHolder template in Maneuvers.Keys.Select(a => new ManeuverHolder(a)).Where(a => a.Speed == AssignedManeuver.ManeuverSpeed))
            {
                availableTemplates.Add(template);
            }

            OnGetAvailableSlamTemplates?.Invoke(ref availableTemplates, action);

            return availableTemplates;
        }

        public void CallUpdateChosenBoostTemplate(ref string boosterTemplateName)
        {
            OnUpdateChosenBoostTemplate?.Invoke(ref boosterTemplateName);
        }

        public void CallUpdateChosenBarrelRollTemplate(ref ManeuverTemplate barrelRollTemplate)
        {
            OnUpdateChosenBarrelRollTemplate?.Invoke(ref barrelRollTemplate);
        }

        public void CallUpdateChosenSlamTemplate(GenericMovement movement)
        {
            OnUpdateChosenSlamTemplate?.Invoke(movement);
        }

        public bool AreWeaponsDisabled()
        {
            bool result = Tokens.HasToken(typeof(WeaponsDisabledToken));

            if (result == true) OnWeaponsDisabledCheck?.Invoke(ref result);

            return result;
        }

        public bool CanAttackBumpedTarget(GenericShip defender)
        {
            bool result = CanAttackBumpedTargetAlways;

            OnCanAttackBumpedTarget?.Invoke(ref result, this, defender);

            OnCanAttackBumpedTargetGlobal?.Invoke(ref result, this, defender);

            return result;
        }

        public bool CanAttackWhileLandedOnObstacle()
        {
            bool result = false;

            OnCanAttackWhileLandedOnObstacle?.Invoke(this, ref result);

            OnCanAttackWhileLandedOnObstacleGlobal?.Invoke(this, ref result);

            return result;
        }

        public List<IShipWeapon> GetAllWeapons()
        {
            List<IShipWeapon> allWeapons = new();

            foreach (PrimaryWeaponClass primaryWeapon in PrimaryWeapons)
            {
                allWeapons.Add(primaryWeapon as IShipWeapon);
            }

            foreach (GenericUpgrade upgrade in UpgradeBar.GetSpecialWeaponsActive())
            {
                allWeapons.Add(upgrade as IShipWeapon);
            }

            return allWeapons;
        }

        public void CallCheckSufferBombDetonation(Action callback)
        {
            IgnoresBombDetonationEffect = false;

            OnCheckSufferBombDetonation?.Invoke(this);
            Triggers.ResolveTriggers(TriggerTypes.OnCheckSufferBombDetonation, callback);
        }

        public bool CallTryConfirmDiceResults()
        {
            bool result = true;

            OnTryConfirmDiceResults?.Invoke(ref result);

            return result;
        }

        public void CallCombatCompareResults()
        {
            OnCombatCompareResults?.Invoke(this);
        }

        public void CallAfterNeutralizeResultsAttacker(Action callback)
        {
            OnAfterNeutralizeResultsAttacker?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.OnAfterNeutralizeResultsAttacker, callback);
        }

        public void CallAfterNeutralizeResults(Action callback)
        {
            OnAfterNeutralizeResults?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.OnAfterNeutralizeResults, callback);
        }

        public void StartBonusAttack(Action callback, Func<GenericShip, IShipWeapon, bool, bool> bonusAttackFilter = null)
        {
            if (IsCannotAttackSecondTime)
            {
                // We should never reach this but just in case.
                Messages.ShowError(PilotInfo.PilotName + ": You have already performed a bonus attack!");
                return;
            }

            IsCannotAttackSecondTime = true;

            Combat.StartSelectAttackTarget(
                this,
                delegate
                {
                    //if bonus attack was skipped, allow bonus attacks again
                    if (IsAttackSkipped) IsCannotAttackSecondTime = false;
                    callback();
                },
                bonusAttackFilter,
                PilotInfo.PilotName,
                "You may perform a bonus attack",
                this
            );
        }

        public void CallBeforeDeviceWillBeDropped(Action callback)
        {
            BeforeBombWillBeDropped?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.BeforeBombWillBeDropped, callback);
        }

        public void CallDeviceWillBeDropped(Action callback)
        {
            OnBombWillBeDropped?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.OnBombWillBeDropped, callback);
        }

        public void CallDeviceWasDropped(Action callback)
        {
            if (Bombs.BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Bomb || Bombs.BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Mine)
            {
                OnBombWasDropped?.Invoke();

                Triggers.ResolveTriggers(
                    TriggerTypes.OnBombWasDropped,
                    delegate { CallCheckDropOfSecondDevice(callback); }
                );
            }
            else if (Bombs.BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
            {
                OnRemoteWasDropped?.Invoke();
                OnRemoteWasDroppedGlobal?.Invoke();

                Triggers.ResolveTriggers(TriggerTypes.OnRemoteWasDropped, callback);
            }
            else
            {
                callback();
            }
        }

        public void CallCheckDropOfSecondDevice(Action callback)
        {
            OnCheckDropOfSecondDevice?.Invoke();

            Triggers.ResolveTriggers(TriggerTypes.OnCheckDropOfSecondDevice, callback);
        }

        public void CallBombWasLaunched(Action callback)
        {
            if (Bombs.BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Bomb || Bombs.BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Mine)
            {
                OnBombWasLaunched?.Invoke();

                Triggers.ResolveTriggers(TriggerTypes.OnBombWasLaunched, callback);
            }
            else if (Bombs.BombsManager.CurrentDevice.UpgradeInfo.SubType == UpgradeSubType.Remote)
            {
                OnRemoteWasLaunched?.Invoke();
                OnRemoteWasLaunchedGlobal?.Invoke();

                Triggers.ResolveTriggers(TriggerTypes.OnRemoteWasLaunched, callback);
            }
            else
            {
                callback();
            }
        }

        public void CallUpdateWeaponRange(IShipWeapon weapon, ref int minRange, ref int maxRange, GenericShip target = null)
        {
            OnUpdateWeaponRange?.Invoke(weapon, ref minRange, ref maxRange, target);

            OnUpdateWeaponRangeGlobal?.Invoke(weapon, ref minRange, ref maxRange, target);
        }

        public void ShowAttackAnimationAndSound()
        {
            if (Combat.ChosenWeapon is not GenericSpecialWeapon chosenSecondaryWeapon || chosenSecondaryWeapon.HasType(UpgradeType.Cannon) || chosenSecondaryWeapon.HasType(UpgradeType.Illicit))
            { // Primary Weapons, Cannons, and Illicits (HotShotBlaster)
                Sounds.PlayShots(SoundInfo.ShotsName, SoundInfo.ShotsCount);
                AnimatePrimaryWeapon();
            }
            else if (chosenSecondaryWeapon.HasType(UpgradeType.Torpedo) || chosenSecondaryWeapon.HasType(UpgradeType.Missile))
            { // Torpedos and Missiles
                Sounds.PlayShots("Proton-Torpedoes", 1);
                AnimateMunitionsShot();
            }
            else if (chosenSecondaryWeapon.HasType(UpgradeType.Turret))
            { // Turrets
                Sounds.PlayShots(SoundInfo.ShotsName, SoundInfo.ShotsCount);
                AnimateTurretWeapon();
            }
        }

        public void CallAfterSufferBombEffect(GenericBomb bomb, Action callback)
        {
            OnAfterSufferBombEffect?.Invoke(this, bomb);

            Triggers.ResolveTriggers(TriggerTypes.OnAfterSufferBombEffect, callback);
        }

        public void CallShotObstructedByMe(GenericShip attacker, ref int count)
        {
            OnShotObstructedByMe?.Invoke(attacker, ref count);
        }

        public void CallTargetForAttackIsAllowed(GenericShip target, ref bool isAllowed)
        {
            OnTargetForAttackIsAllowed?.Invoke(target, ref isAllowed);
        }

        public bool CheckIsForbiddenWeapon(GenericShip ship, IShipWeapon weapon)
        {
            bool isForbidden = false;
            OnCheckIsForbiddenWeapon?.Invoke(ship, weapon, ref isForbidden);
            return isForbidden;
        }
    }
}