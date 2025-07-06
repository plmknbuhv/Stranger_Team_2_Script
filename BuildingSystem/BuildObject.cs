using System;
using System.Collections.Generic;
using System.Linq;
using _01_Work.HS.Building.BuildingSO;
using _01_Work.HS.Core.GameManagement;
using _01_Work.HS.Core.Map;
using _01_Work.KWJ._01_Scripes.WorkingUnit;
using UnityEngine;

namespace _01_Work.HS.Building
{
    public class BuildObject : SelectObject, IPlaceable
    {
        public BuildingDataSO BuildingDataSO { get; private set; } // 이거 나중에 바꿀수도 있음
        public Queue<WorkingUnit> WorkingUnitList = new Queue<WorkingUnit>(); 
        protected List<Ground> _groundList = new List<Ground>();
        
        public Vector2Int ActualSize { get; private set; }

        protected bool _isBuilt;

        public Action OnCanWorkEvent;
        
        public virtual void PrepareBuild(BuildingDataSO buildingSO)
        {
            BuildingDataSO = buildingSO;
            ChangeSize(new Vector2Int(BuildingDataSO.width, BuildingDataSO.height));
            _outlineCompo.enabled = true;
            ChangeColor(Color.cyan);
        }

        public virtual void Build()
        {
            WorkingUnitManager.Instance.WorkPrioritySetting(this);
            ChangeColor(Color.white);
            CancelFocus();
        }

        public void AddPeople(WorkingUnit workingUnit)
        {
            WorkingUnitList.Enqueue(workingUnit);
        }
        public void RemovePeople(WorkingUnit workingUnit) =>  WorkingUnitList.Dequeue();
        public bool CheckCanWork() => WorkingUnitList.Count == BuildingDataSO.personNum;

        public void AddGround(Ground newGround) => _groundList.Add(newGround);
        public void ChangeSize(Vector2Int newSize) => ActualSize = newSize;
        public virtual bool CheckCanPlaceBuilding(List<Ground> ground) => true;
    }
}