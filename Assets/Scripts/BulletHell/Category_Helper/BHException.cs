using System;
using System.Collections.Generic;

namespace BulletHell3D
{
    public enum BHException
    {
        ManagerNotFound,
        TooManyRenderInstances
    }

    public static class BHExceptionRaiser
    {
        private static readonly Dictionary<BHException, string> exceptionMessages = new Dictionary<BHException, string>()
        {
            { BHException.ManagerNotFound, "Cannot find BHManager!" },
            { BHException.TooManyRenderInstances, "Too many render instances! A single type of render object supports at most 1023 instances." }
        };

        public static void RaiseException(BHException exceptionType)
        {
            throw new Exception(exceptionMessages[exceptionType]);
        }
    }
}