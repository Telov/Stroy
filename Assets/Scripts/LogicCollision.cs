using Game.Particles;
using Game.Particles.Enum;
using Game.Tasks;
using General.Audio.Interfaces;
using General.Constants;
using UnityEngine;
using Zenject;

public class LogicCollision : MonoBehaviour
{
    [SerializeField] private BoxCollider[] colliders;

    private bool _isBroken;
    private IAudioService _audioService;
    private TaskTracker _taskTracker;
    private MemoryPool<MonoParticles> _particlesPool;

    [Inject]
    private void Construct(MemoryPool<MonoParticles> particles, IAudioService audioService)
    {
        _audioService = audioService;
        _particlesPool = particles;
    }
    
    public void Notify(Vector3 position)
    {
        if (!_isBroken)
        {
            foreach (var boxCollider in colliders)
            {
                Destroy(boxCollider);
            }
        }
        
        _isBroken = true;
        var particleInstance = _particlesPool.Spawn();
        particleInstance.SetParticle(EParticles.StarPoof);
        particleInstance.SetActive(true);
        particleInstance.transform.position = position;
        particleInstance.Disable(1, () => _particlesPool.Despawn(particleInstance));
        _audioService.PlayOneShot(AudioConstants.HouseCrushing);
        _taskTracker.IncreaseValue();
    }

    public void Initialize(TaskTracker taskTracker)
    {
        _taskTracker = taskTracker;
    }
}
