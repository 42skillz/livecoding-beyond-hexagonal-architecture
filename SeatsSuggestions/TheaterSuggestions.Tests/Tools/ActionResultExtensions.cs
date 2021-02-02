using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SeatsSuggestions.Tests.Tools
{
    public static class ActionResultExtensions
    {
        public static T ExtractValue<T>(this IActionResult actionResult)
        {
            var result = (OkObjectResult) actionResult;
            return (T) result.Value;
        }

        public static HttpStatusCode GetStatusCode(this IActionResult actionResult)
        {
            if (actionResult is OkObjectResult)
            {
                return HttpStatusCode.OK;
            }

            if (actionResult is ObjectResult result)
            {
                if (result.StatusCode.HasValue)
                {
                    return (HttpStatusCode) result.StatusCode.Value;
                }

                throw new Exception("Result has a null StatusCode");
            }

            throw new Exception("Result is neither an ObjectResult nor an OkObjectResult");
        }

        public static void CheckIsOk(this IActionResult actionResult)
        {
            var result = (ObjectResult) actionResult;
            var isOk = result.StatusCode.HasValue && result.StatusCode.Value == (int) HttpStatusCode.OK;

            if (!isOk)
            {
                throw new Exception($"The response does not have the expected 200 HttpStatusCode. HttpStatusCode was: '{result.StatusCode.Value}'.");
            }
        }

        public static void CheckIsCode(this IActionResult actionResult, HttpStatusCode httpStatusCode)
        {
            var result = (ObjectResult) actionResult;
            var isOk = result.StatusCode.HasValue && result.StatusCode.Value == (int) httpStatusCode;

            if (!isOk)
            {
                throw new Exception($"The response does not have the expected {(int) httpStatusCode} HttpStatusCode. HttpStatusCode was: '{result.StatusCode.Value}'.");
            }
        }

        public static void CheckIsErrorWithCode(this IActionResult actionResult, HttpStatusCode httpStatusCode)
        {
            var result = (ObjectResult) actionResult;

            var isErrorWithCode = result.StatusCode.HasValue && result.StatusCode.Value == (int) httpStatusCode;

            if (!isErrorWithCode)
            {
                throw new Exception($"The response does not have the expected HttpStatusCode. Expected: {httpStatusCode} but was: '{result.StatusCode.Value}'.");
            }
        }

        public static void CheckIsErrorWithCodeAndContent(this IActionResult actionResult, HttpStatusCode expectedHttpStatusCode, string expectedContent)
        {
            var result = (ObjectResult) actionResult;

            var isErrorWithCode = result.StatusCode.HasValue && result.StatusCode.Value == (int) expectedHttpStatusCode;

            if (!isErrorWithCode)
            {
                throw new Exception($"The response does not have the expected HttpStatusCode. Expected: {expectedHttpStatusCode} but was: '{result.StatusCode.Value}'.");
            }

            if (result.Value.ToString() != expectedContent)
            {
                throw new Exception($"The response does not have the expected content. Expected content: [{expectedContent}] but was: [{result.Value}].");
            }
        }
    }
}