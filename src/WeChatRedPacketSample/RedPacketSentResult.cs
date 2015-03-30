
namespace WeChatRedPacketSample
{
    public class RedPacketSentResult
    {
        public bool Succeeded { get; set; }

        public string Response { get; set; }

        public RedPacketSentError Error { get; set; }

        public int Amount { get; set; }

        public string BillNumber { get; set; }

    }
}
