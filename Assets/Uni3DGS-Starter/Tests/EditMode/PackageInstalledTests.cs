using NUnit.Framework;

namespace Uni3DGS.Starter.Tests
{
    public sealed class PackageInstalledTests
    {
        [Test]
        public void PublicTypesResolve()
        {
            Assert.That(typeof(GaussianSplat).Assembly.GetName().Name, Is.EqualTo("Uni3DGS.Runtime"));
            Assert.That(typeof(GaussianSplatRenderer).IsPublic, Is.True);
        }
    }
}
