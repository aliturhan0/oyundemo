// Copyright 2021, Infima Games. All Rights Reserved.

using System.Globalization;

namespace InfimaGames.LowPolyShooterPack.Interface
{
    /// <summary>
    /// Total Ammunition Text.
    /// </summary>
    public class TextAmmunitionTotal : ElementText
    {
        #region METHODS
        
        /// <summary>
        /// Tick.
        /// </summary>
        protected override void Tick()
        {
            //Yedek (reserve) mermiyi göster. -1 = sınırsız -> "∞".
            int reserve = equippedWeapon.GetAmmunitionReserve();

            //Update Text.
            textMesh.text = reserve < 0
                ? "∞"
                : reserve.ToString(CultureInfo.InvariantCulture);
        }
        
        #endregion
    }
}