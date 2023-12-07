using ErrorsReporting.Models.Domains;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ErrorsReporting
{
    public partial class ErrorsReporting : ServiceBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<Error> _errors = new List<Error>();
        private DbRepository _dbRepository = new DbRepository();
        private readonly int _intervalInMinutes;
        private Timer _timer; 
        public ErrorsReporting()
        {
            try
            {
                _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinutes"]);
                _timer = new Timer(_intervalInMinutes * 60000);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
            
            InitializeComponent();

        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += ScanForErrors;
            _timer.Start();
        }

        private void ScanForErrors(object sender, ElapsedEventArgs e)
        {
            _errors = _dbRepository.GetNewErrorsFromDb();
            if (_errors?.Any() != true)
            {
                Logger.Info($"Nie znaleziono nowych błędów.");
            }
            else
            {
                foreach(var error in _errors) 
                {
                    Logger.Info($"W bazie danych {error.Date.ToString()} pojawił się nowy błąd o komunikacie: {error.Message}.");
                    _dbRepository.MarkErrorAsSend(error);
                }
                
            }
        }

        protected override void OnStop()
        {
        }
    }
}
