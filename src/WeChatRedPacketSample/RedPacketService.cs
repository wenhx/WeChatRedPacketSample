using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WeChatRedPacketSample
{
    public class RedPacketService
    {
        static readonly Random _Random = new Random();
        readonly string m_PayKey;
        readonly string m_MerchantId;
        readonly IWeChatClient m_WeChatClient;

        static RedPacketService()
        {
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
        }

        public RedPacketService(string merchantId, string payKey, IWeChatClient client)
        {
            if (merchantId == null)
                throw new ArgumentNullException("merchantId");
            if (payKey == null)
                throw new ArgumentNullException("payKey");
            if (client == null)
                throw new ArgumentNullException("client");

            m_PayKey = payKey;
            m_WeChatClient = client;
        }

        public async Task<RedPacketSentResult> SendAsync(RedPacket request)
        {
            if (request == null)
                throw new ArgumentNullException("request");
            if (!request.IsValid())
                throw new ArgumentException("request");

            var amount = GetAmount(request.Amount);
            var amountString = amount.ToString();
            var sendData = new SortedList<string, string>(StringComparer.Ordinal);
            sendData.Add("nonce_str", Guid.NewGuid().ToString("N"));
            sendData.Add("mch_billno", request.BillNumber);
            sendData.Add("mch_id", m_MerchantId);
            sendData.Add("wxappid", request.AppId);
            sendData.Add("nick_name", request.SendName); //简单起见使用send_name。
            sendData.Add("send_name", request.SendName);
            sendData.Add("re_openid", request.OpenId);
            sendData.Add("total_amount", amountString);
            sendData.Add("min_value", amountString);
            sendData.Add("max_value", amountString);
            sendData.Add("total_num", "1");
            sendData.Add("wishing", request.Wishing);
            sendData.Add("client_ip", request.IpAddress);
            sendData.Add("act_name", request.ActName);
            sendData.Add("remark", request.Remark);
            var xml = DictionaryToXml(sendData);

            var result = await m_WeChatClient.PostAsync(xml);
            var result2 = Parse(result);
            result2.Amount = amount;
            result2.BillNumber = request.BillNumber;
            return result2;
        }

        public RedPacketSentResult Parse(string result)
        {
            try
            {
                var xml = XDocument.Parse(result).Root;
                var returnCode = xml.Element("return_code").Value;
                var balanceNotEnough = string.Equals(xml.Element("err_code").Value, "NOTENOUGH", StringComparison.OrdinalIgnoreCase);
                var error = balanceNotEnough ? RedPacketSentError.BalanceNotEnough : RedPacketSentError.Other;
                if (!string.Equals(returnCode, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                    return new RedPacketSentResult { Succeeded = false, Response = result, Error = error };

                var resultCode = xml.Element("result_code").Value;
                if (!string.Equals(resultCode, "SUCCESS", StringComparison.OrdinalIgnoreCase))
                    return new RedPacketSentResult { Succeeded = false, Response = result, Error = error };

                return new RedPacketSentResult { Succeeded = true, Response = result }; ;
            }
            catch (Exception ex)
            {
                //Logger.WriteError("解析微信返回结果时发生错误", ex);
                return new RedPacketSentResult { Succeeded = false, Response = result, Error = RedPacketSentError.InternalError };
            }
        }

        internal int GetAmount(int amount)
        {
            var maxAmount = 20000;
            if (amount == 0)
                return _Random.Next(100, maxAmount);//微信红包必须大于100分。
            else
            {
                return amount > maxAmount ? maxAmount : amount;
            }
        }

        //默认情况下只能使用受信任的证书，此方法可以干预这个逻辑。
        static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

        string Sign(IDictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in dict)
            {
                if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value))
                    continue;

                sb.AppendFormat("{0}={1}&", item.Key, item.Value);
            }
            sb.Append("key=" + m_PayKey);
            var bytesToHash = Encoding.UTF8.GetBytes(sb.ToString()); //注意，必须是UTF-8。
            var hashResult = ComputeMD5Hash(bytesToHash);
            var hash = BytesToString(hashResult, false);
            return hash;
        }

        static byte[] ComputeMD5Hash(byte[] input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            using (var md5 = new MD5CryptoServiceProvider())
            {
                var result = md5.ComputeHash(input);
                return result;
            }
        }

        public static string BytesToString(byte[] input, bool lowercase = true)
        {
            if (input == null || input.Length == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder(input.Length * 2);
            for (var i = 0; i < input.Length; i++)
            {
                sb.AppendFormat(lowercase ? "{0:x2}" : "{0:X2}", input[i]);
            }
            return sb.ToString();
        }

        string DictionaryToXml(IDictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<xml>");
            foreach (var item in dict)
            {
                if (string.IsNullOrEmpty(item.Key) || string.IsNullOrEmpty(item.Value))
                    continue;

                sb.AppendFormat("<{0}>{1}</{0}>", item.Key, item.Value);
            }
            var sign = Sign(dict);
            sb.AppendFormat("<sign>{0}</sign>", sign);
            sb.AppendLine("</xml>");
            return sb.ToString();
        }
    }
}
