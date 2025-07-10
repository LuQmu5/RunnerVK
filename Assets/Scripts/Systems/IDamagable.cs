public interface IDamagable
{
    public float Health { get; }

    public void TakeDamage(float amount);
}