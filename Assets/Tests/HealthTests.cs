using NUnit.Framework;
using UnityEngine;

namespace CastleArcher
{
    public class HealthTests
    {
        private GameObject host;
        private Health health;

        [SetUp]
        public void SetUp()
        {
            host = new GameObject("Health test");
            health = host.AddComponent<Health>();
            health.SetMaxHealth(10);
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(host);
        }

        [Test]
        public void DamageStopsAtZero()
        {
            health.TakeDamage(25);

            Assert.AreEqual(0, health.CurrentHealth);
        }

        [Test]
        public void DeathFiresOnce()
        {
            int deaths = 0;
            health.OnDied += () => deaths++;

            health.TakeDamage(10);
            health.TakeDamage(10);

            Assert.AreEqual(1, deaths);
            Assert.IsTrue(health.IsDead);
        }

        [Test]
        public void HealStopsAtMaximum()
        {
            health.TakeDamage(4);

            health.Heal(100);

            Assert.AreEqual(health.MaxHealth, health.CurrentHealth);
        }
    }
}