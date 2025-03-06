using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IGameSessionService
    {
        Task DeleteSessionByToken(string token);
        Task DeleteSessionByHeader();
        Task<GameSession> GetSessionByToken(string token);
        Task<GameSession> GetSessionByHeader();
        Task UpdateGameSession(GameSession session);
        Task<bool> HasActiveGameSession(string token);
        Task AddNewGameSession(GameSession session);

    }

    public class GameSessionService : IGameSessionService
    {
        private readonly IGameSessionRepository _gameSessionRepository;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        public GameSessionService(IGameSessionRepository gameSessionRepository,IHttpContextAccessorService httpContextAccessorService)
        {
            _gameSessionRepository = gameSessionRepository;
            _httpContextAccessorService = httpContextAccessorService;
        }


        public async Task DeleteSessionByToken(string token)
        {
            DeleteResult deleteResult = await _gameSessionRepository.DeleteSessionByToken(token);
            if (deleteResult.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"GameSession with token {token} was not found.");
            }
        }

        public async Task DeleteSessionByHeader()
        {
            string token = _httpContextAccessorService.GetTokenFromHeader();

            DeleteResult deleteResult = await _gameSessionRepository.DeleteSessionByToken(token);
            if (deleteResult.DeletedCount == 0)
            {
                throw new KeyNotFoundException($"GameSession with token {token} was not found.");
            }
        }

        public async Task<GameSession> GetSessionByToken(string token)
        {
            GameSession gameSession = await _gameSessionRepository.GetGameSessionByToken(token);

            if (gameSession is null)
            {
                throw new KeyNotFoundException($"Game session with token {token} was not found");
            }
            return gameSession;
        }

        public async Task<GameSession> GetSessionByHeader()
        {
            string token =  _httpContextAccessorService.GetTokenFromHeader();

            GameSession gameSession = await _gameSessionRepository.GetGameSessionByToken(token);

            if (gameSession is null)
            {
                throw new KeyNotFoundException($"Game session with token {token} was not found");
            }
            return gameSession;
        }

        public async Task UpdateGameSession(GameSession session)
        {
            var existingSession = await _gameSessionRepository.GetAsync(session.Id);

            if (existingSession == null)
            {
                throw new KeyNotFoundException($"GameSession with ID {session.Id} was not found.");
            }

            await _gameSessionRepository.UpdateAsync(session);
        }

        public async Task<bool> HasActiveGameSession(string token)
        {
            var existingSession = await _gameSessionRepository.GetGameSessionByToken(token);

            if (existingSession is null)
            {
                return false;
            }

            return true;
        }

        public async Task AddNewGameSession(GameSession session)
        {
            GameSession existedSession = await _gameSessionRepository.GetGameSessionByToken(session.Token);
            if (existedSession != null)
            {
                throw new InvalidOperationException("A session with the same token already exists.");
            }
            await _gameSessionRepository.CreateAsync(session);
        }


    }

   
}
