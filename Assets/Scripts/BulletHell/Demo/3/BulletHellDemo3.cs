using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BulletHell3D;
using DG.Tweening;
using VContainer;

public class BulletHellDemo3 : MonoBehaviour
{
    [SerializeField]
    private BHRenderObject demoRenderObj;
    [SerializeField]
    private BHCustomUpdater demoUpdater;
    [SerializeField]
    private float burstBulletCount = 60;

    [Space(10)]
    [SerializeField]
    private BHPattern pattern;
    [SerializeField]
    private int spawnPatternCount;
    [SerializeField]
    private float spawnPatternGap;
    [SerializeField]
    private float scale;
    [SerializeField]
    private float highSpeed;
    [SerializeField]
    private float lowSpeed;
    [SerializeField]
    private float speedDownTime;
    [SerializeField]
    private float speedUpTime;
    [SerializeField]
    private float gapTime;
    [SerializeField]
    private Ease speedDownEase;
    [SerializeField]
    private Ease speedUpEase;

    [Inject]
    private Player player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
            StartCoroutine(Showcase());
    }

    IEnumerator Showcase()
    {
        for (int i = 0; i < burstBulletCount; i++)
            demoUpdater.AddBullet(demoRenderObj, transform.position, Random.insideUnitSphere);
        for (int i = 0; i < spawnPatternCount; i++)
        {
            StartCoroutine(CreatePattern());
            yield return new WaitForSeconds(spawnPatternGap);
        }
    }

    IEnumerator CreatePattern()
    {
        GameObject go = new GameObject();

        go.transform.position = transform.position;
        Vector3 toPlayer = player.transform.position - transform.position;
        Vector3 newFoward = new Vector3(toPlayer.x, 0, toPlayer.z).normalized + Vector3.up * Random.Range(-0.15f, -0.05f);
        go.transform.rotation = Quaternion.LookRotation(newFoward, new Vector3(Random.Range(-0.4f, 0.4f), 1, Random.Range(-0.4f, 0.4f)));
        go.transform.localScale = Vector3.zero;

        BHTransformUpdater updater = go.AddComponent<BHTransformUpdater>();
        updater.SetPattern(pattern);

        float speed = highSpeed;
        float timer = 0;
        go.transform.DOScale(Vector3.one * scale, speedDownTime).SetEase(speedDownEase);
        DOTween.To(() => speed, x => speed = x, lowSpeed, speedDownTime).SetEase(speedDownEase);
        while (go != null && timer < speedDownTime)
        {
            go.transform.position += go.transform.forward * speed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0;
        while (go != null && timer < gapTime)
        {
            go.transform.position += go.transform.forward * speed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        DOTween.To(() => speed, x => speed = x, highSpeed, speedUpTime).SetEase(speedUpEase);
        while (go != null)
        {
            go.transform.position += go.transform.forward * speed * Time.deltaTime;
            yield return null;
        }
    }
}
