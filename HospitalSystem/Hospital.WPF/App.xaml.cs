using Hospital.Business.Models;
using Hospital.Business.Models.Medical;
using Hospital.Business.Models.People;
using Hospital.Data.Context;
using Hospital.Data.Repositories;
using Hospital.Services.Implementations;
using Hospital.Services.Interfaces;
using Hospital.WPF.Services;
using Hospital.WPF.ViewModels;
using Hospital.WPF.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Hospital.WPF
{
    /// <summary>
    /// Главный класс приложения. Отвечает за запуск, настройку зависимостей и управление жизненным циклом.
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Конфигурирует контейнер внедрения зависимостей (DI).
        /// </summary>
        private void ConfigureServices(IServiceCollection services)
        {
            var connectionString = "Data Source=hospital.db";
            Debug.WriteLine($"USING DATABASE AT: {Path.GetFullPath("hospital.db")}");

            // --- Регистрация зависимостей ---
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connectionString));
            services.AddAutoMapper(typeof(App)); // Сканирует сборку в поисках профилей AutoMapper

            // UI-сервисы
            services.AddSingleton<IDialogService, DialogService>(); // Один DialogService на все приложение
            services.AddTransient<IReportGenerator, PdfReportGenerator>(); // Новый генератор отчетов для каждого запроса

            // Репозитории и сервисы бизнес-логики
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IUserService, UserService>();

            // ViewModel'и
            services.AddTransient<LoginViewModel>(); // Новый экземпляр каждый раз, когда требуется
        }

        /// <summary>
        /// Основной метод, управляющий жизненным циклом приложения.
        /// </summary>
        protected override async void OnStartup(StartupEventArgs e)
        {
            await InitializeAsync(); // Применяем миграции и сидируем данные

            // Бесконечный цикл для поддержки "Смены пользователя"
            while (true)
            {
                User? currentUser;
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Получаем LoginViewModel из DI контейнера
                    var loginViewModel = scope.ServiceProvider.GetRequiredService<LoginViewModel>();
                    var loginView = new LoginView(loginViewModel);
                    loginView.ShowDialog();
                    currentUser = loginView.AuthenticatedUser; // Получаем результат от окна
                }

                if (currentUser == null) break; // Если окно входа закрыли, завершаем приложение


                using (var scope = _serviceProvider.CreateScope())
                {
                    var mainViewModel = await MainViewModel.CreateAsync(scope.ServiceProvider, currentUser);
                    var mainWindow = new MainWindow(mainViewModel);
                    Application.Current.MainWindow = mainWindow;

                    bool logoutRequested = false;
                    mainViewModel.LogoutRequested += () =>
                    {
                        logoutRequested = true;
                        mainWindow.Close();
                    };

                    mainWindow.ShowDialog();

                    if (!logoutRequested) break; // Если окно закрыли крестиком, а не кнопкой "Выйти"
                }
            }
            Shutdown();
        }

        /// <summary>
        /// Выполняет первоначальную настройку: миграцию и заполнение БД начальными данными (seeding).
        /// </summary>
        private async Task InitializeAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.MigrateAsync();

                // Проверяем, есть ли в базе хоть какие-то данные. Если есть, ничего не делаем.
                if (await dbContext.Users.AnyAsync())
                {
                    return;
                }

                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                // --- СОЗДАЕМ ОТДЕЛЕНИЯ ---
                var cardioDep = new Department { Name = "Кардиологическое" };
                var surgeryDep = new Department { Name = "Хирургическое" };
                var neuroDep = new Department { Name = "Неврологическое" };
                dbContext.Departments.AddRange(cardioDep, surgeryDep, neuroDep);

                // --- СОЗДАЕМ ПЕРСОНАЛ ---
                var admin = new Administrator { FirstName = "Главный", LastName = "Администратор", Username = "admin", PasswordHash = hasher.Hash("12345") };

                var doctor1 = new Doctor { FirstName = "Иван", LastName = "Иванов", Specialization = "Кардиолог", Username = "doctor", PasswordHash = hasher.Hash("12345"), Department = cardioDep };
                var doctor2 = new Doctor { FirstName = "Петр", LastName = "Петров", Specialization = "Хирург", Username = "doctor2", PasswordHash = hasher.Hash("12345"), Department = surgeryDep };

                var nurse1 = new Nurse { FirstName = "Анна", LastName = "Сидорова", Username = "nurse", PasswordHash = hasher.Hash("12345"), Department = cardioDep };
                var nurse2 = new Nurse { FirstName = "Мария", LastName = "Кузнецова", Username = "nurse2", PasswordHash = hasher.Hash("12345"), Department = surgeryDep };

                dbContext.Administrators.Add(admin);
                dbContext.Doctors.AddRange(doctor1, doctor2);
                dbContext.Nurses.AddRange(nurse1, nurse2);

                // --- СОЗДАЕМ ПАЦИЕНТОВ ---
                var patient1 = new Patient
                {
                    FirstName = "Сергей",
                    LastName = "Волков",
                    MiddleName = "Андреевич",
                    DateOfBirth = new DateTime(1980, 5, 15),
                    Diagnosis = "Ишемическая болезнь сердца",
                    Department = cardioDep,
                    AssignedDoctor = doctor1,
                    MedicalRecord = new MedicalRecord { HospitalizationDate = DateTime.Now.AddDays(-5) }
                };

                var patient2 = new Patient
                {
                    FirstName = "Елена",
                    LastName = "Зайцева",
                    MiddleName = "Викторовна",
                    DateOfBirth = new DateTime(1992, 10, 21),
                    Diagnosis = "Аппендицит",
                    Department = surgeryDep,
                    AssignedDoctor = doctor2,
                    MedicalRecord = new MedicalRecord { HospitalizationDate = DateTime.Now.AddDays(-2) }
                };

                dbContext.Patients.AddRange(patient1, patient2);

                // Сохраняем все изменения в базу одной транзакцией
                await dbContext.SaveChangesAsync();
            }
        }
    }
}