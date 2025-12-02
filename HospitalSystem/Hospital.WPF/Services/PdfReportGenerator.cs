using Hospital.Business.Models.People;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using System.IO;

namespace Hospital.WPF.Services
{
    /// <summary>
    /// Генерирует PDF-отчеты с использованием библиотеки QuestPDF.
    /// </summary>
    public class PdfReportGenerator : IReportGenerator
    {
        public void GenerateDischargeSummary(Patient patient)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"Выписка_{patient.LastName}_{patient.FirstName}.pdf";
            string filePath = Path.Combine(desktopPath, fileName);

            var dischargeDate = DateTime.Now;

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .AlignCenter()
                        .Text("Выписной эпикриз")
                        .SemiBold().FontSize(20).FontColor(Colors.Grey.Darken2);

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(15);

                            column.Item().Text(text =>
                            {
                                text.Span("Пациент: ").SemiBold();
                                text.Span($"{patient.LastName} {patient.FirstName} {patient.MiddleName}");
                            });
                            column.Item().Text(text =>
                            {
                                text.Span("Дата рождения: ").SemiBold();
                                text.Span($"{patient.DateOfBirth:dd.MM.yyyy}");
                            });

                            if (patient.MedicalRecord != null)
                            {
                                column.Item().Text(text =>
                                {
                                    text.Span("Период госпитализации: ").SemiBold();
                                    text.Span($"с {patient.MedicalRecord.HospitalizationDate:dd.MM.yyyy} по {dischargeDate:dd.MM.yyyy}");
                                });
                            }

                            column.Item().Text(text =>
                            {
                                text.Span("Диагноз при поступлении: ").SemiBold();
                                text.Span(patient.Diagnosis);
                            });

                            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                            column.Item().Text("Проведенное лечение и обследования:").SemiBold().FontSize(14);

                            if (patient.MedicalRecord?.Appointments != null && patient.MedicalRecord.Appointments.Any())
                            {
                                foreach (var appointment in patient.MedicalRecord.Appointments)
                                {
                                    column.Item().PaddingLeft(1, Unit.Centimetre).Text(text =>
                                    {
                                        text.Span($"• [{appointment.AppointmentType}] ").SemiBold();
                                        text.Span(appointment.Summary);
                                        text.Span($" (Статус: {appointment.Status})");
                                    });
                                }
                            }
                            else
                            {
                                column.Item().Text("Назначения отсутствуют.");
                            }

                            if (patient.AssignedDoctor != null)
                            {
                                column.Item().PaddingTop(1, Unit.Centimetre).Text(text =>
                                {
                                    text.Span("Лечащий врач: ").SemiBold();
                                    var firstNameInitial = !string.IsNullOrEmpty(patient.AssignedDoctor.FirstName) ? $"{patient.AssignedDoctor.FirstName[0]}." : "";
                                    var middleNameInitial = !string.IsNullOrEmpty(patient.AssignedDoctor.MiddleName) ? $"{patient.AssignedDoctor.MiddleName[0]}." : "";
                                    text.Span($"{patient.AssignedDoctor.LastName} {firstNameInitial} {middleNameInitial}");
                                });
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Дата выписки: ").SemiBold();
                            text.Span($"{dischargeDate:dd.MM.yyyy HH:mm}");
                            text.EmptyLine();
                            text.Span("Страница ");
                            text.CurrentPageNumber();
                        });
                });
            })
            .GeneratePdf(filePath);

            // Открываем созданный файл в программе по умолчанию для PDF
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }
    }
}