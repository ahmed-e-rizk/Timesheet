using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Eunms;
using DTO.Response;
using FluentValidation.Results;
using Helper;

namespace BLL.BaseResponse
{
    public class Response<T> :IResponse<T>
    {


        public bool IsSuccess { get; set; }
        public List<TErrorField> Errors { get; set; } = new List<TErrorField>();
        public T? Data { get; set; }
        public IResponse<T> CreateResponse(List<ValidationFailure> inputValidations = null)
        {
            foreach (var item in inputValidations)
            {
                item.PropertyName = item.PropertyName == "File.File" ? "File" : item.PropertyName;
                this.Errors.Add(new TErrorField
                {
                    FieldName = item.PropertyName,
                    Code = item.ErrorCode,
                    Message = item.ErrorMessage
                });
            }
            return this;
        }

        public IResponse<T> CreateResponse()
        {

            return this;
        }

        public IResponse<T> CreateResponse(T data)
        {

            var result = new Response<T>
            {
                IsSuccess = true,
                Data = data,
                Errors = new List<TErrorField>()
            };
            return result;
        }

        public IResponse<T> CreateResponse(Exception ex)
        {
            var result = new Response<T>()
            {
                IsSuccess = false,
                Errors = new List<TErrorField> { new() { Message = ex.Message } }
            };

            return result;
        }

        public IResponse<T> AppendError(TErrorField error)
        {
            var result = new Response<T>()
            {
                IsSuccess = false
            };
            result.Errors.Add(error);
            return result;
        }

        public IResponse<T> AppendErrors(List<TErrorField> errors)
        {
            var result = new Response<T>()
            {
                IsSuccess = false
            };
            result.Errors.AddRange(errors);
            return result;
        }


        public IResponse<T> CreateResponse(MessageCodes messageCode)
        {

            var result = new Response<T>()
            {
                IsSuccess = false,
                Errors = new List<TErrorField> { new() { Message = EnumExtensions.GetDescription(messageCode), Code = messageCode.ToString() } }
            };

            return result;

        }

    }
}
