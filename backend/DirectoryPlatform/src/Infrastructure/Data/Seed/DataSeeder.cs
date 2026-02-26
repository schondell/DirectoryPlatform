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
            // --- Vehicles & Transport ---
            ("Automobiles", "automobiles", 1, new[]
            {
                ("Cars", "cars", 1),
                ("Utility Vehicles", "utility-vehicles", 2),
                ("Caravans & Motorhomes", "caravans-motorhomes", 3),
                ("Car Parts & Accessories", "car-parts-accessories", 4)
            }),
            ("Motorcycles & Scooters", "motorcycles-scooters", 2, new[]
            {
                ("Motorcycles", "motorcycles", 1),
                ("Scooters", "scooters", 2),
                ("Parts & Accessories", "moto-parts-accessories", 3)
            }),
            ("Nautical", "nautical", 3, new[]
            {
                ("Sailboats", "sailboats", 1),
                ("Motorboats", "motorboats", 2),
                ("Jet Skis", "jet-skis", 3),
                ("Boat Accessories", "boat-accessories", 4)
            }),

            // --- Property ---
            ("Real Estate", "real-estate", 4, new[]
            {
                ("For Sale", "real-estate-for-sale", 1),
                ("For Rent", "real-estate-for-rent", 2),
                ("Commercial", "real-estate-commercial", 3),
                ("Shared Housing", "shared-housing", 4),
                ("Vacation Rentals", "vacation-rentals", 5)
            }),

            // --- Electronics & Tech ---
            ("Computing", "computing", 5, new[]
            {
                ("Laptops", "laptops", 1),
                ("Desktops", "desktops", 2),
                ("Tablets", "tablets", 3),
                ("Components & Accessories", "computer-accessories", 4),
                ("Software", "software", 5),
                ("Networking", "networking", 6)
            }),
            ("Telephony", "telephony", 6, new[]
            {
                ("Mobile Phones", "mobile-phones", 1),
                ("Phone Accessories", "phone-accessories", 2),
                ("Landline Phones", "landline-phones", 3)
            }),
            ("TV & Audio & Home Cinema", "tv-audio-home-cinema", 7, new[]
            {
                ("Televisions", "televisions", 1),
                ("Speakers & Sound Systems", "speakers-sound-systems", 2),
                ("Home Cinema", "home-cinema", 3),
                ("Headphones", "headphones", 4)
            }),
            ("Photography & Cameras", "photography-cameras", 8, new[]
            {
                ("Digital Cameras", "digital-cameras", 1),
                ("Lenses", "lenses", 2),
                ("Video Cameras", "video-cameras", 3),
                ("Photo Accessories", "photo-accessories", 4)
            }),
            ("Video Games", "video-games", 9, new[]
            {
                ("Consoles", "consoles", 1),
                ("Games", "games", 2),
                ("Gaming Accessories", "gaming-accessories", 3)
            }),

            // --- Home & Living ---
            ("Home & Garden & DIY", "home-garden-diy", 10, new[]
            {
                ("Furniture", "furniture", 1),
                ("Home Appliances", "home-appliances", 2),
                ("Garden & Outdoor", "garden-outdoor", 3),
                ("DIY & Tools", "diy-tools", 4),
                ("Decoration", "decoration", 5),
                ("Kitchen", "kitchen", 6)
            }),

            // --- Fashion & Personal ---
            ("Clothing", "clothing", 11, new[]
            {
                ("Women's Clothing", "womens-clothing", 1),
                ("Men's Clothing", "mens-clothing", 2),
                ("Children's Clothing", "childrens-clothing", 3),
                ("Shoes", "shoes", 4),
                ("Bags & Accessories", "bags-accessories", 5)
            }),
            ("Jewelry & Watches", "jewelry-watches", 12, new[]
            {
                ("Watches", "watches", 1),
                ("Rings", "rings", 2),
                ("Necklaces", "necklaces", 3),
                ("Bracelets", "bracelets", 4),
                ("Earrings", "earrings", 5)
            }),
            ("Beauty & Wellness", "beauty-wellness", 13, new[]
            {
                ("Cosmetics", "cosmetics", 1),
                ("Perfumes", "perfumes", 2),
                ("Hair Care", "hair-care", 3),
                ("Wellness Equipment", "wellness-equipment", 4)
            }),

            // --- Family ---
            ("Baby & Childcare", "baby-childcare", 14, new[]
            {
                ("Strollers & Car Seats", "strollers-car-seats", 1),
                ("Toys & Games", "baby-toys", 2),
                ("Baby Clothing", "baby-clothing", 3),
                ("Feeding & Nursing", "feeding-nursing", 4),
                ("Childcare Services", "childcare-services", 5)
            }),

            // --- Hobbies & Culture ---
            ("Sports & Leisure", "sports-leisure", 15, new[]
            {
                ("Winter Sports", "winter-sports", 1),
                ("Cycling", "cycling", 2),
                ("Fitness", "fitness", 3),
                ("Water Sports", "water-sports", 4),
                ("Ball Sports", "ball-sports", 5),
                ("Outdoor & Hiking", "outdoor-hiking", 6)
            }),
            ("Music & Instruments", "music-instruments", 16, new[]
            {
                ("Guitars", "guitars", 1),
                ("Keyboards & Pianos", "keyboards-pianos", 2),
                ("Wind Instruments", "wind-instruments", 3),
                ("Drums & Percussion", "drums-percussion", 4),
                ("DJ Equipment", "dj-equipment", 5)
            }),
            ("Books & Comics & Magazines", "books-comics-magazines", 17, new[]
            {
                ("Books", "books", 1),
                ("Comics", "comics", 2),
                ("Magazines", "magazines", 3),
                ("E-books & Readers", "ebooks-readers", 4)
            }),
            ("Cinema & DVDs", "cinema-dvds", 18, new[]
            {
                ("DVDs & Blu-ray", "dvds-bluray", 1),
                ("Vinyl & Records", "vinyl-records", 2)
            }),
            ("Games & Toys", "games-toys", 19, new[]
            {
                ("Board Games", "board-games", 1),
                ("Puzzles", "puzzles", 2),
                ("Action Figures", "action-figures", 3),
                ("Model Kits", "model-kits", 4)
            }),
            ("Collections", "collections", 20, new[]
            {
                ("Stamps", "stamps", 1),
                ("Coins & Banknotes", "coins-banknotes", 2),
                ("Trading Cards", "trading-cards", 3),
                ("Vintage & Retro", "vintage-retro", 4)
            }),
            ("Art & Antiques", "art-antiques", 21, new[]
            {
                ("Paintings", "paintings", 1),
                ("Sculptures", "sculptures", 2),
                ("Antique Furniture", "antique-furniture", 3),
                ("Collectible Art", "collectible-art", 4)
            }),

            // --- Outdoor & Travel ---
            ("Camping", "camping", 22, new[]
            {
                ("Tents", "tents", 1),
                ("Sleeping Bags & Mats", "sleeping-bags-mats", 2),
                ("Camping Furniture", "camping-furniture", 3),
                ("Camping Accessories", "camping-accessories", 4)
            }),
            ("Vacations & Travel", "vacations-travel", 23, new[]
            {
                ("Holiday Rentals", "holiday-rentals", 1),
                ("Travel Offers", "travel-offers", 2),
                ("Travel Accessories", "travel-accessories", 3)
            }),

            // --- Professional ---
            ("Employment", "employment", 24, new[]
            {
                ("Full-time", "full-time", 1),
                ("Part-time", "part-time", 2),
                ("Temporary", "temporary", 3),
                ("Apprenticeships", "apprenticeships", 4),
                ("Freelance", "freelance", 5),
                ("Internships", "internships", 6)
            }),
            ("SME & Artisans & Farmers", "sme-artisans-farmers", 25, new[]
            {
                ("Business Equipment", "business-equipment", 1),
                ("Agricultural Machinery", "agricultural-machinery", 2),
                ("Commercial Vehicles", "commercial-vehicles", 3),
                ("Business Services", "business-services", 4)
            }),

            // --- Events & Tickets ---
            ("Tickets & Vouchers", "tickets-vouchers", 26, new[]
            {
                ("Concert Tickets", "concert-tickets", 1),
                ("Sports Tickets", "sports-tickets", 2),
                ("Event Tickets", "event-tickets", 3),
                ("Gift Vouchers", "gift-vouchers", 4)
            }),

            // --- Food & Drink ---
            ("Wine & Gastronomy", "wine-gastronomy", 27, new[]
            {
                ("Wine", "wine", 1),
                ("Spirits", "spirits", 2),
                ("Gourmet Food", "gourmet-food", 3),
                ("Kitchen Equipment", "gastro-kitchen-equipment", 4)
            }),

            // --- Animals ---
            ("Animals", "animals", 28, new[]
            {
                ("Dogs", "dogs", 1),
                ("Cats", "cats", 2),
                ("Birds", "birds", 3),
                ("Fish & Aquariums", "fish-aquariums", 4),
                ("Horses", "horses", 5),
                ("Reptiles", "reptiles", 6),
                ("Small Animals", "small-animals", 7),
                ("Pet Accessories", "pet-accessories", 8)
            }),

            // --- Weapons ---
            ("Weapons", "weapons", 29, new[]
            {
                ("Firearms", "firearms", 1),
                ("Airsoft & Paintball", "airsoft-paintball", 2),
                ("Knives & Blades", "knives-blades", 3),
                ("Accessories & Ammunition", "weapons-accessories", 4)
            }),

            // --- Social ---
            ("Dating", "dating", 30, new[]
            {
                ("Men seeking Women", "men-seeking-women", 1),
                ("Women seeking Men", "women-seeking-men", 2),
                ("Friendships", "friendships", 3)
            }),
            ("Modeling", "modeling", 31, new[]
            {
                ("Models", "models", 1),
                ("Photographers", "photographers", 2),
                ("Casting Calls", "casting-calls", 3)
            }),

            // --- Other ---
            ("Other", "other", 32, new[]
            {
                ("Miscellaneous", "miscellaneous", 1),
                ("Lost & Found", "lost-found", 2),
                ("Free Items", "free-items", 3)
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

        // ── Automobiles > Cars ──
        if (categories.TryGetValue("cars", out var carsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Make", Slug = "make", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Audi", "BMW", "Chevrolet", "Citroën", "Dacia", "Fiat", "Ford", "Honda", "Hyundai", "Jaguar", "Jeep", "Kia", "Land Rover", "Mazda", "Mercedes-Benz", "Mini", "Mitsubishi", "Nissan", "Opel", "Peugeot", "Porsche", "Renault", "Seat", "Škoda", "Smart", "Subaru", "Suzuki", "Tesla", "Toyota", "Volkswagen", "Volvo" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Model", Slug = "model", Type = AttributeType.Text, CategoryId = carsId, IsFilterable = false, IsRequired = true, DisplayOrder = 2 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Year", Slug = "year", Type = AttributeType.Number, CategoryId = carsId, IsFilterable = true, DisplayOrder = 3, MinValue = 1950, MaxValue = 2026 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Mileage", Slug = "mileage", Type = AttributeType.Number, CategoryId = carsId, IsFilterable = true, DisplayOrder = 4, Unit = "km" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Fuel Type", Slug = "fuel-type", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, DisplayOrder = 5, Options = JsonSerializer.Serialize(new[] { "Petrol", "Diesel", "Electric", "Hybrid", "Plug-in Hybrid", "LPG", "CNG" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Transmission", Slug = "transmission", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, DisplayOrder = 6, Options = JsonSerializer.Serialize(new[] { "Manual", "Automatic", "Semi-automatic" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Color", Slug = "color", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, DisplayOrder = 7, Options = JsonSerializer.Serialize(new[] { "Black", "White", "Silver", "Grey", "Blue", "Red", "Green", "Brown", "Beige", "Yellow", "Orange" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Body Type", Slug = "body-type", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, DisplayOrder = 8, Options = JsonSerializer.Serialize(new[] { "Sedan", "Hatchback", "SUV", "Coupe", "Convertible", "Wagon", "Van", "Pickup" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Engine Power", Slug = "engine-power", Type = AttributeType.Number, CategoryId = carsId, IsFilterable = true, DisplayOrder = 9, Unit = "HP" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "condition", Type = AttributeType.Select, CategoryId = carsId, IsFilterable = true, DisplayOrder = 10, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair", "For Parts" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "price", Type = AttributeType.Number, CategoryId = carsId, IsFilterable = true, DisplayOrder = 11, Unit = "CHF", MinValue = 0, MaxValue = 1000000 }
            });
        }

        // ── Motorcycles & Scooters > Motorcycles ──
        if (categories.TryGetValue("motorcycles", out var motoId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Make", Slug = "moto-make", Type = AttributeType.Select, CategoryId = motoId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Aprilia", "BMW", "Ducati", "Harley-Davidson", "Honda", "Husqvarna", "Indian", "Kawasaki", "KTM", "Moto Guzzi", "MV Agusta", "Suzuki", "Triumph", "Yamaha" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Engine Size", Slug = "engine-size", Type = AttributeType.Number, CategoryId = motoId, IsFilterable = true, DisplayOrder = 2, Unit = "cc" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Year", Slug = "moto-year", Type = AttributeType.Number, CategoryId = motoId, IsFilterable = true, DisplayOrder = 3, MinValue = 1950, MaxValue = 2026 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Mileage", Slug = "moto-mileage", Type = AttributeType.Number, CategoryId = motoId, IsFilterable = true, DisplayOrder = 4, Unit = "km" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Type", Slug = "moto-type", Type = AttributeType.Select, CategoryId = motoId, IsFilterable = true, DisplayOrder = 5, Options = JsonSerializer.Serialize(new[] { "Sport", "Touring", "Naked", "Cruiser", "Enduro", "Adventure", "Scooter", "Custom", "Classic" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "moto-condition", Type = AttributeType.Select, CategoryId = motoId, IsFilterable = true, DisplayOrder = 6, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair", "For Parts" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "moto-price", Type = AttributeType.Number, CategoryId = motoId, IsFilterable = true, DisplayOrder = 7, Unit = "CHF" }
            });
        }

        // ── Real Estate > For Sale ──
        if (categories.TryGetValue("real-estate-for-sale", out var realEstateId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Property Type", Slug = "property-type", Type = AttributeType.Select, CategoryId = realEstateId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Apartment", "House", "Villa", "Studio", "Penthouse", "Chalet", "Loft", "Land", "Commercial" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Rooms", Slug = "rooms", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, IsRequired = true, DisplayOrder = 2, MinValue = 1, MaxValue = 20 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Area", Slug = "area", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 3, Unit = "m²", MinValue = 10, MaxValue = 10000 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Floor", Slug = "floor", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 4, MinValue = -2, MaxValue = 50 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Year Built", Slug = "year-built", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 5, MinValue = 1800, MaxValue = 2026 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Parking", Slug = "parking", Type = AttributeType.Boolean, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 6 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Balcony", Slug = "balcony", Type = AttributeType.Boolean, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 7 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Garden", Slug = "re-garden", Type = AttributeType.Boolean, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 8 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "re-price", Type = AttributeType.Number, CategoryId = realEstateId, IsFilterable = true, DisplayOrder = 9, Unit = "CHF", MinValue = 0, MaxValue = 50000000 }
            });
        }

        // ── Real Estate > For Rent ──
        if (categories.TryGetValue("real-estate-for-rent", out var rentId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Property Type", Slug = "rent-property-type", Type = AttributeType.Select, CategoryId = rentId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Apartment", "House", "Studio", "Room", "Commercial", "Parking" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Rooms", Slug = "rent-rooms", Type = AttributeType.Number, CategoryId = rentId, IsFilterable = true, IsRequired = true, DisplayOrder = 2, MinValue = 1, MaxValue = 20 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Area", Slug = "rent-area", Type = AttributeType.Number, CategoryId = rentId, IsFilterable = true, DisplayOrder = 3, Unit = "m²" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Monthly Rent", Slug = "monthly-rent", Type = AttributeType.Number, CategoryId = rentId, IsFilterable = true, DisplayOrder = 4, Unit = "CHF/month" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Furnished", Slug = "furnished", Type = AttributeType.Boolean, CategoryId = rentId, IsFilterable = true, DisplayOrder = 5 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Pets Allowed", Slug = "pets-allowed", Type = AttributeType.Boolean, CategoryId = rentId, IsFilterable = true, DisplayOrder = 6 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Available From", Slug = "available-from", Type = AttributeType.Date, CategoryId = rentId, IsFilterable = true, DisplayOrder = 7 }
            });
        }

        // ── Computing > Laptops ──
        if (categories.TryGetValue("laptops", out var laptopsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Brand", Slug = "laptop-brand", Type = AttributeType.Select, CategoryId = laptopsId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Apple", "Asus", "Dell", "HP", "Lenovo", "Microsoft", "MSI", "Razer", "Samsung", "Acer" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Processor", Slug = "processor", Type = AttributeType.Select, CategoryId = laptopsId, IsFilterable = true, DisplayOrder = 2, Options = JsonSerializer.Serialize(new[] { "Intel Core i3", "Intel Core i5", "Intel Core i7", "Intel Core i9", "AMD Ryzen 5", "AMD Ryzen 7", "AMD Ryzen 9", "Apple M1", "Apple M2", "Apple M3", "Apple M4" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "RAM", Slug = "ram", Type = AttributeType.Select, CategoryId = laptopsId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "4GB", "8GB", "16GB", "32GB", "64GB" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Storage", Slug = "laptop-storage", Type = AttributeType.Select, CategoryId = laptopsId, IsFilterable = true, DisplayOrder = 4, Options = JsonSerializer.Serialize(new[] { "128GB SSD", "256GB SSD", "512GB SSD", "1TB SSD", "2TB SSD", "1TB HDD", "2TB HDD" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Screen Size", Slug = "screen-size", Type = AttributeType.Select, CategoryId = laptopsId, IsFilterable = true, DisplayOrder = 5, Options = JsonSerializer.Serialize(new[] { "11\"", "13\"", "14\"", "15\"", "16\"", "17\"" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "laptop-condition", Type = AttributeType.Select, CategoryId = laptopsId, IsFilterable = true, DisplayOrder = 6, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair", "For Parts" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "laptop-price", Type = AttributeType.Number, CategoryId = laptopsId, IsFilterable = true, DisplayOrder = 7, Unit = "CHF" }
            });
        }

        // ── Telephony > Mobile Phones ──
        if (categories.TryGetValue("mobile-phones", out var phonesId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Brand", Slug = "phone-brand", Type = AttributeType.Select, CategoryId = phonesId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Apple", "Samsung", "Google", "Huawei", "Xiaomi", "OnePlus", "Sony", "Nokia", "Motorola", "Nothing", "Oppo" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Storage", Slug = "phone-storage", Type = AttributeType.Select, CategoryId = phonesId, IsFilterable = true, DisplayOrder = 2, Options = JsonSerializer.Serialize(new[] { "32GB", "64GB", "128GB", "256GB", "512GB", "1TB" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "phone-condition", Type = AttributeType.Select, CategoryId = phonesId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair", "For Parts" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "phone-price", Type = AttributeType.Number, CategoryId = phonesId, IsFilterable = true, DisplayOrder = 4, Unit = "CHF" }
            });
        }

        // ── Home & Garden > Furniture ──
        if (categories.TryGetValue("furniture", out var furnitureId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Type", Slug = "furniture-type", Type = AttributeType.Select, CategoryId = furnitureId, IsFilterable = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Sofa", "Table", "Chair", "Bed", "Wardrobe", "Shelf", "Desk", "Cabinet", "Dresser", "Outdoor" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Material", Slug = "material", Type = AttributeType.Select, CategoryId = furnitureId, IsFilterable = true, DisplayOrder = 2, Options = JsonSerializer.Serialize(new[] { "Wood", "Metal", "Glass", "Fabric", "Leather", "Plastic", "Rattan" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "furniture-condition", Type = AttributeType.Select, CategoryId = furnitureId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "furniture-price", Type = AttributeType.Number, CategoryId = furnitureId, IsFilterable = true, DisplayOrder = 4, Unit = "CHF" }
            });
        }

        // ── Employment > Full-time ──
        if (categories.TryGetValue("full-time", out var jobsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Industry", Slug = "industry", Type = AttributeType.Select, CategoryId = jobsId, IsFilterable = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "IT & Software", "Finance & Banking", "Healthcare", "Engineering", "Marketing", "Sales", "Education", "Legal", "Hospitality", "Construction", "Retail", "Logistics", "Pharmaceutical", "Public Administration" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Salary Range", Slug = "salary", Type = AttributeType.Number, CategoryId = jobsId, IsFilterable = true, DisplayOrder = 2, Unit = "CHF/year" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Experience Level", Slug = "experience", Type = AttributeType.Select, CategoryId = jobsId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "Entry Level", "Mid Level", "Senior", "Lead", "Executive" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Work Model", Slug = "work-model", Type = AttributeType.Select, CategoryId = jobsId, IsFilterable = true, DisplayOrder = 4, Options = JsonSerializer.Serialize(new[] { "On-site", "Remote", "Hybrid" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Contract Type", Slug = "contract-type", Type = AttributeType.Select, CategoryId = jobsId, IsFilterable = true, DisplayOrder = 5, Options = JsonSerializer.Serialize(new[] { "Permanent", "Fixed-term", "Temporary" }) }
            });
        }

        // ── Jewelry & Watches > Watches ──
        if (categories.TryGetValue("watches", out var watchesId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Brand", Slug = "watch-brand", Type = AttributeType.Select, CategoryId = watchesId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Rolex", "Omega", "Tag Heuer", "Breitling", "Patek Philippe", "Audemars Piguet", "IWC", "Longines", "Tissot", "Swatch", "Casio", "Seiko", "Cartier", "Hublot" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Movement", Slug = "movement", Type = AttributeType.Select, CategoryId = watchesId, IsFilterable = true, DisplayOrder = 2, Options = JsonSerializer.Serialize(new[] { "Automatic", "Manual", "Quartz", "Solar", "Smartwatch" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Material", Slug = "watch-material", Type = AttributeType.Select, CategoryId = watchesId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "Stainless Steel", "Gold", "Titanium", "Ceramic", "Plastic", "Carbon" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "watch-condition", Type = AttributeType.Select, CategoryId = watchesId, IsFilterable = true, DisplayOrder = 4, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair", "Vintage" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "watch-price", Type = AttributeType.Number, CategoryId = watchesId, IsFilterable = true, DisplayOrder = 5, Unit = "CHF" }
            });
        }

        // ── Sports & Leisure > Winter Sports ──
        if (categories.TryGetValue("winter-sports", out var winterSportsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Type", Slug = "winter-type", Type = AttributeType.Select, CategoryId = winterSportsId, IsFilterable = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Skis", "Snowboard", "Ski Boots", "Snowboard Boots", "Bindings", "Poles", "Helmet", "Goggles", "Clothing" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Size", Slug = "winter-size", Type = AttributeType.Text, CategoryId = winterSportsId, IsFilterable = false, DisplayOrder = 2 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "winter-condition", Type = AttributeType.Select, CategoryId = winterSportsId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "winter-price", Type = AttributeType.Number, CategoryId = winterSportsId, IsFilterable = true, DisplayOrder = 4, Unit = "CHF" }
            });
        }

        // ── Sports & Leisure > Cycling ──
        if (categories.TryGetValue("cycling", out var cyclingId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Type", Slug = "bike-type", Type = AttributeType.Select, CategoryId = cyclingId, IsFilterable = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Road Bike", "Mountain Bike", "E-Bike", "City Bike", "Gravel Bike", "BMX", "Folding Bike", "Cargo Bike" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Frame Size", Slug = "frame-size", Type = AttributeType.Select, CategoryId = cyclingId, IsFilterable = true, DisplayOrder = 2, Options = JsonSerializer.Serialize(new[] { "XS", "S", "M", "L", "XL" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Brand", Slug = "bike-brand", Type = AttributeType.Select, CategoryId = cyclingId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "BMC", "Canyon", "Cannondale", "Cube", "Giant", "Merida", "Scott", "Specialized", "Trek", "Bianchi" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "bike-condition", Type = AttributeType.Select, CategoryId = cyclingId, IsFilterable = true, DisplayOrder = 4, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "bike-price", Type = AttributeType.Number, CategoryId = cyclingId, IsFilterable = true, DisplayOrder = 5, Unit = "CHF" }
            });
        }

        // ── Animals > Dogs ──
        if (categories.TryGetValue("dogs", out var dogsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Breed", Slug = "dog-breed", Type = AttributeType.Text, CategoryId = dogsId, IsFilterable = true, DisplayOrder = 1 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Age", Slug = "dog-age", Type = AttributeType.Text, CategoryId = dogsId, IsFilterable = false, DisplayOrder = 2 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Gender", Slug = "dog-gender", Type = AttributeType.Select, CategoryId = dogsId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "Male", "Female" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Vaccinated", Slug = "dog-vaccinated", Type = AttributeType.Boolean, CategoryId = dogsId, IsFilterable = true, DisplayOrder = 4 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "dog-price", Type = AttributeType.Number, CategoryId = dogsId, IsFilterable = true, DisplayOrder = 5, Unit = "CHF" }
            });
        }

        // ── Music & Instruments > Guitars ──
        if (categories.TryGetValue("guitars", out var guitarsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Type", Slug = "guitar-type", Type = AttributeType.Select, CategoryId = guitarsId, IsFilterable = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Acoustic", "Electric", "Classical", "Bass", "Electro-Acoustic" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Brand", Slug = "guitar-brand", Type = AttributeType.Select, CategoryId = guitarsId, IsFilterable = true, DisplayOrder = 2, Options = JsonSerializer.Serialize(new[] { "Fender", "Gibson", "Martin", "Taylor", "Ibanez", "Yamaha", "PRS", "Epiphone", "Squier" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "guitar-condition", Type = AttributeType.Select, CategoryId = guitarsId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair", "Vintage" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "guitar-price", Type = AttributeType.Number, CategoryId = guitarsId, IsFilterable = true, DisplayOrder = 4, Unit = "CHF" }
            });
        }

        // ── Weapons > Firearms ──
        if (categories.TryGetValue("firearms", out var firearmsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Type", Slug = "firearm-type", Type = AttributeType.Select, CategoryId = firearmsId, IsFilterable = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Rifle", "Shotgun", "Pistol", "Revolver", "Sport Rifle", "Hunting Rifle" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Caliber", Slug = "caliber", Type = AttributeType.Text, CategoryId = firearmsId, IsFilterable = true, DisplayOrder = 2 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Permit Required", Slug = "permit-required", Type = AttributeType.Boolean, CategoryId = firearmsId, IsFilterable = true, DisplayOrder = 3 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "firearm-condition", Type = AttributeType.Select, CategoryId = firearmsId, IsFilterable = true, DisplayOrder = 4, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "firearm-price", Type = AttributeType.Number, CategoryId = firearmsId, IsFilterable = true, DisplayOrder = 5, Unit = "CHF" }
            });
        }

        // ── Video Games > Consoles ──
        if (categories.TryGetValue("consoles", out var consolesId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Platform", Slug = "console-platform", Type = AttributeType.Select, CategoryId = consolesId, IsFilterable = true, IsRequired = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "PlayStation 5", "PlayStation 4", "Xbox Series X/S", "Xbox One", "Nintendo Switch", "Nintendo Switch 2", "Steam Deck", "Retro" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Storage", Slug = "console-storage", Type = AttributeType.Select, CategoryId = consolesId, IsFilterable = true, DisplayOrder = 2, Options = JsonSerializer.Serialize(new[] { "256GB", "512GB", "825GB", "1TB", "2TB" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "console-condition", Type = AttributeType.Select, CategoryId = consolesId, IsFilterable = true, DisplayOrder = 3, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "console-price", Type = AttributeType.Number, CategoryId = consolesId, IsFilterable = true, DisplayOrder = 4, Unit = "CHF" }
            });
        }

        // ── Nautical > Motorboats ──
        if (categories.TryGetValue("motorboats", out var motorboatsId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Length", Slug = "boat-length", Type = AttributeType.Number, CategoryId = motorboatsId, IsFilterable = true, DisplayOrder = 1, Unit = "m" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Engine Power", Slug = "boat-engine", Type = AttributeType.Number, CategoryId = motorboatsId, IsFilterable = true, DisplayOrder = 2, Unit = "HP" },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Year", Slug = "boat-year", Type = AttributeType.Number, CategoryId = motorboatsId, IsFilterable = true, DisplayOrder = 3, MinValue = 1960, MaxValue = 2026 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Condition", Slug = "boat-condition", Type = AttributeType.Select, CategoryId = motorboatsId, IsFilterable = true, DisplayOrder = 4, Options = JsonSerializer.Serialize(new[] { "New", "Like New", "Good", "Fair" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "boat-price", Type = AttributeType.Number, CategoryId = motorboatsId, IsFilterable = true, DisplayOrder = 5, Unit = "CHF" }
            });
        }

        // ── Wine & Gastronomy > Wine ──
        if (categories.TryGetValue("wine", out var wineId))
        {
            attrs.AddRange(new[]
            {
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Type", Slug = "wine-type", Type = AttributeType.Select, CategoryId = wineId, IsFilterable = true, DisplayOrder = 1, Options = JsonSerializer.Serialize(new[] { "Red", "White", "Rosé", "Sparkling", "Dessert" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Region", Slug = "wine-region", Type = AttributeType.Text, CategoryId = wineId, IsFilterable = true, DisplayOrder = 2 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Vintage", Slug = "vintage", Type = AttributeType.Number, CategoryId = wineId, IsFilterable = true, DisplayOrder = 3, MinValue = 1950, MaxValue = 2026 },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Volume", Slug = "volume", Type = AttributeType.Select, CategoryId = wineId, IsFilterable = true, DisplayOrder = 4, Options = JsonSerializer.Serialize(new[] { "75cl", "150cl (Magnum)", "50cl", "37.5cl" }) },
                new AttributeDefinition { Id = Guid.NewGuid(), Name = "Price", Slug = "wine-price", Type = AttributeType.Number, CategoryId = wineId, IsFilterable = true, DisplayOrder = 5, Unit = "CHF" }
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
        var freeId = Guid.NewGuid();
        var standardId = Guid.NewGuid();
        var premiumId = Guid.NewGuid();
        var businessId = Guid.NewGuid();

        var tiers = new List<SubscriptionTier>
        {
            new()
            {
                Id = freeId, Name = "Free", Description = "Basic listing with limited features",
                MonthlyPrice = 0m, AnnualPrice = 0m, IsActive = true, DisplayOrder = 1
            },
            new()
            {
                Id = standardId, Name = "Standard", Description = "Enhanced visibility and more listings",
                MonthlyPrice = 9.90m, AnnualPrice = 99.00m, IsActive = true, DisplayOrder = 2
            },
            new()
            {
                Id = premiumId, Name = "Premium", Description = "Priority listing and advanced analytics",
                MonthlyPrice = 29.90m, AnnualPrice = 299.00m, IsActive = true, DisplayOrder = 3
            },
            new()
            {
                Id = businessId, Name = "Business", Description = "Full professional suite with KPI dashboard and unlimited listings",
                MonthlyPrice = 79.90m, AnnualPrice = 799.00m, IsActive = true, DisplayOrder = 4
            }
        };
        await context.SubscriptionTiers.AddRangeAsync(tiers);

        var features = new List<SubscriptionFeature>
        {
            // Free
            new() { Id = Guid.NewGuid(), SubscriptionTierId = freeId, Name = "Max Listings", Value = "3", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = freeId, Name = "Max Photos Per Listing", Value = "3", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = freeId, Name = "Featured Listing", Value = "false", IsEnabled = false },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = freeId, Name = "Analytics Dashboard", Value = "false", IsEnabled = false },
            // Standard
            new() { Id = Guid.NewGuid(), SubscriptionTierId = standardId, Name = "Max Listings", Value = "15", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = standardId, Name = "Max Photos Per Listing", Value = "10", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = standardId, Name = "Featured Listing", Value = "false", IsEnabled = false },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = standardId, Name = "Analytics Dashboard", Value = "basic", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = standardId, Name = "Priority Support", Value = "false", IsEnabled = false },
            // Premium
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Max Listings", Value = "50", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Max Photos Per Listing", Value = "20", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Featured Listing", Value = "true", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Analytics Dashboard", Value = "advanced", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Priority Support", Value = "true", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = premiumId, Name = "Boost Credits Monthly", Value = "3", IsEnabled = true },
            // Business
            new() { Id = Guid.NewGuid(), SubscriptionTierId = businessId, Name = "Max Listings", Value = "unlimited", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = businessId, Name = "Max Photos Per Listing", Value = "30", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = businessId, Name = "Featured Listing", Value = "true", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = businessId, Name = "Analytics Dashboard", Value = "professional", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = businessId, Name = "Priority Support", Value = "true", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = businessId, Name = "Boost Credits Monthly", Value = "10", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = businessId, Name = "KPI Dashboard", Value = "true", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = businessId, Name = "Export Reports", Value = "true", IsEnabled = true },
            new() { Id = Guid.NewGuid(), SubscriptionTierId = businessId, Name = "API Access", Value = "true", IsEnabled = true }
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
