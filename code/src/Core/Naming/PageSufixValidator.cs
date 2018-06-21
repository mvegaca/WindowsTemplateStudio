namespace Microsoft.Templates.Core
{
    public class PageSufixValidator : Validator
    {
        private const string PageSufix = "Page";

        public override ValidationResult Validate(string suggestedName)
        {
            if (suggestedName.EndsWith(PageSufix))
            {
                return new ValidationResult()
                {
                    IsValid = false,
                    ErrorType = ValidationErrorType.EndsWithPageSufix
                };
            }
            else
            {
                return new ValidationResult()
                {
                    IsValid = true,
                    ErrorType = ValidationErrorType.None
                };
            }
        }
    }
}
