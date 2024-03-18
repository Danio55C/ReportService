using ReportService.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private const int IntervallInMinutes = 30;
        private Timer _timer = new Timer(IntervallInMinutes*60000);
        private ErrorRepository _errorRepository = new ErrorRepository();
        private ReportRepository _reportRepository = new ReportRepository();
        public ReportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += DoWork;
            _timer.Start();
            Logger.Info("Service started...");
        }



        private void DoWork(object sender, ElapsedEventArgs e)
        {

            try
            {

                SendError();
                SendReport();
            }
            catch (Exception ex )
            {

                Logger.Error(ex, ex.Message);
                    throw new Exception(ex.Message);
            }
        }
        private void SendError()
        {
            var errors = _errorRepository.GetLastErrors(IntervallInMinutes);
            if (errors == null || !errors.Any())
                return;
            //send mail
            Logger.Info("Error sent..");
        }
        private void SendReport()
        {
            var actualHour = DateTime.Now.Hour;
            if (actualHour < SendHour)
                return;

            var report = _reportRepository.GetLastNotSentReport();

            if (report == null)
                return;
            //send mail
            _reportRepository.ReportSent(report);

            Logger.Info("Report sent..");
        }

        protected override void OnStop()
        {
            Logger.Info("Service stoped...");
        }
    }
}
