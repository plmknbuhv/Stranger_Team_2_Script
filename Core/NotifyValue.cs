using System;
using UnityEngine;

namespace _01_Work.HS.Core
{
    [Serializable]
    public class NotifyValue<T> // 제네릭 클래스
    {
        public delegate void ValueChanged(T prev, T next); // 이전 값과 새 값을 매개변수로 받는 델리게이트 선언
        public event ValueChanged OnValueChanged; // 값이 변경 이벤트

        [SerializeField] private T _value; // 값 변수 선언

        public T Value // 값 파라미터 선언
        {
            get
            {   
                return _value;
            }
            set
            {
                T before = _value; // 값을 이전 값 변수에 대입
                _value = value; // 받아온 값을 값 변수에 대입
                if ( (before == null && value != null) || !before.Equals(_value)) // Null 확인 & 값 변경 체크
                {
                    OnValueChanged?.Invoke(before, _value); // 값 변경 이벤트 실행
                }
            }
        }
    
        public NotifyValue()
        {
            _value = default(T); // 값 초기화
        }

        public NotifyValue(T value)
        {
            _value = value; // 초기 값 설정
        }
    }
}
