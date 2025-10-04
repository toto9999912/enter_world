using System;
using UnityEngine;
using Stats;

namespace Companion
{
    /// <summary>
    /// 捕捉網品質
    /// </summary>
    public enum CaptureNetQuality
    {
        /// <summary>普通網 (基礎捕捉率)</summary>
        Normal,

        /// <summary>良好網 (+10% 捕捉率)</summary>
        Good,

        /// <summary>優秀網 (+25% 捕捉率)</summary>
        Excellent,

        /// <summary>完美網 (+50% 捕捉率)</summary>
        Perfect,

        /// <summary>大師網 (+100% 捕捉率)</summary>
        Master
    }

    /// <summary>
    /// 捕捉結果
    /// </summary>
    public class CaptureResult
    {
        /// <summary>是否成功</summary>
        public bool success;

        /// <summary>最終捕捉率</summary>
        public float finalCaptureRate;

        /// <summary>隨機值</summary>
        public float rollValue;

        /// <summary>失敗原因</summary>
        public string failureReason;

        /// <summary>捕捉的眷屬實例</summary>
        public CompanionInstance capturedCompanion;
    }

    /// <summary>
    /// 眷屬捕捉系統
    /// 處理捕捉率計算與捕捉判定
    /// </summary>
    public static class CompanionCaptureSystem
    {
        // ===== 可配置參數 (預設值) =====

        /// <summary>HP 百分比影響捕捉率 (HP 越低捕捉率越高)</summary>
        public static float hpFactorWeight = 0.5f; // HP 10% = +45% 捕捉率

        /// <summary>SP 百分比影響捕捉率 (SP 越低捕捉率越高)</summary>
        public static float spFactorWeight = 0.3f; // SP 10% = +27% 捕捉率

        /// <summary>等級差影響捕捉率 (玩家等級高於目標)</summary>
        public static float levelDiffWeight = 2f; // 每級差 = +2% 捕捉率

        /// <summary>最大等級差獎勵上限</summary>
        public static int maxLevelDiffBonus = 10; // 最多 +20% (10級 × 2%)

        /// <summary>網子品質加成</summary>
        public static float GetNetQualityBonus(CaptureNetQuality quality)
        {
            return quality switch
            {
                CaptureNetQuality.Normal => 0f,
                CaptureNetQuality.Good => 10f,
                CaptureNetQuality.Excellent => 25f,
                CaptureNetQuality.Perfect => 50f,
                CaptureNetQuality.Master => 100f,
                _ => 0f
            };
        }

        /// <summary>
        /// 計算最終捕捉率
        /// </summary>
        /// <param name="targetData">目標眷屬資料</param>
        /// <param name="targetLevel">目標等級</param>
        /// <param name="targetHPPercent">目標當前 HP 百分比 (0-1)</param>
        /// <param name="targetSPPercent">目標當前 SP 百分比 (0-1)</param>
        /// <param name="playerLevel">玩家等級</param>
        /// <param name="netQuality">捕捉網品質</param>
        /// <param name="bonusCaptureRate">額外加成 (技能/道具)</param>
        /// <returns>最終捕捉率 (0-100)</returns>
        public static float CalculateCaptureRate(
            CompanionData targetData,
            int targetLevel,
            float targetHPPercent,
            float targetSPPercent,
            int playerLevel,
            CaptureNetQuality netQuality = CaptureNetQuality.Normal,
            float bonusCaptureRate = 0f)
        {
            // 1. 基礎捕捉率
            float baseCaptureRate = targetData.baseCaptureRate;

            // 2. HP 因素 (HP 越低捕捉率越高)
            float hpBonus = (1f - targetHPPercent) * hpFactorWeight * 100f;

            // 3. SP 因素 (SP 越低捕捉率越高)
            float spBonus = (1f - targetSPPercent) * spFactorWeight * 100f;

            // 4. 等級差因素 (玩家等級 > 目標等級)
            int levelDiff = Mathf.Clamp(playerLevel - targetLevel, -maxLevelDiffBonus, maxLevelDiffBonus);
            float levelBonus = levelDiff * levelDiffWeight;

            // 5. 網子品質加成
            float netBonus = GetNetQualityBonus(netQuality);

            // 6. 總和
            float totalCaptureRate = baseCaptureRate + hpBonus + spBonus + levelBonus + netBonus + bonusCaptureRate;

            // 限制在 5-95% 之間 (永遠有失敗/成功的可能)
            return Mathf.Clamp(totalCaptureRate, 5f, 95f);
        }

        /// <summary>
        /// 嘗試捕捉眷屬
        /// </summary>
        public static CaptureResult AttemptCapture(
            CompanionData targetData,
            int targetLevel,
            HealthSystem targetHealth,
            CharacterStats targetStats,
            int playerLevel,
            CaptureNetQuality netQuality = CaptureNetQuality.Normal,
            float bonusCaptureRate = 0f)
        {
            CaptureResult result = new CaptureResult();

            // 檢查是否可被捕捉
            if (!targetData.canBeCaptured)
            {
                result.success = false;
                result.failureReason = "此眷屬無法被捕捉！";
                return result;
            }

            // 檢查目標是否存活
            if (targetHealth.IsDead)
            {
                result.success = false;
                result.failureReason = "目標已死亡，無法捕捉！";
                return result;
            }

            // 計算 SP 百分比 (如果有 SP 的話)
            float spPercent = 1.0f;
            float currentSP = targetStats.GetFinalValue(StatType.SP);
            float maxSP = targetStats.GetFinalValue(StatType.SP);
            if (maxSP > 0)
            {
                spPercent = currentSP / maxSP;
            }

            // 計算捕捉率
            float captureRate = CalculateCaptureRate(
                targetData,
                targetLevel,
                targetHealth.HPPercent,
                spPercent,
                playerLevel,
                netQuality,
                bonusCaptureRate
            );

            result.finalCaptureRate = captureRate;

            // 擲骰判定
            result.rollValue = UnityEngine.Random.Range(0f, 100f);
            result.success = result.rollValue < captureRate;

            if (result.success)
            {
                // 創建眷屬實例
                result.capturedCompanion = new CompanionInstance(targetData, targetLevel);
            }
            else
            {
                result.failureReason = $"捕捉失敗！({result.rollValue:F1} >= {captureRate:F1})";
            }

            return result;
        }

        /// <summary>
        /// 取得捕捉率顯示文字 (用於 UI 提示)
        /// </summary>
        public static string GetCaptureRateDescription(float captureRate)
        {
            if (captureRate >= 80f)
                return "極高";
            else if (captureRate >= 60f)
                return "很高";
            else if (captureRate >= 40f)
                return "中等";
            else if (captureRate >= 20f)
                return "較低";
            else
                return "極低";
        }
    }
}
