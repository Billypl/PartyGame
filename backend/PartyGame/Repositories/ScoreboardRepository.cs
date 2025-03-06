using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Models.ScoreboardModels;
using SortDirection = PartyGame.Models.ScoreboardModels.SortDirection;


namespace PartyGame.Repositories
{
    public interface IScoreboardRepository : IRepository<FinishedGame>
    {
        Task<List<FinishedGame>> GetGames(ScoreboardQuery scoreboardQuery);
    }

    public class ScoreboardRepository :Repository<FinishedGame>,IScoreboardRepository
    {

  
        public ScoreboardRepository(GameDbContext gameDbContext):base(gameDbContext.Database, "GameResults")
        {
        }

        public async Task<List<FinishedGame>> GetGames(ScoreboardQuery scoreboardQuery)
        {
            var filterBuilder = Builders<FinishedGame>.Filter;
            var filter = FilterDefinition<FinishedGame>.Empty;

            // Filtracja po SearchNickname
            if (!string.IsNullOrEmpty(scoreboardQuery.SearchNickname))
            {
                filter &= filterBuilder.Regex(r => r.Nickname, new MongoDB.Bson.BsonRegularExpression(scoreboardQuery.SearchNickname, "i"));
            }

            // Sortowanie po GameScore
            var sortDirection = scoreboardQuery.SortDirection == SortDirection.ASC
                ? Builders<FinishedGame>.Sort.Ascending(r => r.FinalScore)
                : Builders<FinishedGame>.Sort.Descending(r => r.FinalScore);

            // Paginacja
            var pagedScores = await Collection.Find(filter)
                .Sort(sortDirection)
                .Skip((scoreboardQuery.PageNumber - 1) * scoreboardQuery.PageSize)
                .Limit(scoreboardQuery.PageSize)
                .ToListAsync();

            return pagedScores;
        }
    }
}
