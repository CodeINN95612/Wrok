using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wrok.Identity.Application.Common;

namespace Wrok.Identity.Application.Tests.Common;
[TestFixture]
internal class TokenGeneratorTest
{
    [Test]
    public void Generate_ShouldReturnStringOfExpectedLength()
    {
        var generator = new TokenGenerator();
        int byteCount = 32;
        var token = generator.Generate(byteCount);

        // Base64 encoding increases length, but not linearly.
        // Each 3 bytes become 4 chars, so length = ceil(byteCount / 3) * 4
        int expectedLength = ((byteCount + 2) / 3) * 4;
        Assert.That(token.Length, Is.EqualTo(expectedLength));
    }

    [Test]
    public void Generate_ShouldReturnUniqueTokens()
    {
        var generator = new TokenGenerator();
        var token1 = generator.Generate(32);
        var token2 = generator.Generate(32);

        Assert.That(token1, Is.Not.EqualTo(token2));
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(64)]
    public void Generate_WithDifferentByteCounts_ShouldReturnCorrectLength(int byteCount)
    {
        var generator = new TokenGenerator();
        var token = generator.Generate(byteCount);

        int expectedLength = ((byteCount + 2) / 3) * 4;
        Assert.That(token.Length, Is.EqualTo(expectedLength));
    }

    [Test]
    public void Generate_ShouldNotReturnNullOrEmpty()
    {
        var generator = new TokenGenerator();
        var token = generator.Generate(32);

        Assert.Multiple(() =>
        {
            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
        });
    }
}
