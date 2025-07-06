using UnityEngine;

namespace _01_Work.HS.Building.BuildingSO
{
    [CreateAssetMenu(fileName = "AttackTowerDataSO", menuName = "SO/Build/AttackTowerDataSO", order = 0)]
    public class AttackTowerDataSO : BuildingDataSO
    {
        [Header("Tower Stat")]
        public float FindRange; // 공격 범위
        public float Damage;
        public float CoolTime;
        public LayerMask WhatIsUnit;
    }
}