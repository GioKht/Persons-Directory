using System.Globalization;

namespace Persons.Directory.Application.FluentValidationSupport
{
    public class CustomLanguageManager : FluentValidation.Resources.LanguageManager
    {
        //public CustomLanguageManager()
        //{
        //    AddTranslation("en-US", "NotNullValidator", "'{PropertyName}' is required.");
        //    AddTranslation("en-GB", "NotNullValidator", "'{PropertyName}' is required.");
        //}

        public override string GetString(string key, CultureInfo culture = null)
        {
            return base.GetString(key, culture);
        }
    }
}
