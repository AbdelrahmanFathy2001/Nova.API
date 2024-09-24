using AutoMapper;
using System.Linq;
using Talabat.API.Dots;
using Talabat.DAL.Entities;
using Talabat.DAL.Entities.Identity;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.API.Helpers
{
    public class MappingProfiles:Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductPictureUrlResolver>())
                .ForMember(d => d.Images , opt => opt.MapFrom<ProductImagesUrlResolver>())
                //.ForMember(dest => dest.comments , opt => opt.MapFrom(src => src.comments))
                .ReverseMap();

            CreateMap<Product, ProductsToReturnDto>()
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductsPictureUrlResolver>())
                .ReverseMap();

            CreateMap<Image, ImageToReturnDto>();
            CreateMap<Comment, commentToReturnDto>();

            CreateMap<ProductBrand, ProductBrandOrTypeToReturnDto>()
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductBrandPictureUrlResolver>());
            

            CreateMap<ProductType, ProductBrandOrTypeToReturnDto>()
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductTypePictureUrlResolver>());


            CreateMap<DAL.Entities.Identity.Address, AddressDto>().ReverseMap();

            CreateMap<CustomerBasketDto, CustomerBasket>();

            CreateMap<BasketItemDto, BasketItem>();

            CreateMap<UserDashboardDto, AppUser>().ReverseMap();

            CreateMap<AddressDto, DAL.Entities.Order_Aggregate.Address>().ReverseMap();

            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, o => o.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryCost, o => o.MapFrom(s => s.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ItemOrdered.ProductId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ItemOrdered.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ItemOrdered.PictureUrl))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());
        }
    }
}
