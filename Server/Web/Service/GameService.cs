using Game;
using Grpc.Core;
using Business.GameManager;
using Business.Infrastructure.Exceptions;
using Domain.Infrastructure.Enums;

namespace Web.Service
{
    public sealed class GameService : Game.GameService.GameServiceBase
    {
        private readonly IGameManager _gameManager;
        
        public GameService(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }
        public override async Task<JoinMatchResponse> JoinMatch(JoinMatchRequest request, ServerCallContext context)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.OpponentId))
                {
                    var res =  await _gameManager.JoinMatchAsync(request.MatchId, request.OpponentId);
                }
                   var gameResult = await _gameManager.ProcessPlayerMoveAsync(request.MatchId, 
                       request.PlayerMove, request.OpponentId);

               return new JoinMatchResponse()
               {
                   MatchId = gameResult.MatchId,
                   Status = gameResult.Status,
                   Winner = gameResult.Winner
               };
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
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
                var matchId = await _gameManager.CreateMatchAsync(
                    Guid.Parse(request.HostId), 
                    (decimal)request.Bet,
                    request.HostMove
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
