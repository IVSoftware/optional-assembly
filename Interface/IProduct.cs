using System;
using System.Collections.Generic;
using System.Text;

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
