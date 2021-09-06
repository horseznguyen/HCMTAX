using System;

namespace Services.Common.ApplicationService.Dto
{
    [Serializable]
    public class PagedAndSortedRequestDto : PagedRequestDto, IPagedAndSortedResultRequest
    {
        public string SortName { get; set; }
        public bool SortASC { get; set; }
    }
}