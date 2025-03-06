using AutoMapper;
using Microsoft.Identity.Client;
using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IAccountRepository : IRepository<User>
    {

        void AddNewUsers(IEnumerable<User> newUsers);
        Task<User> GetUserByNicknameOrEmailAsync(string nicknameOrEmail);
    }

    public class AccountRepository :Repository<User>,IAccountRepository
    {
        public AccountRepository(GameDbContext gameDbContext):base(gameDbContext.Database,"Users")
        {

        }

        public async void AddNewUsers(IEnumerable<User> newUsers)
        {
            await Collection.InsertManyAsync(newUsers);
        }

        public async Task<User> GetUserByNicknameOrEmailAsync(string nicknameOrEmail)
        {
            var filter = Builders<User>.Filter.Or(
                Builders<User>.Filter.Eq(u => u.Nickname, nicknameOrEmail),
                Builders<User>.Filter.Eq(u => u.Email, nicknameOrEmail)
            );

            return await Collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}