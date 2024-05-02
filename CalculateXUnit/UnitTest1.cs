using IO.Swagger.Controllers;
using IO.Swagger.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using static IO.Swagger.Models.ApiRequest;

namespace CalculateXUnit
{
    public class UnitTest1
    {

        [Theory]
        [ClassData(typeof(CalcDataGenerator))]
        public void CalculatorTest(ApiRequest request, decimal? result)
        {
            var calculator = new CalculateApiController();
            var actResult = calculator.CalculateArithmeticOperation(request) as ObjectResult;
            ModelApiResponse response = actResult.Value as ModelApiResponse;
            Assert.Equal(result, response?.Result);
        }

        [Theory]
        [InlineData("can't divide by zero")]
        public void DivideByZero(string result)
        {
            var calculator = new CalculateApiController();
            ApiRequest request = new ApiRequest()
            {
                FirstValue = 0,
                SecondValue = 0,
                ArithmeticOperation = ArithmeticOperationEnum.Slash
            };

            Assert.Throws<DivideByZeroException>(()=> calculator.CalculateArithmeticOperation(request));
        }

        [Theory]
        [InlineData("invalid arithmetic operation")]
        public void ArithmeticOperationError(string result)
        {
            var calculator = new CalculateApiController();
            ApiRequest request = new ApiRequest()
            {
                FirstValue = 4,
                SecondValue = 5,
                ArithmeticOperation = null
            };

            var actErrorResult = calculator.CalculateArithmeticOperation(request) as ObjectResult;
            if (actErrorResult?.StatusCode == null)
            {
                ModelApiResponse response = actErrorResult.Value as ModelApiResponse;
                Assert.Equal(result, response?.ErrorMsg);
            }
        }

        [Theory]
        [ClassData(typeof(CalcNullDataGenerator))]
        public void NullParametersTest(ApiRequest request, string result)
        {
            var calculator = new CalculateApiController();
            var actResult = calculator.CalculateArithmeticOperation(request) as ObjectResult;
            ModelApiResponse response = actResult.Value as ModelApiResponse;
            Assert.Equal(result, response?.ErrorMsg);
        }

        public class CalcDataGenerator : TheoryData<ApiRequest, decimal?>
        {
            public CalcDataGenerator()
            {
                //add combinations to test CalculatorParam1Param2Post
                Add(new ApiRequest() { FirstValue = null, SecondValue = 3, ArithmeticOperation = ArithmeticOperationEnum.Plus }, 8);
                Add(new ApiRequest() { FirstValue = 5, SecondValue = 3, ArithmeticOperation = ArithmeticOperationEnum.Plus }, 8);
                Add(new ApiRequest() { FirstValue = -1, SecondValue = 3, ArithmeticOperation = ArithmeticOperationEnum.Plus }, 2);
                Add(new ApiRequest() { FirstValue = 0, SecondValue = 0, ArithmeticOperation = ArithmeticOperationEnum.Plus }, 0);
                Add(new ApiRequest() { FirstValue = -1, SecondValue = 3, ArithmeticOperation = ArithmeticOperationEnum.Minus }, -4);
                Add(new ApiRequest() { FirstValue = 0, SecondValue = 0, ArithmeticOperation = ArithmeticOperationEnum.Minus }, 0);
                Add(new ApiRequest() { FirstValue = 15, SecondValue = 3, ArithmeticOperation = ArithmeticOperationEnum.Slash }, 5);
                Add(new ApiRequest() { FirstValue = 10, SecondValue = 4, ArithmeticOperation = ArithmeticOperationEnum.Slash }, 2.50m);
                Add(new ApiRequest() { FirstValue = 1, SecondValue = 3, ArithmeticOperation = ArithmeticOperationEnum.Star }, 3);
                Add(new ApiRequest() { FirstValue = 0, SecondValue = 0, ArithmeticOperation = ArithmeticOperationEnum.Star }, 0);
            }
        }

        public class CalcNullDataGenerator : TheoryData<ApiRequest, string?>
        {
            public CalcNullDataGenerator()
            {
                //add combinations to test CalculatorParam1Param2Post
                Add(new ApiRequest() { FirstValue = null, SecondValue = 3, ArithmeticOperation = ArithmeticOperationEnum.Plus }, "The first and the second parameters must be with value");
                Add(new ApiRequest() { FirstValue = 5, SecondValue = null, ArithmeticOperation = ArithmeticOperationEnum.Plus }, "The first and the second parameters must be with value");
            }
        }
    }
}