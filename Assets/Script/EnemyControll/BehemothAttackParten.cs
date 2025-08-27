using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehemothAttackParten : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    public GameObject flashEffectPrefab;
    private bool isInvincible = false;

    private int minMeteorCount = 1;
    private int maxMeteorCount = 3;

    private int minExplosionCount = 1;
    private int maxExplosionCount = 2;

    private int minThornCount = 2;
    private int maxThornCount = 3;

    public Transform player;

    public GameObject thornPreFab;
    public Transform thornSpwanPoint;

    public GameObject laserRightPrefab;
    public Transform laserRightSpawnPoint;

    public GameObject laserLeftPrefab;
    public Transform laserLeftSpawnPoint;

    public GameObject meteorPrefab;
    public Transform meteorSpawnPoint;

    public GameObject explosionPrefab;

    private float attackInterval = 1.5f;

    private List<WeightedAttack> weightedAttacks = new List<WeightedAttack>();

    private float weightMeteor = 2f;
    private float weightExplosion = 3f;
    private float weightLaserLeft = 1f;
    private float weightLaserRight = 1f;
    private float weightThorn = 4f;

    private Health bossHealth;
    private Coroutine attackRoutine;

    [System.Serializable]
    public struct WeightedAttack
    {
        public string name;
        public Action method;
        public float weight;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        bossHealth = GetComponent<Health>();
        animator = GetComponent<Animator>();

        bossHealth.Died.AddListener(OnBossDied);
        bossHealth.phase2.AddListener(OnPhase2);
        bossHealth.triggerUlt.AddListener(PlayRageAnimation);

        weightedAttacks.Add(new WeightedAttack { name = "Thorn", method = SpawnThorn, weight = weightThorn });
        weightedAttacks.Add(new WeightedAttack { name = "Explosion", method = createExplosion, weight = weightExplosion });
        weightedAttacks.Add(new WeightedAttack { name = "Meteor", method = summonMeteors, weight = weightMeteor });
        weightedAttacks.Add(new WeightedAttack { name = "LaserLeft", method = LaserLeft, weight = weightLaserLeft });
        weightedAttacks.Add(new WeightedAttack { name = "LaserRight", method = LaserRight, weight = weightLaserRight });

        attackRoutine = StartCoroutine(AttackLoop());

        bossHealth.canTakeDamageCheck = () => !isInvincible;

    }

    IEnumerator AttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            if (isInvincible)
                continue; 

            WeightedAttack selected = GetWeightedRandomAttack();
            selected.method.Invoke();

        }
    }

    private WeightedAttack GetWeightedRandomAttack()
    {
        float totalWeight = 0f;
        foreach (var atk in weightedAttacks)
            totalWeight += atk.weight;

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float runningTotal = 0f;

        foreach (var atk in weightedAttacks)
        {
            runningTotal += atk.weight;
            if (randomValue <= runningTotal)
                return atk;
        }

        return weightedAttacks[0]; // fallback
    }

    void summonMeteors()
    {
        StartCoroutine(MeteorShower());
    }

    void createExplosion()
    {
        StartCoroutine(ExplosionShower());
    }

    void LaserLeft()
    {
        GameObject laser = Instantiate(laserLeftPrefab, laserLeftSpawnPoint.position, Quaternion.identity);
        Destroy(laser, 1.25f);
    }

    void LaserRight()
    {
        GameObject laser = Instantiate(laserRightPrefab, laserRightSpawnPoint.position, Quaternion.identity);
        Destroy(laser, 1.25f);
    }

    void SpawnThorn()
    {
        StartCoroutine(ThornShower());
    }

    void PlayRageAnimation()
    {
        animator.SetTrigger("Rage");
        isInvincible = true;

        StartCoroutine(EndRageAfterDelay(2.2f));
    }

    public void FlashOnRageFrame()
    {
        GameObject flash = Instantiate(flashEffectPrefab, transform.position, Quaternion.identity);
        Destroy(flash, 0.5f); 
    }

    void OnBossDied()
    {
        if (attackRoutine != null)
            StopCoroutine(attackRoutine);

        //animator.SetTrigger("Die");
        //StartCoroutine(ShowingContinueStory());
        ContinueStory.instance.ShowContinueStory();
    }

    IEnumerator EndRageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isInvincible = false;
    }
    IEnumerator ThornShower()
    {
        int thornCount = UnityEngine.Random.Range(minThornCount, maxThornCount + 1);

        for (int i = 0; i < thornCount; i++)
        {
            float offset = UnityEngine.Random.Range(-2.5f, 2.5f);
            Vector2 randomPos = new Vector2(player.position.x + offset, thornSpwanPoint.position.y);
            GameObject thorn = Instantiate(thornPreFab, randomPos, Quaternion.identity);
            Destroy(thorn, 0.4f);

            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator MeteorShower()
    {
        int meteorCount = UnityEngine.Random.Range(minMeteorCount, maxMeteorCount + 1);

        for (int i = 0; i < meteorCount; i++)
        {
            float spread = UnityEngine.Random.Range(-3f, 3f);
            Vector2 randomPos = new Vector2(player.position.x + spread, meteorSpawnPoint.position.y);
            GameObject meteor = Instantiate(meteorPrefab, randomPos, Quaternion.identity);
            Destroy(meteor, 2f);

            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator ExplosionShower()
    {
        int explosionCount = UnityEngine.Random.Range(minExplosionCount, maxExplosionCount + 1);

        for (int i = 0; i < explosionCount; i++)
        {
            float offset = UnityEngine.Random.Range(-3f, 3f);
            Vector2 randomPos = new Vector2(player.position.x + offset, player.position.y);
            GameObject explosion = Instantiate(explosionPrefab, randomPos, Quaternion.identity);
            Destroy(explosion, 0.9f);

            yield return new WaitForSeconds(0.15f);
        }
    }
    void OnPhase2()
    {
        spriteRenderer.color = new Color(0.5f, 0f, 0f, 1f);
        StartCoroutine(ShineEffect());

        attackInterval = 0.75f;

        minMeteorCount = 3;
        maxMeteorCount = 5;

        minExplosionCount = 2;
        maxExplosionCount = 4;

        minThornCount = 3;
        maxThornCount = 4;

        weightMeteor = 5f;
        weightExplosion = 4f;
        weightLaserLeft = 3f;
        weightLaserRight = 3f;
        weightThorn = 2f;

        for (int i = 0; i < weightedAttacks.Count; i++)
        {
            switch (weightedAttacks[i].name)
            {
                case "Meteor":
                    weightedAttacks[i] = new WeightedAttack { name = "Meteor", method = summonMeteors, weight = weightMeteor };
                    break;

                case "Explosion":
                    weightedAttacks[i] = new WeightedAttack { name = "Explosion", method = createExplosion, weight = weightExplosion };
                    break;

                case "LaserLeft":
                    weightedAttacks[i] = new WeightedAttack { name = "LaserLeft", method = LaserLeft, weight = weightLaserLeft };
                    break;

                case "LaserRight":
                    weightedAttacks[i] = new WeightedAttack { name = "LaserRight", method = LaserRight, weight = weightLaserRight };
                    break;

                case "Thorn":
                    weightedAttacks[i] = new WeightedAttack { name = "Thorn", method = SpawnThorn, weight = weightThorn };
                    break;
            }
        }
    }

    IEnumerator ShineEffect()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime * 4f;
            float shine = Mathf.PingPong(t, 1f) * 0.5f + 0.5f; // range 0.5 to 1

            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(shine, 0.3f * shine, 0.3f * shine, 1f);
            }

            yield return null;
        }
    }

    IEnumerator ShowingContinueStory()
    {
        yield return new WaitForSeconds(2f);

        ContinueStory.instance.ShowContinueStory();
    }
    
    //public void DestroyBoss()
    //{
    //    SceneLoader.Instance.LoadWinSceneAfterDelay(2f);

    //    Destroy(gameObject);
    //}
}
