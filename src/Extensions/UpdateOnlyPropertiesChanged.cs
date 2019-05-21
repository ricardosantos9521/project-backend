using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace backendProject.Extensions
{
    public static class Extensions
    {
        public static void UpdateOnlyChangedProperties(this DbContext context, object obj, List<String> propertieschanged)
        {
            context.Attach(obj);

            var propertieschangedLower = propertieschanged.Select(x => x.ToLower());

            var properties = obj.GetType()
                                .GetProperties()
                                .Select(x => x.Name)
                                .Where(x =>
                                    propertieschangedLower.Contains(x.ToLower())
                                );

            foreach (var property in properties)
            {
                context.Entry(obj).Property(property).IsModified = true;
            }
        }
    }
}