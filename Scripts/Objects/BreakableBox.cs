using UnityEngine;

public class BreakableBox : Resetable, IDamageable
{
    [SerializeField] private ParticleSystem _particleSmoke;
    [SerializeField] private ParticleSystem _particleBreak;
    [SerializeField] private ParticleSystem _beesParticle;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private string _brokenAudio;

    public void ReceivesHit(int damage = 1)
    {
        SubscribeToReset();
        AudioManager.instance.Play(_brokenAudio);
        _particleSmoke.Play();
        _particleBreak.Play();
        _beesParticle.Stop();
        Turn(false);
        _audioSource.enabled = false;
    }

    protected override void DoReset()
    {
        base.DoReset();
        _audioSource.enabled = true;
        _beesParticle.Play();
    }
}
