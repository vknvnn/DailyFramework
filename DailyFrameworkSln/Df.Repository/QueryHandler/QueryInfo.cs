using System.Collections.Generic;

namespace Df.Repository.QueryHandler
{
    public interface IQueryInfo
    {
        long Id { get; set; }
        string SearchText { get; set; }
        IList<OrderByInfo> OrderBys { get; set; }
    }

    public class QueryInfo<TDto> : IQueryInfo
    where TDto : DtoBase
    {
        public long Id { get; set; }
        public string SearchText { get; set; }
        public IList<OrderByInfo> OrderBys { get; set; }
        public IList<ClauseInfo> Filters { get; set; }
        public string GetPredicate(out object[] args)
        {
            args = null;
            return string.Empty;
        }
    }

    public enum OrderByOperator
    {
        Asc = 0,
        Desc = 1,
    }
    public class OrderByInfo
    {
        public string Field { get; set; }
        public OrderByOperator Op { get; set; }

        public string Fully()
        {
            return $"{Field} {(Op == OrderByOperator.Desc ? "DESC" : "ASC")}";
        }
    }
    public enum FilterOperator
    {
        Content = 0,
    }

    public enum ClauseOperator
    {
        Or = 0,
        And = 1
    }
    public class ClauseInfo
    {
        public ClauseOperator ClauseOp { get; set; }
        public FilterInfo Filter { get; set; }
    }

    public class FilterInfo
    {
        public string Field { get; set; }
        public FilterOperator FilterOp { get; set; }
        public string Value { get; set; }
    }
}
