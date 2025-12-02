using Lab5.Models;
using Lab5.Services.Logic.Interfaces;

namespace Lab5.Services.Logic.Realisations
{
    public class SugarRangeFilter : IFilterable
    {
        private readonly double _minSugar;
        private readonly double _maxSugar;

        public SugarRangeFilter(double minSugar, double maxSugar)
        {
            if (minSugar < 0 || maxSugar < 0 || minSugar > maxSugar)
            {
                throw new ArgumentException("Диапазон сахара задан некорректно.");
            }
            _minSugar = minSugar;
            _maxSugar = maxSugar;
        }

        public bool IsValid(Sweet sweet)
        {
            return sweet.SugarContent >= _minSugar && sweet.SugarContent <= _maxSugar;
        }
    }
}