using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell3D;
using DG.Tweening;
using Cysharp.Threading.Tasks.Triggers;

public class BulletHellDemo2 : MonoBehaviour
{
    [SerializeField]
    private BHPattern pattern;

    [Space(10)]
    [SerializeField]
    private float minRadius = 5;
    [SerializeField]
    private int patternCount = 25;
    [SerializeField]
    private float rotatePerPattern = 11;
    [SerializeField]
    private float radiusPerPattern = 0.8f;
    [SerializeField]
    private float spawnPatternGap = 0.05f;
    [SerializeField]
    private float patternExpandTime = 0.5f;
    [SerializeField]
    private float waitToDropTime = 1;
    [SerializeField]
    private float dropTime = 1;
    [SerializeField]
    private Ease dropEase;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha2))
            StartCoroutine(Showcase());
    }

    IEnumerator Showcase()
    {
        GameObject[] objects = new GameObject[patternCount];
        for (int i = 0; i < patternCount; i++)
        {
            objects[i] = new GameObject();
            objects[i].transform.position = transform.position;
            objects[i].transform.rotation = Quaternion.Euler(0, rotatePerPattern * i, 0);
            objects[i].transform.localScale = Vector3.zero;
            var updater = objects[i].AddComponent<BHTransformUpdater>();
            updater.SetPattern(pattern);
            objects[i].transform
                .DOScale(Vector3.one * (minRadius + i * radiusPerPattern), patternExpandTime)
                .SetLink(objects[i]);
            yield return new WaitForSeconds(spawnPatternGap);
        }
        yield return new WaitForSeconds(waitToDropTime);
        for (int i = 0; i < patternCount; i++)
        {
            objects[i].transform
                .DOMoveY(0, dropTime)
                .SetEase(dropEase)
                .SetLink(objects[i]);
            yield return new WaitForSeconds(spawnPatternGap);
        }
    }
}
