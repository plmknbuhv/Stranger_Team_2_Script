using System.Collections.Generic;
using _01_Work.HS.Core.GameManagement;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace _01_Work.HS.Core.Map
{
    public class MapEditor : MonoBehaviour
    {
        #region Map Setting
        public int fertileGrassProbability = 30;
        public int grassProbability = 40;
        public int barrenGrassProbability = 30; // s
        
        public int horizontalSize = 10;
        public int verticalSize = 10;
        
        public float cellSize = 0.4f;
        public float randomScale = 0.1f;
        #endregion
        
        public Ground fertileGrassPrefab;
        public Ground grassPrefab;
        public Ground barrenGrassPrefab;    
        
        private List<GameObject> _groundObjects = new List<GameObject>();
        
        private NavMeshSurface _navMeshSurface;
        private Grid _mapGrid;
        private MeshFilter _parentMeshFilter;
        
        // private MapDataSO _mapData;

        private void Awake()
        {
            GameManager.Instance.OnGameStartEvent.AddListener(() => CreateMap(false));
            _parentMeshFilter = GetComponent<MeshFilter>();
            _parentMeshFilter.mesh = new Mesh();
        }

        #region Map Create Region
        public void CreateMap(bool isEdit)
        {
            if (_mapGrid == null)
                _mapGrid = GetComponent<Grid>();
            if (_navMeshSurface == null)
                _navMeshSurface = GetComponent<NavMeshSurface>();
            _mapGrid.cellSize = new Vector3(cellSize, 0, cellSize);
            
            ClearMap();

            float seed = Random.Range(0, 1000) * 100f;
            
            for (int hor = 0; hor < horizontalSize; hor++)
            {
                for (int ver = 0; ver < verticalSize; ver++)
                {
                    CreateGround(seed, hor, ver);
                }
            }
            
            if (isEdit) 
                StaticBatchingUtility.Combine(_groundObjects.ToArray(), transform.gameObject);
            else
                CombineMesh();
            
            GameManager.Instance.InitGrounds();
            _navMeshSurface.BuildNavMesh();
        }

        private void CreateGround(float seed, int hor, int ver)
        {
            float randomNoise = Mathf.PerlinNoise(
                seed + (hor * randomScale),
                seed + (ver * randomScale)) * 75f;
                    
            randomNoise += Mathf.PerlinNoise(
                seed + (hor * randomScale * 2f),
                seed + (ver * randomScale * 2f)) * 25f;
                    
            if (randomNoise < fertileGrassProbability)
                CreateGroundBlock(hor, ver, GroundType.FertileGround);
            else if (randomNoise < grassProbability + fertileGrassProbability)
                CreateGroundBlock(hor, ver, GroundType.NormalGround);
            else 
                CreateGroundBlock(hor, ver, GroundType.BarrenGround);
        }

        private void CombineMesh()
        {
            MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>(false);
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            
            int vertexCnt = 0;
            for (int i = 0; i < meshFilters.Length; i++)
            {
                if (meshFilters[i].sharedMesh == null) continue;
                
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix; 
                // 월드 좌표를 기반으로 transform을 다시 만들어 저장
                
                vertexCnt += meshFilters[i].sharedMesh.vertexCount;
                // sharedMesh는 걍 공유 메쉬 가지고 오는겨
            }
            
            _parentMeshFilter.mesh = new Mesh(); // 새로운 매시를 만들어준다.
            if (vertexCnt > 65535) // 정점의 개수가 이게 넘지 않으면 Short자료형으로 충분하다.
                _parentMeshFilter.mesh.indexFormat = IndexFormat.UInt32;
            
            _parentMeshFilter.mesh.CombineMeshes(combine); // 매시 정보하고 트랜스폼 정보를 받아서 합치는 함수

            foreach (var meshFilter in meshFilters)
                meshFilter.GetComponent<MeshRenderer>().enabled = false;
            
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        
        private void CreateGroundBlock(int hor, int ver, GroundType groundType)
        {
            Ground newGround;
            
            switch (groundType)
            {
                case GroundType.NormalGround:
                    newGround = Instantiate(grassPrefab, transform);
                    break;
                case GroundType.FertileGround:
                    newGround = Instantiate(fertileGrassPrefab, transform);
                    break;
                case GroundType.BarrenGround:
                    newGround = Instantiate(barrenGrassPrefab, transform);
                    break;
                default:
                    newGround = Instantiate(grassPrefab, transform);
                    break;
            }
            
            newGround.transform.position = new Vector3(hor * cellSize + 0.2f, 0, ver * cellSize + 0.2f);
            
            _groundObjects.Add(newGround.gameObject);
            newGround.Initialize(groundType);
            
            // Vector3Int cellPoint = _mapGrid.WorldToCell(newGround.transform.position);
            // GroundData ground;
            // ground.groundType = groundType;
            // _mapData.groundDataList.Add(cellPoint, ground);
            // EditorUtility.SetDirty(_mapData);
        }

        public void ClearMap()
        {
            foreach (Transform childTrm in transform)
                _groundObjects.Add(childTrm.gameObject);
            
            foreach (var ground in _groundObjects)
            {
                DestroyImmediate(ground);
            }
            _groundObjects.Clear();
        }
        #endregion
    }
}