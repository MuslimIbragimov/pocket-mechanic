using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using PocketMechanic.Models;

namespace PocketMechanic.Services
{
    public enum VehicleRegion
    {
        NorthAmerica,
        Europe,
        Asia,
        All
    }

    public class VehicleInfoService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://vpic.nhtsa.dot.gov/api/vehicles";
        
        // Cache
        private List<string> _cachedAllMakes;
        private Dictionary<string, Dictionary<int, List<string>>> _cachedModels = new();

        // Logo URL mapping (using a free CDN)
        private static readonly Dictionary<string, string> MakeLogos = new(StringComparer.OrdinalIgnoreCase)
        {
            { "Acura", "https://www.carlogos.org/car-logos/acura-logo.png" },
            { "Alfa Romeo", "https://www.carlogos.org/car-logos/alfa-romeo-logo.png" },
            { "Aston Martin", "https://www.carlogos.org/car-logos/aston-martin-logo.png" },
            { "Audi", "https://www.carlogos.org/car-logos/audi-logo.png" },
            { "Bentley", "https://www.carlogos.org/car-logos/bentley-logo.png" },
            { "BMW", "https://www.carlogos.org/car-logos/bmw-logo.png" },
            { "Buick", "https://www.carlogos.org/car-logos/buick-logo.png" },
            { "Cadillac", "https://www.carlogos.org/car-logos/cadillac-logo.png" },
            { "Chevrolet", "https://www.carlogos.org/car-logos/chevrolet-logo.png" },
            { "Chrysler", "https://www.carlogos.org/car-logos/chrysler-logo.png" },
            { "Citroen", "https://www.carlogos.org/car-logos/citroen-logo.png" },
            { "Citroën", "https://www.carlogos.org/car-logos/citroen-logo.png" },
            { "Dodge", "https://www.carlogos.org/car-logos/dodge-logo.png" },
            { "Ferrari", "https://www.carlogos.org/car-logos/ferrari-logo.png" },
            { "Fiat", "https://www.carlogos.org/car-logos/fiat-logo.png" },
            { "Ford", "https://www.carlogos.org/car-logos/ford-logo.png" },
            { "Genesis", "https://www.carlogos.org/car-logos/genesis-logo.png" },
            { "GMC", "https://www.carlogos.org/car-logos/gmc-logo.png" },
            { "Honda", "https://www.carlogos.org/car-logos/honda-logo.png" },
            { "Hyundai", "https://www.carlogos.org/car-logos/hyundai-logo.png" },
            { "Infiniti", "https://www.carlogos.org/car-logos/infiniti-logo.png" },
            { "Jaguar", "https://www.carlogos.org/car-logos/jaguar-logo.png" },
            { "Jeep", "https://www.carlogos.org/car-logos/jeep-logo.png" },
            { "Kia", "https://www.carlogos.org/car-logos/kia-logo.png" },
            { "Lamborghini", "https://www.carlogos.org/car-logos/lamborghini-logo.png" },
            { "Land Rover", "https://www.carlogos.org/car-logos/land-rover-logo.png" },
            { "Lexus", "https://www.carlogos.org/car-logos/lexus-logo.png" },
            { "Lincoln", "https://www.carlogos.org/car-logos/lincoln-logo.png" },
            { "Maserati", "https://www.carlogos.org/car-logos/maserati-logo.png" },
            { "Mazda", "https://www.carlogos.org/car-logos/mazda-logo.png" },
            { "Mercedes-Benz", "https://www.carlogos.org/car-logos/mercedes-benz-logo.png" },
            { "Mini", "https://www.carlogos.org/car-logos/mini-logo.png" },
            { "Mitsubishi", "https://www.carlogos.org/car-logos/mitsubishi-logo.png" },
            { "Nissan", "https://www.carlogos.org/car-logos/nissan-logo.png" },
            { "Peugeot", "https://www.carlogos.org/car-logos/peugeot-logo.png" },
            { "Porsche", "https://www.carlogos.org/car-logos/porsche-logo.png" },
            { "Ram", "https://www.carlogos.org/car-logos/ram-logo.png" },
            { "Renault", "https://www.carlogos.org/car-logos/renault-logo.png" },
            { "Rolls-Royce", "https://www.carlogos.org/car-logos/rolls-royce-logo.png" },
            { "Subaru", "https://www.carlogos.org/car-logos/subaru-logo.png" },
            { "Suzuki", "https://www.carlogos.org/car-logos/suzuki-logo.png" },
            { "Tesla", "https://www.carlogos.org/car-logos/tesla-logo.png" },
            { "Toyota", "https://www.carlogos.org/car-logos/toyota-logo.png" },
            { "Volkswagen", "https://www.carlogos.org/car-logos/volkswagen-logo.png" },
            { "Volvo", "https://www.carlogos.org/car-logos/volvo-logo.png" }
        };

        // Regional make lists (major brands commonly available in each region)
        private static readonly HashSet<string> NorthAmericanMakes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Acura", "Buick", "Cadillac", "Chevrolet", "Chrysler", "Dodge", "Ford", "GMC", 
            "Honda", "Jeep", "Lincoln", "Ram", "Tesla", "Toyota", "Nissan", "Mazda", 
            "Subaru", "Infiniti", "Lexus", "Hyundai", "Kia", "Volkswagen", "Audi", 
            "BMW", "Mercedes-Benz", "Porsche", "Volvo", "Genesis", "Mitsubishi"
        };

        private static readonly HashSet<string> EuropeanMakes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Audi", "BMW", "Mercedes-Benz", "Volkswagen", "Porsche", "Volvo", "Renault", 
            "Peugeot", "Citroën", "Citroen", "Fiat", "Alfa Romeo", "Ferrari", "Lamborghini", 
            "Maserati", "Opel", "Seat", "Skoda", "Škoda", "Mini", "Land Rover", "Jaguar", 
            "Bentley", "Rolls-Royce", "Aston Martin", "Dacia", "Smart", "Lancia"
        };

        private static readonly HashSet<string> AsianMakes = new(StringComparer.OrdinalIgnoreCase)
        {
            "Toyota", "Honda", "Nissan", "Mazda", "Subaru", "Mitsubishi", "Suzuki", 
            "Lexus", "Infiniti", "Acura", "Hyundai", "Kia", "Genesis", "Daihatsu", 
            "Isuzu", "Hino", "BYD", "Geely", "Great Wall", "Chery", "MG", "Mahindra", "Tata"
        };

        public VehicleInfoService()
        {
            _httpClient = new HttpClient();
        }

        public VehicleRegion DetectRegion()
        {
            try
            {
                var region = RegionInfo.CurrentRegion;
                var countryCode = region.TwoLetterISORegionName;

                // North America
                if (countryCode == "US" || countryCode == "CA" || countryCode == "MX")
                    return VehicleRegion.NorthAmerica;

                // Europe
                var europeanCountries = new[] { "AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE", "FI", 
                    "FR", "DE", "GR", "HU", "IE", "IT", "LV", "LT", "LU", "MT", "NL", "PL", "PT", 
                    "RO", "SK", "SI", "ES", "SE", "GB", "CH", "NO", "IS", "AL", "RS", "BA", "MK", 
                    "ME", "XK", "TR", "UA", "BY", "RU" };
                if (europeanCountries.Contains(countryCode))
                    return VehicleRegion.Europe;

                // Asia
                var asianCountries = new[] { "JP", "CN", "KR", "IN", "TH", "MY", "SG", "ID", "PH", 
                    "VN", "PK", "BD", "TW", "HK", "MM", "KH", "LA", "NP", "LK" };
                if (asianCountries.Contains(countryCode))
                    return VehicleRegion.Asia;

                // Default to all if unknown
                return VehicleRegion.All;
            }
            catch
            {
                return VehicleRegion.All;
            }
        }

        public async Task<List<string>> GetMakesAsync(VehicleRegion region = VehicleRegion.All)
        {
            System.Diagnostics.Debug.WriteLine($"GetMakesAsync: Starting, region={region}");
            
            // Load all makes if not cached
            if (_cachedAllMakes == null)
            {
                System.Diagnostics.Debug.WriteLine("GetMakesAsync: Cache is null, fetching from API");
                try
                {
                    var url = $"{BaseUrl}/GetAllMakes?format=json";
                    System.Diagnostics.Debug.WriteLine($"GetMakesAsync: Calling {url}");
                    
                    var response = await _httpClient.GetStringAsync(url);
                    System.Diagnostics.Debug.WriteLine($"GetMakesAsync: Response length={response?.Length ?? 0}");
                    
                    var result = JsonSerializer.Deserialize<NHTSAResponse>(response, new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    });

                    _cachedAllMakes = result?.Results?
                        .Select(r => r.Make_Name ?? r.MakeName)  // GetAllMakes uses Make_Name
                        .Where(m => !string.IsNullOrWhiteSpace(m))
                        .OrderBy(m => m)
                        .ToList() ?? new List<string>();
                    
                    System.Diagnostics.Debug.WriteLine($"GetMakesAsync: Cached {_cachedAllMakes.Count} makes");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"GetMakesAsync: Error fetching makes: {ex.Message}");
                    return new List<string>();
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"GetMakesAsync: Using cached makes, count={_cachedAllMakes.Count}");
            }

            // Filter by region
            if (region == VehicleRegion.All)
            {
                System.Diagnostics.Debug.WriteLine($"GetMakesAsync: Returning all {_cachedAllMakes.Count} makes");
                return _cachedAllMakes;
            }

            var filteredMakes = _cachedAllMakes.Where(make => region switch
            {
                VehicleRegion.NorthAmerica => NorthAmericanMakes.Contains(make),
                VehicleRegion.Europe => EuropeanMakes.Contains(make),
                VehicleRegion.Asia => AsianMakes.Contains(make),
                _ => true
            }).ToList();

            System.Diagnostics.Debug.WriteLine($"GetMakesAsync: Filtered to {filteredMakes.Count} makes for region {region}");

            // Fallback to all makes if filtered list is empty
            if (filteredMakes.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"GetMakesAsync: No makes found for region {region}, falling back to all makes");
                return _cachedAllMakes;
            }

            return filteredMakes;
        }

        public async Task<List<MakeItem>> GetMakesWithLogosAsync(VehicleRegion region = VehicleRegion.All)
        {
            var makeNames = await GetMakesAsync(region);
            
            return makeNames.Select(name => new MakeItem
            {
                Name = name,
                LogoUrl = MakeLogos.ContainsKey(name) ? MakeLogos[name] : null
            }).ToList();
        }

        public async Task<List<string>> GetModelsAsync(string make, int year)
        {
            if (string.IsNullOrWhiteSpace(make))
                return new List<string>();

            System.Diagnostics.Debug.WriteLine($"GetModelsAsync: Starting for make={make}, year={year}");

            // Check cache
            var cacheKey = $"{make}_{year}";
            if (_cachedModels.ContainsKey(make) && _cachedModels[make].ContainsKey(year))
            {
                System.Diagnostics.Debug.WriteLine($"GetModelsAsync: Using cached models, count={_cachedModels[make][year].Count}");
                return _cachedModels[make][year];
            }

            try
            {
                var url = $"{BaseUrl}/GetModelsForMakeYear/make/{Uri.EscapeDataString(make)}/modelyear/{year}?format=json";
                System.Diagnostics.Debug.WriteLine($"GetModelsAsync: Calling {url}");
                
                var response = await _httpClient.GetStringAsync(url);
                System.Diagnostics.Debug.WriteLine($"GetModelsAsync: Response length={response?.Length ?? 0}");
                System.Diagnostics.Debug.WriteLine($"GetModelsAsync: First 500 chars: {response?.Substring(0, Math.Min(500, response?.Length ?? 0))}");
                
                var result = JsonSerializer.Deserialize<NHTSAResponse>(response, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                System.Diagnostics.Debug.WriteLine($"GetModelsAsync: Results count={result?.Results?.Count ?? 0}");
                
                if (result?.Results != null && result.Results.Count > 0)
                {
                    var firstResult = result.Results[0];
                    System.Diagnostics.Debug.WriteLine($"GetModelsAsync: First result - Model_Name={firstResult.Model_Name}, ModelName={firstResult.ModelName}");
                }

                var models = result?.Results?
                    .Select(r => r.Model_Name ?? r.ModelName)  // Support both formats
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .Where(m => !m.Equals(make, StringComparison.OrdinalIgnoreCase))  // Filter out when model name = make name
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList() ?? new List<string>();

                System.Diagnostics.Debug.WriteLine($"GetModelsAsync: Final models count={models.Count}");

                // Cache the result
                if (!_cachedModels.ContainsKey(make))
                    _cachedModels[make] = new Dictionary<int, List<string>>();
                _cachedModels[make][year] = models;

                return models;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetModelsAsync: Error - {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"GetModelsAsync: StackTrace - {ex.StackTrace}");
                return new List<string>();
            }
        }

        public List<int> GetYears()
        {
            var currentYear = DateTime.Now.Year;
            var years = new List<int>();
            
            // From current year to 1980
            for (int year = currentYear + 1; year >= 1980; year--)
            {
                years.Add(year);
            }
            
            return years;
        }

        // Response models
        private class NHTSAResponse
        {
            public List<NHTSAResult> Results { get; set; }
        }

        private class NHTSAResult
        {
            public string MakeName { get; set; }
            public string Make_Name { get; set; }  // GetAllMakes uses this format
            public string ModelName { get; set; }
            public string Model_Name { get; set; }  // Some endpoints use this format
        }
    }
}
