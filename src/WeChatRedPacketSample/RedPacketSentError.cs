
namespace WeChatRedPacketSample
{
    public enum RedPacketSentError : byte
    {
        None = 0,
        InternalError = 1,
        BalanceNotEnough = 2,
        Other = 128
    }
}
