/* using System;
using System.Collections.Generic;
using Xunit;
namespace ConversationStorageTests.Extensions;

public static class AssertExtensions
{
    public static void AssertListContains<T>(T expectedItem, IEnumerable<T> actualList)
    {
        foreach (var actualItem in actualList)
        {
            if (AreEqualWithTolerance(expectedItem, actualItem))
            {
                return;
            }
        }

        Assert.True(false, $"El elemento esperado no se encuentra en la lista.");
    }

    private static bool AreEqualWithTolerance<T>(T expected, T actual)
    {
        foreach (var property in typeof(T).GetProperties())
        {
            if (property.PropertyType == typeof(DateTime))
            {
                var expectedDate = (DateTime)property.GetValue(expected);
                var actualDate = (DateTime)property.GetValue(actual);

                // Verificar si la propiedad es de tipo DateTime y aplicar tolerancia de 1 segundo
                if (Math.Abs((expectedDate - actualDate).TotalSeconds) > 1)
                {
                    return false;
                }
            }
            else if (!property.GetValue(expected).Equals(property.GetValue(actual)))
            {
                return false;
            }
        }
        return true;
    }
}

 */