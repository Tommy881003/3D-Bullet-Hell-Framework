using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell3D;
using DG.Tweening;
using VContainer;

public class BulletHellDemo5 : MonoBehaviour
{
    [SerializeField]
    private BHPattern pattern;
    [SerializeField]
    private float scale;
    [SerializeField]
    private float patternExpandTime;
    [SerializeField]
    private Ease patternExpandEase;
    [SerializeField]
    private int decomposePerTick;

    [Space(10)]
    [SerializeField]
    private BHRenderObject tracerObj;
    [SerializeField]
    private float tracerSpeed;
    [SerializeField]
    private float tracerDelay;

    [Inject]
    private System.Func<GameObject, BulletHellDemo5_1> createDemo;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
            StartCoroutine(Showcase());
    }

    IEnumerator Showcase()
    {
        GameObject go = new GameObject();
        go.transform.position = transform.position;
        go.transform.rotation = Random.rotation;
        go.transform.localScale = Vector3.zero;

        BulletHellDemo5_1 demoUpdater = this.createDemo(go);
        demoUpdater.SetPattern(pattern);

        go.transform.DOScale(Vector3.one * scale, patternExpandTime).SetEase(patternExpandEase);
        yield return new WaitForSeconds(patternExpandTime);

        float timer = 0;

        while (go != null)
        {
            demoUpdater.SpawnRandom(decomposePerTick, tracerObj, tracerSpeed, tracerDelay);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
