using System.Collections.Generic;
using UnityEngine;

public class SelectionSpace : MonoBehaviour
{
    [SerializeField] private List<GameObject> horizontalPartList;
    [SerializeField] private List<GameObject> verticalPartList;

    [SerializeField] private float partSize = 0.235f;
    [SerializeField] private float otherPartSize = 0.035f;
    [SerializeField] private float selectQuadSize = 0.3f;

    [SerializeField] private GameObject toolObject;
    [SerializeField] private GameObject selectQuad;

    private bool _isActiveQuad;

    public void SetSize(Vector3 centerPos, int hor = 1, int ver = 1)
    {
        centerPos = new Vector3(centerPos.x, 0.265f, centerPos.z);
        transform.position = centerPos;

        for (var i = 0; i < 2; i++)
        {
            var sign = (i * 2) - 1;
            horizontalPartList[i].transform.localPosition 
                = new Vector3(sign * 0.185f + ((ver - 1) * 0.2f * sign), 0, 0);
            horizontalPartList[i].transform.localScale 
                = new Vector3(otherPartSize, otherPartSize, partSize * hor);
        }

        for (var i = 0; i < 2; i++)
        {
            var sign = (i * 2) - 1;
            verticalPartList[i].transform.localPosition 
                = new Vector3(0, 0, sign * 0.185f + ((hor - 1) * 0.2f * sign));
            verticalPartList[i].transform.localScale 
                = new Vector3(otherPartSize, otherPartSize, partSize * ver);
        }

        if (_isActiveQuad)
        {
            selectQuad.transform.localScale = new Vector3(selectQuadSize * ver, 0.08f, selectQuadSize * hor);
        }
    }

    public void ActiveQuad(bool isActive)
    {
        _isActiveQuad = isActive;
        selectQuad.SetActive(isActive);
    }

    public void ActiveTool(bool isActive)
    {
        toolObject.SetActive(isActive);
    }
}
