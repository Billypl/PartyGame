using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using PartyGame.Entities;
using PartyGame.Models.GameModels;
using PartyGame.Services;

namespace PartyGame.Controllers
{
    [ApiController]
    [Route("api/game")]
    public class GameController:ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IGameSessionService _gameSessionService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("start")]
        public async Task<ActionResult> StartGame([FromBody]  StartDataDto startData)
        {
            string token = await _gameService.StartNewGame(startData);

            return Ok(new
            {
                Token = token,
                Message = "Game generated successfully"
            });
        }

   

        [HttpPatch("check")]
        [Authorize]
        public async Task<ActionResult> CheckGuess([FromBody]  Coordinates guessingCoordinates)
        { 
           RoundResultDto result = await _gameService.CheckGuess(guessingCoordinates);
           return Ok(result);
        }

        [HttpGet("round/{roundNumber}")]
        [Authorize]
        public async Task<GuessingPlaceDto> GetGuessingPlace([FromRoute] int roundNumber)
        {
            return await _gameService.GetPlaceToGuess(roundNumber);
        }

        [HttpGet("actual_round")]
        [Authorize]
        public async Task<int> GetActualRoundNumber()
        {
            return await _gameService.GetActualRoundNumber();
        }

        [HttpPatch("finish")]
        [Authorize]
        public async Task<ActionResult> FinishGame()
        {
            SummarizeGameDto result = await _gameService.FinishGame();
            return Ok(result);
        }

        [HttpDelete("delete_session")]
        [Authorize]
        public async Task<ActionResult> DeleteGame()
        {
            await _gameSessionService.DeleteSessionByHeader();
            return Ok(new
                {
                    Message = "Game successfully deleted"
                }
                );
        }


    }
}
