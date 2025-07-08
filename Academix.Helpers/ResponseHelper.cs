using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Academix.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Academix.Helpers
{
    public class ResponseHelper
    {
        private readonly ILocalizationService? _localizationService;
        
        public bool status { get; set; } = false;
        public string Massage { get; set; } = string.Empty;
        public int? StatusCode { get; set; }
        public object? Data { get; set; }
        public Dictionary<string, List<string>>? Validation { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now.ToLocalTime();
        public string Culture { get; set; } = "en";

        public ResponseHelper(ILocalizationService? localizationService = null)
        {
            _localizationService = localizationService;
            status = false;
            Massage = "no content";
            Culture = _localizationService?.GetCurrentCulture() ?? "en";
        }

        public ResponseHelper WithMassage(string message)
        {
            Massage = message?.Trim() ?? string.Empty;
            return this;
        }

        public ResponseHelper WithStatus(bool s)
        {
            status = s;
            return this;
        }

        public ResponseHelper WithData(object viewModel)
        {
            Data = viewModel;
            return this;
        }

        public ResponseHelper WithStatusCode(int code)
        {
            StatusCode = code;
            return this;
        }

        public ResponseHelper WithValidation(ModelStateDictionary keyValuePairs)
        {
            Validation = new Dictionary<string, List<string>>();
            keyValuePairs.ToList().ForEach(keyValue =>
            {
                Validation[keyValue.Key] = keyValue.Value.Errors.Select(e => e.ErrorMessage).ToList();
            });
            status = false;
            StatusCode = 400;
            Massage = Massage ?? GetLocalizedMessage("Validation_Failed");
            return this;
        }

        public ResponseHelper WithValidation(Dictionary<string, List<string>> validationErrors)
        {
            Validation = validationErrors;
            status = false;
            StatusCode = 400;
            Massage = Massage ?? GetLocalizedMessage("Validation_Failed");
            return this;
        }

        public ResponseHelper WithIdentityErrors(IEnumerable<IdentityError> errors)
        {
            Validation = new Dictionary<string, List<string>>();
            foreach (var error in errors)
            {
                if (!Validation.ContainsKey(error.Code))
                    Validation[error.Code] = new List<string>();

                Validation[error.Code].Add(error.Description);
            }
            status = false;
            StatusCode = 400;
            Massage = Massage ?? GetLocalizedMessage("Validation_Failed");
            return this;
        }

        public ResponseHelper Success(object data = null, string culture = null)
        {
            status = true;
            StatusCode = 200;
            Culture = culture ?? _localizationService?.GetCurrentCulture() ?? "en";
            Massage = GetLocalizedMessage("Operation_Success");
            Data = data;
            Validation = null;
            return this;
        }

        public ResponseHelper Created(object data = null, string culture = null)
        {
            status = true;
            StatusCode = 201;
            Culture = culture ?? _localizationService?.GetCurrentCulture() ?? "en";
            Massage = GetLocalizedMessage("Resource_Created");
            Data = data;
            Validation = null;
            return this;
        }

        public ResponseHelper Updated(object data = null, string culture = null)
        {
            status = true;
            StatusCode = 200;
            Culture = culture ?? _localizationService?.GetCurrentCulture() ?? "en";
            Massage = GetLocalizedMessage("Resource_Updated");
            Data = data;
            Validation = null;
            return this;
        }

        public ResponseHelper NotFound(string message = null, string culture = null)
        {
            status = false;
            StatusCode = 404;
            Culture = culture ?? _localizationService?.GetCurrentCulture() ?? "en";
            Massage = message ?? GetLocalizedMessage("Resource_NotFound");
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper BadRequest(string message = null, string culture = null)
        {
            status = false;
            StatusCode = 400;
            Culture = culture ?? _localizationService?.GetCurrentCulture() ?? "en";
            Massage = message ?? GetLocalizedMessage("Invalid_Request");
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper Unauthorized(string message = null, string culture = null)
        {
            status = false;
            StatusCode = 401;
            Culture = culture ?? _localizationService?.GetCurrentCulture() ?? "en";
            Massage = message ?? GetLocalizedMessage("Unauthorized_Access");
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper Forbidden(string message = null, string culture = null)
        {
            status = false;
            StatusCode = 403;
            Culture = culture ?? _localizationService?.GetCurrentCulture() ?? "en";
            Massage = message ?? GetLocalizedMessage("Access_Forbidden");
            Data = null;
            Validation = null;
            return this;
        }

        public ResponseHelper ServerError(string message = null, string culture = null)
        {
            status = false;
            StatusCode = 500;
            Culture = culture ?? _localizationService?.GetCurrentCulture() ?? "en";
            Massage = message ?? GetLocalizedMessage("Server_Error");
            Data = null;
            Validation = null;
            return this;
        }

        private string GetLocalizedMessage(string key)
        {
            if (_localizationService == null)
            {
                return key switch
                {
                    "Operation_Success" => Culture == "ar" ? "تمت العملية بنجاح" : "Operation completed successfully",
                    "Resource_Created" => Culture == "ar" ? "تم إنشاء المورد بنجاح" : "Resource created successfully",
                    "Resource_Updated" => Culture == "ar" ? "تم تحديث المورد بنجاح" : "Resource updated successfully",
                    "Resource_NotFound" => Culture == "ar" ? "المورد غير موجود" : "Resource not found",
                    "Invalid_Request" => Culture == "ar" ? "طلب غير صحيح" : "Invalid request",
                    "Unauthorized_Access" => Culture == "ar" ? "وصول غير مصرح به" : "Unauthorized access",
                    "Access_Forbidden" => Culture == "ar" ? "الوصول محظور" : "Access forbidden",
                    "Server_Error" => Culture == "ar" ? "خطأ داخلي في الخادم" : "Internal server error",
                    "Validation_Failed" => Culture == "ar" ? "فشل التحقق من الصحة" : "Validation failed",
                    _ => key
                };
            }

            return _localizationService.GetLocalizedString(key);
        }

        public static IDictionary<string, string> ModelStateToValidation(ModelStateDictionary modelState)
        {
            return modelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.FirstOrDefault()?.ErrorMessage ?? "Invalid value"
            );
        }

        public static implicit operator ActionResult(ResponseHelper response)
        {
            return new ObjectResult(response)
            {
                StatusCode = response.StatusCode ?? 200
            };
        }
    }
}
