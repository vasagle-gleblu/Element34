using Element34.Utilities;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static Element34.WebDriverExtensions;

namespace SeleniumTest
{
    #region Control Types
    public class CheckBoxControlType : ControlType
    {
        public override void Choose(ReadOnlyCollection<IWebElement> cells, int iColumn, string value = "true")
        {
            Check(cells[iColumn].FindElement(By.XPath(".//mat-checkbox//input[@type='checkbox']")), value);
        }
    }

    public class AnchorControlType : ControlType
    {
        public override void Choose(ReadOnlyCollection<IWebElement> cells, int iColumn, string value = "true")
        {
            cells[iColumn].FindElement(By.XPath(".//a")).Click();
        }
    }

    public class RowControlType : ControlType
    {
        public override void Choose(ReadOnlyCollection<IWebElement> cells, int iColumn, string value = "true")
        {
            cells[iColumn].FindElement(By.XPath(".//parent::tr")).Click();
        }
    }
    #endregion

    internal class Gyupo9GridType : GridType
    {
        public class NoneSelectionType : SelectionType
        {
            public override void Select(IEnumerable<IWebElement> table, int iRow)
            {

            }
        }

        public class NameSelectionType : SelectionType
        {
            public override void Select(IEnumerable<IWebElement> table, int iRow)
            {
                IWebElement row = table.ElementAt(iRow - 1);
                ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));
                AnchorControlType anchor = new AnchorControlType();
                anchor.Choose(cells, 0);
            }
        }

        public new bool GridSearch(IWebDriver driver, Dictionary<string, By> Locators, SelectionType oSelectType, List<string> criteria, bool blnAllTrue)
        {
            return base.GridSearch(driver, Locators, oSelectType, criteria, blnAllTrue);
        }

        public new int findRow(IEnumerable<IWebElement> tableRows, List<string> criteria, bool blnAllTrue = true, bool blnExactMatch = false)
        {
            return base.findRow(tableRows, criteria, blnAllTrue, blnExactMatch);
        }

        public new void RowSelect(ReadOnlyCollection<IWebElement> tableRows, SelectionType oSelectType, int iRowFound)
        {
            base.RowSelect(tableRows, oSelectType, iRowFound);
        }
    }

    internal class MdbootstrapGridType : GridType
    {
        public class NoneSelectionType : SelectionType
        {
            public override void Select(IEnumerable<IWebElement> table, int iRow)
            {

            }
        }

        public class NameSelectionType : SelectionType
        {
            public override void Select(IEnumerable<IWebElement> table, int iRow)
            {
                IWebElement row = table.ElementAt(iRow - 1);
                ReadOnlyCollection<IWebElement> cells = row.FindElements(By.TagName("td"));
                AnchorControlType anchor = new AnchorControlType();              
                anchor.Choose(cells, 0);
            }
        }

        public new bool GridSearch(IWebDriver driver, Dictionary<string, By> Locators, SelectionType oSelectType, List<string> criteria, bool blnAllTrue)
        {
            return base.GridSearch(driver, Locators, oSelectType, criteria, blnAllTrue);
        }

        public new int findRow(IEnumerable<IWebElement> tableRows, List<string> criteria, bool blnAllTrue = true, bool blnExactMatch = false)
        {
            return base.findRow(tableRows, criteria, blnAllTrue, blnExactMatch);
        }

        public new void RowSelect(ReadOnlyCollection<IWebElement> tableRows, SelectionType oSelectType, int iRowFound)
        {
            base.RowSelect(tableRows, oSelectType, iRowFound);
        }
    }
}
