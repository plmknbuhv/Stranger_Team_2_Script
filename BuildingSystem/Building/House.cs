using _01_Work.HS.Building;
using _01_Work.HS.Building.BuildingSO;
using _01_Work.KWJ._01_Scripes.WorkingUnit;

namespace _01_Work.HS.BuildingSystem.Building
{
    public class House : BuildObject
    {
        private HouseDataSO _houseData;
        
        public override void Build()
        {
            base.Build();
            _houseData = BuildingDataSO as HouseDataSO;
            
            WorkingUnitManager.Instance.AddMaxWorkingUnit(_houseData.livingPeopleCount, transform);
            SmallAlarmChat.Instance.AddChatMessage(
                $"왕국에 <color=#00ffffff>{_houseData.livingPeopleCount}명의 시민들</color>이 생겼습니다.");
        }       
    }
}