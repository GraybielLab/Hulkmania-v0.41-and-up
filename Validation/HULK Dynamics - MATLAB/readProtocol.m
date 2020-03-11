function [ protocol propertyIndices ] = readProtocol( fileName )
%READPROTOCOL Returns a struct containing the protocol 
%   
%   propertyIndices     =   A struct with values that are used to index 
%   trial data within protocol structs. E.g. to get matrix coefficient 11 
%   for the 23rd trial in the 1st protocol that was read
%   type: protocols(1).data(23,propertyIndices.matrix11)

    propertyIndices.indexTrialNumber = 1;
    propertyIndices.indexInstructionType = 2;
    propertyIndices.indexJoystickGain = 3;
    propertyIndices.indexAccelConstant = 4;
    propertyIndices.indexMaxAcceleration = 5;
    propertyIndices.indexMaxVelocity = 6;
    propertyIndices.indexMaxAngle = 7;
    propertyIndices.indexRestartWhenMaxAngle = 8;
    propertyIndices.indexTimeLimit = 9;
    propertyIndices.indexUseRoll = 10;
    propertyIndices.indexUsePitch= 11;
    propertyIndices.indexUseYaw= 12;
    propertyIndices.indexDOBRoll = 13;
    propertyIndices.indexDOBPitch= 14;
    propertyIndices.indexDOBYaw= 15;
    propertyIndices.indexInitialAction = 16;
    propertyIndices.indexStartRoll = 17;
    propertyIndices.indexStartPitch  = 18;
    propertyIndices.indexStartYaw  = 19;
    propertyIndices.indexVisualRoll= 20;
    propertyIndices.indexVisualPitch = 21;
    propertyIndices.indexVisualYaw = 22;
    propertyIndices.indexMoveSound = 23;
    propertyIndices.indexTrialStartSound = 24;
    propertyIndices.indexTrialEndSound = 25;
    propertyIndices.indexResetStartSound = 26;
    propertyIndices.indexResetEndSound= 27;
    propertyIndices.indexAnalyze = 28;

%    disp(['Reading protocol: ' fileName]);

    [protocol, result] = readtext( fileName, ',', ';', '"');
    
end

