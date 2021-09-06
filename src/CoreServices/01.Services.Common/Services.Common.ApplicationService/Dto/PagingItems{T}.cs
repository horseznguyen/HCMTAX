using System.Collections.Generic;

namespace Services.Common.ApplicationService.Dto
{
    public class PagingItems<T> where T : class
    {
        public PagingItems()
        {
        }

        public PagingItems(int pageSize, int pageNumber, int totalItems)
        {
            PagingInfo = new PagingInfoDto
            {
                PageSize = pageSize,
                PageNumber = pageNumber,
                TotalItems = totalItems
            };
        }

        public IEnumerable<T> Items { get; set; }
        public PagingInfoDto PagingInfo { get; set; }
    }
}