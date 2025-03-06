using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Models.ScoreboardModels;
using PartyGame.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace PartyGame.Services
{
    public interface IScoreboardService
    {
        Task AddNewGame();
        Task<List<FinishedGame>> GetAllGames();
        Task<PagedResult<FinishedGame>> GetGames(ScoreboardQuery scoreboardQuery);
    }

    public class ScoreboardService : IScoreboardService
    {
        private readonly IHttpContextAccessorService _httpContextAccessorService;
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IScoreboardRepository _scoreboardRepository;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json

        public ScoreboardService(
            IHttpContextAccessorService httpContextAccessorService,
            IGameSessionRepository gameSessionRepository,
            IScoreboardRepository scoreboardRepository)
        {
            _httpContextAccessorService = httpContextAccessorService;
            _gameSessionRepository = gameSessionRepository;
            _scoreboardRepository = scoreboardRepository;
        }

        public async Task AddNewGame()
        {

            string token = _httpContextAccessorService.GetTokenFromHeader();
            GameSession session = await _gameSessionRepository.GetSessionByToken(token);

            if (session == null)
            {
                throw new KeyNotFoundException($"Session was not found");
            }

            if (session.ActualRoundNumber != ROUNDS_NUMBER)
            {
                throw new Exception($"Game was not finished and cannot be saved");
            }

            FinishedGame newFinishedGame = new FinishedGame
            {
                Nickname = session.Nickname,
                FinalScore = session.GameScore,
                Rounds = session.Rounds,
                DifficultyLevel = session.DifficultyLevel
            };

            await _scoreboardRepository.CreateAsync(newFinishedGame);
        }

        public async Task<List<FinishedGame>> GetAllGames()
        {
            IEnumerable<FinishedGame> games = await _scoreboardRepository.GetAllAsync();

            if (games == null)
            {
                throw new KeyNotFoundException($"There no games in history");
            }

            return games.ToList();
        }

        public async Task<PagedResult<FinishedGame>> GetGames(ScoreboardQuery scoreboardQuery)
        {
            List<FinishedGame> games = await _scoreboardRepository.GetGames(scoreboardQuery);
            int totalScores =  (await _scoreboardRepository.GetAllAsync()).Count();

            var result = new PagedResult<FinishedGame>(games,totalScores, scoreboardQuery.PageSize, scoreboardQuery.PageNumber);
            return result;
        }

    }
}
