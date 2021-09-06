namespace Services.Common.ApplicationService.Dto
{
    public class KeywordAndPagedAndSortedRequestDto : PagedAndSortedRequestDto, IKeywordRequestDto
    {
        public string Keyword { get; set; }
    }
}