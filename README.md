One approach that has worked for me is to have an `Interface` class library consisting only of plugin interfaces that the application may attempt to use (but which, at the same time, might not have instances attached). It will be referenced by plugin servers and clients alike. 

***
**`Interface` class library**

    namespace Interface
    {
        /// <summary>
        /// To be a plugin for this app requires IPlugin at minimum .
        /// </summary>
        public interface IPlugin { }
        public interface ISpecialProduct : IPlugin
        {
            string GetSpecialProduct(string id);
        }
    }

***
**ClassLibrary1**

The `SpecialProduct` member is decoupled from `ClassLibrary2` because it's an interface not a class.

    using Interface; // But 'not' ClassLibrary2
    namespace ClassLibrary1
    {
        public class Product
        {
            public ISpecialProduct SpecialProduct { get; set; }
        }
    }

***
**ClassLibrary2**

    using Interface; // The only dependency
    namespace ClassLibrary2
    {
        public class SpecialProduct : ISpecialProduct
        {
            public string GetSpecialProduct(string id) => Guid.NewGuid().ToString();
        }
    }

 
***
**Test (proof of concept using console app)**

Available plugins are located in the `Plugins` subfolder of the application's run directory. `ClassLibrary2` is _not_ initially referenced or loaded.

    using Interface;
    using ClassLibrary1; // But 'not' ClassLibrary2
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
            if(product.SpecialProduct == null)
            {
                Console.WriteLine("SpecialProduct is not loaded yet.");
            }
            #endregion N O T    L O A D E D

            #region L O A D
            var pluginPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Plugins",
                "ClassLibrary2.dll");
            if(File.Exists(pluginPath)) 
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

[![console output][1]][1]


  [1]: https://i.stack.imgur.com/9LpNf.png