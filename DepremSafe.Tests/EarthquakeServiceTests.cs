using DepremSafe.Core.Entities;
using DepremSafe.Service.Services;
using FluentAssertions;
using Moq;
using Xunit;
using DepremSafe.Service.Interfaces;
using AutoMapper;
using DepremSafe.Core.Interfaces;

namespace DepremSafe.Tests
{
    public class EarthquakeServiceTests
    {
        private readonly EarthquakeService _sut;

        public EarthquakeServiceTests()
        {
            var mockRepo = new Mock<IEarthquakeRepository>();
            var mockMapper = new Mock<IMapper>();
            var mockFcm = new Mock<IFcmService>();
            var mockHttp = new HttpClient();

            _sut = new EarthquakeService(
                mockRepo.Object,
                mockMapper.Object,
                mockFcm.Object,
                mockHttp,
                null!
            );
        }

        #region CalculateDistance

        [Fact]
        public void CalculateDistance_SamePoint_ReturnsZero()
        {
            var result = _sut.CalculateDistance(41.0, 29.0, 41.0, 29.0);
            result.Should().BeApproximately(0, 0.001);
        }

        [Fact]
        public void CalculateDistance_IstanbulToAnkara_ReturnsApprox350km()
        {
            // İstanbul: 41.01, 28.97 — Ankara: 39.93, 32.85
            var result = _sut.CalculateDistance(41.01, 28.97, 39.93, 32.85);
            result.Should().BeInRange(340, 360);
        }

        [Fact]
        public void CalculateDistance_IsSymmetric()
        {
            var ab = _sut.CalculateDistance(41.01, 28.97, 39.93, 32.85);
            var ba = _sut.CalculateDistance(39.93, 32.85, 41.01, 28.97);
            ab.Should().BeApproximately(ba, 0.001);
        }

        [Fact]
        public void CalculateDistance_ReturnsPositiveValue()
        {
            var result = _sut.CalculateDistance(41.01, 28.97, 39.93, 32.85);
            result.Should().BePositive();
        }

        #endregion

        #region GetNearest10Cities

        [Fact]
        public void GetNearest10Cities_ReturnsClosestCitiesFirst()
        {
            // Deprem İstanbul'da
            var cities = new List<City>
            {
                new() { Name = "İstanbul", Latitude = 41.01, Longitude = 28.97 },  // çok yakın
                new() { Name = "Ankara",   Latitude = 39.93, Longitude = 32.85 },  // uzak
                new() { Name = "İzmir",    Latitude = 38.42, Longitude = 27.14 },  // orta
            };

            var result = _sut.GetNearest10Cities(41.01, 28.97, cities);

            result[0].Name.Should().Be("İstanbul");
        }

        [Fact]
        public void GetNearest10Cities_ReturnsMax10Cities()
        {
            var cities = Enumerable.Range(1, 20).Select(i => new City
            {
                Name = $"Şehir{i}",
                Latitude = 39.0 + i * 0.1,
                Longitude = 35.0 + i * 0.1
            }).ToList();

            var result = _sut.GetNearest10Cities(41.01, 28.97, cities);

            result.Should().HaveCount(10);
        }

        [Fact]
        public void GetNearest10Cities_EmptyList_ReturnsEmpty()
        {
            var result = _sut.GetNearest10Cities(41.01, 28.97, new List<City>());
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetNearest10Cities_FewerThan10Cities_ReturnsAll()
        {
            var cities = new List<City>
            {
                new() { Name = "İstanbul", Latitude = 41.01, Longitude = 28.97 },
                new() { Name = "Ankara",   Latitude = 39.93, Longitude = 32.85 }
            };

            var result = _sut.GetNearest10Cities(41.01, 28.97, cities);

            result.Should().HaveCount(2);
        }

        #endregion
    }
}