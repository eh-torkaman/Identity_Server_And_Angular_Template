using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Entities.RequestFeatures
{
    public abstract class RequestParameters<T> where T : class
    {
        public string Fields { get; set; }
        public  RequestParameters()
        {
            CheckIfDefaultOrderCoulmnIsSetToValidField();
        }

        #region PageSize
        protected int maxPageSize = 50;
        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;

            set => _pageSize = Math.Max(1, Math.Min(value, maxPageSize));
        }
        private int _pageNumber = 1;
        public int PageNumber { get => _pageNumber; set => _pageNumber = Math.Max(value, 1); }

        #endregion PageSize

        #region orderby
        protected void CheckIfDefaultOrderCoulmnIsSetToValidField()
        {
            var name = this.GetType().Name;
            if (DefaultOrderColumn.TrimEvenNull() == "")
                throw new Exception("نام ستون پیش فرض مرتب سازی به درستی تنطیم نشده است : " + name);
            if (!typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(x => x.Name.ToLower() == DefaultOrderColumn.ToLower()))
                throw new Exception("نام ستون پیش فرض مرتب سازی به درستی تنطیم نشده است : " + name);

        }
        private string _order = "";
        public string Order
        {
            get => _order; set => _order = value.TrimEvenNull();//!= "" ? value+ " , "+DefaultOrderColumn : DefaultOrderColumn;
        }

        protected abstract  string DefaultOrderColumn { get; } 

        public string GetOrderQuery()
        {
            var str0 = _order.NormalizeWhiteSpace().ToUpper();
            if (string.IsNullOrWhiteSpace(str0))
            {
                Order = DefaultOrderColumn;
                return Order;
            }

            var orderQueryBuilder = new StringBuilder();

            var propertyInfos = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var sortItem in str0.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (sortItem.IndexOf(' ') == -1)
                {
                    if (propertyInfos.Any(t => t.Name.ToUpper() == sortItem))
                    {
                        orderQueryBuilder.Append($" {sortItem} ASC , ");
                    }
                }
                else
                {
                    var sortItemTemp = sortItem.Split(' ');
                    if (propertyInfos.Any(t => t.Name.ToUpper() == sortItemTemp[0]))
                    {
                        var orderDirection = sortItemTemp[1].ToUpper() == "DESC" ? "DESC" : "ASC";
                        orderQueryBuilder.Append($" {sortItemTemp[0]} {orderDirection} , ");
                    }
                }
            }
            if (orderQueryBuilder.Length != 0)
                Order = (orderQueryBuilder.ToString().TrimEnd(new char[] { ',', ' ' }));
            else
                Order = DefaultOrderColumn;
            return Order;
        }
        public IQueryable<T> GetIQueriableByOrderStr(IQueryable<T> source)
        {
            var str0 = GetOrderQuery();
            if (str0 == "")
                throw new Exception("this shouldent happen");
            return str0!=""? source.OrderBy(str0):source;
        }
        #endregion orderby

        #region search
        private string _search = "";
        public string Search
        {
            get => _search;
            set
            {
                _search = value;
            }
        }

        public IQueryable<T> TextFilter_Strings(IQueryable<T> source)
        {
            if (string.IsNullOrWhiteSpace(Search)) { return source; }

            var elementType = source.ElementType;

            // Get all the string property names on this specific type.
            var stringProperties =
                elementType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.PropertyType == typeof(string))
                    .ToArray();
            if (!stringProperties.Any()) { return source; }

            // Build the string expression
            string filterExpr = string.Join(
                " || ",
                stringProperties.Select(prp => $"{prp.Name}.Contains(@0)")
            );

            return source.Where(filterExpr, Search);
        }

        #endregion search

    }
}
