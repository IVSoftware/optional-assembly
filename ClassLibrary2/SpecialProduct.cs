
    using System;

    using Interface;
    namespace ClassLibrary2
    {
        public class SpecialProduct : ISpecialProduct
        {
            public string GetSpecialProduct(string id) => Guid.NewGuid().ToString();
        }
    }
