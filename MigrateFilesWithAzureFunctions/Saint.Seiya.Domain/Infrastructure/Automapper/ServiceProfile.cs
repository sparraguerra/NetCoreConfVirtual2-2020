using AutoMapper;
using Saint.Seiya.Shared.Models;
using Saint.Seiya.Shared.Models.Dto;

namespace Encamina.Gada.Domain.Infrastructure.Automapper
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<SeiyaDocument, IndexRequest>()
                .ForMember(m => m.FileName, ac => ac.Ignore()).ReverseMap(); 
            CreateMap<Request, GetDocumentRequest>().ReverseMap();
            CreateMap<IndexRequestWithBinary, SeiyaDocument>()
                .ForMember(m => m.IsDeleted, ac => ac.Ignore());
            CreateMap<DocumentResponse, SeiyaDocument>()
                .ForMember(m => m.IsDeleted, ac => ac.Ignore())
                .ReverseMap();
            CreateMap<UpdateRequest, Request>();
            CreateMap<DeleteRequest, Request>();            
           
        }
    }
}
