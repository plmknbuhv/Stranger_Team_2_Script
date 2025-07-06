using _01_Work.HS.Building;
using _01_Work.HS.Building.BuildingSO;
using _01_Work.LCM._01.Scripts.Day;
using UnityEngine;

namespace _01_Work.HS.BuildingSystem.Building
{
    public class EconomyBuilding : BuildObject
    {
        private EconomyDataSO _economyData;

        protected override void Awake()
        {
            base.Awake();
        }

        public override void Build()
        {
            base.Build();
            _economyData = BuildingDataSO as EconomyDataSO;
            DayManager.Instance.OnChangeMorning += MakeMoney;
        }
        
        protected virtual void MakeMoney()
        {
            Debug.Log(_economyData.salary);
            if (CheckCanWork()) 
                ResourceManager.Instance.AddResorce(ResourceType.GOLD, _economyData.salary);
        }
    }
}