using System.Collections;
using UnityEngine;

public class ParticleControlScript : MonoBehaviour
{
	[Header(" Settings  ")]
    public int coinsCount;
	public float pTime;

	private float _timer;
	private int _combineCoins;

	public void PlayControlledParticles(Vector3 pos, RectTransform targetUI)
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();

        transform.position = pos;
        StartCoroutine(PlayCoinParticlesCoroutine(ps, targetUI));
    }

    IEnumerator PlayCoinParticlesCoroutine(ParticleSystem ps, RectTransform targetUIElement)
    {
        var distances = new Vector3[coinsCount];

        var reached = new bool[coinsCount];

		var em = ps.emission;
        em.SetBurst(0, new ParticleSystem.Burst(0, coinsCount));
		ps.Play();

		yield return new WaitForSeconds(1f);

        // Store the particles positions
        var particles = new ParticleSystem.Particle[ps.particleCount];
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i] = particles[i].position;
        }
		
        while (ps.isPlaying)
        {
            particles = new ParticleSystem.Particle[ps.particleCount];

            ps.GetParticles(particles);
            for (var i = 0; i < particles.Length; i++)
            {
                var targetPos = Vector3.zero;

                targetPos.x = targetUIElement.position.x;
                targetPos.y = targetUIElement.position.y;
                targetPos.z = 0;

				Vector2 dir = targetPos - particles[i].position;
				float smooth = Vector2.Distance(targetPos, distances[i]) / pTime;

                particles[i].position = Vector2.MoveTowards(particles[i].position, targetPos, smooth * Time.deltaTime);

                if (dir.magnitude < 0.05f)
                {
                    particles[i].color = new Color32(0, 0, 0, 0);

                    if (!reached[i])
                    {
                        reached[i] = true;
                    }
                }
            }

            ps.SetParticles(particles, particles.Length);

            _timer += Time.deltaTime / 2f;

            if (_timer > 0.5f)
            {
                ps.Stop();
                yield return null;
            }

            yield return new WaitForSeconds(Time.deltaTime / 2);
        }
		
        _timer = 0;
        yield return null;
    }
}
