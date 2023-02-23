using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Interface;
using ClassLibrary1; // But 'not' ClassLibrary2

namespace optional_assembly
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Test optional lib";
            Assembly assySpecial;
            Product product = new Product(); // In ClassLibrary1

            #region N O T    L O A D E D
            // Try get assy (not strong named or picky about version)
            assySpecial =
                AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(_ => _.GetName().Name.Equals("ClassLibrary2"));
            Debug.Assert(assySpecial == null, "Expecting assy not loaded");
            Debug.Assert(product.SpecialProduct == null, "Expecting null SpecialProduct");
            if (product.SpecialProduct == null)
            {
                Console.WriteLine("SpecialProduct is not loaded yet.");
            }
            #endregion N O T    L O A D E D

            #region L O A D
            var pluginPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Plugins",
                "ClassLibrary2.dll");
            if (File.Exists(pluginPath))
            {
                Assembly.LoadFrom(pluginPath);
            }
            #endregion L O A D

            #region L O A D E D
            assySpecial =
                AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(_ => _.GetName().Name.Equals("ClassLibrary2"));

            Debug.Assert(assySpecial != null, "Expecting assy loaded");

            if (assySpecial != null)
            {
                product.SpecialProduct =
                    (ISpecialProduct)
                    Activator.CreateInstance(
                        assySpecial
                        .GetTypes()
                        .First(_ => _.Name.Equals("SpecialProduct")));
            }

            Console.WriteLine($"SpecialProduct: {product.SpecialProduct?.GetSpecialProduct("123")}");

            Console.ReadKey();
            #endregion L O A D E D
        }
    }
}
