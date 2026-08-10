using Arcs;
using BoardTools;
using Players;
using Ship;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Remote
{
    public abstract class GenericRemote : GenericShip
    {
        public override bool HasCombatActivation { get { return false; } }
        public RemoteInfo RemoteInfo { get; protected set; }
        public abstract Dictionary<string, Vector3> BaseEdges { get; }
        public new BoardObjectType BoardObjectType => BoardObjectType.Remote;

        public GenericRemote(GenericPlayer owner) : base()
        {
            Owner = owner;
        }

        public void SpawnModel(int shipId, Vector3 position, Quaternion rotation)
        {
            ShipId = shipId;

            GeneratePilotInfo();
            GenerateModel(position, rotation);
            GeneratePseudoBase();
            GeneratePseudoShip();
            SetChargesToMax();
            InitializeRosterPanel();

            ActivatePilotAbilities();

            Board.RegisterRemote(this);

            Roster.AddShipToLists(this);

            Roster.UpdateTokensIndicator(this, null);
        }

        private void GeneratePseudoBase()
        {
            ShipBase = new RemoteShipBase(this);
        }

        private void GeneratePilotInfo()
        {
            ShipInfo = new ShipCardInfo(
                "Remote",
                BaseSize.None,
                Faction.None,
                RemoteInfo is RemoteInfo25 ? (RemoteInfo as RemoteInfo25).ArcInfo : new ShipArcsInfo(ArcType.None, 0),
                RemoteInfo.Agility,
                RemoteInfo.Hull,
                0,
                new ShipActionsInfo(),
                new ShipUpgradesInfo()
            );

            PilotInfo = new PilotCardInfo(
                RemoteInfo.Name,
                RemoteInfo.Initiative,
                0,
                abilityType: RemoteInfo.AbilityType,
                charges: RemoteInfo.Charges,
                regensCharges: RemoteInfo.RegensCharges
            );

            ImageUrl = RemoteInfo.ImageUrl;

            SoundInfo = RemoteInfo is RemoteInfo25 ? (RemoteInfo as RemoteInfo25).SoundInfo : new ShipSoundInfo(new(), "TIE-Fire", 2);
        }

        private void GenerateModel(Vector3 position, Quaternion rotation)
        {
            GameObject prefab = Resources.Load<GameObject>($"Prefabs/Remotes/{RemoteInfo.Name}");
            Model = MonoBehaviour.Instantiate(prefab, position, rotation, Board.GetBoard());
            ShipAllParts = Model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts").transform;
            modelCenter = ShipAllParts.Find($"ShipModels/{RemoteInfo.Name}/ModelCenter").transform;

            SetTagOfChildrenRecursive(Model.transform, $"ShipId:{ShipId}");
            SetRaycastTarget(true);
            SetSpotlightMask();
            SetShipIdText(Model);
            SetPlayerCustomization();
        }

        protected virtual void SetPlayerCustomization()
        {
            // Customize to show different view of remote for Player1 and Player2
        }

        public Vector3 GetJointAngles(int jointIndex)
        {
            return ShipAllParts.Find("ShipBase/ManeuverJoints").Find("ManeuverJoint" + jointIndex).Find("Rotation").eulerAngles;
        }

        public Vector3 GetJointPosition(int jointIndex)
        {
            return ShipAllParts.Find("ShipBase/ManeuverJoints").Find("ManeuverJoint" + jointIndex).Find("Rotation").position;
        }

        private void GeneratePseudoShip()
        {
            Damage = new Damage(this);
            ActionBar.Initialize();
            InitializeState();
            InitializeSectors();
            InitializeShipBaseArc();
            InitializePrimaryWeapons();
        }

        private void InitializePrimaryWeapons()
        {
            foreach (ShipArcInfo arcInfo in ShipInfo.ArcInfo.Arcs)
            {
                if (arcInfo.Firepower > 0) PrimaryWeapons.Add(new PrimaryWeaponClass(this, arcInfo));
            }
        }

        public void ToggleJointArrow(int jointIndex, bool isVisible)
        {
            ShipAllParts.Find("ShipBase/ManeuverJoints").Find("ManeuverJoint" + jointIndex).gameObject.SetActive(isVisible);
        }

        public override Vector3 GetCenter()
        {
            return Model.transform.TransformPoint(0, 0, 2f);
        }

        public override Transform GetModelTransform()
        {
            return ShipAllParts.Find($"ShipModels/{RemoteInfo.Name}/ModelCenter");
        }

        public override Vector3 GetModelCenter()
        {
            return GetModelTransform().position;
        }

        public class RemoteShipBase : GenericShipBase
        {
            public RemoteShipBase(GenericShip host) : base(host)
            {
                baseEdges = new Dictionary<string, Vector3>((host as GenericRemote).BaseEdges);

                Size = BaseSize.Small;

                HALF_OF_SHIPSTAND_SIZE = 0.5f;
                SHIPSTAND_SIZE = 1f;
                SHIPSTAND_SIZE_CM = 4f;

                HALF_OF_FIRINGARC_SIZE = 0.425f;
            }

            public override List<ManeuverTemplate> BoostTemplatesAvailable => throw new NotImplementedException();
            public override List<ManeuverTemplate> BarrelRollTemplatesAvailable => throw new NotImplementedException();
            public override List<ManeuverTemplate> DecloakBoostTemplatesAvailable => throw new NotImplementedException();
            public override List<ManeuverTemplate> DecloakBarrelRollTemplatesAvailable => throw new NotImplementedException();
        }
    }
}