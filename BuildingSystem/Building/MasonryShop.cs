using _01_Work.HS.Building;
using _01_Work.HS.Core.GameManagement;
using _01_Work.LCM._01.Scripts.Day;

public class MasonryShop : BuildObject
{
    public override void Build()
    {
        base.Build();
        DayManager.Instance.OnChangeNight += HandleRepairCastle;
    }

    private void HandleRepairCastle()
    {
        GameManager.Instance.HealCastle(50);
    }
}
