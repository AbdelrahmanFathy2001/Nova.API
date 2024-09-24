using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Order_Aggregate;

namespace Talabat.BLL.Specifications.Order_Specifications
{
    public class OrderWithItemsAndDeliveryMethodSpecifications:BaseSpecification<Order>
    {
        public OrderWithItemsAndDeliveryMethodSpecifications() 
        {
            AddInclude(O => O.Items);
            AddInclude(O => O.DeliveryMethod);
            AddOrderByDescending(O => O.OrderDate);
        }

        public OrderWithItemsAndDeliveryMethodSpecifications(int orderId):base(o=>o.Id == orderId)
        {
            AddInclude(O => O.Items);
            AddInclude(O => O.DeliveryMethod);
            AddOrderByDescending(O => O.OrderDate);
        }
        //this constructor is Used For Get All Orders For Specific User
        public OrderWithItemsAndDeliveryMethodSpecifications(string buyerEmail):base(O => O.BuyerEmail == buyerEmail)
        {
            AddInclude(O => O.Items);
            AddInclude(O => O.DeliveryMethod);
            AddOrderByDescending(O => O.OrderDate);
        }


        //this constructor is Used For Get an Orders For a Specific User

        public OrderWithItemsAndDeliveryMethodSpecifications(int orderId , string buyerEmail) 
            : base(O => (O.BuyerEmail == buyerEmail && O.Id == orderId))
        {
            AddInclude(O => O.Items);
            AddInclude(O => O.DeliveryMethod);

        }
    }
}
