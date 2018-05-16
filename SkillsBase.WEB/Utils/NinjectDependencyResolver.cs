using Ninject;
using SkillsBase.BLL.Interfaces;
using SkillsBase.BLL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SkillsBase.WEB.Utils
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
    }
}