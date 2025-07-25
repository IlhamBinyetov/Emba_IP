using Emba_IP.Models;

namespace Emba_IP.ViewModels
{
    public class IpListViewModel
    {
        public string? SearchTerm { get; set; }
        public List<IpModel> IpList { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int PageSize { get; set; } = 20;
        public List<int> PageSizeOptions { get; set; }
    }
}
