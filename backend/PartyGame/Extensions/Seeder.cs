using Microsoft.EntityFrameworkCore;
using PartyGame.Entities;
using System.Text.Json;
using MongoDB.Driver;
using System.Text.Json.Serialization;
using System.Data;
using PartyGame.Models.AccountModels;
using PartyGame.Services;

namespace PartyGame.Extensions
{
    public class Seeder
    {
        private readonly GameDbContext _gameDbContext;
        private string _filePath;
        private readonly IAccountService _accountService;

        public Seeder(GameDbContext gameDbContext, IAccountService contextAccessorService)
        {
            _gameDbContext = gameDbContext;
            _filePath = "Data/Places.json";
            _accountService = contextAccessorService;
        }

        public async Task Seed()
        {
            await _gameDbContext.Places.DeleteManyAsync(Builders<Place>.Filter.Empty);

            var placeCount = await _gameDbContext.Places.CountDocumentsAsync(Builders<Place>.Filter.Empty);

            if (placeCount == 0)
            {
                var places = GetPlaces();
                if (places != null)
                {
                    await _gameDbContext.Places.InsertManyAsync(places);
                }
            }
            var userCount = await _gameDbContext.Users.CountDocumentsAsync(Builders<User>.Filter.Empty);
            if (userCount == 0)
            {
                AddUsers();
            }
        }


        private void AddUsers()
        {

            var admin = new RegisterUserDto
            {
                Nickname = "Admin",
                Password = "AdminAdmin",
                ConfirmPassword = "AdminAdmin",
                Email = "Admin@Admin.com",
            };

            _accountService.RegisterUser(admin, "Admin");

            var moderator = new RegisterUserDto
            {
                Nickname = "Moderator",
                Password = "ModeratorModerator",
                ConfirmPassword = "ModeratorModerator",
                Email = "Moderator@Moderator.com"
            };

            _accountService.RegisterUser(moderator, "Moderator");

            var user = new RegisterUserDto
            {
                Nickname = "User",
                Password = "UserUser",
                ConfirmPassword = "UserUser",
                Email = "User@User.com"
            };
            _accountService.RegisterUser(user, "User");
        }

        private IEnumerable<Place>? GetPlaces()

        {
            string jsonString = File.ReadAllText(_filePath);

            var places = JsonSerializer.Deserialize<List<Place>>(jsonString, new JsonSerializerOptions
            {
                Converters = { new JsonStringEnumConverter() }
            });

            return places;
        }
    }
}
