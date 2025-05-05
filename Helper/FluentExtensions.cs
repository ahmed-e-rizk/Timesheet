

using FluentValidation;

namespace Helper
{
    public static class FluentExtensions
    {

        public static IRuleBuilderOptions<T, TProperty> WithErrorCode<T, TProperty>(this IRuleBuilderOptions<T, TProperty> ruleBuilder, DTO.Eunms.MessageCodes messageCode)
        {
            return ruleBuilder.Configure(cfg =>
            {
                cfg.Current.ErrorCode = Convert.ToInt32(messageCode).ToString();
                cfg.Current.SetErrorMessage(messageCode.GetDescription());
            });
        }

    }
}
