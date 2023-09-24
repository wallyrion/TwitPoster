﻿using FluentValidation;
using FluentValidation.Results;

namespace TwitPoster.BLL.Common.Options;

public interface ITwitposterOptions<T> where T : class
{
    public static abstract string SectionName { get; }

    protected InlineValidator<T> Validator { get; }
    
    public ValidationResult Validate()
        => Validator?.Validate((this as T)!) ?? new();
}