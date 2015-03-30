
namespace WeChatRedPacketSample
{
    public class RedPacket
    {
        public string BillNumber { get; set; }
        public string SendName { get; set; }
        public string ActName { get; set; }
        public string Wishing { get; set; }
        public string Remark { get; set; }
        public string AppId { get; set; }
        public string OpenId { get; set; }
        public string IpAddress { get; set; }
        public int Amount { get; set; }

        public bool IsValid() //可以用ModelBinding机制来替换这个。
        {
            return !string.IsNullOrWhiteSpace(BillNumber) &&
                !string.IsNullOrWhiteSpace(SendName) &&
                !string.IsNullOrWhiteSpace(ActName) &&
                !string.IsNullOrWhiteSpace(Wishing) &&
                !string.IsNullOrWhiteSpace(Remark) &&
                !string.IsNullOrWhiteSpace(AppId) &&
                !string.IsNullOrWhiteSpace(OpenId) &&
                !string.IsNullOrWhiteSpace(IpAddress) &&
                ((Amount == 0) || (Amount != 0 && Amount >= 100));
        }
    }
}
