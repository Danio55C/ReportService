using EmailSender;
using ReportService.Core;
using ReportService.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportService.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var emailReceiver = "daniel.janca@WP.pl";
            var htmlEmail = new GenerateHtmlEmail();
            var email = new Email(new EmailParams
            {
                HostSmtp = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                SenderName = "Daniel",
                SenderEmail = "reportservice.test123@gmail.com",
                SenderEmailPassword = "qxsw pjne ggyr ccba"

            });

            var report = new Report
            {


                Id = 1,
                Title = "R/1/2020",
                Date = new DateTime(2020, 1, 1, 12, 0, 0),
                Positions = new List<ReportPosition>
                {
                    new ReportPosition
                    {
                        Id=1,
                        ReportId=1,
                        Title="Position 1",
                        Description="Description 1",
                        Value=43.01M
                    },
                     new ReportPosition
                    {
                        Id=2,
                        ReportId=1,
                        Title="Position 2",
                        Description="Description 2",
                        Value=4311M
                    },
                      new ReportPosition
                    {
                        Id=3,
                        ReportId=1,
                        Title="Position 3",
                        Description="Description 3",
                        Value=1.99M
                    }

                }
            };

            var errors = new List<Error>
            {
                new Error {Message="Błąd Testowy 1", Date=DateTime.Now},
                new Error {Message="Błąd Testowy 2", Date=DateTime.Now},
            };

            Console.WriteLine("Wysyłanie email (Raport dobowy");
            email.Send("Raport dobowy", htmlEmail.GenerateReport(report), emailReceiver).Wait();
            Console.WriteLine("Wysłano email (Raport dobowy");


            Console.WriteLine("Wysyałenie email (Błędy w apliakcji");
            email.Send("Błędy w applikajci", htmlEmail.GenerateErrors(errors, 10), emailReceiver).Wait();
            Console.WriteLine("Wysłano email (Błędy w apliakcji");



        }
    }
}

            



