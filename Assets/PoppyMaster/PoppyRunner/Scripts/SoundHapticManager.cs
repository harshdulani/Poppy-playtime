using UnityEngine;

public class SoundHapticManager : MonoBehaviour
{
    public static SoundHapticManager instance;
    public AudioSource audioSource, bgAS;
    public AudioClip win, punch, enemySound,BossIceShoot, blast,fail,girlScream, groundSlam;

    private void Awake()
    {
      instance = this;
    }

    public void Vibrate(long MilliSecs)
    {
        if (SavedData.instance.GetHepaticState().Equals("Off"))
            return;

        if (Application.platform == RuntimePlatform.WindowsEditor)
            GameEssentials.instance.PrintOut("Vibrating");
        else 
            Vibration.Vibrate(MilliSecs);
        
    }

    public void PlaySound(AudioClip audio)
    {
        if (SavedData.instance.GetSoundState().Equals("Off") || audio == null)
            return;

         audioSource.PlayOneShot(audio);
    }

    public void StopPlaying()
    {
        audioSource.Stop();
    }

    public void PlayWinSound()
    {
        PlaySound(win);
        audioSource.loop = false;
    }

    public void PlayPunchSound()
    {
        Vibrate(18);
        PlaySound(punch);
        audioSource.loop = false;
    }

    public void PlayEnemySound()
    {
        PlaySound(enemySound);
        audioSource.loop = false;
    }
    public void PlayBossIceShootSound()
    {
        Vibrate(18);

        audioSource.clip = BossIceShoot;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void PlayBlastSound()
    {
        Vibrate(25);
        PlaySound(blast);
        audioSource.loop = false;
    }

    public void PlayGroundSlamSound()
    {
        Vibrate(25);
        PlaySound(groundSlam);
        audioSource.loop = false;
    }

    public void PlayFailSound()
    {
        PlaySound(fail);
        audioSource.loop = false;
    }

    public void PlayGirlScreamSound()
    {
        Vibrate(18);
        PlaySound(girlScream);
        audioSource.loop = false;
    }

    public void Audio_On()
    {
        audioSource.enabled = true;
        bgAS.enabled = true;
    }
    public void Audio_Off()
    {
        audioSource.enabled = false;
        bgAS.enabled = false;
    }
}
