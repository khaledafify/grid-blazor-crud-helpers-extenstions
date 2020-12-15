using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.Api.Models
{
    public class ItemsDTO<T>
    {
        public IEnumerable<T> Items { get; set; }
        public TotalsDTO Totals { get; set; }
        public PagerDTO Pager { get; set; }

        public ItemsDTO()
        {
        }

        public ItemsDTO(IEnumerable<T> items, TotalsDTO totals, PagerDTO pager)
        {
            Items = items;
            Totals = totals;
            Pager = pager;
        }
    }

}
