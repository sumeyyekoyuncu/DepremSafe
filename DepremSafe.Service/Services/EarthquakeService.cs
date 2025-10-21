using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using DepremSafe.Core.DTOs;
using DepremSafe.Core.Entities;
using DepremSafe.Core.Interfaces;
using DepremSafe.Data.Context;
using DepremSafe.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DepremSafe.Service.Services
{
    public class EarthquakeService:IEarthquakeService
    {
        private readonly IEarthquakeRepository _earthquakeRepository;
        private readonly IMapper _mapper;
        private readonly IFcmService _fcmService;
        private readonly HttpClient _httpClient;
        private readonly DepremSafeDbContext _dbContext;

        public EarthquakeService(IEarthquakeRepository earthquakeRepository, IMapper mapper, IFcmService fcmService, HttpClient httpClient,DepremSafeDbContext dbContext)
        {
            _earthquakeRepository = earthquakeRepository;
            _mapper = mapper;
            _fcmService = fcmService;
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task<EarthquakeDTO> GetByIdAsync(Guid id)
        {
            var earthquake = await _earthquakeRepository.GetByIdAsync(id);
            return _mapper.Map<EarthquakeDTO>(earthquake);
        }

        public async Task<IEnumerable<EarthquakeDTO>> GetAllAsync()
        {
            var earthquakes = await _earthquakeRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<EarthquakeDTO>>(earthquakes);
        }

        public async Task AddAsync(EarthquakeDTO earthquakeDto)
        {
            var earthquake = _mapper.Map<Earthquake>(earthquakeDto);
            await _earthquakeRepository.AddAsync(earthquake);
        }
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Dünya yarıçapı km
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Asin(Math.Sqrt(a));
            return R * c;
        }

        public List<City> GetNearest10Cities(double depremLat, double depremLon, List<City> allCities)
        {
            return allCities
                .OrderBy(c => CalculateDistance(depremLat, depremLon, c.Latitude, c.Longitude))
                .Take(10)
                .ToList();
        }
        public async Task CheckAndNotifyLatestEarthquakeAsync()
        {
            // 1️⃣ Kandilli/Orhan Aydoğdu API'den deprem verisi çek
            var url = "https://api.orhanaydogdu.com.tr/deprem";
            var response = await _httpClient.GetStringAsync(url);
            var allDeprems = JsonSerializer.Deserialize<List<EarthquakeDTO>>(response);

            // 2️⃣ 5.0+ en son depremi bul
            var latestDeprem = allDeprems
                .Where(d => d.Magnitude >= 5.0)
                .OrderByDescending(d => d.OccurredAt)
                .FirstOrDefault();

            if (latestDeprem == null)
            {
                Console.WriteLine("5.0+ deprem yok.");
                return;
            }

            Console.WriteLine($"Son deprem: {latestDeprem.Magnitude} şiddetinde, {latestDeprem.Location}");

            // 3️⃣ En yakın 10 şehri bul
            var allCities = _dbContext.Cities.ToList();
            var nearestCities = GetNearest10Cities(latestDeprem.Latitude, latestDeprem.Longitude, allCities);

            // 4️⃣ Bu şehirlerdeki kullanıcıları seç
            var usersToNotify = _dbContext.Users
                .Where(u => nearestCities.Select(c => c.Name).Contains(u.City))
                .ToList();

            // 5️⃣ FCM ile bildirim gönder
            foreach (var user in usersToNotify)
            {
                try
                {
                    await _fcmService.SendNotificationAsync(user.FcmToken,
                        "Deprem oldu!",
                        $"Bölgenizde {latestDeprem.Magnitude} büyüklüğünde bir deprem meydana geldi. Güvende misiniz?");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Bildirim gönderilemedi: {user.City} - {ex.Message}");
                }
            }

            // 6️⃣ Deprem kaydını veritabanına ekle
            _dbContext.Earthquakes.Add(new Earthquake
            {
                Latitude = latestDeprem.Latitude,
                Longitude = latestDeprem.Longitude,
                Magnitude = latestDeprem.Magnitude,
                OccurredAt = latestDeprem.OccurredAt,
                Location = latestDeprem.Location
            });

            await _dbContext.SaveChangesAsync();
        }


    }
}
