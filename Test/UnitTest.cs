using System.Globalization;

namespace Test
{
    public class Tests
    {
        [Test]
        public void TestCalculatePortfolioValueForDateAndInvestorIdShouldPass()
        {
            //Setup
            var existingInvestorId = "Investor17";
            var existingDateWithInvestments = "2017-11-15";

            //Act
            var result = QplixChallengeProgram.CalculatePortfolioValueForDateAndInvestorId(DateTime.ParseExact(existingDateWithInvestments, "yyyy-MM-dd", CultureInfo.InvariantCulture), existingInvestorId);

            //Assert
            Assert.That(result, Is.EqualTo(-2559.5904124580843));
        }

        [Test]
        public void TestCalculatePortfolioValueForDateAndInvestorIdShouldThrowsException()
        {
            //Setup
            var nonExistingInvestorId = "";
            var existingDateWithInvestments = "2017-11-15";

            //Act and Assert
            Assert.Throws<ArgumentException>(() => QplixChallengeProgram.CalculatePortfolioValueForDateAndInvestorId(DateTime.ParseExact(existingDateWithInvestments, "yyyy-MM-dd", CultureInfo.InvariantCulture), nonExistingInvestorId));
        }
    }
}