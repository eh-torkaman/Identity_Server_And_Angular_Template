using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public readonly MetaData MetaData;
        public  PagedList(IQueryable<T> query, int pageSize, int currentPage)
        {
            var totalCount = query.Count();
            this.MetaData = new MetaData()
            {
                TotalCount = totalCount,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };

            AddRange(query.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList());
        }
        public virtual string XPaginationStr
        {
            get => JsonConvert.SerializeObject(MetaData);
        }
    }

    public class MetaData
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}
