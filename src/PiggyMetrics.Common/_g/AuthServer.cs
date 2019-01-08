// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: services/auth.proto
#region Designer generated code

using System; 
using System.Threading.Tasks; 
using DotBPE.Rpc; 
using DotBPE.Protocol.Amp; 
using Google.Protobuf; 

namespace PiggyMetrics.Common {

//start for class AbstractAuthService
public abstract class AuthServiceBase : ServiceActorBase 
{
public override string Id => "1002$0";
//调用委托
private async Task<AmpMessage> ProcessCreateAsync(AmpMessage req)
{
UserReq request = null;
if(req.Data == null ){
   request = new UserReq();
}
else {
request = UserReq.Parser.ParseFrom(req.Data);
}
var data = await CreateAsync(request);
var response = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
response.Sequence = req.Sequence;
response.Data = data.ToByteArray();
return response;
}

//抽象方法
public abstract Task<VoidRsp> CreateAsync(UserReq request);
//调用委托
private async Task<AmpMessage> ProcessAuthAsync(AmpMessage req)
{
UserReq request = null;
if(req.Data == null ){
   request = new UserReq();
}
else {
request = UserReq.Parser.ParseFrom(req.Data);
}
var data = await AuthAsync(request);
var response = AmpMessage.CreateResponseMessage(req.ServiceId, req.MessageId);
response.Sequence = req.Sequence;
response.Data = data.ToByteArray();
return response;
}

//抽象方法
public abstract Task<AuthRsp> AuthAsync(UserReq request);
public override Task<AmpMessage> ProcessAsync(AmpMessage req)
{
switch(req.MessageId){
//方法AuthService.Create
case 1: return this.ProcessCreateAsync(req);
//方法AuthService.Auth
case 2: return this.ProcessAuthAsync(req);
default: return base.ProcessNotFoundAsync(req);
}
}
}
//end for class AbstractAuthService
}

#endregion
