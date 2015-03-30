using System.Threading.Tasks;

namespace WeChatRedPacketSample
{
    public class DebugWeChatClient : IWeChatClient
    {
        public static readonly string DefaultResponse = @"<xml>
        <debug>DebugWeChatClient</debug>
        <return_code><![CDATA[SUCCESS]]></return_code>
        <return_msg><![CDATA[发放成功.]]></return_msg>
        <result_code><![CDATA[SUCCESS]]></result_code>
        <err_code><![CDATA[0]]></err_code>
        <err_code_des><![CDATA[发放成功.]]></err_code_des>
        <mch_billno><![CDATA[1233006302201503230000000001]]></mch_billno>
        <mch_id>1233006302</mch_id>
        <wxappid><![CDATA[wxc5e8d0e4826b8dad]]></wxappid>
        <re_openid><![CDATA[ooECzjkJonPgp6b_g91ehHst-Op8]]></re_openid>
        <total_amount>375</total_amount>
        </xml>";
        string m_Result;

        public DebugWeChatClient(string result = null)
        {
            m_Result = result ?? DefaultResponse;
        }

        public Task<string> PostAsync(string data)
        {
            return Task.FromResult(m_Result);
        }
    }
}
