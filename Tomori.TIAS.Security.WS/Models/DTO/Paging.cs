using Tomori.TIAS.Security.WS.Models.Enums;

namespace Tomori.TIAS.Security.WS.Models.DTO
{
    public class Paging
    {
        public int Page;
        public int TotalPage;
        public int TotalRow;
        public int Size;
        public string SortBy;
        public SortTypeEnum SortType;
    }
}