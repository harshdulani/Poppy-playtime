using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControlScript : MonoBehaviour
{

    [Header(" Manager ")]
    // public GameController gameController;
    public Animator levelCompleteAnimator;

    [Header(" Settings  ")]
    public float particleSpeed = 3000;
    public float speedIncrement;
    public int coinsCount;
    float speed;
    public AudioSource releaseCoinsSound;
    public AudioSource coinSound;
    bool fountainSoundPlayed;
    float timer;
    public float pTime;
    float t = 0;
    bool isLvlComplete;
    bool moreCoins;
    bool sliderLerped;
    bool rewardVideoCoins;

    //UiManager ui;
    int combineCoins;


    public bool gameover;
    private void Start()
    {
        //ui = UiManager.instance;
    }

    public void PlayControlledParticles(Vector3 pos, RectTransform targetUI, bool isRewardVideo = false, bool lvlComplete = false, bool isMoreCoins = false)
    {
        isLvlComplete = lvlComplete;
        moreCoins = isMoreCoins;
        rewardVideoCoins = isRewardVideo;
        sliderLerped = false;

        speed = particleSpeed * Screen.width / 1080f;
        ParticleSystem ps = GetComponent<ParticleSystem>();

        transform.position = pos;
        StartCoroutine(PlayCoinParticlesCoroutine(ps, targetUI));
    }

    IEnumerator PlayCoinParticlesCoroutine(ParticleSystem ps, RectTransform targetUIElement)
    {
        //speed = particleSpeed;
        fountainSoundPlayed = false;

        Vector3[] distances = new Vector3[coinsCount];

        bool[] reached = new bool[coinsCount];

        if (isLvlComplete || moreCoins)
        {
            reached = new bool[coinsCount];
            distances = new Vector3[coinsCount];
        }

        ParticleSystem.EmissionModule em = ps.emission;
        em.SetBurst(0, new ParticleSystem.Burst(0, coinsCount));


        //yield return new WaitForSeconds(1);

        if (releaseCoinsSound != null)
            releaseCoinsSound.Play();

        ps.Play();


        yield return new WaitForSeconds(1f);

        // Store the particles positions
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ps.particleCount];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = particles[i].position;
        }


        while (ps.isPlaying)
        {
            particles = new ParticleSystem.Particle[ps.particleCount];

            ps.GetParticles(particles);
            for (int i = 0; i < particles.Length; i++)
            {
                Vector3 targetPos = Vector3.zero;

                targetPos.x = targetUIElement.position.x;
                targetPos.y = targetUIElement.position.y;
                targetPos.z = 0;


                Vector2 dir = targetPos - particles[i].position;
                t += Time.deltaTime / 2f;

                float smooth = Vector2.Distance(targetPos, distances[i]) / pTime;

                particles[i].position = Vector2.MoveTowards(particles[i].position, targetPos, smooth * Time.deltaTime);


                speed += speedIncrement;

                if (dir.magnitude < 0.05f)
                {
                    particles[i].color = new Color32(0, 0, 0, 0);

                    if (!reached[i])
                    {
                        if (!gameover)
                        {
                            //ui.totalCoins++;
                            //ui.combineCoins.text = "" + ui.totalCoins;
                        }
                        else
                        {
                            //ui.totalCoins--;
                            //ui.combineCoins.text = "" + ui.totalCoins;
                        }

                        reached[i] = true;
                        Vibration.Vibrate(10);
                    }


                    if (coinSound != null && !fountainSoundPlayed)
                    {
                        if (coinSound != null)
                            coinSound.Play();

                        fountainSoundPlayed = true;
                    }
                }
            }

            ps.SetParticles(particles, particles.Length);

            timer += Time.deltaTime / 2f;

            if (timer > 0.5f)
            {
                ps.Stop();
                //  gameController.UpdateCoins();


                if (isLvlComplete)
                {
                    // Hide the gift panel
                    //levelCompleteAnimator.Play("HideGift");
                }

                yield return null;
            }

            yield return new WaitForSeconds(Time.deltaTime / 2);
        }


        // gameController.UpdateCoins();


        if (isLvlComplete)
        {
            // Hide the gift panel
            //levelCompleteAnimator.Play("HideGift");
        }

        timer = 0;

        yield return null;
    }
}
