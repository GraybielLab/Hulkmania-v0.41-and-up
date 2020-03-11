using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania.Tasks.ModeledControllerWithHmd
{
    class CsvReader
    {
        private Dictionary<string, List<string>> mCsvColumns = new Dictionary<string, List<string>>();
        private int mNumLinesRead = 0;
        private string mFileName = "";

        // ---------------------------------------------------------------------------------------------------------
        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        // ---------------------------------------------------------------------------------------------------------
        public int NumLinesRead { get { return mNumLinesRead; } }
        public string FileName { get { return mFileName; } }

        // ---------------------------------------------------------------------------------------------------------
        private void _parseColumnHeaders(string text)
        {
            string line = text.Substring(1, text.Length - 1);

            string[] columns = line.Split(',');
            foreach (string str in columns)
            {
                logger.Debug("Found column " + (mCsvColumns.Count + 1) + " named " + str);
                mCsvColumns[str.ToLower()] = new List<string>();
            }
        }


        // ---------------------------------------------------------------------------------------------------------
        public float getValueAsFloat(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                throw new Exception("The csv file does not contain a column named '" + columnName + "'\r\nFile: " + mFileName);
            }

            float retval;
            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                throw new Exception("Error accessing row " + row + " of " + mNumLinesRead + "- not enough rows in csv file\r\nFile: " + mFileName);
            }

            if (!float.TryParse(values[row], out retval))
            {
                throw new Exception("Error converting row values in column '" + columnName + "' to float\r\nFile: " + mFileName);
            }

            return retval;
        }

        // ---------------------------------------------------------------------------------------------------------
        public bool hasValueAsFloat(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                return false;
            }

            float retval;
            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                return false;
            }

            if (!float.TryParse(values[row], out retval))
            {
                return false;
            }

            return true;
        }

        // ---------------------------------------------------------------------------------------------------------
        public double getValueAsDouble(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                throw new Exception("The csv file does not contain a column named '" + columnName + "'\r\nFile: " + mFileName);
            }

            double retval;
            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                throw new Exception("Error accessing row " + row + " of " + mNumLinesRead + "- not enough rows in csv file\r\nFile: " + mFileName);
            }

            if (!double.TryParse(values[row], out retval))
            {
                throw new Exception("Error converting row values in column '" + columnName + "' to double\r\nFile: " + mFileName);
            }

            return retval;
        }

        // ---------------------------------------------------------------------------------------------------------
        public bool hasValueAsDouble(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                return false;
            }

            double retval;
            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                return false;
            }

            if (!double.TryParse(values[row], out retval))
            {
                return false;
            }

            return true;
        }
        // ---------------------------------------------------------------------------------------------------------
        public int getValueAsInt(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                throw new Exception("The csv file does not contain a column named '" + columnName + "'\r\nFile: " + mFileName);
            }

            int retval;
            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                throw new Exception("Error accessing row " + row + " of " + mNumLinesRead + "- not enough rows in csv file\r\nFile: " + mFileName);
            }

            if (!int.TryParse(values[row], out retval))
            {
                throw new Exception("Error converting row values in column '" + columnName + "' to int\r\nFile: " + mFileName);
            }

            return retval;
        }

        // ---------------------------------------------------------------------------------------------------------
        public bool hasValueAsInt(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                return false;
            }

            int retval;
            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                return false;
            }

            if (!int.TryParse(values[row], out retval))
            {
                return false;
            }

            return true;
        }
        // ---------------------------------------------------------------------------------------------------------
        public bool getValueAsBool(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                throw new Exception("The csv file does not contain a column named '" + columnName + "'\r\nFile: " + mFileName);
            }

            bool retval;
            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                throw new Exception("Error accessing row " + row + " of " + mNumLinesRead + "- not enough rows in csv file\r\nFile: " + mFileName);
            }

            if (!bool.TryParse(values[row].ToLower(), out retval))
            {
                throw new Exception("Error converting row values in column '" + columnName + "' to bool\r\nFile: " + mFileName);
            }

            return retval;
        }

        // ---------------------------------------------------------------------------------------------------------
        public bool hasValueAsBool(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                return false;
            }

            bool retval;
            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                return false;
            }

            if (!bool.TryParse(values[row], out retval))
            {
                return false;
            }

            return true;
        }
        // ---------------------------------------------------------------------------------------------------------
        public string getValueAsString(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                throw new Exception("The csv file does not contain a column named '" + columnName + "'\r\nFile: " + mFileName);
            }

            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                throw new Exception("Error accessing row " + row + " of " + mNumLinesRead + "- not enough rows in csv file\r\nFile: " + mFileName);
            }

            return values[row];
        }

        // ---------------------------------------------------------------------------------------------------------
        public bool hasValueAsString(string columnName, int row)
        {
            if (!mCsvColumns.ContainsKey(columnName.ToLower()))
            {
                return false;
            }

            List<string> values = mCsvColumns[columnName.ToLower()];

            if (row >= values.Count)
            {
                return false;
            }

            return true;
        }

        // ---------------------------------------------------------------------------------------------------------
        public int readFile(string fileName)
        {
            StreamReader reader = null;
            String line = "";

            logger.Info("Reading csv file " + fileName);

            mCsvColumns.Clear();

            try
            {
                reader = new StreamReader(fileName);
            }
            catch (Exception)
            {
                throw new IOException("Error while opening the file " + fileName);
            }

            List<string> columnNames = new List<string>();
            int numLinesRead = 0;
            while ((line = reader.ReadLine()) != null)
            {

                // Ignore comment lines
                if (line.StartsWith(";"))
                {
                    logger.Info("Parsing column headers");
                    mCsvColumns.Clear();
                    _parseColumnHeaders(line);
                    foreach (KeyValuePair<string, List<string>> pair in mCsvColumns)
                    {
                        columnNames.Add(pair.Key);
                    }
                    continue;
                }

                string[] values = line.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    mCsvColumns[columnNames[i]].Add(values[i].ToLower());
                }
                numLinesRead += 1;
            }
            mNumLinesRead = numLinesRead;
            mFileName = fileName;
            logger.Info("Finished reading " + numLinesRead + " lines from csv file");
            return numLinesRead;
        }
    }
}
