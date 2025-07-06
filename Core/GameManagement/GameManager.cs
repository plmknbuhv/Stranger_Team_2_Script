using System;
using System.Collections.Generic;
using System.Linq;
using _01_Work.HS.Building.BuildingSO;
using _01_Work.HS.BuildingSystem.Building;
using _01_Work.HS.Core.GameManagement.States;
using _01_Work.HS.Core.Map;
using UnityEngine;
using UnityEngine.Events;

namespace _01_Work.HS.Core.GameManagement
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [field:SerializeField] public InputSO InputSO { get; private set; }
        
        public Dictionary<Vector3Int, Ground> GroundList { get; private set; } = new Dictionary<Vector3Int, Ground>();
        [SerializeField] private Grid grid;
        
        [SerializeField] private StateMachine stateMachine;
        [field:SerializeField] public SelectionSpace SelectionSpaceObj {get; private set;}
        
        public List<FriendlyUnit> _controlUnitList { get; private set; } = new List<FriendlyUnit>();
        public List<BuildingType> onlyOneBuilding = new List<BuildingType>();
        public Castle Castle { get; private set; }
        public NotifyValue<BuildingType> TargetBuildingType { get; private set; } = new NotifyValue<BuildingType>();
        public SelectObject CurrentSelectObject {get; private set;}

        public bool _isGathering;

        public UnityEvent OnGameOverEvent;
        public UnityEvent OnGameStartEvent;
        
        public UnityEvent<int> OnLevelUpEvent;
        public UnityEvent<int> OnChangeHealthEvent;

        public bool IsGathering
        {
            get => _isGathering;
            set
            {
                _isGathering = value;
                SelectionSpaceObj.ActiveTool(_isGathering);
            }
        }
        public event Action<SelectObject> OnChangeSelectObj;
        
        private void Awake()
        {
            stateMachine.Initialize(this);
        }

        public void InitGrounds()
        {
            grid.GetComponentsInChildren<Ground>().ToList().ForEach(ground =>
            {
                Vector3Int groundCellPoint = grid.WorldToCell(ground.transform.position);
                GroundList.Add(groundCellPoint, ground);
            });
        }

        private void Start()
        {
            ChangeState(GameStateType.Start);
            OnGameStartEvent?.Invoke();
        }

        #region Utility
        public Ground GetGroundData(Vector3 position) => GroundList.GetValueOrDefault(grid.WorldToCell(position));

        public Vector3Int GetCellPosition(Vector3 position) => grid.WorldToCell(new Vector3(position.x, 0, position.z));

        public Vector3 GetCenterCellPosition(Vector3Int position) => grid.GetCellCenterWorld(position);
        #endregion

        public void SetBuilding(BuildingType buildingType)
        {
            TargetBuildingType.Value = buildingType;
            if (stateMachine.CurrentState is not BuildState)
                ChangeState(GameStateType.Build);
        }
        
        public void SetUnit(FriendlyUnit unit)
        {
            _controlUnitList.Add(unit);
            ChangeState(GameStateType.UnitControl);
        }

        public void HealCastle(int healValue) => Castle.Heal(healValue);
        public void SetCastle(Castle castle) => Castle = castle;
        public void ChangeState(GameStateType newStateType) => stateMachine.ChangeState(newStateType);
        public void ChangeSelectObj(SelectObject select) => OnChangeSelectObj?.Invoke(select);
        public bool CheckCanBuild(BuildingType buildingType) => !onlyOneBuilding.Contains(buildingType);
    }
}
