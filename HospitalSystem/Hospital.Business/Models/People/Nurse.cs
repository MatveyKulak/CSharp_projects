namespace Hospital.Business.Models.People
{
    /// <summary>
    /// Представляет медсестру, пользователя системы с ограниченными правами.
    /// </summary>
    public class Nurse : User
    {
        public Guid? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public override string DepartmentName => Department?.Name ?? "Не указано";
    }
}