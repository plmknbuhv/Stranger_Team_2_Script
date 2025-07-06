using _01_Work.HS.Building.BuildingSO;
using _01_Work.HS.BuildingSystem.Building;
using _01_Work.HS.Core.Map;
using UnityEngine;

namespace _01_Work.HS.Core.GameManagement.States
{
    public class StartState : BuildState
    {
        public override void EnterState()
        {
            base.EnterState();
            
            PrepareBuilding(BuildingType.Castle);
        }

        protected override void Build(Ground hitGround, RaycastHit hitInfo)
        {
            // _gameManager.OnGameStartEvent.Invoke();
            _gameManager.SetCastle(_building as Castle);
            base.Build(hitGround, hitInfo);
        }

        protected override void HandleChangeBuildingType(BuildingType prev, BuildingType next)
        {
        }
    }
}