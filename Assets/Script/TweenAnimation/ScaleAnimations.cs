using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAnimations : MonoBehaviour
{
    public virtual void ScaleInLiniar(List<GameObject> targetList, float animationTime)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i].transform.localScale = Vector3.zero;
            targetList[i].transform.DOScale(Vector3.one, animationTime);
        }
    }
    
    public virtual void ScaleInLiniar(List<GameObject> targetList, float animationTime, float targetScale)
    {
        for (int i = 0; i < targetList.Count; i++)
        {
            targetList[i].transform.localScale = Vector3.zero;
            targetList[i].transform.DOScale(new Vector3(targetScale, targetScale), animationTime);
        }
    }


    public virtual void ScaleOutLiniar(List<GameObject> targetList, float animationTime)
    {
        for (int i = 0; i < targetList.Count; i++)
            targetList[i].transform.DOScale(Vector3.zero, animationTime);
    }
}
