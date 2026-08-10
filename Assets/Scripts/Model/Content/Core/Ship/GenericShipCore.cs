using Abilities;
using Arcs;
using Content;
using Players;
using System;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{

    public interface IModifyPilotSkill
    {
        void ModifyPilotSkill(ref int pilotSkill);
    }

    public partial class GenericShip : IImageHolder, IBoardObject
    {
        public ShipCardInfo ShipInfo;
        public PilotCardInfo PilotInfo;
        public ShipDialInfo DialInfo;
        public ShipModelInfo ModelInfo;

        public CustomizedAi Ai;

        public Faction Faction { get { return (PilotInfo.Faction != Faction.None) ? PilotInfo.Faction : ShipInfo.DefaultShipFaction; } }

        public Faction SubFaction
        {
            get
            {
                if (ShipInfo.SubFaction != Faction.None)
                {
                    return ShipInfo.SubFaction;
                }
                else
                {
                    return Faction;
                }
            }
        }

        public ShipStateInfo State;

        public int ShipId { get; protected set; }
        public GenericPlayer Owner { get; protected set; }

        public string PilotName { get; set; }

        public int TargetLockMinRange { get; protected set; }
        public int TargetLockMaxRange { get; protected set; }

        public void CallAfterGetMaxHull(ref int result)
        {
            AfterGetMaxHull?.Invoke(ref result);
        }

        public GameObject Model { get; protected set; }
        public GameObject InfoPanel { get; protected set; }

        public GenericShipBase ShipBase { get; protected set; }

        public ArcsHolder ArcsInfo { get; protected set; }
        public SectorsHolder SectorsInfo { get; set; }

        public ShipUpgradeBar UpgradeBar { get; protected set; }
        public ShipActionBar ActionBar { get; protected set; }
        public List<Type> DefaultUpgrades { get; protected set; }
        public List<Type> MustHaveUpgrades { get; protected set; }

        public TokensManager Tokens { get; protected set; }

        private string pilotNameCanonical;
        public string PilotNameCanonical
        {
            get
            {
                if (!string.IsNullOrEmpty(pilotNameCanonical)) return pilotNameCanonical;

                return Tools.Canonicalize(PilotInfo.PilotName);
            }
            set { pilotNameCanonical = value; }
        }

        public string ShipTypeCanonical
        {
            get { return Tools.Canonicalize(ShipInfo.ShipName); }
        }

        public List<GenericAbility> PilotAbilities = new();
        public List<GenericAbility> ShipAbilities = new();

        public GenericShip()
        {
            IconicPilots = new();
            RequiredMods = new();
            Maneuvers = new();
            UpgradeBar = new(this);
            Tokens = new(this);
            ActionBar = new(this);
            Ai = new(this);
            DefaultUpgrades = new();
            MustHaveUpgrades = new();

            TargetLockMinRange = 0;
            TargetLockMaxRange = 3;
        }

        public void InitializeGenericShip(PlayerNo playerNo, int shipId, Vector3 position)
        {
            Owner = Roster.GetPlayer(playerNo);
            ShipId = shipId;
            StartingPosition = position;

            InitializeShip();
            InitializePilot();
            InitializeUpgrades();

            InitializeState();

            InitializeShipModel();

            InitializeRosterPanel();
        }

        protected void InitializeRosterPanel()
        {
            InfoPanel = Roster.CreateRosterInfo(this);
            Roster.UpdateUpgradesPanel(this, this.InfoPanel);
            Roster.SubscribeSelectionByInfoPanel(this.InfoPanel.transform.Find("ShipInfo").gameObject);
            Roster.SubscribeUpgradesPanel(this, this.InfoPanel);
        }

        public virtual void InitializeUpgrades()
        {
            foreach (UpgradeSlot slot in UpgradeBar.GetUpgradeSlots())
            {
                slot.TryInstallUpgrade(slot.InstalledUpgrade, this);
            }
        }

        public void InitializeState()
        {
            State = new(this)
            {
                Initiative = PilotInfo.Initiative,
                PilotSkillModifiers = new List<IModifyPilotSkill>(),

                Firepower = ShipInfo.Firepower,
                Agility = ShipInfo.Agility,
                HullMax = ShipInfo.Hull,
                ShieldsMax = ShipInfo.Shields
            };
            State.ShieldsCurrent = State.ShieldsMax;

            State.MaxForce = PilotInfo.Force;

            State.MaxCharges = PilotInfo.Charges > 0 ? PilotInfo.Charges : ShipInfo.Charges;
            State.RegensCharges = PilotInfo.RegensCharges + ShipInfo.RegensCharges;

            Maneuvers = new Dictionary<string, Movement.MovementComplexity>();
            if (DialInfo != null)
            {
                foreach (KeyValuePair<Movement.ManeuverHolder, Movement.MovementComplexity> maneuver in DialInfo.PrintedDial)
                {
                    Maneuvers.Add(maneuver.Key.ToString(), maneuver.Value);
                }
            }
        }

        public virtual void InitializeShip()
        {
            InitializePilotForSquadBuilder();

            foreach (ShipArcInfo arcInfo in ShipInfo.ArcInfo.Arcs)
            {
                if (arcInfo.Firepower != -1) PrimaryWeapons.Add(new PrimaryWeaponClass(this, arcInfo));
            }

            Damage = new Damage(this);
            ActionBar.Initialize();
        }

        public void InitializeShipModel()
        {
            CreateModel(StartingPosition);
            InitializeSectors();
            InitializeShipBaseArc();
            SetId();
            SetShipInsertImage();
            SetShipSkin(GetModelTransform(), GetSkinTexture());
        }

        protected void SetId()
        {
            SetTagOfChildrenRecursive(Model.transform, "ShipId:" + ShipId.ToString());

            SetIdMarker();
            SetSpotlightMask();
        }

        public void InitializeSectors()
        {
            SectorsInfo = new SectorsHolder(this);
        }

        public void InitializeShipBaseArc()
        {
            ArcsInfo = new ArcsHolder(this);
            foreach (ShipArcInfo arc in ShipInfo.ArcInfo.Arcs)
            {
                switch (arc.ArcType)
                {
                    case ArcType.Front:
                        ArcsInfo.Arcs.Add(new ArcFront(ShipBase));
                        break;
                    case ArcType.Rear:
                        ArcsInfo.Arcs.Add(new ArcRear(ShipBase));
                        break;
                    case ArcType.FullFront:
                        ArcsInfo.Arcs.Add(new ArcFullFront(ShipBase));
                        break;
                    case ArcType.SingleTurret:
                        ArcsInfo.Arcs.Add(new ArcSingleTurret(ShipBase));
                        break;
                    case ArcType.DoubleTurret:
                        ArcsInfo.Arcs.Add(new ArcDualTurretA(ShipBase));
                        ArcsInfo.Arcs.Add(new ArcDualTurretB(ShipBase));
                        break;
                    case ArcType.Bullseye:
                        ArcsInfo.Arcs.Add(new ArcBullseye(ShipBase));
                        break;
                    case ArcType.TurretPrimaryWeapon:
                        //TODOREVERT
                        // Primary weapon can be used from outside the arc
                        break;
                    case ArcType.SpecialGhost:
                        ArcsInfo.Arcs.Add(new ArcSpecialGhost(ShipBase));
                        break;
                    default:
                        break;
                }
            }
        }

        public void InitializePilotForSquadBuilder()
        {
            InitializeSquadBuilderAbilities();
            InitializeSlots();
        }

        private void InitializeSquadBuilderAbilities()
        {
            foreach (GenericAbility shipAbility in ShipAbilities)
            {
                shipAbility.InitializeForSquadBuilder(this);
            }
        }

        public virtual void InitializePilot()
        {
            PrepareForceInitialization();
            PrepareChargesInitialization();

            ActivateShipAbilities();
            ActivatePilotAbilities();
        }

        private void PrepareForceInitialization()
        {
            OnGameStart += InitializeForce;
        }

        private void InitializeForce()
        {
            OnGameStart -= InitializeForce;
            State.InitializeForceTokens(State.MaxForce);
        }

        private void PrepareChargesInitialization()
        {
            OnGameStart += InitializeCharges;
        }

        private void InitializeCharges()
        {
            OnGameStart -= InitializeCharges;
            SetChargesToMax();
        }

        private void ActivateShipAbilities()
        {
            foreach (GenericAbility shipAbility in ShipAbilities)
            {
                shipAbility.Initialize(this);
            }
        }

        protected void ActivatePilotAbilities()
        {
            if (PilotInfo.AbilityType != null) PilotAbilities.Add((GenericAbility)Activator.CreateInstance(PilotInfo.AbilityType));

            foreach (GenericAbility pilotAbility in PilotAbilities)
            {
                pilotAbility.Initialize(this);
            }
        }

        private void InitializeSlots()
        {
            foreach (UpgradeType slot in ShipInfo.UpgradeIcons.Upgrades)
            {
                UpgradeBar.AddSlot(slot);
            }

            foreach (UpgradeType slot in PilotInfo.ExtraUpgrades)
            {
                UpgradeBar.AddSlot(slot);
            }

            if (DebugManager.FreeMode)
            {
                UpgradeBar.AddSlot(UpgradeType.Omni);
            }
        }

        // STAT MODIFICATIONS

        public void ChangeFirepowerBy(int value)
        {
            if (State != null) State.Firepower += value;
            AfterStatsAreChanged?.Invoke(this);
        }

        public void ChangeAgilityBy(int value)
        {
            if (State != null) State.Agility += value;
            AfterStatsAreChanged?.Invoke(this);
        }

        public void ChangeMaxHullBy(int value)
        {
            if (State != null) State.HullMax += value;
            AfterStatsAreChanged?.Invoke(this);
        }

        public void ChangeShieldBy(int value)
        {
            if (State != null) State.ShieldsCurrent += value;
            AfterStatsAreChanged?.Invoke(this);
        }

        public void SetTargetLockRange(int min, int max)
        {
            TargetLockMinRange = min;
            TargetLockMaxRange = max;
        }

        // CHARGES

        public void SpendCharges(int count)
        {
            for (int i = 0; i < count; i++)
            {
                SpendCharge();
            }
        }

        public void SpendCharge()
        {
            State.Charges--;

            if (State.Charges < 0) throw new InvalidOperationException("Cannot spend charge when you have none left");
        }

        public void LoseCharge()
        {
            State.Charges--;
        }

        public void RemoveCharge(Action callBack)
        {
            // for now this is just an alias of SpendCharge
            SpendCharge();
            callBack();
        }

        public void RestoreCharges(int count)
        {
            if (count > 0 && State.Charges < State.MaxCharges)
            {
                State.Charges = Math.Min(State.Charges + count, State.MaxCharges);
            }
            else if (count < 0 && State.Charges > 0)
            {
                State.Charges = Math.Max(State.Charges + count, 0);
            }
        }

        public void SetChargesToMax()
        {
            State.Charges = State.MaxCharges;
        }

        public bool CanEquipTagRestrictedUpgrade(Tags tag)
        {
            bool result = ((PilotInfo as PilotCardInfo25).Tags.Contains(tag));

            OnUpgradeEquipTagCheck?.Invoke(tag, ref result);

            return result;
        }
    }
}