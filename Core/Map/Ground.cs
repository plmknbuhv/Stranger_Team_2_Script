using UnityEngine;

namespace _01_Work.HS.Core.Map
{
    public class Ground : MonoBehaviour
    {
        public IPlaceable PlaceObject { get; private set; }
        public GroundType GroundType { get; private set; }

        public bool IsCanPlace => PlaceObject is null;

        public void Initialize(GroundType groundType)
        {
            GroundType = groundType;
        }

        public void SetPlaceObject(IPlaceable placeObj)
        {
            PlaceObject = placeObj;
        }
    }
}