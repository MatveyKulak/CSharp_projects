using Lab5.Models;

namespace Lab5.Services.Logic.Interfaces
{
    public interface ISortable
    {
        List<Sweet> Sort(IEnumerable<Sweet> sweets);
    }
}