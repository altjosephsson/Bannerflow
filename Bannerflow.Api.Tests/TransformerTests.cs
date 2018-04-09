using AutoFixture;
using Bannerflow.Api.Models;
using Bannerflow.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Bannerflow.Api.Tests
{
    public class TransformerTests
    {
        private readonly ITransformer sut;

        public TransformerTests()
        {
            sut = new Transformer();
        }

        [Fact]
        public void TransformerTestPropertyValues()
        {
            //arrange
            var fixture = new Fixture();
            fixture.Customize<Banner>(b => b.Without(p => p.InternalId));
            var banner = fixture.Create<Banner>();

            var propertiesToKeep = new[] { "Id", "Modified", "Created", "Html" };

            var rnd = new Random();
            var randomIndex = rnd.Next(0, 3);

            var chosenOne = propertiesToKeep[randomIndex];

            var truePropertyToKeep = new[] { chosenOne };

            //act
            var result1 = sut.Transform(banner, truePropertyToKeep);

            //assert
            var result = result1.GetType().GetProperty(truePropertyToKeep[0]).GetValue(result1);
            Assert.NotNull(result);

            propertiesToKeep = propertiesToKeep.Where(w => w != truePropertyToKeep[0]).ToArray();

            foreach (var property in propertiesToKeep)
            {
                var propResult = result1.GetType().GetProperty(property).GetValue(result1);
                Assert.Null(propResult);
            }

        }
    }
}
