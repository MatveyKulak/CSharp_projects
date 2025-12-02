using Lab5.Models;

namespace Lab5.Services.Logic.Interfaces
{
    public interface IFilterable
    {
        bool IsValid(Sweet sweet);
    }
}