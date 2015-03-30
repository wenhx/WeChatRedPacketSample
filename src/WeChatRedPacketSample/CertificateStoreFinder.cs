using System;
using System.Security.Cryptography.X509Certificates;

namespace WeChatRedPacketSample
{
    public class CertificateStoreFinder : ICertificateFinder
    {
        string m_SubjectDistinguishedName;

        public CertificateStoreFinder(string subjectDistinguishedName)
        {
            if (subjectDistinguishedName == null)
                throw new ArgumentNullException("subjectDistinguishedName");

            m_SubjectDistinguishedName = subjectDistinguishedName;
        }

        public X509Certificate2 Find()
        {
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            //你可以采用别的方式来寻找证书，只要能找到就可以。
            var certs = store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, m_SubjectDistinguishedName, false);
            if (certs.Count == 0)
                throw new Exception("无法找到微信支付证书");

            return certs[0];
        }
    }
}
