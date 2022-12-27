namespace KOM.Scribere.Services.Mapping;

using AutoMapper;

public interface IHaveCustomMappings
{
    void CreateMappings(IProfileExpression configuration);
}