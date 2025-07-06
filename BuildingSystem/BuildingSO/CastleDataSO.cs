using System;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Work.HS.Building.BuildingSO
{
    [CreateAssetMenu(fileName = "CastleDataSO", menuName = "SO/Build/CastleDataSO", order = 0)]
    public class CastleDataSO : BuildingDataSO
    {
        public int health;

        public List<CastleUpgradeData> upgradeDataList = new List<CastleUpgradeData>();
        public List<int> salaryByLevel = new List<int>();
        public List<Mesh> castleMeshes;
    }

    [Serializable]
    public class CastleUpgradeData
    {
        public int gold;
        public int wood;
        public int stone;
        public int crystal;
        public int person;
    }
}