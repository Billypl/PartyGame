﻿

using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using PartyGame.Entities;
using MongoDB.Driver;
using PartyGame.Models.AccountModels;
using PartyGame.Models.GameModels;



namespace PartyGame.Services
{

    public interface IGameService
    {
        string StartNewGame(StartDataDto startData);
        RoundResultDto? CheckGuess(Coordinates guessingCoordinates);
        public GuessingPlaceDto GetPlaceToGuess(int roundsNumber);
        public SummarizeGameDto FinishGame();
        public int GetActualRoundNumber();
    }

    public class GameService : IGameService
    {
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IMapper _mapper;
        private readonly IPlaceService _placeService;
        private readonly IGameSessionService _gameSessionService;
        private readonly IAccountService _accountService;
        private readonly IGameTokenService _gameTokenService;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        const int ROUNDS_NUMBER = 5; // TODO: Need to get this value from appsettgins.json

        public GameService(IOptions<AuthenticationSettings> authenticationSettings
            , IMapper mapper,GameDbContext gameDbContext,IPlaceService placeService,
            IGameSessionService gameSessionService, IHttpContextAccessorService httpContextAccessorService,
            IAccountService accountService,IGameTokenService gameTokenService)
        {
            _authenticationSettings = authenticationSettings.Value;
            _mapper = mapper;
            _placeService = placeService;
            _gameSessionService = gameSessionService;
            _httpContextAccessorService = httpContextAccessorService;
            _accountService = accountService;
            _gameTokenService = gameTokenService;

        }


        public string StartNewGame(StartDataDto startData)
        {
            if (_httpContextAccessorService.IsTokenExist() &&
                _gameSessionService.HasActiveGameSession(_httpContextAccessorService.GetTokenFromHeader()).Result)
            {
                throw new AccessViolationException("Game for this token already exists");
            }


            DifficultyLevel difficulty =
                (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), startData.Difficulty, ignoreCase: true);

            var newGameToken = _httpContextAccessorService.IsUserLoggedIn() ? _httpContextAccessorService.GetTokenFromHeader() 
                : _gameTokenService.GenerateGuessSessionToken(startData);
            // NOW ONLY WORKS FOR EASY DIFFICULTY
            List<Round> gameRounds = GenerateRounds(difficulty);

            var gameSession = new GameSession
            {
                Token = newGameToken,
                Rounds = gameRounds,
                ActualRoundNumber = 0,
                ExpirationDate = DateTime.UtcNow.AddMinutes(30),
                Nickname = startData.Nickname,
                DifficultyLevel = difficulty
            };

            _gameSessionService.AddNewGameSession(gameSession);
            return newGameToken;
        }


        private List<Round> GenerateRounds(DifficultyLevel difficulty)
        {
            List<ObjectId> places = _placeService.GetRandomIDsOfPlaces(ROUNDS_NUMBER, difficulty).Result;

            if (places.Count() < ROUNDS_NUMBER)
            {
                throw new InvalidOperationException("Not enough places was got from db.");
            }

            List<Round> gameRounds = new List<Round>();

            for (int i = 0; i < ROUNDS_NUMBER; i++)
            {
                var newRound = new Round
                {
                    IDPlaceToGuess = places[i],
                    GuessedCoordinates = new Coordinates(),
                    Score = 0
                };

                gameRounds.Add(newRound);
            }

            return gameRounds;
        }


        public GuessingPlaceDto GetPlaceToGuess(int roundsNumber)
        {
            string token = _httpContextAccessorService.GetTokenFromHeader();
            var session = _gameSessionService.GetSessionByToken(token).Result;

            if (session.ActualRoundNumber != roundsNumber)
            {
                throw new InvalidOperationException(
                    $"The actual round number is ({session.ActualRoundNumber}) and getting other round number is not allowed");
            }

            var guessingPlace = _placeService.GetPlaceById(session.Rounds[roundsNumber].IDPlaceToGuess.ToString()).Result;
           
            return _mapper.Map<GuessingPlaceDto>(guessingPlace);
        }

        public RoundResultDto? CheckGuess(Coordinates guessingCoordinates)
        {
            var token = _httpContextAccessorService.GetTokenFromHeader();
            var session = _gameSessionService.GetSessionByToken(token).Result;

            if (session.ActualRoundNumber >= ROUNDS_NUMBER)
            {
                throw new InvalidOperationException(
                    $"The actual round number ({session.ActualRoundNumber}) exceeds or equals the allowed number of rounds ({ROUNDS_NUMBER}).");
            }

            var guessingPlace = _placeService.GetPlaceById(session.Rounds[session.ActualRoundNumber].IDPlaceToGuess.ToString()).Result;
            var distanceDifference = CalculateDistanceBetweenCords(guessingPlace.Coordinates, guessingCoordinates);

            var result = new RoundResultDto
            {
                DistanceDifference = distanceDifference,
                OriginalPlace = guessingPlace,
                RoundNumber = session.ActualRoundNumber
            };

            session.Rounds[session.ActualRoundNumber].Score = distanceDifference;
            session.Rounds[session.ActualRoundNumber].GuessedCoordinates = guessingCoordinates;
            session.ActualRoundNumber++;
            session.GameScore += distanceDifference;

            _gameSessionService.UpdateGameSession(session);

            return result;
        }
        private double CalculateDistanceBetweenCords(Coordinates first, Coordinates second)
        {
            const double EarthRadiusMeters = 6371000.0; 

            double ConvertToRadians(double degrees) => degrees * Math.PI / 180.0;

            double deltaLatitude = ConvertToRadians(second.Latitude - first.Latitude);
            double deltaLongitude = ConvertToRadians(second.Longitude - first.Longitude);

            double firstLatitudeRadians = ConvertToRadians(first.Latitude);
            double secondLatitudeRadians = ConvertToRadians(second.Latitude);

            double a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                       Math.Cos(firstLatitudeRadians) * Math.Cos(secondLatitudeRadians) *
                       Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusMeters * c;

        }

        public SummarizeGameDto FinishGame()
        {
            var token = _httpContextAccessorService.GetTokenFromHeader();
            GameSession session = _gameSessionService.GetSessionByToken(token).Result;
            SummarizeGameDto summarize = CreateSummarize(session);

            return summarize;
        }

        private SummarizeGameDto CreateSummarize(GameSession session)
        {
            SummarizeGameDto summarize = new SummarizeGameDto();

            summarize.Rounds = session.Rounds;
            summarize.Score = session.GameScore;

            return summarize;
        }

        public int GetActualRoundNumber()
        {
            string token = _httpContextAccessorService.GetTokenFromHeader();
            return _gameSessionService.GetSessionByToken(token).Result.ActualRoundNumber;
        }

    }
}
