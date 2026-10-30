using System;

public interface IDamageable
{
    event Action<float> OnDamaged;
    event Action OnDied;
}