// ============================================================================
// FileName: RegistrarCore.cs
//
// Description:
// SIP Registrar that strives to be RFC3822 compliant.
//
// Author(s):
// Aaron Clauson
//
// History:
// 21 Jan 2006	Aaron Clauson	Created.
// 22 Nov 2007  Aaron Clauson   Fixed bug where binding refresh was generating a duplicate exception if the uac endpoint changed but the contact did not.
//
// License: 
// BSD 3-Clause "New" or "Revised" License, see included LICENSE.md file.
//
// Copyright (c) 2006-2007 Aaron Clauson (aaronc@blueface.ie), Blue Face Ltd, Dublin, Ireland (www.blueface.ie)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
// the following conditions are met:
//
// Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 
// Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following 
// disclaimer in the documentation and/or other materials provided with the distribution. Neither the name of Blue Face Ltd. 
// nor the names of its contributors may be used to endorse or promote products derived from this software without specific 
// prior written permission. 
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, 
// BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. 
// IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, 
// OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, 
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GB28181.App;
using GB28181.Cache;
using GB28181.Config;
using GB28181.Sys;
using GB28181.Sys.Model;
using SIPSorcery.SIP;
using SIPSorcery.Sys;

#if UNITTEST
using NUnit.Framework;
#endif

namespace GB28181.Servers
{
    public enum RegisterResultEnum
    {
        Unknown = 0,
        Trying = 1,
        Forbidden = 2,
        Authenticated = 3,
        AuthenticationRequired = 4,
        Failed = 5,
        Error = 6,
        RequestWithNoUser = 7,
        RemoveAllRegistrations = 9,
        DuplicateRequest = 10,
        AuthenticatedFromCache = 11,
        RequestWithNoContact = 12,
        NonRegisterMethod = 13,
        DomainNotServiced = 14,
        IntervalTooBrief = 15,
        SwitchboardPaymentRequired = 16,
    }


    public class SIPRegistrarCore : ISIPRegistrarCore
    {
        private const int MAX_REGISTER_QUEUE_SIZE = 1000;
        private const int MAX_PROCESS_REGISTER_SLEEP = 10000;
        private const string REGISTRAR_THREAD_NAME_PREFIX = "sipregistrar-core";

        //  private static ILog logger = AppState.GetLogger("sipregistrar");

        //最小的注册有效期，30秒
        public const int MINIMUM_EXPIRY_SECONDS = 30;
        private int m_minimumBindingExpiry = MINIMUM_EXPIRY_SECONDS;

        private ISIPTransport _sipTransport;

        private SIPAuthenticateRequestDelegate _sipRequestAuthenticator_External =
            SIPRequestAuthenticator.AuthenticateSIPRequest;

        private string m_serverAgent = SIPConstants.SIP_USERAGENT_STRING;
        private bool m_mangleUACContact = false;
        private bool m_strictRealmHandling = false;
        private Queue<SIPNonInviteTransaction> m_registerQueue = new Queue<SIPNonInviteTransaction>();
        private AutoResetEvent m_registerARE = new AutoResetEvent(false);

        public event Action<double, bool> RegisterComplete;

        public int BacklogLength => m_registerQueue.Count;

        public bool Stop;

        private bool _needAuthentication = false;
        public bool IsNeedAuthentication => _needAuthentication;

        private SIPAccount _localSipAccount;

        private IMemoCache<Camera> _cameraCache = null;

        /// <summary>
        /// 设备注册时
        /// </summary>
        public event RegisterDelegate RegisterReceived;

        /// <summary>
        /// 设备注销时
        /// </summary>
        public event UnRegisterDelegate UnRegisterReceived;

        /// <summary>
        /// 设备有警告时
        /// </summary>
        public event DeviceAlarmSubscribeDelegate DeviceAlarmSubscribe;

        public SIPRegistrarCore(ISIPTransport sipTransport, ISipStorage sipAccountStorage,
            IMemoCache<Camera> cameraCache, bool mangleUACContact = true, bool strictRealmHandling = true)
        {
            _sipTransport = sipTransport;
            m_mangleUACContact = mangleUACContact;
            m_strictRealmHandling = strictRealmHandling;
            _localSipAccount = sipAccountStorage.GetLocalSipAccout();
            _needAuthentication = _localSipAccount.Authentication;
            _cameraCache = cameraCache;
        }

        public void AddRegisterRequest(SIPEndPoint localSIPEndPoint, SIPEndPoint remoteEndPoint,
            SIPRequest registerRequest)
        {
            try
            {
                if (registerRequest.Method == SIPMethodsEnum.REGISTER)
                {
                    SIPSorceryPerformanceMonitor.IncrementCounter(SIPSorceryPerformanceMonitor
                        .REGISTRAR_REGISTRATION_REQUESTS_PER_SECOND);

                    int requestedExpiry = GetRequestedExpiry(registerRequest);

                    if (registerRequest.Header.To == null)
                    {
                        Logger.Logger.Debug("Bad register request, no To header from " + remoteEndPoint + ".");
                        SIPResponse badReqResponse = SIPTransport.GetResponse(registerRequest,
                            SIPResponseStatusCodesEnum.BadRequest, "Missing To header");
                        _sipTransport.SendResponse(badReqResponse);
                    }
                    else if (registerRequest.Header.To.ToURI.User.IsNullOrBlank())
                    {
                        Logger.Logger.Debug("Bad register request, no To user from " + remoteEndPoint + ".");
                        SIPResponse badReqResponse = SIPTransport.GetResponse(registerRequest,
                            SIPResponseStatusCodesEnum.BadRequest, "Missing username on To header");
                        _sipTransport.SendResponse(badReqResponse);
                    }
                    else if (registerRequest.Header.Contact == null || registerRequest.Header.Contact.Count == 0)
                    {
                        Logger.Logger.Debug("Bad register request, no Contact header from " + remoteEndPoint + ".");
                        SIPResponse badReqResponse = SIPTransport.GetResponse(registerRequest,
                            SIPResponseStatusCodesEnum.BadRequest, "Missing Contact header");
                        _sipTransport.SendResponse(badReqResponse);
                    }
                    else if (requestedExpiry > 0 && requestedExpiry < m_minimumBindingExpiry)
                    {
                        Logger.Logger.Debug("Bad register request, no expiry of " + requestedExpiry +
                                            " to small from " +
                                            remoteEndPoint + ".");
                        SIPResponse tooFrequentResponse = GetErrorResponse(registerRequest,
                            SIPResponseStatusCodesEnum.IntervalTooBrief, null);
                        tooFrequentResponse.Header.MinExpires = m_minimumBindingExpiry;
                        _sipTransport.SendResponse(tooFrequentResponse);
                    }
                    else
                    {
                        if (m_registerQueue.Count < MAX_REGISTER_QUEUE_SIZE)
                        {
                            var registrarTransaction = _sipTransport.CreateNonInviteTransaction(registerRequest,
                                remoteEndPoint, localSIPEndPoint, null);
                            lock (m_registerQueue)
                            {
                                m_registerQueue.Enqueue(registrarTransaction);
                            }
                        }
                        else
                        {
                            Logger.Logger.Error("Register queue exceeded max queue size " + MAX_REGISTER_QUEUE_SIZE +
                                                ", overloaded response sent.");
                            SIPResponse overloadedResponse = SIPTransport.GetResponse(registerRequest,
                                SIPResponseStatusCodesEnum.TemporarilyUnavailable,
                                "Registrar overloaded, please try again shortly");
                            _sipTransport.SendResponse(overloadedResponse);
                        }

                        m_registerARE.Set();
                    }
                }
            }
            catch (Exception excp)
            {
                Logger.Logger.Error("Exception AddRegisterRequest (" + remoteEndPoint.ToString() + "). ->" +
                                    excp.Message);
            }
        }

        public void ProcessRegisterRequest()
        {
            Logger.Logger.Debug("SIPRegistrarCore is running at " + _localSipAccount.MsgProtocol + ":" +
                                _localSipAccount.LocalIP + ":" + _localSipAccount.LocalPort);
            try
            {
                while (!Stop)
                {
                    if (m_registerQueue.Count > 0)
                    {
                        try
                        {
                            SIPNonInviteTransaction registrarTransaction = null;
                            lock (m_registerQueue)
                            {
                                registrarTransaction = m_registerQueue.Dequeue();
                            }

                            if (registrarTransaction != null)
                            {
                                DateTime startTime = DateTime.Now;
                                var result = Register(registrarTransaction);
                                var duration = DateTime.Now.Subtract(startTime);

                                RegisterComplete?.Invoke(duration.TotalMilliseconds,
                                    registrarTransaction.TransactionRequest.Header.AuthenticationHeader != null);

                                Logger.Logger.Debug("Camera[" + registrarTransaction.RemoteEndPoint + " deviceid:" +
                                                    registrarTransaction.TransactionRequest.Header.To.ToURI.User +
                                                    "] have completed registering GB service.");
                                //CacheDeviceItem(registrarTransaction.TransactionRequest);

                                //device alarm subscribe
                                DeviceAlarmSubscribe?.Invoke(registrarTransaction);
                            }
                        }
                        catch (InvalidOperationException invalidOpExcp)
                        {
                            // This occurs when the queue is empty.
                            Logger.Logger.Error("InvalidOperationException ProcessRegisterRequest Register Job. ->" +
                                                invalidOpExcp.Message);
                        }
                        catch (Exception regExcp)
                        {
                            Logger.Logger.Error("Exception ProcessRegisterRequest Register Job. ->" + regExcp.Message);
                        }
                    }
                    else
                    {
                        m_registerARE.WaitOne(MAX_PROCESS_REGISTER_SLEEP);
                    }
                }

                Logger.Logger.Warn("ProcessRegisterRequest thread " + Thread.CurrentThread.Name + " stopping.");
            }
            catch (Exception excp)
            {
                Logger.Logger.Error("Exception ProcessRegisterRequest (" + Thread.CurrentThread.Name + "). ->" +
                                    excp.Message);
            }
        }

        private int GetRequestedExpiry(SIPRequest registerRequest)
        {
            int contactHeaderExpiry =
                (registerRequest.Header.Contact != null && registerRequest.Header.Contact.Count > 0)
                    ? registerRequest.Header.Contact[0].Expires
                    : -1;
            return (contactHeaderExpiry == -1) ? registerRequest.Header.Expires : contactHeaderExpiry;
        }


        /// <summary>
        /// 从缓存中移除
        /// </summary>
        /// <param name="sipRequest"></param>
        public void RemoveDeviceItem(SIPRequest sipRequest)
        {
            _cameraCache.TakeOut(sipRequest.URI.Host);
        }

        public void RemoveDeviceItem(string deviceKey)
        {
             _cameraCache.TakeOut(deviceKey);
        }
        public void CacheDeviceItem(SIPRequest sipRequest)
        {
            //Add Camera Item Into Cache
            _cameraCache.PlaceIn(sipRequest.URI.Host, new Camera()
            {
                DeviceID = sipRequest.Header.From.FromURI.User,
                IPAddress = sipRequest.Header.Vias.TopViaHeader.Host,
                Port = sipRequest.Header.Vias.TopViaHeader.Port
            });
        }

        private RegisterResultEnum Register(SIPTransaction registerTransaction)
        {
            try
            {
                SIPRequest sipRequest = registerTransaction.TransactionRequest;
                SIPURI registerURI = sipRequest.URI;
                SIPToHeader toHeader = sipRequest.Header.To;
                string toUser = toHeader.ToURI.User;
                string canonicalDomain = toHeader.ToURI.Host;
                int requestedExpiry = GetRequestedExpiry(sipRequest);

                if (canonicalDomain == null)
                {
                    SIPResponse noDomainResponse = GetErrorResponse(sipRequest, SIPResponseStatusCodesEnum.Forbidden,
                        "Domain not serviced");
                    registerTransaction.SendFinalResponse(noDomainResponse);
                    return RegisterResultEnum.DomainNotServiced;
                }

                SIPAccount sipAccount = new SIPAccount
                {
                    Id = Guid.NewGuid(),
                    Owner = "admin",
                    SIPUsername = toUser,
                    SIPDomain = canonicalDomain
                };
                SIPRequestAuthenticationResult authenticationResult =
                    _sipRequestAuthenticator_External?.Invoke(registerTransaction.LocalSIPEndPoint,
                        registerTransaction.RemoteEndPoint, sipRequest, sipAccount);

                if (!_needAuthentication)
                {
                    SIPResponse okRes = GetOkResponse(sipRequest);

                    registerTransaction.SendFinalResponse(okRes);


                    if (requestedExpiry > 0)
                    {
                        CacheDeviceItem(sipRequest);

                        RegisterReceived?.Invoke(sipRequest, _localSipAccount);
                    }

                    if (requestedExpiry == 0)
                    {
                        RemoveDeviceItem(sipRequest);
                        UnRegisterReceived?.Invoke(sipRequest, _localSipAccount);
                    }


                    return RegisterResultEnum.AuthenticationRequired;
                }

                if (!authenticationResult.Authenticated)
                {
                    // 401 Response with a fresh nonce needs to be sent.
                    SIPResponse authReqdResponse =
                        SIPTransport.GetResponse(sipRequest, authenticationResult.ErrorResponse, null);
                    authReqdResponse.Header.AuthenticationHeader = authenticationResult.AuthenticationRequiredHeader;
                    registerTransaction.SendFinalResponse(authReqdResponse);

                    if (authenticationResult.ErrorResponse == SIPResponseStatusCodesEnum.Forbidden)
                    {
                        return RegisterResultEnum.Forbidden;
                    }
                    else
                    {
                        return RegisterResultEnum.AuthenticationRequired;
                    }
                }
                else
                {
                    if (sipRequest.Header.Contact == null || sipRequest.Header.Contact.Count == 0)
                    {
                        SIPResponse okResponse = GetOkResponse(sipRequest);

                        registerTransaction.SendFinalResponse(okResponse);


                        if (requestedExpiry > 0)
                        {
                            CacheDeviceItem(sipRequest);
                            RegisterReceived?.Invoke(sipRequest, _localSipAccount);
                        }

                        if (requestedExpiry == 0)
                        {
                            RemoveDeviceItem(sipRequest);
                            UnRegisterReceived?.Invoke(sipRequest, _localSipAccount);
                        }
                    }
                    else
                    {
                        SIPEndPoint uacRemoteEndPoint = SIPEndPoint.TryParse(sipRequest.Header.ProxyReceivedFrom) ??
                                                        registerTransaction.RemoteEndPoint;
                        SIPEndPoint proxySIPEndPoint = SIPEndPoint.TryParse(sipRequest.Header.ProxyReceivedOn);
                        SIPEndPoint registrarEndPoint = registerTransaction.LocalSIPEndPoint;
                        SIPResponseStatusCodesEnum updateResult = SIPResponseStatusCodesEnum.Ok;
                        DateTime startTime = DateTime.Now;
                        TimeSpan duration = DateTime.Now.Subtract(startTime);

                        if (updateResult == SIPResponseStatusCodesEnum.Ok)
                        {
                            string proxySocketStr = (proxySIPEndPoint != null)
                                ? " (proxy=" + proxySIPEndPoint.ToString() + ")"
                                : null;
                            SIPResponse okResponse = GetOkResponse(sipRequest);
                            registerTransaction.SendFinalResponse(okResponse);
                            if (requestedExpiry > 0)
                            {
                                CacheDeviceItem(sipRequest);
                                RegisterReceived?.Invoke(sipRequest, _localSipAccount);
                            }

                            if (requestedExpiry == 0)
                            {
                                RemoveDeviceItem(sipRequest);
                                UnRegisterReceived?.Invoke(sipRequest, _localSipAccount);
                            }
                        }
                        else
                        {
                            sipRequest.Header.Contact[0].Expires = m_minimumBindingExpiry;
                            SIPResponse okResponse = GetOkResponse(sipRequest);
                            registerTransaction.SendFinalResponse(okResponse);
                        }
                    }

                    return RegisterResultEnum.Authenticated;
                }
            }
            catch (Exception excp)
            {
                string regErrorMessage = "Exception registrarcore registering. ->" + excp.Message + "->" +
                                         registerTransaction.TransactionRequest.ToString();
                Logger.Logger.Error(regErrorMessage);

                try
                {
                    SIPResponse errorResponse = GetErrorResponse(registerTransaction.TransactionRequest,
                        SIPResponseStatusCodesEnum.InternalServerError, null);
                    registerTransaction.SendFinalResponse(errorResponse);
                }
                catch
                {
                }

                return RegisterResultEnum.Error;
            }
        }

        private int GetBindingExpiry(List<SIPRegistrarBinding> bindings, string bindingURI)
        {
            if (bindings != null || bindings.Count > 0)
            {
                var target = bindings.FirstOrDefault(item => item.ContactURI == bindingURI);

                if (target != null)
                {
                    return target.Expiry;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets a SIP contact header for this address-of-record based on the bindings list.
        /// </summary>
        /// <returns></returns>
        private List<SIPContactHeader> GetContactHeader(List<SIPRegistrarBinding> bindings)
        {
            if (bindings != null && bindings.Count > 0)
            {
                List<SIPContactHeader> contactHeaderList = new List<SIPContactHeader>();

                foreach (SIPRegistrarBinding binding in bindings)
                {
                    SIPContactHeader bindingContact = new SIPContactHeader(null, binding.ContactSIPURI)
                    {
                        Expires = Convert.ToInt32(binding.ExpiryTime.Subtract(DateTime.UtcNow).TotalSeconds %
                                                  Int32.MaxValue)
                    };
                    contactHeaderList.Add(bindingContact);
                }

                return contactHeaderList;
            }
            else
            {
                return null;
            }
        }

        private SIPResponse GetOkResponse(SIPRequest sipRequest)
        {
            try
            {
                SIPResponse okResponse = SIPTransport.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Ok, null);
                SIPHeader requestHeader = sipRequest.Header;
                okResponse.Header = new SIPHeader(requestHeader.Contact, requestHeader.From, requestHeader.To,
                    requestHeader.CSeq, requestHeader.CallId);

                // RFC3261 has a To Tag on the example in section "24.1 Registration".
                if (okResponse.Header.To.ToTag == null || okResponse.Header.To.ToTag.Trim().Length == 0)
                {
                    okResponse.Header.To.ToTag = CallProperties.CreateNewTag();
                }

                okResponse.Header.CSeqMethod = requestHeader.CSeqMethod;
                okResponse.Header.Vias = requestHeader.Vias;
                //okResponse.Header.Server = m_serverAgent;
                okResponse.Header.UserAgent = m_serverAgent;
                okResponse.Header.MaxForwards = Int32.MinValue;
                okResponse.Header.SetDateHeader();

                return okResponse;
            }
            catch (Exception excp)
            {
                Logger.Logger.Error("Exception GetOkResponse. ->" + excp.Message);
                throw excp;
            }
        }

        private SIPResponse GetAuthReqdResponse(SIPRequest sipRequest, string nonce, string realm)
        {
            try
            {
                SIPResponse authReqdResponse =
                    SIPTransport.GetResponse(sipRequest, SIPResponseStatusCodesEnum.Unauthorised, null);
                SIPAuthenticationHeader authHeader =
                    new SIPAuthenticationHeader(SIPAuthorisationHeadersEnum.WWWAuthenticate, realm, nonce);
                SIPHeader requestHeader = sipRequest.Header;
                SIPHeader unauthHeader = new SIPHeader(requestHeader.Contact, requestHeader.From, requestHeader.To,
                    requestHeader.CSeq, requestHeader.CallId);

                if (unauthHeader.To.ToTag == null || unauthHeader.To.ToTag.Trim().Length == 0)
                {
                    unauthHeader.To.ToTag = CallProperties.CreateNewTag();
                }

                unauthHeader.CSeqMethod = requestHeader.CSeqMethod;
                unauthHeader.Vias = requestHeader.Vias;
                unauthHeader.AuthenticationHeader = authHeader;
                //unauthHeader.Server = m_serverAgent;
                unauthHeader.UserAgent = m_serverAgent;
                unauthHeader.MaxForwards = Int32.MinValue;

                authReqdResponse.Header = unauthHeader;

                return authReqdResponse;
            }
            catch (Exception excp)
            {
                Logger.Logger.Error("Exception GetAuthReqdResponse. ->" + excp.Message);
                throw excp;
            }
        }

        private SIPResponse GetErrorResponse(SIPRequest sipRequest, SIPResponseStatusCodesEnum errorResponseCode,
            string errorMessage)
        {
            try
            {
                SIPResponse errorResponse = SIPTransport.GetResponse(sipRequest, errorResponseCode, null);
                if (errorMessage != null)
                {
                    errorResponse.ReasonPhrase = errorMessage;
                }

                SIPHeader requestHeader = sipRequest.Header;
                SIPHeader errorHeader = new SIPHeader(requestHeader.Contact, requestHeader.From, requestHeader.To,
                    requestHeader.CSeq, requestHeader.CallId);

                if (errorHeader.To.ToTag == null || errorHeader.To.ToTag.Trim().Length == 0)
                {
                    errorHeader.To.ToTag = CallProperties.CreateNewTag();
                }

                errorHeader.CSeqMethod = requestHeader.CSeqMethod;
                errorHeader.Vias = requestHeader.Vias;
                //errorHeader.Server = m_serverAgent;
                errorHeader.UserAgent = m_serverAgent;
                errorHeader.MaxForwards = Int32.MinValue;

                errorResponse.Header = errorHeader;

                return errorResponse;
            }
            catch (Exception excp)
            {
                Logger.Logger.Error("Exception GetErrorResponse. ->" + excp.Message);
                throw excp;
            }
        }
    }
}