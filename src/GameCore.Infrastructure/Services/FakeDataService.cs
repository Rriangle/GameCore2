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
    }
}