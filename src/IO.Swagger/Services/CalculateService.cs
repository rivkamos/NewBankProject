using IO.Swagger.Models;

namespace IO.Swagger.Services
{
    /// <summary>
    /// Calculate service for calculate the given parameters
    /// </summary>
    public static class CalculateService
    {
        /// <summary>
        /// Calculate servce for calculate the given parameters
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static ModelApiResponse Calculate(ApiRequest request) 
        {
            ModelApiResponse response = new ModelApiResponse() { StatusCode = 200 };

            if(request.FirstValue == null || request.SecondValue == null) {
                response.ErrorMsg = "The first and the second parameters must be with value";
                response.StatusCode = 400;
                return response;
            }

            switch (request.ArithmeticOperation)
            {
                case ApiRequest.ArithmeticOperationEnum.Plus:
                    response.Result = request.FirstValue + request.SecondValue;
                    break;
                case ApiRequest.ArithmeticOperationEnum.Minus:
                    response.Result = request.FirstValue - request.SecondValue;
                    break;
                case ApiRequest.ArithmeticOperationEnum.Star:
                    response.Result = request.FirstValue * request.SecondValue;
                    break;
                case ApiRequest.ArithmeticOperationEnum.Slash:
                    response.Result = (decimal)request.FirstValue / (decimal)request.SecondValue;
                    break;
                default:
                    response.StatusCode = 400;
                    response.ErrorMsg = "invalid arithmetic operation";
                    break;
            }
            return response;
        }
    }
}
