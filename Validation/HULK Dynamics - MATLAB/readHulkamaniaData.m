function [ data dataIndices] = readHulkamaniaData( dataDir )

%READHULKAMANIADATA Reads data that was recorded with Hulkamania
%   This function returns a struct that describes the data in each column,
%   and a matrix that contains all the data. It is assumed that the first
%   line in the data file contains column names, thus it is skipped.

dataIndices.indexTime = 1;
dataIndices.indexTrialNumber = 2;
dataIndices.indexTrialPhase = 3;
dataIndices.indexDirOfBalanceRoll = 4;
dataIndices.indexDirOfBalancePitch = 5;
dataIndices.indexDirOfBalanceYaw = 6;
dataIndices.indexMovingDOBRoll = 7;
dataIndices.indexMovingDOBPitch = 8;
dataIndices.indexMovingDOBYaw = 9;
dataIndices.indexCurrentPosRoll = 10;
dataIndices.indexCurrentPosPitch = 11;
dataIndices.indexCurrentPosYaw = 12;
dataIndices.indexCurrentVelRoll = 13;
dataIndices.indexCurrentVelPitch = 14;
dataIndices.indexCurrentVelYaw = 15;
dataIndices.indexCalcPosRoll = 16;
dataIndices.indexCalcPosPitch = 17;
dataIndices.indexCalcPosYaw = 18;
dataIndices.indexCalcVelRoll = 19;
dataIndices.indexCalcVelPitch = 20;
dataIndices.indexCalcVelYaw = 21;
dataIndices.indexCalcAccRoll = 22;
dataIndices.indexCalcAccPitch = 23;
dataIndices.indexCalcAccYaw = 24;
dataIndices.indexJoystickX = 25;
dataIndices.indexJoystickY = 26;
dataIndices.indexJoystickButton = 27;

% obtain a list of all files from which the name ends with '.csv'
list_files=[];
list_files=dir([dataDir '*.csv']); 
list_files = sort({list_files.name});
for index=1:length(list_files)
    list_files{index}=[dataDir list_files{index}]; 
    list_files{index};
end
    
disp(['Data folder: ' dataDir]);
disp(['Data file count: ' num2str(length(list_files))]);

% read all protocol files 
data = cell(length(list_files), 2);
for index=1:length(list_files)
   strFile = char(list_files(index));
   disp(['Reading: ' strFile ', index: ' num2str(index)]);

   data(index, 1) = cellstr([strFile]);
%   data(index, 2) = mat2cell(csvread( strFile, 1, 0));
   data(index, 2) = { csvread( strFile, 1, 0) };
end

end

