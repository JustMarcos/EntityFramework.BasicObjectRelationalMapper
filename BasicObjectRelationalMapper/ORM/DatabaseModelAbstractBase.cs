using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicObjectRelationalMapper.ORM
{
    public abstract class DatabaseModelAbstractBase
    {
        /* 
            All models that are being mapped from the database must inherit this model.
            This enfoces the convention so that not just any class can be used. See the MapToModel() methods.
            Any attempt to use other classes that don't inherit from this class will result in a 
            compile time error. 
         */
    }
}
