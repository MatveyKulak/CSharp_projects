namespace Hospital.Business.Models.People
{
    /// <summary>
    /// Базовый класс для всех людей в системе (пациентов и персонала).
    /// </summary>
    public abstract class Person
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        public string FullName => $"{LastName} {FirstName}";
    }
}