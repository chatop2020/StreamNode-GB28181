﻿//-----------------------------------------------------------------------------
// Filename: SIPTransferServerUserAgent.cs
//
// Description: The interface definition for SIP Server User Agents (UAC).
// 
// History:
// 21 Jan 2010	Aaron Clauson	    Created.
//
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//


using System;
using System.Net;
using SIPSorcery.SIP;

namespace GB28181.App
{
    /// <summary>
    /// A server user agent that replaces an existing sip dialogue rather than creating a new dialogue with
    /// a client user agent.
    /// </summary>
    public class SIPTransferServerUserAgent : ISIPServerUserAgent
    {
        //   private static ILog logger = AppState.logger;

        public SIPCallDirection CallDirection
        {
            get { return SIPCallDirection.Out; }
        }

        public SIPDialogue SIPDialogue
        {
            get { throw new NotImplementedException("SIPTransferServerUserAgent SIPDialogue"); }
        }

        public SIPAccount SIPAccount
        {
            get { return null; }
            set { throw new NotImplementedException("SIPTransferServerUserAgent SIPAccount set"); }
        }

        public bool IsAuthenticated
        {
            get { throw new NotImplementedException("SIPTransferServerUserAgent IsAuthenticated get"); }
            set { throw new NotImplementedException("SIPTransferServerUserAgent IsAuthenticated set"); }
        }

        public bool IsB2B
        {
            get { return false; }
        }

        public bool IsInvite
        {
            get { return true; }
        }

        public SIPRequest CallRequest
        {
            get { return m_dummyRequest; }
        }

        public string CallDestination
        {
            get { return m_callDestination; }
        }

        public bool IsUASAnswered
        {
            get { return m_answered; }
        }

        public string Owner
        {
            get { return m_owner; }
        }

        private BlindTransferDelegate BlindTransfer_External;
        private SIPTransport m_sipTransport;

        private SIPEndPoint
            m_outboundProxy; // If the system needs to use an outbound proxy for every request this will be set and overrides any user supplied values.

        private SIPDialogue m_dialogueToReplace;
        private SIPDialogue m_oppositeDialogue;
        private string m_owner;
        private string m_adminID;
        private bool m_answered;
        private string m_callDestination;
        private SIPRequest m_dummyRequest; // Used to get the SDP for into the dialplan.

        public event SIPUASDelegate CallCancelled;
        public event SIPUASDelegate NoRingTimeout;
        public event SIPUASDelegate TransactionComplete;
        public event SIPUASStateChangedDelegate UASStateChanged;


        public SIPTransferServerUserAgent(
            BlindTransferDelegate blindTransfer,
            SIPTransport sipTransport,
            SIPEndPoint outboundProxy,
            SIPDialogue dialogueToReplace,
            SIPDialogue oppositeDialogue,
            string callDestination,
            string owner,
            string adminID)
        {
            BlindTransfer_External = blindTransfer;
            m_sipTransport = sipTransport;
            m_outboundProxy = outboundProxy;
            m_dialogueToReplace = dialogueToReplace;
            m_oppositeDialogue = oppositeDialogue;
            m_callDestination = callDestination;
            m_owner = owner;
            m_adminID = adminID;
            m_dummyRequest = CreateDummyRequest(m_dialogueToReplace, m_callDestination);
        }

        public void Progress(SIPResponseStatusCodesEnum progressStatus, string reasonPhrase, string[] customHeaders,
            string progressContentType, string progressBody)
        {
            UASStateChanged?.Invoke(this, progressStatus, reasonPhrase);

            if (progressBody != null)
            {
                // Re-invite the remote dialogue so that they can listen to some real progress tones.
            }
        }

        public SIPDialogue Answer(string contentType, string body, SIPDialogue answeredDialogue,
            SIPDialogueTransferModesEnum transferMode)
        {
            return Answer(contentType, body, null, answeredDialogue, transferMode);
        }

        /// <summary>
        /// An answer on a blind transfer means the remote end of the dialogue being replaced should be re-invited and then the replaced dialogue 
        /// should be hungup.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="body"></param>
        /// <param name="toTag"></param>
        /// <param name="answeredDialogue"></param>
        /// <param name="transferMode"></param>
        /// <returns></returns>
        public SIPDialogue Answer(string contentType, string body, string toTag, SIPDialogue answeredDialogue,
            SIPDialogueTransferModesEnum transferMode)
        {
            try
            {
                if (m_answered)
                {
                    answeredDialogue.Hangup(m_sipTransport, m_outboundProxy);
                }
                else
                {
                    m_answered = true;

                    UASStateChanged?.Invoke(this, SIPResponseStatusCodesEnum.Ok, null);

                    BlindTransfer_External(m_dialogueToReplace, m_oppositeDialogue, answeredDialogue);
                }

                TransactionComplete.Invoke(this);
                return null;
            }
            catch (Exception excp)
            {
                Logger.Logger.Error("Exception SIPTransferServerUserAgent Answer. ->" + excp.Message);
                throw;
            }
        }

        public void AnswerNonInvite(SIPResponseStatusCodesEnum answerStatus, string reasonPhrase,
            string[] customHeaders, string contentType, string body)
        {
            throw new NotImplementedException();
        }

        public void Reject(SIPResponseStatusCodesEnum failureStatus, string reasonPhrase, string[] customHeaders)
        {
            Logger.Logger.Warn("SIPTransferServerUserAgent Reject called with " + failureStatus + " " + reasonPhrase +
                               ".");

            UASStateChanged?.Invoke(this, failureStatus, reasonPhrase);
        }

        public void Redirect(SIPResponseStatusCodesEnum redirectCode, SIPURI redirectURI)
        {
            NoRingTimeout.Invoke(this);
            throw new NotImplementedException("SIPTransferServerUserAgent Redirect");
        }

        public void NoCDR()
        {
            throw new NotImplementedException("SIPTransferServerUserAgent NoCDR");
        }

        public void SetTraceDelegate(SIPTransactionTraceMessageDelegate traceDelegate)
        {
            //throw new NotImplementedException();
        }

        public bool LoadSIPAccountForIncomingCall()
        {
            throw new NotImplementedException("SIPTransferServerUserAgent LoadSIPAccountForIncomingCall");
        }

        public bool AuthenticateCall()
        {
            throw new NotImplementedException("SIPTransferServerUserAgent AuthenticateCall");
        }

        public void SetOwner(string owner, string adminMemberId)
        {
            m_owner = owner;
            m_adminID = adminMemberId;
        }

        private SIPRequest CreateDummyRequest(SIPDialogue dialogueToReplace, string callDestination)
        {
            SIPRequest dummyInvite = new SIPRequest(SIPMethodsEnum.INVITE,
                SIPURI.ParseSIPURIRelaxed(callDestination + "@sipsorcery.com"));
            SIPHeader dummyHeader = new SIPHeader("<sip:anon@sipsorcery.com>", "<sip:anon@sipsorcery.com>", 1,
                CallProperties.CreateNewCallId())
            {
                CSeqMethod = SIPMethodsEnum.INVITE
            };
            dummyHeader.Vias.PushViaHeader(new SIPViaHeader(new IPEndPoint(SIPTransport.BlackholeAddress, 0),
                CallProperties.CreateBranchId()));
            dummyInvite.Header = dummyHeader;
            dummyInvite.Header.ContentType = "application/sdp";
            dummyInvite.Body = dialogueToReplace.SDP;

            return dummyInvite;
        }

        public void SetDialPlanContextID(Guid dialPlanContextID)
        {
            throw new NotImplementedException("SIPTransferServerUserAgent SetDialPlanContextID");
        }

        /// <summary>
        /// Fired when a transfer is pending and the call leg that is going to be bridged hangs up before the transfer completes.
        /// </summary>
        public void PendingLegHungup()
        {
            try
            {
                CallCancelled?.Invoke(this);
            }
            catch (Exception excp)
            {
                Logger.Logger.Error("Exception PendingLegHungup. ->" + excp);
            }
        }
    }
}