using System.Collections.Generic;

namespace Talabat.API.Helpers
{
    public class PaginationProduct<T>
    {

        public IReadOnlyList<T> Data { get; set; }

        public PaginationProduct(IReadOnlyList<T> data)
        {

            Data = data;
        }

    }
}
