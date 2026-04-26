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
        public BuildingDataSO BuildingDataSO { get; private set; }
        public Queue<WorkingUnit> WorkingUnitList = new Queue<WorkingUnit>();  // 현재 건물에 일 하고 있는 유닛 리스트
        protected List<Ground> _groundList = new List<Ground>();  // 여러 칸을 차지하는 건물 때문에 List로 관리
        
        public Vector2Int ActualSize { get; private set; }

        protected bool _isBuilt;
        // 이후에 건물 설치에 시간이 소모되는 기능 추가를 염두하고 만듬
        // 사용 안하는중

        public Action OnCanWorkEvent; // 유닛이 배치되어서 꽉 찰 경우 발행되는 이벤트

        // 건물 이전에 미리보기 시 실해되는 함수 
        public virtual void PrepareBuild(BuildingDataSO buildingSO)
        {
            BuildingDataSO = buildingSO;
            ChangeSize(new Vector2Int(BuildingDataSO.width, BuildingDataSO.height));
            _outlineCompo.enabled = true;
            ChangeColor(Color.cyan);
        }

        // 설치 함수
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
