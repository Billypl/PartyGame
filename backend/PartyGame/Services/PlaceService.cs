using AutoMapper;
using MongoDB.Bson;
using MongoDB.Driver;
using PartyGame.Entities;
using PartyGame.Models.AccountModels;
using PartyGame.Models.PlaceModels;
using PartyGame.Repositories;

namespace PartyGame.Services
{
    public interface IPlaceService
    {
        Task<Place> GetPlaceById(string id);
        Task<List<Place>> GetAllPlaces();
        void AddNewPlace(NewPlaceDto newPlace);
        Task<int> GetPlacesCount();
        Task<List<ObjectId>> GetRandomIDsOfPlaces(int numberOfRoundsToTake, DifficultyLevel difficultyLevel);
        Task AddNewPlaceToQueue(NewPlaceDto newPlace);
        Task<List<PlaceToCheckDto>> GetAllPlacesInQueue();
        Task AcceptPlace(string PlaceToCheckId);
        Task RejectPlace(string PlaceToRejectId);

    }

    public class PlaceService : IPlaceService
    {
        private readonly IPlacesRepository _placesRepository;
        private readonly IMapper _mapper;
        private readonly IPlacesToCheckRepository _placesToCheckRepository;
        private readonly IHttpContextAccessorService _httpContextAccessorService;

        public PlaceService(IPlacesRepository placesRepository,IMapper mapper,
            IHttpContextAccessorService httpContextAccessorService, IPlacesToCheckRepository placesToCheckRepository)
        {
            _placesRepository = placesRepository;
            _mapper = mapper;
            _httpContextAccessorService = httpContextAccessorService;
            _placesToCheckRepository = placesToCheckRepository;
        }

        public async Task<Place> GetPlaceById(string id)
        {
            var place = await _placesRepository.GetAsync(id);

            if (place == null)
            {
                throw new KeyNotFoundException($"Place with ID {id} was not found.");
            }

            return place;
        }

        public async Task<List<Place>> GetAllPlaces()
        {
            var places = await _placesRepository.GetAllAsync();

            if (places == null)
            {
                throw new KeyNotFoundException($"There is no places in db");
            }

            return places.ToList();
        }

        public void AddNewPlace(NewPlaceDto newPlace)
        {
            Place place = new Place();
            _mapper.Map(newPlace, place);
            _placesRepository.CreateAsync(place);
        }
        public async Task<int> GetPlacesCount()
        {
            var count = await _placesRepository.GetPlacesCount();
            return (int)count;
        }

        public async Task<List<ObjectId>> GetRandomIDsOfPlaces(int numberOfRoundsToTake, DifficultyLevel difficultyLevel)
        {
   
            var placesWithDifficulty = await _placesRepository.GetPlacesByDifficulty(difficultyLevel);
            if (!placesWithDifficulty.Any())
            {
                return new List<ObjectId>();
            }

            numberOfRoundsToTake = Math.Min(numberOfRoundsToTake, placesWithDifficulty.Count);

            var randomIndexes = new HashSet<int>();
            var random = new Random();

            while (randomIndexes.Count < numberOfRoundsToTake)
            {
                randomIndexes.Add(random.Next(0, placesWithDifficulty.Count));
            }

            var randomPlaces = randomIndexes
                .Select(index => placesWithDifficulty[index].Id)
                .ToList();

             return randomPlaces;
        }


        public async Task AddNewPlaceToQueue(NewPlaceDto newPlace)
        {
            AccountDetailsFromTokenDto authorData = _httpContextAccessorService.GetAuthenticatedUserProfile();

            PlaceToCheck newPlaceToCheck = new PlaceToCheck
            {
                AuthorId = authorData.UserId,
                CreatedAt = DateTime.Now,
                NewPlace = newPlace
            };

           await _placesToCheckRepository.CreateAsync(newPlaceToCheck);
        }

        public async Task<List<PlaceToCheckDto>> GetAllPlacesInQueue()
        {
            IEnumerable<PlaceToCheck> placesTask = await _placesToCheckRepository.GetAllAsync();
            List<PlaceToCheckDto> placesDto = _mapper.Map<List<PlaceToCheckDto>>(placesTask);
            return placesDto; 
        }
        public async Task AcceptPlace(string placeToCheckId)
        {
            PlaceToCheck placeToAccept = _placesToCheckRepository.GetAsync(placeToCheckId).Result;

            if (placeToAccept == null)
            {
                throw new KeyNotFoundException("Place you want to add to game doesn't exist");

            }

            Place newPlace = _mapper.Map<Place>(placeToAccept.NewPlace);

            _placesRepository.CreateAsync(newPlace);
            _placesToCheckRepository.DeleteAsync(placeToCheckId);
        }

        public async Task RejectPlace(string placeToRejectId)
        {
            PlaceToCheck placeToAccept = _placesToCheckRepository.GetAsync(placeToRejectId).Result;

            if (placeToAccept == null)
            {
                throw new KeyNotFoundException("Place you want to reject doesn't exist");
            }

            _placesToCheckRepository.DeleteAsync(placeToRejectId);
        }

    }
}
