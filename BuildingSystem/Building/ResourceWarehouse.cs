using _01_Work.HS.Building;
using _01_Work.KWJ._01_Scripes.WorkingUnit;


public class ResourceWarehouse : BuildObject
{
    public override void Build()
    {
        ResourceManager.Instance.AddMaxCount(ResourceType.WOOD, 30);
        ResourceManager.Instance.AddMaxCount(ResourceType.STONE, 30);
        ResourceManager.Instance.AddMaxCount(ResourceType.CRYSTAL, 30);
        
        WorkingUnitManager.Instance.WorkPrioritySetting(this);
        WorkingUnitManager.Instance.SettingTakeResourceList();
    }
}
