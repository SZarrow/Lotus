using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lotus.Core.Collections
{
    public struct PageInfo
    {
        public PageInfo(Int32 pageIndex, Int32 pageSize, Int32 totalCount, Int32 pageNumberShowSize = 5)
        {
            this.PageIndex = pageIndex < 1 ? 1 : pageIndex;
            this.PageSize = pageSize < 0 ? 0 : pageSize;
            this.PageCount = 0;
            this.TotalCount = totalCount < 0 ? 0 : totalCount;
            this.PageNumShowSize = pageNumberShowSize > 0 ? pageNumberShowSize : 5;

            if (this.TotalCount > 0 && this.PageSize > 0)
            {
                this.PageCount = this.TotalCount % this.PageSize == 0 ? this.TotalCount / this.PageSize : (this.TotalCount / this.PageSize + 1);
            }
        }

        public Int32[] PageNumbers
        {
            get
            {
                return this.GetPageNumbers();
            }
        }

        public Boolean HasPrev
        {
            get
            {
                return this.PageIndex > 1;
            }
        }

        public Boolean HasNext
        {
            get
            {
                return this.PageIndex < this.PageCount;
            }
        }

        public Int32 PageNumShowSize { get; private set; }

        public Int32 PageCount { get; private set; }

        public Int32 TotalCount { get; private set; }

        public Int32 PageSize { get; private set; }

        public Int32 PageIndex { get; private set; }

        private Int32[] GetPageNumbers()
        {
            Int32 quo = (this.PageIndex - 1) / this.PageNumShowSize;

            Int32 begin = quo * this.PageNumShowSize + 1;

            Int32 end = Math.Min(begin + this.PageCount - 1, (quo + 1) * this.PageNumShowSize);

            end = Math.Min(end, this.PageCount);

            Int32[] nums = new Int32[end - begin + 1];

            Int32 k = 0;
            for (Int32 i = begin; i <= end; i++)
            {
                nums[k++] = i;
            }

            return nums;
        }
    }
}
