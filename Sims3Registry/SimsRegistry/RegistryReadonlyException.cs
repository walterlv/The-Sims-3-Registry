using System;

namespace m3i.SimsRegistry
{
    /// <summary>
    /// 因为注册表对只读属性的注册表进行写操作而引发的异常
    /// </summary>
    public class RegistryReadonlyException:Exception
    {
        public RegistryReadonlyException()
            : base() { }
        public RegistryReadonlyException(string message)
            : base(message) { }
    }
}
