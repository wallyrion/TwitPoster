namespace TwitPoster.BLL.Exceptions;

[Serializable]
public class ConfigurationValidationException(string configType, Exception ex) : Exception($"Configuration type '{configType}' validation failed", ex);