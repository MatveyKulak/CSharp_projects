using Lab5.Models;
using Lab5.Services.Logic.Interfaces;

namespace Lab5.Services.Logic.Realisations
{
    public class SortBySugarContent : ISortable
    {
        public List<Sweet> Sort(IEnumerable<Sweet> sweets)
        {
            return sweets.OrderBy(sweet => sweet.SugarContent).ToList();
        }
    }
}