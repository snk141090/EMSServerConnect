using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TIBCO.EMS;

namespace EMSServerConnect;

public class JMSPublisher(TibcoEMSService tibcoEMSService, ILogger<JMSPublisher> logger)  
{
    [Function("JMSPublisher")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req)
    {
        try
        {
            logger.LogInformation("JMS publisher HTTP trigger function processed a request.");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            tibcoEMSService.PushMessage(requestBody);
            return new OkObjectResult("Message Pushed Successfully");
        }
        catch (Exception ex) 
        {
            logger.LogError(ex.Message);
            return new BadRequestObjectResult(ex.Message);  
        }
    }
}
