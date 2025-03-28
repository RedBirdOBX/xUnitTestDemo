using EmployeeManagement.Business;
using System.Net;
using System.Text;
using System.Text.Json;

namespace EmployeeManagement.Test.HttpMessageHandlers
{
    public class TestablePromotionEligibilityHandler : HttpMessageHandler  // abstract class
    {
        private readonly bool _isEligibleForPromotion;

        public TestablePromotionEligibilityHandler(bool isEligibleForPromotion)
        {
            _isEligibleForPromotion = isEligibleForPromotion;
        }

        protected override Task<HttpResponseMessage> SendAsync( HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1 - Create the object that will be part of the response
            // this is that the response body is deserialized to.
            var promotionEligibilty = new PromotionEligibility()
            {
                EligibleForPromotion = _isEligibleForPromotion
            };

            //snippet from PromotionService
            //// deserialize content
            //var content = await response.Content.ReadAsStringAsync();
            //var promotionEligibility = JsonSerializer.Deserialize<PromotionEligibility>(content,
            //    new JsonSerializerOptions
            //    {
            //        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            //    });

            // 2 - Create the response
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize
                (
                    promotionEligibilty,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                    Encoding.ASCII,
                    "application/json"
                )
            };

            return Task.FromResult(response);
        }
    }
}
