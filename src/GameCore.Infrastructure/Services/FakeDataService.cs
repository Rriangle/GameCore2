using GameCore.Domain.Entities;
using GameCore.Domain.Interfaces;
using GameCore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameCore.Infrastructure.Services
{
    public class FakeDataService : IFakeDataService
    {
        private readonly GameCoreDbContext _context;
        private readonly Random _random;

        public FakeDataService(GameCoreDbContext context)
        {
            _context = context;
            _random = new Random();
        }

        public async Task GenerateFakeDataAsync()
        {
            await GenerateUsersAsync();
            await GenerateUserProfilesAsync();
            await GenerateUserRightsAsync();
            await GenerateUserWalletsAsync();
            await GenerateMemberSalesProfilesAsync();
            await GenerateUserSalesInformationAsync();
            await GenerateManagerDataAsync();
            await GenerateManagerRolePermissionsAsync();
            await GenerateManagerRolesAsync();
            await GenerateAdminsAsync();
            await GenerateProductCategoriesAsync();
            await GenerateProductsAsync();
            await GenerateOrdersAsync();
            await GeneratePlayerMarketListingsAsync();
            await GeneratePlayerMarketOrdersAsync();
            
            // Stage 3: 熱度與洞察相關假資料
            await GenerateGamesAsync();
            await GenerateMetricSourcesAsync();
            await GenerateMetricsAsync();
            await GenerateGameSourceMapsAsync();
            await GenerateGameMetricDailiesAsync();
            await GeneratePopularityIndexDailiesAsync();
            await GenerateLeaderboardSnapshotsAsync();
            await GenerateForumsAsync();
            await GenerateThreadsAsync();
            await GenerateThreadPostsAsync();
            await GeneratePostsAsync();
            await GeneratePostMetricSnapshotsAsync();
            await GeneratePostSourcesAsync();
            await GenerateReactionsAsync();
            await GenerateBookmarksAsync();
        }

        private async Task GenerateUsersAsync()
        {
            if (await _context.Users.AnyAsync())
                return;

            var users = new List<User>();
            for (int i = 1; i <= 50; i++)
            {
                users.Add(new User
                {
                    Username = $"user{i:D3}",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Email = $"user{i:D3}@example.com",
                    FirstName = GetRandomFirstName(),
                    LastName = GetRandomLastName(),
                    DateOfBirth = DateTime.Now.AddYears(-_random.Next(18, 65)),
                    Gender = (Gender)_random.Next(3),
                    PhoneNumber = $"+1{_random.Next(100, 999)}{_random.Next(100, 999)}{_random.Next(1000, 9999)}",
                    IsActive = _random.Next(100) < 90, // 90% active
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                    LastLoginAt = DateTime.UtcNow.AddDays(-_random.Next(0, 30))
                });
            }

            _context.Users.AddRange(users);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateUserProfilesAsync()
        {
            if (await _context.UserIntroduces.AnyAsync())
                return;

            var users = await _context.Users.ToListAsync();
            var profiles = new List<UserIntroduce>();

            foreach (var user in users)
            {
                profiles.Add(new UserIntroduce
                {
                    UserId = user.Id,
                    Bio = GetRandomBio(),
                    AvatarUrl = GetRandomAvatarUrl(),
                    Location = GetRandomLocation(),
                    Website = _random.Next(100) < 30 ? GetRandomWebsite() : null,
                    SocialMediaLinks = GetRandomSocialMediaLinks(),
                    Interests = GetRandomInterests(),
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _context.UserIntroduces.AddRange(profiles);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateUserRightsAsync()
        {
            if (await _context.UserRights.AnyAsync())
                return;

            var users = await _context.Users.ToListAsync();
            var rights = new List<UserRights>();

            foreach (var user in users)
            {
                rights.Add(new UserRights
                {
                    UserId = user.Id,
                    CanPost = _random.Next(100) < 80,
                    CanComment = _random.Next(100) < 90,
                    CanUploadMedia = _random.Next(100) < 70,
                    CanCreateEvents = _random.Next(100) < 40,
                    CanModerate = _random.Next(100) < 10,
                    IsVerified = _random.Next(100) < 20,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _context.UserRights.AddRange(rights);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateUserWalletsAsync()
        {
            if (await _context.UserWallets.AnyAsync())
                return;

            var users = await _context.Users.ToListAsync();
            var wallets = new List<UserWallet>();

            foreach (var user in users)
            {
                wallets.Add(new UserWallet
                {
                    UserId = user.Id,
                    Balance = _random.Next(0, 10000),
                    Points = _random.Next(0, 50000),
                    LastTransactionAt = DateTime.UtcNow.AddDays(-_random.Next(0, 30)),
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _context.UserWallets.AddRange(wallets);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateMemberSalesProfilesAsync()
        {
            if (await _context.MemberSalesProfiles.AnyAsync())
                return;

            var users = await _context.Users.ToListAsync();
            var salesProfiles = new List<MemberSalesProfile>();

            foreach (var user in users.Take(20)) // Only 20 users have sales profiles
            {
                salesProfiles.Add(new MemberSalesProfile
                {
                    UserId = user.Id,
                    BusinessName = GetRandomBusinessName(),
                    BusinessDescription = GetRandomBusinessDescription(),
                    ContactEmail = user.Email,
                    ContactPhone = user.PhoneNumber,
                    BusinessAddress = GetRandomBusinessAddress(),
                    BusinessLicense = GetRandomBusinessLicense(),
                    TaxId = GetRandomTaxId(),
                    Status = (SalesProfileStatus)_random.Next(4),
                    CreatedAt = user.CreatedAt.AddDays(_random.Next(1, 100)),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _context.MemberSalesProfiles.AddRange(salesProfiles);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateUserSalesInformationAsync()
        {
            if (await _context.UserSalesInformation.AnyAsync())
                return;

            var salesProfiles = await _context.MemberSalesProfiles
                .Where(sp => sp.Status == SalesProfileStatus.Approved)
                .ToListAsync();

            var salesInfo = new List<UserSalesInformation>();

            foreach (var profile in salesProfiles)
            {
                salesInfo.Add(new UserSalesInformation
                {
                    UserId = profile.UserId,
                    SalesBalance = _random.Next(0, 5000),
                    TotalSales = _random.Next(1000, 50000),
                    CommissionRate = _random.Next(5, 15),
                    CreatedAt = profile.CreatedAt.AddDays(_random.Next(1, 30)),
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _context.UserSalesInformation.AddRange(salesInfo);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateManagerDataAsync()
        {
            if (await _context.ManagerData.AnyAsync())
                return;

            var managers = new List<ManagerData>();
            for (int i = 1; i <= 10; i++)
            {
                managers.Add(new ManagerData
                {
                    Username = $"manager{i:D2}",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("manager123"),
                    Email = $"manager{i:D2}@gamecore.com",
                    FirstName = GetRandomFirstName(),
                    LastName = GetRandomLastName(),
                    Department = GetRandomDepartment(),
                    IsActive = _random.Next(100) < 90,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 365)),
                    LastLoginAt = DateTime.UtcNow.AddDays(-_random.Next(0, 30))
                });
            }

            _context.ManagerData.AddRange(managers);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateManagerRolePermissionsAsync()
        {
            if (await _context.ManagerRolePermissions.AnyAsync())
                return;

            var permissions = new List<ManagerRolePermission>
            {
                new ManagerRolePermission { RoleName = "SuperAdmin", CanManageUsers = true, CanManageProducts = true, CanManageOrders = true, CanManageReports = true, CanManageSystem = true },
                new ManagerRolePermission { RoleName = "Admin", CanManageUsers = true, CanManageProducts = true, CanManageOrders = true, CanManageReports = true, CanManageSystem = false },
                new ManagerRolePermission { RoleName = "Moderator", CanManageUsers = true, CanManageProducts = false, CanManageOrders = false, CanManageReports = true, CanManageSystem = false },
                new ManagerRolePermission { RoleName = "Support", CanManageUsers = false, CanManageProducts = false, CanManageOrders = true, CanManageReports = false, CanManageSystem = false }
            };

            _context.ManagerRolePermissions.AddRange(permissions);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateManagerRolesAsync()
        {
            if (await _context.ManagerRoles.AnyAsync())
                return;

            var managers = await _context.ManagerData.ToListAsync();
            var permissions = await _context.ManagerRolePermissions.ToListAsync();
            var roles = new List<ManagerRole>();

            foreach (var manager in managers)
            {
                var permission = permissions[_random.Next(permissions.Count)];
                roles.Add(new ManagerRole
                {
                    ManagerId = manager.Id,
                    RoleName = permission.RoleName,
                    AssignedAt = manager.CreatedAt.AddDays(_random.Next(1, 30)),
                    AssignedBy = 1 // Super admin
                });
            }

            _context.ManagerRoles.AddRange(roles);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateAdminsAsync()
        {
            if (await _context.Admins.AnyAsync())
                return;

            var admins = new List<Admins>
            {
                new Admins
                {
                    Username = "superadmin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("superadmin123"),
                    Email = "superadmin@gamecore.com",
                    Role = "SuperAdmin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-365)
                }
            };

            _context.Admins.AddRange(admins);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateProductCategoriesAsync()
        {
            if (await _context.ProductCategories.AnyAsync())
                return;

            var categories = new List<ProductCategory>
            {
                new ProductCategory { Name = "Gaming Hardware", Description = "Gaming peripherals and hardware", IconUrl = "/icons/hardware.png", SortOrder = 1, IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { Name = "Gaming Apparel", Description = "Gaming-themed clothing and accessories", IconUrl = "/icons/apparel.png", SortOrder = 2, IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { Name = "Collectibles", Description = "Gaming collectibles and merchandise", IconUrl = "/icons/collectibles.png", SortOrder = 3, IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { Name = "Software", Description = "Gaming software and digital products", IconUrl = "/icons/software.png", SortOrder = 4, IsActive = true, CreatedAt = DateTime.UtcNow },
                new ProductCategory { Name = "Accessories", Description = "Gaming accessories and peripherals", IconUrl = "/icons/accessories.png", SortOrder = 5, IsActive = true, CreatedAt = DateTime.UtcNow }
            };

            _context.ProductCategories.AddRange(categories);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateProductsAsync()
        {
            if (await _context.Products.AnyAsync())
                return;

            var categories = await _context.ProductCategories.ToListAsync();
            var products = new List<Product>();

            // Gaming Hardware
            var hardwareCategory = categories.First(c => c.Name == "Gaming Hardware");
            products.AddRange(new[]
            {
                new Product { Name = "Pro Gaming Mouse", Description = "High-precision gaming mouse with customizable DPI", Price = 79.99m, StockQuantity = 100, CategoryId = hardwareCategory.Id, ImageUrl = "/images/mouse.jpg", IsOfficialStore = true, CreatedBy = 1, CreatedAt = DateTime.UtcNow },
                new Product { Name = "Mechanical Keyboard", Description = "RGB mechanical keyboard with Cherry MX switches", Price = 149.99m, StockQuantity = 75, CategoryId = hardwareCategory.Id, ImageUrl = "/images/keyboard.jpg", IsOfficialStore = true, CreatedBy = 1, CreatedAt = DateTime.UtcNow },
                new Product { Name = "Gaming Headset", Description = "7.1 surround sound gaming headset", Price = 89.99m, StockQuantity = 120, CategoryId = hardwareCategory.Id, ImageUrl = "/images/headset.jpg", IsOfficialStore = true, CreatedBy = 1, CreatedAt = DateTime.UtcNow }
            });

            // Gaming Apparel
            var apparelCategory = categories.First(c => c.Name == "Gaming Apparel");
            products.AddRange(new[]
            {
                new Product { Name = "Gaming T-Shirt", Description = "Comfortable cotton gaming t-shirt", Price = 24.99m, StockQuantity = 200, CategoryId = apparelCategory.Id, ImageUrl = "/images/tshirt.jpg", IsOfficialStore = true, CreatedBy = 1, CreatedAt = DateTime.UtcNow },
                new Product { Name = "Gaming Hoodie", Description = "Warm gaming hoodie with logo", Price = 49.99m, StockQuantity = 150, CategoryId = apparelCategory.Id, ImageUrl = "/images/hoodie.jpg", IsOfficialStore = true, CreatedBy = 1, CreatedAt = DateTime.UtcNow }
            });

            // Collectibles
            var collectiblesCategory = categories.First(c => c.Name == "Collectibles");
            products.AddRange(new[]
            {
                new Product { Name = "Limited Edition Figure", Description = "Exclusive gaming character figure", Price = 199.99m, StockQuantity = 50, CategoryId = collectiblesCategory.Id, ImageUrl = "/images/figure.jpg", IsOfficialStore = true, CreatedBy = 1, CreatedAt = DateTime.UtcNow },
                new Product { Name = "Art Book", Description = "Collector's edition art book", Price = 39.99m, StockQuantity = 100, CategoryId = collectiblesCategory.Id, ImageUrl = "/images/artbook.jpg", IsOfficialStore = true, CreatedBy = 1, CreatedAt = DateTime.UtcNow }
            });

            _context.Products.AddRange(products);
            await _context.SaveChangesAsync();
        }

        private async Task GenerateOrdersAsync()
        {
            if (await _context.Orders.AnyAsync())
                return;

            var users = await _context.Users.Take(20).ToListAsync();
            var products = await _context.Products.ToListAsync();
            var orders = new List<Order>();

            foreach (var user in users)
            {
                var orderCount = _random.Next(1, 4);
                for (int i = 0; i < orderCount; i++)
                {
                    var orderItems = new List<CreateOrderItemDto>();
                    var itemCount = _random.Next(1, 4);
                    var totalAmount = 0m;

                    for (int j = 0; j < itemCount; j++)
                    {
                        var product = products[_random.Next(products.Count)];
                        var quantity = _random.Next(1, 4);
                        orderItems.Add(new CreateOrderItemDto
                        {
                            ProductId = product.Id,
                            Quantity = quantity
                        });
                        totalAmount += product.Price * quantity;
                    }

                    var taxAmount = totalAmount * 0.1m;
                    var shippingAmount = totalAmount > 100 ? 0 : 10;
                    var finalAmount = totalAmount + taxAmount + shippingAmount;

                    var order = new Order
                    {
                        OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                        UserId = user.Id,
                        TotalAmount = totalAmount,
                        TaxAmount = taxAmount,
                        ShippingAmount = shippingAmount,
                        FinalAmount = finalAmount,
                        Status = (OrderStatus)_random.Next(7),
                        PaymentStatus = (PaymentStatus)_random.Next(5),
                        ShippingAddress = GetRandomAddress(),
                        ShippingCity = GetRandomCity(),
                        ShippingPostalCode = GetRandomPostalCode(),
                        ShippingCountry = "United States",
                        ContactPhone = user.PhoneNumber,
                        Notes = _random.Next(100) < 30 ? GetRandomOrderNote() : null,
                        CreatedAt = user.CreatedAt.AddDays(_random.Next(1, 100)),
                        UpdatedAt = DateTime.UtcNow
                    };

                    orders.Add(order);
                }
            }

            _context.Orders.AddRange(orders);
            await _context.SaveChangesAsync();

            // Generate order items
            await GenerateOrderItemsAsync(orders, products);
        }

        private async Task GenerateOrderItemsAsync(List<Order> orders, List<Product> products)
        {
            var orderItems = new List<OrderItem>();

            foreach (var order in orders)
            {
                var orderDto = new CreateOrderDto
                {
                    UserId = order.UserId,
                    OrderItems = new List<CreateOrderItemDto>
                    {
                        new CreateOrderItemDto
                        {
                            ProductId = products[_random.Next(products.Count)].Id,
                            Quantity = _random.Next(1, 4)
                        }
                    }
                };

                // This is a simplified version - in real implementation, you'd use the service
                var product = products.First(p => p.Id == orderDto.OrderItems[0].ProductId);
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Quantity = orderDto.OrderItems[0].Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * orderDto.OrderItems[0].Quantity,
                    ProductImageUrl = product.ImageUrl,
                    CreatedAt = order.CreatedAt
                };

                orderItems.Add(orderItem);
            }

            _context.OrderItems.AddRange(orderItems);
            await _context.SaveChangesAsync();
        }

        private async Task GeneratePlayerMarketListingsAsync()
        {
            if (await _context.PlayerMarketListings.AnyAsync())
                return;

            var users = await _context.Users.Take(15).ToListAsync();
            var listings = new List<PlayerMarketListing>();

            foreach (var user in users)
            {
                var listingCount = _random.Next(1, 4);
                for (int i = 0; i < listingCount; i++)
                {
                    var listing = new PlayerMarketListing
                    {
                        SellerId = user.Id,
                        Title = GetRandomListingTitle(),
                        Description = GetRandomListingDescription(),
                        Price = _random.Next(10, 500),
                        Quantity = _random.Next(1, 10),
                        AvailableQuantity = _random.Next(1, 10),
                        Status = (ListingStatus)_random.Next(5),
                        ImageUrl = GetRandomImageUrl(),
                        IsNegotiable = _random.Next(100) < 50,
                        ExpiresAt = DateTime.UtcNow.AddDays(_random.Next(7, 90)),
                        CreatedAt = user.CreatedAt.AddDays(_random.Next(1, 50)),
                        UpdatedAt = DateTime.UtcNow
                    };

                    listings.Add(listing);
                }
            }

            _context.PlayerMarketListings.AddRange(listings);
            await _context.SaveChangesAsync();
        }

        private async Task GeneratePlayerMarketOrdersAsync()
        {
            if (await _context.PlayerMarketOrders.AnyAsync())
                return;

            var listings = await _context.PlayerMarketListings.Where(l => l.Status == ListingStatus.Active).ToListAsync();
            var users = await _context.Users.Take(20).ToListAsync();
            var orders = new List<PlayerMarketOrder>();

            foreach (var listing in listings.Take(10)) // Generate orders for 10 listings
            {
                var buyer = users.First(u => u.Id != listing.SellerId);
                var totalAmount = listing.Price * 1; // Assume quantity 1 for simplicity
                var platformFee = totalAmount * 0.05m;
                var sellerAmount = totalAmount - platformFee;

                var order = new PlayerMarketOrder
                {
                    ListingId = listing.Id,
                    BuyerId = buyer.Id,
                    SellerId = listing.SellerId,
                    Quantity = 1,
                    UnitPrice = listing.Price,
                    TotalAmount = totalAmount,
                    PlatformFee = platformFee,
                    SellerAmount = sellerAmount,
                    Status = (PlayerMarketOrderStatus)_random.Next(7),
                    BuyerNotes = _random.Next(100) < 30 ? GetRandomOrderNote() : null,
                    SellerNotes = _random.Next(100) < 20 ? GetRandomOrderNote() : null,
                    CreatedAt = listing.CreatedAt.AddDays(_random.Next(1, 30)),
                    ConfirmedAt = _random.Next(100) < 70 ? DateTime.UtcNow.AddDays(-_random.Next(1, 10)) : null,
                    CompletedAt = _random.Next(100) < 50 ? DateTime.UtcNow.AddDays(-_random.Next(1, 5)) : null
                };

                orders.Add(order);
            }

            _context.PlayerMarketOrders.AddRange(orders);
            await _context.SaveChangesAsync();
        }

        // Helper methods for generating random data
        private string GetRandomFirstName() => new[] { "John", "Jane", "Mike", "Sarah", "David", "Lisa", "Tom", "Emma", "Chris", "Anna" }[_random.Next(10)];
        private string GetRandomLastName() => new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" }[_random.Next(10)];
        private string GetRandomBio() => new[] { "Gaming enthusiast", "Professional gamer", "Game developer", "Streamer", "Esports fan" }[_random.Next(5)];
        private string GetRandomAvatarUrl() => $"/avatars/avatar{_random.Next(1, 21)}.jpg";
        private string GetRandomLocation() => new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose" }[_random.Next(10)];
        private string GetRandomWebsite() => $"https://{GetRandomFirstName().ToLower()}{GetRandomLastName().ToLower()}.com";
        private string GetRandomSocialMediaLinks() => $"twitter:@{GetRandomFirstName().ToLower()}{GetRandomLastName().ToLower()}, instagram:{GetRandomFirstName().ToLower()}{GetRandomLastName().ToLower()}";
        private string GetRandomInterests() => string.Join(",", new[] { "FPS Games", "RPG Games", "Strategy Games", "MOBA Games", "Racing Games" }.OrderBy(x => _random.Next()).Take(3));
        private string GetRandomBusinessName() => $"{GetRandomFirstName()} {GetRandomLastName()} Gaming";
        private string GetRandomBusinessDescription() => "Professional gaming equipment and accessories retailer";
        private string GetRandomBusinessAddress() => $"{_random.Next(100, 9999)} {GetRandomFirstName()} St, {GetRandomCity()}";
        private string GetRandomBusinessLicense() => $"BL-{_random.Next(100000, 999999)}";
        private string GetRandomTaxId() => $"{_random.Next(10, 99)}-{_random.Next(1000000, 9999999)}";
        private string GetRandomDepartment() => new[] { "Sales", "Marketing", "Support", "Operations", "Development" }[_random.Next(5)];
        private string GetRandomAddress() => $"{_random.Next(100, 9999)} {GetRandomFirstName()} Ave";
        private string GetRandomCity() => new[] { "New York", "Los Angeles", "Chicago", "Houston", "Phoenix" }[_random.Next(5)];
        private string GetRandomPostalCode() => $"{_random.Next(10000, 99999)}";
        private string GetRandomOrderNote() => new[] { "Please deliver to front door", "Gift for my brother", "Need it by Friday", "Handle with care", "Leave with neighbor if not home" }[_random.Next(5)];
        private string GetRandomListingTitle() => new[] { "Gaming Mouse", "Mechanical Keyboard", "Gaming Headset", "Gaming Chair", "Gaming Monitor", "Gaming Laptop", "Gaming Console", "Gaming Accessories" }[_random.Next(8)];
        private string GetRandomListingDescription() => "Excellent condition, barely used. Perfect for gaming enthusiasts.";
        private string GetRandomImageUrl() => $"/images/listing{_random.Next(1, 11)}.jpg";

        // Stage 3: 熱度與洞察相關假資料生成方法

        /// <summary>
        /// 生成遊戲假資料
        /// </summary>
        private async Task GenerateGamesAsync()
        {
            if (await _context.Games.AnyAsync())
                return;

            var games = new List<Game>
            {
                new Game { game_id = 1, name = "英雄聯盟", genre = "MOBA", created_at = DateTime.UtcNow.AddYears(-2) },
                new Game { game_id = 2, name = "原神", genre = "RPG", created_at = DateTime.UtcNow.AddYears(-1) },
                new Game { game_id = 3, name = "Steam", genre = "Platform", created_at = DateTime.UtcNow.AddYears(-3) },
                new Game { game_id = 4, name = "Valorant", genre = "FPS", created_at = DateTime.UtcNow.AddYears(-1) },
                new Game { game_id = 5, name = "Minecraft", genre = "Sandbox", created_at = DateTime.UtcNow.AddYears(-4) },
                new Game { game_id = 6, name = "Fortnite", genre = "Battle Royale", created_at = DateTime.UtcNow.AddYears(-2) },
                new Game { game_id = 7, name = "GTA V", genre = "Action", created_at = DateTime.UtcNow.AddYears(-3) },
                new Game { game_id = 8, name = "Cyberpunk 2077", genre = "RPG", created_at = DateTime.UtcNow.AddYears(-1) },
                new Game { game_id = 9, name = "Call of Duty", genre = "FPS", created_at = DateTime.UtcNow.AddYears(-2) },
                new Game { game_id = 10, name = "FIFA 24", genre = "Sports", created_at = DateTime.UtcNow.AddYears(-1) },
                new Game { game_id = 11, name = "Red Dead Redemption 2", genre = "Action", created_at = DateTime.UtcNow.AddYears(-2) },
                new Game { game_id = 12, name = "The Witcher 3", genre = "RPG", created_at = DateTime.UtcNow.AddYears(-3) },
                new Game { game_id = 13, name = "Elden Ring", genre = "Action RPG", created_at = DateTime.UtcNow.AddYears(-1) },
                new Game { game_id = 14, name = "God of War", genre = "Action", created_at = DateTime.UtcNow.AddYears(-2) },
                new Game { game_id = 15, name = "Spider-Man", genre = "Action", created_at = DateTime.UtcNow.AddYears(-1) }
            };

            _context.Games.AddRange(games);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成指標來源假資料
        /// </summary>
        private async Task GenerateMetricSourcesAsync()
        {
            if (await _context.MetricSources.AnyAsync())
                return;

            var sources = new List<MetricSource>
            {
                new MetricSource { source_id = 1, name = "Steam", note = "Steam平台同時在線用戶數", created_at = DateTime.UtcNow.AddYears(-1) },
                new MetricSource { source_id = 2, name = "YouTube", note = "YouTube遊戲相關影片觀看數", created_at = DateTime.UtcNow.AddYears(-1) },
                new MetricSource { source_id = 3, name = "Twitch", note = "Twitch直播觀看人數", created_at = DateTime.UtcNow.AddYears(-1) },
                new MetricSource { source_id = 4, name = "Reddit", note = "Reddit遊戲版討論熱度", created_at = DateTime.UtcNow.AddYears(-1) },
                new MetricSource { source_id = 5, name = "Twitter", note = "Twitter遊戲相關話題", created_at = DateTime.UtcNow.AddYears(-1) },
                new MetricSource { source_id = 6, name = "Discord", note = "Discord遊戲伺服器活躍度", created_at = DateTime.UtcNow.AddYears(-1) },
                new MetricSource { source_id = 7, name = "Google Trends", note = "Google搜尋趨勢", created_at = DateTime.UtcNow.AddYears(-1) },
                new MetricSource { source_id = 8, name = "App Store", note = "App Store遊戲下載量", created_at = DateTime.UtcNow.AddYears(-1) }
            };

            _context.MetricSources.AddRange(sources);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成指標假資料
        /// </summary>
        private async Task GenerateMetricsAsync()
        {
            if (await _context.Metrics.AnyAsync())
                return;

            var metrics = new List<Metric>
            {
                new Metric { metric_id = 1, source_id = 1, code = "concurrent_users", unit = "users", description = "同時在線用戶數", is_active = true, created_at = DateTime.UtcNow.AddYears(-1) },
                new Metric { metric_id = 2, source_id = 2, code = "video_views", unit = "views", description = "影片觀看次數", is_active = true, created_at = DateTime.UtcNow.AddYears(-1) },
                new Metric { metric_id = 3, source_id = 3, code = "stream_viewers", unit = "viewers", description = "直播觀看人數", is_active = true, created_at = DateTime.UtcNow.AddYears(-1) },
                new Metric { metric_id = 4, source_id = 4, code = "forum_posts", unit = "posts", description = "論壇發文數", is_active = true, created_at = DateTime.UtcNow.AddYears(-1) },
                new Metric { metric_id = 5, source_id = 5, code = "social_mentions", unit = "mentions", description = "社群提及次數", is_active = true, created_at = DateTime.UtcNow.AddYears(-1) },
                new Metric { metric_id = 6, source_id = 6, code = "discord_activity", unit = "users", description = "Discord活躍用戶數", is_active = true, created_at = DateTime.UtcNow.AddYears(-1) },
                new Metric { metric_id = 7, source_id = 7, code = "search_trends", unit = "index", description = "搜尋趨勢指數", is_active = true, created_at = DateTime.UtcNow.AddYears(-1) },
                new Metric { metric_id = 8, source_id = 8, code = "downloads", unit = "downloads", description = "下載量", is_active = true, created_at = DateTime.UtcNow.AddYears(-1) }
            };

            _context.Metrics.AddRange(metrics);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成遊戲來源對應假資料
        /// </summary>
        private async Task GenerateGameSourceMapsAsync()
        {
            if (await _context.GameSourceMaps.AnyAsync())
                return;

            var maps = new List<GameSourceMap>
            {
                new GameSourceMap { id = 1, game_id = 1, source_id = 1, external_key = "league-of-legends" },
                new GameSourceMap { id = 2, game_id = 1, source_id = 2, external_key = "league-of-legends" },
                new GameSourceMap { id = 3, game_id = 2, source_id = 2, external_key = "genshin-impact" },
                new GameSourceMap { id = 4, game_id = 2, source_id = 8, external_key = "genshin-impact" },
                new GameSourceMap { id = 5, game_id = 3, source_id = 1, external_key = "steam" },
                new GameSourceMap { id = 6, game_id = 4, source_id = 1, external_key = "valorant" },
                new GameSourceMap { id = 7, game_id = 4, source_id = 2, external_key = "valorant" },
                new GameSourceMap { id = 8, game_id = 5, source_id = 1, external_key = "minecraft" },
                new GameSourceMap { id = 9, game_id = 5, source_id = 2, external_key = "minecraft" },
                new GameSourceMap { id = 10, game_id = 6, source_id = 1, external_key = "fortnite" },
                new GameSourceMap { id = 11, game_id = 6, source_id = 2, external_key = "fortnite" },
                new GameSourceMap { id = 12, game_id = 7, source_id = 1, external_key = "grand-theft-auto-v" },
                new GameSourceMap { id = 13, game_id = 8, source_id = 1, external_key = "cyberpunk-2077" },
                new GameSourceMap { id = 14, game_id = 9, source_id = 1, external_key = "call-of-duty" },
                new GameSourceMap { id = 15, game_id = 10, source_id = 1, external_key = "fifa-24" }
            };

            _context.GameSourceMaps.AddRange(maps);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成遊戲每日指標假資料
        /// </summary>
        private async Task GenerateGameMetricDailiesAsync()
        {
            if (await _context.GameMetricDailies.AnyAsync())
                return;

            var games = await _context.Games.ToListAsync();
            var metrics = await _context.Metrics.ToListAsync();
            var metricDailies = new List<GameMetricDaily>();

            var startDate = DateTime.Today.AddDays(-30); // 生成最近30天的數據

            foreach (var game in games)
            {
                foreach (var metric in metrics.Take(3)) // 每個遊戲取3個指標
                {
                    for (int i = 0; i < 30; i++)
                    {
                        var date = startDate.AddDays(i);
                        var baseValue = GetBaseMetricValue(metric.code);
                        var randomFactor = _random.Next(80, 121) / 100.0m; // 80%-120%的隨機變化
                        var value = baseValue * randomFactor;

                        metricDailies.Add(new GameMetricDaily
                        {
                            game_id = game.game_id,
                            metric_id = metric.metric_id,
                            date = date,
                            value = value,
                            agg_method = "max",
                            quality = "real",
                            created_at = date,
                            updated_at = date
                        });
                    }
                }
            }

            _context.GameMetricDailies.AddRange(metricDailies);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成每日熱度指數假資料
        /// </summary>
        private async Task GeneratePopularityIndexDailiesAsync()
        {
            if (await _context.PopularityIndexDailies.AnyAsync())
                return;

            var games = await _context.Games.ToListAsync();
            var metricDailies = await _context.GameMetricDailies.ToListAsync();
            var popularityIndices = new List<PopularityIndexDaily>();

            var startDate = DateTime.Today.AddDays(-30);

            foreach (var game in games)
            {
                for (int i = 0; i < 30; i++)
                {
                    var date = startDate.AddDays(i);
                    var gameMetrics = metricDailies.Where(m => m.game_id == game.game_id && m.date == date).ToList();
                    
                    if (gameMetrics.Any())
                    {
                        var indexValue = CalculatePopularityIndex(gameMetrics);
                        
                        popularityIndices.Add(new PopularityIndexDaily
                        {
                            game_id = game.game_id,
                            date = date,
                            index_value = indexValue,
                            created_at = date
                        });
                    }
                }
            }

            _context.PopularityIndexDailies.AddRange(popularityIndices);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成排行榜快照假資料
        /// </summary>
        private async Task GenerateLeaderboardSnapshotsAsync()
        {
            if (await _context.LeaderboardSnapshots.AnyAsync())
                return;

            var popularityIndices = await _context.PopularityIndexDailies.ToListAsync();
            var snapshots = new List<LeaderboardSnapshot>();

            // 生成週榜快照
            var weeklyDates = Enumerable.Range(0, 4).Select(i => DateTime.Today.AddDays(-i * 7)).ToList();
            
            foreach (var date in weeklyDates)
            {
                var weekData = popularityIndices.Where(p => p.date >= date.AddDays(-6) && p.date <= date).ToList();
                var gameAverages = weekData.GroupBy(p => p.game_id)
                    .Select(g => new { GameId = g.Key, AvgIndex = g.Average(p => p.index_value) })
                    .OrderByDescending(x => x.AvgIndex)
                    .Take(20)
                    .ToList();

                for (int rank = 0; rank < gameAverages.Count; rank++)
                {
                    snapshots.Add(new LeaderboardSnapshot
                    {
                        period = "weekly",
                        ts = date,
                        rank = rank + 1,
                        game_id = gameAverages[rank].GameId,
                        index_value = gameAverages[rank].AvgIndex,
                        created_at = date
                    });
                }
            }

            _context.LeaderboardSnapshots.AddRange(snapshots);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成論壇假資料
        /// </summary>
        private async Task GenerateForumsAsync()
        {
            if (await _context.Forums.AnyAsync())
                return;

            var games = await _context.Games.ToListAsync();
            var forums = new List<Forum>();

            foreach (var game in games)
            {
                forums.Add(new Forum
                {
                    forum_id = game.game_id,
                    game_id = game.game_id,
                    name = $"{game.name}討論區",
                    description = $"{game.name}相關討論、攻略、心得分享",
                    created_at = game.created_at
                });
            }

            _context.Forums.AddRange(forums);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成主題假資料
        /// </summary>
        private async Task GenerateThreadsAsync()
        {
            if (await _context.Threads.AnyAsync())
                return;

            var forums = await _context.Forums.ToListAsync();
            var users = await _context.Users.Take(20).ToListAsync();
            var threads = new List<Thread>();

            foreach (var forum in forums)
            {
                var threadCount = _random.Next(5, 15);
                for (int i = 0; i < threadCount; i++)
                {
                    threads.Add(new Thread
                    {
                        forum_id = forum.forum_id,
                        author_user_id = users[_random.Next(users.Count)].Id,
                        title = GetRandomThreadTitle(forum.name),
                        status = "normal",
                        created_at = DateTime.UtcNow.AddDays(-_random.Next(1, 60)),
                        updated_at = DateTime.UtcNow.AddDays(-_random.Next(0, 30))
                    });
                }
            }

            _context.Threads.AddRange(threads);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成主題回覆假資料
        /// </summary>
        private async Task GenerateThreadPostsAsync()
        {
            if (await _context.ThreadPosts.AnyAsync())
                return;

            var threads = await _context.Threads.ToListAsync();
            var users = await _context.Users.Take(30).ToListAsync();
            var posts = new List<ThreadPost>();

            foreach (var thread in threads)
            {
                var postCount = _random.Next(3, 12);
                for (int i = 0; i < postCount; i++)
                {
                    posts.Add(new ThreadPost
                    {
                        thread_id = thread.thread_id,
                        author_user_id = users[_random.Next(users.Count)].Id,
                        content_md = GetRandomPostContent(),
                        parent_post_id = null, // 暫時不生成回覆的回覆
                        status = "normal",
                        created_at = thread.created_at.AddHours(_random.Next(1, 24)),
                        updated_at = DateTime.UtcNow.AddDays(-_random.Next(0, 10))
                    });
                }
            }

            _context.ThreadPosts.AddRange(posts);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成洞察貼文假資料
        /// </summary>
        private async Task GeneratePostsAsync()
        {
            if (await _context.Posts.AnyAsync())
                return;

            var games = await _context.Games.Take(8).ToListAsync();
            var users = await _context.Users.Take(10).ToListAsync();
            var posts = new List<Post>();

            foreach (var game in games)
            {
                var postCount = _random.Next(2, 6);
                for (int i = 0; i < postCount; i++)
                {
                    posts.Add(new Post
                    {
                        type = "insight",
                        game_id = game.game_id,
                        title = GetRandomInsightTitle(game.name),
                        tldr = GetRandomInsightTldr(),
                        body_md = GetRandomInsightBody(),
                        visibility = true,
                        status = "published",
                        pinned = _random.Next(100) < 10, // 10%機率置頂
                        created_by = users[_random.Next(users.Count)].Id,
                        published_at = DateTime.UtcNow.AddDays(-_random.Next(1, 30)),
                        created_at = DateTime.UtcNow.AddDays(-_random.Next(1, 30)),
                        updated_at = DateTime.UtcNow.AddDays(-_random.Next(0, 10))
                    });
                }
            }

            _context.Posts.AddRange(posts);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成貼文指標快照假資料
        /// </summary>
        private async Task GeneratePostMetricSnapshotsAsync()
        {
            if (await _context.PostMetricSnapshots.AnyAsync())
                return;

            var posts = await _context.Posts.Where(p => p.game_id.HasValue).ToListAsync();
            var popularityIndices = await _context.PopularityIndexDailies.ToListAsync();
            var snapshots = new List<PostMetricSnapshot>();

            foreach (var post in posts)
            {
                if (post.game_id.HasValue && post.published_at.HasValue)
                {
                    var gameIndex = popularityIndices.FirstOrDefault(p => p.game_id == post.game_id.Value && p.date == post.published_at.Value.Date);
                    
                    if (gameIndex != null)
                    {
                        snapshots.Add(new PostMetricSnapshot
                        {
                            post_id = post.post_id,
                            game_id = post.game_id.Value,
                            date = post.published_at.Value.Date,
                            index_value = gameIndex.index_value,
                            details_json = GetRandomMetricDetails(),
                            created_at = post.published_at.Value
                        });
                    }
                }
            }

            _context.PostMetricSnapshots.AddRange(snapshots);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成貼文引用來源假資料
        /// </summary>
        private async Task GeneratePostSourcesAsync()
        {
            if (await _context.PostSources.AnyAsync())
                return;

            var posts = await _context.Posts.ToListAsync();
            var sources = new List<PostSource>();

            foreach (var post in posts)
            {
                var sourceCount = _random.Next(1, 4);
                for (int i = 0; i < sourceCount; i++)
                {
                    sources.Add(new PostSource
                    {
                        post_id = post.post_id,
                        source_name = GetRandomSourceName(),
                        url = GetRandomSourceUrl(),
                        created_at = post.created_at
                    });
                }
            }

            _context.PostSources.AddRange(sources);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成反應假資料
        /// </summary>
        private async Task GenerateReactionsAsync()
        {
            if (await _context.Reactions.AnyAsync())
                return;

            var users = await _context.Users.Take(25).ToListAsync();
            var posts = await _context.Posts.ToListAsync();
            var threads = await _context.Threads.ToListAsync();
            var reactions = new List<Reaction>();

            // 為貼文生成反應
            foreach (var post in posts)
            {
                var reactionCount = _random.Next(5, 25);
                var reactedUsers = users.OrderBy(x => _random.Next()).Take(reactionCount).ToList();

                foreach (var user in reactedUsers)
                {
                    reactions.Add(new Reaction
                    {
                        user_id = user.Id,
                        target_type = "post",
                        target_id = post.post_id,
                        kind = GetRandomReactionKind(),
                        created_at = post.created_at.AddHours(_random.Next(1, 48))
                    });
                }
            }

            // 為主題生成反應
            foreach (var thread in threads)
            {
                var reactionCount = _random.Next(3, 15);
                var reactedUsers = users.OrderBy(x => _random.Next()).Take(reactionCount).ToList();

                foreach (var user in reactedUsers)
                {
                    reactions.Add(new Reaction
                    {
                        user_id = user.Id,
                        target_type = "thread",
                        target_id = thread.thread_id,
                        kind = GetRandomReactionKind(),
                        created_at = thread.created_at.AddHours(_random.Next(1, 24))
                    });
                }
            }

            _context.Reactions.AddRange(reactions);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// 生成收藏假資料
        /// </summary>
        private async Task GenerateBookmarksAsync()
        {
            if (await _context.Bookmarks.AnyAsync())
                return;

            var users = await _context.Users.Take(20).ToListAsync();
            var posts = await _context.Posts.ToListAsync();
            var threads = await _context.Threads.ToListAsync();
            var games = await _context.Games.Take(5).ToListAsync();
            var bookmarks = new List<Bookmark>();

            // 為貼文生成收藏
            foreach (var post in posts)
            {
                var bookmarkCount = _random.Next(2, 12);
                var bookmarkedUsers = users.OrderBy(x => _random.Next()).Take(bookmarkCount).ToList();

                foreach (var user in bookmarkedUsers)
                {
                    bookmarks.Add(new Bookmark
                    {
                        user_id = user.Id,
                        target_type = "post",
                        target_id = post.post_id,
                        created_at = post.created_at.AddHours(_random.Next(1, 72))
                    });
                }
            }

            // 為主題生成收藏
            foreach (var thread in threads)
            {
                var bookmarkCount = _random.Next(1, 8);
                var bookmarkedUsers = users.OrderBy(x => _random.Next()).Take(bookmarkCount).ToList();

                foreach (var user in bookmarkedUsers)
                {
                    bookmarks.Add(new Bookmark
                    {
                        user_id = user.Id,
                        target_type = "thread",
                        target_id = thread.thread_id,
                        created_at = thread.created_at.AddHours(_random.Next(1, 48))
                    });
                }
            }

            // 為遊戲生成收藏
            foreach (var game in games)
            {
                var bookmarkCount = _random.Next(5, 20);
                var bookmarkedUsers = users.OrderBy(x => _random.Next()).Take(bookmarkCount).ToList();

                foreach (var user in bookmarkedUsers)
                {
                    bookmarks.Add(new Bookmark
                    {
                        user_id = user.Id,
                        target_type = "game",
                        target_id = game.game_id,
                        created_at = DateTime.UtcNow.AddDays(-_random.Next(1, 90))
                    });
                }
            }

            _context.Bookmarks.AddRange(bookmarks);
            await _context.SaveChangesAsync();
        }

        // Stage 3 輔助方法

        /// <summary>
        /// 獲取指標基礎值
        /// </summary>
        private decimal GetBaseMetricValue(string metricCode)
        {
            return metricCode switch
            {
                "concurrent_users" => _random.Next(10000, 1000000),
                "video_views" => _random.Next(100000, 10000000),
                "stream_viewers" => _random.Next(1000, 100000),
                "forum_posts" => _random.Next(100, 10000),
                "social_mentions" => _random.Next(1000, 100000),
                "discord_activity" => _random.Next(500, 50000),
                "search_trends" => _random.Next(50, 100),
                "downloads" => _random.Next(10000, 1000000),
                _ => _random.Next(1000, 100000)
            };
        }

        /// <summary>
        /// 計算熱度指數
        /// </summary>
        private decimal CalculatePopularityIndex(List<GameMetricDaily> metrics)
        {
            if (!metrics.Any()) return 0;

            var weights = new Dictionary<string, decimal>
            {
                { "concurrent_users", 0.4m },
                { "forum_posts", 0.2m },
                { "social_mentions", 0.15m },
                { "stream_viewers", 0.15m },
                { "video_views", 0.1m }
            };

            decimal totalWeightedValue = 0;
            decimal totalWeight = 0;

            foreach (var metric in metrics)
            {
                if (weights.TryGetValue(metric.Metric?.code ?? "", out var weight))
                {
                    totalWeightedValue += metric.value * weight;
                    totalWeight += weight;
                }
            }

            return totalWeight > 0 ? totalWeightedValue / totalWeight : metrics.Average(m => m.value);
        }

        /// <summary>
        /// 獲取隨機主題標題
        /// </summary>
        private string GetRandomThreadTitle(string forumName)
        {
            var titles = new[]
            {
                "新手求助",
                "攻略分享",
                "心得討論",
                "問題解答",
                "技巧分享",
                "裝備推薦",
                "角色分析",
                "更新情報",
                "Bug回報",
                "建議討論"
            };

            return $"{titles[_random.Next(titles.Length)]} - {forumName}";
        }

        /// <summary>
        /// 獲取隨機貼文內容
        /// </summary>
        private string GetRandomPostContent()
        {
            var contents = new[]
            {
                "這個遊戲真的很棒！推薦大家試試看。",
                "有人知道這個問題怎麼解決嗎？",
                "分享一下我的遊戲心得，希望對大家有幫助。",
                "這個更新改動很大，大家覺得怎麼樣？",
                "新手剛開始玩，有什麼建議嗎？",
                "這個角色真的很強，推薦大家練練看。",
                "有人一起組隊嗎？",
                "這個Bug真的很煩人，希望官方快點修復。",
                "分享一下我的配裝，大家參考一下。",
                "這個關卡真的很難，有人有攻略嗎？"
            };

            return contents[_random.Next(contents.Length)];
        }

        /// <summary>
        /// 獲取隨機洞察標題
        /// </summary>
        private string GetRandomInsightTitle(string gameName)
        {
            var titles = new[]
            {
                $"{gameName} 最新版本分析",
                $"{gameName} 玩家行為洞察",
                $"{gameName} 市場趨勢報告",
                $"{gameName} 社群活躍度分析",
                $"{gameName} 用戶留存率研究",
                $"{gameName} 競品對比分析",
                $"{gameName} 未來發展預測",
                $"{gameName} 用戶滿意度調查"
            };

            return titles[_random.Next(titles.Length)];
        }

        /// <summary>
        /// 獲取隨機洞察摘要
        /// </summary>
        private string GetRandomInsightTldr()
        {
            var tldrs = new[]
            {
                "深入分析遊戲數據，揭示玩家行為模式",
                "透過大數據分析，預測遊戲發展趨勢",
                "研究社群互動，了解用戶需求變化",
                "分析市場競爭，找出差異化優勢",
                "調查用戶滿意度，優化遊戲體驗"
            };

            return tldrs[_random.Next(tldrs.Length)];
        }

        /// <summary>
        /// 獲取隨機洞察內容
        /// </summary>
        private string GetRandomInsightBody()
        {
            return @"## 數據分析

根據最新的遊戲數據分析，我們發現了幾個有趣的趨勢：

### 1. 用戶活躍度
- 日活躍用戶數持續增長
- 平均遊戲時長穩定在2.5小時
- 週末活躍度比平日高30%

### 2. 社群互動
- 論壇發文量月增長15%
- 社交媒體提及度提升25%
- 用戶生成內容增加40%

### 3. 市場表現
- 下載量穩步上升
- 用戶留存率保持在85%以上
- 付費轉化率提升5%

## 結論

整體而言，遊戲表現良好，用戶參與度高，社群活躍。建議繼續優化用戶體驗，加強社群運營。";
        }

        /// <summary>
        /// 獲取隨機指標詳情
        /// </summary>
        private string GetRandomMetricDetails()
        {
            return @"{
                ""metrics"": [
                    {""code"": ""concurrent_users"", ""description"": ""同時在線用戶數"", ""value"": 125000, ""unit"": ""users"", ""quality"": ""real""},
                    {""code"": ""forum_posts"", ""description"": ""論壇發文數"", ""value"": 2500, ""unit"": ""posts"", ""quality"": ""real""},
                    {""code"": ""social_mentions"", ""description"": ""社群提及次數"", ""value"": 15000, ""unit"": ""mentions"", ""quality"": ""real""}
                ],
                ""calculated_at"": """ + DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ") + @"""
            }";
        }

        /// <summary>
        /// 獲取隨機來源名稱
        /// </summary>
        private string GetRandomSourceName()
        {
            var sources = new[]
            {
                "Steam Charts",
                "SteamDB",
                "SteamSpy",
                "PCGamingWiki",
                "Metacritic",
                "OpenCritic",
                "HowLongToBeat",
                "VGChartz",
                "GameSpot",
                "IGN"
            };

            return sources[_random.Next(sources.Length)];
        }

        /// <summary>
        /// 獲取隨機來源URL
        /// </summary>
        private string GetRandomSourceUrl()
        {
            var domains = new[]
            {
                "steamcharts.com",
                "steamdb.info",
                "steamspy.com",
                "pcgamingwiki.com",
                "metacritic.com",
                "opencritic.com",
                "howlongtobeat.com",
                "vgchartz.com",
                "gamespot.com",
                "ign.com"
            };

            var domain = domains[_random.Next(domains.Length)];
            return $"https://{domain}/game/{_random.Next(1000, 9999)}";
        }

        /// <summary>
        /// 獲取隨機反應類型
        /// </summary>
        private string GetRandomReactionKind()
        {
            var kinds = new[] { "like", "love", "laugh", "wow", "sad", "angry" };
            return kinds[_random.Next(kinds.Length)];
        }
    }
}