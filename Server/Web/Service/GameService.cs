using Game;
using Grpc.Core;
using Business.Game;
using Business.Infrastructure.Exceptions;
using Business.Models;

namespace Web.Service
{
    public sealed class GameService : Game.GameService.GameServiceBase
    {
        private readonly IGameManager _gameManager;
        
        public GameService(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }
        /*public override async Task<PlayGameResponse> PlayGame(JoinMatchRequest request, ServerCallContext context)
        {
            try
            {
                var result = await _gameManager.ProcessPlayerMoveAsync(
                    matchId: request.MatchId,
                    playerMove: request.PlayerMove
                );
                
                return new PlayGameResponse
                {
                    Winner = result.Winner,
                    MatchId = result.MatchId
                };
            }
            catch (GameNotFoundException ex)
            {
                throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
            }
            catch (InvalidMoveException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
        }
        
       public override async Task<JoinMatchResponse> JoinMatch(JoinMatchRequest request, ServerCallContext context)
       {
           try
           {
               var response = await _gameManager.JoinMatchAsync(request.MatchId, request.OpponentId);
               return new JoinMatchResponse
               {
                   MatchId = response.MatchId,
                   Status = response.Status
               };
           }
           catch (Exception ex)
           {
               throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
           }
       }*/
       
       public new async Task<JoinMatchResponse> JoinMatch(JoinMatchRequest request, ServerCallContext context)
       {
           try
           {
               // Step 1: If an opponent ID is provided, handle the join match logic
               if (!string.IsNullOrEmpty(request.OpponentId))
               {
                   var joinResponse = await _gameManager.JoinMatchAsync(request.MatchId, request.OpponentId);
            
                   // Early return if the action is only to join the match
                   if (string.IsNullOrEmpty(request.PlayerMove))
                   {
                       return new JoinMatchResponse()
                       {
                           MatchId = joinResponse.MatchId,
                           Status = joinResponse.Status,
                           Winner = null // No winner yet
                       };
                   }
               }

               // Step 2: Handle the play game logic if a move is provided
               if (!string.IsNullOrEmpty(request.PlayerMove))
               {
                   var playResponse = await _gameManager.ProcessPlayerMoveAsync(request.MatchId, request.PlayerMove, request.OpponentId);
                   return new JoinMatchResponse()
                   {
                       MatchId = playResponse.MatchId,
                       Status = "Game Finished",
                       Winner = playResponse.Winner
                   };
               }

               throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request: must include either OpponentId or PlayerMove."));
           }
           catch (Exception ex)
           {
               throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
           }
       }
       
       public override async Task<TransferMoneyResponse> TransferMoney(TransferMoneyRequest request, ServerCallContext context)
       {
           try
           {
               var success = await _gameManager.TransferMoney(Guid.Parse(request.SenderId), Guid.Parse(request.ReceiverId), (decimal)request.Amount);
               return new TransferMoneyResponse
               {
                   Success = success
               };
           }
           catch (Exception ex)
           {
               throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
           }
       }
        public override async Task<CreateMatchResponse> CreateMatch(CreateMatchRequest request, ServerCallContext context)
        {
            try
            {
                // Pass the host's move to the CreateMatchAsync method
                var matchId = await _gameManager.CreateMatchAsync(
                    Guid.Parse(request.HostId), 
                    (decimal)request.Bet,
                    request.HostMove  // Receive the host's move
                );

                return new CreateMatchResponse
                {
                    MatchId = matchId.ToString()
                };
            }
            catch (InvalidBetException ex)
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
            }
        }
        
        public override async Task<GetBalanceResponse> GetBalance(GetBalanceRequest request, ServerCallContext context)
        {
            var balance = await _gameManager.GetBalanceAsync(request.UserId);

            return new GetBalanceResponse
            {
                Balance = (double)balance
            };
        }
        
        public override async Task<GetMatchHistoryResponse> GetMatchHistory(GetMatchHistoryRequest request, ServerCallContext context)
        {
            var matchHistory = await _gameManager.GetAllMatchesForUserAsync(request.UserId);

            return new GetMatchHistoryResponse
            {
                MatchHistory =
                {
                    matchHistory.Select(match => new MatchHistory
                    {
                        MatchId = match.MatchId,
                        Bet = (double)match.Bet,
                        Winner = match.Winner,
                        StartTime = match.StartTime.ToString()
                    })
                }
            };
        }
    }
}
