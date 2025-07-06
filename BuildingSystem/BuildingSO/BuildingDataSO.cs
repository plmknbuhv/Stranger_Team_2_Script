using UnityEngine;

namespace _01_Work.HS.Building.BuildingSO
{
    public enum BuildingType
    {
        SmallHouse, // 완
        MediumHouse, // 완
        LargeHouse, // 완
        Castle, // 완
        Farm,
        Bar, // 완
        Square, // 완
        MasonryShop, // 완
        FoodWarehouse,
        ResourceWarehouse,
        Church,
        ArcherTower,
        CrystalTower,
        TrainingGround,
		Bank,
    }
    
    public enum BuildingCategory
    {
        Battle,
        Unit,
        Resource,
        Economy,
    }
    
    [CreateAssetMenu(fileName = "BuildingDataSO", menuName = "SO/Build/BuildingDataSO", order = 0)]
    public class BuildingDataSO : ScriptableObject
    {
        [Header("Base Stat")] 
        public BuildObject buildingPrefab;
        public BuildingType buildingType;
        public int width; // 건설 가로 길이
        public int height; // 건설 세로 길이
        public int personNum;
        public bool isCanOnlyOne;
        public Sprite buildingIcon;
        public BuildingCategory buildingCategory;
        public bool isCanAround;
        
        [Header("Resource")]
        public int needGold;
        public int needWood;
        public int needStone;
        public int needCrystal;
        public int needFood;
        
        [TextArea]
        public string description;
        public string buildingName;
    }
}
