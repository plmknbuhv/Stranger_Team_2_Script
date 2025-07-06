using EPOOutline;
using UnityEngine;

[RequireComponent(typeof(Outliner))]
public class OutLinerController : MonoBehaviour
{
    private Outliner _outlinerComp;

    private void Awake()
    {
        _outlinerComp = GetComponent<Outliner>();
    }

    private void Update()
    {
        var offset = (Mathf.Sin(Time.time * 2.1f) / 2f) + 1.2f;
        _outlinerComp.DilateShift = offset;
    }
}
