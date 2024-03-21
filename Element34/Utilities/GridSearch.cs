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
    // Reorganization of this class suggested by u/Slypenslyde of Reddit.
    public abstract class ControlType
    {
        public abstract void Choose(ReadOnlyCollection<IWebElement> cells, int iColumn, string value);
    }

    public interface ISelectionType
    {
        void Select(IEnumerable<IWebElement> table, int iRow);
    }

    public abstract class SelectionType : ISelectionType
    {
        public abstract void Select(IEnumerable<IWebElement> table, int iRow);
    }

    public interface IGridType
    {
        bool GridSearch(IWebDriver driver, Dictionary<string, By> Locators, ISelectionType oSelectType, List<string> criteria, bool blnAllTrue);
        int findRow(IEnumerable<IWebElement> tableRows, List<string> criteria, bool blnAllTrue = true, bool blnExactMatch = false);
        void RowSelect(ReadOnlyCollection<IWebElement> tableRows, ISelectionType oSelectType, int iRowFound);
    }

    public abstract class GridType : IGridType
    {
        protected static readonly int _defaultTimeSpan = 1;
        protected static readonly int _timeDelay = 1500;

        ///<summary>
        ///Grid Search:
        ///   1) Find and select a specific row in a table of search results given search criteria.
        ///   2) This method will automatically advance through paginated results until the end is reached.
        ///</summary>
        ///<param name="Locators">Dictionary containing locators for important HTML items</param>
        ///<param name="oSelectType">A derived selection type.</param>
        ///<param name="criteria">Criteria to find in a table row</param>
        ///<param name="blnAllTrue">all criteria must match if true, any one of criteria can match if false</param>
        public bool GridSearch(IWebDriver driver, Dictionary<string, By> Locators, ISelectionType oSelectType, List<string> criteria, bool blnAllTrue)
        {
            int iRowFound = 0;
            bool blnKeepSearching = true;
            bool blnNextDisabled, blnPrevDisabled;
            IWebElement btnNext, btnPrevious;
            IWebElement gridContainer;

            //find row
            while (blnKeepSearching)
            {
                // Wait for busy indicator
                driver.PauseOnBusyIndicator(Locators["busySpinnerLocator"], TimeSpan.FromSeconds(_defaultTimeSpan));
                gridContainer = driver.FindElement(Locators["gridContainerLocator"]);

                // No gridContainer; bail!
                if (gridContainer == null)
                    break;

                // Scroll to gridContainer
                driver.ScrollToElement(gridContainer);
                driver.wait_A_Moment(_timeDelay / 2);

                // Find table within gridContainer
                ReadOnlyCollection<IWebElement> tableRows = gridContainer.FindElements(Locators["tableRowsLocator"]);

                // No results; bail!
                foreach (var row in tableRows)
                {
                    if (row.Text.ToLower().Contains("no records"))
                        return false;
                }

                // Find Next and Previous buttons
                try { btnNext = gridContainer.FindElement(Locators["nextButtonLocator"]); } catch { btnNext = null; }
                try { btnPrevious = gridContainer.FindElement(Locators["previousButtonLocator"]); } catch { btnPrevious = null; }

                // Ascertain state of Next and Previous buttons
                blnNextDisabled = (btnNext == null) || Convert.ToBoolean(btnNext.GetAttribute("disabled"));
                blnPrevDisabled = (btnPrevious == null) || Convert.ToBoolean(btnPrevious.GetAttribute("disabled"));

                // Page Navigation
                if (blnNextDisabled && blnPrevDisabled)  //one page
                {
                    iRowFound = findRow(tableRows, criteria, blnAllTrue);
                    if (iRowFound > 0)
                    {
                        oSelectType.Select(tableRows, iRowFound);
                    }

                    blnKeepSearching = false;
                }
                else if (blnPrevDisabled) //first of multi page
                {
                    iRowFound = findRow(tableRows, criteria, blnAllTrue);
                    if (iRowFound > 0)
                    {
                        oSelectType.Select(tableRows, iRowFound);
                        break;
                    }

                    if (!blnNextDisabled)
                        btnNext.Click();
                }
                else if (blnNextDisabled) // last page (end of search)
                {
                    iRowFound = findRow(tableRows, criteria, blnAllTrue);
                    if (iRowFound > 0)
                    {
                        oSelectType.Select(tableRows, iRowFound);
                    }

                    blnKeepSearching = false;
                }
                else //next pages
                {
                    iRowFound = findRow(tableRows, criteria, blnAllTrue);
                    if (iRowFound > 0)
                    {
                        oSelectType.Select(tableRows, iRowFound);
                        break;
                    }

                    if (!blnNextDisabled)
                        btnNext.Click();
                }
            }

            return (iRowFound > 0);
        }

        ///<summary>
        /// findRow(): Support function for all Grid Search functions.
        /// Returns the index of the first row that matches given criteria (0 is returned if not found).
        /// Subtract 1 to use in zero-based array.
        /// Algorithm improved by u/vidaj from Reddit.
        ///</summary>
        ///<param name="tableRows">Enumerated table rows</param>
        ///<param name="criteria">Criteria to find in a table row</param>
        ///<param name="blnAllTrue">all criteria must match if true, any one of criteria can match if false</param>
        ///<param name="blnExactMatch">text comparison method (Equals if true, Contains if false)</param>
        public int findRow(IEnumerable<IWebElement> tableRows, List<string> criteria, bool blnAllTrue = true, bool blnExactMatch = false)
        {
            // Avoid doing a .Trim() on each criteria for each row and column.
            var normalizedCriteria = criteria.Where(c => !string.IsNullOrEmpty(c)).Select(c => c.Trim()).ToArray();
            if (normalizedCriteria.Length == 0)
                throw new ArgumentException("no criteria", nameof(criteria));

            IWebDriver driver = ((IWrapsDriver)tableRows.FirstOrDefault()).WrappedDriver;

            for (int iRow = 0, rowLength = tableRows.Count(); iRow < rowLength; iRow++)
            {
                IWebElement row = null;
                IEnumerable<IWebElement> rowCells = null;
                string[] cellContents = null;

                // Common place for StaleElementReferenceException
                driver.PerformActionWithRetry(TimeSpan.FromSeconds(45), 15,
                    d =>
                    {
                        try
                        {
                            row = tableRows.ElementAt(iRow);
                            rowCells = row.FindElements(By.TagName("td"));
                            return (row != null) && (rowCells != null);
                        }
                        catch
                        {
                            return false;
                        }
                    });

                // This can cause a slowdown for tables with lots of columns where the criteria matches early columns.
                // If that's the case, one can create an array of strings with null-values and initialize each cell on
                // first read if cellContents[cellColumn] == null
                if (rowCells != null)
                    cellContents = rowCells.Select(cell => DecodeAndTrim(cell.Text)).ToArray();
                else
                    return 0;


                bool isMatch = false;
                foreach (string criterion in normalizedCriteria)
                {
                    foreach (string cellContent in cellContents)
                    {
                        // string.Contains(string, StringComparison) is not available for .Net Framework.
                        // If you're using .Net Framework, substitute by "cellContent.IndexOf(criterion, StringComparison.InvariantCultureIgnoreCase) >= 0
                        isMatch = (blnExactMatch && string.Equals(criterion, cellContent, StringComparison.InvariantCultureIgnoreCase)) || cellContent.IndexOf(criterion, StringComparison.InvariantCultureIgnoreCase) >= 0;

                        if (isMatch)
                        {
                            if (!blnAllTrue)
                            {
                                return iRow + 1;
                            }
                            break;
                        }
                    }

                    if (blnAllTrue && !isMatch)
                    {
                        break;
                    }
                }

                if (isMatch)
                {
                    return iRow + 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Exposes the Select method from the SelectionType.
        /// </summary>
        /// <param name="tableRows">ReadOnlyCollection of HTML "tr" objects as IWebElements.</param>
        /// <param name="oSelectType">A selection type derived from the SelectionType abstract class.</param>
        /// <param name="iRowFound">Index of the table row.</param>
        public void RowSelect(ReadOnlyCollection<IWebElement> tableRows, ISelectionType oSelectType, int iRowFound)
        {
            if (iRowFound > 0)
            {
                oSelectType.Select(tableRows, iRowFound);
            }
        }

        ///<summary>
        /// DecodeAndTrim:
        ///   1) Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
        ///   2) Replace any sequence of whitespaces by a single one.
        ///   3) Remove any leading or trailing whitespaces.
        ///   Function improved by u/vidaj from Reddit.
        ///</summary>
        ///<param name="sInput">Input string</param>
        ///<param name="chNormalizeTo">Whitespace replacement char</param>
        private static string DecodeAndTrim(string sInput, char chNormalizeTo = ' ')
        {
            // If blank, just carry on...
            if (string.IsNullOrWhiteSpace(sInput))
            {
                return string.Empty;
            }

            // Don't allocate a new string if there is nothing to decode
            if (sInput.IndexOf('&') != -1)
            {
                sInput = HttpUtility.HtmlDecode(sInput);
            }

            // Pre-initialize the stringbuilder with the previous string's length.
            // This will over-allocate by the number of extra whitespace,
            // but will avoid new allocations every time the stringbuilder runs out of storage space.
            StringBuilder sbOutput = new StringBuilder(sInput.Length);
            bool blnPreviousWasWhiteSpace = false;
            bool blnHasSeenNonWhiteSpace = false;
            foreach (char c in sInput)
            {
                if (char.IsWhiteSpace(c))
                {
                    // Trims the start of the string
                    if (!blnHasSeenNonWhiteSpace)
                    {
                        continue;
                    }
                    if (!blnPreviousWasWhiteSpace)
                    {
                        sbOutput.Append(chNormalizeTo);
                        blnPreviousWasWhiteSpace = true;
                    }
                }
                else
                {
                    blnPreviousWasWhiteSpace = false;
                    blnHasSeenNonWhiteSpace = true;
                    sbOutput.Append(c);
                }
            }

            if (sbOutput.Length == 0)
            {
                return string.Empty;
            }

            // remove trailing whitespaces
            int i = sbOutput.Length - 1;
            for (; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(sbOutput[i]))
                    break;
            }
            if (i < sbOutput.Length - 1) sbOutput.Length = i + 1;

            // trim leading whitespaces
            i = 0;
            for (; i <= (sbOutput.Length - 1); i++)
            {
                if (!char.IsWhiteSpace(sbOutput[i]))
                    break;
            }
            if (i > 0) sbOutput.Remove(sbOutput.Length - i, i);

            return sbOutput.ToString();
        }
    }
}
