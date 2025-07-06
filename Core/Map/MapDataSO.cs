using System;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Work.HS.Core.Map
{
    [CreateAssetMenu(fileName = "MapEditorSO", menuName = "SO/MapEditorSO", order = 0)]
    public class MapDataSO : ScriptableObject
    {
        public Dictionary<Vector3Int , GroundData> groundDataList = new Dictionary<Vector3Int, GroundData>();

        public GroundData GetGroundData(Vector3Int cellPosition)
        {
            return groundDataList.GetValueOrDefault(cellPosition);
        }
    }

    [Serializable]
    public struct GroundData
    {
        public GroundType groundType;
    }

    public enum GroundType
    {
        NormalGround,
        FertileGround, 
        BarrenGround
    }
}