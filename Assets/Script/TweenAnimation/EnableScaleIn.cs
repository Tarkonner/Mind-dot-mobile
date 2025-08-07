using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableScaleIn : ScaleAnimations
{
    [SerializeField] float scaleTime = 1;

    private void OnEnable()
    {
        ScaleInLiniar(gameObject, scaleTime);
    }

    public void ScaleOut() => ScaleOutLiniar(gameObject, scaleTime);
}
