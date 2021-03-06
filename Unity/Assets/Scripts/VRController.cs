﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum HandType
{
    Right,
    Left,
    Both
}

public class VRController : MonoBehaviour {
    private GameObject TouchObject;
    private Coroutine PressTriggerCoroutine;
    [SerializeField]
    public GameObject HandCamera;
    // Use this for initialization

    void Start()
    {
        StartCoroutine(DeploymentScope());
    }

    public HandType type;

    void OnTriggerEnter(Collider col)
    {
        TouchObject = col.gameObject;
        TouchObject.GetComponentInChildren<Text>().color = Color.red;
        PressTriggerCoroutine = StartCoroutine(PressTrigger());
    }

    void OnTriggerExit(Collider col)
    {
        TouchObject.GetComponentInChildren<Text>().color = Color.black;
        TouchObject = null;
        StopCoroutine(PressTriggerCoroutine);
    }

    IEnumerator PressTrigger()
    {
        while (true)
        {
            if (TouchObject && InputVRController.GetPress(InputVRController.InputPress.PressTrigger,HandType.Right))
            {
                TouchObject.GetComponent<TitleButton>().OnClick();
            }
            yield return null;
        }
    }

    IEnumerator DeploymentScope()
    {
        while (true)
        {
            if(InputVRController.GetPress(InputVRController.InputPress.PressMenu,type))
            {
                if(type == HandType.Left)
                {
                    yield break;
                }
                HandCamera.SetActive(!HandCamera.activeSelf);
                if (HandCamera.activeSelf && GameManager.NowScene == GameManager.Scenes.stage && type == HandType.Right)
                {
                    FindObjectOfType<ReticleSystem>().EnableReticle();
                }
            }
            yield return null;
        }
    }

}
