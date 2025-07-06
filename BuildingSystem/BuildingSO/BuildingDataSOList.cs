using System.Collections.Generic;
using UnityEngine;

namespace _01_Work.HS.Building.BuildingSO
{
    [CreateAssetMenu(fileName = "BuildDataSOList", menuName = "SO/Build/BuildDataList", order = 0)]
    public class BuildingDataSOList : ScriptableObject
    {
        public List<BuildingDataSO> buildDataSOList = new List<BuildingDataSO>();
    }
}