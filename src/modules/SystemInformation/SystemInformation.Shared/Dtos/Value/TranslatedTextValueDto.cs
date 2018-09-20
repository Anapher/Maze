namespace SystemInformation.Shared.Dtos.Value
{
    public class TranslatedTextValueDto : ValueDto
    {
        public TranslatedTextValueDto(string translationKey)
        {
            TranslationKey = translationKey;
        }

        public TranslatedTextValueDto()
        {
        }

        public string TranslationKey { get; set; }
        public override ValueDtoType Type { get; } = ValueDtoType.TranslatedText;
    }
}