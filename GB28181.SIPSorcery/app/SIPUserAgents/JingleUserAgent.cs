using System;
using System.Net;
using GB28181.Net;
using SIPSorcery.SIP;

namespace GB28181.App
{
    public class JingleUserAgent : ISIPClientUserAgent
    {
        private static string m_CRLF = SDP.CRLF;


        //  private XMPPClient m_xmppClient;

        public string Owner { get; private set; }
        public string AdminMemberId { get; private set; }
        public UACInviteTransaction ServerTransaction { get; private set; }
        public SIPDialogue SIPDialogue { get; private set; }
        public SIPCallDescriptor CallDescriptor { get; private set; }
        public bool IsUACAnswered { get; private set; }

        public event SIPCallResponseDelegate CallTrying;
        public event SIPCallResponseDelegate CallRinging;
        public event SIPCallResponseDelegate CallAnswered;
        public event SIPCallFailedDelegate CallFailed;

        public JingleUserAgent(
            string owner,
            string adminMemberId
        )
        {
            Owner = owner;
            AdminMemberId = adminMemberId;
        }

        public void Call(SIPCallDescriptor sipCallDescriptor)
        {
            //CallDescriptor = sipCallDescriptor;
            //m_xmppClient = new XMPPClient("talk.google.com", 5222, "google.com", null, null);
            //m_xmppClient.IsBound += IsBound;
            //m_xmppClient.Answered += Answered;

            //IPEndPoint sdpEndPoint = SDP.GetSDPRTPEndPoint(CallDescriptor.Content);
            //if (IPSocket.IsPrivateAddress(sdpEndPoint.Address.ToString()))
            //{
            //    bool wasSDPMangled;
            //    CallDescriptor.Content = SIPPacketMangler.MangleSDP(CallDescriptor.Content, CallDescriptor.MangleIPAddress.ToString(), out wasSDPMangled);
            //}

            //ThreadPool.QueueUserWorkItem(delegate { m_xmppClient.Connect(); });
        }

        private void IsBound()
        {
            Console.WriteLine("XMPP client IsBound");
            //   m_xmppClient.PlaceCall(SIPURI.ParseSIPURI(CallDescriptor.To).User + "@voice.google.com", SDP.ParseSDPDescription(CallDescriptor.Content));
        }

        private void Answered(SDP sdp)
        {
            Console.WriteLine("XMPP client call answered.");

            IsUACAnswered = true;

            SIPResponse okResponse = new SIPResponse(SIPResponseStatusCodesEnum.Ok, "Ok",
                new SIPEndPoint(new IPEndPoint(IPAddress.Loopback, 0)));
            okResponse.Header.ContentType = SDP.SDP_MIME_CONTENTTYPE;
            okResponse.Body = sdp.ToString();

            SIPDialogue = new SIPDialogue(null, null, null, null, -1, null, null, null, Guid.NewGuid(), Owner,
                AdminMemberId, null, sdp.ToString());
            SIPDialogue.CallDurationLimit = CallDescriptor.CallDurationLimit;

            CallAnswered(this, okResponse);
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void Update(CRMHeaders crmHeaders)
        {
            throw new NotImplementedException();
        }
    }
}