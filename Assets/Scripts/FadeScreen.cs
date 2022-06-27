using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeScreen : MonoBehaviour
{

    [SerializeField] private GameObject fadeScreen;

    public void FadeOut()
    {

        fadeScreen.GetComponent<Animator>().Play("FadeOut");

    }

}
