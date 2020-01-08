using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTransition : MonoBehaviour
{

    public Image transitionImage;

    public bool transition;

    private void Update()
    {

        transitionImage.fillAmount = Mathf.MoveTowards(transitionImage.fillAmount, 1, Time.deltaTime);

    }

}
