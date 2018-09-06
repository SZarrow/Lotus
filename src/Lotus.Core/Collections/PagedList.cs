using System;
using System.Collections.Generic;
using System.Linq;

namespace Lotus.Core.Collections
{
    [Serializable]
    public class PagedList<T> : IEnumerable<T> where T : class
    {
        private List<T> _container;

        public PagedList(IOrderedQueryable<T> dataSource, Int32 pageIndex, Int32 pageSize, Int32 pageNumberShowSize = 5)
        {
            if (dataSource == null)
            {
                return;
            }

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }

            if (pageSize < 0)
            {
                pageSize = 0;
            }

            if (pageSize > 0)
            {
                try
                {
                    Int32 totalCount = dataSource.Count();
                    _container = dataSource.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                    this.PageInfo = new PageInfo(pageIndex, pageSize, totalCount, pageNumberShowSize);
                }
                catch (Exception ex)
                {
                    this.Exception = ex;
                }
            }
        }

        public PageInfo PageInfo { get; private set; }
        public Exception Exception { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return _container != null ? _container.GetEnumerator() as IEnumerator<T> : null;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
