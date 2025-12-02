using Lab5.Services.Logic.Interfaces;

namespace Lab5.Models
{
    public class Gift
    {
        private List<Sweet> _sweets;
        public string Name { get; private set; }

        public Gift(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Имя подарка не может быть пустым.", nameof(name));
            }
            Name = name;
            _sweets = new List<Sweet>();
        }

        public void AddSweet(Sweet sweet)
        {
            if (sweet == null)
            {
                throw new ArgumentNullException(nameof(sweet));
            }
            _sweets.Add(sweet);
        }

        public void AddSweets(IEnumerable<Sweet> newSweets)
        {
            _sweets.AddRange(newSweets);
        }

        public double CalculateTotalWeight()
        {
            return _sweets.Sum(sweet => sweet.Weight);
        }

        public IEnumerable<Sweet> GetSweets()
        {
            return _sweets;
        }

        public void SortSweets(ISortable sorter)
        {
            _sweets = sorter.Sort(_sweets);
        }

        public IEnumerable<Sweet> FindSweets(IFilterable filter)
        {
            var foundSweets = new List<Sweet>();
            foreach (var sweet in _sweets)
            {
                if (filter.IsValid(sweet))
                {
                    foundSweets.Add(sweet);
                }
            }
            return foundSweets;
        }

        public void RemoveSweetAt(int index)
        {
            if (index >= 0 && index < _sweets.Count)
            {
                _sweets.RemoveAt(index);
            }
        }

        public void ReplaceSweetAt(int index, Sweet newSweet)
        {
            if (index >= 0 && index < _sweets.Count)
            {
                _sweets[index] = newSweet;
            }
        }

        public override string ToString()
        {
            return $"Подарок '{Name}' ({_sweets.Count} сладостей, {CalculateTotalWeight():F2} г.)";
        }
    }
}