using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class MessageController: Controller
    {
        private IOptions<ConfigData> _configData;
        private IMessageFactory _messageFactory;
        private IMessageClient _messageClient;

        public MessageController(IOptions<ConfigData> configData, IMessageFactory mf, IMessageClient mc)
        {
            _configData = configData;
            _messageFactory = mf;
            _messageClient = mc;
        }

        [HttpGet("config")]
        public IEnumerable<string> Config()
        {
            return new string[]
            {
                string.Format("RabbitMqIP:{0}",_configData.Value.RabbitMqIp),
                string.Format("InputQueue:{0}",_configData.Value.InputQueue),
                string.Format("OutputQueue:{0}",_configData.Value.OutputQueue),
                string.Format("Version:{0}",_configData.Value.Version)
            };
        }

        [HttpGet("send/{prefix}")]
        public string Send(string prefix)
        {
            BackgroundJob.Enqueue<IMessageFactory>(s => s.StartGeneratingMsg(prefix));
            return "ok";
        }

        [HttpGet("receive")]
        public string Receive()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                _messageClient.ReciveMsg(a => { sb.Append(a); });
            }
            catch (Exception ex)
            {
                sb.Append(ex);
            }
            return sb.ToString();
        }

    }
}
