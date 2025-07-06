using EPOOutline;
using UnityEngine;

namespace _01_Work.HS.Core.GameManagement
{
    [RequireComponent(typeof(Outlinable))]
    public abstract class SelectObject : MonoBehaviour
    {
        protected Outlinable _outlineCompo;

        protected virtual void Awake()
        {
            _outlineCompo = GetComponent<Outlinable>();
            
            SetUpOutLine();
        }

        private void SetUpOutLine()
        {
            _outlineCompo.OutlineParameters.BlurShift = 0.5f;
            _outlineCompo.OutlineParameters.DilateShift = 1f;
            ChangeColor(Color.white);
            _outlineCompo.enabled = false;
        }

        public void ChangeColor(Color color)
        {
            _outlineCompo.OutlineParameters.Color = new Color(color.r, color.g, color.b, 0.75f);
        }

        public void Focus()
        {
            _outlineCompo.enabled = true;
        }

        public void CancelFocus()
        {
            _outlineCompo.enabled = false;
        }
    }
}