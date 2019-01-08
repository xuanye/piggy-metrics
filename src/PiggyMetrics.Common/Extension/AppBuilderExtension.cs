using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.ServiceRegistry;
using Microsoft.Extensions.DependencyInjection;


namespace PiggyMetrics.Common.Extension
{
    public static class AppBuilderExtension
    {
        public static IAppBuilder UseConsuleDiscovery(this IAppBuilder builder){
            builder.ServiceProvider.GetRequiredService<IBridgeRouter<AmpMessage>>(); // 获取示例，内部会自动初始化链接
            return builder;
        }

        public static IAppBuilder UseConsulRegistration(this IAppBuilder builder,
            string serviceName,
            string serviceCategory,
            string localAddress,
            int port)
        {
            // 服务注册
            var localImpls = builder.ServiceProvider.GetServices<IServiceActor<AmpMessage>>();
            var serviceRegistor = builder.ServiceProvider.GetRequiredService<IServiceRegistrationProvider>();

            foreach (var actors in localImpls)
            {
                string hashId = CryptographyManager.Md5Encrypt(localAddress+port);
                ServiceMeta meta = new ServiceMeta
                {
                    Id = hashId + "$" + actors.Id.Split('$')[0], //还要根据IP和端口添加一个MD5
                    ServiceName =serviceName,
                    IPAddress = localAddress,
                    Port = port,
                    Tags = new string[]{serviceCategory}
                };

                serviceRegistor.RegisterAsync(meta).Wait();
            }
            return builder;
        }
    }
}
