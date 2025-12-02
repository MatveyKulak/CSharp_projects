using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital.Business.Models.People
{
    /// <summary>
    /// Базовый класс для всех пользователей, которые могут входить в систему.
    /// </summary>
    public abstract class User : Person
    {
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public string Role
        {
            get
            {
                return this switch
                {
                    Doctor => "Врач",
                    Nurse => "Медсестра",
                    Administrator => "Администратор",
                    _ => "Пользователь"
                };
            }
        }
        public virtual string DepartmentName => string.Empty;

        [NotMapped]
        public Guid? TempDepartmentId { get; set; }
    }
}