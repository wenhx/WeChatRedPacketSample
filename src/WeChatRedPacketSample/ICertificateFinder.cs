using System.Security.Cryptography.X509Certificates;

namespace WeChatRedPacketSample
{
    public interface ICertificateFinder
    {
        X509Certificate2 Find();
    }
}
