using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell3D;
using DG.Tweening;
using VContainer;

public class BulletHellDemo4 : MonoBehaviour
{
    [SerializeField]
    private BHPattern pattern;
    [SerializeField]
    private int spawnPatternCount;
    [SerializeField]
    private float spawnPatternGap;
    [SerializeField]
    private float scalePerPattern;
    [SerializeField]
    private float rotatePerPattern;
    [SerializeField]
    private float patternExpandTime;
    [SerializeField]
    private float speed;

    [Inject]
    private Player player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
            StartCoroutine(Showcase(transform.position, (player.transform.position - transform.position).normalized));
    }

    IEnumerator Showcase(Vector3 postion, Vector3 forward)
    {
        for (int i = 0; i < spawnPatternCount; i++)
        {
            StartCoroutine(SpawnPattern(postion, forward, i));
            yield return new WaitForSeconds(spawnPatternGap);
        }
    }

    IEnumerator SpawnPattern(Vector3 postion, Vector3 forward, int index)
    {
        GameObject go = new GameObject();

        float angle = index * rotatePerPattern * Mathf.Deg2Rad;
        float scale = index * scalePerPattern;

        go.transform.position = postion;
        go.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        go.transform.rotation = Quaternion.LookRotation(forward, go.transform.right * Mathf.Cos(angle) + go.transform.up * Mathf.Sin(angle));
        go.transform.localScale = Vector3.zero;
        go.AddComponent<BHTransformUpdater>().SetPattern(pattern);

        go.transform.DOScale(Vector3.one * scale, patternExpandTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(patternExpandTime);
        while (go != null)
        {
            go.transform.position += forward * speed * Time.deltaTime;
            yield return null;
        }
    }
}
