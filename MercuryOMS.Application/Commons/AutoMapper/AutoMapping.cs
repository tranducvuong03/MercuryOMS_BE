using AutoMapper;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.Commons.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Category, CategoryResponse>();
        }
    }
}
