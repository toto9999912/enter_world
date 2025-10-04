using System.Collections.Generic;
using Companion;
using Core;

namespace Services
{
    /// <summary>
    /// 眷屬服務介面
    /// 提供眷屬管理、部署、召回等功能
    /// </summary>
    public interface ICompanionService : IService
    {
        /// <summary>
        /// 新增眷屬
        /// </summary>
        bool AddCompanion(CompanionInstance companion);
        
        /// <summary>
        /// 移除眷屬
        /// </summary>
        bool RemoveCompanion(CompanionInstance companion);
        
        /// <summary>
        /// 部署眷屬
        /// </summary>
        bool DeployCompanion(CompanionInstance companion);
        
        /// <summary>
        /// 召回眷屬
        /// </summary>
        bool RecallCompanion(CompanionInstance companion);
        
        /// <summary>
        /// 召回所有眷屬
        /// </summary>
        void RecallAllCompanions();
        
        /// <summary>
        /// 取得所有擁有的眷屬
        /// </summary>
        IReadOnlyList<CompanionInstance> GetOwnedCompanions();
        
        /// <summary>
        /// 取得所有出戰的眷屬
        /// </summary>
        IReadOnlyList<CompanionInstance> GetActiveCompanions();
        
        /// <summary>
        /// 取得可用 SP
        /// </summary>
        float GetAvailableSP();
        
        /// <summary>
        /// 取得最大 SP
        /// </summary>
        float GetMaxSP();
        
        /// <summary>
        /// 清空所有眷屬 (用於載入存檔)
        /// </summary>
        void ClearAllCompanions();
    }
}
