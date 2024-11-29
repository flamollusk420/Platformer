using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
    public AudioClip PlayerJump, PlayerHighJump, Dash, PlayerHit, PlayerMeleeHit, PlayerMelee, PlayerShoot, Parry, ParriedBulletHit, ParriedBulletDeath, BulletCollision, BulletDestroyedByDash, PlayerExplosion, PlayerFireWave, BothFireWavesDestroyed, DoubleFireWaveKill, DoubleFireWaveOneShot, PlayerSoulClaw, PlayerSoulClawHit, PlayerSoulClawKill, PlayerHeal, PlayerDeath, EnemyDeath1, EnemyDeath2, EnemyDeath3, CommonCollectible, UncommonCollectible, CharmFound, StyleRankUp, PrimarySwap, Pause, Unpause, Menu1, Menu2;
    private AudioSource audioSource;

    void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayClip(AudioClip audioClip, Transform transformToSpawnAt, float volume) {
        AudioSource SFXaudioSource = Instantiate(audioSource, transformToSpawnAt.position, Quaternion.identity);
        SFXaudioSource.gameObject.tag = "SoundManagerSound";
        SFXaudioSource.clip = audioClip;
        SFXaudioSource.volume = volume;
        SFXaudioSource.Play();
        float clipLength = SFXaudioSource.clip.length;
        Destroy(SFXaudioSource.gameObject, clipLength);
    }
}
