using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PiggyMetrics.Common;
using DotBPE.Plugin.WebApi;
using Google.Protobuf;

namespace PiggyMetrics.HttpApi
{
    public class ForwardService : AbstractForwardService<AmpMessage>
    {
        static readonly JsonFormatter AmpJsonFormatter = new JsonFormatter(new JsonFormatter.Settings(true));

        static ILogger Logger = DotBPE.Rpc.Environment.Logger.ForType<ForwardService>();

        public ForwardService(IRpcClient<AmpMessage> rpcClient, 
            IOptionsSnapshot<WebApiRouterOption> optionsAccessor) : base(rpcClient, optionsAccessor)
        {
        }

        protected override AmpMessage EncodeRequest(RequestData reqData)
        {
            ushort serviceId = (ushort)reqData.ServiceId;
            ushort messageId = (ushort)reqData.MessageId;
            AmpMessage message = AmpMessage.CreateRequestMessage(serviceId, messageId);
            IMessage reqTemp = ProtobufObjectFactory.GetRequestTemplate(serviceId, messageId);
            if (reqTemp == null)
            {
                return null;
            }
            try
            {
                var descriptor = reqTemp.Descriptor;
                if (!string.IsNullOrEmpty(reqData.Body))
                {
                    reqTemp = descriptor.Parser.ParseJson(reqData.Body);
                }

                if (reqData.Data.Count > 0)
                {
                    foreach (var field in descriptor.Fields.InDeclarationOrder())
                    {
                        if (reqData.Data.ContainsKey(field.Name))
                        {
                            field.Accessor.SetValue(reqTemp, reqData.Data[field.Name]);
                        }
                        else if (reqData.Data.ContainsKey(field.JsonName))
                        {
                            field.Accessor.SetValue(reqTemp, reqData.Data[field.JsonName]);
                        }

                    }
                }


                message.Data = reqTemp.ToByteArray();

            }
            catch (Exception ex)
            {
                Logger.Error(ex, "从HTTP请求中解析数据错误:" + ex.Message);
                message = null;
            }

            return message;

        }

        protected override CallInvoker<AmpMessage> GetProtocolCallInvoker(IRpcClient<AmpMessage> rpcClient)
        {
            return new AmpCallInvoker(rpcClient);
        }

        protected override string MessageToJson(AmpMessage message)
        {
            string ret = "";
            if (message != null)
            {
                var rspTemp = ProtobufObjectFactory.GetResponseTemplate(message.ServiceId, message.MessageId);
                if (rspTemp == null)
                {                  
                    return ret;
                }

                if (message.Data != null)
                {
                    rspTemp.MergeFrom(message.Data);
                }
                ret = AmpJsonFormatter.Format(rspTemp);
            }
            return ret;
        }
    }
}
