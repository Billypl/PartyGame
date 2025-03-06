using AutoMapper;
using MongoDB.Driver;
using PartyGame.Entities;

namespace PartyGame.Repositories
{
    public interface IGameSessionRepository:IRepository<GameSession>
    {
        Task<GameSession> GetSessionByToken(string token);
        Task<DeleteResult> DeleteSessionByToken(string token);
        Task<GameSession> GetGameSessionByToken(string token);
    }

    public class GameSessionRepository : Repository<GameSession>,IGameSessionRepository
    {

        public GameSessionRepository(GameDbContext gameDbContext):base(gameDbContext.Database,"GameSessions")
        {
         
        }
        public async Task<GameSession> GetSessionByToken(string token)
        {
            return await Collection.Find(gs => gs.Token == token)
                .FirstOrDefaultAsync();
        }

        public async Task<DeleteResult> DeleteSessionByToken(string token)
        {
            return await Collection.DeleteOneAsync(gs => gs.Token == token);
        }

        public async Task<GameSession> GetGameSessionByToken(string token)
        {
            return await Collection
                .Find(s => s.Token == token)
                .FirstOrDefaultAsync();
        }

    }
}
