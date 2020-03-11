clear all;
close all;
clc;

% -------------------------------------------------------------------------
% change the settings below to suit your needs ----------------------------

forcingFunctionFolderFolder = [pwd '\data\forcing functions\'];

freq = '6.4';                   % 0.1 
                                % 0.4 
                                % 1.6 
                                % 6.4
                                
usePeakDetectionHack = false;   % see plotData.m This hack tries to ensure that 
                                % all detected peaks are near the absolute
                                % maximum of the signal, and not at some
                                % local maximum

% change the settings above to suit your needs ----------------------------
% -------------------------------------------------------------------------


% add folders containing helper scripts to search path --------------------
addpath( [pwd '\helpers']); 

% configure data folders
resultsFolder0 = [pwd '\data\results\' freq 'hz_offset0\'];
resultsFolder30 = [pwd '\data\results\' freq 'hz_offset30\'];

% read forcing functions --------------------------------------------------
global frequencies forcingFunctions forcingFunctionIndices;

list_files = dir([forcingFunctionFolderFolder  '\*.csv']); 
forcingFunctions = cell(size(list_files, 1), 1);
frequencies = zeros(size(list_files, 1), 1);

disp(['analyzeDelays: Found ' num2str(size(list_files, 1)) ' forcing function(s) in folder: ' forcingFunctionFolderFolder]);

for index = 1 : size(list_files, 1)
    fileName = list_files(index).name;
    functionFile = [forcingFunctionFolderFolder '\' fileName];
    disp(['analyzeDelays: Reading forcing function: ' num2str(index) ', file: ' fileName]);
    [functionData forcingFunctionIndices] = readForcingFunction(functionFile);
    forcingFunctions{index}.data = functionData;
    forcingFunctions{index}.fileName = fileName;
    
    k = strfind(fileName, '_');
    forcingFunctions{index}.frequency = str2num(fileName(1:(k(1)-1))); 
    frequencies(index) = str2num(fileName(1:(k(1)-1))); 
end

clear functionFile functionData list_files index k fileName;

% read data ---------------------------------------------------------------
[data0 dataIndices] = readHulkamaniaData(resultsFolder0);
[data30 dataIndices] = readHulkamaniaData(resultsFolder30);

% plot data ---------------------------------------------------------------
[outDataAmpl0 outDataMean0 outDataStd0 outDataXCorr0 outDataXCorrLag0] = plotData( data0, dataIndices, 'Offset = 0', usePeakDetectionHack );
[outDataAmpl30 outDataMean30 outDataStd30 outDataXCorr30 outDataXCorrLag30] = plotData( data30, dataIndices, 'Offset = 30', usePeakDetectionHack );

% plot analysis -----------------------------------------------------------
plotAnalysis( outDataAmpl0, outDataMean0, outDataStd0, outDataXCorr0, outDataXCorrLag0, outDataAmpl30, outDataMean30, outDataStd30, outDataXCorr30, outDataXCorrLag30, freq )

disp('Done with analyzing delays!');
  



