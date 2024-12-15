using PartnerAPI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerAPI.Tests
{

    [TestClass]
    public class DiscountCalculatorTests
    {
        [TestMethod]
        public void CalculateTotalDiscount_AmountLessThan200_ShouldReturnZeroDiscount()
        {
            long result = PartnerController.CalculateTotalDiscount(150); 
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void CalculateTotalDiscount_AmountBetween200And500_ShouldReturn5PercentDiscount()
        {
            long result = PartnerController.CalculateTotalDiscount(300);
            Assert.AreEqual(15, result);
        }

        [TestMethod]
        public void CalculateTotalDiscount_AmountBetween501And800_ShouldReturn7PercentDiscount()
        {
            long result = PartnerController.CalculateTotalDiscount(700);
            Assert.AreEqual(49, result);
        }

        [TestMethod]
        public void CalculateTotalDiscount_AmountBetween801And1200_ShouldReturn10PercentDiscount()
        {
            long result = PartnerController.CalculateTotalDiscount(1000);
            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void CalculateTotalDiscount_AmountGreaterThan1200_ShouldReturn15PercentDiscount()
        {
            long result = PartnerController.CalculateTotalDiscount(1500);
            Assert.AreEqual(225, result);
        }

        [TestMethod]
        public void CalculateTotalDiscount_AmountIsPrimeAndGreaterThan500_ShouldAdd8PercentDiscount()
        {
            long result = PartnerController.CalculateTotalDiscount(701);
            Assert.AreEqual(105, result);
        }

        [TestMethod]
        public void CalculateTotalDiscount_AmountGreaterThan900AndEndsWith5_ShouldAdd10PercentDiscount()
        {
            long result = PartnerController.CalculateTotalDiscount(905);
            Assert.AreEqual(181, result);
        }

        [TestMethod]
        public void CalculateTotalDiscount_TotalDiscountExceedsMax_ShouldReturnMaxDiscount()
        {
            long result = PartnerController.CalculateTotalDiscount(2005);
            Assert.AreEqual(401, result); 
        }
    }

}
