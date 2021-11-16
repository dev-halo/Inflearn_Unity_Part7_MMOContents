using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBar : MonoBehaviour
{
    [SerializeField]
    Transform hpBar = null;

    public void SetHpBar(float ratio)
    {
        ratio = Mathf.Clamp(ratio, 0f, 1f);
        hpBar.localScale = new Vector3(ratio, 1f, 1f);
    }
}
