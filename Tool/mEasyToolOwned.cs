using mToolkitPlatformComponentLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mToolkitFrameworkExtensions.Tool
{
    public abstract class mEasyToolOwned : mToolOwned
    {
        protected readonly mTool Owner;

        public mEasyToolOwned(mTool owner)
        {
            Owner = owner;
        }

        public mTool GetOwner()
        {
            return Owner;
        }
    }
}
