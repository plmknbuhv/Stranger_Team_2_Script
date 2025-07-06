using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace _01_Work.HS.Core.Map.Editor
{
    [CustomEditor(typeof(MapEditor))]
    public class CustomMapEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset visualTreeAsset = default;
        
        private MapEditor _mapEditor;

        private MapDataSO _mapDataSO;
        
        private Button _editButton;
        private Button _deleteButton;

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();
            visualTreeAsset.CloneTree(root);
            
            _mapEditor = target as MapEditor;
            
            SetUpSO();
            InitializeButton(root);
            
            return root;
        }

        private void SetUpSO()
        {
            _mapDataSO = AssetDatabase.LoadAssetAtPath<MapDataSO>("Assets/08_SO/Map/MapDataSO.asset");
            if (!_mapDataSO)
            {
                _mapDataSO = CreateInstance<MapDataSO>();
                AssetDatabase.CreateAsset(_mapDataSO, "Assets/08_SO/Map/MapDataSO.asset");
                AssetDatabase.SaveAssets();
            }
            
            EditorUtility.SetDirty(_mapDataSO);
            AssetDatabase.SaveAssets();
        }

        private void InitializeButton(VisualElement root)
        {
            _editButton = root.Q<Button>("EditButton");
            _deleteButton = root.Q<Button>("DeleteButton");

            _editButton.clicked += HandleEditButtonEvent;
            _deleteButton.clicked += HandleDeleteButtonEvent;
        }

        private void HandleDeleteButtonEvent()
        {
            _mapEditor.ClearMap();
        }

        private void HandleEditButtonEvent()
        {
            bool isTotalHundred = (_mapEditor.grassProbability + _mapEditor.barrenGrassProbability + _mapEditor.fertileGrassProbability) == 100;
            bool isNotZeroSize = _mapEditor.horizontalSize * _mapEditor.verticalSize != 0;
            if (isTotalHundred == false)
            {
                EditorUtility.DisplayDialog("Error", "땅 생성 확률의 합이 100이 아닙니다.", "확인");
                return;
            }
            if (isNotZeroSize == false)
            {
                EditorUtility.DisplayDialog("Error", "가로 혹은 세로의 사이즈가 0입니다.", "확인");
                return;
            }

            _mapEditor.CreateMap(true);
        }
    }
}