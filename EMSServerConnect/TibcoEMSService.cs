using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TIBCO.EMS;

public class TibcoEMSService
{
    private readonly TibcoEMSSettings _settings;

    public TibcoEMSService(IOptions<TibcoEMSSettings> settings)
    {
        _settings = settings.Value;
    }

    public void PushMessage(string input)
    {
        Connection connection = null;
        Session session = null;
        try
        {
            ConnectionFactory factory = new ConnectionFactory(_settings.ServerUrl);
            factory.SetTargetHostName(_settings.HostName);
            factory.SetCertificateStoreType(EMSSSLStoreType.EMSSSL_STORE_TYPE_SYSTEM, new EMSSSLSystemStoreInfo());
            connection = factory.CreateConnection(_settings.Username, _settings.Password);
            session = connection.CreateSession(false, Session.AUTO_ACKNOWLEDGE);
            Queue queue = session.CreateQueue(_settings.QueueName);
            MessageProducer producer = session.CreateProducer(queue);
            connection.Start();

            TextMessage message = session.CreateTextMessage(input);
            producer.Send(message);
            connection.Stop();
        }
        catch (EMSException ex)
        {
            Console.WriteLine($"EMS Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General Error: {ex.Message}");
        }
        finally
        {
            if (session != null)
            {
                try { session.Close(); } catch { }
            }
            if (connection != null)
            {
                try { connection.Close(); } catch { }
            }
        }
    }
}

public class TibcoEMSSettings
{
    public string ServerUrl { get; set; }
    public string HostName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string QueueName { get; set; }
}

