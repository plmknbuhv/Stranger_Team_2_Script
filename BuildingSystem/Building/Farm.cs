using System.Collections.Generic;
using _01_Work.HS.Building;
using _01_Work.HS.Building.BuildingSO;
using _01_Work.HS.Core.Map;
using _01_Work.LCM._01.Scripts.Day;
using UnityEngine;

namespace _01_Work.HS.BuildingSystem.Building
{
    public class Farm : BuildObject
    {
        private MeshFilter _meshFilter;
        private bool _isGrown;
        
        [SerializeField] private Mesh beforeGrow;
        [SerializeField] private Mesh afterGrow;

        protected override void Awake()
        {
            base.Awake();
            _meshFilter = GetComponent<MeshFilter>();
        }

        public override void PrepareBuild(BuildingDataSO buildingSO)
        {
            base.PrepareBuild(buildingSO);
            _meshFilter.mesh = beforeGrow;
        }

        public override void Build()
        {
            base.Build();
            DayManager.Instance.OnChangeMorning += HandleHarvestFarm;
            DayManager.Instance.OnChangeNight += HandleGrowFarm;
        }

        public override bool CheckCanPlaceBuilding(List<Ground> grounds)
        {
            foreach (var ground in grounds)
            {
                if (ground.GroundType == GroundType.BarrenGround)
                    return false;
            }

            return true;
        }

        private void HandleGrowFarm()
        {
            if (CheckCanWork() == false) return;
            
            _isGrown = true;
            _meshFilter.mesh = afterGrow;
        }
        
        private void HandleHarvestFarm()
        {
            if (_isGrown)
            {
                _isGrown = false;
                _meshFilter.mesh = beforeGrow;
                
                if (_groundList[0].GroundType == GroundType.NormalGround)
                    ResourceManager.Instance.AddResorce(ResourceType.FOOD, 3);
                else
                    ResourceManager.Instance.AddResorce(ResourceType.FOOD, 5);
            }
        }
    }
}
