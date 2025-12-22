using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoCare.Application.Common.Interfaces;
using AutoCare.Domain.Entities;
using AutoCare.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AutoCare.Infrastructure.Data.Seed
{
    /// <summary>
    /// Main data seeder - coordinates all seeders
    /// </summary>
    public static class DataSeeder
    {
        /// <summary>
        /// Seeds all initial data if database is empty
        /// </summary>
        public static async Task SeedAllAsync(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            try
            {
                logger.LogInformation("Starting data seeding...");

                // Check if data already exists
                if (await context.Users.AnyAsync())
                {
                    logger.LogInformation("Database already seeded. Skipping...");
                    return;
                }

                // Cast to ApplicationDbContext to disable interceptors
                var dbContext = context as ApplicationDbContext;
                dbContext?.DisableDomainEventDispatcher();

                // Seed in order (respecting foreign keys)
                await SeedUsersAsync(context, passwordHasher, logger);
                await SeedServiceCentersAsync(context, logger);
                await SeedServicesAsync(context, logger);
                await SeedServiceCenterServicesAsync(context, logger);

                // Re-enable interceptors
                dbContext?.EnableDomainEventDispatcher();

                logger.LogInformation("✅ Data seeding completed successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Error during data seeding");
                throw;
            }
        }

        #region User Seeding

        private static async Task SeedUsersAsync(
            IApplicationDbContext context,
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            logger.LogInformation("Seeding users...");

            var password = passwordHasher.HashPassword("Password123@");

            var users = new List<User>
            {
                // Admin User
                User.Create(
                    "admin@autocare.com",
                    password,
                    "System Administrator",
                    "01012345678",
                    UserType.Admin),

                // Customer Users
                User.Create(
                    "ahmed.mohamed@gmail.com",
                    password,
                    "Ahmed Mohamed Ali",
                    "01123456789",
                    UserType.Customer),

                User.Create(
                    "sara.hassan@gmail.com",
                    password,
                    "Sara Hassan Ibrahim",
                    "01234567890",
                    UserType.Customer),

                User.Create(
                    "mohamed.omar@gmail.com",
                    password,
                    "Mohamed Omar Mahmoud",
                    "01098765432",
                    UserType.Customer),

                // Employee Users (will be linked to service centers)
                User.Create(
                    "manager1@autocare.com",
                    password,
                    "Mahmoud Khalil",
                    "01011111111",
                    UserType.Employee),

                User.Create(
                    "tech1@autocare.com",
                    password,
                    "Hassan Ali",
                    "01022222222",
                    UserType.Employee),

                User.Create(
                    "manager2@autocare.com",
                    password,
                    "Amira Fahmy",
                    "01033333333",
                    UserType.Employee),

                User.Create(
                    "tech2@autocare.com",
                    password,
                    "Youssef Sherif",
                    "01044444444",
                    UserType.Employee)
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Create Customer Profiles
            var customerUsers = users.Where(u => u.UserType == UserType.Customer).ToList();
            var customers = new List<Customer>
            {
                Customer.Create(customerUsers[0].Id, "15 شارع الجمهورية، وسط البلد", "Cairo"),
                Customer.Create(customerUsers[1].Id, "42 طريق الكورنيش", "Alexandria"),
                Customer.Create(customerUsers[2].Id, "8 شارع التحرير، الدقي", "Giza")
            };

            context.Customers.AddRange(customers);
            await context.SaveChangesAsync();

            // Create Vehicles for Customers
            var vehicles = new List<Vehicle>
            {
                // Ahmed's vehicles
                Vehicle.Create(customers[0].Id, "Toyota", "Corolla", 2022, "ABC123", null, "White"),
                Vehicle.Create(customers[0].Id, "Honda", "Civic", 2021, "XYZ789", null, "Black"),

                // Sara's vehicle
                Vehicle.Create(customers[1].Id, "Hyundai", "Elantra", 2023, "DEF456", null, "Silver"),

                // Mohamed's vehicles
                Vehicle.Create(customers[2].Id, "Nissan", "Sunny", 2020, "GHI789", null, "Blue"),
                Vehicle.Create(customers[2].Id, "Kia", "Cerato", 2022, "JKL012", null, "Red")
            };

            context.Vehicles.AddRange(vehicles);
            await context.SaveChangesAsync();

            logger.LogInformation($"✅ Seeded {users.Count} users, {customers.Count} customers, {vehicles.Count} vehicles");
        }

        #endregion

        #region Service Center Seeding

        private static async Task SeedServiceCentersAsync(
            IApplicationDbContext context,
            ILogger logger)
        {
            logger.LogInformation("Seeding service centers...");

            var serviceCenters = new List<ServiceCenter>
            {
                ServiceCenter.Create(
                    "AutoCare Center - Nasr City",
                    "مركز أوتوكير - مدينة نصر",
                    "25 Abbas El Akkad Street, Nasr City",
                    "25 شارع عباس العقاد، مدينة نصر",
                    "Cairo",
                    30.0444m,
                    31.3447m,
                    "01012345001",
                    "nasr@autocare.com",
                    "Sat-Thu: 9AM-6PM"),

                ServiceCenter.Create(
                    "Quick Fix Garage - Mohandessin",
                    "كويك فكس جراج - المهندسين",
                    "12 Gameat El Dewal Street, Mohandessin",
                    "12 شارع جامعة الدول، المهندسين",
                    "Giza",
                    30.0594m,
                    31.2000m,
                    "01012345002",
                    "mohandessin@quickfix.com",
                    "Sat-Thu: 8AM-8PM"),

                ServiceCenter.Create(
                    "Pro Auto Service - Heliopolis",
                    "برو أوتو سيرفس - مصر الجديدة",
                    "45 Al Ahram Street, Heliopolis",
                    "45 شارع الأهرام، مصر الجديدة",
                    "Cairo",
                    30.0871m,
                    31.3256m,
                    "01012345003",
                    "heliopolis@proauto.com",
                    "Sat-Thu: 9AM-7PM"),

                ServiceCenter.Create(
                    "Elite Car Care - Maadi",
                    "إليت كار كير - المعادي",
                    "18 Road 9, Maadi",
                    "18 طريق 9، المعادي",
                    "Cairo",
                    29.9602m,
                    31.2494m,
                    "01012345004",
                    "maadi@elitecare.com",
                    "Sat-Thu: 9AM-6PM"),

                ServiceCenter.Create(
                    "Speed Auto Workshop - 6th October",
                    "سبيد أوتو ورشة - 6 أكتوبر",
                    "Central Axis, 6th October City",
                    "المحور المركزي، مدينة 6 أكتوبر",
                    "Giza",
                    29.9390m,
                    30.9191m,
                    "01012345005",
                    "october@speedauto.com",
                    "Sat-Thu: 8AM-9PM")
            };

            context.ServiceCenters.AddRange(serviceCenters);
            await context.SaveChangesAsync();

            // Create Employees for Service Centers
            var employeeUsers = await context.Users
                .Where(u => u.UserType == UserType.Employee)
                .ToListAsync();

            var employees = new List<Employee>
            {
                // Service Center 1 - Nasr City
                Employee.Create(employeeUsers[0].Id, serviceCenters[0].Id, EmployeeRole.Manager, DateTime.Parse("2023-01-15")),
                Employee.Create(employeeUsers[1].Id, serviceCenters[0].Id, EmployeeRole.Technician, DateTime.Parse("2023-03-20")),

                // Service Center 2 - Mohandessin
                Employee.Create(employeeUsers[2].Id, serviceCenters[1].Id, EmployeeRole.Manager, DateTime.Parse("2022-11-10")),
                Employee.Create(employeeUsers[3].Id, serviceCenters[1].Id, EmployeeRole.Technician, DateTime.Parse("2023-05-05"))
            };

            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();

            logger.LogInformation($"✅ Seeded {serviceCenters.Count} service centers, {employees.Count} employees");
        }

        #endregion

        #region Service Seeding

        private static async Task SeedServicesAsync(
            IApplicationDbContext context,
            ILogger logger)
        {
            logger.LogInformation("Seeding services...");

            var services = new List<Service>
            {
                // Maintenance Services
                Service.Create(
                    "Oil Change",
                    "تغيير الزيت",
                    250m,
                    30,
                    ServiceType.Maintenance,
                    "Complete engine oil and filter replacement",
                    "تغيير زيت المحرك والفلتر بالكامل"),

                Service.Create(
                    "Brake Inspection & Service",
                    "فحص وصيانة الفرامل",
                    400m,
                    60,
                    ServiceType.Maintenance,
                    "Full brake system check and pad replacement if needed",
                    "فحص نظام الفرامل بالكامل وتغيير التيل إذا لزم الأمر"),

                Service.Create(
                    "Tire Rotation & Balancing",
                    "تدوير وترصيص الإطارات",
                    200m,
                    45,
                    ServiceType.Maintenance,
                    "Rotate tires and balance wheels for even wear",
                    "تدوير الإطارات وترصيص العجلات للتآكل المتساوي"),

                Service.Create(
                    "Battery Check & Replacement",
                    "فحص واستبدال البطارية",
                    150m,
                    30,
                    ServiceType.Maintenance,
                    "Battery testing and replacement if required",
                    "اختبار البطارية واستبدالها إذا لزم الأمر"),

                Service.Create(
                    "Air Conditioning Service",
                    "صيانة التكييف",
                    500m,
                    90,
                    ServiceType.Maintenance,
                    "A/C system check, gas refill, and filter cleaning",
                    "فحص نظام التكييف، إعادة شحن الغاز، وتنظيف الفلتر"),

                Service.Create(
                    "Engine Diagnostic",
                    "فحص المحرك الإلكتروني",
                    300m,
                    45,
                    ServiceType.Maintenance,
                    "Computer diagnostic scan for engine issues",
                    "فحص إلكتروني شامل لمشاكل المحرك"),

                Service.Create(
                    "Transmission Service",
                    "صيانة ناقل الحركة",
                    800m,
                    120,
                    ServiceType.Maintenance,
                    "Transmission fluid change and inspection",
                    "تغيير زيت الفتيس والفحص الشامل"),

                Service.Create(
                    "Full Car Inspection",
                    "فحص شامل للسيارة",
                    600m,
                    90,
                    ServiceType.Maintenance,
                    "Complete vehicle safety and performance check",
                    "فحص شامل لسلامة وأداء السيارة"),

                // Spare Parts Services
                Service.Create(
                    "Brake Pads Replacement",
                    "تغيير تيل الفرامل",
                    450m,
                    60,
                    ServiceType.SpareParts,
                    "Original or aftermarket brake pad installation",
                    "تركيب تيل فرامل أصلي أو تجاري"),

                Service.Create(
                    "Air Filter Replacement",
                    "تغيير فلتر الهواء",
                    120m,
                    15,
                    ServiceType.SpareParts,
                    "Engine air filter replacement",
                    "استبدال فلتر هواء المحرك"),

                Service.Create(
                    "Spark Plugs Replacement",
                    "تغيير البواجي",
                    350m,
                    45,
                    ServiceType.SpareParts,
                    "Complete spark plug set replacement",
                    "تغيير طقم البواجي بالكامل"),

                Service.Create(
                    "Wiper Blades Replacement",
                    "تغيير مساحات الزجاج",
                    100m,
                    10,
                    ServiceType.SpareParts,
                    "Front and rear wiper blade replacement",
                    "تغيير مساحات الزجاج الأمامية والخلفية")
            };

            context.Services.AddRange(services);
            await context.SaveChangesAsync();

            logger.LogInformation($"✅ Seeded {services.Count} services");
        }

        #endregion

        #region Service Center Services Mapping

        private static async Task SeedServiceCenterServicesAsync(
            IApplicationDbContext context,
            ILogger logger)
        {
            logger.LogInformation("Mapping services to service centers...");

            var serviceCenters = await context.ServiceCenters.ToListAsync();
            var services = await context.Services.ToListAsync();

            var mappings = new List<ServiceCenterService>();

            // Service Center 1 (Nasr City) - All services, some with custom pricing
            foreach (var service in services)
            {
                decimal? customPrice = service.ServiceType == ServiceType.Maintenance
                    ? service.BasePrice * 0.95m // 5% discount
                    : null;

                mappings.Add(ServiceCenterService.Create(
                    serviceCenters[0].Id,
                    service.Id,
                    customPrice));
            }

            // Service Center 2 (Mohandessin) - Most services with premium pricing
            foreach (var service in services.Take(10))
            {
                decimal? customPrice = service.BasePrice * 1.1m; // 10% premium

                mappings.Add(ServiceCenterService.Create(
                    serviceCenters[1].Id,
                    service.Id,
                    customPrice));
            }

            // Service Center 3 (Heliopolis) - Selected services, standard pricing
            foreach (var service in services.Where(s => s.ServiceType == ServiceType.Maintenance))
            {
                mappings.Add(ServiceCenterService.Create(
                    serviceCenters[2].Id,
                    service.Id,
                    null)); // Use base price
            }

            // Service Center 4 (Maadi) - Half services with custom pricing
            foreach (var service in services.Take(6))
            {
                decimal? customPrice = service.BasePrice * 0.98m; // 2% discount

                mappings.Add(ServiceCenterService.Create(
                    serviceCenters[3].Id,
                    service.Id,
                    customPrice));
            }

            // Service Center 5 (6th October) - All services, budget pricing
            foreach (var service in services)
            {
                decimal? customPrice = service.BasePrice * 0.90m; // 10% discount

                mappings.Add(ServiceCenterService.Create(
                    serviceCenters[4].Id,
                    service.Id,
                    customPrice));
            }

            context.ServiceCenterServices.AddRange(mappings);
            await context.SaveChangesAsync();

            logger.LogInformation($"✅ Created {mappings.Count} service-center mappings");
        }

        #endregion
    }
}