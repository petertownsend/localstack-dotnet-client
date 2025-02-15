﻿namespace LocalStack.Client.Utils;

public class SessionReflection : ISessionReflection
{
    public IServiceMetadata ExtractServiceMetadata<TClient>() where TClient : AmazonServiceClient
    {
        Type clientType = typeof(TClient);

        return ExtractServiceMetadata(clientType);
    }

    public IServiceMetadata ExtractServiceMetadata(Type clientType)
    {
        FieldInfo serviceMetadataField = clientType.GetField("serviceMetadata", BindingFlags.Static | BindingFlags.NonPublic) ??
                                         throw new InvalidOperationException($"Invalid service type {clientType}");

        var serviceMetadata = (IServiceMetadata) serviceMetadataField.GetValue(null);

        return serviceMetadata;
    }

    public ClientConfig CreateClientConfig<TClient>() where TClient : AmazonServiceClient
    {
        Type clientType = typeof(TClient);

        return CreateClientConfig(clientType);
    }

    public ClientConfig CreateClientConfig(Type clientType)
    {
        ConstructorInfo clientConstructorInfo = FindConstructorWithCredentialsAndClientConfig(clientType);
        ParameterInfo clientConfigParam = clientConstructorInfo.GetParameters()[1];

        return (ClientConfig) Activator.CreateInstance(clientConfigParam.ParameterType);
    }


    public bool SetForcePathStyle(ClientConfig clientConfig, bool value = true)
    {
        PropertyInfo forcePathStyleProperty = clientConfig.GetType().GetProperty("ForcePathStyle", BindingFlags.Public | BindingFlags.Instance);

        if (forcePathStyleProperty == null)
        {
            return false;
        }

        forcePathStyleProperty.SetValue(clientConfig, value);

        return true;
    }

    private static ConstructorInfo FindConstructorWithCredentialsAndClientConfig(Type clientType)
    {
        return clientType.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                         .Where(info =>
                         {
                             ParameterInfo[] parameterInfos = info.GetParameters();

                             if (parameterInfos.Length != 2)
                             {
                                 return false;
                             }

                             ParameterInfo credentialsParameter = parameterInfos[0];
                             ParameterInfo clientConfigParameter = parameterInfos[1];

                             return credentialsParameter.Name == "credentials" &&
                                    credentialsParameter.ParameterType == typeof(AWSCredentials) &&
                                    clientConfigParameter.Name == "clientConfig" &&
                                    clientConfigParameter.ParameterType.IsSubclassOf(typeof(ClientConfig));
                         })
                         .Single();
    }
}