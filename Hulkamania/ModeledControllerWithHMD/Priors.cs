using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Brandeis.AGSOL.Hulkamania.Tasks.ModeledControllerWithHmd
{
    internal class Priors
    {
        #region Constants

        private static readonly LogHandler logger = new LogHandler(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion

        #region Fields

        public static double[] positionBins = null;
        public static double[] velocityBins = null;
        public static double[] deflectionBins = null;

        public static double positionBinSize = 0;
        public static double velocityBinSize = 0;
        public static double deflectionBinSize = 0;

        public static double[, ,] positionByVelocityPriors = null;
        public static double[,] positionPriors = null;
        public static double[,] velocityPriors = null;
        
        #endregion

        #region Reading priors
        /// <summary>
        /// Reads a 1D matrix from a CSV file. The CSV file should have one row, and an arbitrary number of columns
        /// </summary>
        /// <param name="filename">The filename of the csv file</param>
        /// <returns>An array containing the values</returns>
        private static double[] ReadMatrix1D(string filename)
        {
            double[] retval = null;
            StreamReader reader = null;
            String line = "";

            logger.Info("Reading 1D matrix from file " + filename);

            try
            {
                reader = new StreamReader(filename);
            }
            catch (Exception)
            {
                throw new IOException("Error while opening the file " + filename);
            }

            if ((line = reader.ReadLine()) != null)
            {
                // Ignore comment lines
                if (!line.StartsWith(";"))
                {
                    string[] values = line.Split(',');

                    if(values.Length<2)
                    {
                        throw new IOException("Error while parsing the file " + filename + " - must have two or more columns!");
                    }

                    retval = new double[values.Length];
                    
                    for (int i = 0; i < values.Length; i++)
                    {
                        retval[i] = double.Parse(values[i].ToLower());
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Reads a 2D matrix from a CSV file. The CSV file can have more than one row, and an arbitrary number of columns
        /// </summary>
        /// <param name="filename">The filename of the csv file</param>
        /// <returns>A 2D array containing the values</returns>
        private static double[,] ReadMatrix2D(string filename, int expectedRowCount, int expectedColumnCount)
        {
            double[,] retval = null;
            StreamReader reader = null;
            String line = "";

            logger.Info("Reading 2D matrix from file " + filename);

            try
            {
                reader = new StreamReader(filename);
            }
            catch (Exception)
            {
                throw new IOException("Error while opening the file " + filename);
            }

            int nCol = 0;
            int nRow = 0;
            bool isFirstLine = true;

            // figure out number of rows/columns
            while ((line = reader.ReadLine()) != null)
            {
                if (!line.StartsWith(";"))
                {
                    string[] values = line.Split(',');
                    if (isFirstLine)
                    {
                        isFirstLine = false;
                        nCol = values.Length;

                        if (nCol != expectedColumnCount)
                        {
                            throw new IOException("Error while opening the file " + filename + " - number of columns is not what was expected");
                        }
                    }

                    if (nCol != values.Length)
                    {
                        throw new IOException("Error while opening the file " + filename + " - inconsistent number of columns");
                    }

                    nRow = nRow + 1;
                }
            }

            if (nRow != expectedRowCount)
            {
                throw new IOException("Error while opening the file " + filename + " - number of rows is not what was expected");
            }

            // create the array
            retval = new double[nRow, nCol];
            reader.Close();
            
            // reopen the file, this time read all the data
            try
            {
                reader = new StreamReader(filename);
            }
            catch (Exception)
            {
                throw new IOException("Error while opening the file " + filename);
            }

            nRow = 0;
            while ((line = reader.ReadLine()) != null)
            {
                // Ignore comment lines
                if (!line.StartsWith(";"))
                {
                    string[] values = line.Split(',');

                    for (int i = 0; i < values.Length; i++)
                    {
                        retval[nRow, i] = double.Parse(values[i].ToLower());
                    }

                    nRow = nRow + 1;
                }
            }

            return retval;
        }


        /// <summary>
        /// Read position by velocity priors. Note that the position, velocity, and deflection bins must have been read prior to calling this method!
        /// </summary>
        private static void ReadPositionByVelocityPriors()
        {
            if ((positionBins == null) ||
                (velocityBins == null) ||
                (deflectionBins == null))
            {
                return;
            }

            positionByVelocityPriors = new double[velocityBins.Length, positionBins.Length, deflectionBins.Length];

            for (int i = 0; i < deflectionBins.Length; i++)
            {
                string fileName = Environment.CurrentDirectory + "\\Priors\\positionByVelocityPriors_deflectionBin" + (i+1).ToString() + ".csv";
                double[,] priorsForDeflection = ReadMatrix2D(fileName, velocityBins.Length, positionBins.Length);
                for (int x = 0; x < velocityBins.Length; x++)
                {
                    for (int y = 0; y < positionBins.Length; y++)
                    {
                        positionByVelocityPriors[x, y, i] = priorsForDeflection[x, y];
                    }
                }
            }
        }

        #endregion
        
        #region Querying deflection PDFs
        /// <summary>
        /// returns true if a prior was found for the given position
        /// </summary>
        /// <param name="position">the position to query</param>
        /// <returns>true if a prior was found</returns>
        public static bool hasPriorForPosition(double position)
        {
            double[] pdf = getDeflectionPDFForPosition(position);
            foreach (double d in pdf)
            {
                if (Math.Abs(d) > 0.001)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// returns true if a prior was found for the given velocity
        /// </summary>
        /// <param name="velocity">the velocity to query</param>
        /// <returns>true if a prior was found</returns>
        public static bool hasPriorForVelocity(double velocity)
        {
            double[] pdf = getDeflectionPDFForVelocity(velocity);
            foreach (double d in pdf)
            {
                if (Math.Abs(d) > 0.001)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// returns true if a prior was found for the given position and velocity
        /// </summary>
        /// <param name="position">the position to query</param>
        /// <returns>true if a prior was found</returns>
        public static bool hasPriorForPositionAndVelocity(double position, double velocity)
        {
            double[] pdf = getDeflectionPDFForPositionAndVelocity(position, velocity);
            foreach (double d in pdf)
            {
                if (Math.Abs(d) > 0.001)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// returns the probability of observing each possible deflection, given the specified position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static double[] getDeflectionPDFForPosition(double position)
        {
            double[] retval = null;

            int binIndex = -1;
            for (int i = 0; i < positionBins.Length; i++)
            {
                double binCenter = positionBins[i];
                if( ( (binCenter-positionBinSize/2) <= position) &&
                    ((binCenter + positionBinSize / 2) > position))
                {
                    binIndex = i;
                    break;
                }
            }

            if (binIndex == -1)
            {
                if (position < positionBins[0])
                {
                    binIndex = 0;
                }
                if (position > positionBins[positionBins.Length - 1])
                {
                    binIndex = positionBins.Length - 1;
                }
            }
            

            retval = new double[deflectionBins.Length];
            for (int i = 0; i < deflectionBins.Length; i++)
            {
                retval[i] = positionPriors[binIndex, i];
            }

            return retval;
        }


        /// <summary>
        /// returns the probability of observing each possible deflection, given the specified velocity
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static double[] getDeflectionPDFForVelocity(double velocity)
        {
            double[] retval = null;

            int binIndex = -1;
            for (int i = 0; i < velocityBins.Length; i++)
            {
                double binCenter = velocityBins[i];
                if (((binCenter - velocityBinSize / 2) <= velocity) &&
                    ((binCenter + velocityBinSize / 2) > velocity))
                {
                    binIndex = i;
                    break;
                }
            }

            if (binIndex == -1)
            {
                if (velocity < velocityBins[0])
                {
                    binIndex = 0;
                }
                if (velocity > velocityBins[velocityBins.Length-1])
                {
                    binIndex = velocityBins.Length - 1;
                }
            }

            retval = new double[deflectionBins.Length];
            for (int i = 0; i < deflectionBins.Length; i++)
            {
                retval[i] = velocityPriors[binIndex, i];
            }

            return retval;
        }

        /// <summary>
        /// returns the probability of observing each possible deflection, given the specified velocity and position
        /// </summary>
        /// <param name="position"></param>
        /// <param name="velocity"></param>
        /// <returns></returns>
        public static double[] getDeflectionPDFForPositionAndVelocity(double position, double velocity)
        {
            double[] retval = null;

            // determine bin index for position
            int binIndexP = -1;
            for (int i = 0; i < positionBins.Length; i++)
            {
                double binCenter = positionBins[i];
                if (((binCenter - positionBinSize / 2) <= position) &&
                    ((binCenter + positionBinSize / 2) > position))
                {
                    binIndexP = i;
                    break;
                }
            }

            if (binIndexP == -1)
            {
                if (position < positionBins[0])
                {
                    binIndexP = 0;
                }
                if (position > positionBins[positionBins.Length - 1])
                {
                    binIndexP = positionBins.Length - 1;
                }
            }


            // determine bin for velocity
            int binIndexV = -1;
            for (int i = 0; i < velocityBins.Length; i++)
            {
                double binCenter = velocityBins[i];
                if (((binCenter - velocityBinSize / 2) <= velocity) &&
                    ((binCenter + velocityBinSize / 2) > velocity))
                {
                    binIndexV = i;
                    break;
                }
            }

            if (binIndexV == -1)
            {
                if (velocity < velocityBins[0])
                {
                    binIndexV = 0;
                }
                if (velocity > velocityBins[velocityBins.Length - 1])
                {
                    binIndexV = velocityBins.Length - 1;
                }
            }
            
            retval = new double[deflectionBins.Length];
            for (int i = 0; i < deflectionBins.Length; i++)
            {
                retval[i] = positionByVelocityPriors[binIndexV, binIndexP, i];
            }

            return retval;
        }

        #endregion


        /// <summary>
        /// Read all bins and priors from the csv files in the folder .\Priors
        /// </summary>
        /// <returns></returns>
        public static bool Read()
        {
            positionBins = ReadMatrix1D(Environment.CurrentDirectory + "\\Priors\\positionBins.csv");
            velocityBins = ReadMatrix1D(Environment.CurrentDirectory + "\\Priors\\velocityBins.csv");
            deflectionBins = ReadMatrix1D(Environment.CurrentDirectory + "\\Priors\\deflectionBins.csv");

            positionBinSize = positionBins[1] - positionBins[0];
            velocityBinSize = velocityBins[1] - velocityBins[0];
            deflectionBinSize = deflectionBins[1] - deflectionBins[0];

            positionPriors = ReadMatrix2D(Environment.CurrentDirectory + "\\Priors\\positionPriors.csv", positionBins.Length, deflectionBins.Length);
            velocityPriors = ReadMatrix2D(Environment.CurrentDirectory + "\\Priors\\velocityPriors.csv", velocityBins.Length, deflectionBins.Length);

            ReadPositionByVelocityPriors();

            bool readSuccessful =   (positionBins!=null) &&
                                    (deflectionBins!=null) &&
                                    (velocityBins!=null) &&
                                    (positionPriors!=null) &&
                                    (velocityPriors!=null) &&
                                    (positionByVelocityPriors!=null);
                                    
            return readSuccessful;
        }


    }
}
