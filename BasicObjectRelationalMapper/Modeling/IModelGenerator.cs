using System.Collections.Generic;

namespace BasicObjectRelationalMapper.Modeling
{
    public interface IModelGenerator
    {
        PocoDataClass GetPocoDataClass(StoredProcedureInfo storedProcedureInfo);
        List<PocoDataClass> GetPocoDataClasses(IEnumerable<StoredProcedureInfo> storedProcedureList);
    }
}