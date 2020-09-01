using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Velocity.Core
{
    public struct Paginated<TItem>
    {
        [JsonConstructor]
        public Paginated(List<TItem> results, int currentPage, int pageSize, int total)
        {
            Results = results;
            CurrentPage = currentPage;
            PageSize = pageSize;
            Total = total;
        }

        public List<TItem> Results { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }
        public int Total { get; }

        [JsonIgnore]
        public int ShowingFrom => ReadShowingFrom;

        [JsonProperty("showingFrom")]
        private int ReadShowingFrom
        {
            get
            {
                if (CurrentPage == 0)
                    return 0;

                return (CurrentPage - 1) * PageSize + 1;
            }
            set { }
        }

        [JsonIgnore]
        public int ShowingTo => ReadShowingTo;

        [JsonProperty("showingTo")]
        private int ReadShowingTo
        {
            get
            {
                if (CurrentPage == 0)
                    return 0;

                if (CurrentPage == LastPage)
                    return Total;

                return CurrentPage * PageSize;
            }
            set { }
        }


        [JsonIgnore]
        public int LastPage => ReadLastPage;

        [JsonProperty("lastPage")]
        private int ReadLastPage
        {
            get
            {
                if (PageSize == 0)
                    return 0;

                var result = Total / PageSize;

                return (Total % PageSize) > 0 ? result + 1 : result;
            }
            set { }
        }

        [JsonIgnore]
        public IEnumerable<int> Pages => ReadPages;


        [JsonProperty("pages")]
        private IEnumerable<int> ReadPages
        {
            get
            {
                var pages = new[] { 1, 2 }
                    .Concat(Enumerable.Range(CurrentPage - 2, 5))
                    .Concat(new[] { LastPage - 1, LastPage });

                var lastPage = LastPage;
                return pages.Where(n => n >= 1 && n <= lastPage).Distinct();
            }
            set { }
        }
    }
}
