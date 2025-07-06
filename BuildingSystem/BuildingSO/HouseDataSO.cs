using UnityEngine;

namespace _01_Work.HS.Building.BuildingSO
{
    [CreateAssetMenu(fileName = "HouseDataSO", menuName = "SO/Build/HouseDataSO", order = 0)]
    public class HouseDataSO : BuildingDataSO
    {
        [Header("House Stat")]
        public int livingPeopleCount; // 거주 인구
    }
}