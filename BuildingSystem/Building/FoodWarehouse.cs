using _01_Work.HS.Building;
using _01_Work.KWJ._01_Scripes.WorkingUnit;

public class FoodWarehouse : BuildObject
{
    public override void Build()
    {
        ResourceManager.Instance.AddMaxCount(ResourceType.FOOD, 15);
        
        WorkingUnitManager.Instance.SettingTakeResourceList();
    }
}