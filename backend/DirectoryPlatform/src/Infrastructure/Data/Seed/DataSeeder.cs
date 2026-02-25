using System.Security.Cryptography;
using System.Text.Json;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        await SeedLanguages(context);
        await SeedRegions(context);
        var categories = await SeedCategories(context);
        await SeedAttributeDefinitions(context, categories);
        await SeedUsers(context);
        await SeedSubscriptionTiers(context);
        await context.SaveChangesAsync();
    }

    private static async Task SeedLanguages(ApplicationDbContext context)
    {
        var languages = new List<Language>
        {
            new() { Id = Guid.NewGuid(), Code = "en", Name = "English", NativeName = "English", DisplayOrder = 1 },
            new() { Id = Guid.NewGuid(), Code = "fr", Name = "French", NativeName = "Français", DisplayOrder = 2 },
            new() { Id = Guid.NewGuid(), Code = "de", Name = "German", NativeName = "Deutsch", DisplayOrder = 3 },
            new() { Id = Guid.NewGuid(), Code = "it", Name = "Italian", NativeName = "Italiano", DisplayOrder = 4 },
            new() { Id = Guid.NewGuid(), Code = "es", Name = "Spanish", NativeName = "Español", DisplayOrder = 5 },
            new() { Id = Guid.NewGuid(), Code = "pt", Name = "Portuguese", NativeName = "Português", DisplayOrder = 6 },
            new() { Id = Guid.NewGuid(), Code = "nl", Name = "Dutch", NativeName = "Nederlands", DisplayOrder = 7 },
            new() { Id = Guid.NewGuid(), Code = "ru", Name = "Russian", NativeName = "Русский", DisplayOrder = 8 },
            new() { Id = Guid.NewGuid(), Code = "zh", Name = "Chinese", NativeName = "中文", DisplayOrder = 9 },
            new() { Id = Guid.NewGuid(), Code = "ja", Name = "Japanese", NativeName = "日本語", DisplayOrder = 10 },
            new() { Id = Guid.NewGuid(), Code = "ar", Name = "Arabic", NativeName = "العربية", DisplayOrder = 11 },
            new() { Id = Guid.NewGuid(), Code = "tr", Name = "Turkish", NativeName = "Türkçe", DisplayOrder = 12 }
        };
        await context.Languages.AddRangeAsync(languages);
    }

    private static async Task SeedRegions(ApplicationDbContext context)
    {
        var switzerland = new Region { Id = Guid.NewGuid(), Name = "Switzerland", Slug = "switzerland", CountryCode = "CH", DisplayOrder = 1 };
        await context.Regions.AddAsync(switzerland);

        var cantons = new (string Name, string Slug, int Order, string[] Cities)[]
        {
            ("Zurich", "zurich", 1, new[] { "Zurich", "Winterthur", "Uster", "Dübendorf" }),
            ("Bern", "bern", 2, new[] { "Bern", "Biel/Bienne", "Thun", "Köniz" }),
            ("Lucerne", "lucerne", 3, new[] { "Lucerne", "Emmen", "Kriens", "Horw" }),
            ("Uri", "uri", 4, new[] { "Altdorf", "Erstfeld" }),
            ("Schwyz", "schwyz", 5, new[] { "Schwyz", "Freienbach", "Küssnacht" }),
            ("Obwalden", "obwalden", 6, new[] { "Sarnen", "Engelberg" }),
            ("Nidwalden", "nidwalden", 7, new[] { "Stans", "Hergiswil" }),
            ("Glarus", "glarus", 8, new[] { "Glarus", "Netstal" }),
            ("Zug", "zug", 9, new[] { "Zug", "Baar", "Cham" }),
            ("Fribourg", "fribourg", 10, new[] { "Fribourg", "Bulle", "Villars-sur-Glâne" }),
            ("Solothurn", "solothurn", 11, new[] { "Solothurn", "Olten", "Grenchen" }),
            ("Basel-Stadt", "basel-stadt", 12, new[] { "Basel", "Riehen" }),
            ("Basel-Landschaft", "basel-landschaft", 13, new[] { "Liestal", "Allschwil", "Reinach" }),
            ("Schaffhausen", "schaffhausen", 14, new[] { "Schaffhausen", "Neuhausen" }),
            ("Appenzell Ausserrhoden", "appenzell-ausserrhoden", 15, new[] { "Herisau", "Teufen" }),
            ("Appenzell Innerrhoden", "appenzell-innerrhoden", 16, new[] { "Appenzell" }),
            ("St. Gallen", "st-gallen", 17, new[] { "St. Gallen", "Rapperswil-Jona", "Wil" }),
            ("Graubünden", "graubuenden", 18, new[] { "Chur", "Davos", "St. Moritz" }),
            ("Aargau", "aargau", 19, new[] { "Aarau", "Baden", "Wettingen", "Wohlen" }),
            ("Thurgau", "thurgau", 20, new[] { "Frauenfeld", "Kreuzlingen", "Arbon" }),
            ("Ticino", "ticino", 21, new[] { "Lugano", "Bellinzona", "Locarno", "Mendrisio" }),
            ("Vaud", "vaud", 22, new[] { "Lausanne", "Yverdon-les-Bains", "Montreux", "Nyon" }),
            ("Valais", "valais", 23, new[] { "Sion", "Brig", "Sierre", "Zermatt" }),
            ("Neuchâtel", "neuchatel", 24, new[] { "Neuchâtel", "La Chaux-de-Fonds" }),
            ("Geneva", "geneva", 25, new[] { "Geneva", "Carouge", "Lancy", "Vernier" }),
            ("Jura", "jura", 26, new[] { "Delémont", "Porrentruy" })
        };

        foreach (var (name, slug, order, cities) in cantons)
        {
            var canton = new Region
            {
                Id = Guid.NewGuid(), Name = name, Slug = slug,
                ParentId = switzerland.Id, CountryCode = "CH", DisplayOrder = order
            };
            await context.Regions.AddAsync(canton);

            foreach (var city in cities)
            {
                var citySlug = (slug + "-" + city).ToLower().Replace(" ", "-").Replace(".", "").Replace("/", "-").Replace("â", "a").Replace("ü", "u").Replace("é", "e");
                await context.Regions.AddAsync(new Region
                {
                    Id = Guid.NewGuid(), Name = city, Slug = citySlug,
                    ParentId = canton.Id, CountryCode = "CH", DisplayOrder = 0
                });
            }
        }
    }

    private static async Task<Dictionary<string, Guid>> SeedCategories(ApplicationDbContext context)
    {
        var categories = new Dictionary<string, Guid>();

        var roots = new (string Name, string Slug, int Order, (string Name, string Slug, int Order)[] Children)[]
        {
            ("Vehicles", "vehicles", 1, new[]
            {
                ("Cars", "cars", 1),
                ("Motorcycles", "motorcycles", 2),
                ("Trucks", "trucks", 3),
                ("Boats", "boats", 4)
            }),
            ("Real Estate", "real-estate", 2, new[]
            {
                ("For Sale", "real-estate-for-sale", 1),
                ("For Rent", "real-estate-for-rent", 2),
                ("Commercial", "real-estate-commercial", 3)
            }),
            ("Services", "services", 3, new[]
            {
                ("Home Services", "home-services", 1),
                ("Professional", "professional-services", 2),
                ("Personal", "personal-services", 3)
            }),
            ("Electronics", "electronics", 4, new[]
            {
                ("Computers", "computers", 1),
                ("Phones", "phones", 2),
                ("Audio & Video", "audio-video", 3)
            }),
            ("Jobs", "jobs", 5, new[]
            {
                ("Full-time", "full-time", 1),
                ("Part-time", "part-time", 2),
                ("Freelance", "freelance", 3)
            }),
            ("Home & Garden", "home-garden", 6, new[]
            {
                ("Furniture", "furniture", 1),
                ("Appliances", "appliances", 2),
                ("Garden", "garden", 3)
            })
        };

        foreach (var (name, slug, order, children) in roots)
        {
            var parent = new Category { Id = Guid.NewGuid(), Name = name, Slug = slug, DisplayOrder = order };
            await context.Categories.AddAsync(parent);
            categories[slug] = parent.Id;

            foreach (var (cName, cSlug, cOrder) in children)
            {
                var child = new Category { Id = Guid.NewGuid(), Name = cName, Slug = cSlug, ParentId = parent.Id, DisplayOrder = cOrder };
                await context.Categories.AddAsync(child);
                categories[cSlug] = child.Id;
            }
        }

        return categories;
    }

    private static async Task SeedAttributeDefinitions(ApplicationDbContext context, Dictionary<string, Guid> categories)
    {
        var attrs = new List<AttributeDefinition>();

        // Cars
        if (categories.TryGetValue("cars", out var carsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Make", Slug = "make", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Audi", "BMW", "Chevrolet", "Citroën", "Fiat", "Ford", "Honda", "Hyundai", "Kia", "Mazda", "Mercedes-Benz", "Nissan", "Opel", "Peugeot", "Renault", "Seat", "Škoda", "Subaru", "Suzuki", "Tesla", "Toyota", "Volkswagen", "Volvo" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Model", Slug = "model", Type = AttributeType.Text, CategoryId = carsId, IsFilterable = false, IsRequired = true, DisplayOrder = 2 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Year", Slug = "year", Type = AttributeType.Number, CategoryId = carsId, IsFilterable = true, DisplayOrder = 3, MinValue = 1950, MaxValue = 2026 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Mileage", Slug = "mileage", Type = AttributeType.Number, CategoryId = carsId, IsFilterable = true, DisplayOrder = 4, Unit = "km" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Fuel Type", Slug = "fuel-type", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, DisplayOrder = 5, Options = JsonSerializer.Serialize(new[] { "Petrol", "Diesel", "Electric", "Hybrid", "LPG" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Transmission", Slug = "transmission", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, DisplayOrder = 6, Options = JsonSerializer.Serialize(new[] { "Manual", "Automatic", "Semi-automatic" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Color", Slug = "color", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, DisplayOrder = 7, Options = JsonSerializer.Serialize(new[] { "Black", "White", "Silver", "Grey", "Blue", "Red", "Green", "Brown", "Beige", "Yellow", "Orange" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "condition", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, DisplayOrder = 8, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair", "For Parts" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "price", Type = AttributeType.Number, CategoryId = carsId, IsFilterable = true, DisplayOrder = 9, Unit = "CHF", MinValue = 0, MaxValue = 1000000 }
            });
        }

        // Real Estate For Sale
        if (categories.TryGetValue("real-estate-for-sale", out var realEstateId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Rooms", Slug = "rooms", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, MinValue = 1, MaxValue = 20 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Area", Slug = "area", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 2, Unit = "m²", MinValue = 10, MaxValue = 10000 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Floor", Slug = "floor", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 3, MinValue = -2, MaxValue = 50 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Year Built", Slug = "year-built", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 4, MinValue = 1800, MaxValue = 2026 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Parking", Slug = "parking", Type = AttributeType.Boolean, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 5 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Balcony", Slug = "balcony", Type = AttributeType.Boolean, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 6 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Property Type", Slug = "property-type", Type = AttributeType.Select, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 7, Options = JsonSerializer.Serialize(new[] { "Apartment", "House", "Villa", "Studio", "Penthouse", "Land" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "re-price", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 8, Unit = "CHF", MinValue = 0, MaxValue = 50000000 }
            });
        }

        // Real Estate For Rent
        if (categories.TryGetValue("real-estate-for-rent", out var rentId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Rooms", Slug = "rooms", Type = AttributeType.Number, CategoryId = rentId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, MinValue = 1, MaxValue = 20 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Area", Slug = "area", Type = AttributeType.Number, CategoryId = rentId, IsFilterable = true, DisplayOrder = 2, Unit = "m²" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Monthly Rent", Slug = "monthly-rent", Type = AttributeType.Number, CategoryId = rentId, IsFilterable = true, DisplayOrder = 3, Unit = "CHF/month" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Furnished", Slug = "furnished", Type = AttributeType.Boolean, CategoryId = rentId, IsFilterable = true, DisplayOrder = 4 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Pets Allowed", Slug = "pets-allowed", Type = AttributeType.Boolean, CategoryId = rentId, IsFilterable = true, DisplayOrder = 5 }
            });
        }

        // Phones
        if (categories.TryGetValue("phones", out var phonesId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Brand", Slug = "brand", Type = AttributeType.Select, CategoryId = phonesId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Apple", "Samsung", "Google", "Huawei", "Xiaomi", "OnePlus", "Sony", "Nokia", "Motorola" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Storage", Slug = "storage", Type = AttributeType.Select, CategoryId = phonesId, IsFilterable = true, DisplayOrder = 2, Options = JsonSerializer.Serialize(new[] { "32GB", "64GB", "128GB", "256GB", "512GB", "1TB" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "phone-condition", Type = AttributeType.Select, CategoryId = phonesId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair", "For Parts" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "phone-price", Type = AttributeType.Number, CategoryId = phonesId, IsFilterable = true, DisplayOrder = 4, Unit = "CHF" }
            });
        }

        // Motorcycles
        if (categories.TryGetValue("motorcycles", out var motoId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Make", Slug = "moto-make", Type = AttributeType.Select, CategoryId = motoId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "BMW", "Ducati", "Harley-Davidson", "Honda", "Kawasaki", "KTM", "Suzuki", "Triumph", "Yamaha" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Engine Size", Slug = "engine-size", Type = AttributeType.Number, CategoryId = motoId, IsFilterable = true, DisplayOrder = 2, Unit = "cc" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Year", Slug = "moto-year", Type = AttributeType.Number, CategoryId = motoId, IsFilterable = true, DisplayOrder = 3, MinValue = 1950, MaxValue = 2026 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Type", Slug = "moto-type", Type = AttributeType.Select, CategoryId = motoId, IsFilterable = true, DisplayOrder = 4, Options = JsonSerializer.Serialize(new[] { "Sport", "Touring", "Naked", "Cruiser", "Enduro", "Scooter" }) }
            });
        }

        // Jobs Full-time
        if (categories.TryGetValue("full-time", out var jobsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Industry", Slug = "industry", Type = AttributeType.Select, CategoryId = jobsId, IsFilterable = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "IT & Software", "Finance", "Healthcare", "Engineering", "Marketing", "Sales", "Education", "Legal", "Hospitality", "Construction" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Salary Range", Slug = "salary", Type = AttributeType.Number, CategoryId = jobsId, IsFilterable = true, DisplayOrder = 2, Unit = "CHF/year" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Experience Level", Slug = "experience", Type = AttributeType.Select, CategoryId = jobsId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "Entry Level", "Mid Level", "Senior", "Lead", "Executive" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Remote", Slug = "remote", Type = AttributeType.Boolean, CategoryId = jobsId, IsFilterable = true, DisplayOrder = 4 }
            });
        }

        await context.AttributeDefinitions.AddRangeAsync(attrs);
    }

    private static async Task SeedUsers(ApplicationDbContext context)
    {
        var (adminHash, adminSalt) = HashPassword("Admin123!");
        var (userHash, userSalt) = HashPassword("User123!");

        var users = new List<User>
        {
            new()
            {
                Id = Guid.NewGuid(), Username = "admin", Email = "admin@admin.com",
                PasswordHash = adminHash, PasswordSalt = adminSalt,
                Role = UserRole.Admin, IsEmailVerified = true,
                FirstName = "Admin", LastName = "User"
            },
            new()
            {
                Id = Guid.NewGuid(), Username = "user", Email = "user@user.com",
                PasswordHash = userHash, PasswordSalt = userSalt,
                Role = UserRole.User, IsEmailVerified = true,
                FirstName = "Regular", LastName = "User"
            }
        };
        await context.Users.AddRangeAsync(users);
    }

    private static async Task SeedSubscriptionTiers(ApplicationDbContext context)
    {
        var standardId = Guid.NewGuid();
        var premiumId = Guid.NewGuid();

        var tiers = new List<SubscriptionTier>
        {
            new()
            {
                Id = standardId, Name = "Standard", Description = "Basic listing features",
                MonthlyPrice = 9.90m, AnnualPrice = 99.00m, IsActive = true, DisplayOrder = 1
            },
            new()
            {
                Id = premiumId, Name = "Premium", Description = "Advanced features with priority listing",
                MonthlyPrice = 29.90m, AnnualPrice = 299.00m, IsActive = true, DisplayOrder = 2
            }
        };
        await context.SubscriptionTiers.AddRangeAsync(tiers);

        var features = new List<SubscriptionFeature>
        {
            new() { Id = Guid.NewGuid(), SubscriptionTierId = standardId, Name = "Max Listings", Value = "10", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = standardId, Name = "Max Photos Per Listing", Value = "5", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = standardId, Name = "Featured Listing", Value = "false", IsEnabled = false },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Max Listings", Value = "50", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Max Photos Per Listing", Value = "20", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Featured Listing", Value = "true", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Priority Support", Value = "true", IsEnabled = true }
        };
        await context.SubscriptionFeatures.AddRangeAsync(features);
    }

    private static (string Hash, string Salt) HashPassword(string password)
    {
        var saltBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 100000, HashAlgorithmName.SHA256);
        var hash = Convert.ToBase64String(pbkdf2.GetBytes(32));

        return (hash, salt);
    }
}
