using System.Collections;
using System.Collections.Generic;
using Bogay.SceneAudioManager;
using SpellBound.Core;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [field: SerializeField]
    public Character character { get; private set; }

    [SerializeField]
    private GameObject deathVFX;

    void Start()
    {
        this.character = ScriptableObject.Instantiate(this.character);
        this.character.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.character.HP <= 0)
        {
            Instantiate(this.deathVFX, transform.position, Quaternion.identity);
            SceneAudioManager.instance.PlayByName("EnemyDeath");
            Destroy(gameObject);
        }
    }
}
