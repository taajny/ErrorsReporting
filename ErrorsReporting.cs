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
using Infobip.Api.Client;
using Infobip.Api.Client.Api;
using Infobip.Api.Client.Model;


namespace ErrorsReporting
{
    public partial class ErrorsReporting : ServiceBase
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private List<Error> _errors = new List<Error>();
        private DbRepository _dbRepository = new DbRepository();
        private readonly int _intervalInMinutes;
        private Timer _timer;

        private readonly string _baseURL;
        private readonly string _apiKey;
        private readonly string _sender;
        private readonly string _recipient;
        
        public ErrorsReporting()
        {
            try
            {
                _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinutes"]);
                _baseURL = ConfigurationManager.AppSettings["BaseURL"];
                _apiKey = ConfigurationManager.AppSettings["ApiKey"];
                _sender = ConfigurationManager.AppSettings["Sender"];
                _recipient = ConfigurationManager.AppSettings["Recipient"];

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
                    SendSMS($"W bazie danych {error.Date.ToString()} pojawił się nowy błąd o komunikacie: {error.Message}.");
                    _dbRepository.MarkErrorAsSend(error);
                }
                
            }
        }

        private void SendSMS(string textMessage)
        {
            var configuration = new Infobip.Api.Client.Configuration()
            {
                BasePath = _baseURL,
                ApiKeyPrefix = "App",
                ApiKey = _apiKey
            };

            var sendSmsApi = new SendSmsApi(configuration);

            var smsMessage = new SmsTextualMessage()
            {
                From = _sender,
                Destinations = new List<SmsDestination>()
                {
                    new SmsDestination(to: _recipient)
                },
                Text = textMessage
            };

            var smsRequest = new SmsAdvancedTextualRequest()
            {
                Messages = new List<SmsTextualMessage>() { smsMessage }
            };

            try
            {
                var smsResponse = sendSmsApi.SendSmsMessage(smsRequest);
                Logger.Info($"Wysłano SMS z komunikatem błędu.");
            }
            catch (ApiException apiException)
            {
                Logger.Info($"Podczas wysyłania SMSa wystąpił błąd: {apiException.ErrorContent}.");
            }

        }

        protected override void OnStop()    
        {
        }
    }
}
