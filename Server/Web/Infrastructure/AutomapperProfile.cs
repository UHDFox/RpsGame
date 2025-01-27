using AutoMapper;
using Business.Models;
using Game;

namespace Web.Infrastructure;

public class AutomapperProfile : Profile
{
    public AutomapperProfile()
    {
        CreateMap<TransferMoneyRequest, TransferMoneyModel>().ReverseMap();
        CreateMap<TransferMoneyModel, TransferMoneyResponse >().ReverseMap();
        
        CreateMap<MatchHistoryModel, GetMatchHistoryRequest>().ReverseMap();
        CreateMap<CreateMatchRequest, MatchHistoryModel>().ReverseMap();
        CreateMap<Game.CreateMatchRequest, MatchHistoryModel>()
            .ConstructUsing(x => new MatchHistoryModel(
                Guid.Parse(x.HostId), 
                (decimal)x.Bet, 
                x.HostMove));
    }
}