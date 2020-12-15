using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Utilities
{
    public class PagingUtility
    {
        public int Page { get; }

        public int PageSize { get;  }

        public int Offset { get; }

        public PagingUtility(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
            this.Offset = Page * PageSize;
        }
    }
}
