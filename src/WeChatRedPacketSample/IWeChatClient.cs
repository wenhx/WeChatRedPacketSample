using System.Threading.Tasks;

namespace WeChatRedPacketSample
{
    public interface IWeChatClient
    {
        Task<string> PostAsync(string data);
    }
}
