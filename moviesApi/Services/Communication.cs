using AfricasTalkingCS;
using moviesApi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace moviesApi.Services
{
    public  class Communication
    {
        public static void sendSMs(string phoneNumber, string message)
        {

            var username = SmsConstants.userId;
            var apiKey = SmsConstants.userKey;

          
            phoneNumber = "+254"+phoneNumber.Substring(1);

            var gateway = new AfricasTalkingGateway(username, apiKey);

            try
            {
                var sms = gateway.SendMessage(phoneNumber, message);
            }
            catch (AfricasTalkingGatewayException exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
