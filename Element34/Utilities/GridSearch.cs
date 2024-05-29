using Element34.ExtensionClasses;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;

namespace Element34.Utilities
{
    /// <summary>
    /// Abstract class representing a control type.
    /// </summary>
    public abstract class ControlType
    {
        /// <summary>
        /// Abstract method to choose a value in a specified column of a table.
        /// </summary>
        /// <param name="cells">The table cells.</param>
        /// <param name="iColumn">The column index.</param>
        /// <param name="value">The value to choose.</param>
        public abstract void Choose(ReadOnlyCollection<IWebElement> cells, int iColumn, string value);
    }

    /// <summary>
    /// Interface representing a selection type.
    /// </summary>
    public interface ISelectionType
    {
        /// <summary>
        /// Selects a row in a table.
        /// </summary>
        /// <param name="table">The table elements.</param>
        /// <param name="iRow">The row index.</param>
        void Select(IEnumerable<IWebElement> table, int iRow);
    }

    /// <summary>
    /// Abstract class representing a selection type.
    /// </summary>
    public abstract class SelectionType : ISelectionType
    {
        /// <summary>
        /// Selects a row in a table.
        /// </summary>
        /// <param name="table">The table elements.</param>
        /// <param name="iRow">The row index.</param>
        public abstract void Select(IEnumerable<IWebElement> table, int iRow);
    }

    /// <summary>
    /// Interface representing a grid type.
    /// </summary>
    public interface IGridType
    {
        /// <summary>
        /// Searches a grid for a row that matches the specified criteria.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="Locators">Dictionary containing locators for important HTML items.</param>
        /// <param name="oSelectType">A derived selection type.</param>
        /// <param name="criteria">Criteria to find in a table row.</param>
        /// <param name="blnAllTrue">All criteria must match if true, any one of criteria can match if false.</param>
        /// <returns>True if a matching row is found; otherwise, false.</returns>
        bool GridSearch(IWebDriver driver, Dictionary<string, By> Locators, ISelectionType oSelectType, List<string> criteria, bool blnAllTrue);

        /// <summary>
        /// Finds a row in a table that matches the specified criteria.
        /// </summary>
        /// <param name="tableRows">The table rows.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <param name="blnAllTrue">All criteria must match if true, any one of criteria can match if false.</param>
        /// <param name="blnExactMatch">Text comparison method (Equals if true, Contains if false).</param>
        /// <returns>The index of the matching row; 0 if no match is found.</returns>
        int FindRow(IEnumerable<IWebElement> tableRows, List<string> criteria, bool blnAllTrue = true, bool blnExactMatch = false);

        /// <summary>
        /// Selects a row in the table.
        /// </summary>
        /// <param name="tableRows">The table rows.</param>
        /// <param name="oSelectType">A derived selection type.</param>
        /// <param name="iRowFound">The index of the row to select.</param>
        void RowSelect(ReadOnlyCollection<IWebElement> tableRows, ISelectionType oSelectType, int iRowFound);
    }

    /// <summary>
    /// Abstract class representing a grid type.
    /// </summary>
    public abstract class GridType : IGridType
    {
        protected static readonly int _defaultTimeSpan = 1;
        protected static readonly int _timeDelay = 1500;

        /// <summary>
        /// Searches a grid for a row that matches the specified criteria.
        /// </summary>
        /// <param name="driver">The WebDriver instance.</param>
        /// <param name="Locators">Dictionary containing locators for important HTML items.</param>
        /// <param name="oSelectType">A derived selection type.</param>
        /// <param name="criteria">Criteria to find in a table row.</param>
        /// <param name="blnAllTrue">All criteria must match if true, any one of criteria can match if false.</param>
        /// <returns>True if a matching row is found; otherwise, false.</returns>
        public bool GridSearch(IWebDriver driver, Dictionary<string, By> Locators, ISelectionType oSelectType, List<string> criteria, bool blnAllTrue)
        {
            int iRowFound = 0;
            bool blnKeepSearching = true;
            IWebElement btnNext, btnPrevious, gridContainer;

            while (blnKeepSearching)
            {
                WaitForBusyIndicator(driver, Locators["busySpinnerLocator"]);
                gridContainer = driver.FindElement(Locators["gridContainerLocator"]);

                if (gridContainer == null)
                    break;

                driver.ScrollToElement(gridContainer);
                driver.wait_A_Moment(_timeDelay / 2);

                ReadOnlyCollection<IWebElement> tableRows = gridContainer.FindElements(Locators["tableRowsLocator"]);
                if (TableHasNoRecords(tableRows))
                    return false;

                btnNext = TryFindElement(gridContainer, Locators["nextButtonLocator"]);
                btnPrevious = TryFindElement(gridContainer, Locators["previousButtonLocator"]);

                bool blnNextDisabled = IsButtonDisabled(btnNext);
                bool blnPrevDisabled = IsButtonDisabled(btnPrevious);

                if (blnNextDisabled && blnPrevDisabled)
                {
                    iRowFound = FindRow(tableRows, criteria, blnAllTrue);
                    if (iRowFound > 0)
                        oSelectType.Select(tableRows, iRowFound);

                    blnKeepSearching = false;
                }
                else if (blnPrevDisabled)
                {
                    iRowFound = FindRow(tableRows, criteria, blnAllTrue);
                    if (iRowFound > 0)
                    {
                        oSelectType.Select(tableRows, iRowFound);
                        break;
                    }

                    btnNext?.Click();
                }
                else if (blnNextDisabled)
                {
                    iRowFound = FindRow(tableRows, criteria, blnAllTrue);
                    if (iRowFound > 0)
                        oSelectType.Select(tableRows, iRowFound);

                    blnKeepSearching = false;
                }
                else
                {
                    iRowFound = FindRow(tableRows, criteria, blnAllTrue);
                    if (iRowFound > 0)
                    {
                        oSelectType.Select(tableRows, iRowFound);
                        break;
                    }

                    btnNext?.Click();
                }
            }

            return iRowFound > 0;
        }

        private static void WaitForBusyIndicator(IWebDriver driver, By busySpinnerLocator)
        {
            driver.PauseOnBusyIndicator(busySpinnerLocator, TimeSpan.FromSeconds(_defaultTimeSpan));
        }

        private static bool TableHasNoRecords(ReadOnlyCollection<IWebElement> tableRows)
        {
            return tableRows.Any(row => row.Text.ToLower().Contains("no records"));
        }

        private static IWebElement TryFindElement(IWebElement container, By locator)
        {
            try
            {
                return container.FindElement(locator);
            }
            catch
            {
                return null;
            }
        }

        private static bool IsButtonDisabled(IWebElement button)
        {
            return button == null || Convert.ToBoolean(button.GetAttribute("disabled"));
        }

        /// <summary>
        /// Finds a row in a table that matches the specified criteria.
        /// </summary>
        /// <param name="tableRows">The table rows.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <param name="blnAllTrue">All criteria must match if true, any one of criteria can match if false.</param>
        /// <param name="blnExactMatch">Text comparison method (Equals if true, Contains if false).</param>
        /// <returns>The index of the matching row; 0 if no match is found.</returns>
        public int FindRow(IEnumerable<IWebElement> tableRows, List<string> criteria, bool blnAllTrue = true, bool blnExactMatch = false)
        {
            string[] normalizedCriteria = criteria.Where(c => !string.IsNullOrEmpty(c)).Select(c => c.Trim()).ToArray();
            if (normalizedCriteria.Length == 0)
                throw new ArgumentException("No criteria provided", nameof(criteria));

            IWebDriver driver = ((IWrapsDriver)tableRows.FirstOrDefault()).WrappedDriver;

            for (int iRow = 0; iRow < tableRows.Count(); iRow++)
            {
                if (TryGetRowCells(driver, tableRows, iRow, out string[] cellContents))
                {
                    if (DoesRowMatchCriteria(normalizedCriteria, cellContents, blnAllTrue, blnExactMatch))
                        return iRow + 1;
                }
            }

            return 0;
        }

        private static bool TryGetRowCells(IWebDriver driver, IEnumerable<IWebElement> tableRows, int rowIndex, out string[] cellContents)
        {
            IWebElement row = null;
            IEnumerable<IWebElement> rowCells = null;

            bool success = driver.PerformActionWithRetry(TimeSpan.FromSeconds(60), 15, d =>
            {
                try
                {
                    row = tableRows.ElementAt(rowIndex);
                    rowCells = row.FindElements(By.TagName("td"));
                    return row != null && rowCells != null;
                }
                catch
                {
                    return false;
                }
            });

            cellContents = success ? rowCells.Select(cell => DecodeAndTrim(cell.Text)).ToArray() : null;
            return success;
        }

        private static bool DoesRowMatchCriteria(string[] criteria, string[] cellContents, bool allCriteriaMustMatch, bool exactMatch)
        {
            foreach (string criterion in criteria)
            {
                bool isMatch = cellContents.Any(cellContent =>
                    exactMatch ? string.Equals(criterion, cellContent, StringComparison.InvariantCultureIgnoreCase)
                               : cellContent.IndexOf(criterion, StringComparison.InvariantCultureIgnoreCase) >= 0);

                if (isMatch && !allCriteriaMustMatch)
                    return true;

                if (!isMatch && allCriteriaMustMatch)
                    return false;
            }

            return allCriteriaMustMatch;
        }

        /// <summary>
        /// Selects a row in the table.
        /// </summary>
        /// <param name="tableRows">The table rows.</param>
        /// <param name="oSelectType">A derived selection type.</param>
        /// <param name="iRowFound">The index of the row to select.</param>
        public void RowSelect(ReadOnlyCollection<IWebElement> tableRows, ISelectionType oSelectType, int iRowFound)
        {
            if (iRowFound > 0)
            {
                oSelectType.Select(tableRows, iRowFound);
            }
        }

        /// <summary>
        /// Decodes an HTML-encoded string and trims excess whitespace.
        /// </summary>
        /// <param name="sInput">The input string.</param>
        /// <param name="chNormalizeTo">Whitespace replacement character.</param>
        /// <returns>The decoded and trimmed string.</returns>
        private static string DecodeAndTrim(string sInput, char chNormalizeTo = ' ')
        {
            if (string.IsNullOrWhiteSpace(sInput))
            {
                return string.Empty;
            }

            if (sInput.IndexOf('&') != -1)
            {
                sInput = HttpUtility.HtmlDecode(sInput);
            }

            StringBuilder sbOutput = new StringBuilder(sInput.Length);
            bool previousWasWhiteSpace = false;
            bool hasSeenNonWhiteSpace = false;

            foreach (char c in sInput)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!hasSeenNonWhiteSpace)
                    {
                        continue;
                    }
                    if (!previousWasWhiteSpace)
                    {
                        sbOutput.Append(chNormalizeTo);
                        previousWasWhiteSpace = true;
                    }
                }
                else
                {
                    previousWasWhiteSpace = false;
                    hasSeenNonWhiteSpace = true;
                    sbOutput.Append(c);
                }
            }

            if (sbOutput.Length == 0)
            {
                return string.Empty;
            }

            return sbOutput.ToString().Trim();
        }
    }
}
