namespace CastleArcher
{
    /// <summary>
    /// Anything that can receive damage. Attackers depend on this rather than on <see cref="Health"/>,
    /// so a projectile does not need to know whether it hit an enemy or the castle.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int amount);
    }
}
