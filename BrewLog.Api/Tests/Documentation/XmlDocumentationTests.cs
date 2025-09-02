using System.Reflection;
using BrewLog.Api.Controllers;
using BrewLog.Api.DTOs;
using BrewLog.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace BrewLog.Api.Tests.Documentation;

/// <summary>
/// Tests to validate that all public APIs have proper XML documentation
/// </summary>
public class XmlDocumentationTests
{
    [Fact]
    public void All_Public_DTOs_Should_Have_XML_Documentation()
    {
        // Arrange
        var assembly = Assembly.GetAssembly(typeof(CoffeeBeanResponseDto));
        var dtoTypes = assembly!.GetTypes()
            .Where(t => t.Namespace == "BrewLog.Api.DTOs" && 
                       t.IsPublic && 
                       t.IsClass && 
                       !t.IsAbstract &&
                       t.Name.EndsWith("Dto"))
            .ToList();

        // Act & Assert
        var undocumentedTypes = new List<string>();
        
        foreach (var type in dtoTypes)
        {
            var xmlDocFile = GetXmlDocumentationPath();
            var hasDocumentation = HasXmlDocumentation(type, xmlDocFile);
            
            if (!hasDocumentation)
            {
                undocumentedTypes.Add(type.Name);
            }
        }

        undocumentedTypes.Should().BeEmpty(
            $"The following DTO types are missing XML documentation: {string.Join(", ", undocumentedTypes)}");
    }

    [Fact]
    public void All_DTO_Properties_Should_Have_XML_Documentation()
    {
        // Arrange
        var assembly = Assembly.GetAssembly(typeof(CoffeeBeanResponseDto));
        var dtoTypes = assembly!.GetTypes()
            .Where(t => t.Namespace == "BrewLog.Api.DTOs" && 
                       t.IsPublic && 
                       t.IsClass && 
                       !t.IsAbstract &&
                       t.Name.EndsWith("Dto"))
            .ToList();

        // Act & Assert
        var undocumentedProperties = new List<string>();
        var xmlDocFile = GetXmlDocumentationPath();
        
        foreach (var type in dtoTypes)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            foreach (var property in properties)
            {
                var hasDocumentation = HasXmlDocumentation(property, xmlDocFile);
                
                if (!hasDocumentation)
                {
                    undocumentedProperties.Add($"{type.Name}.{property.Name}");
                }
            }
        }

        undocumentedProperties.Should().BeEmpty(
            $"The following DTO properties are missing XML documentation: {string.Join(", ", undocumentedProperties)}");
    }

    [Fact]
    public void All_Controller_Methods_Should_Have_XML_Documentation()
    {
        // Arrange
        var assembly = Assembly.GetAssembly(typeof(CoffeeBeansController));
        var controllerTypes = assembly!.GetTypes()
            .Where(t => t.Namespace == "BrewLog.Api.Controllers" && 
                       t.IsPublic && 
                       t.IsClass && 
                       !t.IsAbstract &&
                       t.Name.EndsWith("Controller"))
            .ToList();

        // Act & Assert
        var undocumentedMethods = new List<string>();
        var xmlDocFile = GetXmlDocumentationPath();
        
        foreach (var controllerType in controllerTypes)
        {
            var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(m => m.IsPublic && 
                           !m.IsSpecialName && 
                           m.DeclaringType == controllerType &&
                           (m.GetCustomAttribute<HttpGetAttribute>() != null ||
                            m.GetCustomAttribute<HttpPostAttribute>() != null ||
                            m.GetCustomAttribute<HttpPutAttribute>() != null ||
                            m.GetCustomAttribute<HttpDeleteAttribute>() != null ||
                            m.GetCustomAttribute<HttpPatchAttribute>() != null))
                .ToList();
            
            foreach (var method in methods)
            {
                var hasDocumentation = HasXmlDocumentation(method, xmlDocFile);
                
                if (!hasDocumentation)
                {
                    undocumentedMethods.Add($"{controllerType.Name}.{method.Name}");
                }
            }
        }

        undocumentedMethods.Should().BeEmpty(
            $"The following controller methods are missing XML documentation: {string.Join(", ", undocumentedMethods)}");
    }

    [Fact]
    public void All_Model_Classes_Should_Have_XML_Documentation()
    {
        // Arrange
        var assembly = Assembly.GetAssembly(typeof(CoffeeBean));
        var modelTypes = assembly!.GetTypes()
            .Where(t => t.Namespace == "BrewLog.Api.Models" && 
                       t.IsPublic && 
                       t.IsClass && 
                       !t.IsAbstract &&
                       !t.IsEnum)
            .ToList();

        // Act & Assert
        var undocumentedTypes = new List<string>();
        var xmlDocFile = GetXmlDocumentationPath();
        
        foreach (var type in modelTypes)
        {
            var hasDocumentation = HasXmlDocumentation(type, xmlDocFile);
            
            if (!hasDocumentation)
            {
                undocumentedTypes.Add(type.Name);
            }
        }

        undocumentedTypes.Should().BeEmpty(
            $"The following model types are missing XML documentation: {string.Join(", ", undocumentedTypes)}");
    }

    [Fact]
    public void All_Enum_Types_Should_Have_XML_Documentation()
    {
        // Arrange
        var assembly = Assembly.GetAssembly(typeof(RoastLevel));
        var enumTypes = assembly!.GetTypes()
            .Where(t => t.Namespace == "BrewLog.Api.Models" && 
                       t.IsPublic && 
                       t.IsEnum)
            .ToList();

        // Act & Assert
        var undocumentedEnums = new List<string>();
        var xmlDocFile = GetXmlDocumentationPath();
        
        foreach (var enumType in enumTypes)
        {
            var hasDocumentation = HasXmlDocumentation(enumType, xmlDocFile);
            
            if (!hasDocumentation)
            {
                undocumentedEnums.Add(enumType.Name);
            }
        }

        undocumentedEnums.Should().BeEmpty(
            $"The following enum types are missing XML documentation: {string.Join(", ", undocumentedEnums)}");
    }

    [Fact]
    public void All_Enum_Values_Should_Have_XML_Documentation()
    {
        // Arrange
        var assembly = Assembly.GetAssembly(typeof(RoastLevel));
        var enumTypes = assembly!.GetTypes()
            .Where(t => t.Namespace == "BrewLog.Api.Models" && 
                       t.IsPublic && 
                       t.IsEnum)
            .ToList();

        // Act & Assert
        var undocumentedEnumValues = new List<string>();
        var xmlDocFile = GetXmlDocumentationPath();
        
        foreach (var enumType in enumTypes)
        {
            var enumValues = Enum.GetNames(enumType);
            
            foreach (var enumValue in enumValues)
            {
                var fieldInfo = enumType.GetField(enumValue);
                var hasDocumentation = HasXmlDocumentation(fieldInfo!, xmlDocFile);
                
                if (!hasDocumentation)
                {
                    undocumentedEnumValues.Add($"{enumType.Name}.{enumValue}");
                }
            }
        }

        undocumentedEnumValues.Should().BeEmpty(
            $"The following enum values are missing XML documentation: {string.Join(", ", undocumentedEnumValues)}");
    }

    private static string GetXmlDocumentationPath()
    {
        var assembly = Assembly.GetAssembly(typeof(CoffeeBeansController));
        var assemblyName = assembly!.GetName().Name;
        var xmlFileName = $"{assemblyName}.xml";
        
        // Look for the XML file in the output directory
        var baseDirectory = AppContext.BaseDirectory;
        var xmlPath = Path.Combine(baseDirectory, xmlFileName);
        
        return xmlPath;
    }

    private static bool HasXmlDocumentation(Type type, string xmlDocPath)
    {
        if (!File.Exists(xmlDocPath))
        {
            return false;
        }

        try
        {
            var xmlContent = File.ReadAllText(xmlDocPath);
            var memberName = $"T:{type.FullName}";
            return xmlContent.Contains($"<member name=\"{memberName}\">");
        }
        catch
        {
            return false;
        }
    }

    private static bool HasXmlDocumentation(PropertyInfo property, string xmlDocPath)
    {
        if (!File.Exists(xmlDocPath))
        {
            return false;
        }

        try
        {
            var xmlContent = File.ReadAllText(xmlDocPath);
            var memberName = $"P:{property.DeclaringType!.FullName}.{property.Name}";
            return xmlContent.Contains($"<member name=\"{memberName}\">");
        }
        catch
        {
            return false;
        }
    }

    private static bool HasXmlDocumentation(MethodInfo method, string xmlDocPath)
    {
        if (!File.Exists(xmlDocPath))
        {
            return false;
        }

        try
        {
            var xmlContent = File.ReadAllText(xmlDocPath);
            var parameters = method.GetParameters();
            var parameterTypes = string.Join(",", parameters.Select(p => p.ParameterType.FullName));
            var memberName = parameters.Length > 0 
                ? $"M:{method.DeclaringType!.FullName}.{method.Name}({parameterTypes})"
                : $"M:{method.DeclaringType!.FullName}.{method.Name}";
            
            return xmlContent.Contains($"<member name=\"{memberName}\">");
        }
        catch
        {
            return false;
        }
    }

    private static bool HasXmlDocumentation(FieldInfo field, string xmlDocPath)
    {
        if (!File.Exists(xmlDocPath))
        {
            return false;
        }

        try
        {
            var xmlContent = File.ReadAllText(xmlDocPath);
            var memberName = $"F:{field.DeclaringType!.FullName}.{field.Name}";
            return xmlContent.Contains($"<member name=\"{memberName}\">");
        }
        catch
        {
            return false;
        }
    }
}