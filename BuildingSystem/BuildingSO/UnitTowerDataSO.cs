using _01_Work.KHJ.CombatUnit;
using UnityEngine;

namespace _01_Work.HS.Building.BuildingSO
{
    [CreateAssetMenu(fileName = "UnitTowerDataSO", menuName = "SO/Build/UnitTowerDataSO", order = 0)]
    public class UnitTowerDataSO : BuildingDataSO
    {
        [Header("Tower Stat")]
        public int CombatUnitCount; // 병사 수
        public float FindRange; // 공격 범위
        public float UnitRespawnCoolTime; // 유닛 재생성 시간
        public CombatUnit SpawnUnit; // 유닛 재생성 시간
        public LayerMask WhatIsUnit;
    }
}