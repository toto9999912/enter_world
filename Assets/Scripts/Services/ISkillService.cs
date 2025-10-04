using System.Collections.Generic;
using Skill;
using Element;
using Core;

namespace Services
{
    /// <summary>
    /// 技能服務介面
    /// 提供技能學習、升級、查詢等功能
    /// </summary>
    public interface ISkillService : IService
    {
        /// <summary>
        /// 學習技能
        /// </summary>
        bool LearnSkill(SkillData skillData);
        
        /// <summary>
        /// 升級技能
        /// </summary>
        bool UpgradeSkill(string skillId);
        
        /// <summary>
        /// 取得已學習的技能
        /// </summary>
        SkillInstance GetSkill(string skillId);
        
        /// <summary>
        /// 檢查是否學習了技能
        /// </summary>
        bool HasLearnedSkill(string skillId);
        
        /// <summary>
        /// 增加技能點數
        /// </summary>
        void AddSkillPoints(int points);
        
        /// <summary>
        /// 取得可用技能點數
        /// </summary>
        int GetAvailableSkillPoints();
        
        /// <summary>
        /// 取得所有已學習的技能
        /// </summary>
        IReadOnlyDictionary<string, SkillInstance> GetLearnedSkills();
        
        /// <summary>
        /// 取得玩家主屬性
        /// </summary>
        ElementType GetPlayerMainElement();
        
        /// <summary>
        /// 清空所有技能 (用於載入存檔)
        /// </summary>
        void ClearAllSkills();
        
        /// <summary>
        /// 設定技能點數 (用於載入存檔)
        /// </summary>
        void SetSkillPoints(int points);
        
        /// <summary>
        /// 設定主屬性 (用於載入存檔)
        /// </summary>
        void SetMainElement(ElementType element);
    }
}
