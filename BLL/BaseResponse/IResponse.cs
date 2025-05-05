using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTO.Eunms;
using DTO.Response;
using FluentValidation.Results;

namespace BLL.BaseResponse
{
    public interface IResponse<T>
    {
        public bool IsSuccess { get; set; }
        public List<TErrorField> Errors { get; set; }
        public T Data { get; set; }

        public IResponse<T> CreateResponse(T data);
        public IResponse<T> CreateResponse(Exception ex);
        public IResponse<T> AppendError(TErrorField error);
        public IResponse<T> AppendErrors(List<TErrorField> errors);
        public IResponse<T> CreateResponse(MessageCodes messageCode);
        public IResponse<T> CreateResponse(List<ValidationFailure> inputValidations = null);
        public IResponse<T> CreateResponse();
       
      
    }
}
