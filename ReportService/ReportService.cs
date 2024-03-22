using EmailSender;
using ReportService.Core;
using ReportService.Core.Repositories;
using System;
using System.Collections.Generic;
using Cipher;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

using System.Threading.Tasks;
using System.Timers;

namespace ReportService
{
    public partial class ReportService : ServiceBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private const int SendHour = 8;
        private const int IntervallInMinutes = 1;
        private Timer _timer = new Timer(IntervallInMinutes*60000);
        private ErrorRepository _errorRepository = new ErrorRepository();
        private ReportRepository _reportRepository = new ReportRepository();
        private Email _email;
        private GenerateHtmlEmail _htmlEmail = new GenerateHtmlEmail();
        private string _emailReceiver;
        private StringCipher _stringCipher = new StringCipher("25F48648-F382-460F-BBA4-4170CD7FE2E3");
        private const string NotEnscyptedPasswordPrefix = "encrypt:";
        public ReportService()
        {
            
            InitializeComponent();
           

            try
            {
                _emailReceiver = ConfigurationManager.AppSettings["ReceiverEmail"];

                

                _email = new Email(new EmailParams
                {
                    HostSmtp = ConfigurationManager.AppSettings["HostSmtp"],
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]),
                    EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]),
                    SenderName = ConfigurationManager.AppSettings["SenderName"],
                    SenderEmail = ConfigurationManager.AppSettings["SenderEmail"],
                    SenderEmailPassword = DecryptSenderEmailPassword(),
                });

            }
            catch (Exception ex)
            {

                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private string DecryptSenderEmailPassword()
        {
            var encryptedPassword = ConfigurationManager.AppSettings["SenderEmailPassword"];

            if (encryptedPassword.StartsWith(NotEnscyptedPasswordPrefix))
            {
                encryptedPassword = _stringCipher.Encrypt(encryptedPassword.Replace(NotEnscyptedPasswordPrefix, ""));

                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configFile.AppSettings.Settings["SenderEmailPassword"].Value = encryptedPassword;
                configFile.Save();
            }

            return _stringCipher.Decrypt(encryptedPassword);
        }
        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += DoWork;
            _timer.Start();
            Logger.Info("Service started...");
        }

        private async void DoWork(object sender, ElapsedEventArgs e)
        {

            try
            {
                await SendError();
                await SendReport();
            }

            catch (Exception ex )
            {

                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }
        private async Task SendError()
        {
            var errors = _errorRepository.GetLastErrors(IntervallInMinutes);
            if (errors == null || !errors.Any())
                return;
           await _email.Send("Błędy w applikajci", _htmlEmail.GenerateErrors(errors, IntervallInMinutes), _emailReceiver);
            Logger.Info("Error sent..");
        }
        private async Task SendReport()
        {
            var actualHour = DateTime.Now.Hour;
            if (actualHour < SendHour)
                return;

            var report = _reportRepository.GetLastNotSentReport();

            if (report == null)
                return;
            await _email.Send("Raport dobowy", _htmlEmail.GenerateReport(report), _emailReceiver);
            Logger.Info("Error sent..");
            _reportRepository.ReportSent(report);
            Logger.Info("Report sent..");
        }
        protected override void OnStop()
        {
            Logger.Info("Service stoped...");
        }
    }
}




